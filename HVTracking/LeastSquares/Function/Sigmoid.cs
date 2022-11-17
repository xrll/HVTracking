using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct Sigmoid : IFunction
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
            int maxIters = 5000,iter = 0;
            double lambda = 0.01, minDeltaChi2 = 0.000001;
            double[] nw = (double[])w.Clone();
            double[] g;
            double[,] tj = new double[3,3];
            double[] ty = new double[3];
            double mse = 0, lmse = 0;
            while (iter < maxIters)
            {
                mse = 0;
                if (nw == null)
                    break;
                for (int i = 0; i < vx.Length; i++)
                {
                    double y = Function(vx[i], nw);
                    double tmp = 1.0/(1.0 + Math.Exp(nw[2] - vx[i]));
                    g = new double[3] { 1.0, tmp, -nw[1] * tmp * (1 - tmp) };
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
                nw = nw.Add(DoubleArrayExtensions.InvertSsgjUnsafeU(tj).Dot(ty));
//                if (Math.Abs(mse - lmse) < minDeltaChi2)
//                    break;
                
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
            //    y = a + b/(1+exp(u - x))
            return w[0] + w[1] / (1.0 + Math.Exp(-(x - w[2])));
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
