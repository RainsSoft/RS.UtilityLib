using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// 可以显示图像的ImageCombox。适用于Item的图像内容都不一样的情况。
    /// 注：如果item的图像内容有重复，应该使用ImageList来存储图片
    /// </summary>
    public class ImageComboBox : System.Windows.Forms.ComboBox
    {
        // private FileSystemWatcher m_Watch;


        public ImageComboBox()
            : base() {

            this.MaxDropDownItems = 10;
            this.DropDownHeight = 64 * 8;
            this.ImageSize = new Size(48, 48);
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            // this.ItemHeight = this.ImageSize.Height;
        }
        private Size m_ImageSize;
        public Size ImageSize {
            get {
                return m_ImageSize;
            }
            set {
                m_ImageSize = value;
                //this.ItemHeight = this.ImageSize.Height;
            }
        }
        protected override void OnMeasureItem(MeasureItemEventArgs e) {
            e.ItemHeight = this.ImageSize.Height + 4;//留点空
            base.OnMeasureItem(e);
        }
        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (e.Index == -1) return;
            ImageComboBoxItem item = this.Items[e.Index] as ImageComboBoxItem;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                //画带焦点的。
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                e.Graphics.DrawString(item.ToString(), e.Font, SystemBrushes.HighlightText, this.ImageSize.Width + 4, e.Bounds.Y);

            }
            else {
                //普通
                e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds);
                e.Graphics.DrawString(item.ToString(), e.Font, SystemBrushes.ControlText, this.ImageSize.Width + 4, e.Bounds.Y);
            }
            DrawImage(e);
            base.OnDrawItem(e);
        }

        protected virtual void DrawImage(DrawItemEventArgs e) {
            Rectangle bound = e.Bounds;
            ImageComboBoxItem item = this.Items[e.Index] as ImageComboBoxItem;
            if (item.PrivewImage != null) {
                int offset = (int)((bound.Height - item.PrivewImage.Height) * 0.5f);
                e.Graphics.DrawImage(item.PrivewImage, new Rectangle(new Point(bound.X, bound.Y + offset), item.PrivewImage.Size));
            }
        }

        protected override void Dispose(bool disposing) {
            foreach (var v in Items) {
                (v as ImageComboBoxItem).Dispose();
            }
            Items.Clear();
            base.Dispose(disposing);
        }

        //-------------------------
        public class ImageComboBoxItem : IDisposable
        {
            public string Text {
                get;
                set;
            }
            public string FullPath {
                get;
                set;
            }
            public Image PrivewImage {
                get;
                set;
            }
            public ImageComboBoxItem(string text, Image img) {
                this.Text = text;
                this.PrivewImage = img;
            }
            ~ImageComboBoxItem() {
                if (this.PrivewImage != null) {
                    this.PrivewImage.Dispose();
                }
            }
            public override string ToString() {
                return this.Text;
            }

            public object UserTag {
                get;
                set;
            }
            #region IDisposable 成员

            public void Dispose() {
                UserTag = null;
                if (this.PrivewImage != null) {
                    this.PrivewImage.Dispose();
                }
                GC.SuppressFinalize(this);
            }

            #endregion

        }
        public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
        //public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
        /// <summary>
        /// 基于文件夹的带监控和异步加载ImageComboBox
        /// </summary>
        public class ImageLazyLoadComboBox : ImageComboBox
        {
            //Dictionary<string,> m_Files=new List<string>();
            FileSystemWatcher m_Watcher;
            private bool m_Inited = false;
            private bool m_Cancel = false;
            private string m_ImageDir = string.Empty;
            Thread m_InitThread;
            /// <summary>
            /// 图片文件目录
            /// </summary>
            public string ImageDir {
                get {
                    return m_ImageDir;
                }
                set {
                    if (m_Watcher == null) {
                        m_Watcher = new FileSystemWatcher();
                        m_Watcher.Deleted += new FileSystemEventHandler(m_Watcher_Notify);
                        m_Watcher.Created += new FileSystemEventHandler(m_Watcher_Notify);
                        m_Watcher.Changed += new FileSystemEventHandler(m_Watcher_Notify);
                        m_Watcher.Renamed += new RenamedEventHandler(m_Watcher_Renamed);
                    }
                    m_Watcher.Path = value;
                    m_Watcher.EnableRaisingEvents = true;
                    m_Watcher.Filter = "*.*";
                    m_Watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                    m_ImageDir = value;
                }
            }

            void m_Watcher_Renamed(object sender, RenamedEventArgs e) {
                System.Threading.Thread.Sleep(2000);
                try {
                    if (IsImageFile(e.Name)) {
                        RenameItem(e.OldName, e.Name, e.FullPath);
                    }
                }
                catch { }
            }
            private bool IsImageFile(string f) {
                string ext = Path.GetExtension(f).ToLower();
                return (ext == ".png" || ext == ".bmp" || ext == ".jpg" || ext == ".jpeg");
            }
            private Image LoadImage(string f) {
                using (Image img = Image.FromFile(f, true)) {
                    Image ret = img.GetThumbnailImage(this.ImageSize.Width, this.ImageSize.Height, new Image.GetThumbnailImageAbort(() => { return false; }), IntPtr.Zero);
                    return ret;
                }
            }
            void m_Watcher_Notify(object sender, FileSystemEventArgs e) {
                System.Threading.Thread.Sleep(2000);
                try {
                    if (IsImageFile(e.Name)) {
                        switch (e.ChangeType) {
                            case WatcherChangeTypes.Changed:
                                ModifyItem(e.Name, e.FullPath);
                                break;
                            case WatcherChangeTypes.Created:
                                AddItem(e.Name, e.FullPath);
                                break;
                            case WatcherChangeTypes.Deleted:
                                DelItem(e.Name);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch {
                }
            }
            private void AddItem(string name, string path) {
                Image img = LoadImage(path);
                if (this.InvokeRequired) {
                    this.Invoke(new Func<string, Image, string, bool>((o1, o2, path1) => {
                        ImageComboBoxItem item = new ImageComboBoxItem(o1, o2);
                        item.FullPath = path1;
                        this.Items.Add(item);
                        return true;
                    }), name, img, path);
                }
                else {
                    ImageComboBoxItem item = new ImageComboBoxItem(name, img);
                    item.FullPath = path;
                    this.Items.Add(item);
                }
            }

            private void ModifyItem(string name, string path) {
                Image img = LoadImage(path);
                if (this.InvokeRequired) {
                    this.Invoke(new Func<string, Image, bool>((o1, o2) => {
                        foreach (var v in this.Items) {
                            if ((v as ImageComboBoxItem).Text.Equals(o1, StringComparison.OrdinalIgnoreCase)) {
                                (v as ImageComboBoxItem).PrivewImage.Dispose();
                                (v as ImageComboBoxItem).PrivewImage = o2;
                            }
                        }
                        return true;
                    }), name, img, path);
                }
                else {
                    foreach (var v in this.Items) {
                        if ((v as ImageComboBoxItem).Text.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                            (v as ImageComboBoxItem).PrivewImage.Dispose();
                            (v as ImageComboBoxItem).PrivewImage = img;
                        }
                    }
                }
            }
            private void RenameItem(string oldname, string newname, string path) {
                ImageComboBoxItem item;
                if (this.InvokeRequired) {
                    this.Invoke(new Func<string, string, string, bool>((o1, o2, path1) => {

                        foreach (var v in this.Items) {
                            item = (v as ImageComboBoxItem);
                            if (item.Text.Equals(oldname, StringComparison.OrdinalIgnoreCase)) {
                                item.Text = newname;
                                item.FullPath = path1;
                            }
                        }
                        return true;
                    }), oldname, newname, path);
                }
                else {
                    foreach (var v in this.Items) {
                        item = (v as ImageComboBoxItem);
                        if (item.Text.Equals(oldname, StringComparison.OrdinalIgnoreCase)) {
                            item.Text = newname;
                            item.FullPath = path;
                        }
                    }
                }
            }
            private void DelItem(string name) {
                ImageComboBoxItem item = null;
                if (this.InvokeRequired) {
                    this.Invoke(new Action<string>((o1) => {

                        foreach (var v in this.Items) {
                            item = (v as ImageComboBoxItem);
                            if (item.Text.Equals(o1, StringComparison.OrdinalIgnoreCase)) {
                                item.Dispose();
                            }
                        }
                        this.Items.Remove(item);
                    }), name);
                }
                else {
                    foreach (var v in this.Items) {
                        item = (v as ImageComboBoxItem);
                        if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                            item.Dispose();
                        }
                    }
                    if (item != null) {
                        this.Items.Remove(item);
                    }
                }
            }

            protected override void OnDropDown(EventArgs e) {
                if (m_Inited == false) {
                    m_InitThread = new Thread(() => InitImages());
                    m_InitThread.Start();
                }
                base.OnDropDown(e);
            }

            private void InitImages() {
                if (string.IsNullOrEmpty(this.ImageDir)) {
                    return;
                }
                string[] files = Directory.GetFiles(this.ImageDir);
                foreach (var v in files) {

                    if (m_Cancel) {
                        break;
                    }
                    if (IsImageFile(v)) {
                        lock (disposeLocker) {
                            AddItem(Path.GetFileName(v), v);
                        }
                    }
                }
                m_Inited = true;
            }
            private object disposeLocker = new object();
            protected override void Dispose(bool disposing) {
                m_Cancel = true;
                if (m_InitThread != null) {
                    try {
                        m_InitThread.Abort();
                    }
                    catch { }
                }
                if (m_Watcher != null) {
                    m_Watcher.Dispose();
                }
                base.Dispose(disposing);

            }
        }

    }
}
