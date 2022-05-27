using System;

namespace project_OS
{
    class Program
    {
        public static Directory currentDirectory;
        public static string currentPath;

        static void Main(string[] args)
        {
            Virtual_Disk.initalize(@"C:\Users\fady\Desktop\fady.txt");
            currentPath = new string(currentDirectory.fileorDirName);
            while (true)
            {
                Console.Write(currentPath.Trim());
                string inputuser = Console.ReadLine().ToLower();
                if (!inputuser.Contains(" "))
                {
                    if (inputuser == "help")
                    {
                        cmd.help();
                    }
                    else if (inputuser == "quit")
                    {
                        cmd.quit();
                    }
                    else if (inputuser == "cls")
                    {
                        cmd.cls();
                    }
                    else if (inputuser == "cd")
                    {
                        if (currentDirectory.parent != null)
                        {
                            currentDirectory = currentDirectory.parent;
                            currentPath = new string(currentDirectory.fileorDirName);
                        }
                        currentPath = new string(currentDirectory.fileorDirName);
                    }
                    else if (inputuser == "md")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "rd")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "dir")
                    {
                        cmd.dir();
                    }
                    else if (inputuser == "import")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "type")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "export")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "del")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "copy")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (inputuser == "rename")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else
                    {
                        Console.WriteLine("Wrong command");
                    }

                }
                else if (inputuser.Contains(" "))
                {
                    string[] arrInput = inputuser.Split();
                    if (arrInput[0] == "md")
                    {
                        cmd.md(arrInput[1]);
                    }
                    else if (arrInput[0] == "rd")
                    {
                        cmd.rd(arrInput[1]);
                    }
                    else if (arrInput[0] == "cd")
                    {
                        cmd.cd(arrInput[1]);
                    }
                    else if (arrInput[0] == "import")
                    {
                        cmd.import(arrInput[1]);
                    }
                    else if (arrInput[0] == "rename")
                    {
                        cmd.rename(arrInput[1], arrInput[2]);
                    }
                    else if (arrInput[0] == "type")
                    {
                        cmd.type(arrInput[1]);
                    }
                    else if (arrInput[0] == "export")
                    {
                        cmd.export(arrInput[1], arrInput[2]);
                    }
                    else if (arrInput[0] == "del")
                    {
                        cmd.del(arrInput[1]);
                    }
                    else if (arrInput[0] == "copy")
                    {
                        cmd.copy(arrInput[1], arrInput[2]);
                    }
                }

            }
        }
    }


}
