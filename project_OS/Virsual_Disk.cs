using System.IO;

namespace project_OS
{
    class Virtual_Disk
    {
        public static FileStream Disk;
        public static void CREATE_Disk(string path)
        {
            Disk = new FileStream(@"C:\Users\fady\Desktop\fady.txt", FileMode.Create, FileAccess.ReadWrite);
            // Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            for (int i = 0; i < 1024; i++)
                Disk.WriteByte(0);
            for (int i = 0; i < 4 * 1024; i++)
                Disk.WriteByte((byte)'*');
            for (int i = 0; i < 1019 * 1024; i++)
                Disk.WriteByte((byte)'#');
            Disk.Flush();
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


                Fat_Table.initialize();
                Directory root = new Directory(@"K:\", 0x10, 5, 0, null);
                root.Write_Directory();
                Program.currentDirectory = root;

            }
            else
            {
                Fat_Table.get_fat_table();
                Directory root = new Directory(@"K:\", 0x10, 5, 0, null);
                root.readDirectory();
                Program.currentDirectory = root;
            }

        }
        public static void writeBlock(byte[] data, int Index)
        {
            Disk = new FileStream(@"C:\Users\fady\Desktop\fady.txt", FileMode.Open, FileAccess.Write);
            Disk.Seek(Index * 1024, SeekOrigin.Begin);
            Disk.Write(data, 0, 1024);
            Disk.Flush();
            Disk.Close();
        }
        public static byte[] readBlock(int clusterIndex)
        {
            Disk = new FileStream(@"C:\Users\fady\Desktop\fady.txt", FileMode.Open, FileAccess.Read);
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            Disk.Close();
            return bytes;
        }
    }
}
