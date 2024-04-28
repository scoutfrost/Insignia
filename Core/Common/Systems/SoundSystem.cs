using Terraria.Audio;
using Terraria.ModLoader;

namespace Insignia.Core.Common.Systems
{
    public class SoundSystem : ModSystem
    {
        public static readonly SoundStyle GlacialChunkSound;
        public static readonly SoundStyle GlacialChunkKillSound;
        public static readonly SoundStyle TreeHurt;
        public static readonly SoundStyle TreeKill;
        public static readonly SoundStyle CyroBang;


        static SoundSystem()
        {
            GlacialChunkSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkSound", 0);
            GlacialChunkKillSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkKillSound", 0);
            TreeHurt = new SoundStyle("Insignia/Assets/Sounds/TreeHurt", 0);
            TreeKill = new SoundStyle("Insignia/Assets/Sounds/TreeKill", 0);
            CyroBang = new SoundStyle("Insignia/Assets/Sounds/CyroBang", 0);

        }
    }
}