using System;
using System.Collections.Generic;
using UnityEngine;

namespace Parallel
{
    public class CubicSolver4
    {
        public static double[] Solve(double a, double b, double c, double d)
        {
            if (a == 0 && b == 0)
            {
                double[] roots = { -d / c };
                return roots;
            }
            else if ( a == 0)
            {
                double D = c * c - 4.0 * b * d;
                double x1, x2;

                if(d>=0)
                {
                    D = Math.Sqrt(D);
                    x1 = (-c + D) / (2.0 * b);
                    x2 = (-c - D) / (2.0 * b);

                    double[] roots = { x1, x2 };
                    return roots;
                }
                else
                {
                    double[] roots = {  };
                    return roots;
                }
            }

            double f = findF(a, b, c);
            double g = findG(a, b, c, d);
            double h = findH(g, f);

            if (f == 0 && g == 0 && h == 0)
            {
                if((d/a) >= 0)
                {
                    double x = -1 * (Math.Pow(d / a, 1 / 3.0));
                    double[] roots = { x };
                    return roots;
                }
                else
                {
                    double x = (Math.Pow(- d / a, 1 / 3.0));
                    double[] roots = { x };
                    return roots;
                }
            }
            else if ( h <= 0 )
            {
                double i = Math.Sqrt((Math.Pow(g, 2.0) / 4.0) - h);
                double j = Math.Pow(i, (1 / 3.0));
                double k = Math.Acos(-(g / (2 * i)));
                double L = j * -1;
                double M = Math.Cos(k / 3.0);
                double N = Math.Sqrt(3) * Math.Sin(k / 3.0);
                double P = (b / (3.0 * a)) * -1;

                double x1 = 2 * j * Math.Cos(k / 3.0) - (b / (3.0 * a));
                double x2 = L * (M + N) + P;
                double x3 = L * (M - N) + P;

                double[] roots = { x1, x2, x3 };
                return roots;
            }
            else if (h > 0)
            {
                double R = -(g / 2.0) + Math.Sqrt(h);
                double S, U;
                if (R >= 0)
                {
                    S = Math.Pow(R, (1 / 3.0));
                }
                else
                {
                    S = Math.Pow((-R), (1 / 3.0)) * -1;
                }

                double T = -(g / 2.0) - Math.Sqrt(h);

                if(T>=0)
                {
                    U = Math.Pow(T, (1 / 3.0));
                }
                else
                {
                    U = Math.Pow(-T, (1 / 3.0)) * -1;
                }

                double x1 = (S + U) - (b / (3.0 * a));

                double[] roots = { x1 };
                return roots;
            }

            double[] roots1 = { };
            return roots1;
        }
        
        public static double findF(double a, double b, double c)
        {
            return ((3.0 * c / a) - (Math.Pow(b, 2) / Math.Pow(2, 2))) / 3.0;
        }

        public static double findG(double a, double b, double c, double d)
        {
            return (((2.0 * Math.Pow(b, 3)) / Math.Pow(a, 3)) - ((9.0 * b * c) / Math.Pow(a, 2)) + (27.0 * d / a)) / 27.0;
        }

        public static double findH(double g, double f)
        {
            return(Math.Pow(g, 2) / 4.0 + Math.Pow(f, 3) / 27.0);
        }
    }

    public class CubicSolver3
    {
        public static double NthRoot(double A, int N)
        {
            return Math.Pow(A, 1.0 / N);
        }

        public static List<double> Discrimination(double a, double b, double c)
        {
            double temp = default(double);
            List<double> result = new List<double>();
            double root = default(double);
            temp = b * b - 4 * a * c;
            if (temp < 0)
            {
                Console.WriteLine($"This function * ({a})x^2+({b})x+({c})=0 * hasn't any solution !");
            }
            else if (temp == 0)
            {
                root = -(b) / (2 * a);
                result.Add(root);
                return result;
            }

            temp = Math.Sqrt(temp);
            root = (-(b) + temp) / (2 * a);
            result.Add(root);
            root = (-(b) - temp) / (2 * a);
            result.Add(root);
            return result;
        }

