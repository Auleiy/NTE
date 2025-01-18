using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NTE.Editor.UI.Controlling;
using NTE.Core.Utils;

using UnityEngine;

namespace NTE.Editor.UI.FileTree
{
    public class FileTree : MonoBehaviour
    {
        public TreeItem RootItem;

        public FileSystemWatcher Watcher;

        public string SelectedIndex
        {
            get => m_SelectedIndex;
            set
            {
                if (!string.IsNullOrEmpty(m_SelectedIndex))
                    Get(m_SelectedIndex).Deselect();
                m_SelectedIndex = value;
                Get(value).Select();
            }
        }
        [SerializeField] private string m_SelectedIndex;

        public string Root
        {
            get => m_Root;
            set
            {
                m_Root = value;
                Watcher?.Dispose();
                Watcher = new(value)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };
                Watcher.Created += OnFileChanged;
                Watcher.Deleted += OnFileChanged;
                Watcher.Renamed += OnFileChanged;
                RootItem.Initialize(value);
                Initialize(value);
            }
        }
        private string m_Root;

        public TreeItem Selection => Get(SelectedIndex);

        public TreeItem Get(string rpath)
        {
            if (string.IsNullOrEmpty(rpath))
                return RootItem;

            string[] s = rpath.Split('\\', '/');
            TreeItem ptr = RootItem;
            foreach (string p in s)
            {
                if (ptr.Items.TryGetValue(p, out TreeItem i))
                    ptr = i;
            }
            return ptr;
        }

        private void Start()
        {
            Root = "H:\\ProjectData";
        }

        private readonly List<string> InitRelativePath = new();
        private void Initialize(string path)
        {
            string[] entries = Directory.GetFileSystemEntries(path);
            foreach (string entry in entries)
            {
                TreeItem i = RootItem;
                foreach (string s in InitRelativePath)
                    i = i.Items[s];
                i.AddChild(entry);
                InitRelativePath.Add(Path.GetFileName(entry));
                if (Directory.Exists(entry))
                    Initialize(entry);
                InitRelativePath.Remove(Path.GetFileName(entry));
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
                WatcherDispatch(UpdateDelete, e);
            if (e.ChangeType == WatcherChangeTypes.Created)
                WatcherDispatch(UpdateCreate, e);
            if (e.ChangeType == WatcherChangeTypes.Renamed)
                WatcherDispatch(UpdateRename, e);
        }

        private void UpdateDelete(int id)
        {
            string rel = WatcherThreadDispatchedEventArgs[id].FullPath.GetRelativePathOf(Root);
            Get(rel).OnDeleted();
        }
        private void UpdateCreate(int id)
        {
            string[] rel = WatcherThreadDispatchedEventArgs[id].FullPath.GetRelativePathOf(Root).Split('/', '\\');
            TreeItem t = RootItem;
            for (int i = 0; i < rel.Length - 1; i++)
                t = t.Items[rel[i]];
            t.AddChild(WatcherThreadDispatchedEventArgs[id].FullPath);
        }
        private void UpdateRename(int id)
        {
            string rel = WatcherThreadDispatchedEventArgs[id].FullPath.GetRelativePathOf(Root);
            Get(rel).Name = WatcherThreadDispatchedEventArgs[id].Name;
        }

        public void WatcherDispatch(Action<int> act, FileSystemEventArgs e)
        {
            WatcherThreadDispatchedActions.Add(act);
            WatcherThreadDispatchedEventArgs.Add(e);
        }

        private void Update()
        {
            if (WatcherThreadDispatchedActions != null)
            {
                for (int i = 0; i < WatcherThreadDispatchedActions.Count; i++)
                    WatcherThreadDispatchedActions[i](i);
                WatcherThreadDispatchedEventArgs.Clear();
                WatcherThreadDispatchedActions.Clear();
            }
        }

        private readonly List<Action<int>> WatcherThreadDispatchedActions = new();
        private readonly List<FileSystemEventArgs> WatcherThreadDispatchedEventArgs = new();

        #region 右键菜单
        public RightMenuConfig FolderConfig;

        private void Awake()
        {
            FolderConfig = new(new()
            {
                new RightMenuButton("新建文件", () => {}),
                new RightMenuButton("新建文件夹", () => {}),
                new RightMenuButton("在资源管理器中打开", OpenInExplorer),
                new RightMenuButton("在资源管理器中选择", SelectInExplorer),
                new RightMenuSeparator(),
                new RightMenuButton("复制", () => {}),
                new RightMenuButton("剪切", () => { }),
                new RightMenuButton("粘贴", () => {}),
                new RightMenuSeparator(),
                new RightMenuButton("重命名", () => {}),
                new RightMenuButton("删除", Delete),
            });
        }

        private void Delete()
        {
            Selection.Delete();
        }

        private void OpenInExplorer()
        {
            Process.Start("explorer.exe", Selection.Information.FullName);
        }
        private void SelectInExplorer()
        {
            Process.Start("explorer.exe", $"/select,{Selection.Information.FullName}");
        }
        #endregion
    }
}
