using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    public abstract class ControlAnimation : IDisposable
    {
        protected Timer m_Timer;
        private Control m_Target;
        private float m_Time;
        private float m_ElspTime;
        private bool m_Start = false;
        private Stopwatch m_sw;
        public static ControlAnimation CreateAcceAnimation(Point from, Point to, float t) {
            AcceAnimation ani = new AcceAnimation();
            ani.From = from;
            ani.To = to;
            ani.m_Time = t;

            return ani;
        }

        protected abstract void StartExecute(Control target);
        protected abstract void Execute(Control target, float deltaT);
        protected abstract void EndExecute(Control target);
        public void Start(Control target) {
            m_Timer = new Timer();
            m_Timer.Interval = 30;
            m_Target = target;
            m_Start = false;
            m_Timer.Tick += new EventHandler(m_Timer_Tick);
            m_Timer.Start();

            //m_ElspTime = m_sw.ElapsedMilliseconds;
        }
        public float Time {
            get {
                return m_Time;
            }
            set {
                m_Time = value;
            }
        }
        void m_Timer_Tick(object sender, EventArgs e) {
            float dt = 0;


            if (m_ElspTime >= m_Time) {
                m_Timer.Stop();
                EndExecute(m_Target);
                m_Start = false;
                this.Dispose();
                return;
            }
            else if (!m_Start) {
                m_Start = true;
                StartExecute(m_Target);
                m_sw = Stopwatch.StartNew();

            }
            else {
                dt = m_sw.ElapsedMilliseconds - m_ElspTime;
                Execute(m_Target, dt);

            }
            m_ElspTime = m_sw.ElapsedMilliseconds;
            //m_ElspTime += m_Timer.Interval;
        }



        #region IDisposable 成员

        public virtual void Dispose() {
            if (m_Timer != null) {
                m_Timer.Dispose();
                m_Timer = null;
            }
            m_Target = null;
        }

        #endregion
    }


    public class AcceAnimation : ControlAnimation
    {
        internal Point From;
        internal Point To;

        float m_v = 0f;
        float m_at;//加速的时间
        float m_st;//减速的时间
        float m_alen;//加速的长度
        float m_slen;//减速的长度
        float m_aa;//加速度
        float m_sa;//加速度(后半段)

        float m_esp;
        bool m_inS = false;//减速阶段
        protected override void Execute(Control target, float deltaT) {
            //m_esp+=deltaT;
            //float ds;
            //if (m_esp < m_at) {
            //    ds = 0.5f * m_aa * m_esp * m_esp * 0.000001f; //1/2*a*t*t
            //    m_v=m_aa*m_esp*0.001f;
            //}
            //else {
            //    if(!m_inS){
            //        m_esp=deltaT; //开始减速
            //        m_inS=true;
            //        m_sa = 2 * (m_slen - m_v * m_st) / (m_st * m_st * 0.000001f);
            //    }

            //    ds = m_v * m_esp*0.001f + 0.5f * m_sa * m_esp * m_esp* 0.000001f;
            //}

            m_esp += deltaT;
            float s = m_v * m_esp * 0.001f;
            //Mogre.Vector3 p = ray.GetPoint(s);
            //target.Location = new Point((int)p.x, (int)p.y);
            //Debug.WriteLine(m_esp);
        }
        //Mogre.Ray ray;
        protected override void StartExecute(Control target) {
            m_at = 2 * this.Time / 3;
            m_st = this.Time / 3;


            int x = To.X - From.X;
            int y = To.Y - From.Y;
            float len = (float)Math.Sqrt(x * x + y * y);
            m_alen = 2 * len / 3;
            m_slen = len / 3;

            m_aa = 2 * m_alen / (m_at * 0.001f * m_at * 0.001f); //加速度
            m_sa = 2 * m_slen / (m_st * 0.001f * m_st * 0.001f);

            m_v = len / (this.Time * 0.001f);
            //ray = new Mogre.Ray(new Mogre.Vector3((float)From.X, (float)From.Y, 0), (new Mogre.Vector3((float)x, (float)y, 0)).NormalisedCopy);

        }
        protected override void EndExecute(Control target) {
            target.Location = To;
        }

        internal AcceAnimation() { }
    }
}
