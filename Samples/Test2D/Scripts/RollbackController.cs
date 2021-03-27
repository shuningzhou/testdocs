﻿using Parallel;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum RollbackControllerState
{
    Playing = 0,
    Stopped = 1
}

public class RollbackController : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;
    public Text stepNumberText;
    public Text previousHashText;
    public Text hashText;

    public ParallelPhysicsController2D physicsEngine;
    PShapeOverlapResult2D result = new PShapeOverlapResult2D();

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

    public Vector3 mousePosition;
    public FVector2 fixedMousePosition;

    PSnapshot2D[] snapshots;

    ByteBuffer buffer = new ByteBuffer(1024 * 100);

    public uint[] previousHashes;
    public uint[] hashes;

    public uint fullTestStep = 0;
    bool _finishedFirstRun = false;

    void Awake()
    {
        physicsEngine.Initialize();
        //Action<uint, ushort, PBody2D> action = AddMissingRigidbody;

        physicsEngine.SetRollbackAddRigidbodyCallback(RestoreRigidbody);

        snapshots = new PSnapshot2D[maxSteps + 1];
        hashes = new uint[maxSteps + 1];
        previousHashes = new uint[maxSteps + 1];
        state = RollbackControllerState.Stopped;

        slider.minValue = 0;
        slider.maxValue = maxSteps;

        toggle.SetIsOnWithoutNotify(false);
    }

    void RestoreRigidbody(uint externalId, ushort bodyId, IntPtr pBody2D)
    {
        int index = (int)externalId;
        GameObject prefab = prefabs[index];

        ParallelPhysicsController2D.RestoreParallelObject(prefab, externalId, bodyId, pBody2D);
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        fixedMousePosition = (FVector2)mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                bool hit = Parallel2D.OverlapCircle(fixedMousePosition, FFloat.FromDivision(1, 10), result);
                if (hit)
                {
                    ParallelRigidbody2D rb = (ParallelRigidbody2D)result.rigidbodies[0];
                    ParallelPhysicsController2D.DestroyParallelObject(rb.gameObject);
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
        if(value)
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
        if(state == RollbackControllerState.Playing)
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
        if(prefabs.Length == 0)
        {
            return;
        }
        int size = prefabs.Length;
        int index = UnityEngine.Random.Range(0, size);

        GameObject prefab = prefabs[index];

        ParallelPhysicsController2D.InstantiateParallelObject(prefab, (FVector3)transform.position, (FQuaternion)Quaternion.identity);
    }

    private void OnDestroy()
    {
        foreach (PSnapshot2D pSnapshot2D in snapshots)
        {
            if(pSnapshot2D != null)
            {
                Parallel2D.DestroySnapshot(pSnapshot2D);
            }
        }
    }

    void ReloadStep()
    {
        Debug.Log($"========RELOAD STEP({step})========");
        stepNumberText.text = $"{step}";
        previousHashText.text = $"{previousHashes[step]}";
        hashText.text = $"{hashes[step]}";

        PSnapshot2D snapshot = snapshots[step];
        Parallel2D.Restore(snapshot);
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
                    snapshots[step] = Parallel2D.Snapshot();
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
                snapshots[step] = Parallel2D.Snapshot();
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
            foreach (ParallelRigidbody2D rigidbody in physicsEngine.rigidbodies)
            {
                FVector2 linearVelocity = rigidbody.LinearVelocity;
                FFloat angularVelocity = rigidbody.AngularVelocity;

                ParallelTransform pTransform = rigidbody.pTransform;

                FVector2 pos = (FVector2)pTransform.position;
                FFloat rot = pTransform.eulerAngles.z;

                buffer.Push(linearVelocity);
                buffer.Push(angularVelocity);
                buffer.Push(pos);
                buffer.Push(rot);

                //PrintRigidbody(rigidbody);
            }
        }

        //return;

        //using (new SProfiler($"Hash"))
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

    void PrintRigidbody(ParallelRigidbody2D rigidbody)
    {
        if (!detailLogging)
        {
            return;
        }

        Debug.Log($"{rigidbody.bodyId}\n" +
                $"{rigidbody.LinearVelocity} {rigidbody.AngularVelocity}\n" +
                $"{rigidbody.pTransform.position} {rigidbody.pTransform.localEulerAngles.z}");
    }


    void PrintInternalState(PInternalState2D internalState)
    {
        if (!detailLogging)
        {
            return;
        }

        for (int i = 0; i < internalState.contactCount; i++)
        {
            PContactExport2D export = internalState.contactExports[i];
            Debug.Log($"id={export.id} count={export.count}\n" +
                $"{export.k1} {export.n1} {export.t1}\n" +
                $"{export.k2} {export.n2} {export.t2}");
        }
    }
}