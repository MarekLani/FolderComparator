using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FolderComparator
{
    class Program
    {
        static List<FolderOrFileItem> NewDirectoryTree = new List<FolderOrFileItem>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            NewDirectoryTree = DirSearch("C:\\Users\\mlani\\Desktop\\FolderCheck", "C:\\Users\\mlani\\Desktop\\FolderCheckOld");

            Console.ReadLine();
        }


        static List<FolderOrFileItem> DirSearch(string newDir, string oldDir)
        {
            try
            {
                List<FolderOrFileItem> dirItems = new List<FolderOrFileItem>();

                var newFiles = GetFileNames(Directory.GetFiles(newDir));
                var oldFiles = GetFileNames(Directory.GetFiles(oldDir));

                Console.WriteLine("New Files");
                foreach (var file in newFiles.Where(ff => !oldFiles.Contains(ff)))
                {
                    dirItems.Add(new FileItem(file, FileChangeType.New));
                    Console.WriteLine(file);
                }

                Console.WriteLine("Deleted Files");
                foreach (var file in oldFiles.Where(ff => !newFiles.Contains(ff)))
                {
                    dirItems.Add(new FileItem(file, FileChangeType.Deleted));
                    Console.WriteLine(file);
                }

                Console.WriteLine("ChangedFiles");
                foreach(var file in newFiles.Where(ff => oldFiles.Contains(ff)))
                {
                    if(!File.ReadAllBytes(Path.Combine(newDir, file)).SequenceEqual(File.ReadAllBytes(Path.Combine(oldDir, file))))
                    {
                        dirItems.Add(new FileItem(file, FileChangeType.Edited));
                        Console.WriteLine(file);
                    }
                    else
                    {
                        //Unchanged
                        dirItems.Add(new FileItem(file, FileChangeType.Unchanged));
                    }

                }

                var newDirs = GetFileNames(Directory.GetDirectories(newDir));
                var oldDirs = GetFileNames(Directory.GetDirectories(oldDir));

                Console.WriteLine("Deleted Directories");
                foreach (var dir in oldDirs.Where(d => !newDirs.Contains(d)))
                {
                    //Deleted Directories
                    Console.WriteLine(dir);
                    dirItems.Add(new FolderItem(dir, FileChangeType.Deleted));
                }

                Console.WriteLine("New  Directories");
                foreach (var dir in newDirs.Where(d => !oldDirs.Contains(d)))
                {
                    //New Directories
                    Console.WriteLine(dir);
                    var d = new FolderItem(dir, FileChangeType.New);

                    //We need only simple traverse as everything inside new dir is new
                    d.ChildItems = SimpleDirTraverse(Path.Combine(newDir, dir));
                    dirItems.Add(d);
                }

                //Directories existing in both root dirs need traversal with comparison
                foreach (var dir in newDirs.Where(d => oldDirs.Contains(d)))
                {
                    //Recursion
                    var d = new FolderItem(dir, FileChangeType.Deleted);
                    d.ChildItems = DirSearch(Path.Combine(newDir, dir), Path.Combine(oldDir, dir));
                    dirItems.Add(d);
                }
                return dirItems;
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
                return null;
            }
        }


        static List<FolderOrFileItem> SimpleDirTraverse(string dir)
        {
            try
            {
                List<FolderOrFileItem> dirItems = new List<FolderOrFileItem>();

                var newFiles = GetFileNames(Directory.GetFiles(dir));

                Console.WriteLine("New Files");
                foreach (var file in newFiles)
                {
                    dirItems.Add(new FileItem(file, FileChangeType.New));
                    Console.WriteLine(file);
                }

                var newDirs = GetFileNames(Directory.GetDirectories(dir));

                Console.WriteLine("New Directories");
                foreach (var d in newDirs)
                {
                    //New Directories
                    Console.WriteLine(d);
                    var dd = new FolderItem(dir, FileChangeType.New);
                    dd.ChildItems = SimpleDirTraverse(Path.Combine(dir,d));
                    dirItems.Add(dd);
                }

                return dirItems;
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
                return null;
            }
        }

        static byte[] ComputeMDHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        static IEnumerable<string> GetFileNames(string[] filePaths)
        {
            foreach (var item in filePaths)
                yield return Path.GetFileName(item); 

        }
    }
}
