using System;
using System.Collections.Generic;
using System.Text;

namespace FolderComparator
{
    public class FolderOrFileItem
    {
        public string Name { get; set; }
        public FileChangeType FileChangeType { get; set; }

        public FolderOrFileItem(string name, FileChangeType changeType)
        {
            this.Name = name;
            this.FileChangeType = changeType;

            //We do not need to allocate memory for deleted directories, as we will not traverse subtree
            //if(initializeChildItems)
            //    ChildItems = new List<FolderOrFileItem>();

        }
    }

    public class FileItem : FolderOrFileItem
    {
        //public Byte[] Checksum { get; set; }
        public FileItem(string name, FileChangeType changeType) : base (name, changeType)
        {
        }
    }

    public class FolderItem : FolderOrFileItem
    {

        public FolderItem(string name, FileChangeType changeType) : base(name, changeType)
        {
        }

        public List<FolderOrFileItem> ChildItems { get; set; }

    }

    public enum FileChangeType
    {
        Deleted,
        Edited,
        Unchanged,
        New
    }
}
