using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
namespace LeastSquares.Function
{
    public struct Gauss : IFunction
    {

        /// <summary>
        /// </summary>
        /// 
        public double[] Regression(double[] vx,double[] vy)
        {
            int n = vx.Length;
            double a = 0, b = 0, c = 0, u = 0;
            double[,] M = new double[4, 4];
            double[] V = new double[4];
            double[] S = new double[n];
            double[] T = new double[n];
            double x1 = vx[0];
            double y1 = vy[0];
            for (int i = 0; i < n; i++)
            {
                double x = vx[i];
                double y = vy[i];
                double xx = x * x;
                double xx1 = x1 * x1;
                double dx1 = x - x1;
                double dy1 = y - y1;
                double dxx1 = xx - xx1;
                double xy = x * y;
                if (i > 0)
                {
                    double lx = vx[i - 1];
                    double ly = vy[i - 1];
                    double dx = (x - lx);
                    S[i] = S[i - 1] + 0.5 * (y + ly) * dx;
                    T[i] = T[i - 1] + 0.5 * (x * y + lx * ly) * dx;
                    V[0] += S[i] * dy1;
                    V[1] += T[i] * dy1;
                    V[2] += dxx1 * dy1;
                    V[3] += dx1 * dy1;
                }
                M[0, 0] += S[i] * S[i];
                M[0, 1] += S[i] * T[i];
                M[0, 2] += S[i] * dxx1;
                M[0, 3] += S[i] * dx1;
                M[1, 1] += T[i] * T[i];
                M[1, 2] += T[i] * dxx1;
                M[1, 3] += T[i] * dx1;
                M[2, 2] += dxx1 * dxx1;
                M[2, 3] += dxx1 * dx1;
                M[3, 3] += dx1 * dx1;
            }
            M[1, 0] = M[0, 1];
            M[2, 0] = M[0, 2];
            M[3, 0] = M[0, 3];
            M[2, 1] = M[1, 2];
            M[3, 1] = M[1, 3];
            M[3, 2] = M[2, 3];

            double[] pTmp = new double[4];
            for (int k = 0; k < 4; k++)
            {
                double w = M[0, 0];
                if (w == 0)
                    return null;
                int m = 4 - k - 1;
                for (int i = 1; i < 4; i++)
                {
                    double g = M[i, 0];
                    pTmp[i] = g / w;
                    if (i <= m)
                        pTmp[i] = -pTmp[i];
                    for (int j = 1; j <= i; j++)
                        M[(i - 1), j - 1] = M[i, j] + g * pTmp[j];
                }

                M[3, 3] = 1.0 / w;
                for (int i = 1; i < 4; i++)
                    M[3, i - 1] = pTmp[i];
            }
            for (int i = 0; i < 3; i++)
                for (int j = i + 1; j < 4; j++)
                    M[i, j] = M[j, i];

            double MV0 = M[0, 0] * V[0] + M[0, 1] * V[1] + M[0, 2] * V[2] + M[0, 3] * V[3];
            double MV1 = M[1, 0] * V[0] + M[1, 1] * V[1] + M[1, 2] * V[2] + M[1, 3] * V[3];
            u = -MV0 / MV1;
            c = -1 / MV1;
            double[] tk = new double[n];
            double[,] M2 = new double[2, 2];
            double[,] V2 = new double[2, 1];
            for (int i = 0; i < n; i++)
            {
                double g = Math.Exp(-0.5 * Math.Pow(Math.Abs(vx[i] - u), 2) / c);
                M2[0, 0] += 1;
                M2[0, 1] += g;
                M2[1, 0] += g;
                M2[1, 1] += g * g;
                V2[0, 0] += vy[i];
                V2[1, 0] += g * vy[i];
            }
            double detM2 = M2[0, 0] * M2[1, 1] - M2[0, 1] * M2[1, 0];
            a = (M2[1, 1] * V2[0, 0] - M2[0, 1] * V2[1, 0]) / detM2;
            b = (-M2[1, 0] * V2[0, 0] + M2[0, 0] * V2[1, 0]) / detM2;
            return new double[] { a, b, c, u };
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
            double[,] tj = new double[4,4];
            double[] ty = new double[4];
            double mse = 0, lmse = 0;
            while (iter < maxIters)
            {
                mse = 0;
                for (int i = 0; i < vx.Length; i++)
                {
                    double y = Function(vx[i], nw);
                    double tmp = Math.Exp(-0.5 * Math.Pow((vx[i] - nw[3]), 2) / nw[2]);
                    g = new double[4] { 1, tmp, nw[1] * tmp * 0.5 * Math.Pow((vx[i] - nw[3]), 2) / (nw[2] * nw[2]), nw[1] * tmp * ((vx[i] - nw[3])) / (nw[2]) };
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
                nw = nw.Add(Utilities.DoubleArrayExtensions.InvertSsgjUnsafeU(tj).Dot(ty));

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
            //    y = a + b·exp(-½ (x - u)² / c)
            return w[0] + w[1] * Math.Exp(-0.5 * Math.Pow(Math.Abs(x - w[3]), 2) / w[2]);
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
