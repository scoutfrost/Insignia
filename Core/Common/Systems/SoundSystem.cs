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
    
        static SoundSystem()
        {
            GlacialChunkSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkSound", (SoundType)0);
            GlacialChunkKillSound = new SoundStyle("Insignia/Assets/Sounds/GlacialChunkKillSound", (SoundType)0);
        }
    }
}