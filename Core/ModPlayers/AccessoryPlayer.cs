using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;

namespace Insignia.Core.ModPlayers
{
    public class AccessoryPlayer : ModPlayer
    {
        public event Action<Player> UpDateEquipEvent;
        public event Action ResetEffectsEvent;
        public event Action<NPC, NPC.HitInfo, int, Player> OnHitNPCEvent;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitNPCEvent?.Invoke(target, hit, damageDone, Entity);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ResetEffects()
        {
            ResetEffectsEvent?.Invoke();
        }
        public override void UpdateEquips()
        {
            UpDateEquipEvent?.Invoke(Player);
        }
    }
}
