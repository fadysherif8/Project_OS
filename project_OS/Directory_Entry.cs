using System;
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
            if (name.Length < 11)
            {
                for (int i = name.Length; i < 11; i++)
                    name += " ";

            }
            this.fileorDirName = name.ToCharArray();
            filaAttribute = attribute;
            fileFirstCluster = firstCluster;
            fileSize = fsize;
        }
        public Directory_Entry() { }





        public byte[] GetBytes()
        {

            byte[] b = new byte[32];
            byte[] name = new byte[11];
            name = Encoding.ASCII.GetBytes(fileorDirName);
            for (int i = 0; i < 11; i++)
            {
                if (i < name.Length)
                    b[i] = name[i];
                else
                    b[i] = (byte)' ';

            }
            b[11] = filaAttribute;

            for (int i = 0; i < 12; i++)
            {
                b[i + 12] = 0;
            }

            byte[] fc = new byte[4];
            fc = BitConverter.GetBytes(fileFirstCluster);
            for (int i = 0; i < 4; i++)
                b[i + 24] = fc[i];

            byte[] fs = new byte[4];
            fs = BitConverter.GetBytes(fileSize);
            for (int i = 0; i < 4; i++)
                b[i + 28] = fs[i];

            return b;

        }

        public Directory_Entry GetDirectory_Entry(byte[] b)
        {
            Directory_Entry de = new Directory_Entry();
            //file name
            for (int i = 0; i < 11; i++)
                de.fileorDirName[i] = (char)b[i];
            //file attrib
            de.filaAttribute = b[11];
            //file empty
            for (int i = 0; i < 12; i++)
                de.fileEmpty[i] = 0;
            //first cluster
            byte[] fc = new byte[4];
            for (int i = 24; i < 28; i++)
                fc[i % 24] = b[i];
            de.fileFirstCluster = BitConverter.ToInt32(fc, 0);
            //file size
            byte[] fs = new byte[4];
            for (int i = 28; i < 32; i++)
                fs[i % 28] = b[i];
            de.fileSize = BitConverter.ToInt32(fs, 0);
            return de;
        }
    }

}
