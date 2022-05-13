using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace project_OS
{
    class cmd
    {
        public static void cls()
        {
            Console.Clear();
        }

        public static void help()
        {
            Console.WriteLine("CD                     Change the current default directory ");
            Console.WriteLine("CLS                    Clear the screen ");
            Console.WriteLine("DIR                    List the contents of directory  ");
            Console.WriteLine("QUIT                   Quit the shell ");
            Console.WriteLine("COPY                   Copies one or more files to another location ");
            Console.WriteLine("DEL                    Deletes one or more files ");
            Console.WriteLine("HELP                   Provides Help information for commands");
            Console.WriteLine("MD                     Creates a directory ");
            Console.WriteLine("RD                     Removes a directory");
            Console.WriteLine("RENAME                 Renames a file");
            Console.WriteLine("TYPE                   Displays the contents of a text file ");
            Console.WriteLine("IMPORT                 import text file(s) from your computer");
            Console.WriteLine("EXPORT                 export text file(s) to your computer");
        }

        public static void quit()
        {
            System.Environment.Exit(1);
        }

        public static void md(string name)
        {
            if (Program.currentDirectory.searchDirectory(name) == -1)
            {
                Directory_Entry newdirectory = new Directory_Entry(name, 0x10, 0, 0);
                Program.currentDirectory.directoryTable.Add(newdirectory);
                Program.currentDirectory.Write_Directory();
                if (Program.currentDirectory.parent != null)
                {
                    Program.currentDirectory.parent.updateContent(Program.currentDirectory.parent);
                    Program.currentDirectory.parent.Write_Directory();
                }
            }
            else
            {
                Console.WriteLine("A subdirectory or file " + name + " already exists.");
            }
        }

        public static void rd(string name)
        {
            int index = Program.currentDirectory.searchDirectory(name);
            if (index != -1)
            {
                int firstCluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                Directory d1 = new Directory(name, 0x10, firstCluster, 0, Program.currentDirectory);
                d1.deleteDirectory();
                Program.currentPath = new string(Program.currentDirectory.fileorDirName).Trim();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }

        public static void cd(string name)
        {
            int index = Program.currentDirectory.searchDirectory(name);

            if (index != -1)
            {
                int firstCluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                Directory d1 = new Directory(name, 0x10, firstCluster, 0, Program.currentDirectory);
                Program.currentPath = new string(Program.currentDirectory.fileorDirName).Trim() + "\\" + new string(d1.fileorDirName).Trim();
                Program.currentDirectory.readDirectory();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void dir()
        {
            int counterDirectory = 0, counterfiles = 0, filesizecounter = 0;
            Console.WriteLine("Directory of " + Program.currentPath);
            for (int i = 0; i < Program.currentDirectory.directoryTable.Count; i++)
            {
                if (Program.currentDirectory.filaAttribute == 0x0)
                {
                    Console.WriteLine(Program.currentDirectory.directoryTable[i].fileSize + "  " + Program.currentDirectory.directoryTable[i].fileorDirName);
                    counterfiles++;
                    filesizecounter += Program.currentDirectory.directoryTable[i].fileSize;
                }
                else
                {
                    Console.Write("<dir>" + "      ");
                    Console.WriteLine(Program.currentDirectory.directoryTable[i].fileorDirName);
                    counterDirectory++;
                }
            }
            Console.WriteLine(counterfiles + " File(s)       " + filesizecounter + " bytes");
            Console.WriteLine(counterDirectory + " Dir(s)   " + Fat_Table.Get_free_space() + "  bytes Free");
        }
        public static void import(string path)
        {
            if (File.Exists(path))
            {
                int start_name = path.LastIndexOf("\\");
                string name = path.Substring(start_name + 1);
                string content = File.ReadAllText(path);
                int size = content.Length;
                int index = Program.currentDirectory.searchDirectory(name);
                if (index == -1)
                {
                    int firstCluster;
                    if (size > 0)
                    {

                        firstCluster = Fat_Table.Getavaliableblock();
                    }
                    else
                    {
                        firstCluster = 0;
                    }
                    File_Entry f1 = new File_Entry(name, 0x0, firstCluster, 0, content, Program.currentDirectory);
                    f1.writeFileContent();
                    Directory_Entry d1 = new Directory_Entry(name, 0x0, firstCluster, 0);
                    Program.currentDirectory.directoryTable.Add(d1);
                    Program.currentDirectory.Write_Directory();
                }
                else
                {
                    Console.WriteLine("This file already exist");
                }
            }
            else
            {
                Console.WriteLine("This file is not exist");
            }
        }
        public static void type(string name)
        {
            int index = Program.currentDirectory.searchDirectory(name);
            if (index != -1)
            {
                int first_cluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                int filesize = Program.currentDirectory.directoryTable[index].fileSize;
                string content = null;
                File_Entry f = new File_Entry(name, 0x0, first_cluster, filesize, content, Program.currentDirectory);

                f.readFileContent();
                Console.WriteLine(f.content);
            }
            else
            {
                Console.WriteLine("The system can't find the file ");
            }
        }
        public static void export(string source, string destination)
        {
            int index = Program.currentDirectory.searchDirectory(source);
            if (index != -1)
            {
                if (System.IO.Directory.Exists(destination))
                {
                    int first_cluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                    int filesize = Program.currentDirectory.directoryTable[index].fileSize;
                    string temp = null;
                    File_Entry f = new File_Entry(source, 0x0, first_cluster, filesize, temp, Program.currentDirectory);
                    f.readFileContent();

                    StreamWriter sw = new StreamWriter(destination + "\\" + source);
                    sw.Write(f.content);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    Console.WriteLine("the system can't find this path in computer Disk");
                }
            }
            else
            {
                Console.WriteLine("This file doesn't exist");
            }
        }
        /*public static void rename(string oldname, string newname)
        {
            int index = Program.currentDirectory.searchDirectory(oldname);
            if (index != -1)
            {
                int n = Program.currentDirectory.searchDirectory(newname);
                if (n != -1)
                {

                    Directory_Entry d = new Dirctory_Entry();
                    d = Program.currentDirectory.directoryTable[index];
                    d.file_name = newname;
                    Program.currentDirectory.directoryTable.RemoveAt(index);
                    Program.currentDirectory.directoryTable.Insert(n, d);
                    Program.currentDirectory.Write_Directory();
                }
                else
                {
                    Console.WriteLine("A duplicate file name exists, or the file cannot be found.");
                }

            }
            else
            {
                Console.WriteLine(" The system cannot find the file specified.");
            }

        }*/
        public static void del(string name)
        {
            int index = Program.currentDirectory.searchDirectory(name);
            if (index != -1)
            {
                int f = Program.currentDirectory.directoryTable[index].filaAttribute;
                if (f == 0x0)
                {
                    int first_cluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                    int file_size = Program.currentDirectory.directoryTable[index].fileSize;
                    File_Entry d = new File_Entry(name, 0x0, first_cluster, 0, null, Program.currentDirectory);
                    d.deleteFileContent();
                }
                else
                {
                    Console.WriteLine(" The system cannot find the file specified. ");
                }
            }
            else
            {
                Console.WriteLine(" The system cannot find the file specified. ");
            }
        }
        public static void copy(string num1, string num2)
        {
            int index1 = Program.currentDirectory.searchDirectory(num1);
            if (index1 != -1)
            {
                int start_index = num2.LastIndexOf("\\");
                string name = num2.Substring(start_index + 1);

                int index_destenation = Program.currentDirectory.searchDirectory(name);
                if (index_destenation == -1)
                {

                    if (num2 != Program.currentDirectory.fileorDirName.ToString())
                    {
                        int firstcluster = Program.currentDirectory.directoryTable[index1].fileFirstCluster;
                        int f_size = Program.currentDirectory.directoryTable[index1].fileSize;
                        Directory_Entry entry = new Directory_Entry(num1.ToString(), 0x0, firstcluster, f_size);
                        Directory dir = new Directory(num2.ToString(), 0x10, firstcluster, f_size, Program.currentDirectory.parent);
                        dir.directoryTable.Add(entry);


                    }
                    else Console.WriteLine("not fff");

                }
            }
        }

    }
}
