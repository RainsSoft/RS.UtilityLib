using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Globalization;

namespace RS.UtilityLib.WinFormCommon.UI
{
    /// <summary>
    /// Ping client user control.
    /// </summary>
    public  class PingClient : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PingClient() {
            InitializeComponent();
            _ping = new Ping();
        }

        private Ping _ping = null;

        /// <summary>
        /// Load user constrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingClient_Load(object sender, EventArgs e) {
            // Attach to the ping complete event.
            _ping.PingCompleted += new PingCompletedEventHandler(OnPingCompleted);
        }

        /// <summary>
        /// Send a ping request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPing_Click(object sender, EventArgs e) {
            try {
                // Select all the text in the address box.
                txtHostName.SelectAll();

                // If ip address and host name
                // has been entered.
                if (txtHostName.Text.Trim().Length != 0) {
                    // Disable the Send button.
                    btnPing.Enabled = false;

                    // Add the host address to the
                    // ping result details.
                    txtPingResult.Text +=
                        "Pinging " + txtHostName.Text + " . . .\r\n";

                    // Send ping request.
                    _ping.SendAsync(txtHostName.Text.Trim(), null);
                }
            }
            catch { }
        }

        /// <summary>
        /// Stop the ping request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e) {
            btnPing.Enabled = true;

            try {
                // Cancell the ping request.
                _ping.SendAsyncCancel();
            }
            catch { }
        }

        /// <summary>
        /// Ping complete event.
        /// </summary>
        /// <param name="sender">Current sender.</param>
        /// <param name="e">Event arguments.</param>
        void OnPingCompleted(object sender, PingCompletedEventArgs e) {
            // Check to see if an error occurred.  If no error, then display 
            // the address used and the ping time in milliseconds.
            if (e.Error == null) {
                // If the operation was cancelled.
                if (e.Cancelled)
                    txtPingResult.Text += "  Ping cancelled. \r\n";
                else {
                    // If the ping request succeded.
                    if (e.Reply.Status == IPStatus.Success) {
                        // Show the result of the
                        // ping request.
                        txtPingResult.Text +=
                            "  " + e.Reply.Address.ToString() + " " +
                            e.Reply.RoundtripTime.ToString(
                            NumberFormatInfo.CurrentInfo) + "ms" + "\r\n";
                    }
                    else
                        // If the ping was not succesful
                        // then get ip status.
                        txtPingResult.Text +=
                            "  " + GetStatusString(e.Reply.Status) + "\r\n";
                }
            }
            else {
                // Otherwise display the error.
                txtPingResult.Text += "  Ping error.\r\n";
                MessageBox.Show("An error occurred while sending this ping. " +
                    e.Error.InnerException.Message);
            }

            // Enable the send ping button
            // when the ping was complete.
            btnPing.Enabled = true;
        }

        /// <summary>
        /// Get the ping status
        /// </summary>
        /// <param name="status">The ip status code.</param>
        /// <returns>The ping status code.</returns>
        private string GetStatusString(IPStatus status) {
            switch (status) {
                case IPStatus.Success:
                    return "Success.";
                case IPStatus.DestinationHostUnreachable:
                    return "Destination host unreachable.";
                case IPStatus.DestinationNetworkUnreachable:
                    return "Destination network unreachable.";
                case IPStatus.DestinationPortUnreachable:
                    return "Destination port unreachable.";
                case IPStatus.DestinationProtocolUnreachable:
                    return "Destination protocol unreachable.";
                case IPStatus.PacketTooBig:
                    return "Packet too big.";
                case IPStatus.TtlExpired:
                    return "TTL expired.";
                case IPStatus.ParameterProblem:
                    return "Parameter problem.";
                case IPStatus.SourceQuench:
                    return "Source quench.";
                case IPStatus.TimedOut:
                    return "Timed out.";
                default:
                    return "Ping failed.";
            }
        }

        /// <summary>
        /// Text change on host name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHostName_TextChanged(object sender, EventArgs e) {
            if (!String.IsNullOrEmpty(txtHostName.Text))
                btnPing.Enabled = true;
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lblHostName = new System.Windows.Forms.Label();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.txtPingResult = new System.Windows.Forms.TextBox();
            this.btnPing = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Location = new System.Drawing.Point(5, 10);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(116, 13);
            this.lblHostName.TabIndex = 0;
            this.lblHostName.Text = "IP Address/Host Name";
            // 
            // txtHostName
            // 
            this.txtHostName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHostName.Location = new System.Drawing.Point(127, 7);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(221, 20);
            this.txtHostName.TabIndex = 1;
            this.txtHostName.TextChanged += new System.EventHandler(this.txtHostName_TextChanged);
            // 
            // txtPingResult
            // 
            this.txtPingResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPingResult.Location = new System.Drawing.Point(8, 36);
            this.txtPingResult.Multiline = true;
            this.txtPingResult.Name = "txtPingResult";
            this.txtPingResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPingResult.Size = new System.Drawing.Size(340, 132);
            this.txtPingResult.TabIndex = 2;
            // 
            // btnPing
            // 
            this.btnPing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPing.Enabled = false;
            this.btnPing.Location = new System.Drawing.Point(192, 174);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(75, 23);
            this.btnPing.TabIndex = 3;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(273, 174);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // PingClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPing);
            this.Controls.Add(this.txtPingResult);
            this.Controls.Add(this.txtHostName);
            this.Controls.Add(this.lblHostName);
            this.Name = "PingClient";
            this.Size = new System.Drawing.Size(356, 200);
            this.Load += new System.EventHandler(this.PingClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.TextBox txtHostName;
        private System.Windows.Forms.TextBox txtPingResult;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Button btnStop;
    }
}
