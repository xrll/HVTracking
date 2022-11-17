using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct Polynomial : IFunction
    {
        int order;
        public Polynomial(int order)
        {
            this.order = order;
        }
        /// <summary>
        ///   Gets or sets the sigma value for the kernel. When setting
        ///   sigma, gamma gets updated accordingly (gamma = 0.5/sigma^2).
        /// </summary>
        /// 
        public double[] Regression(double[] vx,double[] vy)
        {
            int n = order + 1;
            double[] tw = new double[n];
            double[] tx = new double[n];
            double[] ty = new double[n];
            double[,] ls = new double[n,n];
            for(int i =0;i<vx.Length;i++)
            {
                for (int j = 0; j < order + 1; j++)
                {
                    tx[j] = Math.Pow(vx[i], j);
                    ty[j] += vy[i]* Math.Pow(vx[i], j);
                }
                ls = ls.Add(tx.Cov());
            }
            ty = ty.Divide(vx.Length);
            ls.Divide(vx.Length);
            tw = DoubleArrayExtensions.InvertSsgjUnsafeU(ls).Dot(ty);
//            tw = Matrix.CholeskyDecompositionInverse4(ls, ty);
            return tw;
        }
        /// <summary>
        ///   Gets or sets the sigma value for the kernel. When setting
        ///   sigma, gamma gets updated accordingly (gamma = 0.5/sigma^2).
        /// </summary>
        /// 
        public double[] Regression2(double[] vx, double[] vy)
        {
            int n = order + 1;
            int m = vx.Length;
            double[] tw = new double[n];
            double[,] tx = new double[m,n];
            double[] ty = new double[n];
            double[,] ls = new double[n, n];
            for (int i = 0; i < vx.Length; i++)
            {
                for (int j = 0; j < order + 1; j++)
                {
                    tx[i,j] = Math.Pow(vx[i], j);
                }
            }
            double[,] Q, R;
            DoubleArrayExtensions.HouseholderDecomposition(tx,out Q,out R);            
            //double[,] S = Q.Dot(R);
            //double[,] I = Q.Dot(Q.Transpose());

          //  tw =R.PseudoInverse().Dot(Q.Transpose()).Dot(vy);
            tw=R.InvUpperTRI().Dot(Q.Transpose()).Dot(vy);
            return tw;
        }
        /// <summary>
        /// </summary>
        /// 
        public double[] NLRegression(double[] vx, double[] vy, double[] w)
        {
            int maxIters = 1000, iter = 0;
            double lambda = 0.0001, minDeltaChi2 = 0.0001;
            int n = order + 1;
            double[] nw = (double[])w.Clone();
            double[] g = new double[n+1];
            double[,] tj = new double[n + 1, n + 1];
            double[] beta = new double[n+1];
            double mse = 0, lmse = 0;
            while (iter < maxIters)
            {
                mse = 0;
                for (int i = 0; i < vx.Length; i++)
                {
                    double y = Function(vx[i], nw);
                    for (int j = 0; j < order + 1; j++)
                        g[j] = Math.Pow(vx[i], j);
                    tj = tj.Add(g.Cov());
                    double dy = vy[i] - y;
                    mse += dy * dy;
                    beta = beta.Add(g.Multiply(dy));
                }
                //                tj.Divide(vx.Length);
                //                beta.Divide(vx.Length);
                for(int i=0;i<order+1;i++)
                    tj[i, i] *= (1.0 + lambda);
                tj = DoubleArrayExtensions.InvertSsgjUnsafeL(tj);
                if (tj == null)
                    break;
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
