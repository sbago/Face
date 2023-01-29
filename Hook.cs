using Dalamud.Hooking;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Face
{
    public class Hook:IDisposable
    {
        private delegate IntPtr UpdatePos2(IntPtr a1, IntPtr a2, uint a3);
        private Hook<UpdatePos2> _updatePos2;
        private IntPtr A1= IntPtr.Zero;
        private IntPtr A2 = IntPtr.Zero;
        private Struct.C2S_UpdatePos_H LastPos = new();
        public bool State;
        public float Rotion { get; set; }
        public Hook() {
            _updatePos2 = Hook<UpdatePos2>.FromAddress(DalamudApi.SigScanner.ScanText("48 89 5C 24 08 48 89 74 24 20 57 48 81 EC 80 0F 00 00 48 8B 05 E7 98 B3 01"), UpdatePos2Detour);
            //_updatePos2.Enable();
        }

        //updatepos的上游，记录数据以便调用。
        private unsafe IntPtr UpdatePos2Detour(IntPtr a1, IntPtr a2, uint a3)
        {

            var pos = (Struct.C2S_UpdatePos_H*) a2;
            LastPos.R = pos->R;
            LastPos.R1 = pos->R1;
            LastPos.Pading= pos->Pading;
            LastPos.X= pos->X;
            LastPos.Y= pos->Y;
            LastPos.Z= pos->Z;
            LastPos.X1 = pos->X1;
            LastPos.Y1 = pos->Y1;
            LastPos.Z1 = pos->Z1;
            return _updatePos2.Original(a1, a2, a3);
        }
        public unsafe void Invoke()
        {
            if (A1 == IntPtr.Zero)
                return;
            var pos = LastPos;
            A2 = (IntPtr)(&pos);//不确定堆栈位置会不会变 手动创建一个。
            _updatePos2.Original.Invoke(A1, A2, 1);
        }
        public void Enable(float r)
        {
            this.State = true;
            this.Rotion= r;
            _updatePos2.Enable();
            DalamudApi.ChatGui.PrintChat(new() { Message = new Dalamud.Game.Text.SeStringHandling.SeString(new Dalamud.Game.Text.SeStringHandling.Payloads.TextPayload($"已设置面向:{r:f3}")) });
        }
        public void Disable()
        {
            this.State = false;
            _updatePos2.Disable();
            DalamudApi.ChatGui.Print("已关闭");
        }
        public void Dispose()
        {
            _updatePos2.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
