using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
            this.lb_NameList.OnSelect += lb_NameList_MouseUp;
            this.lb_NameList.LostFocus += lb_NameList_LostFocus;
            this.lb_NameList.OnDelete += new Action<object>(lb_NameList_OnDelete);
        }

        private void btn_LoginNameList_Click(object sender, EventArgs e) {
            this.lb_NameList.Visible = !this.lb_NameList.Visible;
            if (this.lb_NameList.Visible) {
                this.lb_NameList.Focus();
                if (this.lb_NameList.Items.Count > 0) {
                    this.lb_NameList.ScrollControlIntoView(this.lb_NameList.Items[0]);
                }
            }
        }
        void lb_NameList_LostFocus(object sender, EventArgs e) {
            this.lb_NameList.Visible = false;
        }

        void lb_NameList_MouseUp(object obj) {

            if (obj == null) {
                return;
            }
            //NamePassword np = obj as NamePassword;

            //this.txt_LoginUserName.Text = np.UserName;

            //this.txt_LoginPwd.Text = np.Pwd;

            this.lb_NameList.Visible = false;
            //this.txt_LoginUserName.Focus();

        }
        void lb_NameList_OnDelete(object obj) {
            //LoginService.DeleteNameFromList(obj.ToString());
        }
        private void Form1_Load(object sender, EventArgs e) {
            List<NamePassword> nps = new List<NamePassword>();
            for (int i = 0; i < 10; i++) {
                NamePassword np = new NamePassword("test_" + i, "pwd_" + i);
                nps.Add(np);
            }
            foreach (var v in nps) {
                this.lb_NameList.AddItem(v);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            btn_LoginNameList_Click(sender, e);
        }

        public class NamePassword
        {
            public NamePassword(string userName, string pwd) {
                this.m_UserName = userName;
                this.m_Pwd = pwd;
            }
            private string m_UserName;
            private string m_Pwd;
            public string UserName {
                get {
                    return m_UserName;
                }
            }
            public string Pwd {
                get {
                    return m_Pwd;
                }
                internal set {
                    m_Pwd = value;
                }
            }
            public static string EncodePwd(string pwd) {
                string str = EncryStrHex(pwd);
                str = EncodeBase64(str);
                return str;
            }
            public static string DecodePwd(string pwd) {
                string str = DecodeBase64(pwd);
                str = DecryStrHex(str);
                return str;
            }
            public override string ToString() {
                return this.UserName;
            }
            #region
            private static string EncodeBase64(Encoding encode, string source) {
                byte[] bytes = encode.GetBytes(source);
                return Convert.ToBase64String(bytes);
            }
            private static string EncodeBase64(string source) {
                return EncodeBase64(Encoding.UTF8, source);
            }
            private static string DecodeBase64(Encoding encode, string result) {

                byte[] bytes = Convert.FromBase64String(result);
                return encode.GetString(bytes);
            }
            private static string DecodeBase64(string result) {
                return DecodeBase64(Encoding.UTF8, result);
            }


            private static string EncryStrHex(string aText) {
                StringBuilder sb = new StringBuilder("");
                byte[] bsText = Encoding.UTF8.GetBytes(aText);
                byte[] bsKey = Encoding.UTF8.GetBytes("rains");
                int j = 0;

                foreach (byte b in bsText) {
                    if (j == bsKey.Length)
                        j = 0;
                    sb.AppendFormat("{0:X2}", b ^ bsKey[j]);
                    j++;
                }

                return sb.ToString();
            }

            private static string DecryStrHex(string aText) {
                byte[] bs = new byte[aText.Length / 2];
                byte[] bsKey = Encoding.UTF8.GetBytes("rains");
                byte b;
                int j = 0;

                for (int i = 0; i < aText.Length / 2; i++) {
                    if (j == bsKey.Length)
                        j = 0;
                    b = (byte)Convert.ToInt16(aText.Substring(i * 2, 2), 16);
                    bs[i] = (byte)(b ^ bsKey[j]);
                    j++;
                }

                return Encoding.UTF8.GetString(bs, 0, bs.Length);
            }
            #endregion
        }


    }
}
