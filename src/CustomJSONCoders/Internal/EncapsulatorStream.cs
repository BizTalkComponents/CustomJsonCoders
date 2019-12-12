using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BizTalkComponents.PipelineComponents.CustomJsonCoders.Internal
{
    public class EncapsulatorStream : Stream
    {
        private Stream baseStream;
        private bool m_IsJsonArray;
        private string m_JArrName;
        public EncapsulatorStream(Stream stream, string jArrName)
        {
            baseStream = stream;
            m_JArrName = string.IsNullOrEmpty(jArrName) ? "data" : jArrName;
        }

        public override bool CanRead
        {
            get
            {
                return baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return baseStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return baseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return baseStream.Position;
            }

            set
            {
                baseStream.Position = value;
            }
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int retval = 0, bytesRead = 0;
            if (baseStream.Position == 0)
            {
                // take the half of the buffer so we'll be able to inject the required bytes to encapsulate
                int newCount = count / 2 + 1;
                byte[] buff = new byte[newCount];
                //read the first block of JSON data.
                bytesRead = baseStream.Read(buff, 0, newCount);
                retval += bytesRead;
                int i = 0, newOffset = offset;
                // check where the first byte of json comes.
                while (buff[i] != '{' & buff[i] != '[')
                    i++;
                // if it is Json array
                if (buff[i] == '[')
                {
                    m_IsJsonArray = true;
                    if (i > 0)
                    {
                        Array.Copy(buff, 0, buffer, newOffset, i);
                        newOffset += i;
                    }
                    var bytesToInject = Encoding.ASCII.GetBytes("{ \"" + m_JArrName + "\": ");
                    Array.Copy(bytesToInject, 0, buffer, newOffset, bytesToInject.Length);
                    newOffset += bytesToInject.Length;
                    retval += bytesToInject.Length;
                }
                Array.Copy(buff, i, buffer, newOffset, bytesRead - i);
                newOffset += bytesRead - i;
                //read the rest of the buffer
                if (baseStream.Position < baseStream.Length - 1)
                {
                    newCount = count - (newOffset - offset);
                    bytesRead = baseStream.Read(buff, 0, newCount);
                    Array.Copy(buff, 0, buffer, newOffset, bytesRead);
                    newOffset += bytesRead;
                    retval += bytesRead;
                }
            }
            else //if (baseStream.Position < baseStream.Length - 1)
            {
                retval = baseStream.Read(buffer, offset, count);
            }
            if (baseStream.Position == baseStream.Length & retval > 0 & m_IsJsonArray)
            {
                if (retval < buffer.Length - offset)
                {
                    buffer[retval] = (byte)'}';
                    retval++;
                }
            }
            return retval;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(); 
        }
        #region Not supported methods
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        public override int ReadByte()
        {
            throw new NotSupportedException();
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
        #endregion
        public override void Close()
        {
            baseStream.Close();
            m_Closed = true; 
        }
        private bool m_Disposed, m_Closed;
        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                m_Disposed = true;
                if (!m_Closed) Close();
                baseStream = null;
            }
        }
    }
}

