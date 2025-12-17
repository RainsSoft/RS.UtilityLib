using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing.Drawing2D;


namespace RS.UtilityLib.WinFormCommon.UISamples
{
//    public class IRQ_Res
//    {
//        internal static Image GetImage(string v) {
//            throw new NotImplementedException();
//        }
//    }
//    public abstract class ABFieldViewImage : IDisposable
//    {
//        protected Image m_BackImg;
//        protected Image m_Num10Img;
//        protected Image m_Status3Img;
//        protected Image m_SeatImage;

//        protected Rectangle m_SeatNoManImgRec;
//        protected Rectangle m_PlayNoFaceImgRec;
//        protected RectangleF m_PlayNameStrRec;
//        //增加座位不可用 绘制效果
//        protected Rectangle m_SeatUnEnableImgRec;
//        //
//        protected int m_MesstionType;
//        protected int m_SeatCount;
//        protected bool m_VS;
//        protected Rectangle[] m_SeatsRect;

//        internal Image BackImg { get { return m_BackImg; } }//底图
//        internal Image Num10Img { get { return m_Num10Img; } }//数字图
//        internal Image Status3Img { get { return m_Status3Img; } }//状态图
//        internal Image SeatImage { get { return m_SeatImage; } }

//        internal Rectangle SeatNoManImgRec { get { return m_SeatNoManImgRec; } }//没有坐上去绘制的图
//        //增加座位不可用 绘制效果
//        internal Rectangle SeatUnEnableImgRec { get { return m_SeatUnEnableImgRec; } }//位置不可用绘制图的图
//        //
//        internal Rectangle PlayNoFaceImgRec { get { return m_PlayNoFaceImgRec; } }//玩家没有头像绘制的图
//        internal RectangleF PlayNameStrRec { get { return m_PlayNameStrRec; } }//玩家名字绘制的地方

//        internal int MesstionType { get { return m_MesstionType; } }
//        internal int SeatCount { get { return m_SeatCount; } }
//        internal bool IsVS { get { return m_VS; } }

//        internal Rectangle[] GetSeatsRect() {
//            return m_SeatsRect;
//        }
//        internal Size GetBackImgSize() {
//            return m_BackImg.Size;
//        }
//        internal ABFieldViewImage(int messtionType, int seatCount) {
//            m_MesstionType = messtionType;
//            m_SeatCount = seatCount;
//            Load(messtionType, seatCount);
//        }
//        protected virtual void Load(int messionType, int seatCount) {
//            this.Dispose();
//            //
//            this.m_Num10Img = IRQ_Res.GetImage(@"Skin\NetPlat\num10img.png");
//            this.m_Status3Img = IRQ_Res.GetImage(@"Skin\NetPlat\bfiledstatusimg.png");
//            this.m_SeatImage = IRQ_Res.GetImage(@"Skin\NetPlat\seatimg.png");
//            this.m_PlayNoFaceImgRec = new Rectangle(32 * 2, 0, 32, 32);
//            this.m_SeatNoManImgRec = new Rectangle(32 * 0, 0, 32, 32);
//            this.m_SeatUnEnableImgRec = new Rectangle(32 * 1, 0, 32, 32);
//            bool vs = false;
//            if (messionType == MessionType.V11 ||
//                messionType == MessionType.V22 ||
//                messionType == MessionType.V44 ||
//                messionType == MessionType.VS_Free) {
//                vs = true;
//            }
//            this.m_VS = vs;
//            if (vs) {
//                //使用带VS的底图                
//                System.Diagnostics.Debug.Assert(seatCount % 2 == 0);
//                switch (seatCount) {
//                    case 1://如果座位1个，那就要加载一下图片，避免出错
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree1.png");
//                        break;
//                    case 2:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seat1v1.png");
//                        break;
//                    case 3: //如果座位3个，那就要加载一下图片，避免出错
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree3.png");
//                        break;
//                    case 4:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seat2v2.png");
//                        break;
//                    default:
//                        break;
//                }
//            }
//            else {
//                switch (seatCount) {
//                    case 1:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree1.png");
//                        break;
//                    case 2:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree2.png");
//                        break;
//                    case 3:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree3.png");
//                        break;
//                    case 4:
//                        this.m_BackImg = IRQ_Res.GetImage(@"Skin\NetPlat\seatfree4.png");
//                        break;
//                }
//            }
//            //
//            m_SeatsRect = new Rectangle[seatCount];
//            switch (seatCount) {
//                case 1:
//                    //43，50
//                    m_SeatsRect[0] = new Rectangle(31, 51, 32, 32);
//                    break;
//                case 2:
//                    if (vs) {
//                        //20，50	104，50	
//                        m_SeatsRect[0] = new Rectangle(31, 35, 32, 32);
//                        m_SeatsRect[1] = new Rectangle(87, 84, 32, 32);
//                    }
//                    else {
//                        //20，50	66，50	
//                        m_SeatsRect[0] = new Rectangle(31, 35, 32, 32);
//                        m_SeatsRect[1] = new Rectangle(31, 87, 32, 32);
//                    }
//                    break;
//                case 3:
//                    //20，50	66，50	112，50	
//                    m_SeatsRect[0] = new Rectangle(31, 35, 32, 32);
//                    m_SeatsRect[1] = new Rectangle(31, 73, 32, 32);
//                    m_SeatsRect[2] = new Rectangle(31, 111, 32, 32);
//                    break;
//                case 4:
//                    if (vs) {
//                        //20，50	66，50	150，50	196，50
//                        m_SeatsRect[0] = new Rectangle(31, 35, 32, 32);
//                        m_SeatsRect[1] = new Rectangle(31, 69, 32, 32);
//                        m_SeatsRect[2] = new Rectangle(87, 105, 32, 32);
//                        m_SeatsRect[3] = new Rectangle(87, 139, 32, 32);
//                    }
//                    else {
//                        //20，50	66，50	112，50	158，50
//                        m_SeatsRect[0] = new Rectangle(31 - 1, 35 - 1, 32, 32);
//                        m_SeatsRect[1] = new Rectangle(31 - 1, 69 - 1, 32, 32);
//                        m_SeatsRect[2] = new Rectangle(31, 106, 32, 32);
//                        m_SeatsRect[3] = new Rectangle(31, 140, 32, 32);
//                    }
//                    break;
//            }
//            this.m_PlayNameStrRec = new RectangleF(32 + 4f, 12f, 112, 24);
//            //int leftoffset = ((this.Index + 1) % 2) == 0 ? 0 : this.Width / 3;
//            //int withoffset = ((this.Index + 1) % 2) == 0 ? this.Width / 3 : this.Width / 4;
//            //this.m_Graph.DrawString(this.PlayName, System.Drawing.SystemFonts.StatusFont, System.Drawing.SystemBrushes.ActiveCaption, new RectangleF(this.Left + this.Owner.Left - leftoffset, this.Top + this.Height + this.Owner.Top, 40 + withoffset, 60));

//        }

//        protected bool m_Disposed = false;
//        protected virtual void Dispose(bool disposing) {
//            if (m_Disposed) {
//                return;
//            }
//            this.InternalDispose(disposing);
//            if (m_BackImg != null) {
//                m_BackImg.Dispose();
//                m_BackImg = null;
//            }
//            if (m_Num10Img != null) {
//                m_Num10Img.Dispose();
//                m_Num10Img = null;
//            }
//            if (m_SeatImage != null) {
//                m_SeatImage.Dispose();
//                m_SeatImage = null;
//            }
//            if (m_Status3Img != null) {
//                m_Status3Img.Dispose();
//                m_Status3Img = null;
//            }
//            m_Disposed = true;
//        }
//        public void Dispose() {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//        ~ABFieldViewImage() {
//            Dispose(false);
//        }
//        internal abstract void InternalDispose(bool disposing);
//    }


//    /// <summary>
//    /// 场地显示基类 场地的间距至少 >=64 房间总的尺寸 需要N列*(场地的宽+场地的间距) m行*(场地的高+场地的间距)
//    /// 注意：所有资源都在外面释放 这里只取消引用
//    /// </summary>
//    public abstract class ABFieldView : IDisposable
//    {

//        internal ABFieldViewImage BFImage;

//        protected Graphics m_Graph;
//        private bool m_Hover = false;
//        private bool m_BeginEnter = false;
//        private bool m_BeginLeave = false;
//        public bool MouseHover { get { return m_Hover; } }
//        public bool MouseBeginEnter { get { return m_BeginEnter; } }
//        public bool MouseBeginLeave { get { return m_BeginLeave; } }
//        internal event BFieldMouseHandler OnMouseMoveEvent;
//        internal event BFieldMouseHandler OnMouseLeaveEvent;
//        internal event BFieldMouseHandler OnMouseEnterEvent;
//        internal event BFieldMouseHandler OnMouseDownEvent;
//        internal event BFieldMouseHandler OnMouseUpEvent;


//        public ABFieldView(/*Graphics graph,*/ int index,
//            BattleFieldInfo bfieldinfo,
//            int messionType,
//            Rectangle rec,
//            ABFieldViewImage bfieldImg) {
//            //this.m_Graph = graph;
//            this.Rect = rec;
//            this.BFImage = bfieldImg;
//            //
//            this.BFiledInfo = bfieldinfo;
//            this.Index = index;
//            this.ViewType = messionType;
//        }
//        /// <summary>
//        /// 第几号桌子
//        /// </summary>
//        internal int Index {
//            get;
//            set;
//        }
//        internal BattleFieldInfo BFiledInfo {
//            get;
//            set;
//        }
//        internal int Left { //相对于父容器的位置
//            get { return Rect.Left; }
//            set { Rect.X = value; }
//        }
//        internal int Top {
//            get { return Rect.Top; }
//            set { Rect.Y = value; }
//        }
//        internal int Width {
//            get { return Rect.Width; }
//            set { Rect.Width = value; }
//        }
//        internal int Height {
//            get { return Rect.Height; }
//            set { Rect.Height = value; }
//        }
//        internal Rectangle Rect;

//        internal object UserObject {
//            get;
//            set;
//        }
//        /// <summary>
//        /// 显示类型 比如1：level 2:score,3: 1v1,4: 2v2 5:4v4 6:nouv 7: free
//        /// 与 IRQ_MessionInfo 的任务类型对应
//        /// </summary>
//        internal int ViewType {
//            get;
//            private set;
//        }
//        /// <summary>
//        /// [只读]
//        /// </summary>
//        internal int SeatCount {
//            get { return Seats.Count; }
//        }
//        /// <summary>
//        ///[已实例化对象] 座位列表
//        /// </summary>
//        internal List<ASeatView> Seats = new List<ASeatView>();
//        internal void OnMouseDown(MouseEventArgs e) {
//            if (Rect.Contains(new Point(e.X, e.Y))) {
//                if (OnMouseDownEvent != null) {
//                    OnMouseDownEvent(this);
//                }
//                foreach (var v in Seats) {
//                    v.OnMouseDown(e);
//                }
//            }
//        }
//        internal void OnMouseUp(MouseEventArgs e) {
//            if (Rect.Contains(new Point(e.X, e.Y))) {
//                if (OnMouseUpEvent != null) {
//                    OnMouseUpEvent(this);
//                }
//                foreach (var v in Seats) {
//                    v.OnMouseUp(e);
//                }
//            }
//        }

//        internal void OnMouseMove(MouseEventArgs e) {
//            if (Rect.Contains(new Point(e.X, e.Y))) {
//                if (!m_Hover) {
//                    m_Hover = true;
//                    m_BeginEnter = true;
//                    if (OnMouseEnterEvent != null) {
//                        OnMouseEnterEvent(this);
//                    }
//                }
//                else {
//                    m_BeginEnter = false;
//                }
//                if (OnMouseMoveEvent != null) {
//                    OnMouseMoveEvent(this);
//                }
//                foreach (var v in Seats) {
//                    v.OnMouseMove(e);
//                }
//            }
//            else {
//                if (m_Hover) {
//                    foreach (var v in Seats) {
//                        v.OnMouseMove(e);
//                    }
//                    if (OnMouseLeaveEvent != null) {
//                        OnMouseLeaveEvent(this);
//                    }
//                    m_Hover = false;
//                    m_BeginLeave = true;
//                }
//                else {
//                    m_BeginLeave = false;
//                }
//            }
//        }

//        internal virtual void Draw(Graphics graph) {
//            //if (m_Graph == null) return;
//            this.m_Graph = graph;
//            m_Graph.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
//            m_Graph.SmoothingMode = SmoothingMode.HighQuality;
//            m_Graph.InterpolationMode = InterpolationMode.HighQualityBilinear;
//            m_Graph.CompositingQuality = CompositingQuality.HighQuality;
//            //TODO:画底图
//            DrawBackGround();
//            foreach (var v in Seats) {
//                v.Draw(graph);
//            }
//            //画状态   
//            DrawBFieldStatus();
//            DrawBFIndex();
//        }
//        internal ASeatView GetSeatViewByIndex(int seatIndex) {
//            foreach (var v in this.Seats) {
//                if (v.SeatInfo != null && v.SeatInfo.Index == seatIndex) {
//                    return v;
//                }
//            }
//            return null;
//        }

//        //seatIndex 0开始
//        protected abstract ASeatView CreateSeatView(SeatInfo seatinfo, Rectangle relRec);


//        protected virtual void DrawBackGround() {
//            if (this.BFImage.BackImg != null) {
//                this.m_Graph.DrawImage(this.BFImage.BackImg, this.Rect, 0, 0, this.BFImage.BackImg.Width, this.BFImage.BackImg.Height, GraphicsUnit.Pixel);
//            }
//#if DEBUG
//            //this.m_Graph.DrawRectangle(System.Drawing.Pens.AntiqueWhite, this.Rect);
//#endif
//        }

//        protected virtual void DrawBFieldStatus() {
//            if (this.BFiledInfo != null) {
//                //string drawStr = "";
//                //int drawX = 0;
//                //int drawY = 0;
//                //drawX = this.Left + this.Width / 2 - (/*(int)System.Drawing.SystemFonts.CaptionFont.Size+*/System.Drawing.SystemFonts.CaptionFont.Height) * 2 + (int)(System.Drawing.SystemFonts.CaptionFont.Size / 2);
//                //drawY = this.Top + this.Height / 2 - (int)(System.Drawing.SystemFonts.CaptionFont.Size / 2);
//                //switch (this.BFiledInfo.Status) {
//                //    case BFieldStatus.Free:
//                //        drawStr = "空  闲";
//                //        break;
//                //    case BFieldStatus.ReadyForBattle:
//                //    // break;
//                //    case BFieldStatus.HasSomeOne:
//                //        drawStr = "准  备";
//                //        break;
//                //    case BFieldStatus.InGame:
//                //        drawStr = "游戏中";
//                //        break;
//                //    default:
//                //        break;
//                //}
//                //this.m_Graph.DrawString(drawStr, System.Drawing.SystemFonts.CaptionFont, System.Drawing.SystemBrushes.ActiveCaptionText, drawX, drawY);
//                //以图片模式显示 32*32的尺寸 

//                Rectangle statuGlobRec = new Rectangle(this.Left + this.Width - 32 - 118, this.Top + 4, 24, 24);
//                int w = this.BFImage.Status3Img.Width / 4;
//                int h = this.BFImage.Status3Img.Height;
//                switch (this.BFiledInfo.Status) {
//                    case BFieldStatus.Free:

//                        this.m_Graph.DrawImage(this.BFImage.Status3Img, statuGlobRec, new Rectangle(0 * w, 0, w, h), GraphicsUnit.Pixel);
//                        break;
//                    case BFieldStatus.HasSomeOne:
//                        //#if DEBUG
//                        //                        bool hassomeon=false;
//                        //                        foreach(var v in BFiledInfo.Seats){
//                        //                            if(v.HasUser){
//                        //                                hassomeon=true;
//                        //                                break;
//                        //                            }
//                        //                        } 
//                        //                        System.Diagnostics.Debug.Assert(hassomeon,"没有人的场地与状态不匹配");
//                        //#endif
//                        this.m_Graph.DrawImage(this.BFImage.Status3Img, statuGlobRec, new Rectangle(1 * w, 0, w, h), GraphicsUnit.Pixel);
//                        break;
//                    case BFieldStatus.ReadyForBattle:
//                    //this.m_Graph.DrawImage(this.BFImage.Status3Img, statuGlobRec, new Rectangle(2 * w, 0, w, h), GraphicsUnit.Pixel);
//                    //break;
//                    case BFieldStatus.GameLoading:
//                    case BFieldStatus.ReadyQuit:
//                    case BFieldStatus.InGame:
//                        this.m_Graph.DrawImage(this.BFImage.Status3Img, statuGlobRec, new Rectangle(2 * w, 0, w, h), GraphicsUnit.Pixel);
//                        break;
//                    default:
//                        System.Diagnostics.Debug.Assert(false, "准备状态还没有使用，不应该存在:" + BFieldStatus.ReadyForBattle.ToString());
//                        break;
//                }
//            }
//        }

//        protected virtual void DrawBFIndex() {
//            //string strIndex = string.Format("-- {0} --", this.Index + 1);
//            //int drawX = 0;
//            //int drawY = 0;
//            //drawX = this.Left + this.Width / 2 - (/*(int)System.Drawing.SystemFonts.CaptionFont.Size+*/System.Drawing.SystemFonts.CaptionFont.Height) * 2 + (int)(System.Drawing.SystemFonts.CaptionFont.Size / 2);
//            //drawY = this.Top + this.Height;// -(int)(System.Drawing.SystemFonts.CaptionFont.Size / 2);
//            //this.m_Graph.DrawString(strIndex, System.Drawing.SystemFonts.CaptionFont, System.Drawing.SystemBrushes.ActiveCaptionText, drawX, drawY);
//            //
//            this.DrawNum(this.Index + 1, this.Left + 8, this.Top + 6, 16, 16);//new Rectangle(this.Left + i * 16, this.Top + 6, 16, 16)
//        }
//        protected void DrawNum(int num, int left, int top, int width, int height) {
//            char[] nums = num.ToString().ToCharArray();
//            for (int i = 0; i < nums.Length; i++) {
//                this.m_Graph.DrawImage(this.BFImage.Num10Img, new Rectangle(left + i * 16, top, width, height), new Rectangle((this.BFImage.Num10Img.Width / 10) * int.Parse(nums[i].ToString()), 0, this.BFImage.Num10Img.Width / 10, this.BFImage.Num10Img.Height), GraphicsUnit.Pixel);
//            }
//        }
//        #region IDisposable 成员
//        /// <summary>
//        /// 对图片资源只取消引用 取消事件引用 释放座位信息
//        /// </summary>
//        public virtual void Dispose() {
//            for (int i = this.Seats.Count - 1; i >= 0; i--) {
//                this.Seats[i].Dispose();
//            }
//            this.Seats.Clear();
//            this.Seats = null;

//            this.BFImage = null;

//            this.m_Graph = null;

//            this.m_Hover = false;
//            this.m_BeginEnter = false;
//            this.m_BeginLeave = false;
//            this.OnMouseDownEvent = null;
//            this.OnMouseLeaveEvent = null;
//            this.OnMouseEnterEvent = null;
//            this.OnMouseMoveEvent = null;
//            this.OnMouseUpEvent = null;
//            this.UserObject = null;
//            this.BFiledInfo = null;
//        }

//        #endregion
//    }



//    internal delegate void BFieldMouseHandler(ABFieldView sender);
//    internal delegate void SeatMouseHandler(ASeatView sender);

//    public abstract class ASeatView : IDisposable
//    {

//        private bool m_Hover = false;  //鼠标覆盖
//        private bool m_BeginEnter = false;
//        private bool m_BeginLeave = false;
//        public bool MouseHover { get { return m_Hover; } }
//        public bool MouseBeginEnter { get { return m_BeginEnter; } }
//        public bool MouseBeginLeave { get { return m_BeginLeave; } }
//        private bool m_MouseDown = false;//鼠标按下
//        internal event SeatMouseHandler OnMouseUpEvent;
//        internal event SeatMouseHandler OnMouseMoveEvent;
//        internal event SeatMouseHandler OnMouseLeaveEvent;
//        internal event SeatMouseHandler OnMouseEnterEvent;
//        internal event SeatMouseHandler OnMouseDownEvent;
//        protected ABFieldView m_Owner;
//        //因为要绘制名字 所以房间的间隔 横向至少增加64
//        internal ASeatView(ABFieldView owner, SeatInfo seatinfo,/* Graphics g,*/ Rectangle rec) {
//            this.m_Owner = owner;
//            this.SeatInfo = seatinfo;
//            //this.m_Graph = g;
//            this.Rect = rec;
//            this.Left = rec.Left;
//            this.Top = rec.Top;
//            this.Width = rec.Width;
//            this.Height = rec.Height;
//            m_StrFormat = new StringFormat();
//            m_StrFormat.Alignment = StringAlignment.Near;
//        }
//        protected StringFormat m_StrFormat;
//        /// <summary>
//        /// [只读]
//        /// </summary>
//        internal ABFieldView Owner {
//            get { return m_Owner; }
//        }
//        /// <summary>
//        /// [只读]座位号 0开始
//        /// </summary>
//        internal int Index {
//            get { return this.SeatInfo.Index; }

//        }
//        /// <summary>
//        /// 相对位置 与Owner有关
//        /// </summary>
//        internal int Left {
//            get { return this.Rect.Left; }
//            set { this.Rect.X = value; }
//        }
//        /// <summary>
//        ///  相对位置 与Owner有关
//        /// </summary>
//        internal int Top {
//            get { return this.Rect.Top; }
//            set { this.Rect.Y = value; }
//        }
//        internal int Width {
//            get { return this.Rect.Width; }
//            set { this.Rect.Width = value; }
//        }
//        internal int Height {
//            get { return this.Rect.Height; }
//            set { this.Rect.Height = value; }
//        }
//        /// <summary>
//        /// 相对于BFieldView位置 
//        /// </summary>
//        internal Rectangle Rect;

//        /// <summary>
//        /// 内部创建好模式 后面必须要赋值
//        /// </summary>
//        internal SeatInfo SeatInfo {
//            get;
//            set;
//        }
//        ///// <summary>
//        /// 用户头像，在坐下时获取
//        /// </summary>
//        internal Image PlayImage {
//            get;
//            set;
//        }
//        //internal Image PlayNoFaceImage {
//        //    get;
//        //    set;
//        //}
//        ///// <summary>
//        ///// 用户绘制默认座位图
//        ///// </summary>
//        //internal Image SeatNoManImage {
//        //    get;
//        //    set;
//        //}
//        /// <summary>
//        /// 是否可用 默认为true
//        /// 主要用于 同场协同
//        /// </summary>
//        internal bool Enable = true;
//        internal string PlayName {
//            get;
//            set;
//        }
//        internal object UserObject {
//            get;
//            set;
//        }

//        public virtual void Draw(Graphics g) {
//            if (SeatInfo == null || this.Owner == null)
//                return;

//            Rectangle globRec = new Rectangle(this.Left + this.Owner.Left, this.Top + this.Owner.Top, this.Width, this.Height);
//            if (SeatInfo.HasUser) {
//                this.DrawPlayImage(g, globRec);
//            }
//            else {
//                //绘制默认的图标
//                if (!this.Enable) {
//                    if (this.Owner.BFImage.SeatUnEnableImgRec != null) {
//                        g.DrawImage(this.Owner.BFImage.SeatImage, globRec, this.Owner.BFImage.SeatUnEnableImgRec, GraphicsUnit.Pixel);
//                    }
//                }
//                else if (this.Owner.BFImage.SeatNoManImgRec != null) {
//                    g.DrawImage(this.Owner.BFImage.SeatImage, globRec, this.Owner.BFImage.SeatNoManImgRec, GraphicsUnit.Pixel);
//                }
//            }
//            this.DrawSeatIndex(g, globRec);
//            //依据鼠标状态画效果
//            if (this.m_Hover) {
//                //绘制一层效果
//                if (!this.Enable) { //不处理
//                }
//                else if (this.m_MouseDown) {
//                    DrawRoundRectangleHelper.RenderSelection(g, globRec, 2f, true);
//                }
//                else if (this.m_Hover) {
//                    DrawRoundRectangleHelper.RenderSelection(g, globRec, 2f, this.m_MouseDown);
//                }

//            }

//        }

//        protected virtual void DrawSeatIndex(Graphics g, Rectangle glonRec) {
//            //ToDo：对有需要的绘制
//        }

//        protected virtual void DrawPlayImage(Graphics g, Rectangle globRec) {
//            //画头像
//            if (this.PlayImage == null) {
//                if (this.Owner.BFImage.PlayNoFaceImgRec != null) {
//                    g.DrawImage(this.Owner.BFImage.SeatImage, globRec, this.Owner.BFImage.PlayNoFaceImgRec, GraphicsUnit.Pixel);
//                }
//            }
//            else {
//                g.DrawImage(this.PlayImage, globRec, 0, 0, this.PlayImage.Width, this.PlayImage.Height, GraphicsUnit.Pixel);
//            }
//            //画名字
//            this.DrawName(g, globRec);
//        }
//        protected virtual void DrawName(Graphics g, Rectangle globRec) {
//            if (!string.IsNullOrEmpty(this.PlayName)) {
//                int leftoffset = ((this.Index + 1) % 2) == 0 ? 0 : this.Width / 3;
//                int withoffset = ((this.Index + 1) % 2) == 0 ? this.Width / 3 : this.Width / 4;
//                //ToDo:
//                RectangleF nrc = new RectangleF(globRec.Left + this.m_Owner.BFImage.PlayNameStrRec.Left, globRec.Top + this.m_Owner.BFImage.PlayNameStrRec.Top, this.m_Owner.BFImage.PlayNameStrRec.Width, this.m_Owner.BFImage.PlayNameStrRec.Height);
//                //
//                RectangleF dRec = new RectangleF(this.Left + this.Owner.Left - leftoffset, this.Top + this.Height + this.Owner.Top, 40 + withoffset, 60);
//                nrc = dRec;
//                //SizeF size = this.m_Graph.MeasureString(this.PlayName, System.Drawing.SystemFonts.StatusFont);
//                //float startx = (size.Width > dRec.Width?dRec.X:(dRec.X+(dRec.Width/2-size.Width/2)));
//                //nrc = new RectangleF(startx,dRec.Y,dRec.Width-(startx-dRec.X),dRec.Height);
//                g.DrawString(this.PlayName, System.Drawing.SystemFonts.StatusFont, System.Drawing.SystemBrushes.ActiveCaption, dRec, m_StrFormat);
//            }
//        }

//        internal void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
//            int x = e.X - this.Owner.Left;
//            int y = e.Y - this.Owner.Top;
//            if (this.Rect.Contains(x, y)) {
//                if (!this.m_Hover) {
//                    if (OnMouseEnterEvent != null) {
//                        OnMouseEnterEvent(this);
//                    }
//                    this.m_Hover = true;
//                    this.m_BeginEnter = true;
//                }
//                else {
//                    this.m_BeginEnter = false;
//                }
//                if (OnMouseMoveEvent != null) {
//                    OnMouseMoveEvent(this);
//                }
//            }
//            else {
//                if (this.m_Hover) {
//                    if (OnMouseLeaveEvent != null) {
//                        OnMouseLeaveEvent(this);
//                    }
//                    this.m_Hover = false;
//                    this.m_BeginLeave = true;
//                }
//                else {
//                    this.m_BeginLeave = false;
//                }
//            }
//        }

//        internal void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
//            int x = e.X - this.Owner.Left;
//            int y = e.Y - this.Owner.Top;
//            if (this.Rect.Contains(x, y)) {
//                this.m_MouseDown = true;
//                if (OnMouseDownEvent != null) {
//                    OnMouseDownEvent(this);
//                }
//            }
//            else {
//                this.m_MouseDown = false;
//            }
//        }

//        internal void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
//            int x = e.X - this.Owner.Left;
//            int y = e.Y - this.Owner.Top;
//            if (m_MouseDown && m_Hover && OnMouseUpEvent != null) {
//                OnMouseUpEvent(this);
//            }
//            this.m_MouseDown = false;
//        }
//        internal void SetReDrawOk() {
//            this.m_BeginEnter = false;
//            this.m_BeginLeave = false;
//        }
//        /// <summary>
//        /// 只有在特殊情况下不能刷新才会使用
//        /// </summary>
//        /// <param name="mouseHover"></param>
//        internal void SetMouseHoverState(bool mouseHover) {
//            // SGD ：2013/5/3 17:31:09
//            // 说明：当意外失去焦点没有触发事件时 增加hover也设置为false
//            this.m_Hover = false;
//        }
//        #region IDisposable 成员

//        public void Dispose() {
//            this.PlayImage = null;
//            //this.PlayNoFaceImage = null;
//            this.UserObject = null;
//            this.SeatInfo = null;
//            //this.SeatNoManImage = null;
//            this.m_MouseDown = false;
//            this.m_Hover = false;
//            this.m_BeginEnter = false;
//            this.m_BeginLeave = false;
//            this.OnMouseDownEvent = null;
//            this.OnMouseLeaveEvent = null;
//            this.OnMouseEnterEvent = null;
//            this.OnMouseMoveEvent = null;
//            this.OnMouseUpEvent = null;
//        }

//        #endregion
//    }

//    /// <summary>
//    /// 绘制边界半透明层
//    /// </summary>
//    public static class DrawRoundRectangleHelper
//    {

//        //
//        public static void RenderSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
//            System.Drawing.Color[] colorArray;
//            float[] numArray;
//            ColorBlend blend;
//            LinearGradientBrush brush;
//            System.Drawing.Color[] colorArray2;

//            if (rectangle.Height <= 0) {
//                rectangle.Height = 1;
//            }
//            if (rectangle.Width <= 0) {
//                rectangle.Width = 1;
//            }

//            if (pressed) {
//                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0xfe, 0xd8, 170), System.Drawing.Color.FromArgb(70, 0xfb, 0xb5, 0x65), System.Drawing.Color.FromArgb(70, 250, 0x9d, 0x34), System.Drawing.Color.FromArgb(70, 0xfd, 0xee, 170) };

//                //colorArray = colorArray2;
//                //numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
//                Color c1 = GetColor(Color.Green, 1);

//                //颜色
//                colorArray2 = new System.Drawing.Color[] { 
//                    System.Drawing.Color.FromArgb(90, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(05, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(15, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(60, c1.R, c1.G, c1.B) };

//                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


//                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0, 0, 0), System.Drawing.Color.FromArgb(70, 10, 10, 10), System.Drawing.Color.FromArgb(70, 20, 20, 20), System.Drawing.Color.FromArgb(70, 50, 50, 50) };
//                colorArray = colorArray2;
//                numArray = new float[] { 0f, 0.4f, 0.85f, 1.0f }; //调整对应颜色的位置
//                blend = new ColorBlend();
//                blend.Colors = colorArray;
//                blend.Positions = numArray;
//                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
//                brush.InterpolationColors = blend;
//                FillRoundRectangle(g, brush, rectangle, 2f);
//                using (Pen p = new Pen(System.Drawing.Color.FromArgb(0xab, 0xa1, 140))) {
//                    DrawRoundRectangle(g, p, rectangle, radius);
//                }
//                rectangle.Offset(1, 1);
//                rectangle.Width -= 2;
//                rectangle.Height -= 2;
//                //DrawRoundRectangle(g, new Pen(new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xdf, 0xb7, 0x88), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)), rectangle, radius);
//            }
//            else {
//                Color c1 = GetColor(Color.White, 1);
//                colorArray2 = new System.Drawing.Color[] { 
//                    System.Drawing.Color.FromArgb(90, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(05, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(15, c1.R, c1.G, c1.B), 
//                    System.Drawing.Color.FromArgb(60, c1.R, c1.G, c1.B) };

//                //colorArray2 = new Color[] { RibbonTabControl.GetColor(1.125), RibbonTabControl.GetColor(1.075), RibbonTabControl.GetColor(1.0), RibbonTabControl.GetColor(1.075) };


//                //colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(70, 0, 0, 0), System.Drawing.Color.FromArgb(70, 10, 10, 10), System.Drawing.Color.FromArgb(70, 20, 20, 20), System.Drawing.Color.FromArgb(70, 50, 50, 50) };
//                colorArray = colorArray2;
//                numArray = new float[] { 0f, 0.35f, 0.8f, 1.0f };
//                blend = new ColorBlend();
//                blend.Colors = colorArray;
//                blend.Positions = numArray;
//                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
//                brush.InterpolationColors = blend;
//                FillRoundRectangle(g, brush, rectangle, 2f);
//                using (Pen p = new Pen(System.Drawing.Color.FromArgb(210, 0xc0, 0x8d))) {
//                    DrawRoundRectangle(g, p, rectangle, radius);
//                }
//                rectangle.Offset(1, 1);
//                rectangle.Width -= 2;
//                rectangle.Height -= 2;

//                using (LinearGradientBrush brush2 = new LinearGradientBrush(rectangle,
//                    Color.FromArgb(0xff, 0xff, 0xf7),
//                    Color.Transparent, LinearGradientMode.ForwardDiagonal)) {

//                    using (Pen p2 = new Pen(brush2)) {
//                        DrawRoundRectangle(g, p2, rectangle, 2f);
//                    }
//                }
//            }
//            brush.Dispose();

//        }
//        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
//            float width = radius * 2f;
//            GraphicsPath path = new GraphicsPath();
//            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
//            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
//            path.CloseFigure();
//            g.FillPath(brush, path);
//            path.Dispose();
//        }
//        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
//            float width = radius * 2f;
//            GraphicsPath path = new GraphicsPath();
//            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (rectangle.Y + rectangle.Height) - width, width, width, 0f, 90f);
//            path.AddArc((float)rectangle.X, (rectangle.Y + rectangle.Height) - width, width, width, 90f, 90f);
//            path.CloseFigure();
//            g.DrawPath(pen, path);
//            path.Dispose();
//        }
//        //tab 
//        public static void RenderTopSelection(Graphics g, Rectangle rectangle, float radius, bool pressed) {
//            System.Drawing.Color[] colorArray;
//            float[] numArray;
//            ColorBlend blend;
//            LinearGradientBrush brush;
//            System.Drawing.Color[] colorArray2;
//            if (pressed) {
//                colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(0xfe, 0xd8, 170), System.Drawing.Color.FromArgb(0xfb, 0xb5, 0x65), System.Drawing.Color.FromArgb(250, 0x9d, 0x34), System.Drawing.Color.FromArgb(0xfd, 0xee, 170) };
//                colorArray = colorArray2;
//                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
//                blend = new ColorBlend();
//                blend.Colors = colorArray;
//                blend.Positions = numArray;
//                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
//                brush.InterpolationColors = blend;
//                FillTopRoundRectangle(g, brush, rectangle, 2f);
//                using (Pen p1 = new Pen(System.Drawing.Color.FromArgb(0xab, 0xa1, 140))) {
//                    DrawTopRoundRectangle(g, p1, rectangle, radius);
//                }
//                rectangle.Offset(1, 1);
//                rectangle.Width -= 2;
//                rectangle.Height -= 2;
//                using (LinearGradientBrush lgb1 = new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xdf, 0xb7, 0x88), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
//                    using (Pen p1 = new Pen(lgb1)) {
//                        DrawTopRoundRectangle(g, p1, rectangle, radius);
//                    }
//                }
//            }
//            else {
//                colorArray2 = new System.Drawing.Color[] { System.Drawing.Color.FromArgb(0xff, 0xfe, 0xe3), System.Drawing.Color.FromArgb(0xff, 0xe7, 0x97), System.Drawing.Color.FromArgb(0xff, 0xd7, 80), System.Drawing.Color.FromArgb(0xff, 0xe7, 150) };
//                colorArray = colorArray2;
//                numArray = new float[] { 0f, 0.4f, 0.4f, 1f };
//                blend = new ColorBlend();
//                blend.Colors = colorArray;
//                blend.Positions = numArray;
//                brush = new LinearGradientBrush(rectangle, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, LinearGradientMode.Vertical);
//                brush.InterpolationColors = blend;
//                FillTopRoundRectangle(g, brush, rectangle, 2f);
//                using (Pen p1 = new Pen(System.Drawing.Color.FromArgb(210, 0xc0, 0x8d))) {
//                    DrawTopRoundRectangle(g, p1, rectangle, radius);
//                }
//                rectangle.Offset(1, 1);
//                rectangle.Width -= 2;
//                rectangle.Height -= 2;
//                using (LinearGradientBrush lgb1 = new LinearGradientBrush(rectangle, System.Drawing.Color.FromArgb(0xff, 0xff, 0xf7), System.Drawing.Color.Transparent, LinearGradientMode.ForwardDiagonal)) {
//                    using (Pen p1 = new Pen(lgb1)) {
//                        DrawTopRoundRectangle(g, p1, rectangle, 2f);
//                    }
//                }
//            }
//            brush.Dispose();
//        }
//        public static void FillTopRoundRectangle(Graphics g, Brush brush, Rectangle rectangle, float radius) {
//            float width = radius * 2f;
//            GraphicsPath path = new GraphicsPath();
//            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
//            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
//            path.CloseFigure();
//            g.FillPath(brush, path);
//            path.Dispose();
//        }

//        public static void DrawTopRoundRectangle(Graphics g, Pen pen, Rectangle rectangle, float radius) {
//            float width = radius * 2f;
//            GraphicsPath path = new GraphicsPath();
//            path.AddArc((float)rectangle.X, (float)rectangle.Y, width, width, 180f, 90f);
//            path.AddArc((rectangle.X + rectangle.Width) - width, (float)rectangle.Y, width, width, 270f, 90f);
//            path.AddLine(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.X, rectangle.Y + rectangle.Height);
//            path.CloseFigure();
//            g.DrawPath(pen, path);
//            path.Dispose();

//        }


//        public static System.Drawing.Color GetColor(Color c, double luminance) {
//            System.Drawing.Color color = HSL2RGB(((double)c.GetHue()) / 360.0, ((double)c.GetSaturation()) / 2.0, c.GetBrightness() * 1.025);
//            double num = color.R * luminance;
//            double num2 = color.G * luminance;
//            double num3 = color.B * luminance;
//            if (num > 255.0) {
//                num = 255.0;
//            }
//            if (num2 > 255.0) {
//                num2 = 255.0;
//            }
//            if (num3 > 255.0) {
//                num3 = 255.0;
//            }
//            return System.Drawing.Color.FromArgb((int)num, (int)num2, (int)num3);
//        }
//        public static System.Drawing.Color HSL2RGB(double hue, double saturation, double luminance) {
//            double num = 0.0;
//            double num2 = 0.0;
//            double num3 = 0.0;
//            if (luminance == 0.0) {
//                num = num2 = num3 = 0.0;
//            }
//            else if (saturation == 0.0) {
//                num = num2 = num3 = luminance;
//            }
//            else {
//                double num4 = (luminance <= 0.5) ? (luminance * (1.0 + saturation)) : ((luminance + saturation) - (luminance * saturation));
//                double num5 = (2.0 * luminance) - num4;
//                double[] numArray = new double[] { hue + 0.33333333333333331, hue, hue - 0.33333333333333331 };
//                double[] numArray2 = new double[3];
//                for (int i = 0; i < 3; i++) {
//                    if (numArray[i] < 0.0) {
//                        numArray[i]++;
//                    }
//                    if (numArray[i] > 1.0) {
//                        numArray[i]--;
//                    }
//                    if ((6.0 * numArray[i]) < 1.0) {
//                        numArray2[i] = num5 + (((num4 - num5) * numArray[i]) * 6.0);
//                    }
//                    else if ((2.0 * numArray[i]) < 1.0) {
//                        numArray2[i] = num4;
//                    }
//                    else if ((3.0 * numArray[i]) < 2.0) {
//                        numArray2[i] = num5 + (((num4 - num5) * (0.66666666666666663 - numArray[i])) * 6.0);
//                    }
//                    else {
//                        numArray2[i] = num5;
//                    }
//                }
//                num = numArray2[0];
//                num2 = numArray2[1];
//                num3 = numArray2[2];
//            }
//            if (num > 1.0) {
//                num = 1.0;
//            }
//            if (num2 > 1.0) {
//                num2 = 1.0;
//            }
//            if (num3 > 1.0) {
//                num3 = 1.0;
//            }
//            return System.Drawing.Color.FromArgb((int)(255.0 * num), (int)(255.0 * num2), (int)(255.0 * num3));
//        }
//    }
}
