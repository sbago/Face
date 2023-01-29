using Dalamud.Logging;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace Face
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Face";
        public static Plugin _Plugin { get; private set; }
        private Hook hook;
        private bool Enable =>hook.State;
        public Plugin(DalamudPluginInterface dalamudPluginInterface)
        {
            try
            {
                _Plugin = this;
                DalamudApi.Initialize(this, dalamudPluginInterface);
                hook = new Hook();
                DalamudApi.GameNetwork.NetworkMessage += GameNetwork_NetworkMessage;
                DalamudApi.Framework.Update += Framework_Update;
                DalamudApi.CommandManager.AddHandler("/Face", new Dalamud.Game.Command.CommandInfo(FaceCommand) { HelpMessage = "设置面向 -3.14到3.14之间 s关闭 \n 例如/Face 1.5，/Face s" });
            }catch(Exception ex)
            {
                PluginLog.Error(ex.ToString());
            }
        }

        private void Framework_Update(Dalamud.Game.Framework framework)
        {
            if (Enable)
                hook.Invoke();
        }

        private unsafe void GameNetwork_NetworkMessage(IntPtr dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, Dalamud.Game.Network.NetworkMessageDirection direction)
        {
            if (!Enable)
                return;
            if (direction == Dalamud.Game.Network.NetworkMessageDirection.ZoneUp)
            {
                if ((Struct.Opcode)opCode == Struct.Opcode.C2S_ActionRequest)
                {
                    hook.Invoke();
                }
                else if ((Struct.Opcode)opCode == Struct.Opcode.C2S_UpdatePos_W)
                {
                    var Test = (Struct.C2S_UpdatePos_W*)dataPtr;
                    Test->R = hook.Rotion;
                }
                else if ((Struct.Opcode)opCode == Struct.Opcode.C2S_UpdatePos_H)
                {
                    var Test = (Struct.C2S_UpdatePos_H*)dataPtr;
                    Test->R = hook.Rotion;
                    Test->R1 = hook.Rotion;
                }
            }
            else
            {
                if ((Struct.Opcode)opCode == Struct.Opcode.S2C_ActorControlSelf)
                {
                    var Test = (Struct.S2C_ActorControlSelf*)dataPtr;
                    if (Test->category == 17)
                        hook.Invoke();
                }
            }
        }

        public void Dispose()
        {
            DalamudApi.Framework.Update -= Framework_Update;
            DalamudApi.GameNetwork.NetworkMessage -= GameNetwork_NetworkMessage;
            hook.Dispose();
            DalamudApi.CommandManager.RemoveHandler("/Face");
            GC.SuppressFinalize(this);
        }
        //[Command("/Face")]
        //[HelpMessage("Set face")]
        public void FaceCommand(string command,string args)
        {
            try
            {
                if(DalamudApi.ClientState.LocalPlayer != null)
                {
                    if (args == "s")
                    {
                        this.hook.Disable();
                    }
                    var Rotion = float.Parse(args);
                    if(Rotion>3.14 && Rotion < -3.14)
                    {
                        PluginLog.Error("set wrong");
                        return;
                    }
                    //PluginLog.Information(Rotion.ToString());
                    this.hook.Enable(Rotion);
                }
            }catch(Exception ex)
            {
                PluginLog.Error(ex.ToString());
            }
        }
    }
}