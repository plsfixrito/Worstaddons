using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaUtility.Common.Misc;
using SharpDX;

namespace KappaUtility.Brain.Utility.Tracker.SpellTracker
{
    internal sealed class SpellTracker : SpellTrackerBase
    {
        private Menu menu;

        private static Text trackettext = new Text("", new Font("calibri", 15, FontStyle.Regular)) { Color = System.Drawing.Color.AliceBlue };

        internal override void Load(bool SpectatorMode = false, Menu mainMenu = null)
        {
            try
            {
                menu = SpectatorMode ? mainMenu?.AddSubMenu("SpellTracker")
                    : Utility.Load.menu.AddSubMenu("SpellTracker");

                menu.AddGroupLabel("Spells Tracker");
                menu.CreateCheckBox("enable", "Enable Spells Tracker", false);

                menu.CreateCheckBox("screen", "Track Visible on Screen Only (FPS Boost)");
                var clear = menu.CreateKeyBind("clear1", "Clear All Detected Spells (Fix FPS Drop)", false, KeyBind.BindTypes.HoldActive);
                clear.OnValueChange += delegate
                    {
                        if (clear.CurrentValue)
                        {
                            DetectedSpells.Clear();
                            clear.CurrentValue = false;
                        }
                    };

                clear.IsVisible = false;

                foreach (var c in EntityManager.Heroes.AllHeroes)
                {
                    if(TrackableSpells.Any(t => c.ChampionName.Equals(t.ChampionName)))
                    {
                        menu.AddGroupLabel("[" + c.ChampionName + "] Spells");
                        foreach (var spell in TrackableSpells.Where(s => s.ChampionName.Equals(c.ChampionName) && !EnabledSpells.Contains(s)))
                        {
                            menu.CreateCheckBox(c.ChampionName + spell.SpellName, spell.SpellName);
                            EnabledSpells.Add(spell);
                        }
                    }
                }

                GameObject.OnCreate += GameObject_OnCreate;
                GameObject.OnDelete += GameObject_OnDelete;
                Drawing.OnEndScene += Drawing_OnEndScene;
                Game.OnTick += Game_OnTick;
            }
            catch (Exception ex)
            {
                Logger.Send("Error At KappaUtility.Brain.Utility.Tracker.SpellTracker.Init", ex, Logger.LogLevel.Error);
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.GetType() == typeof(Obj_GeneralParticleEmitter))
            {
                var gameObject = (Obj_GeneralParticleEmitter)sender;

                DetectedSpells.RemoveAll(o => (o.Object != null && o.Object.IdEquals(gameObject)) || (o.Sender != null && o.Sender.IdEquals(gameObject)));
            }
        }

        private HashSet<SpellInfo> EnabledSpells = new HashSet<SpellInfo>();

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.GetType() == typeof (Obj_GeneralParticleEmitter))
            {
                var gameObject = (Obj_GeneralParticleEmitter) sender;

                if (EnabledSpells.All(x => x.ObjectName != gameObject.Name) || DetectedSpells.Any(x=> x.ObjectName == gameObject.Name))
                    return;

                var info = EnabledSpells.FirstOrDefault(x => x.ObjectName == gameObject.Name);

                if (info != null)
                {
                    DetectedSpells.Add(new DetectedSpellInfo(info.SpellName, info.ChampionName, info.SpellTime, SpellType.GameObject, gameObject.Name, Game.Time+info.SpellTime, gameObject.NetworkId, gameObject.Position, null, gameObject));
                }
            }

            if (sender.GetType() != typeof (MissileClient))
                return;

            var missile = (MissileClient) sender;

            if (EnabledSpells.All(x => x.ObjectName != missile.Name) || DetectedSpells.Any(x => x.ObjectName != missile.Name))
                return;

            var spellInfo = EnabledSpells.FirstOrDefault(x => x.ObjectName == missile.Name);

