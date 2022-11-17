using System.Drawing.Drawing2D;
using System.Text;
using Utilities;
namespace HVTracking
{
    public partial class HTrackingForm : Form
    {
        double[,] A, IRQT;
        int N =>(int)Num.Value;
        int order => (int)ORD.Value;

        double[] cpv, cnv, vpv, vnv, pang, nang;
        StringBuilder sbn = new StringBuilder();
        double[] cpw, cnw, vpw, vnw, vx, cpy, cny, vpy, vny;
        double[] pcpw, pcnw, pvpw, pvnw, pvx, pcpy, pcny, pvpy, pvny;
        //LeastSquares.Function.Polynomial q;
        float labelw = 22f;
        StringFormat sf = new StringFormat();
        float fontsize = 26f;
        Font font;
        Utilities.Timer Tm;
        bool running = false;
        int dt = 10;
        CancellationTokenSource cts = new CancellationTokenSource();
        Task task;
        AutoResetEvent autoEvent = new AutoResetEvent(false);

        public HTrackingForm()
        {
            InitializeComponent();
            font = new Font("Microsoft YaHei", fontsize, FontStyle.Regular, GraphicsUnit.World);
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            //q = new LeastSquares.Function.Polynomial(order);
            Init();
            ReadData();
            Tm = new Utilities.Timer
            {
                Period = dt,
            };
            Tm.Tick += Tm_Tick;
        }
        private void Tm_Tick(object sender, EventArgs e)
        {
            autoEvent.Set();
        }

        private void Simu_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            if (task == null)
            {
                task = new Task(Simulation, cts.Token);
                task.Start();
                Tm.Start();
                running = true;
                ((Button)sender).Text = "Pause";
                panel2.Enabled = false;
            }
            else
            {
                if (running)
                {
                    Tm.Stop();
                    running = false;
                    ((Button)sender).Text = "Start";
                    panel2.Enabled = true;
                }
                else
                {
                    Tm.Start();
                    running = true;
                    ((Button)sender).Text = "Pause";
                    panel2.Enabled = false;
                }
            }
        }
        void Simulation()
        {
            while (true)
            {
                DCount.Invoke(new EventHandler(delegate
                {
                    DCount.Value++;
                    if (DCount.Value + N >= DCount.Maximum)
                    {
                        DCount.Value = 0;
                        Simu_Click(Simu, null);
                    }
                }));
                Thread.Sleep(dt);
                autoEvent.WaitOne();
            }
        }
        void Init()
        {
            A = new double[N, order + 1];
            vx = new double[N];
            for (int i = 0; i < N; i++)
            {
                vx[i] = (i + 1.0) / 10.0;
                A[i, 0] = 1.0;
                for (int j = 1; j < order + 1; j++)
                    A[i, j] = A[i, j - 1] * vx[i];
            }
            double[,] R, Q;
            A.HouseholderDecomposition(out Q, out R);
            IRQT = R.InvUpperTRI().Dot(Q.Transpose());
            cpy = new double[N];
            cny = new double[N];
            vpy = new double[N];
            vny = new double[N];
            pcpy = new double[N];
            pcny = new double[N];
            pvpy = new double[N];
            pvny = new double[N];
            pcpy[N - 1] = 0;
            pcny[N - 1] = 0;
            pvpy[N - 1] = 0;
            pvny[N - 1] = 0;
        }
        private void DCount_ValueChanged(object sender, EventArgs e)
        {           

            int sc = (int)DCount.Value * sizeof(double);
            Buffer.BlockCopy(cpv, sc, cpy, 0, N * sizeof(double));
            Buffer.BlockCopy(cnv, sc, cny, 0, N * sizeof(double));
            Buffer.BlockCopy(vpv, sc, vpy, 0, N * sizeof(double));
            Buffer.BlockCopy(vnv, sc, vny, 0, N * sizeof(double));
            Buffer.BlockCopy(cpv, sc + sizeof(double), pcpy, 0, (N - 1) * sizeof(double));
            Buffer.BlockCopy(cnv, sc + sizeof(double), pcny, 0, (N-1) * sizeof(double));
            Buffer.BlockCopy(vpv, sc + sizeof(double), pvpy, 0, (N-1) * sizeof(double));
            Buffer.BlockCopy(vnv, sc + sizeof(double), pvny, 0, (N-1) * sizeof(double));
            cpy = cpy.Reverse().ToArray();
            cny = cny.Reverse().ToArray();
            vpy = vpy.Reverse().ToArray();
            vny = vny.Reverse().ToArray();
            pcpy = pcpy.Reverse().ToArray();
            pcny = pcny.Reverse().ToArray();
            pvpy = pvpy.Reverse().ToArray();
            pvny = pvny.Reverse().ToArray();
            //cpw = q.Regression2(vx, cpy);
            cpw = IRQT.Dot(cpy);
            cnw = IRQT.Dot(cny);
            vpw = IRQT.Dot(vpy);
            vnw = IRQT.Dot(vny);
            pcpw = IRQT.Dot(pcpy);
            pcnw = IRQT.Dot(pcny);
            pvpw = IRQT.Dot(pvpy);
            pvnw = IRQT.Dot(pvny);
            richTextBox1.Text = "\r\n正侧角度：" + pang[(int)DCount.Value + N].ToString("F4") + "\r\n负侧角度：" + nang[(int)DCount.Value + N].ToString("F4") + "\r\n";
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
            pnlCanvas1.Refresh();
            pnlCanvas2.Refresh();
        }

