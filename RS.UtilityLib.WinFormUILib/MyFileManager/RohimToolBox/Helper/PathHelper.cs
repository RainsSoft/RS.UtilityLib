using RohimToolBox.Enums;
using RohimToolBox.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RohimToolBox.Helper {
  public class PathHelper {

    /// <summary>
    /// C:\aaa => C:\aaa\, C:\bbb\ => C:\bbb\
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/20406065/8140473"/>
    /// <returns></returns>
    public static string EnsureTrailingSlash(string path) {
      string separator1 = Path.DirectorySeparatorChar.ToString();
      string separator2 = Path.AltDirectorySeparatorChar.ToString();

      // Trailing white spaces are always ignored
      path = path.TrimEnd();

      if (path.EndsWith(separator1) || path.EndsWith(separator2)) { 
        return path;
      }

      if (path.Contains(separator2)) { 
        return path + separator2;
      }

      return path + separator1;
    }


    /// <summary>
    /// return FileSystemItemType.Folder or FileSystemItemType.File
    /// </summary>
    /// <param name="path"></param>
    /// <see cref="https://stackoverflow.com/a/1395226/8140473"/>
    /// <returns></returns>
    public static FileSystemItemType GetPathType(string path) {
      // get the file attributes for file or directory
      FileAttributes attr = File.GetAttributes(path);

      //detect whether its a directory or file
      if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
        return FileSystemItemType.Folder;
      } else {
        return FileSystemItemType.File;
      }
    }

    public static List<FileSystemItem> GetFirstLevelItems(string path) {
      List<FileSystemItem> results = new List<FileSystemItem>();

      string[] dirs = Directory.GetDirectories(path);
      List<FileSystemItem> folderItems = dirs
        .Select(filePath => new FileSystemItem(filePath, new DirectoryInfo(filePath).Name))
        .ToList();
      results.AddRange(folderItems);

      string[] files = Directory.GetFiles(path);
      List<FileSystemItem> fileItems = files
        .Select(filePath => new FileSystemItem(filePath, new FileInfo(filePath).Name, new FileInfo(filePath).Length))
        .ToList();
      results.AddRange(fileItems);

      return results;
    }

    /// <summary>
    /// get file extension. <br/>
    /// "AAA.TXT" -> ".txt", "ABC.xlsx" -> ".xlsx", null -> null, "SSS" -> string.Empty
    /// </summary>
    /// <returns></returns>
    public static string GetExtension(string path) {
      string fileExtension = Path.GetExtension(path);
      return fileExtension?.ToLower();
    }

    ///// <summary>
    ///// https://stackoverflow.com/questions/468119/whats-the-best-way-to-calculate-the-size-of-a-directory-in-net
    ///// </summary>
    ///// <param name="d"></param>
    ///// <returns></returns>
    //public static async Task<long> GetDirectorySize(DirectoryInfo d) {
    //  long size = 0;

    //  // Add file sizes.
    //  FileInfo[] fis = d.GetFiles();
    //  foreach (FileInfo fi in fis) {
    //    size += fi.Length;
    //  }

    //  // Add subdirectory sizes.
    //  DirectoryInfo[] dis = d.GetDirectories();
    //  foreach (DirectoryInfo di in dis) {
    //    size += await GetDirectorySize(di);        
    //  }

    //  return size;
    //}

  }
}
