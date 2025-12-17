using RohimToolBox.CustControls;
using RohimToolBox.Enums;
using RohimToolBox.Extensions;
using RohimToolBox.Helper;
using RohimToolBox.Models;
using RohimToolBox.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RohimToolBox.Forms.TWSBatHelper {
	public partial class frmFileExplorerPage : Form {
		public readonly string SEARCH_PLACEHOLDER = "Search...";
		private ImageList ilstLvwFiles;

		public frmFileExplorerPage() {
			InitializeComponent();

			// initialize listview
			this.lvwFiles.View = View.Details;
			this.lvwFiles.FullRowSelect = true;
			this.lvwFiles.Font = new Font("Consolas", 10f);
			this.lvwFiles.Columns.Add(new ColumnHeader { Text = "Name", Width = 200 });
			this.lvwFiles.Columns.Add(new ColumnHeader { Text = "Size", Width = 90 });
			this.ilstLvwFiles = new ImageList();
			this.ilstLvwFiles.Images.Add($"{FileSystemItemType.Folder}", Resources.icon_folder);
			this.ilstLvwFiles.Images.Add($"{FileSystemItemType.File}", Resources.icon_file);
			this.ilstLvwFiles.ImageSize = new Size(18, 18);
			this.lvwFiles.SmallImageList = ilstLvwFiles;

			// initialize statusstrip
			this.statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStripStatusLabel1.Alignment = ToolStripItemAlignment.Right;
			this.toolStripStatusLabel1.Text = $"0 selected ";

			// load root drives
			this.LoadRootDrives(this.lvwFiles);
		}

		/// <summary>
		/// Hide Placeholder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearchText_Enter(object sender, EventArgs e) {
			if (this.txtSearchText.Text == SEARCH_PLACEHOLDER) {
				this.txtSearchText.Text = null;
				this.txtSearchText.ForeColor = Color.Black;
			}
		}

		/// <summary>
		/// Show Placeholder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtSearchText_Leave(object sender, EventArgs e) {
			if (string.IsNullOrEmpty(this.txtSearchText.Text)) {
				this.txtSearchText.Text = SEARCH_PLACEHOLDER;
				this.txtSearchText.ForeColor = Color.DarkGray;
			}
		}

		/// <summary>
		/// Do not close ContextMenuStrip on selection of certain items
		/// https://stackoverflow.com/a/10660751/8140473
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsSearchText_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) {
				e.Cancel = true;
			}
		}

		/// <summary>
		/// double-click would trigger this method 
		/// (if lvwFiles.Activation == Standard) 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvwFiles_ItemActivate(object sender, EventArgs e) {
			ListView listView = (ListView)sender;
			if (listView.SelectedItems.Count > 0) {
				ListViewItem selectedLvwItem = listView.SelectedItems[0];

				string path = selectedLvwItem.Name;
				FileSystemItemType pathType = PathHelper.GetPathType(path);

				switch (pathType) {
					// if double clicked item is a directory
					case FileSystemItemType.Folder:
						if (Directory.Exists(path)) {
							this.txtWorkingPath.Text = path;
							this.LoadFirstLevelItems(this.lvwFiles, selectedLvwItem.Name);
						}
						break;

					// if double clicked item is a file
					case FileSystemItemType.File:
						Process.Start(path);
						break;
				}
			}
		}

		private void btnNavUpper_Click(object sender, EventArgs e) {
			string workingPath = this.txtWorkingPath.Text;

			// do nothing if already topmost level
			if (string.IsNullOrEmpty(workingPath)) {
				return;
			}

			// return if invalid working path
			if (!Directory.Exists(workingPath)) {
				MessageBox.Show("Directory not exists");
				return;
			}

			DirectoryInfo currentDirInfo = new DirectoryInfo(workingPath);
			string parentFullPath;
			if (currentDirInfo.Parent is null) {
				// root
				parentFullPath = null;
				this.LoadRootDrives(this.lvwFiles);

			} else {
				parentFullPath = currentDirInfo.Parent.FullName;
				this.LoadFirstLevelItems(this.lvwFiles, parentFullPath);
			}

			this.txtWorkingPath.Text = parentFullPath;
		}

		/// <summary>
		/// Get root drives for current computer, and update UI
		/// </summary>
		/// <param name="listView"></param>
		private void LoadRootDrives(ListView listView) {
			DriveInfo[] driveInfos = DriveInfo.GetDrives();
			List<FileSystemItem> fileSystemItems = driveInfos
			  .Select(x => new FileSystemItem(x.Name, x.Name))
			  .ToList();
			listView.SetItems(fileSystemItems);
		}

		/// <summary>
		/// Given a directory, Load it's direct level child items, and update UI
		/// </summary>
		/// <param name="listView"></param>
		/// <param name="dirPath"></param>
		private void LoadFirstLevelItems(ListView listView, string dirPath) {
			List<FileSystemItem> firstLvlItems = PathHelper.GetFirstLevelItems(dirPath);
			listView.SetItems(firstLvlItems);
		}

		private void lvwFiles_KeyDown(object sender, KeyEventArgs e) {
			// ctrl+a: select all
			if (e.Control && e.KeyCode == Keys.A) {
				foreach (ListViewItem item in this.lvwFiles.Items) {
					item.Selected = true;
				}
			}

			// ctrl+c: copy files
			if (e.Control && e.KeyCode == Keys.C) {
				if (this.lvwFiles.SelectedItems.Count > 0) {
					StringCollection fileFullPathsCollection = new StringCollection();
					this.lvwFiles
						.SelectedItems
						.OfType<ListViewItem>()
						.ToList()
						.ForEach(x => fileFullPathsCollection.Add(x.Name));

					Clipboard.SetFileDropList(fileFullPathsCollection);

					this.toolStripStatusLabel1.Text =
						$"{this.lvwFiles.SelectedItems.Count} Items Copied! ";
				}
			}

			// ctrl+shift+c: copy files's paths
			if (e.Control && e.Shift && e.KeyCode == Keys.C) {
				if (this.lvwFiles.SelectedItems.Count > 0) {
					List<string> filePullPaths = this.lvwFiles
					.SelectedItems
					.OfType<ListViewItem>()
					.Select(x => x.Name)
					.ToList();

					string filePullPathsStr = string.Join(Environment.NewLine, filePullPaths);
					Clipboard.SetText(filePullPathsStr);

					this.toolStripStatusLabel1.Text =
						$"{this.lvwFiles.SelectedItems.Count} Paths Copied! ";
				}
			}
		}

		private void lvwFiles_SelectedIndexChanged(object sender, EventArgs e) {
			// show selected files count
			this.toolStripStatusLabel1.Text =
			  $"{this.lvwFiles.SelectedItems.Count} selected ";
		}
	}
}
