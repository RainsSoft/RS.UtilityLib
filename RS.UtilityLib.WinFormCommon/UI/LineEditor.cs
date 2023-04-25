using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{

    [ToolboxBitmap(typeof(LineEditor), "")]
    public class LineEditor : UserControl
    {
        // Fields
        private Brush BackgroundBrush;
        private bool bottomAligned = true;
        private Pen DarkPen;
        private bool editable = false;
        private int inx = -1;
        private float lastba = 0f;
        private int lastSBValue = 0;
        private Pen LightPen;
        private LineCollection lines;
        private ScrollBar scrollbar;
        private Brush SelectedBrush;
        private Pen SelectionPen;
        private bool showselection = true;

        // Events
        public event EventHandler SelectionChanged;

        // Methods
        public LineEditor() {
            this.InitializeComponent();
        }

        public int AddLine(Color col, string text) {
            return this.AddLine(col, text, null);
        }

        public int AddLine(Color col, string text, object data) {
            Line line = new Line(col, text, data);
            int num = this.Lines.Add(line);
            this.SafeRefresh();
            return num;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.scrollbar.Dispose();
                if (this.LightPen != null) {
                    this.LightPen.Dispose();
                }
                if (this.DarkPen != null) {
                    this.DarkPen.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void DoScroll() {
            base.Invalidate();
            this.lastSBValue = this.scrollbar.Value;
            if (this.bottomAligned) {
                this.lastba = this.OffsetFromBottom;
            }
        }

        public void EnsureVisible(int inx) {
            if (this.scrollbar.Visible) {
                if (inx < 0) {
                    this.scrollbar.Value = this.bottomAligned ? this.scrollbar.Maximum : 0;
                }
                else {
                    int num = 0;
                    for (int i = 0; i < inx; i++) {
                        num += this.lines[i].GetHeight(this.Font);
                    }
                    int height = this.lines[inx].GetHeight(this.Font);
                    if (num < this.YScroll) {
                        this.YScroll = num;
                    }
                    else if ((num + height) > (this.YScroll + base.ClientSize.Height)) {
                        this.YScroll = (num + height) - base.ClientSize.Height;
                    }
                }
            }
        }

        protected virtual int GetIndexAt(int x, int y) {
            if (((x >= 0) && (x <= this.VisibleWidth)) && ((y >= 0) && (y <= base.Height))) {
                int num = 0;
                y += this.YScroll;
                foreach (Line line in this.lines) {
                    num += line.GetHeight(this.Font);
                    if (y < num) {
                        return this.lines.GetIndex(line);
                    }
                }
            }
            return -1;
        }

        private void InitializeComponent() {
            base.Name = "LineEditor";
            base.Size = new Size(100, 50);
            this.scrollbar = new VScrollBar();
            this.scrollbar.Dock = DockStyle.Right;
            this.scrollbar.Visible = false;
            this.scrollbar.Scroll += new ScrollEventHandler(this.ScrollbarMoved);
            base.Controls.Add(this.scrollbar);
            base.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.UserPaint, true);
            this.Lines = new LineCollection(this);
#if DEBUG
            //if (this.DesignMode) {
            for (int i = 0; i < 10; i++) {
                this.lines.Add(new Line(Color.Gray, i.ToString(), i));
            }
            for (int i = 0; i < 10; i++) {
                this.lines.Add(new Line(Color.Green, i.ToString(), i));
            }
            //}
#endif
        }

        protected override bool IsInputChar(char c) {
            return true;
        }

        protected override bool IsInputKey(Keys k) {
            switch ((k & Keys.KeyCode)) {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(k);
        }

        private void MoveSelection(int di) {
            this.SelectedIndex = this.Lines.ClipIndex(this.SelectedIndex + di);
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            base.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (!e.Handled && ((this.SelectedLine == null) || !(e.Handled = this.SelectedLine.OnKeyDown(e)))) {
                switch (e.KeyData) {
                    case Keys.Prior:
                        this.MoveSelection(-10);
                        break;

                    case Keys.Next:
                        this.MoveSelection(10);
                        break;

                    case Keys.Up:
                        this.MoveSelection(-1);
                        break;

                    case Keys.Down:
                        this.MoveSelection(1);
                        break;

                    case (Keys.Control | Keys.C):
                        if (this.SelectedLine != null) {
                            Clipboard.SetDataObject(this.SelectedLine.Text);
                        }
                        break;
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if ((!e.Handled && this.editable) && (this.SelectedLine != null)) {
                this.SelectedLine.OnKeyPress(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (!e.Handled && (this.SelectedLine != null)) {
                this.SelectedLine.OnKeyUp(e);
            }
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            base.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (this.Selectable) {
                base.Focus();
                this.SelectedIndex = this.GetIndexAt(e.X, e.Y);
                base.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (this.LightPen == null) {
                this.LightPen = new Pen(SystemColors.ControlLightLight);
            }
            if (this.DarkPen == null) {
                this.DarkPen = new Pen(SystemColors.ControlDark);
            }
            e.Graphics.DrawLine(this.DarkPen, 0, 0, 0, base.ClientSize.Height - 1);
            e.Graphics.DrawLine(this.DarkPen, 0, 0, base.ClientSize.Width, 0);
            e.Graphics.DrawLine(this.LightPen, base.ClientSize.Width - 1, 1, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
            e.Graphics.DrawLine(this.LightPen, 0, base.ClientSize.Height - 1, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
            int y = -this.YScroll;
            foreach (Line line in this.lines) {
                if (y > base.ClientSize.Height) {
                    break;
                }
                int height = line.GetHeight(this.Font);
                Color c = base.Enabled ? line.Color : Color.Gray;
                if (line == this.SelectedLine) {
                    if ((this.showselection && base.Enabled) && this.Focused) {
                        c = SystemColors.HighlightText;
                        e.Graphics.FillRectangle(this.SelectedBrush, 1, y, this.VisibleWidth - 2, height);
                    }
                    if (this.SelectionPen == null) {
                        this.SelectionPen = new Pen(SystemColors.ControlDark);
                        this.SelectionPen.DashStyle = DashStyle.Dot;
                    }
                    e.Graphics.DrawRectangle(this.SelectionPen, 1, y, this.VisibleWidth - 3, height);
                }
                if ((y + height) >= 0) {
                    line.Paint(e.Graphics, this.Font, c, y);
                }
                y += height;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            if (this.BackgroundBrush == null) {
                this.BackgroundBrush = new SolidBrush(this.BackColor);
            }
            if (this.SelectedBrush == null) {
                this.SelectedBrush = new SolidBrush(SystemColors.Highlight);
            }
            e.Graphics.FillRectangle(this.BackgroundBrush, 0, 0, base.ClientSize.Width - 1, base.ClientSize.Height - 1);
        }

        protected override void OnResize(EventArgs e) {
            if ((this.scrollbar != null) && this.scrollbar.Visible) {
                this.RecalculateScrollbar();
                this.scrollbar.Left = base.ClientSize.Width - this.scrollbar.Width;
                this.scrollbar.Height = base.ClientSize.Height;
            }
            this.ScrollFromBottom();
            base.Invalidate();
        }

        private void RecalculateScrollbar() {
            int num = 0;
            foreach (Line line in this.lines) {
                num += line.GetHeight(this.Font);
            }
            if (num < base.ClientSize.Height) {
                this.scrollbar.Visible = false;
                this.scrollbar.Value = 0;
            }
            else {
                this.scrollbar.Visible = true;
                this.scrollbar.Maximum = num;
                this.scrollbar.LargeChange = base.ClientSize.Height;
                this.scrollbar.SmallChange = this.Font.Height;
                this.lastSBValue = this.scrollbar.Value;
            }
        }

        internal void SafeRefresh() {
            if (base.IsHandleCreated) {
                base.Invoke(new MethodInvoker(this.RecalculateScrollbar));
                if (this.bottomAligned) {
                    this.ScrollFromBottom();
                }
                base.Invoke(new MethodInvoker(this.Invalidate));
                base.Invoke(new MethodInvoker(this.Update));
            }
        }

        private void ScrollbarMoved(object sender, ScrollEventArgs e) {
            this.DoScroll();
        }

        private void ScrollFromBottom() {
            float num = this.OffsetFromBottom - this.lastba;
            if (((this.scrollbar != null) && this.scrollbar.Visible) && (num != 0f)) {
                int num2 = this.scrollbar.Value + ((int)num);
                this.scrollbar.Value = (num2 < 0) ? 0 : ((num2 > this.scrollbar.Maximum) ? (this.scrollbar.Maximum - 1) : num2);
                this.lastSBValue = num2;
            }
        }

        // Properties
        public bool BottomAligned {
            get {
                return this.bottomAligned;
            }
            set {
                this.bottomAligned = value;
            }
        }

        public bool Editable {
            get {
                return this.editable;
            }
            set {
                this.editable = value;
            }
        }

        public LineCollection Lines {
            get {
                return this.lines;
            }
            set {
                this.lines = value;
            }
        }

        public float OffsetFromBottom {
            get {
                if ((this.scrollbar == null) || !this.scrollbar.Visible) {
                    return 0f;
                }
                int num = 0;
                foreach (Line line in this.lines) {
                    num += line.GetHeight(this.Font);
                }
                return ((num - this.lastSBValue) - base.ClientSize.Height);
            }
        }

        public bool Selectable {
            get {
                return base.GetStyle(ControlStyles.Selectable);
            }
            set {
                base.SetStyle(ControlStyles.Selectable, value);
            }
        }

        public int SelectedIndex {
            get {
                if (this.inx >= this.Lines.Count) {
                    this.inx = -1;
                }
                return this.inx;
            }
            set {
                if (this.inx >= 0) {
                    this.SelectedLine.Selected = false;
                }
                if ((value < 0) || (value >= this.Lines.Count)) {
                    this.inx = -1;
                }
                else {
                    this.inx = value;
                }
                if ((this.inx >= 0) && this.scrollbar.Visible) {
                    this.EnsureVisible(this.inx);
                }
                if (this.SelectionChanged != null) {
                    this.SelectionChanged(this, new EventArgs());
                }
                if (this.inx >= 0) {
                    this.SelectedLine.Selected = true;
                }
                this.SafeRefresh();
            }
        }

        public Line SelectedLine {
            get {
                if (this.inx == -1) {
                    return null;
                }
                return this.Lines[this.inx];
            }
            set {
                this.SelectedIndex = this.Lines.GetIndex(value);
            }
        }

        public bool ShowSelection {
            get {
                return this.showselection;
            }
            set {
                this.showselection = value;
            }
        }

        public int VisibleWidth {
            get {
                return (base.ClientSize.Width - (this.scrollbar.Visible ? this.scrollbar.Width : 0));
            }
        }

        public int YScroll {
            get {
                return this.scrollbar.Value;
            }
            set {
                this.scrollbar.Value = value;
                this.DoScroll();
            }
        }

        // Nested Types
        public class LineCollection : ICollection, IEnumerable
        {
            // Fields
            private Line[] array;
            private int count;
            private LineEditor parent;

            // Methods
            public LineCollection(LineEditor parent) : this(parent, 0) {
            }

            public LineCollection(LineEditor parent, int capacity) {
                this.count = 0;
                this.array = new Line[capacity];
                this.parent = parent;
            }

            public int Add(Line line) {
                if (this.count < this.Capacity) {
                    this.array[this.count++] = line;
                    line.host = this;
                    this.parent.SafeRefresh();
                    return (this.count - 1);
                }
                this.Expand();
                return this.Add(line);
            }

            public void Clear() {
                this.count = 0;
            }

            public int ClipIndex(int inx) {
                return ((inx < 0) ? 0 : ((inx >= this.count) ? (this.count - 1) : inx));
            }

            public void CopyTo(Array array, int count) {
                Line[] lineArray = (Line[])array;
                for (int i = 0; i < count; i++) {
                    lineArray[i] = this.array[i];
                }
            }

            private void Expand() {
                Line[] array = new Line[this.Capacity + 5];
                this.CopyTo(array, this.Capacity);
                this.array = array;
            }

            public IEnumerator GetEnumerator() {
                return new LineEditor.LineCollectionEnumerator(this);
            }

            public int GetIndex(Line line) {
                for (int i = this.array.Length - 1; i >= 0; i--) {
                    if (this.array[i] == line) {
                        return i;
                    }
                }
                return -1;
                throw new ArgumentException("Line " + line + " is not in this collection");
            }

            public void Insert(Line line, int index) {
                if (index < 0) {
                    index = 0;
                }
                else if (index >= this.count) {
                    index = this.count;
                }
                if (this.Count < this.Capacity) {
                    for (int i = this.count; i > index; i--) {
                        this.array[i] = this.array[i - 1];
                    }
                    this.array[index] = line;
                    this.count++;
                    line.host = this;
                    this.parent.SafeRefresh();
                }
                else {
                    this.Expand();
                    this.Insert(line, index);
                }
            }

            public void Remove(Line line) {
                for (int i = this.array.Length - 1; i >= 0; i--) {
                    if (this.array[i] == line) {
                        this.Remove(i);
                    }
                }
            }

            public void Remove(int index) {
                Line[] lineArray = new Line[this.array.Length - 1];
                this.count--;
                for (int i = 0; i < index; i++) {
                    lineArray[i] = this.array[i];
                }
                for (int j = index; j < lineArray.Length; j++) {
                    lineArray[j] = this.array[j + 1];
                }
                this.array = lineArray;
                this.parent.DoScroll();
                if (index == this.count) {
                    this.parent.inx = -1;
                    this.parent.SelectedIndex = this.count - 1;
                }
                else {
                    this.parent.SelectedIndex = this.parent.SelectedIndex;
                }
                this.parent.SafeRefresh();
            }

            // Properties
            public int Capacity {
                get {
                    return this.array.Length;
                }
            }

            public int Count {
                get {
                    return this.count;
                }
            }

            public bool IsSynchronized {
                get {
                    return false;
                }
            }

            public Line this[int inx] {
                get {
                    return this.array[inx];
                }
                set {
                    this.array[inx] = value;
                }
            }

            public LineEditor Parent {
                get {
                    return this.parent;
                }
            }

            public object SyncRoot {
                get {
                    return this.array;
                }
            }
        }

        public class LineCollectionEnumerator : IEnumerator
        {
            // Fields
            private int index;
            private LineEditor.LineCollection parent;

            // Methods
            internal LineCollectionEnumerator(LineEditor.LineCollection parent) {
                this.parent = parent;
                this.Reset();
            }

            public bool MoveNext() {
                return (++this.index < this.parent.Count);
            }

            public void Reset() {
                this.index = -1;
            }

            // Properties
            public object Current {
                get {
                    return this.parent[this.index];
                }
            }
        }
        [Serializable]
        public class Line
        {

            public Line(Color col, string text, object data) {
                this.Color = col;
                this.Text = text;
                this.UserData = data;
            }
            public object UserData { get; internal set; }
            public LineCollection host { get; internal set; }
            public bool Selected { get; internal set; }
            public Color Color { get; internal set; }
            public string Text { get; internal set; }

            internal int GetHeight(Font font) {
                var size = TextRenderer.MeasureText("F", font);
                return size.Height;// + 2;
            }

            internal bool OnKeyDown(KeyEventArgs e) {
                return this.Selected;
            }

            internal void OnKeyPress(KeyPressEventArgs e) {
                //return this.Selected;
            }

            internal void OnKeyUp(KeyEventArgs e) {
                //
            }

            internal void Paint(Graphics graphics, Font font, Color c, int y) {
                using (Pen p = new Pen(c, 1)) {
                    graphics.DrawLine(p, 0, y, 64, y);
                }
                TextRenderer.DrawText(graphics, this.Text, font, new Point(0, y), this.Color);
            }
        }
    }



}
