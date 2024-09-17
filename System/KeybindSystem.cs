using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace TAgent.System
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind Train { get; private set; }

        public override void Load()
        {
            Train = KeybindLoader.RegisterKeybind(Mod, "Train", "K");
        }
        public override void Unload()
        {
            Train = null;
        }
    }
}