        public static List<double> CheckAllPossibleRoots(double a, double b, double c, double d)
        {
            List<double> allPossibleRootsP = new List<double>();
            List<double> allPossibleRootsQ = new List<double>();
            List<double> realRoots = new List<double>();

            for (double i = 1; i <= Math.Abs(d); i++)
            {
                if (d % i == 0)
                    allPossibleRootsP.Add(i);
            }

            for (double j = 1; j <= Math.Abs(a); j++)
            {
                if (d % j == 0)
                    allPossibleRootsQ.Add(j);
            }

            foreach (var rootQ in allPossibleRootsQ)
            {
                foreach (var rootP in allPossibleRootsP)
                {
                    double ratioRoot = rootP / rootQ;

                    if (((((ratioRoot * a + b) * ratioRoot) + c) * ratioRoot) + d == 0)
                    {
                        foreach (var root in Discrimination(a, (ratioRoot * a + b), (((ratioRoot * a + b) * ratioRoot) + c)))
                        {
                            realRoots.Add(root);
                        }
                        realRoots.Add(ratioRoot);
                        return realRoots;
                    }
                    else if ((((((-ratioRoot) * a + b) * (-ratioRoot)) + c) * (-ratioRoot)) + d == 0)
                    {
                        foreach (var root in Discrimination(a, ((-ratioRoot) * a + b), ((((-ratioRoot) * a + b) * (-ratioRoot)) + c)))
                        {
                            realRoots.Add(root);
                        }
                        realRoots.Add(-ratioRoot);
                        return realRoots;
                    }
                }
            }
            return realRoots;
        }
    }
    public struct TKomplex
    {
        public double real;
        public double imag;
    };

    public class CubicSolver2
    {
        public TKomplex x1, x2, x3;
        public double a1, a, b, c, d;

        public double Xroot(double a, double x)
        {
            if (a < 0)
            {
                double j = Math.Log(-a);
                j = j / x;
                j = Math.Exp(j);
                return -j;
            }
            else
            {
                double j = Math.Log(a);
                j = j / x;
                j = Math.Exp(j);
                return j;
            }
        }

