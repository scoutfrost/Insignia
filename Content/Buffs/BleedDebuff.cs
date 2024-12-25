using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Insignia.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Insignia.Content.Buffs
{
    public class BleedDebuff : ModBuff
    {
        public override string Texture => Helpers.GeneralHelper.Empty;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        List<int> stack = [];
        List<NPC> npcs = [];
        /// <summary>
        /// Called when the debuff gets reapplied to an npc. Allows for custom behavior when at a certain bleed stack count.
        /// </summary>
        /// <param name="stack">The list with the bleed stack counts. Every index is a different NPC.</param>
        /// <param name="npc">The target with the debuff.</param>
        /// <param name="index">The index of the target with the debuff in the stack list.</param>
        public delegate void ReApplyBleedDelegate(ref List<int> stack, NPC npc, int index);
        public ReApplyBleedDelegate ReApplyBleedDebuff;
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            if (npcs.Contains(npc))
            {
                return false;
            }
            npcs.Add(npc);
            stack.Add(0);
            return false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npcs.Contains(npc))
            {
                ReApplyBleedDebuff(ref stack, npc, npcs.IndexOf(npc));
            }
        }

        public void ReApplyBleed(ref List<int> stackList, NPC npc, int index, Player player, float bleedBuildup)
        {
            stackList[npcs.IndexOf(npc)] += (int)(bleedBuildup * 1 + player.GetModPlayer<InsigniaPlayer>().BleedBuildupMultiplier);

            int stack = stackList[index];
            //Main.NewText(stack);
            int npcBleedHealth = npc.boss ? npc.lifeMax / 10 : 10 + npc.lifeMax / 3;
            if (npcBleedHealth >= npcBleedHealth / 3)
            {
                npc.lifeRegen -= 12;
            }

            if (stack >= npcBleedHealth && npc.type != NPCID.TargetDummy)
            {
                player.GetModPlayer<InsigniaPlayer>().BleedProc = true;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustSpeed = new Vector2(0, -4).RotatedByRandom(MathHelper.ToRadians(20));
                    Dust.NewDust(Main.rand.NextVector2FromRectangle(npc.Hitbox), 10, 10, DustID.Blood, dustSpeed.X,
                        dustSpeed.Y, default, default, 2f);
                }

                int bleedDamage = (int)(npc.lifeMax * 0.09 * player.GetModPlayer<InsigniaPlayer>().BleedDamageMultiplier);

                if (bleedDamage < 0)
                    bleedDamage = 0;

                npc.life -= bleedDamage;
                CombatText.NewText(new((int)npc.Center.X, (int)npc.Center.Y, 10, 10), Color.DarkRed, bleedDamage);

                if (npc.life <= 0)
                    npc.life = 1;

                stack = 0;
                stackList[index] = stack;
            }
        }

        public override void Unload()
        {
            stack = null;
            npcs = null;
        }
    }
}
