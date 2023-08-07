
using Insignia;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Tiles
{
	public class GlacialChunkTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;

			DustType = DustID.Ice;
			HitSound = new SoundStyle($"{nameof(Insignia)}/Assets/Sounds/GlacialChunkSound");

			AddMapEntry(new Color(20, 80, 200));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

//# TO DO: FIX UP THE KILL SOUND (I THINK I GOTTA MAKE AN EntitySource_OnBreak

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            new SoundStyle($"{nameof(Insignia)}/Assets/Sounds/GlacialChunkKillSound");

        }
       
    }
}