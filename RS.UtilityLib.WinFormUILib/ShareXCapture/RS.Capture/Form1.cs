using HelpersLib;
using RS.Capture.Properties;
using ScreenCapture;
using ShareX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RS.Capture
{
    public partial class Form1 : Form
    {
        private delegate Image ScreenCaptureDelegate();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Resources.RS_snap;
            CaptureScreenshot(CaptureType.Rectangle);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
      
        }

        public void CaptureScreenshot(CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            if (taskSettings == null) { 
                taskSettings = TaskSettings.GetDefaultTaskSettings();
            }

            switch (captureType)
            {
                case CaptureType.Screen:
                    DoCapture(Screenshot.CaptureFullscreen, CaptureType.Screen, taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveWindow:
                    //CaptureActiveWindow(taskSettings, autoHideForm);
                    break;
                case CaptureType.ActiveMonitor:
                    DoCapture(Screenshot.CaptureActiveMonitor, CaptureType.ActiveMonitor, taskSettings, autoHideForm);
                    break;
                case CaptureType.RectangleWindow:
                case CaptureType.Rectangle:
                case CaptureType.RoundedRectangle:
                case CaptureType.Ellipse:
                case CaptureType.Triangle:
                case CaptureType.Diamond:
                case CaptureType.Polygon:
                case CaptureType.Freehand:
                    CaptureRegion(captureType, taskSettings, autoHideForm);
                    break;
                case CaptureType.LastRegion:
                    //CaptureLastRegion(taskSettings, autoHideForm);
                    break;
            }
        }
        private void CaptureRegion(CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            Surface surface;

            switch (captureType)
            {
                default:
                case CaptureType.Rectangle:
                    //if (taskSettings.AdvancedSettings.UseLightRectangleCrop)
                    //{
                    //    CaptureLightRectangle(taskSettings, autoHideForm);
                    //    return;
                    //}

                    surface = new RectangleRegion();
                    break;
                case CaptureType.RectangleWindow:
                    RectangleRegion rectangleRegion = new RectangleRegion();
                    rectangleRegion.AreaManager.WindowCaptureMode = true;
                    surface = rectangleRegion;
                    break;
                case CaptureType.RoundedRectangle:
                    surface = new RoundedRectangleRegion();
                    break;
                case CaptureType.Ellipse:
                    surface = new EllipseRegion();
                    break;
                case CaptureType.Triangle:
                    surface = new TriangleRegion();
                    break;
                case CaptureType.Diamond:
                    surface = new DiamondRegion();
                    break;
                case CaptureType.Polygon:
                    surface = new PolygonRegion();
                    break;
                case CaptureType.Freehand:
                    surface = new FreeHandRegion();
                    break;
            }

            DoCapture(() =>
            {
                Image img = null;
                Image screenshot = Screenshot.CaptureFullscreen();

                try
                {
                    surface.Config = taskSettings.SurfaceOptions;
                    surface.SurfaceImage = screenshot;
                    surface.Prepare();
                    surface.ShowDialog();

                    if (surface.Result == SurfaceResult.Region)
                    {
                        img = surface.GetRegionImage();
                        screenshot.Dispose();
                    }
                    else if (surface.Result == SurfaceResult.Fullscreen)
                    {
                        img = screenshot;
                    }
                }
                finally
                {
                    surface.Dispose();
                }

                return img;
            }, captureType, taskSettings, autoHideForm);
        }
        private void DoCapture(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings = null, bool autoHideForm = true)
        {
            //if (taskSettings == null) taskSettings = TaskSettings.GetDefaultTaskSettings();

            //if (taskSettings.CaptureSettings.IsDelayScreenshot && taskSettings.CaptureSettings.DelayScreenshot > 0)
            //{
            //    int sleep = (int)(taskSettings.CaptureSettings.DelayScreenshot * 1000);
            //    BackgroundWorker bw = new BackgroundWorker();
            //    bw.DoWork += (sender, e) => Thread.Sleep(sleep);
            //    bw.RunWorkerCompleted += (sender, e) => DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
            //    bw.RunWorkerAsync();
            //}
            //else
            //{
                DoCaptureWork(capture, captureType, taskSettings, autoHideForm);
            //}
        }
        private void DoCaptureWork(ScreenCaptureDelegate capture, CaptureType captureType, TaskSettings taskSettings, bool autoHideForm = true)
        {
            //if (autoHideForm)
            {
                Hide();
                Thread.Sleep(250);
            }

            Image img = null;

            try
            {
                Screenshot.CaptureCursor = taskSettings.CaptureSettings.ShowCursor;
                Screenshot.CaptureShadow = taskSettings.CaptureSettings.CaptureShadow;
                Screenshot.ShadowOffset = taskSettings.CaptureSettings.CaptureShadowOffset;
                Screenshot.CaptureClientArea = taskSettings.CaptureSettings.CaptureClientArea;
                Screenshot.AutoHideTaskbar = taskSettings.CaptureSettings.CaptureAutoHideTaskbar;

                img = capture();

                if (img != null )//&& taskSettings.GeneralSettings.PlaySoundAfterCapture)
                {
                    Helpers.PlaySoundAsync(Resources.Camera);
                    //img.Save("capture.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    string file;
                    using (SaveFileDialog sd = new SaveFileDialog())
                    {
                        sd.Title = "保存为...";
                        sd.DefaultExt = ".bmp";
                        sd.Filter = "图像文件|*.bmp";
                        if (sd.ShowDialog() == DialogResult.OK)
                        {
                            file = sd.FileName;
                            img.Save(file, System.Drawing.Imaging.ImageFormat.Bmp);
                        }                        
                        else
                        {
                            //return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误："+ex.ToString());
                //DebugHelper.WriteException(ex);
            }
            finally
            {
                //if (autoHideForm)
                //{
                   this.Close();
                //}

                //AfterCapture(img, captureType, taskSettings);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CaptureScreenshot(CaptureType.Rectangle);
        }
    }
}
