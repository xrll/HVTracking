using System.Drawing.Drawing2D;
using System.Text;
using Utilities;
namespace HVTracking
{
    public partial class VTrackingForm : Form
    {

        PID pidC,pidV;
        double[]? cv,scv, vv,svv, ang;
        StringBuilder sbn = new StringBuilder();

        float labelw = 22f;
        StringFormat sf = new StringFormat();
        float fontsize = 26f;
        Font font;
        Utilities.Timer Tm;
        bool running = false;
        int dt = 100;
        CancellationTokenSource cts = new CancellationTokenSource();
        Task? task;
        AutoResetEvent autoEvent = new AutoResetEvent(false);

        public VTrackingForm()
        {
            InitializeComponent();
            font = new Font("Microsoft YaHei", fontsize, FontStyle.Regular, GraphicsUnit.World);
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            pidC = new PID();
            pidV = new PID();
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
                DCount.Enabled = false;
            }
            else
            {
                if (running)
                {
                    Tm.Stop();
                    running = false;
                    ((Button)sender).Text = "Start";
                    DCount.Enabled = true;
                }
                else
                {
                    Tm.Start();
                    running = true;
                    ((Button)sender).Text = "Pause";
                    DCount.Enabled = false;
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
                    if (DCount.Value >= DCount.Maximum)
                    {
                        DCount.Value = 0;
                        Simu_Click(Simu, null);
                    }
                }));
                Thread.Sleep(dt);
                autoEvent.WaitOne();
            }
        }
        private void DCount_ValueChanged(object sender, EventArgs e)
        {           

            int sc = (int)DCount.Value;
            pidC.sp = scv[sc];
            pidC.pv = cv[sc];
            pidC.Compute();
            pidV.sp = svv[sc];
            pidV.pv = vv[sc];
            pidV.Compute();
            richTextBox1.Text = "\r\n ½Ç¶È£º" + ang![sc].ToString("F4") + "\r\n dc £º" + (cv[sc]-scv[sc]).ToString("F4") + "\r\n" + "\r\n dc £º" + (vv[sc] - svv[sc]).ToString("F4") + "\r\n";
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.ScrollToCaret();
            pnlCanvas1.Refresh();
            pnlCanvas2.Refresh();
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
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\DataFile\\" + "20220427 - ÉÏÎç -3   ÓÒÊ½  Ë«Ç¹ T1 h.txt";
            //System.Text.RegularExpressions.Regex regex =  new System.Text.RegularExpressions.Regex(@"^\d+$");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^([0-9]{1,}[.][0-9]*)$");
            using (StreamReader sr = new StreamReader(fileName))
            {
                //using (TextWriter tw = new StreamWriter("t2a.csv"))
                {
                    List<double> lcv = new List<double>();
                    List<double> lvv = new List<double>();
                    List<double> lscv = new List<double>();
                    List<double> lsvv = new List<double>();
                    List<double> lang = new List<double>();
                    string? line;
                    List<string[]> lineData = new List<string[]>();
                    int len = 0;
                    while ((line = sr.ReadLine()) != "")
                    {
                        string[] parts = line!.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        double tc = double.Parse(parts[2]);
                        double tv = double.Parse(parts[3]);
                        double c = double.Parse(parts[4]);
                        double v = double.Parse(parts[5]);
                        lcv.Add(c);
                        lscv.Add(tc);
                        lvv.Add(v);
                        lsvv.Add(tv);

                        lang.Add(double.Parse(parts[0]));
                        len++;
                    }
                    cv = lcv.ToArray();
                    vv = lvv.ToArray();
                    scv = lscv.ToArray();
                    svv = lsvv.ToArray();
                    ang=lang.ToArray();

                    lcv.Clear();
                    lvv.Clear();
                    lscv.Clear();
                    lsvv.Clear();
                    lang.Clear();

                    DCount.Maximum = cv.Length-1;
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
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            Rectangle rect = new Rectangle(50, 50, 500, font.Height);
            string s = "kp:  "+pidC.p_kp.ToString("F3")+";  Ti:  "+pidC.p_ti.ToString("F3")+";   Td:  "+pidC.p_td.ToString("F3");
            g.DrawString(s, font, new SolidBrush(Color.PaleVioletRed), rect, sf);

            float pX = this.pnlCanvas1.ClientRectangle.Width / 4.0f;
            float pY = this.pnlCanvas1.ClientRectangle.Height / 2.0f;
            float tickY = (this.pnlCanvas1.ClientRectangle.Height * 2.0f / 3.0f) / 80f;
            using (Pen pen = new Pen(Color.Blue, 5.5f), pen2 = new Pen(Color.DarkRed, 3.5f),pen3 = new Pen(Color.DarkGreen, 2.5f), pen4 = new Pen(Color.DarkOliveGreen, 2.5f))
            {
                g.DrawLine(pen, pX-50f, pY, pX+50, pY);
                double dc = (pidC.pv - pidC.sp);
                float cY = (float)(pY - dc * tickY);
                if(dc!=0)
                    g.DrawLine(pen2, pX - 50f,cY , pX + 50, cY);
                Rectangle pRect = new Rectangle((int)(pX - 50f), (int)(cY - font.Height - 3), 100, font.Height);
                if(dc>=0)
                    g.DrawString(dc.ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                else
                {
                    pRect = new Rectangle((int)(pX - 50f), (int)(cY + 3), 100, font.Height);
                    g.DrawString(dc.ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                }
                pen2.CustomStartCap = myArrow;
                pen2.CustomEndCap = myArrow;
                g.DrawLine(pen2, pX, pY, pX, cY);

                g.DrawLine(pen3, pX * 2.0f - 50f, pY, pX * 2.0f + 50, pY);
                g.DrawLine(pen3, pX * (2.0f + 2.0f / 3.0f) - 50f, pY, pX * (2.0f + 2.0f / 3.0f) + 50, pY);
                g.DrawLine(pen3, pX * (2.0f + 4.0f / 3.0f) - 50f, pY, pX * (2.0f + 4.0f / 3.0f) + 50f, pY);
                pRect = new Rectangle((int)(pX * 2.0f - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidC.p_xout_p).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * 2.0f - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("P_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                pRect = new Rectangle((int)(pX * (2.0f + 2.0f / 3.0f) - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidC.p_xout_i).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * (2.0f + 2.0f / 3.0f) - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("I_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                pRect = new Rectangle((int)(pX * (2.0f + 4.0f / 3.0f) - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidC.p_xout_d).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * (2.0f + 4.0f / 3.0f) - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("D_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
            }
            
        }
        AdjustableArrowCap myArrow = new AdjustableArrowCap(4, 4);
        private void DrawV(Graphics g, Rectangle rectangle)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            Rectangle rect = new Rectangle(50, 50, 500, font.Height);
            string s = "kp:  " + pidV.p_kp.ToString("F3") + ";  Ti:  " + pidV.p_ti.ToString("F3") + ";   Td:  " + pidV.p_td.ToString("F3");
            g.DrawString(s, font, new SolidBrush(Color.PaleVioletRed), rect, sf);

            float pX = this.pnlCanvas2.ClientRectangle.Width / 4.0f;
            float pY = this.pnlCanvas2.ClientRectangle.Height / 2.0f;
            float tickY = (this.pnlCanvas2.ClientRectangle.Height * 2.0f / 3.0f) / 80f;
            using (Pen pen = new Pen(Color.Blue, 5.5f), pen2 = new Pen(Color.DarkRed, 3.5f), pen3 = new Pen(Color.DarkGreen, 2.5f), pen4 = new Pen(Color.DarkOliveGreen, 2.5f))
            {
                g.DrawLine(pen, pX - 50f, pY, pX + 50, pY);
                double dc = (pidV.pv - pidV.sp);
                float cY = (float)(pY - dc * tickY);
                if (dc != 0)
                    g.DrawLine(pen2, pX - 50f, cY, pX + 50, cY);
                Rectangle pRect = new Rectangle((int)(pX - 50f), (int)(cY - font.Height - 3), 100, font.Height);
                if (dc >= 0)
                    g.DrawString(dc.ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                else
                {
                    pRect = new Rectangle((int)(pX - 50f), (int)(cY + 3), 100, font.Height);
                    g.DrawString(dc.ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                }
                pen2.CustomStartCap = myArrow;
                pen2.CustomEndCap = myArrow;
                g.DrawLine(pen2, pX, pY, pX, cY);

                g.DrawLine(pen3, pX * 2.0f - 50f, pY, pX * 2.0f + 50, pY);
                g.DrawLine(pen3, pX * (2.0f + 2.0f / 3.0f) - 50f, pY, pX * (2.0f + 2.0f / 3.0f) + 50, pY);
                g.DrawLine(pen3, pX * (2.0f + 4.0f / 3.0f) - 50f, pY, pX * (2.0f + 4.0f / 3.0f) + 50f, pY);
                pRect = new Rectangle((int)(pX * 2.0f - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidV.p_xout_p).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * 2.0f - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("P_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                pRect = new Rectangle((int)(pX * (2.0f + 2.0f / 3.0f) - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidV.p_xout_i).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * (2.0f + 2.0f / 3.0f) - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("I_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);

                pRect = new Rectangle((int)(pX * (2.0f + 4.0f / 3.0f) - 50f), (int)(pY - font.Height - 3), 100, font.Height);
                g.DrawString((pidV.p_xout_d).ToString("F4"), font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
                pRect = new Rectangle((int)(pX * (2.0f + 4.0f / 3.0f) - 50f), (int)(pY + 3), 100, font.Height);
                g.DrawString("D_Out", font, new SolidBrush(Color.PaleVioletRed), pRect, sf);
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