using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace project_OS
{
    class Virsual_Disk
    {
        public static FileStream Disk;

        public static void CREATE_Disk(string path)
        {
            Disk = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            Disk.Close();
        }

        public static int getFreeSpace()
        {
            return (1024 * 1024) - (int)Disk.Length;
        }

        public static void initalize(string path)
        {
            if (!File.Exists(path))
            {
                CREATE_Disk(path);
                byte[] b = new byte[1024];
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = 0;
                }

                writeBlock(b, 0);
                Fat_Table f = new Fat_Table();
                Fat_Table.initialize();
                Directory root = new Directory("K:\\>", 0x10, 5, 0, null);
                root.Write_Directory();
                Fat_Table.set_Next(5, -1);
                Program.currentDirectory = root;
                Fat_Table.Write_Fat_Table();
            }
            else
            {

                Fat_Table.get_fat_table();

                Directory root = new Directory("K:\\>", 0x10, 5, 0, null);

                root.readDirectory();

                Program.currentDirectory = root;

            }




        }

        public static void writeBlock(byte[] data, int Index, int offset = 0, int count = 1024)
        {
            Disk = new FileStream("File.txt", FileMode.Open, FileAccess.Write);
            Disk.Seek(Index * 1024, SeekOrigin.Begin);
            Disk.Write(data, offset, count);
            Disk.Flush();
            Disk.Close();
        }

        public static byte[] readBlock(int clusterIndex)
        {
            Disk = new FileStream("File.txt", FileMode.Open, FileAccess.Read);
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            Disk.Close();
            return bytes;
        }

    }
}
