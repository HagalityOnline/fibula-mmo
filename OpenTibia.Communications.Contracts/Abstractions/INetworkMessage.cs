// -----------------------------------------------------------------
// <copyright file="INetworkMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    public interface INetworkMessage
    {
        byte[] Buffer { get; }

        int Length { get; }

        int Position { get; }

        void AddByte(byte value);

        void AddBytes(byte[] value);

        void AddPaddingBytes(int count);

        void AddString(string value);

        void AddUInt16(ushort value);

        void AddUInt32(uint value);

        byte GetByte();

        byte[] GetBytes(int count);

        byte[] GetPacket();

        string GetString();

        ushort GetUInt16();

        uint GetUInt32();

        byte PeekByte();

        byte[] PeekBytes(int count);

        string PeekString();

        ushort PeekUInt16();

        uint PeekUInt32();

        bool PrepareToRead(uint[] xteaKey);

        bool PrepareToSend(uint[] xteaKey);

        bool PrepareToSendWithoutEncryption(bool insertOnlyOneLength = false);

        void ReplaceBytes(int index, byte[] value);

        void Reset();

        void Reset(int startingIndex);

        void Resize(int size);

        void RsaDecrypt(bool useCipKeys = true);

        void SkipBytes(int count);

        bool XteaDecrypt(uint[] key);

        bool XteaEncrypt(uint[] key);

        INetworkMessage Copy();
    }
}