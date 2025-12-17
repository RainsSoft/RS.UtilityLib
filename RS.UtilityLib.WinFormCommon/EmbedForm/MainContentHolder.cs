using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
//


namespace RS.UtilityLib.WinFormCommon.EmbedForm
{
    /// <summary>
    /// 主内容窗口容器（支持内容切换）
    /// </summary>
    public partial class MainContentHolder : UserControl {
        /// <summary>
        /// 窗口字典
        /// </summary>
        private IDictionary<string, Form> _contentFormDict = new Dictionary<string, Form>();
        /// <summary>
        /// 窗口列表
        /// </summary>
        private IList<Form> _contentFormList = new List<Form>();

        /// <summary>
        /// 当前显示的窗口引用
        /// </summary>
        private Form _currentShowForm = null;
        /// <summary>
        /// 当前显示的窗口的标识名
        /// </summary>
        private string _currentShowFormName = string.Empty;

        /// <summary>
        /// 切换动画持续时间
        /// </summary>
        private int _switchMilliseconds = 150;
        /// <summary>
        /// 切换动画模式
        /// </summary>
        private WindowSwitchMode _switchMode = WindowSwitchMode.Horhorizontal;
        /// <summary>
        /// 切换动画方向
        /// </summary>
        private WindowSwitchDirection _switchDirection = WindowSwitchDirection.Positive;

        /// <summary>
        /// 全局切换标志，任何一个实例处于切换状态时，该标志被设置为true
        /// </summary>
        private static bool SwitchSignal = false;

        [Description("窗口切换动画的移出和移入持续时间")]
        public int SwitchMilliseconds {
            get {
                return _switchMilliseconds;
            }
            set {
                _switchMilliseconds = value;
            }
        }

        [Description("窗口切换动画的切换方式")]
        public WindowSwitchMode SwitchMode {
            get {
                return _switchMode;
            }
            set {
                _switchMode = value;
            }
        }

        [Description("窗口切换动画的切换方向")]
        public WindowSwitchDirection SwitchDirection {
            get {
                return _switchDirection;
            }
            set {
                _switchDirection = value;
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;

                //设置控件的WS_EX_COMPOSITED样式，使窗口背景刷新时控件不闪烁，但该标志会影响窗口滑动切换的动画效果
                //因此在MainContentHolder.SwitchToForm函数中，窗口动画开始前会禁用该标志，动画结束后重新打开该标志
                cp.ExStyle |= (int)NativeConst.WS_EX_COMPOSITED;  // Turn on WS_EX_COMPOSITED

                return cp;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainContentHolder() {
            InitializeComponent();
        }

        /// <summary>
        /// 向窗口容器中添加指定名称的窗口实例
        /// </summary>
        /// <param name="name">窗口名称</param>
        /// <param name="contentForm">窗口实例</param>
        public void AddContentForm(string name, Form contentForm) {
            if(string.IsNullOrEmpty(name)) {
                throw new Exception("向MainContentHolder中添加了未命名的Form实例！");
            }
            if(contentForm == null) {
                throw new Exception("向MainContentHolder中添加了名为" + name + "空Form实例！");
            }

            contentForm.Dock = DockStyle.None;
            contentForm.FormBorderStyle = FormBorderStyle.None; //设置窗体无边框。
            contentForm.Location = new Point(0, 0);
            contentForm.Size = new Size(1, 1);
            contentForm.TopLevel = false;                       //设置不以顶级窗口显示。
            this.pnLoader.Controls.Add(contentForm);            //先加到pnLoader中，使窗口初次绘图
            contentForm.Show();                                 //窗口初次绘图

            //WindowAnimator.AnimateWindow(contentForm.Handle, 1, WindowAnimator.AW_HIDE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_POSITIVE);
            this.pnMain.Controls.Add(contentForm);              //将窗口移动到主显示Panel中
            //contentForm.Dock = DockStyle.None ;               //设置窗口停靠模式为Fill
            contentForm.Size = new Size(1, 1);

            if(_contentFormDict.ContainsKey(name))             //如果新加入的窗口与已有窗口同名，则卸载旧窗口，安装新窗口
            {
                RemoveContentForm(name);
            }

            _contentFormDict.Add(name, contentForm);
            _contentFormList.Add(contentForm);

        }

        /// <summary>
        /// 从窗口容器容器中移除指定名称的窗口实例
        /// </summary>
        /// <param name="name">窗口名称</param>
        public void RemoveContentForm(string name) {
            if(_contentFormDict.ContainsKey(name)
                && _contentFormDict[name] != null) {
                RemoveContentForm(_contentFormDict[name]);
            }

        }

        /// <summary>
        /// 从窗口容器容器中移除指定实例的窗口
        /// </summary>
        /// <param name="contentForm">窗口实例</param>
        public void RemoveContentForm(Form contentForm) {
            if(contentForm != null) {
                string formName = null;
                foreach(string key in _contentFormDict.Keys) {
                    if(_contentFormDict[key].Equals(contentForm)) {
                        formName = key;                             //获得指定窗口实例对应的窗口名
                    }
                }
                if(formName != null)                                  //从窗口字典移除窗口
                {
                    _contentFormDict.Remove(formName);
                }
                _contentFormList.Remove(contentForm);               //从窗口列表移除窗口

                if(_currentShowForm == contentForm)                 //若移除的窗口实例是当前正在显示的窗口实例，则取消当前显示窗口变量的引用
                {
                    _currentShowForm = null;
                    _currentShowFormName = string.Empty;
                }

                this.pnMain.Controls.Remove(contentForm);           //从主显示panel中移除重复的窗口
                contentForm.Close();                                //关闭窗口
            }
        }

        /// <summary>
        /// 清除容器中所有窗口实例
        /// </summary>
        public void Clear() {
            List<Form> formList = new List<Form>();
            foreach(Form form in _contentFormDict.Values) {
                formList.Add(form);
            }

            formList.ForEach(f => {
                RemoveContentForm(f);
            });
        }

        public void OnContainerShown() {
            for(int i = 0; i < _contentFormList.Count; i++) {
                Form form = _contentFormList[i];
                if(form != null) {
                    //将窗口隐藏
                    WindowAnimator.AnimateWindow(form.Handle, 1, WindowAnimator.AW_HIDE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_POSITIVE);
                }
            }
            //for (int i = 0; i < _contentFormList.Count; i++)
            //{
            //    Form form = _contentFormList[i];
            //    if (form != null)
            //    {
            //        this.pnMain.Controls.Add(form);  //将窗口移动到主显示Panel中
            //    }
            //}
            for(int i = 0; i < _contentFormList.Count; i++) {
                Form form = _contentFormList[i];
                if(form != null) {
                    form.Dock = DockStyle.Fill;      //设置窗口停靠模式为Fill
                }
            }
        }

        public void SwitchToForm(string name) {
            if(_currentShowFormName == name) {
                return;
            }

            Form toForm = null;
            if(_contentFormDict.ContainsKey(name)) {
                toForm = _contentFormDict[name];
            }
            if(toForm == null) {
                throw new Exception("主内容区无法切换到名为" + name + "的Content窗口！");
            }

            //取消窗口WS_EX_COMPOSITED样式，以解决本控件切换内容窗口时图像卡顿的问题
            UnsetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);

            if(_currentShowForm != null) {
                int currentShowFormIndex = _contentFormList.IndexOf(_currentShowForm);
                int toFormIndex = _contentFormList.IndexOf(toForm);

                //toForm.Dock = DockStyle.Fill;

                //if (toFormIndex > currentShowFormIndex)
                //{
                //    WindowAnimator.AnimateWindow(_currentShowForm.Handle, _switchMilliseconds,
                //        WindowAnimator.AW_HIDE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_NEGATIVE);
                //    WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                //        WindowAnimator.AW_ACTIVATE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_NEGATIVE);
                //}
                //else
                //{
                //    WindowAnimator.AnimateWindow(_currentShowForm.Handle, _switchMilliseconds,
                //        WindowAnimator.AW_HIDE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_POSITIVE);
                //    WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                //        WindowAnimator.AW_ACTIVATE | WindowAnimator.AW_SLIDE | WindowAnimator.AW_HOR_POSITIVE);
                //}
                WindowAnimator.AnimateWindow(_currentShowForm.Handle, _switchMilliseconds,
                       WindowAnimator.AW_HIDE | GetWindowSwitchFlags(toFormIndex > currentShowFormIndex));
                WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                    WindowAnimator.AW_ACTIVATE | GetWindowSwitchFlags(toFormIndex > currentShowFormIndex));

                //_currentShowForm.Dock = DockStyle.None;
                //_currentShowForm.Location = new Point(0, 0);
                //_currentShowForm.Size = new Size(1, 1);
            }
            else {
                //toForm.Dock = DockStyle.Fill;

                WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                    WindowAnimator.AW_ACTIVATE | GetWindowSwitchFlags(true));
            }
            _currentShowForm = toForm;
            _currentShowFormName = name;

