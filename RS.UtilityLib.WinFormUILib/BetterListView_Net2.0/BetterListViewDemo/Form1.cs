using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ComponentOwl.BetterListView;
using ComponentOwl.BetterListView.Samples.CSharp;

namespace BetterListViewDemo
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            test(this.betterListView1);
            test(this.betterListView2);
            SetControls(this.betterListView2);
        }

        private void test(ComponentOwl.BetterListView.BetterListView listView ) {
            //var listView = this.betterListView1;

            listView.BeginUpdate();

            listView.Columns.AddRange(new[]
                {
                    new BetterListViewColumnHeader("First Name", 128),
                    new BetterListViewColumnHeader("Last Name", 128),
                    new BetterListViewColumnHeader("Year of Birth", 128)
                });

            listView.Items.AddRange(new[]
                {
                    new BetterListViewItem(new[]
                        {
                            "Phil", "Norton", "1956"
                        }),
                    new BetterListViewItem(new[]
                        {
                            "Joshua", "Wright", "1980"
                        }),
                    new BetterListViewItem(new[]
                        {
                            "Alex", "Skywalker", "1942"
                        }),
                    new BetterListViewItem(new[]
                        {
                            "Stanley", "Sawyer", "1967"
                        })
                });

            listView.Font = WinFormsUtils.CreateFont();
            listView.FullRowSelect = true;
            listView.View = BetterListViewView.Details;

            listView.EndUpdate();

           
        }
       
        private void SetControls(ComponentOwl.BetterListView.BetterListView listView) {
            var g = new GridLinesOptionsControl(listView);
        }
    }
}
