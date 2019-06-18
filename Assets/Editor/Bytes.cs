using System.Text;
using System;
using System.IO;

namespace rf{
    public class Bytes : BinaryWriter{

        public Bytes() : base(new MemoryStream())
        {
            
        }

        public byte[] ToArray()
        {
            return (base.BaseStream as MemoryStream).ToArray();
        }

        public void write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            write(bytes);
        }

        public void write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);			
            write(bytes);
        }

        public void write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            write(bytes);
        }

        public void write(bool value)
        {
            this.BaseStream.WriteByte(value ? ((byte)1) : ((byte)0));
        }

        public void WriteUTF(string value)
        {
            if (value==null)
            {
                value = "";
            }
            //Length - max 65536.
            int byteCount = Encoding.UTF8.GetByteCount(value);
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            this.write(byteCount);
            if (buffer.Length > 0)
                base.Write(buffer);
        }

        public void write(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes((ushort)value);
            write(bytes);
        }

        public void WriteUTFBytes(string value)
        {
            if (value == null)
            {
                value = "";
            }
            //Length - max 65536.
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            if (buffer.Length > 0)
                base.Write(buffer);
        }


        // private void WriteBigEndian(byte[] bytes)
        // {
        //     if( bytes == null )
        //         return;
        //     for(int i = bytes.Length-1; i >= 0; i--)
        //     {
        //         base.BaseStream.WriteByte( bytes[i] );
        //     }
        // }
        public void write(byte value)
        {
            this.BaseStream.WriteByte(value);
        }

        public void write(byte[] buffer)
        {
            for(int i = 0; buffer != null && i < buffer.Length; i++)
                this.BaseStream.WriteByte(buffer[i]);
        }

        public void Dispose(){
            base.Dispose(true);
        }
    }
}