        public int Calc_Cardano()  // solve cubic equation according to cardano
        {
            double p, q, u, v;
            double r, alpha;
            int res;
            res = 0;
            if (a1 != 0)
            {
                a = b / a1;
                b = c / a1;
                c = d / a1;

                p = -(a * a / 3.0) + b;
                q = (2.0 / 27.0 * a * a * a) - (a * b / 3.0) + c;
                d = q * q / 4.0 + p * p * p / 27.0;
                if (Math.Abs(d) < Math.Pow(10.0, -11.0))
                    d = 0;
                // 3 cases D > 0, D == 0 and D < 0
                if (d > 1e-20)
                {
                    u = Xroot(-q / 2.0 + Math.Sqrt(d), 3.0);
                    v = Xroot(-q / 2.0 - Math.Sqrt(d), 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    x2.imag = Math.Sqrt(3.0) / 2.0 * (u - v);
                    x3.real = x2.real;
                    x3.imag = -x2.imag;
                    res = 1;
                }
                if (Math.Abs(d) <= 1e-20)
                {
                    u = Xroot(-q / 2.0, 3.0);
                    v = Xroot(-q / 2.0, 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    res = 2;
                }
                if (d < -1e-20)
                {
                    r = Math.Sqrt(-p * p * p / 27.0);
                    alpha = Math.Atan(Math.Sqrt(-d) / -q * 2.0);
                    if (q > 0)                         // if q > 0 the angle becomes 2 * PI - alpha
                        alpha = 2.0 * Math.PI - alpha;

                    x1.real = Xroot(r, 3.0) * (Math.Cos((6.0 * Math.PI - alpha) / 3.0) + Math.Cos(alpha / 3.0)) - a / 3.0;
                    x2.real = Xroot(r, 3.0) * (Math.Cos((2.0 * Math.PI + alpha) / 3.0) + Math.Cos((4.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    x3.real = Xroot(r, 3.0) * (Math.Cos((4.0 * Math.PI + alpha) / 3.0) + Math.Cos((2.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    res = 3;
                }
            }
            else
                res = 0;
            return res;
        }
    }
    public class CubicSolver
    {
        //https://github.com/dnpp73/CubicEquationSolver/blob/master/Sources/Solver/CubicEquation.swift
        public static FFloat[] Solve(FFloat a, FFloat b, FFloat c, FFloat d)
        {
            Debug.Log($"Solve: {a}, {b}, {c}, {d}");
            if ( a == FFloat.zero ) {
                return SolveQuadratic(b, c, d);
            }
            else
            {
                return Solve1(b / a, c / a, d / a);
          }
        }

        // x^3 + Ax^2 + Bx + C = 0
        internal static FFloat[] Solve1(FFloat a, FFloat b, FFloat c)
        {
            if (a == FFloat.zero)
            {
                return Solve2(b, c);
            }
            else
            {
                FFloat p = b - (a * a / FFloat.three);
                FFloat q = c - (a * b) / FFloat.three + ((FFloat.two / FFloat.FromDivision(27, 1)) * a * a * a);
                FFloat[] roots = Solve2(p, q);

                for(int x = 0; x < roots.Length; x++)
                {
                    FFloat v = roots[x];
                    v = v - a / FFloat.three;
                    roots[x] = v;
                }

                return roots;
            }
        }

        // x^3 + px + q = 0
        // for avoid considering the complex plane, I choose geometric solution.
        // respect François Viète
        // ref: https://pomax.github.io/bezierinfo/#extremities
        internal static FFloat[] Solve2(FFloat p, FFloat q)
        {
            FFloat p3 = p / FFloat.three;
            FFloat q2 = q / FFloat.two;

            FFloat discriminant = q2 * q2 + p3 * p3 * p3;

            if (discriminant < FFloat.zero)
            {
                // three possible real roots
                FFloat r = FMath.Sqrt(FMath.Pow(-p3, FFloat.three));
                FFloat t = -q / (FFloat.two * r);
                FFloat cosphi = FMath.Min(FMath.Max(t, -FFloat.one), FFloat.one);
                FFloat phi = FMath.Acos(cosphi);
                FFloat c = FFloat.two * cubeRoot(r);
                FFloat[] roots =
                {
                    c * FMath.Cos(phi / FFloat.three),
                    c * FMath.Cos((phi + FFloat.two * FFloat.pi) / FFloat.three),
                    c * FMath.Cos((phi + FFloat.two * FFloat.two * FFloat.pi) / FFloat.three)
                };

                return roots;
            }
            else if (discriminant == FFloat.zero)
            {
                // three real roots, but two of them are equal

                FFloat u = FFloat.zero;

                if(q2 < FFloat.zero)
                {
                    u = cubeRoot(-q2);
                }
                else
                {
                    u = -cubeRoot(q2);
                }

                FFloat[] roots = { FFloat.two * u, -u };

                return roots;
            }
            else
            {
                // one real root, two complex roots

                FFloat sd = FMath.Sqrt(discriminant);
                FFloat u = cubeRoot(sd - q2);
                FFloat v = cubeRoot(-sd - q2);

                FFloat[] roots = { u + v };

                return roots;
            }
        }

        internal static FFloat cubeRoot(FFloat v)
        {
            if(FMath.Abs(v) < FFloat.err)
            {
                return FFloat.zero;
            }

            if ( v < FFloat.zero)
            {
                FFloat j = FMath.Log(-v);
                j = j / FFloat.three;
                j = FMath.Pow(FFloat.e, j);
                return -j;
            }
            else
            {
                FFloat j = FMath.Log(v);
                j = j / FFloat.three;
                j = FMath.Pow(FFloat.e, j);
                return j;
            }
        }

        internal static FFloat[] SolveQuadratic(FFloat a, FFloat b, FFloat c)
        {
            if( a == FFloat.zero)
            {
                if (b == FFloat.zero)
                {
                    FFloat[] roots = { };
                    return roots;
                }
                else
                {
                    FFloat[] roots = { -c / b };
                    return roots;
                }
            }

            FFloat discriminant = b * b - FFloat.two * FFloat.two * a * c;
            if(discriminant < FFloat.zero)
            {
                FFloat[] roots = { };
                return roots;
            }
            else if (discriminant == FFloat.zero)
            {
                FFloat[] roots = { - b / ( FFloat.two * a ) };
                return roots;
            }
            else
            {
                FFloat d = FMath.Sqrt(discriminant);
                FFloat[] roots =
                {
                    ( -b + d ) / ( FFloat.two * a ),
                    ( -b - d ) / ( FFloat.two * a ),
                };
                return roots;
            }
        }
    }


    [Serializable]
    public struct FKeyframe : IEquatable<FKeyframe>, IComparable<FKeyframe>
    {
        public FFloat _value;
        public FFloat _time;

        public FFloat _inTangent;
        public FFloat _outTangent;

        public FFloat _inWeight;
        public FFloat _outWeight;

        public FKeyframe(FFloat value, FFloat time, FFloat inT, FFloat outT, FFloat inW, FFloat outW)
        {
            _value = value;
            _time = time;
            _inTangent = inT;
            _outTangent = outT;
            _inWeight = inW;
            _outWeight = outW;
        }

        public override string ToString() => $"time={_time} value={_value} inTangent={_inTangent} outTangent={_outTangent} inWeight={_inWeight} outWeight={_outWeight}";

        public override bool Equals(object obj) =>
            (obj is FKeyframe part)
                    ? Equals(part)
                    : false;

        public int CompareTo(FKeyframe compareObj) =>
            compareObj._time < _time ? 1 : 0;

        public override int GetHashCode() => _time.GetHashCode();

        public bool Equals(FKeyframe other) =>
            other._time == _time &&
            other._value == _value &&
            other._inTangent == _inTangent &&
            other._outTangent == _outTangent &&
            other._inWeight == _inWeight &&
            other._outWeight == _outWeight;
    }

    [Serializable]
    public class FAnimationCurve
    {
        bool _sorted = true;
        public AnimationCurve displayingCurve = null;

        public FAnimationCurve(bool sorted)
        {
            _sorted = sorted;
            Keyframe[] ks = new Keyframe[2];

            ks[0] = new Keyframe(0, 0, 0, 0);
            ks[0].weightedMode = WeightedMode.Both;

            ks[1] = new Keyframe(1, 1, 0, 0);
            ks[1].weightedMode = WeightedMode.Both;

            displayingCurve = new AnimationCurve(ks);
            ImportUnityAnimationCurve();
        }

        public FAnimationCurve()
        {
            _sorted = true;
            Keyframe[] ks = new Keyframe[2];
            ks[0] = new Keyframe(0, 0, 0, 0);
            ks[0].weightedMode = WeightedMode.Both;
            ks[1] = new Keyframe(1, 1, 0, 0);
            ks[1].weightedMode = WeightedMode.Both;
            displayingCurve = new AnimationCurve(ks);
            ImportUnityAnimationCurve();
        }

        public void ImportUnityAnimationCurve()
        {
            Clear();

            for (int x = 0; x < displayingCurve.length; x++)
            {
                Keyframe keyframe = displayingCurve[x];

                if(keyframe.weightedMode != WeightedMode.Both)
                {
                    Debug.LogError($"FixAnimationCurve only supports WeightedMode.Both Keyframes: key{keyframe.time} has been converted({keyframe.weightedMode}->WeightedMode.Both)");
                    keyframe.weightedMode = WeightedMode.Both;
                    displayingCurve.MoveKey(x, keyframe);
                }

                FKeyframe fixKeyFrame = new FKeyframe(
                                                (FFloat)keyframe.value,
                                                (FFloat)keyframe.time,
                                                (FFloat)keyframe.inTangent,
                                                (FFloat)keyframe.outTangent,
                                                (FFloat)keyframe.inWeight,
                                                (FFloat)keyframe.outWeight);

                Debug.Log("Add " + fixKeyFrame);
                AddKey(fixKeyFrame);
            }
        }

        public List<FKeyframe> _keys = new List<FKeyframe>();

        public FKeyframe[] keys 
        { 
            get 
            { 
                return _keys.ToArray(); 
            } 
            
            set 
            {
                _keys.Clear();

                _keys.AddRange(value);

                if(_sorted)
                {
                    _keys.Sort();
                }
            } 
        }

        public int length
        {
            get
            {
                return _keys.Count;
            }
        }

        public FKeyframe this[int index]
        {
            get 
            {
                return _keys[index];
            }
        }

        public int MoveKey(int index, FKeyframe key)
        {
            FKeyframe old = _keys[index];

            if(old._time == key._time)
            {
                old._value = key._value;
                _keys[index] = old;

                return index;
            }

            if(_sorted)
            {
                _keys.RemoveAt(index);

                return AddKey(key);
            }
            else
            {
                _keys[index] = key;
                return index;
            }
        }

        public void RemoveKey(int index)
        {
            _keys.RemoveAt(index);
        }

        public int AddKey(FKeyframe key)
        {
            if(_sorted)
            {
                int indexToAdd = -1;
                for( int x = 0; x < length; x++)
                {
                    FKeyframe k = _keys[x];
                    if(k._time > key._time)
                    {
                        indexToAdd = x - 1;
                        break;
                    }
                }

                if(indexToAdd == -1)
                {
                    _keys.Add(key);
                    return _keys.Count - 1;
                }
                else
                {
                    _keys.Insert(indexToAdd, key);
                    return indexToAdd;
                }
            }
            else
            {
                _keys.Add(key);
                return _keys.Count - 1;
            }
        }

        public void Clear()
        {
            _keys.Clear();
        }

        public FFloat Evaluate(FFloat time)
        {
            if (!_sorted)
            {
                return FFloat.zero;
            }
            else
            {
                if (_keys.Count < 2)
                {
                    return FFloat.zero;
                }

                int startIndex = 0;
                int endIndex = _keys.Count - 1;

                if (time <= _keys[startIndex]._time)
                {
                    return _keys[startIndex]._value;
                }

                if (time >= _keys[endIndex]._time)
                {
                    return _keys[endIndex]._value;
                }


                for (int x = 0; x < length; x++)
                {
                    FKeyframe k = _keys[x];
                    if (k._time > time)
                    {
                        startIndex = x - 1;
                        endIndex = x;
                        break;
                    }
                }

                FKeyframe startFrame = _keys[startIndex];
                FKeyframe endFrame = _keys[endIndex];

                FFloat x0 = startFrame._time; // 0
                FFloat y0 = startFrame._value; // 0
                FFloat ow0 = startFrame._outWeight; // 0
                FFloat ot0 = startFrame._outTangent; // 1

                FFloat x1 = endFrame._time; // 1
                FFloat y1 = endFrame._value; // 1
                FFloat iw1 = endFrame._inWeight; // 0
                FFloat it1 = endFrame._inTangent; // 1

                
                FVector2 A = new FVector2(x0, y0); //( 0,0)
                FVector2 B = new FVector2(x0 + ow0 * ( x1 - x0 ), y0 + ow0 * ot0 * ( x1 - x0 ));//( 0,0)
                FVector2 C = new FVector2(x1 - iw1 * ( x1 - x0 ), y1 - iw1 * it1 * ( x1 - x0 ));//( 1,1)
                FVector2 D = new FVector2(x1, y1);//( 1,1)

                FFloat X1 = A.x;
                FFloat X2 = B.x;
                FFloat X3 = C.x;
                FFloat X4 = D.x;

                //(1-t) * (1-t) * (1-t) * x1 + 3 * (1-t) * (1-t) * t * x2 + 3 * (1-t) * t^2 * x3 + t^3 * x4 = x
                // (1-2t+t^2)(1-t)x1 + 3 * ( 1-2t+t^2 ) t * x2 + (3t^2 - 3t^3)x3 + t^3 * x4 - x = 0
                // (1-t -2t +2t^2 + t^2 - t^3)x1 + (3t - 6t^2 + 3t^3)x2 + (3t^2 - 3t^3)x3 + t^3 * x4 - x = 0
                // (1-3t + 3t^2 - t^3)x1 + (3t - 6t^2 + 3t^3)x2 + (3t^2 - 3t^3)x3 + t^3 * x4 - x = 0
                // (-x1 + 3x2 - 3x3 + x4) * t^3 + (3x1 -6x2 + 3x3) * t^2 + (-3x1 + 3x2) * t + (x1 - x) = 0

                FFloat a = -X1 + FFloat.three * X2 - FFloat.three * X3 + X4;
                FFloat b = FFloat.three * X1 - FFloat.three * FFloat.two * X2 + FFloat.three * X3;
                FFloat c = -FFloat.three * X1 + FFloat.three * X2;
                FFloat d = X1 - time;

                //Debug.Log($"SOLVE :a={a} b={b} c={c} d={d}");
                FFloat[] roots = CubicSolver.Solve(a, b, c, d);

                if(roots.Length == 0)
                {
                    return FFloat.zero;
                }

                FFloat t = FFloat.zero;

                for(int x = 0; x < roots.Length; x++)
                {
                    FFloat r = roots[x];

                    if(r >= FFloat.zero && r <=FFloat.one)
                    {
                        t = r;
                    }
                }

                FFloat t2 = t * t; // 0.04
                FFloat t3 = t2 * t; // 0.008
                FFloat u = FFloat.one - t; // 0.8
                FFloat u2 = u * u; // 0.64
                FFloat u3 = u2 * u; // 0.512
                FFloat three = FFloat.FromDivision(3, 1); // 3

                FVector2 result = u3 * A + // (0,0) 
                                    three * u2 * t * B + // (0,0)
                                    three * u * t2 * C + // (0.096, 0.096)
                                    t3 * D; // (0.08, 0.08)

                return result.y;
            }
        }
    }
}
