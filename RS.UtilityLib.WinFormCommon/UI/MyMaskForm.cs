using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon.UI
{
    public delegate DialogResult FuncDR<DialogResult>();
    public class MyMaskForm:Form
    {
        public MyMaskForm() : base() {

            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.BackColor = System.Drawing.Color.Silver;
            this.Opacity = 0.7D;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Escape) {
                    this.Close();
                }
            };
          
        }
        public static DialogResult ShowDialogWithMask(FuncDR<DialogResult> funcDialogResult) {
            System.Windows.Forms.DialogResult Result = System.Windows.Forms.DialogResult.None;
            var Temp = new MyMaskForm();
            Temp.Load += (sender, e) =>
            {
                //using (var Confirm = new ConfirmMessageBox()) {
                //    Confirm.Caption = Caption;
                //    Confirm.SetMessage = Message;
                //    Result = Confirm.ShowDialog();
                //    Temp.Close();
                //}
                Result = funcDialogResult();
                Temp.Close();
            };
            Temp.ShowDialog();
            return Result;
        }
        public static void ShowFormWithMask(Form owner, Form displayForm) {
            var Temp = new MyMaskForm();         
            Temp.Load += (sender, e) => {
                displayForm.Disposed += (sender2,e2)=> {
                    Temp.Close();
                };
                displayForm.Show(Temp);
            };
            Temp.Show(owner);
            //
            //ShowDialogWithMask(() => {
            //    return MessageBox.Show("这是一个测试");                
            //});
        }

      
    }
}
