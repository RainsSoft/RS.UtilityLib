using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Test.EmbedForm
{
    /// <summary>
    /// Process上下文
    /// </summary>
    public class ProcessContext : IDisposable
    {
        public ProcessContext(IHasProcess owner) {
            m_owner = owner;
        }
        private IHasProcess m_owner;
        /// <summary>
        /// 是否已经触发过ok事件
        /// </summary>
        private bool m_proc_hastrigge_ok = false;
        /// <summary>
        /// 是否已经触发过exit事件
        /// </summary>
        private bool m_proc_hastrigge_exit = false;
        /// <summary>
        /// 进程启动参数信息
        /// </summary>
        public ProcessStartInfo StartInfo {
            get;
            set;
        }
        /// <summary>
        /// 关联进程
        /// </summary>
        public Process Proc {
            get;
            set;
        }
        /// <summary>
        /// 进程ID
        /// </summary>
        public int PrcoId {
            get;
            set;
        }
        /// <summary>
        /// 进程名称
        /// </summary>
        public string Name {
            get;
            set;
        }
        /// <summary>
        /// 是否已经开始检查进程状态
        /// </summary>
        public bool StartCheckProcState {
            get;
            set;
        }
        /// <summary>
        /// 如果在非主线程中调用,
        /// </summary>
        public void OnUpdate() {
            if (this.StartCheckProcState == false) {
                return;
            }
            if (this.Proc != null) {
                if (this.Proc.HasExited == false) {
                    if (m_proc_hastrigge_ok == false && ProcessMainWinHandleHelper.GetMainWindowHandle(this.Proc) != IntPtr.Zero) {
                        m_proc_hastrigge_ok = true;
                        //DebugLog.Log("ProcessContext:" + this.Name + " OnOk");
                        HasProcessHelper.OnOk(m_owner);
                    }
                }
                else {
                    if (m_proc_hastrigge_exit == false) {
                        //DebugLog.Log("ProcessContext:" + this.Name + " OnExit");
                        m_proc_hastrigge_exit = true;
                        HasProcessHelper.OnExit(m_owner);
                    }
                    this.Proc = null;
                }
            }
            else {
                //DebugLog.Log("ProcessContext:" + this.Name + "  Proc Null");
            }
        }
        /// <summary>
        /// 关闭进程树并通知HasProcessHelper
        /// </summary>
        public void Dispose() {
            if (StartCheckProcState
                //&& m_proc_hastrigge_exit == false
                && this.PrcoId != 0
                ) {
                //DebugLog.Log("ProcessContext:" + this.Name + " OnExit");
                try {
                    ProcessKillHelper.KillTree(this.PrcoId, true);
                }
                catch {
                }
                if (m_proc_hastrigge_exit == false) {
                    //触发退出事件
                    m_proc_hastrigge_exit = true;
                    HasProcessHelper.OnExit(m_owner);
                }
            }
            this.Proc = null;
            this.StartCheckProcState = false;
            // HasProcessHelper.Add(this);

        }


    }
}
