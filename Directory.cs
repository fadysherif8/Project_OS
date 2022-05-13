using System;
using System.Collections.Generic;
using System.Text;

namespace project_OS
{
    class Directory : Directory_Entry
    {
        public List<Directory_Entry> directoryTable;
        public Directory parent = null;
        public Directory(string namefile, byte attributefile, int firstClusturfile, int sizefile, Directory p)
                    : base
                    (namefile,
                     attributefile,
                     firstClusturfile,
                     sizefile)
        {
            directoryTable = new List<Directory_Entry>();
            if (p != null)
            {
                parent = p;
            }
        }

        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry me = new Directory_Entry(new string(fileorDirName), filaAttribute, fileFirstCluster, fileSize);
            return me;
        }

        public void Write_Directory()
        {
            int lastIndex = -1;
            byte[] DTB = new byte[32 * directoryTable.Count];
            byte[] DEB = new byte[32];
            for (int i = 0; i < directoryTable.Count; i++)
            {
                DEB = directoryTable[i].GetBytes();
                for (int j = i * 32, c = 0; c < 32; c++, j++)
                {
                    DTB[j] = DEB[c];
                }
            }
            double NFQB = (DTB.Length / 1024);
            NFQB = Math.Ceiling(NFQB);
            int Reminder = (DTB.Length % 1024);
            int fatIndex;
            if (NFQB <= Fat_Table.GetAvilaibleBlocks())
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
                for (int j = 0; j < NFQB * 1024; j++)
                {
                    if (j % 1024 == 0 && j != 0)
                    {
                        Blocks.Add(block);
                    }
                    block[j % 1024] = DTB[j];
                }
                for (int i = 0; i < NFQB; i++)
                {
                    Virsual_Disk.writeBlock(Blocks[i], fatIndex);
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



        public void readDirectory()
        {
            if (this.fileFirstCluster != 0)
            {
                int fatIndex = this.fileFirstCluster;

                int next = Fat_Table.get_Next(fatIndex);
                List<byte> ls = new List<byte>();
                List<Directory_Entry> dt = new List<Directory_Entry>();

                do
                {
                    ls.AddRange(Virsual_Disk.readBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = Fat_Table.get_Next(fatIndex);
                    }
                } while (next != -1);

                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                        break;
                    dt.Add(GetDirectoryEntry(b));
                }

            }



        }

        public int searchDirectory(string name)
        {
            if (name.Length < 11)
            {
                name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < directoryTable.Count; i++)
            {
                string n = new string(directoryTable[i].fileorDirName);
                if (n.Equals(name))
                    return i;
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
