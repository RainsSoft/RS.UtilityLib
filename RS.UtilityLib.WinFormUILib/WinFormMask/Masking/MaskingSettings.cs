using System;
using System.Drawing;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 蒙版弹层配置
    /// </summary>
    public class MaskingSettings
    {
        #region 属性

        private MaskingStylePattern stylePattern = MaskingStylePattern.Dot;
        /// <summary>
        /// 风格模式
        /// </summary>
        public MaskingStylePattern StylePattern
        {
            get { return this.stylePattern; }
        }

        private Padding padding = new Padding(0);
        /// <summary>
        /// 蒙版弹层拥有者内边距（相对于拥有者工作区） 
        /// </summary>
        public Padding Padding
        {
            get { return this.padding; }
        }

        private Color backColor = Color.FromArgb(150, 0, 0, 0);
        /// <summary>
        /// 蒙版弹层背景颜色 
        /// </summary>
        public Color BackColor
        {
            get { return this.backColor; }
        }

        private Color brushColor = Color.White;
        /// <summary>
        /// 加载动画画笔颜色
        /// </summary>
        public Color BrushColor
        {
            get { return this.brushColor; }
        }

        private int brushSize = 7;
        /// <summary>
        /// 加载动画画笔大小
        /// </summary>
        public int BrushSize
        {
            get { return this.brushSize; }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                this.brushSize = value;
            }
        }

        private string text = null;
        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        private MaskingTextAlignment textAlignment = MaskingTextAlignment.Bottom;
        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public MaskingTextAlignment TextAlignment
        {
            get { return this.textAlignment; }
        }

        private Font textFont = null;
        /// <summary>
        /// 文本字体
        /// </summary>
        public Font TextFont
        {
            get { return this.textFont; }
        }

        private Color textColor = Color.White;
        /// <summary>
        /// 文本颜色 
        /// </summary>
        public Color TextColor
        {
            get { return this.textColor; }
        }

        private float[] brushAngles = null;
        /// <summary>
        /// 加载动画画笔角度集合
        /// </summary>
        public float[] BrushAngles
        {
            get { return this.brushAngles; }
        }

        private Color[] brushColors = null;
        /// <summary>
        /// 加载动画画笔颜色集合
        /// </summary>
        public Color[] BrushColors
        {
            get { return this.brushColors; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style">蒙版弹层显示风格</param>
        /// <param name="padding">蒙版弹层拥有者内边距（相对于拥有者工作区） </param>
        /// <param name="backColor">蒙版弹层背景颜色</param>
        /// <param name="brushColor">加载动画画笔颜色</param>
        /// <param name="text">文本</param>
        /// <param name="textAlignment">文本对齐方式</param>
        /// <param name="textFont">文本字体</param>
        /// <param name="textColor">文本颜色</param>
        public MaskingSettings(MaskingStylePattern style, Padding padding, Color backColor, Color brushColor, string text, MaskingTextAlignment textAlignment, Font textFont, Color textColor)
        {
            this.stylePattern = style;
            this.padding = padding;
            this.backColor = backColor;
            this.brushColor = brushColor;
            this.text = text;
            this.textAlignment = textAlignment;
            this.textFont = (textFont != null ? textFont : new Font("宋体", 11));
            this.textColor = textColor;
            this.InitializeBrushsAngle();
            this.InitializeBrushsColor();
        }

        #region 私有方法

        /// <summary>
        /// 初始化画笔角度
        /// </summary>
        /// <returns></returns>
        private void InitializeBrushsAngle()
        {
            float[] brushsAngle_tmp = new float[Masking.BrushCount];
            float avgAngle = 360f / Masking.BrushCount;
            for (int i = 0; i < Masking.BrushCount; i++)
            {
                brushsAngle_tmp[i] = (i + 1) * avgAngle;
            }
            this.brushAngles = brushsAngle_tmp;
        }

        /// <summary>
        /// 初始化画笔颜色
        /// </summary>
        /// <returns></returns>
        private void InitializeBrushsColor()
        {
            Color[] brushsColor_tmp = new Color[Masking.BrushCount];

            byte transparent = 0;//颜色透明度
            byte transparentIncrement = (byte)(byte.MaxValue / Masking.BrushCount);//颜色透明度增量
            for (int i = 0; i < Masking.BrushCount; i++)
            {
                Color color = this.BrushColor;
                if (i > 0)
                {
                    transparent += transparentIncrement;
                    transparent = Math.Min(transparent, byte.MaxValue);
                    color = Color.FromArgb(transparent, Math.Min(this.BrushColor.R, byte.MaxValue), Math.Min(this.BrushColor.G, byte.MaxValue), Math.Min(this.BrushColor.B, byte.MaxValue));
                }
                brushsColor_tmp[i] = color;
            }
            this.brushColors = brushsColor_tmp;
        }

        #endregion

    }

}
