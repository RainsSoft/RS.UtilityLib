using RohimToolBox.Enums;
using RohimToolBox.Models;
using RohimToolBox.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RohimToolBox.Extensions {
  public static class ListViewExtensions {

    public static void SetItems(this ListView listView, IEnumerable<FileSystemItem> fileSystemItems) {
      // TODO: handle file without file extension

      listView.BeginUpdate();
      listView.Items.Clear();
      ImageList imageList = listView.SmallImageList;
      foreach (FileSystemItem item in fileSystemItems) {
        string imageKey = item.ItemType == FileSystemItemType.Folder
          ? FileSystemItemType.Folder.ToString()
          : item.FileExtension;

        // https://www.notion.so/rohimchou/C-File-Extract-the-Icon-Associated-with-a-File-edf1902f364f4dcea011365cf26b75f8
        if (item.ItemType == FileSystemItemType.File
          && !imageList.Images.ContainsKey(item.FileExtension)) {

          Icon iconForItem = Icon.ExtractAssociatedIcon(item.FullPath);
          imageList.Images.Add(item.FileExtension, iconForItem.ToBitmap());
        }

        ListViewItem listViewItem = listView.Items.Add(item.FullPath, item.ItemName, imageKey);
        string itemSize = FormatFileItemSize(item.ItemSize);
        listViewItem.SubItems.Add(itemSize);
      }
      listView.EndUpdate();
    }

    private static string FormatFileItemSize(long itemSize) {
      int sizeInKb = (int)Math.Round(itemSize / 1000d, MidpointRounding.AwayFromZero);
      string result = $"{sizeInKb:N0} KB";

      if (sizeInKb == 0) {
        return null;
      }
      return result;
    }
  }
}
