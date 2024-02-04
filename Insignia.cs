using Insignia.Core.Common.Systems;
using System;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Insignia
{
    public class Insignia : Mod
    {
        public static Insignia Instance = new();

        public const string AssetPath = $"{nameof(Insignia)}/Assets/";

        public static float ModTime { get; internal set; }
        public static object MessageType { get; internal set; }

        internal static object GetLegacySoundSlot(object custom, string v)
        {
            throw new NotImplementedException();
        }

        internal static object GetLegacySoundSlot(SoundType soundType)
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            ParticleSystem.Unload();
        }
    }
}