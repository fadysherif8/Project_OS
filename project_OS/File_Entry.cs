using System;
using System.Collections.Generic;

namespace project_OS
{
    class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte file_attr, int file_firstCluster, int file_size, string file_content, Directory pa) : base(name, file_attr, file_firstCluster, file_size)
        {
            content = file_content;
            if (pa != null)
                parent = pa;

        }

        public void writeFileContent()
        {
            int lastIndex = -1;
            byte[] byteContent = new byte[this.content.Length];
            for (int i = 0; i < this.content.Length; i++)
            {
                byteContent[i] = Convert.ToByte(content[i]);
            }
            double requiredBlocks = (byteContent.Length / 1024.0);
            requiredBlocks = Math.Ceiling(requiredBlocks);
            int Reminder = (byteContent.Length % 1024);
            int fatIndex;
            if (Fat_Table.GetAvilaibleBlocks() >= requiredBlocks)
            {
                if (this.fileFirstCluster != 0)
                {
                    fatIndex = this.fileFirstCluster;
                }
                else
                {
                    fatIndex = Fat_Table.Getavaliableblock();
                    this.fileFirstCluster = fatIndex;
                }
                List<byte[]> Blocks = new List<byte[]>();
                byte[] block = new byte[1024];
                for (int j = 0; j < requiredBlocks * 1024; j++)
                {
                    if (j % 1023 == 0 && j != 0)
                    {
                        Blocks.Add(block);
                    }
                    if (j == byteContent.Length)
                    {
                        break;
                    }
                    block[j % 1024] = byteContent[j];
                }
                for (int i = 0; i < requiredBlocks; i++)
                {
                    if (i == requiredBlocks - 1)
                    {
                        byte[] lblock = new byte[1024];
                        for (int x = 0; x < content.Length % 1024; x++)
                        {
                            lblock[x] = byteContent[i * 1024 + x];
                        }
                        Blocks.Add(lblock);
                    }
                    Virtual_Disk.writeBlock(Blocks[i], fatIndex);
                    Fat_Table.set_Next(fatIndex, -1);
                    if (lastIndex != -1)
                    {
                        Fat_Table.set_Next(lastIndex, fatIndex);
                    }
                    lastIndex = fatIndex;
                    fatIndex = Fat_Table.Getavaliableblock();
                }
                Fat_Table.Write_Fat_Table();
            }
        }
        public void readFileContent()
        {
            List<byte> tableBytes = new List<byte>();
            int fatIndex;
            int nextIndex;
            if (fileFirstCluster != 0)
            {
                fatIndex = fileFirstCluster;
                nextIndex = Fat_Table.get_Next(fatIndex);
                do
                {
                    tableBytes.AddRange(Virtual_Disk.readBlock(fatIndex));
                    fatIndex = nextIndex;
                    if (fatIndex != -1)
                        nextIndex = Fat_Table.get_Next(fatIndex);
                }
                while (nextIndex != -1);
                byte[] byteContent = new byte[tableBytes.Count];
                for (int i = 0; i < tableBytes.Count; i++)
                {
                    char c = Convert.ToChar(tableBytes[i]);
                    if (c != '\0')
                    {
                        this.content += c;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public void deleteFile()
        {
            if (this.fileFirstCluster != 0)
            {
                int index = this.fileFirstCluster;
                int next = Fat_Table.get_Next(index);
                do
                {
                    Fat_Table.set_Next(index, 0);
                    index = next;
                    if (index != -1)
                        next = Fat_Table.get_Next(index);
                }
                while (index != -1);
            }
            if (this.parent != null)
            {
                parent.readDirectory();
                int index = this.parent.searchDirectory(new string(this.fileorDirName));
                if (index != -1)
                {
                    parent.directoryTable.RemoveAt(index);
                    this.parent.Write_Directory();
                    Fat_Table.Write_Fat_Table();
                }
            }
        }

    }


}
