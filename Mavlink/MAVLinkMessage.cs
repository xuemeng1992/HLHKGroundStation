﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkMessage
    { 

        public static readonly MAVLinkMessage Invalid = new MAVLinkMessage();
        object _locker = new object();

        public byte[] buffer { get; internal set; }

        public byte header { get; internal set; }
        public byte payloadlength { get; internal set; }

        public byte incompat_flags { get; internal set; }
        public byte compat_flags { get; internal set; }

        public byte seq { get; internal set; }
        public byte sysid { get; internal set; }
        public byte compid { get; internal set; }

        public uint msgid { get; internal set; }

        public bool ismavlink2 {
            get
            {
                if (buffer != null && buffer.Length > 0)
                    return (buffer[0] == MAVLINK_STX);

                return false;
            }
        }

        object _data;
        public object data
        {
            get
            {
                // lock the entire creation of the packet. to prevent returning a incomplete packet.
                lock (_locker)
                {
                    if (_data != null)
                        return _data;

                    _data = Activator.CreateInstance(MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid).type);

                    try
                    {
                        // fill in the data of the object
                        if (buffer[0] == MAVLINK_STX)
                        {
                            MavlinkUtil.ByteArrayToStructure(buffer, ref _data, MAVLINK_NUM_HEADER_BYTES, payloadlength);
                        }
                        else
                        {
                            MavlinkUtil.ByteArrayToStructure(buffer, ref _data, 6, payloadlength);
                        }
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }

                return _data;
            }
        }

        public T ToStructure<T>()
        {
            return (T)data;
        }

        public ushort crc16 { get; internal set; }

        public byte[] sig { get; internal set; }

        public byte sigLinkid
        {
            get
            {
                if (sig != null)
                {
                    return sig[0];
                }

                return 0;
            }
        }

        public ulong sigTimestamp 
        {
            get
            {
                if (sig != null)
                {
                    byte[] temp = new byte[8];
                    Array.Copy(sig, 1, temp, 0, 6);
                    return BitConverter.ToUInt64(temp, 0);
                }

                return 0;
            }
        }

        public int Length
        {
            get
            {
                if (buffer == null) return 0;
                return buffer.Length;
            }
        }

        public MAVLinkMessage()
        {

        }

        public MAVLinkMessage(byte[] buffer)
        {
            this.buffer = buffer;

            if (buffer[0] == MAVLINK_STX)
            {
                header = buffer[0];
                payloadlength = buffer[1];
                incompat_flags = buffer[2];
                compat_flags = buffer[3];
                seq = buffer[4];
                sysid = buffer[5];
                compid = buffer[6];
                msgid = (uint)((buffer[9] << 16) + (buffer[8] << 8) + buffer[7]);

                var crc1 = MAVLINK_CORE_HEADER_LEN + payloadlength + 1;
                var crc2 = MAVLINK_CORE_HEADER_LEN + payloadlength + 2;

                crc16 = (ushort)((buffer[crc2] << 8) + buffer[crc1]);

                if ((incompat_flags & MAVLINK_IFLAG_SIGNED) > 0)
                {
                    sig = new byte[MAVLINK_SIGNATURE_BLOCK_LEN];
                    Array.ConstrainedCopy(buffer, buffer.Length - MAVLINK_SIGNATURE_BLOCK_LEN, sig, 0,
                        MAVLINK_SIGNATURE_BLOCK_LEN);
                }
            }
            else
            {
                header = buffer[0];
                payloadlength = buffer[1];
                seq = buffer[2];
                sysid = buffer[3];
                compid = buffer[4];
                msgid = buffer[5];

                var crc1 = MAVLINK_CORE_HEADER_MAVLINK1_LEN + payloadlength + 1;
                var crc2 = MAVLINK_CORE_HEADER_MAVLINK1_LEN + payloadlength + 2;

                crc16 = (ushort)((buffer[crc2] << 8) + buffer[crc1]);
            }
        }
    }
}