            if (spellInfo != null)
            {
                DetectedSpells.Add(new DetectedSpellInfo(spellInfo.SpellName, spellInfo.ChampionName, spellInfo.SpellTime, SpellType.GameObject, missile.SData.Name, Game.Time + spellInfo.SpellTime, missile.NetworkId, missile.Position, missile.SpellCaster, missile));
            }
        }

        private static float lastupdate;
        private void Game_OnTick(EventArgs args)
        {
            if (!menu.CheckBoxValue("enable"))
                return;

            if (Core.GameTickCount - lastupdate > Utility.Load.FPSProtection)
            {
                foreach (var aiHeroClient in EntityManager.Heroes.AllHeroes)
                {
                    foreach (var spellInfo in EnabledSpells.Where(x => x?.SpellType == SpellType.Buff))
                    {
                        if (aiHeroClient.HasBuff(spellInfo.ObjectName) && !DetectedSpells.Any(x => x.Sender?.NetworkId == aiHeroClient.NetworkId && x.ObjectName == spellInfo.ObjectName))
                        {
                            var buff = aiHeroClient.Buffs?.FirstOrDefault(x => x.Name == spellInfo.ObjectName && x.IsActive && x.IsValid && (((AIHeroClient)x.Caster).ChampionName.Equals(spellInfo.ChampionName) || spellInfo.ChampionName == "All"));
                            if (buff != null && aiHeroClient != null)
                            {
                                DetectedSpells.Add(new DetectedSpellInfo(spellInfo.SpellName, spellInfo.ChampionName, (int)(buff.EndTime - Game.Time), SpellType.Buff, spellInfo.ObjectName, buff.EndTime, aiHeroClient.NetworkId, aiHeroClient.Position, aiHeroClient, null));
                            }
                        }
                    }
                }

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    foreach (var spellInfo in EnabledSpells.Where(x => x?.ObjectName == minion?.BaseSkinName && x?.SpellType == SpellType.Minion))
                    {
                        if (!DetectedSpells.Any(x => x.Sender?.NetworkId == minion?.NetworkId && x?.ObjectName == spellInfo?.ObjectName))
                        {
                            var info = EnabledSpells.FirstOrDefault(x => x?.ObjectName == minion?.BaseSkinName);
                            if (info != null && minion != null)
                                DetectedSpells.Add(new DetectedSpellInfo(spellInfo.SpellName, spellInfo.ChampionName, info.SpellTime, SpellType.Minion, spellInfo.ObjectName, Game.Time + info.SpellTime, minion.NetworkId, minion.Position, minion, null));
                        }
                    }
                }

                DetectedSpells.RemoveAll(x => x.EndTime - Game.Time <= 0);
                DetectedSpells.RemoveAll(x => x.SpellType == SpellType.Buff && (!x.Sender.HasBuff(x.ObjectName) || x.Sender.IsDead));
                DetectedSpells.RemoveAll(x => (x.SpellType.Equals(SpellType.Minion) || x.SpellType.Equals(SpellType.GameObject))
                && ((x.Sender != null && (x.Sender.IsDead || !x.Sender.IsValid)) || (x.Object != null && (x.Object.IsDead || !x.Object.IsValid))));

                lastupdate = Core.GameTickCount;
            }
        }

        private Vector2 LastDrawPos;
        private void Drawing_OnEndScene(EventArgs args)
        {
            if(!menu.CheckBoxValue("enable"))
                return;

            var i = 0f;
            foreach (var detectedSpellInfo in DetectedSpells.Where(s => (s.ChampionName == "All") ? menu.CheckBoxValue(s.Sender.BaseSkinName + s.SpellName) : menu.CheckBoxValue(s.ChampionName + s.SpellName) && s.Position.IsOnScreen()))
            {
                var msg = detectedSpellInfo.SpellName + " : " + (detectedSpellInfo.EndTime - Game.Time).ToString("F1");

                Vector3 pos = Game.CursorPos;

                if (detectedSpellInfo.SpellType == SpellType.Minion)
                {
                    if (detectedSpellInfo.Sender != null && detectedSpellInfo.Sender.IsHPBarRendered)
                    {
                        pos = detectedSpellInfo.Sender.ServerPosition;
                    }
                }

                if (detectedSpellInfo.SpellType == SpellType.GameObject)
                {
                    pos = detectedSpellInfo.Object != null && detectedSpellInfo.Object.IsValid ? detectedSpellInfo.Object.Position : detectedSpellInfo.Position;
                }

                if (detectedSpellInfo.SpellType == SpellType.Buff)
                {
                    if (detectedSpellInfo.Sender != null && detectedSpellInfo.Sender.IsHPBarRendered)
                    {
                        pos = new Vector3(detectedSpellInfo.Sender.ServerPosition.X, detectedSpellInfo.Sender.ServerPosition.Y, detectedSpellInfo.Sender.ServerPosition.Z);
                    }
                }

                /*if(pos == new Vector3(Game.CursorPos.X, Game.CursorPos.Y + i, Game.CursorPos.Z))
                    text.TextValue = detectedSpellInfo.Sender.Name() + ": " + detectedSpellInfo.SpellName + " : " + (detectedSpellInfo.EndTime - Game.Time).ToString("F1");*/

                if(pos == Game.CursorPos)
                    return;

                if (LastDrawPos == pos.WorldToScreen() && DetectedSpells.Count(s => menu.CheckBoxValue(s.ChampionName + s.SpellName) && s.Position.IsOnScreen()) > 1)
                {
                    i += 45f;
                    pos = new Vector3(pos.X, pos.Y + i, pos.Z);
                }

                var c = System.Drawing.Color.AliceBlue;
                
                LastDrawPos = pos.WorldToScreen();
                if (detectedSpellInfo.Sender != null)
                    c = detectedSpellInfo.Sender.IsEnemy ? System.Drawing.Color.Red : System.Drawing.Color.GreenYellow;
                else if (detectedSpellInfo.Object != null)
                    c = detectedSpellInfo.Object.IsEnemy ? System.Drawing.Color.Red : System.Drawing.Color.GreenYellow;

                if (pos.IsOnScreen() && menu.CheckBoxValue("screen") || !menu.CheckBoxValue("screen"))
                    trackettext.Draw(msg, c, pos.WorldToScreen());
            }
        }
    }
}