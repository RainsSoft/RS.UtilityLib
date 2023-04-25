using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace RS.UtilityLib.WinFormCommon
{
    public static class UIRes
    {
        static IResAccess m_resAccess=new UIResAccess();
        public static void Init(IResAccess resAccess) {
            m_resAccess = resAccess;
        }
        internal static Image GetImage(string file) {
            return m_resAccess.GetImage(file);
        }
        internal static string GetScript(string file) {
            return m_resAccess.GetScript(file);
        }
        internal static bool IsExists(string file) {
            return m_resAccess.IsExists(file);
        }
        internal static Stream GetResourceStream(string file) {
            return m_resAccess.GetResourceStream(file);
        }
    }

    public interface IResAccess
    {
        Image GetImage(string file);
        string GetString(string file);
        string GetScript(string file);
        bool IsExists(string strFileName);
        Stream GetResourceStream(string file);
    }
    class UIResAccess : IResAccess
    {

        #region IResAccess 成员

        public System.Drawing.Image GetImage(string file) {
            return new Bitmap(file);
        }

        public string GetString(string file) {
           return File.ReadAllText(file);
        }

        public string GetScript(string file) {
            throw new NotImplementedException();
        }

        public bool IsExists(string strFileName) {
            return File.Exists(strFileName);
        }

        public Stream GetResourceStream(string file) {
            return File.OpenRead(file);
        }

        #endregion
    }
}