            //子窗口含有Tab和MainContentHolder时，切换时调用该接口函数，
            //完成其中下一级子窗体的延迟加载
            ISubContentHolder subContentHolder = _currentShowForm as ISubContentHolder;
            if(subContentHolder != null) {
                //执行下一级子窗体延迟加载
                subContentHolder.OnFirstShow();
            }

            //设置窗口WS_EX_COMPOSITED样式，以解决本控件单独刷新时闪烁的问题
            SetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);
        }

        public void SwitchToForm(string name, int switchFlags) {
            if(_currentShowFormName == name) {
                return;
            }

            Form toForm = null;
            if(_contentFormDict.ContainsKey(name)) {
                toForm = _contentFormDict[name];
            }
            if(toForm == null) {
                throw new Exception("主内容区无法切换到名为" + name + "的Content窗口！");
            }

            //取消窗口WS_EX_COMPOSITED样式，以解决本控件切换内容窗口时图像卡顿的问题
            UnsetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);

            if(_currentShowForm != null) {

                //toForm.Dock = DockStyle.Fill;

                WindowAnimator.AnimateWindow(_currentShowForm.Handle, _switchMilliseconds,
                    WindowAnimator.AW_HIDE | switchFlags);
                WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                    WindowAnimator.AW_ACTIVATE | switchFlags);

                //_currentShowForm.Dock = DockStyle.None;
                //_currentShowForm.Location = new Point(0, 0);
                //_currentShowForm.Size = new Size(1, 1);
            }
            else {
                //toForm.Dock = DockStyle.Fill;
                WindowAnimator.AnimateWindow(toForm.Handle, _switchMilliseconds,
                    WindowAnimator.AW_ACTIVATE | switchFlags);
            }
            _currentShowForm = toForm;
            _currentShowFormName = name;

            //子窗口含有Tab和MainContentHolder时，切换时调用该接口函数，
            //完成其中下一级子窗体的延迟加载
            ISubContentHolder subContentHolder = _currentShowForm as ISubContentHolder;
            if(subContentHolder != null) {
                //执行下一级子窗体延迟加载
                subContentHolder.OnFirstShow();
            }

            //设置窗口WS_EX_COMPOSITED样式，以解决本控件单独刷新时闪烁的问题
            SetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);
        }

        /// <summary>
        /// 直接显示指定名称的窗口
        /// </summary>
        /// <param name="name"></param>
        public void ShowForm(string name) {
            if(_currentShowFormName == name) {
                return;
            }

            Form toForm = null;
            if(_contentFormDict.ContainsKey(name)) {
                toForm = _contentFormDict[name];
            }
            if(toForm == null) {
                throw new Exception("主内容区无法切换到名为" + name + "的Content窗口！");
            }

            //取消窗口WS_EX_COMPOSITED样式，以解决本控件切换内容窗口时图像卡顿的问题
            UnsetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);

            if(_currentShowForm != null) {
                NativeMethods.ShowWindow(_currentShowForm.Handle, NativeConst.SW_HIDE);
                NativeMethods.ShowWindow(toForm.Handle, NativeConst.SW_SHOW);
            }
            else {
                NativeMethods.ShowWindow(toForm.Handle, NativeConst.SW_SHOW);
            }
            _currentShowForm = toForm;
            _currentShowFormName = name;

            //子窗口含有Tab和MainContentHolder时，切换时调用该接口函数，
            //完成其中下一级子窗体的延迟加载
            ISubContentHolder subContentHolder = _currentShowForm as ISubContentHolder;
            if(subContentHolder != null) {
                //执行下一级子窗体延迟加载
                subContentHolder.OnFirstShow();
            }

            //设置窗口WS_EX_COMPOSITED样式，以解决本控件单独刷新时闪烁的问题
            SetStyle(NativeConst.GWL_EXSTYLE, NativeConst.WS_EX_COMPOSITED);
        }

        /// <summary>
        /// 获取窗口切换动画的参数
        /// </summary>
        /// <param name="dynamicSwitchDirection">动态切换方向，根据当前窗口与即将切换到的窗口的顺序决定</param>
        /// <returns></returns>
        private int GetWindowSwitchFlags(bool dynamicSwitchDirection) {
            int flag = 0;
            if(this._switchMode == WindowSwitchMode.Horhorizontal
                || this._switchMode == WindowSwitchMode.Vertical) {
                flag |= WindowAnimator.AW_SLIDE;
                if(this._switchMode == WindowSwitchMode.Vertical) {
                    if(this._switchDirection == WindowSwitchDirection.Positive) {
                        if(dynamicSwitchDirection) {
                            flag |= WindowAnimator.AW_VER_POSITIVE;
                        }
                        else {
                            flag |= WindowAnimator.AW_VER_NEGATIVE;
                        }
                    }
                    else {
                        if(dynamicSwitchDirection) {
                            flag |= WindowAnimator.AW_VER_NEGATIVE;
                        }
                        else {
                            flag |= WindowAnimator.AW_VER_POSITIVE;
                        }
                    }
                }
                else {
                    if(this._switchDirection == WindowSwitchDirection.Positive) {
                        if(dynamicSwitchDirection) {
                            flag |= WindowAnimator.AW_HOR_NEGATIVE;
                        }
                        else {
                            flag |= WindowAnimator.AW_HOR_POSITIVE;
                        }
                    }
                    else {
                        if(dynamicSwitchDirection) {
                            flag |= WindowAnimator.AW_HOR_POSITIVE;
                        }
                        else {
                            flag |= WindowAnimator.AW_HOR_NEGATIVE;
                        }
                    }
                }
            }
            else {
                flag |= WindowAnimator.AW_CENTER;
            }

            return flag;
        }

        public Form GetContentForm(string name) {
            if(_contentFormDict.ContainsKey(name)) {
                return _contentFormDict[name] as Form;
            }
            else {
                return null;
            }
        }


        /// <summary>
        /// 设置当前控件及其相同类型父控件的Window Style或者Extended Window Styles
        /// </summary>
        /// <param name="nIndex">偏移量，取GWL_和DWL_开头的常量</param>
        /// <param name="windowStyle">窗口样式常量，取WS_和WS_EX_开头的常量</param>
        public void SetStyle(int nIndex, long windowStyle) {
            Control currentControl = this;
            while(currentControl != null) {
                //如果当前控件是MainContentHolder类型控件，则设置该控件的样式
                MainContentHolder holderControl = currentControl as MainContentHolder;
                if(holderControl != null) {
                    CommonFunctions.SetWindowStyle(holderControl.Handle, nIndex, windowStyle);
                }

                ////如果当前控件是DUIForm类型控件，则设置该控件的样式
                //DUIForm duiForm = currentControl as DUIForm;
                //if (duiForm != null)
                //{
                //    CommonFunctions.SetWindowStyle(duiForm.Handle, nIndex, windowStyle);
                //}

                //继续处理父控件
                currentControl = currentControl.Parent;
            }
        }

        /// <summary>
        /// 取消设置当前控件及其相同类型父控件的Window Style或者Extended Window Styles
        /// </summary>
        /// <param name="nIndex">偏移量，取GWL_和DWL_开头的常量</param>
        /// <param name="windowStyle">窗口样式常量，取WS_和WS_EX_开头的常量</param>
        public void UnsetStyle(int nIndex, long windowStyle) {
            Control currentControl = this;
            while(currentControl != null) {
                //如果当前控件是MainContentHolder类型控件，则设置该控件的样式
                MainContentHolder holderControl = currentControl as MainContentHolder;
                if(holderControl != null) {
                    CommonFunctions.UnsetWindowStyle(holderControl.Handle, nIndex, windowStyle);
                }

                ////如果当前控件是DUIForm类型控件，则设置该控件的样式
                //DUIForm duiForm = currentControl as DUIForm;
                //if (duiForm != null)
                //{
                //    CommonFunctions.UnsetWindowStyle(duiForm.Handle, nIndex, windowStyle);
                //}

                //继续处理父控件
                currentControl = currentControl.Parent;
            }
        }

    }

    /// <summary>
    /// 窗口切换模式
    /// </summary>
    public enum WindowSwitchMode {
        /// <summary>
        /// 水平切换
        /// </summary>
        Horhorizontal,

        /// <summary>
        /// 垂直切换
        /// </summary>
        Vertical,

        /// <summary>
        /// 中心向外围切换
        /// </summary>
        Center
    }

    /// <summary>
    /// 窗口切换方向
    /// </summary>
    public enum WindowSwitchDirection {
        /// <summary>
        /// 正向
        /// </summary>
        Positive,

        /// <summary>
        /// 反向
        /// </summary>
        Nagetive
    }
    /// <summary>
    /// 子窗口内容容器接口，用户子窗口内容延迟加载
    /// </summary>
    public interface ISubContentHolder {
        /// <summary>
        /// 第一次切换到本窗口时由MainContentHolder调用的函数
        /// 用于延迟加载窗口子内容
        /// </summary>
        void OnFirstShow();
    }
    #region Native
    /// <summary>
    /// win32消息及其他常量定义
    /// </summary>
    public static class NativeConst {
        public const int WM_CREATE = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_MOVE = 0x0003;
        public const int WM_SIZE = 0x0005;
        public const int WM_ACTIVATE = 0x0006;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_ENABLE = 0x000A;
        public const int WM_SETREDRAW = 0x000B;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_GETTEXT = 0x000D;
        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int WM_PAINT = 0x000F;
        public const int WM_CLOSE = 0x0010;
        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_QUIT = 0x0012;
        public const int WM_QUERYOPEN = 0x0013;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_SYSCOLORCHANGE = 0x0015;
        public const int WM_ENDSESSION = 0x0016;
        public const int WM_SHOWWINDOW = 0x0018;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_FONTCHANGE = 0x001D;
        public const int WM_TIMECHANGE = 0x001E;
        public const int WM_CANCELMODE = 0x001F;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int WM_CHILDACTIVATE = 0x0022;
        public const int WM_QUEUESYNC = 0x0023;
        public const int WM_GETMINMAXINFO = 0x0024;
        public const int WM_PAINTICON = 0x0026;
        public const int WM_ICONERASEBKGND = 0x0027;
        public const int WM_NEXTDLGCTL = 0x0028;
        public const int WM_SPOOLERSTATUS = 0x002A;
        public const int WM_DRAWITEM = 0x002B;
        public const int WM_MEASUREITEM = 0x002C;
        public const int WM_VKEYTOITEM = 0x002E;
        public const int WM_CHARTOITEM = 0x002F;
        public const int WM_SETFONT = 0x0030;
        public const int WM_GETFONT = 0x0031;
        public const int WM_SETHOTKEY = 0x0032;
        public const int WM_GETHOTKEY = 0x0033;
        public const int WM_QUERYDRAGICON = 0x0037;
        public const int WM_COMPAREITEM = 0x0039;
        public const int WM_COMPACTING = 0x0041;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_POWER = 0x0048;
        public const int WM_COPYDATA = 0x004A;
        public const int WM_CANCELJOURNA = 0x004B;
        public const int WM_NOTIFY = 0x004E;
        public const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public const int WM_INPUTLANGCHANGE = 0x0051;
        public const int WM_TCARD = 0x0052;
        public const int WM_HELP = 0x0053;
        public const int WM_USERCHANGED = 0x0054;
        public const int WM_NOTIFYFORMAT = 0x0055;
        public const int WM_CONTEXTMENU = 0x007B;
        public const int WM_STYLECHANGING = 0x007C;
        public const int WM_STYLECHANGED = 0x007D;
        public const int WM_DISPLAYCHANGE = 0x007E;
        public const int WM_GETICON = 0x007F;
        public const int WM_SETICON = 0x0080;
        public const int WM_NCCREATE = 0x0081;
        public const int WM_NCDESTROY = 0x0082;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCPAINT = 0x0085;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_GETDLGCODE = 0x0087;
        public const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_NCLBUTTONUP = 0x00A2;
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        public const int WM_NCRBUTTONUP = 0x00A5;
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        public const int WM_NCMBUTTONUP = 0x00A8;
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_DEADCHAR = 0x0103;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_SYSDEADCHAR = 0x0107;
        public const int WM_INITDIALOG = 0x0110;
        public const int WM_COMMAND = 0x0111;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_TIMER = 0x0113;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_INITMENU = 0x0116;
        public const int WM_INITMENUPOPUP = 0x0117;
        public const int WM_MENUSELECT = 0x011F;
        public const int WM_MENUCHAR = 0x0120;
        public const int WM_ENTERIDLE = 0x0121;
        public const int WM_CTLCOLORMSGBOX = 0x0132;
        public const int WM_CTLCOLOREDIT = 0x0133;
        public const int WM_CTLCOLORLISTBOX = 0x0134;
        public const int WM_CTLCOLORBTN = 0x0135;
        public const int WM_CTLCOLORDLG = 0x0136;
        public const int WM_CTLCOLORSCROLLBAR = 0x0137;
        public const int WM_CTLCOLORSTATIC = 0x0138;
        public const int WM_SHARED_MENU = 0x01E2;
        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_PRINT = 0x0317;

        public const int HC_ACTION = 0;
        public const int WH_CALLWNDPROC = 4;

        //LONG GetWindowLong(HWND hWnd，int nlndex)函数nIndex取值,以及
        //LONG SetWindowLong（HWND hWnd，int nlndex，LONG dwNewLong）函数nIndex取值
        public const int GWL_EXSTYLE = -20;  //获得扩展窗口风格。
        public const int GWL_STYLE = -16;  //获得窗口风格。
        public const int GWL_WNDPROC = -4;   //获得窗口过程的地址，或代表窗口过程的地址的句柄。必须使用CallWindowProc函数调用窗口过程。
        public const int GWL_HINSTANCE = -6;   //获得应用事例的句柄。
        public const int GWL_HWNDPARENT = -8;   //如果父窗口存在，获得父窗口句柄。
        public const int GWL_ID = -12;  //获得窗口标识。
        public const int GWL_USERDATA = -21;  //获得与窗口有关的32位值。每一个窗口均有一个由创建该窗口的应用程序使用的32位值。
                                              //在hWnd参数标识了一个对话框时也可用下列值：
        public const int DWL_DLGPROC = 4;    //获得对话框过程的地址，或一个代表对话框过程的地址的句柄。必须使用函数CallWindowProc来调用对话框过程。
        public const int DWL_MSGRESULT = 0;    //获得在对话框过程中一个消息处理的返回值。
        public const int DWL_USER = 8;    //获得应用程序私有的额外信息，例如一个句柄或指针。[1]

        //Extended Window Styles
        public const long WS_EX_ACCEPTFILES = 0x00000010;
        public const long WS_EX_APPWINDOW = 0x00040000;
        public const long WS_EX_CLIENTEDGE = 0x00000200;
        public const long WS_EX_COMPOSITED = 0x02000000;
        public const long WS_EX_CONTEXTHELP = 0x00000400;
        public const long WS_EX_CONTROLPARENT = 0x00010000;
        public const long WS_EX_DLGMODALFRAME = 0x00000001;
        public const long WS_EX_LAYERED = 0x00080000;
        public const long WS_EX_LAYOUTRTL = 0x00400000;
        public const long WS_EX_LEFT = 0x00000000;
        public const long WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const long WS_EX_LTRREADING = 0x00000000;
        public const long WS_EX_MDICHILD = 0x00000040;
        public const long WS_EX_NOACTIVATE = 0x08000000;
        public const long WS_EX_NOINHERITLAYOUT = 0x00100000;
        public const long WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const long WS_EX_NOREDIRECTIONBITMAP = 0x00200000;
        public const long WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const long WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const long WS_EX_RIGHT = 0x00001000;
        public const long WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const long WS_EX_RTLREADING = 0x00002000;
        public const long WS_EX_STATICEDGE = 0x00020000;
        public const long WS_EX_TOOLWINDOW = 0x00000080;
        public const long WS_EX_TOPMOST = 0x00000008;
        public const long WS_EX_TRANSPARENT = 0x00000020;
        public const long WS_EX_WINDOWEDGE = 0x00000100;
        //Window Styles
        public const long WS_BORDER = 0x00800000;
        public const long WS_CAPTION = 0x00C00000;
        public const long WS_CHILD = 0x40000000;
        public const long WS_CHILDWINDOW = 0x40000000;
        public const long WS_CLIPCHILDREN = 0x02000000;
        public const long WS_CLIPSIBLINGS = 0x04000000;
        public const long WS_DISABLED = 0x08000000;
        public const long WS_DLGFRAME = 0x00400000;
        public const long WS_GROUP = 0x00020000;
        public const long WS_HSCROLL = 0x00100000;
        public const long WS_ICONIC = 0x20000000;
        public const long WS_MAXIMIZE = 0x01000000;
        public const long WS_MAXIMIZEBOX = 0x00010000;
        public const long WS_MINIMIZE = 0x20000000;
        public const long WS_MINIMIZEBOX = 0x00020000;
        public const long WS_OVERLAPPED = 0x00000000;
        public const long WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const long WS_POPUP = 0x80000000;
        public const long WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
        public const long WS_SIZEBOX = 0x00040000;
        public const long WS_SYSMENU = 0x00080000;
        public const long WS_TABSTOP = 0x00010000;
        public const long WS_THICKFRAME = 0x00040000;
        public const long WS_TILED = 0x00000000;
        public const long WS_TILEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const long WS_VISIBLE = 0x10000000;
        public const long WS_VSCROLL = 0x00200000;

        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;

        public const int SC_RESTORE = 0xF120; //还原  
        public const int SC_MOVE = 0xF010; //移动  
        public const int SC_SIZE = 0xF000; //大小  
        public const int SC_MINIMIZE = 0xF020; //最小化  
        public const int SC_MAXIMIZE = 0xF030; //最大化  
        public const int SC_CLOSE = 0xF060; //关闭 

        //WM_NCHITTEST消息处理返回值
        public const int HTERROR = -2;
        public const int HTTRANSPARENT = -1;
        public const int HTNOWHERE = 0;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const int HTSYSMENU = 3;
        public const int HTGROWBOX = 4;
        public const int HTSIZE = HTGROWBOX;
        public const int HTMENU = 5;
        public const int HTHSCROLL = 6;
        public const int HTVSCROLL = 7;
        public const int HTMINBUTTON = 8;
        public const int HTMAXBUTTON = 9;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;
        public const int HTBORDER = 18;
        public const int HTREDUCE = HTMINBUTTON;
        public const int HTZOOM = HTMAXBUTTON;
        public const int HTSIZEFIRST = HTLEFT;
        public const int HTSIZELAST = HTBOTTOMRIGHT;
        public const int HTOBJECT = 19;
        public const int HTCLOSE = 20;
        public const int HTHELP = 21;

        //GetSystemMetrics参数
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public const int SM_CXFULLSCREEN = 16;
        public const int SM_CYFULLSCREEN = 17;
        public const int SM_CYMENU = 15;
        public const int SM_CYCAPTION = 4;
        public const int SM_CXFRAME = 32;
        public const int SM_CYFRAME = 33;
        public const int SM_CXHSCROLL = 21;
        public const int SM_CYHSCROLL = 3;
        public const int SM_CXVSCROLL = 2;
        public const int SM_CYVSCROLL = 20;
        public const int SM_CXSIZE = 30;
        public const int SM_CYSIZE = 31;
        public const int SM_CXCURSOR = 13;
        public const int SM_CYCURSOR = 14;
        public const int SM_CXBORDER = 5;
        public const int SM_CYBORDER = 6;
        public const int SM_CXDOUBLECLICK = 36;
        public const int SM_CYDOUBLECLICK = 37;
        public const int SM_CXDLGFRAME = 7;
        public const int SM_CXFIXEDFRAME = SM_CXDLGFRAME;
        public const int SM_CYDLGFRAME = 8;
        public const int SM_CYFIXEDFRAME = SM_CYDLGFRAME;
        public const int SM_CXICON = 11;
        public const int SM_CYICON = 12;
        public const int SM_CXICONSPACING = 38;
        public const int SM_CYICONSPACING = 39;
        public const int SM_CXMIN = 28;
        public const int SM_CYMIN = 29;
        public const int SM_CXMINTRACK = 34;
        public const int SM_CYMINTRACK = 35;
        public const int SM_CXHTHUMB = 10;
        public const int SM_CYVTHUMB = 9;
        public const int SM_DBCSENABLED = 42;
        public const int SM_DEBUG = 22;
        public const int SM_MENUDROPALIGNMENT = 40;
        public const int SM_MOUSEPRESENT = 19;
        public const int SM_PENWINDOWS = 41;
        public const int SM_SWAPBUTTON = 23;

        //ShowWindow参数
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_HIDE = 0;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNORMAL = 1;

        public const int MF_REMOVE = 0x1000;

        /// <summary>
        /// 贴图选项：将源矩形区域直接拷贝到目标矩形区域
        /// </summary>
        public const int ROP_SRCCOPY = 0x00CC0020;
    }
    public static class NativeMethods {


        #region 系统函数

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool AdjustWindowRectEx(RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);

        [DllImport("user32.dll")]
        public static extern long SetWindowLong(IntPtr hWnd, int nIndex, long nNewLong);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        //[DllImport("user32.dll")]
        //public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("User32.dll")]
        public static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, out Rectangle prcRect);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        #endregion

        #region 图形相关

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdcDest, // 目标 DC的句柄         
            int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc,  // 源DC的句柄         
            int nXSrc, int nYSrc,
            System.Int32 dwRop // 光栅的处理数值         
            );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        #endregion
    }


    //[StructLayout(LayoutKind.Sequential)]
    //public struct RECT
    //{
    //    public int Left;
    //    public int Top;
    //    public int Right;
    //    public int Bottom;

    //}

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int left, int top, int right, int bottom) {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public RECT(System.Drawing.Rectangle r) {
            this.left = r.Left;
            this.top = r.Top;
            this.right = r.Right;
            this.bottom = r.Bottom;
        }

        public static RECT FromXYWH(int x, int y, int width, int height) {
            return new RECT(x, y, x + width, y + height);
        }

        public System.Drawing.Size Size {
            get {
                return new System.Drawing.Size(this.right - this.left, this.bottom - this.top);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PWINDOWPOS {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public uint flags;
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct NCCALCSIZE_PARAMS {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public RECT[] rgrc;
        public PWINDOWPOS lppos;
    }
    #endregion
    /// <summary>
    /// 通用方法类
    /// </summary>
    public class CommonFunctions {
        /// <summary>
        /// 判断当前代码是否在设计时状态下执行
        /// </summary>
        /// <returns>在设计时状态则返回true</returns>
        public static bool IsInDesignMode() {
            //bool ReturnFlag = false;
            //if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            //    ReturnFlag = true;

            //    ReturnFlag = true;
            ////if (ReturnFlag)
            ////    Msg.Warning("设计模式");
            ////else Msg.Warning("非设计模式！");
            //return ReturnFlag;
            if(System.ComponentModel.LicenseManager.UsageMode
                == System.ComponentModel.LicenseUsageMode.Designtime) {
                //MessageBox.Show("Design Time!");
                return true;
            }
            else if(System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv") {
                return true;
            }
            else {
                //MessageBox.Show("RunTime Time!");
                return false;
            }
        }

        /// <summary>
        /// 返回当前程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetAppBaseDirectory() {
            if(CommonFunctions.IsInDesignMode()) {
                //方案1：
                //return @"F:\技术资料\斌哥App\场地管理\2012-09-27\SportsVenues\UIDemo\bin\Debug";
                //return @"F:\技术资料\斌哥App\场地管理\2012-12-03\.Venues\UI\bin\Debug";
                //return @"F:\private\创\预定\code\.Venues\UI\bin\Debug";

                //方案2
                //ITypeResolutionService typeResService
                //    = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
                //string path = typeResService.GetPathOfAssembly(Assembly.GetExecutingAssembly().GetName());
                //MessageBox.Show(path);

                //方案3：可获得解决方案路径
                return Environment.CurrentDirectory;

                //方案4：
                //string solutionDirectory = ((EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.10.0")).ActiveDocument.ProjectItem.ContainingProject.FullName;
                //MessageBox.Show(solutionDirectory);
                //solutionDirectory = System.IO.Path.GetDirectoryName(solutionDirectory) + "\\bin\\Debug";
                //MessageBox.Show(solutionDirectory);

                //方案5：正确的方法
                ////DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
                //DTE dte = GetDTEObject();
                //string solutionDirectory = GetActiveProject(dte).FullName;
                //solutionDirectory = System.IO.Path.GetDirectoryName(solutionDirectory);// + "\\bin\\Debug";
                ////MessageBox.Show(solutionDirectory);
                //return solutionDirectory;


            }
            else {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        ////获取当前版本的visual studio实例（版本从10.0开始尝试往上加）
        //internal static EnvDTE.DTE GetDTEObject() {
        //    DTE dteObject = null;
        //    for(int version = 10; version <= 40; version++) {
        //        string vsVersionString = string.Format("VisualStudio.DTE.{0}.0", version);
        //        try {
        //            dteObject = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject(vsVersionString);
        //            if(dteObject != null) {
        //                return dteObject;
        //            }
        //        }
        //        catch {
        //            continue;
        //        }
        //    }
        //    return dteObject;
        //}

        //internal static Project GetActiveProject(DTE dte) {
        //    Project activeProject = null;
        //    Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
        //    if(activeSolutionProjects != null && activeSolutionProjects.Length > 0) {
        //        activeProject = activeSolutionProjects.GetValue(0) as Project;
        //    }
        //    return activeProject;
        //}

        /// <summary>
        /// 获取逻辑坐标点对应实际图片上的实际点坐标
        /// 逻辑坐标点横纵坐标为负数表示距离窗口右或下边界的距离，不是实际坐标）
        /// </summary>
        /// <param name="sourceImage">图片对象</param>
        /// <param name="p">逻辑坐标点</param>
        /// <returns>实际图片坐标点</returns>
        public static Point GetImagePoint(Image sourceImage, int offsetX, int offsetY, Point p) {
            Point newPoint = new Point(p.X, p.Y);

            //负数表示使用实际图片宽度减掉当前值
            if(newPoint.X < 0) {
                newPoint.X = sourceImage.Width + newPoint.X;// +offsetX;
            }
            //负数表示使用实际图片高度减掉当前值
            if(newPoint.Y < 0) {
                newPoint.Y = sourceImage.Height + newPoint.Y;// +offsetY;
            }
            return newPoint;
        }

        /// <summary>
        /// 从指定路径和文件名加载图片文件并以Image对象返回
        /// </summary>
        /// <param name="imageFileName">图片路径和文件名</param>
        /// <returns>Image对象</returns>
        public static Image LoadImageFromFile(string imageFileName) {
            Image retImage = null;
            if(!File.Exists(imageFileName)) {
                throw new Exception("加载图片资源时找不到文件" + imageFileName + "!");
            }

            using(Stream stream = File.Open(imageFileName, FileMode.Open)) {
                retImage = Image.FromStream(stream);
            }
            return retImage;
        }

        /// <summary>
        /// 返回设计时窗口采用的主题的唯一名称
        /// </summary>
        /// <returns></returns>
        public static string GetDesignModeSkinName() {
            return "Default";
        }

        /// <summary>
        /// 调用Win32 BitBlt函数拷贝图像
        /// </summary>
        /// <param name="desGraphics">目标Graphics对象</param>
        /// <param name="desX">目标贴图起始横坐标</param>
        /// <param name="dexY">目标贴图起始纵坐标</param>
        /// <param name="desWidth">目标贴图宽度</param>
        /// <param name="desHeight">目标贴图高度</param>
        /// <param name="srcImage">贴图源Image对象</param>
        /// <param name="srcX">贴图源起始横坐标</param>
        /// <param name="srcY">贴图源起始纵坐标</param>
        public static void BitBltDrawImage(Graphics desGraphics, int desX, int dexY, int desWidth, int desHeight,
            Bitmap srcImage, int srcX, int srcY) {
            using(Graphics srcGraphics = Graphics.FromImage(srcImage)) {
                IntPtr hdcSrc = srcGraphics.GetHdc();
                IntPtr hBitmapSrc = srcImage.GetHbitmap();
                IntPtr cdcSrc = NativeMethods.CreateCompatibleDC(hdcSrc);
                IntPtr oldObject = NativeMethods.SelectObject(cdcSrc, hBitmapSrc);

                IntPtr hdcDes = desGraphics.GetHdc();

                bool result = NativeMethods.BitBlt(
                    hdcDes, desX, dexY, desWidth, desHeight, cdcSrc, srcX, srcY, NativeConst.ROP_SRCCOPY);

                NativeMethods.SelectObject(cdcSrc, oldObject);
                NativeMethods.DeleteObject(hBitmapSrc);
                NativeMethods.DeleteDC(cdcSrc);

                srcGraphics.ReleaseHdc(hdcSrc);
                desGraphics.ReleaseHdc(hdcDes);
            }
        }

        /// <summary>
        /// 设置指定hWnd窗口句柄对应控件的Window Style或者Extended Window Styles
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">偏移量，取GWL_和DWL_开头的常量</param>
        /// <param name="windowStyle">窗口样式常量，取WS_和WS_EX_开头的常量</param>
        public static void SetWindowStyle(IntPtr hWnd, int nIndex, long windowStyle) {
            long WindowLongValue = NativeMethods.GetWindowLong(hWnd, nIndex);
            WindowLongValue |= windowStyle;

            NativeMethods.SetWindowLong(hWnd, nIndex, WindowLongValue);
        }

        /// <summary>
        /// 取消设置指定hWnd窗口句柄对应控件的Window Style或者Extended Window Styles
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">偏移量，取GWL_和DWL_开头的常量</param>
        /// <param name="windowStyle">窗口样式常量，取WS_和WS_EX_开头的常量</param>
        public static void UnsetWindowStyle(IntPtr hWnd, int nIndex, long windowStyle) {
            long WindowLongValue = NativeMethods.GetWindowLong(hWnd, nIndex);
            WindowLongValue &= ~windowStyle;

            NativeMethods.SetWindowLong(hWnd, nIndex, WindowLongValue);
        }

        private static int iCaptionHeight = NativeMethods.GetSystemMetrics(NativeConst.SM_CYCAPTION);  //标题栏高度
        private static int iCYFrame = NativeMethods.GetSystemMetrics(NativeConst.SM_CYFRAME);          //可变大小的窗口的上下边框的厚度
        private static int iCXFrame = NativeMethods.GetSystemMetrics(NativeConst.SM_CYFRAME);          //可变大小的窗口的左右边框的厚度
        private static int iFixedCYFrame = NativeMethods.GetSystemMetrics(NativeConst.SM_CYFIXEDFRAME);//不可变大小的窗口的上下边框的厚度
        private static int iFixedCXFrame = NativeMethods.GetSystemMetrics(NativeConst.SM_CXFIXEDFRAME);//不可变大小的窗口的左右边框的厚度

        /// <summary>
        /// 获取指定边框类型的DUIFrom窗口Resize后,窗口大小需要调整的偏移量
        /// </summary>
        /// <param name="boderStyle">窗口边框类型</param>
        /// <returns>偏移量对象，包含Width偏移量和Height偏移量</returns>
        public static Size GetWindowResizeOffset(FormBorderStyle boderStyle) {
            Size offsetSize = new Size(0, 0);

            //当前窗口为不可变窗口时
            if(boderStyle == System.Windows.Forms.FormBorderStyle.Fixed3D
                || boderStyle == System.Windows.Forms.FormBorderStyle.FixedDialog
                || boderStyle == System.Windows.Forms.FormBorderStyle.FixedSingle
                || boderStyle == System.Windows.Forms.FormBorderStyle.FixedToolWindow) {
                offsetSize.Width = 2 * CommonFunctions.iFixedCXFrame;
                offsetSize.Height = (2 * CommonFunctions.iFixedCYFrame + CommonFunctions.iCaptionHeight);
            }
            //当前窗口为可变窗口时
            else if(boderStyle == System.Windows.Forms.FormBorderStyle.Sizable
                || boderStyle == System.Windows.Forms.FormBorderStyle.SizableToolWindow) {
                offsetSize.Width = 2 * CommonFunctions.iCXFrame;
                offsetSize.Height = (2 * CommonFunctions.iCYFrame + CommonFunctions.iCaptionHeight);
            }
            //当前窗口无边框
            else {
                ;
            }

            return offsetSize;
        }

        /// <summary>
        /// 窗口截图，包含标题栏和内容区
        /// </summary>
        /// <param name="window">窗口对象</param>
        /// <returns>截图Bitmap对象</returns>
        public static Bitmap CaptureWindow(System.Windows.Forms.Form window) {
            Rectangle rc = new Rectangle(window.Location, window.Size);
            Bitmap memoryImage = null;

            try {
                // Create new graphics object using handle to window.
                using(Graphics graphics = window.CreateGraphics()) {
                    memoryImage = new Bitmap(rc.Width, rc.Height, graphics);

                    using(Graphics memoryGrahics = Graphics.FromImage(memoryImage)) {
                        memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                    }
                }
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return memoryImage;
        }

        /*const int SRCCOPY = 0x00CC0020;
         public bool CutPictrueToStream(Bitmap BmpSource)
         {
             Graphics grSource = Graphics.FromImage(BmpSource);
             Bitmap Bitmap_cutted = new Bitmap(SizeX, SizeY, grSource);
             IntPtr hdcTarget = IntPtr.Zero;
             IntPtr hdcSource = IntPtr.Zero;
             IntPtr hBitmapSource = IntPtr.Zero;
             IntPtr hOldObject = IntPtr.Zero;
             hBitmapSource = BmpSource.GetHbitmap();
             MemorySource = new MemoryStream[ClassCount][][];
             for (int i = 0; i < ClassCount; i++)
             {
                 MemorySource[i] = new MemoryStream[DirectionCount][];
                 for (int j = 0; j < DirectionCount; j++)
                 {
                     MemorySource[i][j] = new MemoryStream[Frames[i]];
                     for (int k = 0; k < Frames[i]; k++)
                     {
                         Graphics grTarget = Graphics.FromImage(Bitmap_cutted);
                         hdcTarget = grTarget.GetHdc();
                         hdcSource = grSource.GetHdc();
                         hOldObject = SelectObject(hdcSource, hBitmapSource);
                         BitBlt(hdcTarget, 0, 0, SizeX, SizeY, hdcSource, (i * Frames[i] + k) * SizeX, j * SizeY, SRCCOPY);
                         //必须释放DC，否则保存为黑图
                         if (hdcTarget != IntPtr.Zero
                         {
                             grTarget.ReleaseHdc(hdcTarget);
                         }
                         if (hdcSource != IntPtr.Zero)
                         {
                             grSource.ReleaseHdc(hdcSource);
                         }
                         Bitmap_cutted.MakeTransparent();//保存为透明背景
                         Bitmap_cutted.Save(@"F:\Project\VS 2008\C#\(十一)地图遮罩层的实现\(十一)地图遮罩层的实现\Player\" + i.ToString() + "_" + j.ToString() + "_" + k.ToString() + ".Png",ImageFormat.Png);
                         //
                         MemorySource[i][j][k] = new MemoryStream();
                         //
                         Bitmap_cutted.Save(MemorySource[i][j][k], ImageFormat.Png);
                         grTarget.Dispose();
                     }
                 }
             }
             if (hOldObject != IntPtr.Zero)
                 SelectObject(hdcSource, hOldObject);
             if (hBitmapSource != IntPtr.Zero)
                 DeleteObject(hBitmapSource);
             grSource.Dispose();
             Bitmap_cutted.Dispose();
             return true;
         }*/
    }

    public class WindowAnimator {
        #region 常量申明
        /// <summary>
        /// 自左向右显示控件.当使用AW_CENTER标志时,该标志将被忽略.
        /// </summary>
        public const Int32 AW_HOR_POSITIVE = 0x00000001;

        /// <summary>
        /// 自右向左显示控件.当使用AW_CENTER标志时,该标志将被忽略.
        /// </summary>
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;

        /// <summary>
        /// 自顶向下显示控件.该标志可以在滚动动画和滑动动画中使用.当使用AW_CENTER标志时,该标志将被忽略.
        /// </summary>
        public const Int32 AW_VER_POSITIVE = 0x00000004;

        /// <summary>
        /// 自下向上显示控件.该标志可以在滚动动画和滑动动画中使用.当使用AW_CENTER标志时,该标志将被忽略.
        /// </summary>
        public const Int32 AW_VER_NEGATIVE = 0x00000008;

        /// <summary>
        /// 若使用AW_HIDE标志,则使控件向内重叠;若未使用AW_HIDE标志,则使控件向外扩展.
        /// </summary>
        public const Int32 AW_CENTER = 0x00000010;

        /// <summary>
        /// 隐藏控件.默认则显示控件.
        /// </summary>
        public const Int32 AW_HIDE = 0x00010000;

        /// <summary>
        /// 激活控件.在使用AW_HIDE标志后不要使用这个标志.
        /// </summary>
        public const Int32 AW_ACTIVATE = 0x00020000;

        /// <summary>
        /// 使用滑动类型.默认则为滚动动画类型.当使用AW_CENTER标志时,这个标志就被忽略.
        /// </summary>
        public const Int32 AW_SLIDE = 0x00040000;

        /// <summary>
        /// 使用淡入效果.只有当hWnd为顶层控件时才可以使用此标志.
        /// </summary>
        public const Int32 AW_BLEND = 0x00080000;
        #endregion

        [DllImportAttribute("user32.dll")]
        public static extern bool AnimateWindow(IntPtr hWnd, int dwTime, int dwFlags);
    }

}
