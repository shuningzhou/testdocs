using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parallel;

public class CurveField : MonoBehaviour
{
    public FAnimationCurve curve;
    public float time;
    public float unityCurveValue;
    public FFloat fixCurveValue;
    public Vector4 t = new Vector4(4, -6, 3, -0.72f);
    public FFloat ttime = FFloat.half;
    // Start is called before the first frame update
    void Start()
    {
        //AnimationCurve ac = new AnimationCurve();
        //Keyframe kf = new Keyframe(0, 0, 0, 0, 1, 1);
        //Keyframe kf2 = new Keyframe(1, 1, 0, 0, 1, 1);

        //ac.AddKey(kf);
        //ac.AddKey(kf2);

        //curve.displayingCurve = ac;
        //curve.ImportUnityAnimationCurve();



    }
    private void OnValidate()
    {
        Debug.Log("========test========");

        //AnimationCurve ac = new AnimationCurve();
        //Keyframe kf = new Keyframe(0, 0, 0, 0, 1, 1);
        //Keyframe kf2 = new Keyframe(1, 1, 0, 0, 1, 1);

        //ac.AddKey(kf);
        //ac.AddKey(kf2);

        //curve.displayingCurve = ac;
        //curve.ImportUnityAnimationCurve();

        //unityCurveValue = curve.displayingCurve.Evaluate((float)ttime);
        //fixCurveValue = curve.Evaluate(ttime);

        //Debug.Log(unityCurveValue);
        //Debug.Log(fixCurveValue);


        CubicSolver2 cs = new CubicSolver2();
        cs.a1 = t.x;
        cs.b = t.y;
        cs.c = t.z;
        cs.d = t.w;

        switch (cs.Calc_Cardano())
        {
            case 0:
                {
                    Debug.Log("NO ROOTS");
                    break;
                }
            case 1:
                {
                    Debug.Log(cs.x1.real);
                    break;
                }
            case 2:
                {
                    Debug.Log(cs.x1.real);
                    Debug.Log(cs.x2.real);
                    break;
                }
            case 3:
                {
                    Debug.Log(cs.x1.real);
                    Debug.Log(cs.x2.real);
                    Debug.Log(cs.x3.real);
                    break;
                }
        }


        FFloat[] roots = CubicSolver.Solve((FFloat)t.x, (FFloat)(t.y), (FFloat)(t.z), (FFloat)(t.w));

        foreach (FFloat f in roots)
        {
            Debug.Log(f);
        }

        //List<double> roots = CubicSolver3.CheckAllPossibleRoots(t.x, t.y, t.z, t.w);

        //foreach (double f in roots)
        //{
        //    Debug.Log(f);
        //}

        //double[] roots = CubicSolver4.Solve(t.x, t.y, t.z, t.w);

        //foreach (double f in roots)
        //{
        //    Debug.Log(f);
        //}
    }
    // Update is called once per frame
    void Update()
    {



        unityCurveValue = curve.displayingCurve.Evaluate(time);
        fixCurveValue = curve.Evaluate((FFloat)time);

        Debug.Log("============Update==========");
        Debug.Log(unityCurveValue);
        Debug.Log(fixCurveValue);
    }
}
