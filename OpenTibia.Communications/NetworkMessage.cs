// <copyright file="NetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using System.Text;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Security.Encryption;

    /// <summary>
    /// Class that represents a network message.
    /// </summary>
    public class NetworkMessage : INetworkMessage
    {
        /// <summary>
        /// The number of reserved bytes in the network message for outbound messages.
        /// </summary>
        public const int OutboundMessageStartingIndex = 4;

        /// <summary>
        /// The size of the message's buffer.
        /// </summary>
        public const int BufferSize = 16394;

        /// <summary>
        /// The default index at which the message's content starts.
        /// </summary>
        private const int DefaultStartingIndex = 2;

        /// <summary>
        /// The buffer of the message.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// The length of the message.
        /// </summary>
        private int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        /// <param name="isOutbound">A value indicating whether this message is an outboud message.</param>
        public NetworkMessage(bool isOutbound = true)
            : this (isOutbound ? OutboundMessageStartingIndex : 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        private NetworkMessage()
        {
            this.Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMessage"/> class.
        /// </summary>
        /// <param name="startingIndex">The index at which to set the <see cref="Position"/> of this message.</param>
        private NetworkMessage(int startingIndex)
        {
            this.Reset(startingIndex);
        }

        /// <summary>
        /// Gets the length of the message.
        /// </summary>
        public int Length => this.length;

        /// <summary>
        /// Gets the position that the message will read from next.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the buffer of this message.
        /// </summary>
        public byte[] Buffer => this.buffer;

        /// <summary>
        /// Clears the message buffer and resets the <see cref="Position"/> to the given index.
        /// </summary>
        /// <param name="startingIndex">The index at which to reset the <see cref="Position"/> of this message.</param>
        public void Reset(int startingIndex)
        {
            this.buffer = new byte[NetworkMessage.BufferSize];
            this.length = startingIndex;
            this.Position = startingIndex;
        }

        /// <summary>
        /// Resets the message.
        /// </summary>
        public void Reset()
        {
            this.Reset(DefaultStartingIndex);
        }

        public byte GetByte()
        {
            if (this.Position + 1 > this.Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetByte() out of range.");
            }

            return this.buffer[this.Position++];
        }

        public byte[] GetBytes(int count)
        {
            if (this.Position + count > this.Length)
            {
                throw new IndexOutOfRangeException("NetworkMessage GetBytes() out of range.");
            }

            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Position, t, 0, count);

            this.Position += count;
            return t;
        }

        public INetworkMessage Copy()
        {
            NetworkMessage newMessage = new NetworkMessage
            {
                length = this.Length,
                Position = this.Position,
            };

            this.Buffer.CopyTo(newMessage.buffer, 0);

            return newMessage;
        }

        public string GetString()
        {
            int len = this.GetUInt16();
            string t = Encoding.Default.GetString(this.buffer, this.Position, len);

            this.Position += len;
            return t;
        }

        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(this.GetBytes(sizeof(ushort)), 0);
        }

        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(this.GetBytes(sizeof(uint)), 0);
        }

        public byte[] GetPacket()
        {
            byte[] t = new byte[this.Length - 2];
            Array.Copy(this.buffer, 2, t, 0, this.Length - 2);
            return t;
        }

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(this.buffer, 0);
        }

        public void AddByte(byte value)
        {
            if (1 + this.Length > NetworkMessage.BufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            this.AddBytes(new[] { value });
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length + this.Length > NetworkMessage.BufferSize)
            {
                throw new Exception("NetworkMessage buffer is full.");
            }

            Array.Copy(value, 0, this.buffer, this.Position, value.Length);

            this.Position += value.Length;

            if (this.Position > this.Length)
            {
                this.length = this.Position;
            }
        }

        public void AddString(string value)
        {
            this.AddUInt16((ushort)value.Length);
            this.AddBytes(Encoding.Default.GetBytes(value));
        }

        public void AddUInt16(ushort value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        public void AddUInt32(uint value)
        {
            this.AddBytes(BitConverter.GetBytes(value));
        }

        public void AddPaddingBytes(int count)
        {
            this.Position += count;

            if (this.Position > this.Length)
            {
                this.length = this.Position;
            }
        }

        public byte PeekByte()
        {
            return this.buffer[this.Position];
        }

        public void Resize(int size)
        {
            this.length = size;
            this.Position = 0;
        }

        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(this.buffer, this.Position, t, 0, count);
            return t;
        }

        public ushort PeekUInt16()
        {
            return BitConverter.ToUInt16(this.PeekBytes(sizeof(ushort)), 0);
        }

        public uint PeekUInt32()
        {
            return BitConverter.ToUInt32(this.PeekBytes(sizeof(uint)), 0);
        }

        public string PeekString()
        {
            int len = this.PeekUInt16();
            return Encoding.ASCII.GetString(this.PeekBytes(len + 2), 2, len);
        }

        public void ReplaceBytes(int index, byte[] value)
        {
            if (this.Length - index >= value.Length)
            {
                Array.Copy(value, 0, this.buffer, index, value.Length);
            }
        }

        public void SkipBytes(int count)
        {
            if (this.Position + count > this.Length)
            {
                throw new IndexOutOfRangeException($"NetworkMessage {nameof(this.SkipBytes)} out of range.");
            }

            this.Position += count;
        }

        public void RsaDecrypt(bool useCipKeys = true)
        {
            Rsa.Decrypt(ref this.buffer, this.Position, this.length, useCipKeys);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.Decrypt(ref this.buffer, ref this.length, 2, key);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.Encrypt(ref this.buffer, ref this.length, 2, key);
        }

        public bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false)
        {
            if (!insertOnlyOneLength)
            {
                this.InsertPacketLength();
            }

            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToSend(uint[] xteaKey)
        {
            // Must be before Xtea, because the packet length is encrypted as well
            this.InsertPacketLength();

            if (!this.XteaEncrypt(xteaKey))
            {
                return false;
            }

            // Must be after Xtea, because takes the checksum of the encrypted packet
            // InsertAdler32();
            this.InsertTotalLength();

            return true;
        }

        public bool PrepareToRead(uint[] xteaKey)
        {
            if (!this.XteaDecrypt(xteaKey))
            {
                return false;
            }

            this.Position = 4;
            return true;
        }

        private void InsertPacketLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 4)), 0, this.buffer, 2, 2);
        }

        private void InsertTotalLength()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(this.length - 2)), 0, this.buffer, 0, 2);
        }
    }
}
