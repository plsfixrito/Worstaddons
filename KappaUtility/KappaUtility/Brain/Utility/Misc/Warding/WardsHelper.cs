using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Databases;
using KappaUtility.Common.Misc;
using SharpDX;

namespace KappaUtility.Brain.Utility.Misc.Warding
{
    internal class WardsHelper
    {
        private static Menu menu;

        internal static void Init()
        {
            try
            {
                menu = Utility.Load.menu.AddSubMenu("Warding Assist");

                menu.AddGroupLabel("Warding Assist [BETA]");
                menu.CreateCheckBox("wardshelper", "Enable Wards Helper", false);
                menu.AddLabel("This Helps You To Place You Wards Correctly Into Grass Without Miss-Placing It");
                menu.AddSeparator(0);
                menu.CreateCheckBox("wardsblock", "Block Warding Visible Grass", false);
                menu.AddLabel("Blocks the ward if there is a ward in the same Grass");

                //Drawing.OnDraw += Drawing_OnDraw;
                Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Misc.Warding.WardsHelper.Init", ex, Logger.LogLevel.Error);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ClosestGrass(Game.CursorPos) != null && ClosestGrass(Game.CursorPos) != Vector2.Zero)
                Circle.Draw(Color.AliceBlue, 100, ClosestGrass(Game.CursorPos).To3D());

            Circle.Draw(Color.AliceBlue, 600, Player.Instance);
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe || !IsWard(args.Slot))
                return;

            var endpos = args.EndPosition;

            if (menu.CheckBoxValue("wardshelper"))
            {
                if (!endpos.IsGrass() && ClosestGrass(endpos) != null && ClosestGrass(endpos) != Vector2.Zero)
                {
                    args.Process = false;
                }

                endpos = ClosestGrass(args.EndPosition).To3D();

                if (menu.CheckBoxValue("wardsblock") && !WardNearby(endpos) && endpos != Vector3.Zero || !menu.CheckBoxValue("wardsblock"))
                {
                    if (endpos.IsInRange(Game.CursorPos, 1250))
                        Player.CastSpell(args.Slot, endpos);
                }
            }
            if (menu.CheckBoxValue("wardsblock"))
            {
                if (WardNearby(endpos))
                {
                    args.Process = false;
                }
            }
        }

        private static bool WardNearby(Vector3 pos)
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(m => m.Name.Contains("Ward") && !m.Name.Contains("Corpse") && !m.IsDead && m.IsValid && m.IsAlly && m.ServerPosition.IsGrass() && m.Distance(pos) < 600);
        }

        private static bool IsWard(SpellSlot slot)
        {
            return slot != SpellSlot.Unknown
                   && (ItemsDatabase.WardingItems.Any(i => i.IsOwned(Player.Instance) && i.IsReady() && i.Slots.Any(s => s.Equals(slot)))
                       || ItemsDatabase.VisionWardingItems.Any(i => i.IsOwned(Player.Instance) && i.IsReady() && i.Slots.Any(s => s.Equals(slot))));
        }

        private static Vector2 ClosestGrass(Vector3 pos)
        {
            var Grass = CellsAnalyze(pos).OrderBy(c => c.WorldPosition.Distance(pos)).FirstOrDefault(b => b.WorldPosition.IsInRange(Player.Instance, 600));
            return Grass?.WorldPosition.To2D() ?? Vector2.Zero;
        }

        // credits to hellsing (Dev-a-lot)
        private static IEnumerable<NavMeshCell> CellsAnalyze(Vector3 pos)
        {
            var sourceGrid = pos.ToNavMeshCell();
            var startPos = new NavMeshCell(sourceGrid.GridX - (short)Math.Floor(15f), sourceGrid.GridY - (short)Math.Floor(15f));

            var cells = new List<NavMeshCell> { startPos };
            for (var y = startPos.GridY; y < startPos.GridY + 15; y++)
            {
                for (var x = startPos.GridX; x < startPos.GridX + 15; x++)
                {
                    if (x == startPos.GridX && y == startPos.GridY)
                    {
                        continue;
                    }
                    if (x == sourceGrid.GridX && y == sourceGrid.GridY)
                    {
                        cells.Add(sourceGrid);
                    }
                    else
                    {
                        cells.Add(new NavMeshCell(x, y));
                    }
                }
            }
            return cells.Where(c => c.WorldPosition.IsGrass());
        }
    }
}
