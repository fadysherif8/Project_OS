using System;
using System.Collections.Generic;
using System.Text;

namespace project_OS
{
    class Directory_Entry
    {
        public char[] fileorDirName = new char[11];
        public byte filaAttribute;
        public byte[] fileEmpty = new byte[12];
        public int fileSize;
        public int fileFirstCluster;

        public Directory_Entry(string name, byte attribute, int firstCluster, int fsize)
        {

            filaAttribute = attribute;

            if (filaAttribute == 0x0)
            {
                string[] filename = name.Split('.');
                assignFileName(filename[0].ToCharArray(), filename[1].ToCharArray());
            }
            else
            {
                assignDIRName(name.ToCharArray());
            }

            fileFirstCluster = firstCluster;
            fileSize = fsize;
        }

        public void assignFileName(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.fileorDirName[i] = name[i];
                }
                j++;
                this.fileorDirName[j] = '.';
                for (int i = 0; i < extension.Length; i++)
                {
                    j++;
                    this.fileorDirName[j] = extension[i];
                }
                for (int i = ++j; i < fileorDirName.Length; i++)
                {
                    this.fileorDirName[i] = ' ';
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.fileorDirName[i] = name[i];
                }
                this.fileorDirName[7] = '.';
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    this.fileorDirName[j] = extension[i];
                }
            }
        }

        public void assignDIRName(char[] name)
        {
            if (name.Length <= 11)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.fileorDirName[i] = name[i];
                }
                for (int i = ++j; i < fileorDirName.Length; i++)
                {
                    this.fileorDirName[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 11; i++)
                {
                    j++;
                    this.fileorDirName[i] = name[i];
                }
            }
        }

        public byte[] GetBytes()
        {

            byte[] b = new byte[32];

            for (int i = 0; i < 11; i++)
            {
                b[i] = (byte)fileorDirName[i];
            }

            b[11] = filaAttribute;

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                b[i] = fileEmpty[j];
            }

            for (int i = 24; i < 28; i++)
            {
                b[i] = (byte)fileFirstCluster;
            }

            for (int i = 28; i < 32; i++)
            {
                b[i] = (byte)fileSize;
            }

            return b;
        }

        public Directory_Entry GetDirectoryEntry(byte[] b)
        {

            for (int i = 0; i < 11; i++)
            {
                fileorDirName[i] = (char)b[i];
            }

            filaAttribute = b[11];

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                fileEmpty[j] = b[i];
            }

            for (int i = 24; i < 28; i++)
            {
                fileFirstCluster = b[i];
            }

            for (int i = 28; i < 32; i++)
            {
                fileSize = b[i];
            }

            Directory_Entry d1 = new Directory_Entry(new string(fileorDirName), filaAttribute, fileFirstCluster, fileSize);
            return d1;
        }

    }
}
