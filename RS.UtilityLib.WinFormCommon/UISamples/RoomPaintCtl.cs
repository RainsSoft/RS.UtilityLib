using RS.UtilityLib.WinFormCommon.UI.ScrollPanel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UISamples
{

    ///// <summary>
    ///// 房间显示所有场地/坐位控件
    ///// 需要增加滚动条
    ///// </summary>
    //public partial class IRQ_RoomPaintCtl : UserControl
    //{
    //    public IRQ_RoomPaintCtl() {
    //        InitializeComponent();
    //        base.SetStyle(ControlStyles.UserPaint, true);
    //        base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
    //        base.SetStyle(ControlStyles.DoubleBuffer, true);
    //        //base.SetStyle(ControlStyles.ResizeRedraw, true);
    //        //base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    //    }
    //    internal event RoomMouseHandler OnRoomMouseMoveEvent;
    //    internal event RoomMouseHandler OnRoomMouseDownEvent;
    //    internal event RoomMouseHandler OnRoomMouseUpEvent;
    //    //internal event RoomMouseHandler OnRoomMouseWheelEvent;

    //    private RoomView m_RoomView;
    //    internal RoomView GetRoomView() {
    //        return this.m_RoomView;
    //    }
    //    internal void ChangeRoomView(IRQ_Room room, int messionType) {
    //        if (m_RoomView != null) {
    //            m_RoomView.Dispose();
    //            m_RoomView = null;
    //        }
    //        this.Width = this.Parent.Width;
    //        this.Height = this.Parent.Height;

    //        m_RoomView = new RoomView(this, room, messionType);
    //        IRQ_ShowUserInfo_HintWindow uhw = new IRQ_ShowUserInfo_HintWindow();
    //        uhw.ShowTipEvent -= (ShowUserTip);
    //        uhw.ShowTipEvent += (ShowUserTip);
    //        m_RoomView.Hint = uhw;
    //        //
    //        m_RoomView.IsShowTip = false;

    //        ChangeRoomWith(this.Width);
    //        //注册响应事件
    //        m_RoomView.OnRoomMouseDownEvent -= (m_RoomView_OnRoomMouseDownEvent);
    //        m_RoomView.OnRoomMouseMoveEvent -= (m_RoomView_OnRoomMouseMoveEvent);
    //        m_RoomView.OnRoomMouseUpEvent -= (m_RoomView_OnRoomMouseUpEvent);

    //        m_RoomView.OnRoomMouseDownEvent += (m_RoomView_OnRoomMouseDownEvent);
    //        m_RoomView.OnRoomMouseMoveEvent += (m_RoomView_OnRoomMouseMoveEvent);
    //        m_RoomView.OnRoomMouseUpEvent += (m_RoomView_OnRoomMouseUpEvent);
    //    }

    //    private void ShowUserTip() {
    //        if (this.m_RoomView == null || this.m_RoomView.HoverItem == null) {
    //            return;
    //        }
    //        UserBaseInfo mi = IRQ_GameNet.UserService.GetUserById(this.m_RoomView.HoverItem.SeatInfo.UserId);
    //        if (mi != null) {
    //            //IRQ_ShowUserInfoFrm.Sington.Hide();
    //            (this.m_RoomView.Hint as IRQ_ShowUserInfo_HintWindow).ReflashUserInfo(mi);
    //            if (!this.m_RoomView.Hint.Visible) {
    //                this.m_RoomView.Hint.Visible = true;
    //            }
    //        }
    //    }


    //    void m_RoomView_OnRoomMouseUpEvent(BFieldView bfieldView, SeatView seatView) {
    //        if (OnRoomMouseUpEvent != null) {
    //            OnRoomMouseUpEvent(bfieldView, seatView);
    //        }
    //    }

    //    void m_RoomView_OnRoomMouseMoveEvent(BFieldView bfieldView, SeatView seatView) {
    //        if (OnRoomMouseUpEvent != null) {
    //            OnRoomMouseMoveEvent(bfieldView, seatView);
    //        }
    //    }

    //    void m_RoomView_OnRoomMouseDownEvent(BFieldView bfieldView, SeatView seatView) {
    //        if (OnRoomMouseUpEvent != null) {
    //            OnRoomMouseDownEvent(bfieldView, seatView);
    //        }
    //    }
    //    internal void ChangeRoomWith(int with) {
    //        if (m_RoomView != null) {
    //            this.m_RoomView.SetWidth(with);
    //            this.Invalidate();
    //        }
    //    }
    //    protected override void WndProc(ref Message m) {
    //        //Console.WriteLine("{0}:{1}",DateTime.Now,m.Msg);
    //        switch (m.Msg) {
    //            case 133:
    //            case 675:
    //                if (this.m_RoomView != null && this.m_RoomView.Hint != null && this.m_RoomView.Hint.IsDisposed == false) {
    //                    Rectangle rec = this.m_RoomView.Hint.RectangleToScreen(new Rectangle(0, 0, this.Width, this.Height));
    //                    if (!rec.Contains(Cursor.Position)) {
    //                        this.m_RoomView.Hint.Hide();
    //                    }
    //                }
    //                break;
    //        }
    //        base.WndProc(ref m);
    //    }
    //    protected override void OnResize(EventArgs e) {
    //        base.OnResize(e);
    //        //
    //        ChangeRoomWith(this.Width);
    //        //
    //        if (this.m_RoomView != null && this.m_RoomView.Hint != null) {
    //            this.m_RoomView.Hint.Hide();
    //        }
    //    }
    //    protected override void OnPaint(PaintEventArgs e) {
    //        //base.OnPaint(e);
    //        if (m_RoomView != null) {
    //            m_RoomView.OnPaint(e);
    //        }
    //    }
    //    //SGD_2011_7_10:怎么解决变为非活动窗口_提示框隐藏;


    //    #region 获得焦点 还原到老位置
    //    delegate void AutoScrollPositionDelegate(ScrollableControl sender, Point p);

    //    private void SetAutoScrollPosition(ScrollableControl sender, Point p) {
    //        p.X = Math.Abs(p.X);
    //        p.Y = Math.Abs(p.Y);
    //        sender.AutoScrollPosition = p;
    //    }
    //    protected override void OnMouseEnter(EventArgs e) {
    //        if (Parent is Panel) // My AutoScroll is set on the panel that contains this (schedule)control for now.
    //         {
    //            Point p = (this.Parent as Panel).AutoScrollPosition;
    //            AutoScrollPositionDelegate del = new AutoScrollPositionDelegate(SetAutoScrollPosition);
    //            Object[] args = { this.Parent as Panel, p };
    //            BeginInvoke(del, args);
    //        }
    //        base.OnMouseEnter(e);
    //    }
    //    protected override void OnMouseWheel(MouseEventArgs e) {
    //        //base.OnMouseWheel(e);
    //    }
    //    protected override void OnGotFocus(EventArgs e) {
    //        //base.OnGotFocus(e);
    //    }
    //    protected override void OnLostFocus(EventArgs e) {
    //        // base.OnLostFocus(e);
    //    }
    //    #endregion
    //    protected override void OnMouseMove(MouseEventArgs e) {
    //        if (!(this.Parent.Parent as MyVScrollPanel).GetScrollBar().Focused) {
    //            (this.Parent.Parent as MyVScrollPanel).GetScrollBar().Focus();
    //        }
    //        if (m_RoomView != null) {
    //            m_RoomView.OnMouseMove(e);
    //        }

    //        //base.OnMouseMove(e);
    //    }
    //    protected override void OnMouseDown(MouseEventArgs e) {
    //        if (m_RoomView != null) {
    //            m_RoomView.OnMouseDown(e);
    //        }
    //        //base.OnMouseDown(e);
    //        //this.Focus();
    //    }
    //    protected override void OnMouseUp(MouseEventArgs e) {
    //        if (m_RoomView != null) {
    //            try {
    //                m_RoomView.OnMouseUp(e);
    //            }
    //            catch (Exception ee) {
    //                DebugLog.Log("因网络问题，座位列表已经清空，取消操作", ee);
    //            }
    //        }
    //        //base.OnMouseUp(e);
    //        //this.Focus();
    //    }

    //    ///// <summary>
    //    ///// 滚轮滚动
    //    ///// </summary>
    //    ///// <remarks>WINNT4.0以上才支持此消息</remarks>
    //    //const int WM_MOUSEWHEEL = 0x020A;
    //    //protected override void WndProc(ref Message m) {
    //    //    switch (m.Msg) { 
    //    //        case WM_MOUSEWHEEL:
    //    //            int i = 0;
    //    //            break;
    //    //    }
    //    //    base.WndProc(ref m);
    //    //}
    //    /// <summary> 
    //    /// 清理所有正在使用的资源。
    //    /// </summary>
    //    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    //    protected override void Dispose(bool disposing) {
    //        OnRoomMouseMoveEvent = null;
    //        OnRoomMouseDownEvent = null;
    //        OnRoomMouseUpEvent = null;

    //        if (m_RoomView != null) {
    //            m_RoomView.Dispose();
    //            m_RoomView = null;
    //        }
    //        if (disposing && (components != null)) {
    //            components.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }
    //    #region designer
    //    /// <summary> 
    //    /// 必需的设计器变量。
    //    /// </summary>
    //    private System.ComponentModel.IContainer components = null;



    //    #region 组件设计器生成的代码

    //    /// <summary> 
    //    /// 设计器支持所需的方法 - 不要
    //    /// 使用代码编辑器修改此方法的内容。
    //    /// </summary>
    //    private void InitializeComponent() {
    //        this.SuspendLayout();
    //        // 
    //        // IRQ_RoomPaintCtl
    //        // 
    //        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
    //        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    //        this.BackColor = System.Drawing.Color.Transparent;
    //        this.Name = "IRQ_RoomPaintCtl";
    //        this.Size = new System.Drawing.Size(800, 600);
    //        this.ResumeLayout(false);

    //    }

    //    #endregion

    //    #endregion

    //}

    ///*
    // * 房间显示：
    // *    1：鼠标等事件
    // *    2：绘制座子
    // *    3：绘制玩家
    // */
    //public delegate void RoomMouseHandler(BFieldView bfieldView, SeatView seatView);
    //internal class RoomView
    //{
    //    public event RoomMouseHandler OnRoomMouseMoveEvent;
    //    public event RoomMouseHandler OnRoomMouseDownEvent;
    //    public event RoomMouseHandler OnRoomMouseUpEvent;
    //    //private string m_UserRoomInfoFrm = "UserRoomInfoFrm";
    //    public bool IsShowTip { get; set; }

    //    internal RoomView(Control owner, IRQ_Room room, int messionType) {
    //        this.m_RoomTip = new IRQ_ToolTipEx();
    //        string tipImageBackPath = "Skin\\Res\\roomuserinfo.bmp";//
    //        //tipImageBackPath=@"Skin\Res\tooltip_room.png"
    //        Image imgt = IRQ_Res.GetImage(tipImageBackPath);
    //        Debug.Assert(imgt != null, "不存在Skin\\Res\\tooltip_room.png");
    //        m_RoomTip.Size = imgt.Size;
    //        m_RoomTip.BackImage = imgt;
    //        m_RoomTip.TitleTxtOffset = new Point(50, 4);
    //        m_RoomTip.TipTxtOffset = new Point(100, 4);
    //        //
    //        //m_RoomTip.Opacity = 0.8d;
    //        //m_RoomTip.IsShowWidget = false;

    //        IRQ_ToolTipWidget user_Widgetphoto = new IRQ_ToolTipWidget(16, 66, 64, 64);
    //        user_Widgetphoto.Name = "User_Tip_Photo";
    //        user_Widgetphoto.Image = IRQ_GameNet.UserService.GetUserPhoto64(IRQ_GameNet.UserService.CurrentUser.Id);
    //        m_RoomTip.AddWidget(user_Widgetphoto);

    //        IRQ_ToolTipWidget user_WidgetUrl = new IRQ_ToolTipWidget(120, 152, 80, 16);
    //        user_WidgetUrl.Text = "用户空间";
    //        user_WidgetUrl.TextLinkUrl = "用户空间网页";
    //        user_WidgetUrl.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);
    //        m_RoomTip.AddWidget(user_WidgetUrl);

    //        IRQ_ToolTipWidget user_Widgetrongyu = new IRQ_ToolTipWidget(200, 152, 80, 16);
    //        user_Widgetrongyu.Text = "用户荣誉";
    //        user_Widgetrongyu.TextLinkUrl = "用户荣誉网页";
    //        user_Widgetrongyu.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 255);
    //        m_RoomTip.AddWidget(user_Widgetrongyu);
    //        //

    //        m_RoomTip.OnAfterHideEvent += (m_RoomTip_OnAfterHideEvent);
    //        m_RoomTip.OnMouseDownEvent += (m_RoomTip_OnMouseDownEvent);
    //        m_RoomTip.OnMouseMoveEvent += (m_RoomTip_OnMouseMoveEvent);
    //        //
    //        //IRQ_ShowUserInfoFrm.GetUserInfoFrm(m_UserRoomInfoFrm).Hide();
    //        //
    //        this.m_OwnerCtl = owner;
    //        this.MessionType = messionType;

    //        this.Room = room;
    //        this.BFieldSeatNum = (int)room.BFieldSeatsNum;
    //        this.BFieldCount = room.BFieldCount;
    //        //
    //        this.BFieldViews = new List<BFieldView>();
    //        this.BFieldRec = new List<Rectangle>();
    //        //this.BFieldSeatsRelRec = new List<Rectangle>();
    //        //
    //        this.m_BFImg = new BFieldViewImage(messionType, this.BFieldSeatNum);
    //        this.BFieldViewSize = this.m_BFImg.GetBackImgSize();
    //        //
    //        this.Init();
    //    }


    //    void m_RoomTip_OnMouseMoveEvent(IRQ_ToolTipWidget obj) {
    //        if (obj != null) {
    //            this.m_OwnerCtl.Cursor = Cursors.Hand;
    //        }
    //    }

    //    void m_RoomTip_OnMouseDownEvent(IRQ_ToolTipWidget obj) {
    //        if (obj != null) {
    //            //Console.WriteLine(obj.Text);
    //        }
    //    }

    //    void m_RoomTip_OnAfterHideEvent(IRQ_ToolTipEx obj) {
    //        //Console.WriteLine("tooltip hide");
    //    }
    //    private Control m_OwnerCtl;
    //    private IRQ_ToolTipEx m_RoomTip;
    //    private BFieldViewImage m_BFImg;

    //    /// <summary>
    //    /// 场地的所有绘制对象 在 Init方法中初始化
    //    /// </summary>
    //    internal List<BFieldView> BFieldViews;
    //    /// <summary>
    //    ///  场地的位置信息 在 Init方法中初始化
    //    /// </summary>
    //    internal List<Rectangle> BFieldRec;
    //    ///// <summary>
    //    ///// 座位数BFieldSeatNum的 座位相对位置 在 Init方法中初始化
    //    ///// </summary>
    //    //internal List<Rectangle> BFieldSeatsRelRec;
    //    internal IRQ_Room Room;

    //    public int BFiledCoulum = 5;
    //    public int BFieldCount = 100;
    //    public int BFieldSeatNum = 3;
    //    public int BFieldRow = 20;

    //    /// <summary>
    //    /// 场地尺寸128/150
    //    /// </summary>
    //    public Size BFieldViewSize = new Size(150, 150);
    //    /// <summary>
    //    /// 场地之间的间隔24/上下左右
    //    /// </summary>
    //    public int BFieldViewSpace = 24;

    //    /// <summary>
    //    /// 鼠标在坐位上悬浮时 显示的文本
    //    /// </summary>
    //    internal string RoomTipText {
    //        get;
    //        set;
    //    }

    //    internal int MessionType {
    //        get;
    //        set;
    //    }
    //    /// <summary>
    //    /// 创建控件 room的场地信息必须已经赋值 任务类型
    //    /// </summary>
    //    /// <param name="room"></param>
    //    protected void Init() {
    //        //
    //        IRQ_BattleField[] bfs = this.Room.GetBFileds();
    //        System.Diagnostics.Debug.Assert(bfs.Length == this.BFieldCount && this.BFieldCount != 0);//
    //        //
    //        //this.m_OwnerCtl.Width = (this.BFieldViewSize + this.BFieldViewSpace) * this.BFiledCoulum;
    //        //
    //        CaculRowCoulum();

    //        //128x128 -64-64
    //        //for (int s = 0; s < this.BFieldSeatNum; s++) {
    //        //    //默认初始化座位的相对位置
    //        //    Rectangle rRec = new Rectangle(s % 4 * 32, (s / 4) * 64, 32, 32);
    //        //    BFieldSeatsRelRec.Add(rRec);
    //        //}
    //        //BFieldSeatsRelRec.AddRange(this.m_BFImg.GetSeatsRect());

    //        for (int i = 0; i < this.BFieldRow; i++) {
    //            for (int j = 0; j < this.BFiledCoulum; j++) {
    //                int currentPos = i * this.BFiledCoulum + j;
    //                if (currentPos < this.BFieldCount) {
    //                    Rectangle rec = new Rectangle(j * this.BFieldViewSize.Width + (j + 1) * this.BFieldViewSpace, i * this.BFieldViewSize.Height + (i + 1) * this.BFieldViewSpace, this.BFieldViewSize.Width, this.BFieldViewSize.Height);
    //                    BFieldView bv = new BFieldView(currentPos, bfs[currentPos], this.MessionType, rec, this.m_BFImg);
    //                    //
    //                    this.BFieldViews.Add(bv);

    //                    this.BFieldRec.Add(rec);
    //                    //座位
    //                    //for (int k = 0; k < this.BFieldSeatNum; k++) {
    //                    //Rectangle rRec=new Rectangle(k%4*32,(k/4)*64,32,32);
    //                    //Rectangle rRec = BFieldSeatsRelRec[k];//前面以默认的位置增加了
    //                    //
    //                    //SeatView cbv = bv.CreateSeatView(bfs[currentPos].Seats[k], rRec, this.RoomSeatNoManImg, this.RoomSeatNoFaceImg);
    //                    // }
    //                }
    //                else {
    //                    break;
    //                }

    //            }
    //        }

    //    }
    //    #region paint
    //    internal void OnPaint(PaintEventArgs e) {
    //        foreach (var bf in BFieldViews) {
    //            //只绘制需要更新的地方
    //            if (bf.Rect.IntersectsWith(e.ClipRectangle)) {
    //                bf.Draw(e.Graphics);
    //            }
    //        }
    //    }
    //    #endregion

    //    #region hit win
    //    private RibbonHintWindow m_hintWnd;
    //    /// <summary>
    //    /// 提示窗口
    //    /// </summary>
    //    public RibbonHintWindow Hint {
    //        get {
    //            return m_hintWnd;
    //        }
    //        set {
    //            m_hintWnd = value;
    //        }
    //    }
    //    public SeatView HoverItem { get { return m_HoverItem; } }
    //    private SeatView m_HoverItem;
    //    public SeatView PreItemHit { get { return m_PreItemHit; } }
    //    private SeatView m_PreItemHit;
    //    private void ShowCustomHintWindow(MouseEventArgs e) {
    //        if (this.m_hintWnd != null) {

    //            if (m_HoverItem != null) {
    //                bool showHint = false;
    //                if (m_HoverItem.SeatInfo.HasUser)
    //                    showHint = true;
    //                if (showHint == false) {
    //                    RibbonHintWindow.HideHint(this.m_hintWnd);
    //                    return;
    //                }
    //                if (m_HoverItem != m_PreItemHit) {
    //                    Point p = new Point(m_HoverItem.Left + m_HoverItem.Owner.Left/* + m_HoverItem.Width*/, m_HoverItem.Top + m_HoverItem.Owner.Top + m_HoverItem.Height);
    //                    p = this.m_OwnerCtl.PointToScreen(p);
    //                    int screenHeight = Screen.FromHandle(this.m_OwnerCtl.Handle).WorkingArea.Height;
    //                    if (this.m_hintWnd.Size.Height + p.Y > screenHeight) {
    //                        p.Y = screenHeight - this.m_hintWnd.Size.Height;
    //                    }
    //                    RibbonHintWindow.ShowHint(this.m_hintWnd, p.X, p.Y);
    //                }
    //            }
    //            else {
    //                RibbonHintWindow.HideHint(this.m_hintWnd);
    //            }
    //        }
    //    }

    //    #endregion

    //    #region mouse event
    //    internal void OnMouseMove(MouseEventArgs e) {
    //        if (this.m_RoomTip.ActionMouseMove()) return;
    //        bool mousuHandle = false;
    //        bool hasUser = false;
    //        SeatView sv = null;
    //        foreach (var b in this.BFieldViews) {
    //            b.OnMouseMove(e);
    //            //
    //            if (b.MouseHover || b.MouseBeginEnter || b.MouseBeginLeave) {

    //                foreach (var s in b.Seats) {
    //                    //包含右侧的名字条
    //                    Rectangle newRct = new Rectangle(s.Rect.X, s.Rect.Y, s.Width + 118, s.Height);
    //                    if (s.MouseHover || (newRct.Contains(e.X - b.Left, e.Y - b.Top))) {
    //                        sv = s as SeatView;
    //                        mousuHandle = true;
    //                        m_HoverItem = sv;
    //                        if (s.SeatInfo.HasUser) {
    //                            hasUser = true;
    //                        }
    //                    }
    //                    if (s.MouseBeginEnter || s.MouseBeginLeave) {
    //                        this.m_OwnerCtl.Invalidate(new Rectangle(s.Left + b.Left, s.Top + b.Top, s.Width + 2, s.Height + 2));
    //                        s.SetReDrawOk();
    //                    }
    //                }
    //                if (this.OnRoomMouseMoveEvent != null) {
    //                    this.OnRoomMouseMoveEvent(b, sv);
    //                }
    //            }
    //        }
    //        if (mousuHandle) {
    //            if (m_HoverItem.Enable)//增加对不可用的手势设置
    //                this.m_OwnerCtl.Cursor = Cursors.Hand;
    //            if (hasUser && !string.IsNullOrEmpty(this.RoomTipText) && !m_TipVisible) {
    //                m_TipVisible = true;
    //                //
    //                if (IsShowTip) {
    //                    this.m_RoomTip.Show(this.RoomTipText, this.m_OwnerCtl, sv.Left + sv.Owner.Left /*+ sv.Width*/, sv.Top + sv.Owner.Top + sv.Height);
    //                }
    //                this.ShowCustomHintWindow(e);
    //                m_PreItemHit = m_HoverItem;
    //                //Point lp = new Point(sv.Left + sv.Owner.Left + sv.Width, sv.Top + sv.Owner.Top + sv.Height);
    //                //UserBaseInfo user = IRQ_GameNet.UserService.GetUserById(sv.SeatInfo.UserId);
    //                //IRQ_ShowUserInfoFrm.Sington.ReflashUserInfo(user);
    //                //IRQ_ShowUserInfoFrm.Sington.Show(this.m_OwnerCtl, lp);
    //            }
    //            //
    //        }
    //        else {
    //            this.m_OwnerCtl.Cursor = Cursors.Default;
    //            this.m_TipVisible = false;
    //            this.m_HoverItem = null;
    //            this.m_PreItemHit = null;
    //            this.m_RoomTip.Hide(this.m_OwnerCtl);
    //            //IRQ_ShowUserInfoFrm.Sington.Hide();
    //            if (this.m_hintWnd != null) {
    //                this.m_hintWnd.Hide();
    //            }
    //        }

    //    }
    //    private bool m_TipVisible = false;
    //    internal void OnMouseDown(MouseEventArgs e) {
    //        if (this.m_RoomTip.ActionMouseDown()) return;
    //        foreach (var b in this.BFieldViews) {
    //            b.OnMouseDown(e);
    //            if (b.Rect.Contains(e.X, e.Y)) {
    //                SeatView sv = null;
    //                foreach (var s in b.Seats) {
    //                    if (s.Rect.Contains(e.X - b.Left, e.Y - b.Top)) {
    //                        sv = s as SeatView;
    //                        this.m_OwnerCtl.Invalidate(new Rectangle(s.Left + b.Left, s.Top + b.Top, s.Width, s.Height));
    //                        s.SetReDrawOk();
    //                    }
    //                    else {
    //                        //包含右侧的名字条
    //                        Rectangle newRct = new Rectangle(s.Rect.X, s.Rect.Y, s.Width + 118, s.Height);
    //                        if (newRct.Contains(e.X - b.Left, e.Y - b.Top)) {
    //                            sv = s as SeatView;
    //                            this.m_OwnerCtl.Invalidate(new Rectangle(s.Left + b.Left, s.Top + b.Top, s.Width, s.Height));
    //                            s.SetReDrawOk();
    //                        }
    //                    }
    //                }
    //                if (this.OnRoomMouseDownEvent != null) {
    //                    this.OnRoomMouseDownEvent(b, sv);
    //                }
    //            }
    //        }
    //    }
    //    internal void OnMouseUp(MouseEventArgs e) {
    //        if (this.m_RoomTip.ActionMouseUp()) return;
    //        foreach (var b in this.BFieldViews) {
    //            b.OnMouseUp(e);
    //            if (b.Rect.Contains(e.X, e.Y)) {
    //                SeatView sv = null;
    //                foreach (var s in b.Seats) {
    //                    if (s.Rect.Contains(e.X - b.Left, e.Y - b.Top)) {
    //                        sv = s as SeatView;
    //                        this.m_OwnerCtl.Invalidate(new Rectangle(s.Left + b.Left, s.Top + b.Top, s.Width, s.Height));
    //                        s.SetReDrawOk();
    //                    }
    //                    else {
    //                        //包含右侧的名字条
    //                        Rectangle newRct = new Rectangle(s.Rect.X, s.Rect.Y, s.Width + 118, s.Height);
    //                        if (newRct.Contains(e.X - b.Left, e.Y - b.Top)) {
    //                            sv = s as SeatView;
    //                            this.m_OwnerCtl.Invalidate(new Rectangle(s.Left + b.Left, s.Top + b.Top, s.Width, s.Height));
    //                            s.SetReDrawOk();
    //                        }
    //                    }
    //                }
    //                if (this.OnRoomMouseUpEvent != null) {
    //                    this.OnRoomMouseUpEvent(b, sv);
    //                }
    //            }
    //        }
    //    }
    //    #endregion

    //    /// <summary>
    //    /// 计算行列
    //    /// </summary>
    //    private int CaculRowCoulum() {
    //        int offsetX = 0;
    //        //
    //        this.BFiledCoulum = this.m_OwnerCtl.Width / (this.BFieldViewSize.Width + this.BFieldViewSpace);
    //        if (BFiledCoulum < 1) {
    //            BFiledCoulum = 1;
    //        }
    //        offsetX = this.m_OwnerCtl.Width - this.BFiledCoulum * (this.BFieldViewSize.Width + this.BFieldViewSpace);
    //        offsetX = offsetX / 2;
    //        if (offsetX < 0) {
    //            offsetX = 0;
    //        }
    //        this.BFieldRow = this.BFieldCount / this.BFiledCoulum;
    //        int rrow = this.BFieldCount % this.BFiledCoulum;
    //        if (rrow > 0) {
    //            this.BFieldRow += 1;
    //        }
    //        int h = (this.BFieldViewSize.Height + this.BFieldViewSpace) * this.BFieldRow + this.BFieldViewSpace;
    //        if (this.m_OwnerCtl.Height < h) {
    //            this.m_OwnerCtl.Height = h;
    //        }
    //        //
    //        return offsetX;
    //    }

    //    /// <summary>
    //    /// 设置列来设置控件的宽度与高度  
    //    /// </summary>
    //    /// <param name="coulum"></param>
    //    public void SetCoulum(int coulum) {
    //        //计算
    //        this.BFiledCoulum = coulum;
    //        this.m_OwnerCtl.Width = (this.BFieldViewSize.Width + this.BFieldViewSpace) * this.BFiledCoulum;
    //        //           
    //        int offsetX = CaculRowCoulum();
    //        offsetX = offsetX - this.BFieldViewSpace / 2;

    //        for (int i = 0; i < this.BFieldRow; i++) {
    //            for (int j = 0; j < this.BFiledCoulum; j++) {
    //                int currentPos = i * this.BFiledCoulum + j;
    //                if (currentPos < this.BFieldCount) {
    //                    Rectangle rec = new Rectangle(offsetX + j * this.BFieldViewSize.Width + (j + 1) * this.BFieldViewSpace, i * this.BFieldViewSize.Height + (i + 1) * this.BFieldViewSpace, this.BFieldViewSize.Width, this.BFieldViewSize.Height);
    //                    this.BFieldRec[currentPos] = rec;
    //                }
    //                else {
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 设置控件的宽度 更新行列数和控件的高度
    //    /// 已经包括了滚动条的宽度
    //    /// </summary>
    //    /// <param name="width"></param>
    //    public void SetWidth(int width) {

    //        if (width < this.BFieldViewSize.Width + this.BFieldViewSpace + 20) {
    //            width = this.BFieldViewSize.Width + this.BFieldViewSpace + 20;
    //        }
    //        this.m_OwnerCtl.Width = width;
    //        if (this.m_OwnerCtl.Width == 0)
    //            return;//如果修改失败 返回 
    //        int offsetX = CaculRowCoulum();
    //        offsetX = offsetX - this.BFieldViewSpace / 2;
    //        for (int i = 0; i < this.BFieldRow; i++) {
    //            for (int j = 0; j < this.BFiledCoulum; j++) {
    //                int currentPos = i * this.BFiledCoulum + j;
    //                if (currentPos < this.BFieldCount) {
    //                    Rectangle rec = new Rectangle(offsetX + j * this.BFieldViewSize.Width + (j + 1) * this.BFieldViewSpace, i * this.BFieldViewSize.Height + (i + 1) * this.BFieldViewSpace, this.BFieldViewSize.Width, this.BFieldViewSize.Height);
    //                    this.BFieldRec[currentPos] = rec;
    //                    //座位
    //                    this.BFieldViews[currentPos].Rect = rec;
    //                }
    //                else {
    //                    break;
    //                }
    //            }
    //        }

    //    }

    //    internal void Dispose() {
    //        this.OnRoomMouseDownEvent = null;
    //        this.OnRoomMouseMoveEvent = null;
    //        this.OnRoomMouseUpEvent = null;

    //        if (m_hintWnd != null && !m_hintWnd.IsDisposed) {
    //            m_hintWnd.Dispose();
    //            m_hintWnd = null;
    //        }

    //        foreach (var bf in this.BFieldViews) {
    //            bf.Dispose();
    //        }


    //        this.BFieldViews.Clear();
    //        this.BFieldViews = null;
    //        //this.Room.Dispose();
    //        this.Room = null;

    //        this.m_BFImg.Dispose();
    //        this.m_RoomTip.Dispose();
    //        this.m_RoomTip = null;

    //    }


    //    /*
    //    /// <summary>
    //    /// 更新绘制界面 设置完绘制参数 必须执行本方法更新
    //    /// </summary>
    //    public void UpdateDraw() {

    //        //CaculRowCoulum();
    //        //128x128 -64-64

    //        //for (int i = 0; i < this.BFieldRow; i++) {
    //        //    for (int j = 0; j < this.BFiledCoulum; j++) {
    //        //        int currentPos = i * this.BFiledCoulum + j;
    //        //        if (currentPos < this.BFieldCount ) {
    //        //            //Rectangle rec = new Rectangle(j * 128 + j * 64, i * 128 + i * 64, 128, 128);
    //        //            BFieldView bv = this.BFieldViews[currentPos];
    //        //            bv.Rect = this.BFieldRec[currentPos];//rec;
    //        //            //座位
    //        //            for (int k = 0; k < this.BFieldSeatNum; k++) {
    //        //                SeatView cbv = bv.Seats[k];
    //        //                cbv.Rect = BFieldSeatsRelRec[k];//
    //        //            }
    //        //        }
    //        //        else {
    //        //            break;
    //        //        }
    //        //        //
    //        //    }
    //        //}
    //        //for (int i = 0; i < this.Room.BFieldCount; i++) {
    //        //    this.BFieldViews[i].Rect = this.BFieldRec[i];
    //        //    for (int k = 0; k < this.BFieldSeatNum; k++) {
    //        //        SeatView cbv = this.BFieldViews[i].Seats[k];
    //        //        cbv.Rect = BFieldSeatsRelRec[k];//
    //        //    }
    //        //}
    //        //this.m_OwnerCtl.Invalidate();
    //    }
    //    /// <summary>
    //    /// 设置第Index（0开始）个座位的起始位置 不会重新绘制
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <param name="relLeft"></param>
    //    /// <param name="relTop"></param>
    //    public void SetSeatRelRec(int index, int relLeft, int relTop) {
    //        if (index > this.BFieldSeatsRelRec.Count - 1)
    //            return;
    //        //bool change = false;
    //        //if (this.BFieldSeatsRelRec[index].X != relLeft || this.BFieldSeatsRelRec[index].Y != relTop) {
    //        //    change = true;
    //        //}
    //        //if (change) {
    //        this.BFieldSeatsRelRec[index] = new Rectangle(relLeft, relTop, 32, 32);
    //        //
    //        //foreach (var v in this.BFieldViews) {
    //        //    int i=0;
    //        //    foreach (var s in v.Seats) {
    //        //        s.Rect = BFieldSeatsRelRec[i];
    //        //    }
    //        //}
    //        //}
    //    }
    //    /// <summary>
    //    /// 设置总个控件背景
    //    /// </summary>
    //    /// <param name="img"></param>
    //    public void SetRoomBackImg(Image img) {
    //        this.BackgroundImage = img;
    //    }
    //    /// <summary>
    //    /// 设置桌子背景
    //    /// </summary>
    //    /// <param name="img"></param>
    //    public void SetBFieldBackImg(Image img) {
    //        this.RoomBFieldBackImg = img;
    //        if (this.m_Init) {
    //            foreach (var v in this.BFieldViews) {
    //                v.SetBackImg(img);
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 设置座位背景
    //    /// </summary>
    //    /// <param name="img"></param>
    //    public void SetSeatNoManImg(Image img) {
    //        this.RoomSeatNoManImg = img;
    //        if (this.m_Init) {
    //            foreach (var v in this.BFieldViews) {
    //                foreach (var s in v.Seats) {
    //                    s.SeatNoManImage = img;
    //                }
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 设置玩家没有设置头像时在坐位的显示
    //    /// </summary>
    //    /// <param name="img"></param>
    //    public void SetSeatNoFaceImg(Image img) {
    //        this.RoomSeatNoFaceImg = img;
    //        if (this.m_Init) {
    //            foreach (var v in this.BFieldViews) {
    //                foreach (var s in v.Seats) {
    //                    s.PlayNoFaceImage = img;
    //                }
    //            }
    //        }
    //    }
    //             /// <summary>
    //    /// 设置任务/显示类型
    //    /// </summary>
    //    /// <param name="messionType"></param>
    //    public void SetMessionType(int messionType) {
    //        this.RoomMessionType = messionType;
    //        if (this.m_Init) {
    //            foreach (var v in this.BFieldViews) {
    //                v.ViewType = messionType;
    //            }
    //        }
    //    }
    //     */
    //}
    ///// <summary>
    ///// 场地显示类 场地的间距至少 >=64 房间总的尺寸 需要N列*(场地的宽+场地的间距) m行*(场地的高+场地的间距)
    ///// 注意：所有资源都在外面释放，这里只取消引用
    ///// </summary>
    //public class BFieldView : ABFieldView
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="g"></param>
    //    /// <param name="rec">场地的位置 尺寸128x128</param>
    //    /// <param name="backImg"></param>
    //    internal BFieldView(int index, BattleFieldInfo bfieldinfo, int messionType, Rectangle rec, ABFieldViewImage bfieldimg)
    //        : base(index, bfieldinfo, messionType, rec, bfieldimg) {
    //        //
    //        //this.Rect = new Rectangle(0, 0, this.BFImage.BackImg.Width, this.BFImage.BackImg.Height);
    //        //创建座位
    //        CreateAllSeatViews();
    //    }


    //    protected void CreateAllSeatViews() {
    //        int scount = this.BFiledInfo.Seats.Count;
    //        Rectangle[] seats = this.BFImage.GetSeatsRect();
    //        for (int i = 0; i < scount; i++) {
    //            CreateSeatView(this.BFiledInfo.Seats[i], seats[i]);
    //        }
    //    }
    //    /// <summary>
    //    /// 创建座位并添加到场地中 
    //    /// </summary>
    //    /// <param name="seatIndex">座位号 0开始</param>
    //    /// <param name="relRec">相对与场地的位置信息 尺寸32x32</param>
    //    /// <param name="seatBackImg">座位的默认绘制图，如果有用户坐下，将绘制用户头像</param>
    //    /// <returns>返回当前创建的座位</returns>
    //    protected override ASeatView CreateSeatView(SeatInfo seatinfo, Rectangle relRec) {
    //        //创建座位
    //        //Rectangle relRec = new Rectangle(relLeft, relTop, 32, 32);
    //        SeatView seatv = new SeatView(this, seatinfo, relRec);
    //        //seatv.SeatNoManImage = seatNoManImg;
    //        //seatv.PlayNoFaceImage = playNoFaceImg;
    //        System.Diagnostics.Debug.Assert(this.Seats.Count < this.BFiledInfo.Seats.Count);
    //        this.Seats.Add(seatv);

    //        return seatv;
    //    }


    //    public override void Dispose() {

    //        base.Dispose();
    //    }
    //}
    //internal class BFieldViewImage : ABFieldViewImage
    //{
    //    internal BFieldViewImage(int messiontype, int seatCount)
    //        : base(messiontype, seatCount) {
    //    }

    //    internal override void InternalDispose(bool disposing) {

    //    }
    //}
    //public class SeatView : ASeatView
    //{
    //    internal SeatView(BFieldView bfieldview, SeatInfo seatinfo, Rectangle rec) :
    //        base(bfieldview, seatinfo, rec) {
    //    }
    //    protected override void DrawName(Graphics g, Rectangle globRec) {
    //        //base.DrawName(globRec);
    //        int left = globRec.Left;
    //        int top = globRec.Top;

    //        RectangleF nrc = this.Owner.BFImage.PlayNameStrRec;
    //        RectangleF dRec = new RectangleF(left + nrc.Left, top + nrc.Top, nrc.Width, nrc.Height);
    //        g.DrawString(this.PlayName, System.Drawing.SystemFonts.DefaultFont, System.Drawing.SystemBrushes.HighlightText, dRec, m_StrFormat);
    //    }
    //}
}
