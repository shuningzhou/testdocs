using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RollbackController3D : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;
    public Text stepNumberText;
    public Text previousHashText;
    public Text hashText;

    public ParallelPhysicsController3D physicsEngine;

    public GameObject[] prefabs;

    public RollbackControllerState state = RollbackControllerState.Stopped;

    public bool fastStep = false;
    public bool fullTest = false;
    public bool warmStart = false;
    public bool detailLogging = false;

    public uint maxSteps = 100;
    public uint step = 0;
    public uint hash = 0;
    public uint size = 0;
    public uint gzipSize = 0;
    public double gzipRate = 0;
    public uint maxSize = 0;
    public uint maxZipSize = 0;


    public uint firstRunResult = 0;

    public float fullTestFixedUpdateTime = 0.001f;

    PSnapshot3D[] snapshots;

    ByteBuffer buffer = new ByteBuffer(1024 * 100);

    public uint[] previousHashes;
    public uint[] hashes;

    public uint fullTestStep = 0;
    bool _finishedFirstRun = false;

    void Awake()
    {
        physicsEngine.Initialize();
        physicsEngine.SetRollbackAddRigidbodyCallback(RestoreRigidbody);

        snapshots = new PSnapshot3D[maxSteps + 1];
        hashes = new uint[maxSteps + 1];
        previousHashes = new uint[maxSteps + 1];
        state = RollbackControllerState.Stopped;

        slider.minValue = 0;
        slider.maxValue = maxSteps;

        toggle.SetIsOnWithoutNotify(false);
    }


    void RestoreRigidbody(uint externalId, ushort bodyId, IntPtr pBody3D)
    {
        int index = (int)externalId;
        GameObject prefab = prefabs[index];

        ParallelPhysicsController3D.RestoreParallelObject(prefab, externalId, bodyId, pBody3D);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                PRaycastHit3D result;

                bool hit = false;

                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                Vector3 start = ray.origin;
                Vector3 end = ray.origin + ray.direction * 1000.0f;

                hit = Parallel3D.RayCast((Fix64Vec3)start, (Fix64Vec3)end, out result);

                if (hit)
                {
                    ParallelRigidbody3D rb = (ParallelRigidbody3D)result.rigidbody;
                    ParallelPhysicsController3D.DestroyParallelObject(rb.gameObject);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case RollbackControllerState.Playing:
                Playing();
                break;

            case RollbackControllerState.Stopped:
                Stopped();
                break;
        }
    }

    public void OnAutoToggleValueChanged(bool value)
    {
        if (value)
        {
            state = RollbackControllerState.Playing;
        }
        else
        {
            state = RollbackControllerState.Stopped;
        }
    }

    public void OnStepButtonPressed()
    {
        if (state == RollbackControllerState.Playing)
        {
            return;
        }
        else
        {
            Playing();
        }
    }

    public void OnSliderValueChanged(float value)
    {
        int stepValue = (int)value;
        step = (uint)stepValue;
        ReloadStep();
    }

    public void OnCreateButtonPressed()
    {
        if (prefabs.Length == 0)
        {
            return;
        }
        int size = prefabs.Length;
        int index = 2;// UnityEngine.Random.Range(0, size);

        GameObject prefab = prefabs[index];

        ParallelPhysicsController3D.InstantiateParallelObject(prefab, (Fix64Vec3)transform.position, (Fix64Quat)Quaternion.identity);
    }

    private void OnDestroy()
    {
        foreach (PSnapshot3D pSnapshot3D in snapshots)
        {
            if(pSnapshot3D != null)
            {
                Parallel3D.DestroySnapshot(pSnapshot3D);
            }
        }
    }

    void ReloadStep()
    {
        Debug.Log($"========RELOAD STEP({step})========");

        stepNumberText.text = $"{step}";
        previousHashText.text = $"{previousHashes[step]}";
        hashText.text = $"{hashes[step]}";

        PSnapshot3D snapshot = snapshots[step];

        Parallel3D.Restore(snapshot);
    }

    void Playing()
    {
        if (step < maxSteps)
        {
            if (step == 0)
            {
                Debug.Log($"STEP={step} SAVE rigidbody:");
                if (!_finishedFirstRun)
                {
                    snapshots[step] = Parallel3D.Snapshot();
                    hashes[step] = CalculateHash();
                }
            }

            step++;

            Debug.Log($"========Playing STEP({step})========");
            stepNumberText.text = $"{step}";
            slider.SetValueWithoutNotify(step);

            physicsEngine.Step(physicsEngine.fixedUpdateTime);

            if (!_finishedFirstRun)
            {
                snapshots[step] = Parallel3D.Snapshot();
            }

            hashes[step] = CalculateHash();

            //fails faster
            uint a = previousHashes[step];

            previousHashText.text = $"{previousHashes[step]}";
            hashText.text = $"{hashes[step]}";

            if (a != 0)
            {
                uint b = hashes[step];
                if (a != b)
                {
                    Debug.LogError($"Error {step}");
                }
            }
        }
        else
        {
            state = RollbackControllerState.Stopped;
            toggle.SetIsOnWithoutNotify(false);

            uint hash = CalculateHash();
            if (!_finishedFirstRun)
            {
                _finishedFirstRun = true;
                firstRunResult = hash;
                //copy to previous hash
                Array.Copy(hashes, 0, previousHashes, 0, maxSteps + 1);
            }
            else
            {
                for (int i = 0; i < maxSteps + 1; i++)
                {
                    uint a = previousHashes[i];
                    uint b = hashes[i];
                    if (a != b)
                    {
                        //Debug.LogError($"Error {i}");
                    }
                }
            }
        }
    }

    void Stopped()
    {
        if (_finishedFirstRun)
        {
            if (fullTest)
            {
                Time.fixedDeltaTime = fullTestFixedUpdateTime;
                if (fullTestStep != maxSteps)
                {
                    step = fullTestStep;
                    Debug.Log($"FULL TEST:{fullTestStep}");
                    ReloadStep();
                    state = RollbackControllerState.Playing;
                    fullTestStep++;
                }
            }
        }
    }

    uint CalculateHash()
    {
        buffer.Reset();

        //using (new SProfiler("ExportEngineState"))
        {
            //pSettings2D.ExportEngineState(lastInternalState);
        }

        //using (new SProfiler($"Push engine state {internalState.contactCount}"))
        {
            //buffer.Push(lastInternalState);
        }

        //using (new SProfiler($"Push body state {rigidbodies.Length}"))
        {
            foreach (ParallelRigidbody3D rigidbody in physicsEngine.rigidbodies)
            {
                Fix64Vec3 linearVelocity = rigidbody.LinearVelocity;
                Fix64Vec3 angularVelocity = rigidbody.AngularVelocity;

                ParallelTransform pTransform = rigidbody.pTransform;

                Fix64Vec3 pos = pTransform.position;
                Fix64Quat rot = pTransform.rotation;

                buffer.Push(linearVelocity);
                buffer.Push(angularVelocity);
                buffer.Push(pos);
                buffer.Push(rot);

                PrintRigidbody(rigidbody);
            }
        }

        {
            hash = buffer.Hash();
        }

        size = buffer.Length();
        if (size > maxSize)
        {
            maxSize = size;
        }

        return hash;
    }

    void PrintRigidbody(ParallelRigidbody3D rigidbody)
    {
        if (!detailLogging)
        {
            return;
        }

        Debug.Log($"{rigidbody.bodyId} awake={rigidbody.isAwake}\n" +
        $"lv={rigidbody.LinearVelocity} av={rigidbody.AngularVelocity}\n" +
        $"pos={rigidbody.pTransform.position} rot={rigidbody.pTransform.localRotation}");
    }
}
