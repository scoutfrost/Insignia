using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Insignia.Helpers 
{
    public static class GeneralHelper
    {
        public const string Empty = "Insignia/Assets/Textures/Empty";

        public const string BleedDescription = "Bleed is an accumulative debuff where after a certain amount of buildup, the enemy loses a certain amount of their max life." +
            "\nThe amount of buildup and damage done when fully accumulated can be altered and buffed with certain accessories and armor.";

        public static void AddExpandableTooltip(ref List<TooltipLine> tooltips, Mod mod, Color pressForMoreInfoLineColor, string expandedDescription, Color expandedDescriptionColor)
        {
            int index;
            if (tooltips[^1].Name == "JourneyResearch")
                index = tooltips.Count - 1;
            else
                index = tooltips.Count;

            TooltipLine infoLine = new(mod, "Tooltip#", "Press left shift for more info") { OverrideColor = pressForMoreInfoLineColor };
            tooltips.Insert(index, infoLine);

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                tooltips.Insert(tooltips.Count - 1, new TooltipLine(mod, "Tooltip#", expandedDescription) { OverrideColor = expandedDescriptionColor });
                tooltips.Remove(infoLine);
            }
        }
        public static float Pythagoras(float a = default, float b = default, float c = default)
        {
            if (c == default)
                return (float)Math.Sqrt(a * a + b * b);

            if (a == default)
                return (float)Math.Sqrt(c * c - b * b);

            if (b == default)
                return (float)Math.Sqrt(c * c - a * a);

            return default;
        }
        public static bool DoesTileCollideWithLine(Vector2 start, Vector2 end)
        {
            for (int i = 0; i < (end - start).Length(); i++)
            {
                Vector2 tile = Vector2.Lerp(start, end, i / (end - start).Length());
                if (!WorldGen.TileEmpty((int)tile.X / 16, (int)tile.Y / 16) && WorldGen.SolidTile((int)tile.X / 16, (int)tile.Y / 16))
                {
                    return true;
                }
            }
            return false;
        }
    }
}