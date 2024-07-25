using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Insignia.Content.Buffs
{
    public class BleedDebuff : ModBuff
    {
        public override string Texture => Helpers.GeneralHelper.Empty;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
        List<int> stack = new();
        List<NPC> npcs = new();
        /// <summary>
        /// Called when the debuff gets reapplied to an npc. Allows for custom behavior when at a certain bleed stack count.
        /// </summary>
        /// <param name="stack">The list with the bleed stack counts. Every index is a different NPC.</param>
        /// <param name="npc">The target with the debuff.</param>
        /// <param name="index">The index of the target with the debuff in the stack list.</param>
        public delegate void ReApplyBleed(ref List<int> stack, NPC npc, int index);
        public ReApplyBleed reApplyBleed;
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            if (!npcs.Contains(npc))
            {
                npcs.Add(npc);
                stack.Add(0);
            }
            else
            {
                stack[npcs.IndexOf(npc)]++;
            }
            return true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npcs.Contains(npc))
            {
                reApplyBleed(ref stack, npc, npcs.IndexOf(npc));
            }
        }
        public override void Unload()
        {
            stack = null;
            npcs = null;
        }
    }
}
