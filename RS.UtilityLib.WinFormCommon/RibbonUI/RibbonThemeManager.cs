using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.RibbonUI
{
    //class RibbonThemeManager


    /// <summary>
    /// 管理Theme的公共资源
    /// </summary>
    public class RibbonThemeManager
    {
        private static RibbonThemeManager m_instance;
        //public static RibbonThemeManager Singleton {
        //    get {
        //        if (m_instance == null) {
        //            m_instance = new RibbonThemeManager();
        //        }
        //        return m_instance;
        //    }
        //}
        static RibbonThemeManager() {
            if (ImageAttr == null) {
                // AlphaBlend an image, alpha [0.01,1]
                float alpha = 0.9f;
                if (alpha > 1f)
                    alpha = 1f;
                else if (alpha < 0.01f)
                    alpha = 0.01f;
                using (ImageAttributes ia = new ImageAttributes()) {
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix00 = 1f;
                    cm.Matrix11 = 1f;
                    cm.Matrix22 = 1f;
                    cm.Matrix44 = 1f;
                    cm.Matrix33 = alpha;
                    ia.SetColorMatrix(cm);
                }
            }
            if (DisableImageAttr == null) {
                float[][] newColorMatrix = new float[5][];
                //newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
                //newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
                //newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
                //newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
                //newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

                newColorMatrix[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
                newColorMatrix[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
                newColorMatrix[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
                newColorMatrix[3] = new float[] { 0, 0, 0, 1f, 0f };
                newColorMatrix[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };

                ColorMatrix matrix = new ColorMatrix(newColorMatrix);
                DisableImageAttr = new ImageAttributes();
                DisableImageAttr.ClearColorKey();
                DisableImageAttr.SetColorMatrix(matrix);
            }

        }
        static void Clear() {
            if (DisableImageAttr != null) {
                DisableImageAttr.Dispose();
            }
            if (ImageAttr != null) {
                ImageAttr.Dispose();
            }
        }
        private static System.Drawing.Color m_Color = SystemColors.Control;
        private static System.Drawing.Color m_TextColor = Color.FromArgb(36, 36, 36);
        //private static ColorScheme m_ColorScheme = ColorScheme.Custom;
        public static Color BackColor {
            get { return m_Color; }
            set {
                m_Color = value;
                //foreach (var v in m_Controls) {
                //    (v as IRibbon).OnRibbonThemeChanged();
                //}
                //if (m_ImgAttr == null) {
                //    m_ImgAttr = new ImageAttributes();
                //}
                //m_ImgAttr.ClearColorMatrix();
                //float[][] matrix=new float[]{
                //    new float[]{},
                //    new float[]{},
                //    new float[]{},
                //    new float[]{}
                //};
                //ColorMatrix cm=new ColorMatrix(matrix);
                //m_ImgAttr.SetColorMatrix(cm);
            }
        }
        public static Color TextColor {
            get { return m_TextColor; }
            set { m_TextColor = value; }
        }
        public static Color HighLightTextColor {
            get;
            set;
        }

        /// <summary>
        /// 公共的颜色变换效果
        /// </summary>
        public static ImageAttributes ImageAttr;
        /// <summary>
        /// 公共的不可用效果
        /// </summary>
        public static ImageAttributes DisableImageAttr;
        //public static ColorScheme ColorScheme {
        //    get { return m_ColorScheme; }
        //    set { m_ColorScheme = value; }
        //}

        private static List<Control> m_Controls = new List<Control>();
        private static object m_Locker = new object();
        public static void RegisteControl(Control c) {
            lock (m_Locker) {
                c.Disposed += new EventHandler(c_Disposed);
                m_Controls.Add(c);
            }
        }

        static void c_Disposed(object sender, EventArgs e) {
            lock (m_Locker) {
                m_Controls.Remove(sender as Control);
            }
        }

        /// <summary>
        /// 公共的Font
        /// </summary>
        public static Font NormalFont = new Font("宋体", 9);
        public static Font NormalFont_11 = new Font("宋体", 11);
        public static Font NormalFont_12 = new Font("宋体", 12);

        public static Font NormalFont_Blob = new Font("宋体", 9, FontStyle.Bold);
        public static Font NormalFont_11_Blob = new Font("宋体", 11, FontStyle.Bold);
        public static Font NormalFont_12_Blob = new Font("宋体", 12, FontStyle.Bold);
        public static bool IsCacheFont(Font f) {
            //return false;
            return f == NormalFont
                || f == NormalFont_11
                || f == NormalFont_12
                || f == NormalFont_Blob
                || f == NormalFont_11_Blob
                || f == NormalFont_12_Blob;

        }
    }
}
