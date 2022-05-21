using System;
using System.Collections.Generic;

namespace project_OS
{
    class Directory : Directory_Entry
    {
        public List<Directory_Entry> directoryTable;
        public Directory parent;
        public Directory(string namefile, byte attributefile, int firstClusturfile, int sizefile, Directory p)
                    : base
                    (namefile,
                     attributefile,
                     firstClusturfile,
                     sizefile)
        {
            if (p != null)
                parent = p;
            directoryTable = new List<Directory_Entry>();
        }

        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry me = new Directory_Entry(new string(fileorDirName), filaAttribute, fileFirstCluster, fileSize);
            return me;
        }

        public void Write_Directory()
        {

            byte[] DTB = new byte[32 * directoryTable.Count];
            byte[] DEB = new byte[32];
            double NFQB = Math.Ceiling(DTB.Length / 1024.0);
            int numFull = DTB.Length / 1024;
            int remainder = DTB.Length % 1024;
            for (int i = 0; i < directoryTable.Count; i++)
            {
                DEB = directoryTable[i].GetBytes();
                for (int j = i * 32, c = 0; c < 32; j++, c++)
                {
                    DTB[j] = DEB[c];
                }
            }
            if (NFQB <= Fat_Table.GetAvilaibleBlocks())
            {
                int fatIndex;
                int lastIndex = -1;
                if (fileFirstCluster != 0)
                {
                    fatIndex = fileFirstCluster;
                }
                else
                {
                    fatIndex = Fat_Table.Getavaliableblock();
                    fileFirstCluster = fatIndex;
                }

                List<byte[]> ls = new List<byte[]>();
                for (int i = 0; i < numFull; i++)
                {
                    byte[] block = new byte[1024];
                    for (int j = i * 1024, c = 0; c < 1024; j++, c++)
                    {

                        block[c] = DTB[j];

                    }
                    ls.Add(block);
                }
                if (remainder > 0)
                {
                    int start = numFull * 1024;
                    byte[] b = new byte[1024];
                    for (int i = start; i < (start + remainder); i++)
                    {
                        b[i % 1024] = DTB[i];

                    }
                    ls.Add(b);




                }
                for (int i = 0; i < ls.Count; i++)
                {
                    Virtual_Disk.writeBlock(ls[i], fatIndex);
                    Fat_Table.set_Next(fatIndex, -1);
                    if (lastIndex != -1)
                        Fat_Table.set_Next(lastIndex, fatIndex);
                    lastIndex = fatIndex;
                    fatIndex = Fat_Table.Getavaliableblock();
                }
                if (directoryTable.Count == 0)
                {
                    if (fileFirstCluster != 0)
                    {
                        Fat_Table.set_Next(fileFirstCluster, 0);
                        fileFirstCluster = 0;


                    }

                }

                Fat_Table.Write_Fat_Table();
            }
        }

        public void readDirectory()
        {
            directoryTable = new List<Directory_Entry>();
            List<byte> ls = new List<byte>();
            //  List<Directory_Entry> dt = new List<Directory_Entry>();
            int fatIndex;
            if (fileFirstCluster != 0 && Fat_Table.get_Next(fileFirstCluster) != 0)
            {
                fatIndex = fileFirstCluster;

                int next = Fat_Table.get_Next(fatIndex);


                do
                {
                    ls.AddRange(Virtual_Disk.readBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = Fat_Table.get_Next(fatIndex);
                    }
                } while (next != -1);
                byte[] b = new byte[32];
                for (int i = 0; i < ls.Count; i++)
                {
                    b[i % 32] = ls[i];
                    if ((i + 1) % 32 == 0)
                    {
                        if (b[0] != 0)
                            directoryTable.Add(GetDirectory_Entry(b));
                    }
                }
            }
            else
            {
                fatIndex = Fat_Table.Getavaliableblock();
                fileFirstCluster = fatIndex;
            }
        }
        public int searchDirectory(string name)
        {
            readDirectory();
            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                { name += " "; }
            }

            for (int i = 0; i < this.directoryTable.Count; i++)
            {
                string n = new string(this.directoryTable[i].fileorDirName);
                if (n == name)
                {
                    return i;

                }
            }
            return -1;
        }
        public void updateContent(Directory_Entry d)
        {
            string name = new string(d.fileorDirName);
            readDirectory();
            int index = searchDirectory(name);
            if (index != -1)
            {
                directoryTable.RemoveAt(index);
                directoryTable.Insert(index, d);
            }
            Write_Directory();
        }
        public void deleteDirectory()
        {
            if (this.fileFirstCluster != 0)
            {
                int index = this.fileFirstCluster;
                int next = -1;
                do
                {
                    Fat_Table.set_Next(index, 0);
                    next = index;

                    if (index != -1)
                        index = Fat_Table.get_Next(index);

                } while (next != -1);
            }
            if (this.parent != null)
            {
                parent.readDirectory();
                int Index = parent.searchDirectory(new string(fileorDirName));
                if (Index != -1)
                {
                    this.parent.directoryTable.RemoveAt(Index);
                    this.parent.Write_Directory();
                }
            }
        }
    }
}
