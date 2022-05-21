using System;
using System.IO;

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
            Console.WriteLine("cls\t\t Clear the screen");
            Console.WriteLine("quit\t\t Quits the CMD.exe program");
            Console.WriteLine("help\t\t Provides help information for windows command");
            Console.WriteLine("md\t\t Creates a directory");
            Console.WriteLine("cd\t\t Display the name of or change the current directory");
            Console.WriteLine("dir\t\t List the contents of directory");
            Console.WriteLine("del\t\t delete the file from the directory");
            Console.WriteLine("copy\t\t Copies one or more files to another location​");
            Console.WriteLine("rd\t\t Removes a directory.");
            Console.WriteLine("type\t\t Displays the contents of a text file");
            Console.WriteLine("rename\t\t Renames a file");
            Console.WriteLine("import\t\t import text file(s) from your computer​");
            Console.WriteLine("export\t\t export text file(s) to your computer​​");

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
                Directory dir = new Directory(name, 0x10, firstCluster, 0, Program.currentDirectory);
                Program.currentPath = new string(Program.currentDirectory.fileorDirName).Trim() + new string(dir.fileorDirName).Trim() + "\\";
                Program.currentDirectory.Write_Directory();
                Program.currentDirectory.readDirectory();
                Program.currentDirectory = dir;
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
                if (Program.currentDirectory.directoryTable[i].filaAttribute == 0x0)
                {
                    Console.WriteLine("\t\t" + Program.currentDirectory.directoryTable[i].fileSize + "\t" + new string(Program.currentDirectory.directoryTable[i].fileorDirName));
                    counterfiles++;
                    filesizecounter += Program.currentDirectory.directoryTable[i].fileSize;
                }
                else
                {
                    Console.Write("<DIR>" + "\t");
                    Console.WriteLine(Program.currentDirectory.directoryTable[i].fileorDirName);
                    counterDirectory++;
                }
            }
            Console.WriteLine("\t\t" + counterfiles + " File(s)       " + filesizecounter + " bytes");
            Console.WriteLine("\t\t" + counterDirectory + " Dir(s)      " + Fat_Table.Get_free_space() + "  bytes Free");
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
                    File_Entry f1 = new File_Entry(name, 0x0, firstCluster, size, content, Program.currentDirectory);
                    f1.writeFileContent();

                    Directory_Entry d1 = new Directory_Entry(name, 0x0, firstCluster, size);
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
                if (Program.currentDirectory.directoryTable[index].filaAttribute == 0x0)
                {
                    int FirstCluster = Program.currentDirectory.directoryTable[index].fileFirstCluster;
                    int FileSize = Program.currentDirectory.directoryTable[index].fileSize;
                    string Content = string.Empty;
                    File_Entry file = new File_Entry(name, 0x0, FirstCluster, FileSize, Content, Program.currentDirectory);
                    file.readFileContent();
                    Console.WriteLine(file.content);
                }
            }
            else
            {
                Console.WriteLine("The system can't find the file ");
            }
        }
        public static int export(string source, string destination)
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
            return 0;
        }
        public static void rename(string oldname, string newname)
        {
            int index = Program.currentDirectory.searchDirectory(oldname);
            if (index != -1)
            {
                int n = Program.currentDirectory.searchDirectory(newname);
                if (n == -1)
                {

                    Directory_Entry f = Program.currentDirectory.directoryTable[index];
                    f.fileorDirName = newname.ToCharArray();
                    Program.currentDirectory.directoryTable.RemoveAt(index);
                    Program.currentDirectory.directoryTable.Insert(n + 1, f);

                }
                else
                {
                    Console.WriteLine("dublicate file name");
                }

            }
            else
            {
                Console.WriteLine("system cannot find the file specified");
            }

        }
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
                    File_Entry d = new File_Entry(name, 0x0, first_cluster, file_size, null, Program.currentDirectory);
                    d.deleteFile();
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
        public static void copy(string source, string destination)
        {
            {
                string Name = " ";
                if (File.Exists(source))
                {
                    char[] separators = new char[2];
                    separators[0] = '\\';
                    separators[1] = ':';
                    string[] nam = source.Split(separators);
                    Name = nam[nam.Length - 1];

                    string Content = " ";
                    int index = Program.currentDirectory.searchDirectory(Name);
                    if (index == -1)
                    {
                        Content += File.ReadAllText(source);
                        int size = Content.Length;
                        int firstCluster = 0;
                        if (size > 0)
                        {
                            firstCluster = Fat_Table.Getavaliableblock();
                        }

                        File_Entry file = new File_Entry(Name, 0x0, firstCluster, size, Content, Program.currentDirectory);
                        file.writeFileContent();
                        Directory_Entry fil = new Directory_Entry(Name, 0x0, firstCluster, size);
                        Program.currentDirectory.directoryTable.Add(fil);


                        if (Program.currentDirectory.parent != null)
                        {
                            Program.currentDirectory.parent.updateContent(Program.currentDirectory.parent);
                            Program.currentDirectory.parent.Write_Directory();
                        }
                        Console.WriteLine("\t" + Name + " file is been sorted");
                        Program.currentDirectory.Write_Directory();

                    }
                    else
                    {
                        if (Program.currentDirectory.directoryTable[index].filaAttribute == 0x10)
                        {
                            Console.WriteLine(Name + " is already exists as folder name Pleas " + "from name Or rename The folder");
                        }
                    }
                }
                else if (!File.Exists(source))
                {
                    Console.WriteLine("Path isn't correct");
                }
                int n = export(Name, destination);
                if (n == 1)
                {
                    Console.WriteLine("Succeed");

                }
                else Console.WriteLine("failed");
            }
        }
    }

}
