using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Insignia.Core.Common.Systems
{
    public class SoundSystem : ModSystem
    {
        public static readonly SoundStyle GlacialChunkSound;
        public static readonly SoundStyle GlacialChunkKillSound;
        public static readonly SoundStyle TreeHurt;
        public static readonly SoundStyle TreeKill;



        static SoundSystem()
        {
            GlacialChunkSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkSound", (SoundType)0);
            GlacialChunkKillSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkKillSound", (SoundType)0);
            TreeHurt = new SoundStyle("Insignia/Assets/Sounds/TreeHurt", (SoundType)0);
            TreeKill= new SoundStyle("Insignia/Assets/Sounds/TreeKill", (SoundType)0);


        }
    }
}