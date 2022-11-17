using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct abExponential : IFunction
    {

        /// <summary>
        /// </summary>
        /// 
        public double[] Regression(double[] vx,double[] vy)
        {
            double[] tw = new double[3];
            double mx = vx.Average();
            double my = vy.Average();
            double[] tx = new double[2];
            double[] ty = new double[2];
            double[,] ls = new double[2,2];
            for(int i =0;i<vx.Length;i++)
            {
                tx = new double[] {1.0,vx[i]};
                ty = ty.Add(new double[] { Math.Log(vy[i]), vx[i] * Math.Log(vy[i]) });
                ls = ls.Add(tx.Cov());
            }
            ty = ty.Divide(vx.Length);
            ls.Divide(vx.Length);
//            tw = Matrix.InvertSsgjUnsafeU(ls).Dot(ty);
            double det = ls[0, 0] * ls[1, 1] - ls[0, 1] * ls[1, 0];
            if (det == 0)
                return null;
            tw[0] = Math.Exp((ls[1, 1] * ty[0] - ls[0, 1] * ty[1]) / det);
            tw[1] = Math.Exp((-ls[1, 0] * ty[0] + ls[0, 0] * ty[1]) / det);
            tw[2] = -1;
            return tw;
        }
        /// <summary>
        /// </summary>
        /// 
        public double[] NLRegression(double[] vx, double[] vy,double[] w)
        {
            int maxIters = 1000,iter = 0;
            double lambda = 0.0001, minDeltaChi2 = 0.0001;
            double[] nw = (double[])w.Clone();
            double[] g;
            double[,] tj = new double[2,2];
            double[] beta = new double[2];
            double mse = 0, lmse = 0, det = 0;
            while (iter < maxIters)
            {
                mse = 0;
                for (int i = 0; i < vx.Length; i++)
                {
                    double y = Function(vx[i], nw);
                    g = new double[2] { Math.Pow(nw[1], vx[i]), nw[0] * vx[i] * Math.Pow(nw[1], (vx[i] - 1)) };
                    tj = tj.Add(g.Cov());
                    double dy = vy[i] - y;
                    mse += dy * dy;
                    beta = beta.Add(g.Multiply(dy));
                }
//                tj.Divide(vx.Length);
//                beta.Divide(vx.Length);
                tj[0, 0] *= (1.0 + lambda);
                tj[1, 1] *= (1.0 + lambda);
                det = tj[0, 0] * tj[1, 1] - tj[0, 1] * tj[1, 0];
                nw[0] += (tj[1, 1] * beta[0] - tj[0, 1] * beta[1]) / det;
                nw[1] += (-tj[1, 0] * beta[0] + tj[0, 0] * beta[1]) / det;
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
                y = *pw * Math.Pow(*(pw + 1), x);
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
