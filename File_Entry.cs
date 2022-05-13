using System;
using System.Collections.Generic;
using System.Text;

namespace project_OS
{
    class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte f_attr, int f_firstCluster, int f_size, string f_content, Directory pa) : base (name, f_attr, f_firstCluster, f_size)
        {
            content = f_content;
            if (pa != null)
                parent = pa;
        }

        public void writeFileContent()
        {
            byte[] contentBYTES = Encoding.ASCII.GetBytes(content);

            double numOfBlocks = contentBYTES.Length / 1024;

            int numOfRequiredBlock = Convert.ToInt32(Math.Ceiling(numOfBlocks));

            int numOfFullSizeBlock = Convert.ToInt32(Math.Floor(numOfBlocks));

            double reminder = contentBYTES.Length % 1024;

            int fatIndex = 0;
            int lastIndex = -1;

            if (numOfRequiredBlock <= Fat_Table.GetAvilaibleBlocks())
            {
                if (fileFirstCluster != 0)
                {
                    fatIndex = fileFirstCluster;
                }
                else
                {
                    fileFirstCluster = Fat_Table.Getavaliableblock();
                    fatIndex = Fat_Table.Getavaliableblock();
                }
            }

            for (int i = 0; i < numOfFullSizeBlock; i++)
            {
                Virsual_Disk.writeBlock(contentBYTES, fatIndex);
                Fat_Table.set_Next(fatIndex, -1);
                if (lastIndex != -1)
                {
                    lastIndex = fatIndex;
                    Fat_Table.set_Next(lastIndex, fatIndex);
                }
                fatIndex = Fat_Table.Getavaliableblock();
                Fat_Table.Write_Fat_Table();

            }

        }

        List<byte> ls;

        public void readFileContent()
        {

            int fatIndex = fileFirstCluster;

            int next = Fat_Table.get_Next(fatIndex);

            if (fileFirstCluster != 0)
            {
                do
                {
                    ls.AddRange(Virsual_Disk.readBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = Fat_Table.get_Next(fatIndex);
                    }
                } while (next != -1);

            }
        }

        public void deleteFileContent()
        {
            if (fileFirstCluster != 0)
            {
                int index = fileFirstCluster;
                int next = -1;
                do
                {
                    Fat_Table.set_Next(index, 0);
                    next = index;

                    if (index != -1)
                        index = Fat_Table.get_Next(index);

                } while (next != -1);
            }
            if (parent != null)
            {
                parent.readDirectory();
                int Index = parent.searchDirectory(fileorDirName.ToString());
                if (Index != -1)
                {
                    parent.directoryTable.RemoveAt(Index);
                    parent.Write_Directory();
                }
            }
        }

    }
}
