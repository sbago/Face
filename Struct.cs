using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Face
{
    public struct Set
    {
        public bool Enable;
        public float Rotiion;
    }
    public static class Struct
    {
        public enum Opcode :ushort
        {
            C2S_ActionRequest = 0x363,
            C2S_UpdatePos_W = 0xd4,
            C2S_UpdatePos_H = 0x39a,
            S2C_ActorControl = 0x365,
            S2C_ActorControlSelf = 0x245
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct C2S_ActionRequest
        {
            public byte ActionProcState; // see ActionManager.GetAdjustedCastTime implementation, last optional arg
            public ActionType Type;
            public ushort u1;
            public uint ActionID;
            public ushort Sequence;
            public ushort IntCasterRot; // 0 = N, increases CCW, 0xFFFF = 2pi
            public ushort IntDirToTarget; // 0 = N, increases CCW, 0xFFFF = 2pi
            public ushort u3;
            public ulong TargetID;
            public ushort ItemSourceSlot;
            public ushort ItemSourceContainer;
            public uint u4;
            public ulong u5;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct C2S_UpdatePos_W
        {
            public float R;
            public float X;
            public float Y;
            public float Z;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct C2S_UpdatePos_H
        {
            public float R;
            public float R1;
            public float Pading;
            public float X;
            public float Y;
            public float Z;
            public float X1;
            public float Y1;
            public float Z1;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct S2C_ActorControl
        {
            public ushort category;
            public ushort unk0;
            public uint param1;
            public uint param2;
            public uint param3;
            public uint param4;
            public uint param5;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct S2C_ActorControlSelf
        {
            public ushort category;
            public ushort unk0;
            public uint param1;
            public uint param2;
            public uint param3;
            public uint param4;
            public uint param5;
            public uint param6;
            public uint param7;
        }
    }
}