        private void ORD_ValueChanged(object sender, EventArgs e)
        {
            if (ORD.Value >= Num.Value)
                ORD.Value = Num.Value - 1;
            Init();
            DCount_ValueChanged(null, null);
        }

        private void HTrackingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tm.Stop();
            running = false;
            if (task != null)
            {
                cts.Cancel();
            }
        }

        StringBuilder sbp = new StringBuilder();
        int dd = -1;
        void ReadData()
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\DataFile\\" + "20220427 - 上午 -3   右式  双枪 T1 h.txt";
            //System.Text.RegularExpressions.Regex regex =  new System.Text.RegularExpressions.Regex(@"^\d+$");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^([0-9]{1,}[.][0-9]*)$");
            using (StreamReader sr = new StreamReader(fileName))
            {
                //using (TextWriter tw = new StreamWriter("t2a.csv"))
                {
                    List<double> lcpv = new List<double>();
                    List<double> lcnv = new List<double>();
                    List<double> lvpv = new List<double>();
                    List<double> lvnv = new List<double>();
                    List<double> lpang = new List<double>();
                    List<double> lnang = new List<double>();
                    string line, cstring, lasts = "", ang = "";
                    List<string[]> lineData = new List<string[]>();
                    int len = 0;
                    while ((line = sr.ReadLine()) != "")
                    {
                        string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int d = (int)float.Parse(parts[1]);
                        double tc = double.Parse(parts[2]);
                        double tv = double.Parse(parts[3]);
                        double c = double.Parse(parts[4]);
                        double v = double.Parse(parts[5]);
                        double dc = c - tc;
                        double dv = v - tv;
                        if (d == 1)
                        {
                            lcpv.Add(dc > 0 ? dc : 0);
                            lvpv.Add(dv > 0 ? 0 : dv);
                            lpang.Add(double.Parse(parts[0]));
                        }
                        else
                        {
                            lcnv.Add(dc > 0 ? dc : 0);
                            lvnv.Add(dv > 0 ? 0 : dv);
                            lnang.Add(double.Parse(parts[0]));
                        }
                        len++;
                    }
                    cpv = lcpv.ToArray();
                    vpv = lvpv.ToArray();
                    cnv = lcnv.ToArray();
                    vnv = lvnv.ToArray();
                    pang=lpang.ToArray();
                    nang=lnang.ToArray();

                    lcpv.Clear();
                    lcnv.Clear();
                    lvpv.Clear();
                    lvnv.Clear();
                    lpang.Clear();
                    lnang.Clear();

                    DCount.Maximum = Math.Min(cpv.Length, cnv.Length) - N;
                    DCount.Value = 0;
                    DCount_ValueChanged(null, null);
                }
            }
        }
        private BufferedGraphics bufferedGraphics1;
        private BufferedGraphics bufferedGraphics2;

        private void pnlCanvas1_Paint(object sender, PaintEventArgs e)
        {
            if (!this.IsDisposed && this.Visible)
            {
                BufferedGraphicsContext myContext = BufferedGraphicsManager.Current;
                bufferedGraphics1 = myContext.Allocate(e.Graphics, this.pnlCanvas1.ClientRectangle);
                if (bufferedGraphics1.Graphics != null)
                {
                    bufferedGraphics1.Graphics.Clear(Color.White);
                    this.DrawC(bufferedGraphics1.Graphics, this.pnlCanvas1.ClientRectangle);
                }
                bufferedGraphics1.Render();
                bufferedGraphics1.Dispose();
            }
        }
        private void pnlCanvas2_Paint(object sender, PaintEventArgs e)
        {
            if (!this.IsDisposed && this.Visible)
            {
                BufferedGraphicsContext myContext = BufferedGraphicsManager.Current;
                bufferedGraphics2 = myContext.Allocate(e.Graphics, this.pnlCanvas2.ClientRectangle);
                if (bufferedGraphics2.Graphics != null)
                {
                    bufferedGraphics2.Graphics.Clear(Color.White);
                    this.DrawV(bufferedGraphics2.Graphics, this.pnlCanvas2.ClientRectangle);
                }
                bufferedGraphics2.Render();
                bufferedGraphics2.Dispose();
            }
        }
        float margin = 20f;
        SolidBrush sb = new SolidBrush(Color.DarkBlue);
        private void DrawC(Graphics g, Rectangle rectangle)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            float pX = this.pnlCanvas1.ClientRectangle.Width / 2.0f - this.pnlCanvas1.ClientRectangle.Width / 6.0f;
            float nX = this.pnlCanvas1.ClientRectangle.Width / 2.0f + this.pnlCanvas1.ClientRectangle.Width / 6.0f;
            float tickX = (this.pnlCanvas1.ClientRectangle.Width / 3.0f - margin) / 40.0f;
            float tickY = (this.pnlCanvas1.ClientRectangle.Height-2*margin) / (N + 2.0f);
            float pY = margin;
            float nY = margin + tickY / 2.0f;
            using (Pen pen = new Pen(Color.DarkGreen, 2.5f))
            {
                g.DrawLine(pen, pX, pY, pX, this.pnlCanvas1.ClientRectangle.Height - margin);
                g.DrawLine(pen, nX, pY, nX, this.pnlCanvas1.ClientRectangle.Height - margin);
            }
            using (Pen pen = new Pen(Color.Red, 2.5f))
            {
                g.DrawLine(pen, pX - 5.0f * tickX, pY, pX - 5.0f * tickX, this.pnlCanvas1.ClientRectangle.Height - margin);
                g.DrawLine(pen, nX + 5.0f * tickX, pY, nX + 5.0f * tickX, this.pnlCanvas1.ClientRectangle.Height - margin);
            }
            Color c = Color.FromArgb(40, 0, 255, 0);
            RectangleF rc = new RectangleF(pX - 5.0f * tickX, margin, this.pnlCanvas1.ClientRectangle.Width / 3.0f + 10.0f * tickX, this.pnlCanvas1.ClientRectangle.Height - 2 * margin);
            g.FillRectangle(new SolidBrush(c),rc);
            sf.Alignment= StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            g.DrawString("电流", font, new SolidBrush(Color.DarkRed), rc, sf);

            PointF[] pps = new PointF[N + 1];
            PointF[] nps = new PointF[N + 1];
            PointF[] ppps = new PointF[N];
            PointF[] pnps = new PointF[N];

            pps[0] = new PointF((float)(pX - cpw[0] * tickX), margin + tickY);
            nps[0] = new PointF((float)(nX + cnw[0] * tickX), margin + 1.5f * tickY);

            double cp1 = 0, cn1 = 0;
            for(int i = 1;i<cpw.Length;i++)
            {
                cp1 -= i*cpw[i] * Math.Pow(0.1, i - 1);
                cn1 -= i*cnw[i] * Math.Pow(0.1, i - 1);
            }
            Rectangle pRect = new Rectangle((int)(pps[0].X - labelw * 2.0), (int)(pps[0].Y - font.Height- 3), (int)labelw * 4, font.Height);
            if(cpw[0]>5)
                g.DrawString((cpw[0]).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
            else
                g.DrawString((cpw[0]).ToString("F4"), font, sb, pRect, sf);

            pRect = new Rectangle((int)(nps[0].X - labelw * 2.0), (int)(nps[0].Y - font.Height - 3), (int)labelw * 4, font.Height);
            if (cnw[0] > 5)
                g.DrawString((cnw[0]).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
            else
                g.DrawString((cnw[0]).ToString("F4"), font, sb, pRect, sf);

            using (Pen pen = new Pen(Color.DarkRed, 2.5f), pen2 = new Pen(Color.YellowGreen, 2.5f), pen3 = new Pen(Color.IndianRed, 4.5f))
            {
                double[] ncpy = A.Dot(cpw);
                double[] ncny = A.Dot(cnw);
                double[] pncpy = A.Dot(pcpw);
                double[] pncny = A.Dot(pcnw);
                PointF[] ps = new PointF[N * 2];
                for (int i = 0; i < N; i++)
                {
                    PointF cp = new PointF((float)(pX - cpy[i] * tickX), (float)(margin + (i + 2.0) * tickY));
                    g.DrawEllipse(pen, cp.X - 4, cp.Y - 4, 8, 8);
                    PointF cn = new PointF((float)(nX + cny[i] * tickX), (float)(margin + (i + 2.5) * tickY));
                    g.DrawEllipse(pen, cn.X - 4, cn.Y - 4, 8, 8);


                    ps[i * 2] = cp;
                    ps[i * 2 + 1] = cn;
                    if (i == 0)
                    {
                        g.FillEllipse(new SolidBrush(Color.DeepPink), (float)(pps[0].X - 4), (float)(margin + tickY - 4), 8, 8);
                        g.FillEllipse(new SolidBrush(Color.DeepPink), (float)(nps[0].X - 4), (float)(margin + 1.5f * tickY - 4), 8, 8);

                        pRect = new Rectangle((int)(cp.X - labelw * 2.0), (int)(cp.Y + 3), (int)labelw * 4, font.Height);
                        g.DrawString((cp1).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                        pRect = new Rectangle((int)(cn.X - labelw * 2.0), (int)(cn.Y + 3), (int)labelw * 4, font.Height);
                        g.DrawString((cn1).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                    }
                    ppps[i] = new PointF((float)(pX - pncpy[i] * tickX), (float)(margin + (i + 1.0) * tickY));
                    pnps[i] = new PointF((float)(nX + pncny[i] * tickX), (float)(margin + (i + 1.5) * tickY));

                    pps[i + 1] = new PointF((float)(pX - ncpy[i] * tickX), (float)(margin + (i + 2.0) * tickY));
                    nps[i + 1] = new PointF((float)(nX + ncny[i] * tickX), (float)(margin + (i + 2.5) * tickY));
                    pRect = new Rectangle((int)(cp.X - labelw * 2.0), (int)(cp.Y - font.Height - 3), (int)labelw * 4, font.Height);
                    //g.DrawString((cpy[i]).ToString("F4"), font, sb, pRect, sf);
                    if (cpy[i] > 5)
                        g.DrawString((cpy[i]).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                    else
                        g.DrawString((cpy[i]).ToString("F4"), font, sb, pRect, sf);

                    pRect = new Rectangle((int)(cn.X - labelw * 2.0), (int)(cn.Y - font.Height - 3), (int)labelw * 4, font.Height);
                    //g.DrawString((cny[i]).ToString("F4"), font, sb, pRect, sf);
                    if (cny[i] > 5)
                        g.DrawString((cny[i]).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                    else
                        g.DrawString((cny[i]).ToString("F4"), font, sb, pRect, sf);

                }
                g.DrawLines(pen2, ps);
                pen3.CustomStartCap = myArrow;
                g.DrawCurve(pen3, pps);
                g.DrawCurve(pen3, nps);
                pen3.DashStyle = DashStyle.DashDot;
                g.DrawCurve(pen3, ppps);
                g.DrawCurve(pen3, pnps);

            }
        }
        AdjustableArrowCap myArrow = new AdjustableArrowCap(4, 4);
        private void DrawV(Graphics g, Rectangle rectangle)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            float pX = this.pnlCanvas2.ClientRectangle.Width / 2.0f - this.pnlCanvas2.ClientRectangle.Width / 6.0f;
            float nX = this.pnlCanvas2.ClientRectangle.Width / 2.0f + this.pnlCanvas2.ClientRectangle.Width / 6.0f;
            float tickX = (this.pnlCanvas2.ClientRectangle.Width / 3.0f - margin) / 20.0f;
            float tickY = (this.pnlCanvas2.ClientRectangle.Height - 2 * margin) / (N + 2.0f);
            float pY = margin;
            float nY = margin + tickY / 2.0f;
            using (Pen pen = new Pen(Color.DarkGreen, 2.5f))
            {
                g.DrawLine(pen, pX, pY, pX, this.pnlCanvas1.ClientRectangle.Height - margin);
                g.DrawLine(pen, nX, pY, nX, this.pnlCanvas1.ClientRectangle.Height - margin);
            }
            using (Pen pen = new Pen(Color.Red, 2.5f))
            {
                g.DrawLine(pen, pX - 2.0f * tickX, pY, pX - 2.0f * tickX, this.pnlCanvas1.ClientRectangle.Height - margin);
                g.DrawLine(pen, nX + 2.0f * tickX, pY, nX + 2.0f * tickX, this.pnlCanvas1.ClientRectangle.Height - margin);
            }
            Color c = Color.FromArgb(40,0,255,0);
            RectangleF rc = new RectangleF(pX - 2.0f * tickX, margin, this.pnlCanvas2.ClientRectangle.Width / 3.0f + 4.0f * tickX, this.pnlCanvas2.ClientRectangle.Height - 2 * margin);
            g.FillRectangle(new SolidBrush(c), rc);
            g.FillRectangle(new SolidBrush(c), rc);
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            g.DrawString("电压", font, new SolidBrush(Color.DarkRed), rc, sf);

            PointF[] pps = new PointF[N + 1];
            PointF[] nps = new PointF[N + 1];
            PointF[] ppps = new PointF[N];
            PointF[] pnps = new PointF[N];


            pps[0] = new PointF((float)(pX + vpw[0] * tickX), margin + tickY);
            nps[0] = new PointF((float)(nX - vnw[0] * tickX), margin + 1.5f * tickY);

            double vp1 = 0, vn1 = 0;
            for (int i = 1; i < cpw.Length; i++)
            {
                vp1 += i * vpw[i] * Math.Pow(0.1, i - 1);
                vn1 += i * vnw[i] * Math.Pow(0.1, i - 1);
            }

            Rectangle pRect = new Rectangle((int)(pps[0].X - labelw * 2.0), (int)(pps[0].Y - font.Height - 3), (int)labelw * 4, font.Height);
            //g.DrawString((vpw[0]/10.0).ToString("F4"), font, sb, pRect, sf);
            if (vpw[0] <= -2)
                g.DrawString((vpw[0]/10.0).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
            else
                g.DrawString((vpw[0]/10.0).ToString("F4"), font, sb, pRect, sf);

            pRect = new Rectangle((int)(nps[0].X - labelw * 2.0), (int)(nps[0].Y - font.Height - 3), (int)labelw * 4, font.Height);
            //g.DrawString((vnw[0]/10.0).ToString("F4"), font, sb, pRect, sf);
            if (vnw[0] <= -2)
                g.DrawString((vnw[0] / 10.0).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
            else
                g.DrawString((vnw[0] / 10.0).ToString("F4"), font, sb, pRect, sf);

            using (Pen pen = new Pen(Color.DarkRed, 2.5f), pen2 = new Pen(Color.YellowGreen, 2.5f), pen3 = new Pen(Color.IndianRed, 4.5f))
            {
                double[] nvpy = A.Dot(vpw);
                double[] nvny = A.Dot(vnw);
                double[] pnvpy = A.Dot(pvpw);
                double[] pnvny = A.Dot(pvnw);

                PointF[] ps = new PointF[N * 2];
                for (int i = 0; i < N; i++)
                {
                    PointF cp = new PointF((float)(pX + vpy[i] * tickX), (float)(margin + (i + 2.0) * tickY));
                    g.DrawEllipse(pen, cp.X - 4, cp.Y - 4, 8, 8);
                    PointF cn = new PointF((float)(nX - vny[i] * tickX), (float)(margin + (i + 2.5) * tickY));
                    g.DrawEllipse(pen, cn.X - 4, cn.Y - 4, 8, 8);
                    ps[i * 2] = cp;
                    ps[i * 2 + 1] = cn;
                    if (i == 0)
                    {
                        g.FillEllipse(new SolidBrush(Color.DeepPink), (float)(pps[0].X - 4), (float)(margin + tickY - 4), 8, 8);
                        g.FillEllipse(new SolidBrush(Color.DeepPink), (float)(nps[0].X - 4), (float)(margin + 1.5f * tickY - 4), 8, 8);

                        pRect = new Rectangle((int)(cp.X - labelw * 2.0), (int)(cp.Y + 3), (int)labelw * 4, font.Height);
                        g.DrawString((vp1).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                        pRect = new Rectangle((int)(cn.X - labelw * 2.0), (int)(cn.Y + 3), (int)labelw * 4, font.Height);
                        g.DrawString((vn1).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                    }
                    pps[i + 1] = new PointF((float)(pX + nvpy[i] * tickX), (float)(margin + (i + 2.0) * tickY));
                    nps[i + 1] = new PointF((float)(nX - nvny[i] * tickX), (float)(margin + (i + 2.5) * tickY));
                    ppps[i] = new PointF((float)(pX + pnvpy[i] * tickX), (float)(margin + (i + 1.0) * tickY));
                    pnps[i] = new PointF((float)(nX - pnvny[i] * tickX), (float)(margin + (i + 1.5) * tickY));

                    pRect = new Rectangle((int)(cp.X - labelw * 2.0), (int)(cp.Y - font.Height - 3), (int)labelw * 4, font.Height);
                    //g.DrawString((vpy[i]/10.0).ToString("F4"), font, sb, pRect, sf);
                    if (vpy[i] <= -2)
                        g.DrawString((vpy[i] / 10.0).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                    else
                        g.DrawString((vpy[i] / 10.0).ToString("F4"), font, sb, pRect, sf);

                    pRect = new Rectangle((int)(cn.X - labelw * 2.0), (int)(cn.Y - font.Height - 3), (int)labelw * 4, font.Height);
                    //g.DrawString((vny[i]/10.0).ToString("F4"), font, sb, pRect, sf);
                    if (vny[i] <= -2)
                        g.DrawString((vny[i] / 10.0).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                    else
                        g.DrawString((vny[i] / 10.0).ToString("F4"), font, sb, pRect, sf);
                }
                g.DrawLines(pen2, ps);
                pen3.CustomStartCap = myArrow;
                g.DrawCurve(pen3, pps);
                g.DrawCurve(pen3, nps);
                pen3.DashStyle = DashStyle.DashDot;
                g.DrawCurve(pen3, ppps);
                g.DrawCurve(pen3, pnps);
            }
        }
        private void pnlCanvas1_SizeChanged(object sender, EventArgs e)
        {
            this.pnlCanvas1.Refresh();
        }
        private void pnlCanvas2_SizeChanged(object sender, EventArgs e)
        {
            this.pnlCanvas2.Refresh();
        }
    }
}