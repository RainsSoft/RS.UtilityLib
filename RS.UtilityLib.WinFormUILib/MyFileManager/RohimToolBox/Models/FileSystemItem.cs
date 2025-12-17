using RohimToolBox.Enums;
using RohimToolBox.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RohimToolBox.Models {
  public class FileSystemItem {
    public FileSystemItem(string fullPath, string itemName, long sizeInByte = 0) {
      this.FullPath = fullPath;
      this.ItemName = itemName;
      this.ItemSize = sizeInByte;
    }

    private string fullPath;

    /// <summary>
    /// e.g. D:\ccc\eee\, C:\aaa\bbb.txt <br/>
    /// including trailing slash 
    /// </summary>
    public string FullPath { 
      get => this.fullPath; 
      set {
        this.ItemType = PathHelper.GetPathType(value);
        switch (this.ItemType) {
          case FileSystemItemType.Folder:
            this.fullPath = PathHelper.EnsureTrailingSlash(value);
            break;
          case FileSystemItemType.File:
            this.fullPath = value;
            this.FileExtension = PathHelper.GetExtension(value);
            break;
        }
      } 
    }

    /// <summary>
    /// e.g. D:\ccc\eee\ => "eee", C:\aaa\bbb.txt => "bbb.txt"
    /// </summary>
    public string ItemName { get; set; }

    /// <summary>
    /// return null if Folder type. <br/>
    /// "AAA.TXT" -> ".txt", "ABC.xlsx" -> ".xlsx", "SSS" -> string.Empty
    /// </summary>
    public string FileExtension { get; private set; }

    /// <summary>
    /// Enum: Folder or File
    /// </summary>
    public FileSystemItemType ItemType { get; private set; }

    public long ItemSize { get; set; }
  }
}
