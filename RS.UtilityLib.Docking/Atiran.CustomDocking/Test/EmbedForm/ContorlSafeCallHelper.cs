using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Test.EmbedForm
{
    public delegate TResult Func<TResult>();
    public delegate void Action();
    public static class ContorlSafeCallHelper
    {
        /// <summary>
        /// 非安全模式执行 修改控件属性
        /// 前提需要设置  System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls=false;
        /// 先检查contrl c是否创建并没有销毁 然后执行action ac对contrl c的属性直接修改
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ac"></param>
        public static void UnSafeAction(this Control c, Action ac) {
            if (CheckHasCreatedControl(c) == false) {
                return;
            }
            try {
                ac();
            }
            catch (Exception ee) {
                System.Diagnostics.Debug.Assert(true, "UnSafeAction-action()错误");
                Console.WriteLine("UnSafeAction: 执行action()异常" + ee.ToString());
            }

        }
        /// <summary>
        /// 非安全模式执行 获取控件属性
        /// 前提需要设置  System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls=false;
        /// 先检查contrl c是否创建并没有销毁 然后执行获取Func func对contrl c的属性对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="_default"></param>
        /// <returns></returns>
        public static T UnSafeFunc<T>(this Control c, Func<T> func, T _default) {
            if (CheckHasCreatedControl(c)) {
                try {
                    var TR = func();
                    return TR;
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-action()错误");
                    Console.WriteLine("SafeFunc<T>: 执行func()异常" + ee.ToString());
                }
            }
            return _default;//default(T);
        }
        #region 跨线程访问控件辅助方法
        /// <summary>
        /// 安全读写Control属性,外面注意Invoke嵌套使用的死锁问题
        /// </summary>
        /// <param name="c"></param>
        /// <param name="ac"></param>
        /// <param name="asyn"></param>
        public static void SafeAction(this Control c, Action ac, bool asyn = false) {
            if (CheckHasCreatedControl(c)) {
                Action acWrapper = () => {
                    try {
                        ac();
                    }
                    catch (Exception ee) {
                        System.Diagnostics.Debug.Assert(true, "SafeAction-action()错误");
                        Console.WriteLine("SafeAction: 执行action()异常" + ee.ToString());
                    }
                };
                try {
                    if (asyn) {
                        c.BeginInvoke(acWrapper);
                    }
                    else {
                        if (c.InvokeRequired) {
                            c.Invoke(acWrapper);
                        }
                        else {
                            acWrapper();
                        }
                    }
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeAction-控件委托错误");
                    Console.WriteLine("SafeAction:执行控件委托异常：" + ee.ToString());
                }
            }
        }
        /// <summary>
        /// 安全获取Control属性值,外面注意Invoke嵌套使用的死锁问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="func"></param>
        /// <param name="_default"></param>
        /// <returns></returns>
        public static T SafeFunc<T>(this Control c, Func<T> func, T _default) {
            if (CheckHasCreatedControl(c)) {
                var TR = _default;//default(T);
                Action acWrapper = () => {
                    try {
                        TR = func();
                    }
                    catch (Exception ee) {
                        System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-action()错误");
                        Console.WriteLine("SafeFunc<T>: 执行func()异常" + ee.ToString());
                    }
                };

                try {
                    if (c.InvokeRequired) {
                        c.Invoke(acWrapper);
                        return TR;
                    }
                    else {
                        acWrapper();
                        return TR;
                    }
                }
                catch (Exception ee) {
                    System.Diagnostics.Debug.Assert(true, "SafeFunc<T>-控件委托错误");
                    Console.WriteLine("SafeFunc<T>:执行控件委托异常：" + ee.ToString());
                }
            }
            return _default;//default(T);
        }
        /// <summary>
        /// 检测Control是否已经创建好(已经Show)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckHasCreatedControl(this Control c) {
            try {
                if (c == null ||
                    c.IsDisposed ||
                    c.IsHandleCreated == false ||
                    c.FindForm().IsHandleCreated == false
                    ) {
                    // some exceptional condition:
                    // handle in whatever way is appropriate for your app
                    return false;
                }
                return true;
            }
            catch (Exception ee) {
                Console.WriteLine("CheckHasCreatedControl:" + ee.ToString());
            }
            return false;
        }
        #endregion
    }
}
