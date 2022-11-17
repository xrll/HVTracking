using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVTracking
{
    internal class PID
    {

        int tflag = 8;//跟踪标志
        //输入误差范围（单位克）
        double p_xerr_min = -40;
        double p_xerr_xmax = 40;
        //输出
        double p_xout_min = -8;
        double p_xout_max = 8;
        public double p_kp
        {
            set;
            get;
        } = 0.6;//    p_xout_max/p_xerr_xmax
        public double p_ti
        {
            set;
            get;
        } = 20.0;
        public double p_td
        {
            set;
            get;
        } = 0.03;//    kp/5

        bool p_i_itl_on = true;
        double p_i_itlval = -0.05;
        public PID()
        {
            /*
            p_kp = p_xout_max / p_xerr_xmax;
             p_td = p_kp / 12.0;
 //          p_td = p_ti / 40.0;
            */
            p_kp = p_xout_max / p_xerr_xmax*1.2;
            p_ti = 80;
            p_td = p_ti / 4.0;
        }
        public double pv
        {
            set;
            get;
        }
        public double sp
        {
            set;
            get;
        }
        public double p_out
        {
            set;
            get;
        }
        public double p_xout_p
        {
            set;
            get;
        }
        public double p_xout_i
        {
            set;
            get;
        }
        public double p_xout_d
        {
            set;
            get;
        }
        double l_pv = 0, l_e = 0, p_xout = 0, l_p = 0, l_i = 0, l_d = 0;
        public void Compute()
        {
            double p_pv = pv;//MEASUREMENT VALUE
            double dc = sp - p_pv;
            double e = dc;
            //if (p_pv <= l_pv)
            //    tflag++;
            //else
            //    tflag = 0;
            l_pv = p_pv;


            if (e > p_xerr_xmax)
            {
                e = p_xerr_xmax;
            }
            else if(Math.Abs(e)<=1.0)
            {
                p_xout = 0;
                return;
            }
            //if (e < 50)
            //    p_kp /= 1.06;//在50以下减少KP
            //p_td = p_kp / 20.0;
            //if (tflag > 8)
            //{
            //    //Stop
            //}

            p_xout_p = p_kp * e;
            p_xout_d = -p_td * p_kp * (e - l_e);
            p_xout_i = l_i + p_kp * e / p_ti;

            /*
            if (p_ti != 0)
            {
                double tout = p_xout_p + p_xout_d;
                double rdiff = 0.02 / p_ti * (p_xout_p * 0.6 + 0.4 * l_p);
                if ((rdiff > 0 && tout >= p_xout_max) || (rdiff < 0 && tout <= p_xout_min))
                    rdiff = 0;
                p_xout_i = l_i + rdiff;
            }
            else
            {
                if (p_i_itl_on)
                    p_xout_i = p_i_itlval;
                else
                    p_xout_i = 0;
            }*/
            p_xout = (p_xout_p + p_xout_i + p_xout_d);
            if (p_xout >= p_xout_max)
                p_xout = p_xout_max;
            else if (p_xout <= p_xout_min)
                p_xout = p_xout_min;
            p_out = p_xout;
            l_e = e;
            l_i = p_xout_i;
            l_p = p_xout_p;
        }
    }
}
