using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeastSquares
{
    public interface IFunction
    {
        /// <summary>
        ///   The Common Function Regression
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        double[] Regression(double[] x, double[] y);
        /// <summary>
        ///   The NON-Linear Least Squares Regression
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <param name="w">Vector <c>y</c> in input space.</param>
        double[] NLRegression(double[] x, double[] y,double[] w);
        /// <summary>
        ///   The Common function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="w">Vector <c>w</c> in input space.</param>
        /// 
        double Function(double x, double[] w);
    }
}
