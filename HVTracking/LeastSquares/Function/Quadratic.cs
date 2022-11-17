using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct Quadratic : IFunction
    {

        /// <summary>
        ///   Gets or sets the sigma value for the kernel. When setting
        ///   sigma, gamma gets updated accordingly (gamma = 0.5/sigma^2).
        /// </summary>
        /// 
        public double[] Regression(double[] vx,double[] vy)
        {
            double[] tw = new double[3];
            double mx = vx.Average();
            double my = vy.Average();
            double[] tx = new double[3];
            double[] ty = new double[3];
            double[,] ls = new double[3,3];
            for(int i =0;i<vx.Length;i++)
            {
                tx = new double[] {1.0,vx[i],vx[i]*vx[i] };
                ty = ty.Add(new double[] { vy[i], vx[i] * vy[i], vx[i] * vx[i] * vy[i] });
                ls = ls.Add(tx.Cov());
            }
            ty = ty.Divide(vx.Length);
            ls.Divide(vx.Length);
            tw = DoubleArrayExtensions.InvertSsgjUnsafeU(ls).Dot(ty);
//            tw = Matrix.CholeskyDecompositionInverse3(ls, ty);
            return tw;
        }
        /// <summary>
        /// </summary>
        /// 
        public double[] NLRegression(double[] vx, double[] vy, double[] w)
        {
            int maxIters = 1000, iter = 0;
            double lambda = 0.0001, minDeltaChi2 = 0.0001;
            double[] nw = (double[])w.Clone();
            double[] g;
            double[,] tj = new double[3, 3];
            double[] beta = new double[3];
            double mse = 0, lmse = 0, det = 0;
            while (iter < maxIters)
            {
                mse = 0;
                for (int i = 0; i < vx.Length; i++)
                {
                    double y = Function(vx[i], nw);
                    g = new double[3] { 1, vx[i], vx[i] * vx[i] };
                    tj = tj.Add(g.Cov());
                    double dy = vy[i] - y;
                    mse += dy * dy;
                    beta = beta.Add(g.Multiply(dy));
                }
                //                tj.Divide(vx.Length);
                //                beta.Divide(vx.Length);
                for (int i = 0; i < 3; i++)
                    tj[i, i] *= (1.0 + lambda);
                tj = DoubleArrayExtensions.InvertSsgjUnsafeL(tj);
                if (tj == null)
                    continue;
                nw = nw.Add(tj.Dot(beta));
                if (Math.Abs(mse - lmse) < minDeltaChi2)
                    break;

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
            double y = 0.0;

                fixed (double* pw = w)
                {
                    for (int i = 0; i < w.Length; i++)
                    {
                        y += Math.Pow(x,i)* *(pw + i);
                    }
                }
            return y;
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
