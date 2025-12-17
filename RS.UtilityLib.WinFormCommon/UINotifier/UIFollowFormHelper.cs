using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace RS.UtilityLib.WinFormCommon.UINotifier
{
    public enum UIFollowMode
    {
        None,
        /// <summary>
        /// 相对
        /// </summary>
        Offset,
        /// <summary>
        /// 右侧跟随
        /// </summary>
        FixedRight,
        /// <summary>
        /// 下方跟随
        /// </summary>
        FixedDown
    }
    /// <summary>
    /// 要求子窗体与主窗体在一个线程环境
    /// </summary>
    public class UIFollowFormHelper : IDisposable
    {
        private Form m_frmOwner;
        private Form m_frmChildFollow;
        /// <summary>
        /// 窗口初始大小
        /// </summary>
        private Size m_frmChildStartSize;
        private Point m_frmChildStartLocation;
        private UIFollowMode m_followMode = UIFollowMode.Offset;
        private int m_followDistanceX = 0;
        private int m_followDistanceY = 0;
        private Point m_ownerFrm_PrevPos;
        private bool m_noNeedFollow = false;
        private bool m_inFollowing = false;
        private bool m_AutoDisposed = false;
        public UIFollowFormHelper(Form owner, Form childfollow, int followDistanceX = 0, int followDistanceY = 0, UIFollowMode followMode = UIFollowMode.Offset, bool autoDispose = true) {
            System.Diagnostics.Debug.Assert(owner != null && owner.IsDisposed == false && owner.IsHandleCreated);
            System.Diagnostics.Debug.Assert(childfollow != null && childfollow.IsDisposed == false);
            this.m_frmOwner = owner;
            this.m_frmChildFollow = childfollow;
            this.m_AutoDisposed = autoDispose;
            //
            this.m_followMode = followMode;
            this.m_followDistanceX = followDistanceX;
            this.m_followDistanceY = followDistanceY;
            this.m_frmChildFollow = childfollow;
            //         
            //setFollowOwnerFrm(owner);
            {
                m_noNeedFollow = true;
                //this.Owner = ownerFrm;
                this.m_ownerFrm_PrevPos = this.m_frmOwner.Location;
            }
            //           
            HookEvents();
        }
        private void HookEvents() {
            this.m_frmOwner.LocationChanged -= frmOwner_LocationChanged;
            this.m_frmOwner.LocationChanged += frmOwner_LocationChanged;
            this.m_frmOwner.Resize -= frmOwner_Resize;
            this.m_frmOwner.Resize += frmOwner_Resize;
            this.m_frmChildFollow.Shown -= frmChildFollow_Shown;
            this.m_frmChildFollow.Shown += frmChildFollow_Shown;
            this.m_frmChildFollow.Disposed -= frmChildFollow_Disposed;
            this.m_frmChildFollow.Disposed += frmChildFollow_Disposed;
            //
            this.m_frmOwner.AddOwnedForm(this.m_frmChildFollow);
        }

        private void UnHookEvents() {
            if (m_frmOwner.CheckHasCreatedControl()) {
                this.m_frmOwner.LocationChanged -= frmOwner_LocationChanged;
                this.m_frmOwner.Resize -= frmOwner_Resize;
            }
            if (m_frmChildFollow.CheckHasCreatedControl()) {
                this.m_frmChildFollow.Shown -= frmChildFollow_Shown;
                this.m_frmChildFollow.Disposed -= frmChildFollow_Disposed;
            }
            if (m_frmOwner.CheckHasCreatedControl()&&
                m_frmChildFollow.CheckHasCreatedControl()) {
                this.m_frmOwner.RemoveOwnedForm(this.m_frmChildFollow);
            }
        }

        private void frmChildFollow_Disposed(object sender, EventArgs e) {
            if (this.m_AutoDisposed) {
                this.Dispose();
            }
        }
        private void frmChildFollow_Shown(object sender, EventArgs e) {
            this.m_frmOwner.SafeAction(() => {
                this.m_frmChildStartSize = m_frmChildFollow.Size;
                this.m_frmChildStartLocation = m_frmChildFollow.Location;
                //
                this.m_ownerFrm_PrevPos = this.m_frmOwner.Location;
                m_noNeedFollow = false;
            }, true);
        }


        private void frmOwner_Resize(object sender, EventArgs e) {
            if (checkNeedFollow() == false) {
                return;
            }
            if (m_inFollowing || m_noNeedFollow) {
                return;
            }
            if (checkNeedFollow() == false) {
                return;
            }
            followOwnerFrm();

        }
        private void frmOwner_LocationChanged(object sender, EventArgs e) {
            if (m_inFollowing || m_noNeedFollow) {
                return;
            }
            if (checkNeedFollow() == false) {
                return;
            }
            followOwnerFrm();
        }

        /// <summary>
        /// invoke模式检测是否需要跟随，请不要invoke嵌套造成死锁
        /// </summary>
        /// <returns></returns>
        bool checkNeedFollow() {
            if (this.m_frmOwner.CheckHasCreatedControl() == false ||
                this.m_frmChildFollow.CheckHasCreatedControl() == false) {
                return false;
            }
            FormWindowState frmWindowState = this.m_frmChildFollow.WindowState;//this.SafeFunc<FormWindowState>(() => this.WindowState, FormWindowState.Minimized);
            if (frmWindowState != FormWindowState.Normal)
                return false;
            FormWindowState frmOwnerWindowState = this.m_frmOwner.WindowState;//this.frmOwner.SafeFunc<FormWindowState>(() => this.frmOwner.WindowState, FormWindowState.Minimized);
            if (frmWindowState != FormWindowState.Normal) {
                return false;
            }
            return true;
        }
        protected void followOwnerFrm() {
            if (m_followMode == UIFollowMode.None) {
                return;
            }

            if (m_followMode == UIFollowMode.Offset) {
                m_inFollowing = true;
                //Point location = this.frmOwner.SafeFunc<Point>(() => this.frmOwner.Location, m_OwnerFrm_PrevPos);
                Point location = this.m_frmOwner.Location;
                var offset_x = location.X - m_ownerFrm_PrevPos.X;
                var offset_y = location.Y - m_ownerFrm_PrevPos.Y;
                m_ownerFrm_PrevPos = location;
                this.m_frmChildFollow.SafeAction(() => {
                    this.m_frmChildFollow.Location = new Point(this.m_frmChildFollow.Location.X + offset_x + m_followDistanceX, this.m_frmChildFollow.Location.Y + offset_y + m_followDistanceY);

                }, true);
                m_inFollowing = false;
            }
            else if (this.m_followMode == UIFollowMode.FixedRight) {
                m_inFollowing = true;
                this.m_frmChildFollow.SafeAction(() => {
                    this.m_frmChildFollow.Location = new System.Drawing.Point(
                         this.m_frmOwner.Location.X + this.m_frmOwner.Width  + m_followDistanceX,
                         this.m_frmOwner.Location.Y + m_followDistanceY
                        );
                }, true);
                m_inFollowing = false;
            }
            else if (this.m_followMode == UIFollowMode.FixedDown) {
                m_inFollowing = true;
                this.m_frmChildFollow.SafeAction(() => {
                    this.m_frmChildFollow.Location = new System.Drawing.Point(
                        this.m_frmOwner.Location.X + m_followDistanceX,
                        this.m_frmOwner.Location.Y + this.m_frmOwner.Height + m_followDistanceY
                        );
                }, true);
                m_inFollowing = false;
            }

        }

        public void Dispose() {
            UnHookEvents();
            this.m_frmOwner = null;
            this.m_frmChildFollow = null;
        }
    }
}
