using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace HML
{
    /// <summary>
    /// 监控信息管理
    /// </summary>
    public static class MonitorManager
    {
        /// <summary>
        /// 动画定时器状态
        /// </summary>
        public static bool GetAnimationTimerEnabled
        {
            get
            {
                string fielName = "timer";
                Type type = typeof(MainThreadAnimationControl);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                Timer timer = (Timer)fiel.GetValue(null);
                return (timer == null) ? false : timer.Enabled;
            }
        }
        /// <summary>
        /// 当前要处理动画数量
        /// </summary>
        public static int GetAnimationCount
        {
            get
            {
                string fielName = "animationcontrols";
                Type type = typeof(MainThreadAnimationControl);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                List<MainThreadAnimationControl> list = (List<MainThreadAnimationControl>)fiel.GetValue(null);
                return (list == null || list.Count < 1) ? 0 : list.Count;
            }
        }
        /// <summary>
        /// 当前要处理空动画数量
        /// </summary>
        public static int GetNullAnimationCount
        {
            get
            {
                string fielName = "animationcontrols";
                Type type = typeof(MainThreadAnimationControl);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                List<MainThreadAnimationControl> list = (List<MainThreadAnimationControl>)fiel.GetValue(null);
                return (list == null || list.Count < 1) ? 0 : list.FindAll(a => a == null).Count;
            }
        }

        /// <summary>
        /// 获取蒙版弹层数量
        /// </summary>
        public static int GetMaskingMaskingNativeWindowCount
        {
            get
            {
                string fielName = "maskingObjectCollection";
                Type type = typeof(Masking);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                List<MaskingObject> list = (List<MaskingObject>)fiel.GetValue(null);
                return (list == null || list.Count < 1) ? 0 : list.Count;
            }
        }
        /// <summary>
        /// 获取蒙版弹层定时器状态
        /// </summary>
        public static bool GetMaskingMaskingTimerEnabled
        {
            get
            {
                string fielName = "rotateTimer";
                Type type = typeof(Masking);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                Timer timer = (Timer)fiel.GetValue(null);
                return (timer == null) ? false : timer.Enabled;
            }
        }

        /// <summary>
        /// DropDownListPlus下拉列表弹层鼠标钩子数量
        /// </summary>
        public static int GetDropDownListPlusHookCount
        {
            get
            {
                string fielName = "hook_id_array";
                Type type = typeof(DropDownListPlus.DropDownListPlusNativeWindow);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                ArrayList arr = (ArrayList)fiel.GetValue(null);
                return (arr == null || arr.Count < 1) ? 0 : arr.Count;
            }
        }

        /// <summary>
        /// SkinPicker下拉列表弹层鼠标钩子数量
        /// </summary>
        public static int GetSkinPickerHookCount
        {
            get
            {
                string fielName = "hook_id_array";
                Type type = typeof(SkinPicker);
                FieldInfo fiel = type.GetField(fielName, BindingFlags.NonPublic | BindingFlags.Static);
                if (fiel == null)
                {
                    throw new NullReferenceException(type.FullName + " 找不到 " + fielName);
                }
                ArrayList arr = (ArrayList)fiel.GetValue(null);
                return (arr == null || arr.Count < 1) ? 0 : arr.Count;
            }
        }

    }
}
