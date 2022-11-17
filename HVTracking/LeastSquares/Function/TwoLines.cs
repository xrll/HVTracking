using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct TwoLines : IFunction
    {

        /// <summary>
        /// </summary>
        /// 
        public double[] Regression(double[] vx,double[] vy)
        {
            int n = vx.Length;
            return null;
        }
        /// <summary>
        /// </summary>
        /// 
        public double[] NLRegression(double[] vx, double[] vy,double[] w)
        {
            int maxIters = 1000,iter = 0;
            double lambda = 0.1, minDeltaChi2 = 0.000001;
            double[] nw = (double[])w.Clone();
            double[] g;
            double[,] tj = new double[4,4];
            double[] ty = new double[4];
            double mse = 0, lmse = 0;
            while (iter < maxIters)
            {
                mse = 0;
                for (int i = 0; i < vx.Length; i++)
                {
                    if (nw == null)
                        return null;
                    double y = Function(vx[i], nw);
                    if (vx[i] >= nw[3])
                        g = new double[4] { 1.0, vx[i], vx[i], -nw[2] };
                    else
                        g = new double[4] { 1.0, vx[i], 0.0, 0.0 };
                    tj = tj.Add(g.Cov());
                    double dy = vy[i] - y;
                    mse += dy * dy;
                    ty = ty.Add(g.Multiply(dy));
                }
                tj.Divide(vx.Length);
                ty.Divide(vx.Length);
                tj[0, 0] *= (1.0 + lambda);
                tj[1, 1] *= (1.0 + lambda);
                tj[2, 2] *= (1.0 + lambda);
                tj[3, 3] *= (1.0 + lambda);
                /*
                tj[0, 0] += lambda;
                tj[1, 1] += lambda;
                tj[2, 2] += lambda;
                tj[3, 3] += lambda;
                */
                nw = nw.Add(DoubleArrayExtensions.InvertSsgjUnsafeU(tj).Dot(ty));
 //               if (Math.Abs(mse - lmse) < minDeltaChi2)
 //                   break;
                
                if (lmse > mse)
                {
                    lambda *= 10;
                }
                else
                {
                    lambda /= 10;
                }
                lmse = mse;
                iter++;
            }
            return nw;
        }
        /// <summary>
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public unsafe double Function(double x,double[] w)
        {
            if (w == null)
                return 0;

            //     y = a + b·x + c*(x-d)
            if (x < w[3])
                return w[0] + w[1] * x;
            return w[0] + w[1] * x + w[2] * (x - w[3]);
        }
        /// <summary>
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="w">Vector <c>w</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public unsafe double[] BatchFunction(double[] x, double[] w)
        {
            if (w == null)
                return null;
            double[] y = new double[x.Length];
            fixed (double* px = x)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    y[i] = Function(*(px + i), w);
                }
            }
            return y;
        }
    }
}
