using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolderLinker
{
    /// <summary>
    /// 监控源文件夹，当源文件夹发生改动时，将改动同步到目标文件夹。
    /// </summary>
    public class DirectoryWatcher
    {
        private string dstDir;
        private string srcDir;
        private FileSystemWatcher watcher;
        public DirectoryWatcher(string src, string dst)
        {
            srcDir = src;
            dstDir = dst;
            watcher = new FileSystemWatcher(srcDir);
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
            this.AddEvents();
        }

        private void AddEvents()
        {
            watcher.Created += CreatedHandler;
            watcher.Deleted += DeletedHandler;
            watcher.Renamed += RenamedHandler;
            watcher.Error += ErrorHandler;
        }

        private void RemoveEvents()
        {
            watcher.Created -= CreatedHandler;
            watcher.Deleted -= DeletedHandler;
            watcher.Renamed -= RenamedHandler;
            watcher.Error -= ErrorHandler;
        }

        private void RenamedHandler(object sender, RenamedEventArgs e)
        {
            string dstOldName = Path.Combine(dstDir, e.OldName);
            string dstNewName = Path.Combine(dstDir, e.Name);
            if (Directory.Exists(dstOldName))
            {
                Directory.Move(dstOldName, dstNewName);
            }
        }

        private void ErrorHandler(object sender, ErrorEventArgs e)
        {
            this.RemoveEvents();
            watcher.Dispose();
            watcher = null;
            watcher = new FileSystemWatcher(dstDir);
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
            this.AddEvents();
        }

        private void DeletedHandler(object sender, FileSystemEventArgs e)
        {
            string dstPath = Path.Combine(dstDir, e.Name);
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath);
        }

        private void CreatedHandler(object sender, FileSystemEventArgs e)
        {
            string dstPath = Path.Combine(dstDir, e.Name);
            Linker linker = new Linker(dstPath, e.FullPath);
            linker.Link();
        }
    }
}
