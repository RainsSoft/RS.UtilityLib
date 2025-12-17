using System;
using System.ComponentModel;

namespace HML
{
    /// <summary>
    /// 动画工具类
    /// </summary>
    public static class AnimationHelper
    {
        #region Ease

        /// <summary>
        /// 变速
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <param name="power">动画曲线幂(默认值3.0) </param>
        /// <returns></returns>
        public static double EaseIn(double progressTime, double power)
        {
            power = Math.Max(0.0, power);
            return Math.Pow(progressTime, power);
        }

        /// <summary>
        /// 变速
        /// </summary>
        /// <param name="usedTime">动画已进行时间</param>
        /// <param name="allTime">动画总时间</param>
        /// <param name="power">动画曲线幂(默认值3.0) </param>
        /// <returns></returns>    
        public static double EaseOut(double usedTime, double allTime, double power)
        {
            return 1.0 - EaseIn(1.0 - (usedTime / allTime), power);
        }

        /// <summary>
        /// 变速
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <param name="power">动画曲线幂(默认值3.0)</param>
        /// <returns></returns>
        public static double EaseBoth(double progressTime, double power)
        {
            if (progressTime >= 0.5)
                return (1.0 - EaseIn((1.0 - progressTime) * 2.0, power)) * 0.5 + 0.5;
            return EaseIn(progressTime * 2.0, power) * 0.5;
        }

        #endregion

        #region Back

        /// <summary>
        /// 收缩
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <param name="power">动画曲线幂(默认值3.0) </param>
        /// <param name="amplitude">收缩与相关联的幅度动画。此值必须大于或等于 0。 默认值为 1。</param>
        /// <returns></returns>
        public static double BackIn(double progressTime, double power, double amplitude)
        {
            amplitude = Math.Max(0.0, amplitude);
            return Math.Pow(progressTime, power) - progressTime * amplitude * Math.Sin(Math.PI * progressTime);
        }

        /// <summary>
        /// 收缩
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <param name="power">动画曲线幂(默认值3.0) </param>
        /// <param name="amplitude">收缩与相关联的幅度动画。此值必须大于或等于 0。 默认值为 1.0。</param>
        /// <returns></returns>
        public static double BackOut(double progressTime, double power, double amplitude)
        {
            return 1.0 - BackIn(1.0 - progressTime, power, amplitude);
        }

        /// <summary>
        /// 收缩
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <param name="power">动画曲线幂(默认值3.0) </param>
        /// <param name="amplitude">收缩与相关联的幅度动画。此值必须大于或等于 0。 默认值为 1.0。 </param>
        /// <returns></returns>
        public static double BackBoth(double progressTime, double power, double amplitude)
        {
            if (progressTime >= 0.5)
                return (1.0 - BackIn((1.0 - progressTime) * 2.0, power, amplitude)) * 0.5 + 0.5;
            return BackIn(progressTime * 2.0, power, amplitude) * 0.5;
        }

        #endregion

        #region Quadratic

        /// <summary>
        /// 二次
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <returns></returns>
        public static double QuadraticIn(double progressTime)
        {
            return -4 * Math.Pow((progressTime - 0.5), 2.0) + 1.0;
        }

        /// <summary>
        /// 二次
        /// </summary>
        /// <param name="progressTime">已进行动画时间/总动画时间</param>
        /// <returns></returns>
        public static double QuadraticOut(double progressTime)
        {
            if (progressTime >= 0.5)
            {
                return 4 * Math.Pow((progressTime - 0.5 - 0.5), 2.0);
            }
            return 4 * Math.Pow((progressTime), 2.0);
        }

        #endregion
    }

}
