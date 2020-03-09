using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public class PacketBuffer : IDisposable
    {

        List<byte> _bufferlist;
        byte[] _readbuffer;
        int _readpos;
        bool _buffUpdate = false;

        public PacketBuffer()
        {
            _bufferlist = new List<byte>();
            _readpos = 0;
        }

        public int GetReadPos()
        {
            return _readpos;
        }

        public byte[] ToArray()
        {
            return _bufferlist.ToArray();
        }

        public int Count()
        {
            return _bufferlist.Count;
        }

        public int Lenght()
        {
            return Count() - _readpos;
        }

        public void Clear()
        {
            _bufferlist.Clear();
            _readpos = 0;
        }

        //Write data

        public void WriteBytes(byte[] input)
        {
            _bufferlist.AddRange(input);
            _buffUpdate = true;
        }
        public void WriteByte(byte input)
        {
            _bufferlist.Add(input);
            _buffUpdate = true;
        }
        public void WriteInteger(int input)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        public void WriteFloat(float input)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(input));
            _buffUpdate = true;
        }
        public void WriteString(string input)
        {
            _bufferlist.AddRange(BitConverter.GetBytes(input.Length));
            _bufferlist.AddRange(Encoding.ASCII.GetBytes(input));
            _buffUpdate = true;
        }

        // Read data

        public int ReadInteger(bool peek = true)
        {
            if (_bufferlist.Count > _readpos)
            {
                if (_buffUpdate)
                {
                    _readbuffer = _bufferlist.ToArray();
                    _buffUpdate = false;
                }

                int value = BitConverter.ToInt32(_readbuffer, _readpos);
                if (peek & _bufferlist.Count > _readpos)
                {
                    _readpos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past its Limit!");
            }
        }
        public float ReadFloat(bool peek = true)
        {
            if (_bufferlist.Count > _readpos)
            {
                if (_buffUpdate)
                {
                    _readbuffer = _bufferlist.ToArray();
                    _buffUpdate = false;
                }

                float value = BitConverter.ToSingle(_readbuffer, _readpos);
                if (peek & _bufferlist.Count > _readpos)
                {
                    _readpos += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past its Limit!");
            }
        }
        public byte ReadByte(bool peek = true)
        {
            if (_bufferlist.Count > _readpos)
            {
                if (_buffUpdate)
                {
                    _readbuffer = _bufferlist.ToArray();
                    _buffUpdate = false;
                }

                byte value = _readbuffer[_readpos];
                if (peek & _bufferlist.Count > _readpos)
                {
                    _readpos += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Buffer is past its Limit!");
            }
        }
        public byte[] ReadBytes(int lenght, bool peek = true)
        {
            if (_buffUpdate)
            {
                _readbuffer = _bufferlist.ToArray();
                _buffUpdate = false;
            }

            byte[] value = _bufferlist.GetRange(_readpos, lenght).ToArray();
            if (peek & _bufferlist.Count > _readpos)
            {
                _readpos += lenght;
            }
            return value;
        }
        public string ReadString(bool peek = true)
        {
            int lenght = ReadInteger(true);
            if (_buffUpdate)
            {
                _readbuffer = _bufferlist.ToArray();
                _buffUpdate = false;
            }

            string value = Encoding.ASCII.GetString(_readbuffer, _readpos, lenght);
            if (peek & _bufferlist.Count > _readpos)
            {
                _readpos += lenght;
            }
            return value;
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _bufferlist.Clear();
            }
            _readpos = 0;
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
