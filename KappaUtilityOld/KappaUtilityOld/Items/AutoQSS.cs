using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using KappaUtilityOld.Common;

namespace KappaUtilityOld.Items
{
    internal class AutoQSS
    {
        public static Spell.Active Cleanse;

        protected static readonly Item Mercurial_Scimitar = new Item(3139);

        protected static readonly Item Quicksilver_Sash = new Item(3140);

        public static readonly Item Dervish_Blade = new Item(3137);

        public static readonly Item Mikaels_Crucible = new Item(3222);

        protected static bool loaded = false;

        public static Menu QssMenu { get; private set; }

        internal static void OnLoad()
        {
            QssMenu = Load.UtliMenu.AddSubMenu("AutoQSS");
            QssMenu.AddGroupLabel("AutoQSS Settings");
            QssMenu.Checkbox("enable", "Enable");
            QssMenu.Checkbox("Mercurial", "Use Mercurial Scimitar");
            QssMenu.Checkbox("Quicksilver", "Use Quicksilver Sash");
            QssMenu.Checkbox("Dervish_Blade", "Use Dervish Blade");
            QssMenu.Checkbox("Mikaels_Crucible", "Use Mikaels Crucible");
            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerBoost")) != null)
            {
                QssMenu.Checkbox("Cleanse", "Use Cleanse Spell");
                Cleanse = new Spell.Active(Player.Instance.GetSpellSlotFromName("SummonerBoost"));
            }

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("Debuffs Settings:");
            QssMenu.Checkbox("blind", "Use On Blinds?");
            QssMenu.Checkbox("charm", "Use On Charms?");
            QssMenu.Checkbox("disarm", "Use On Disarm?");
            QssMenu.Checkbox("fear", "Use On Fear?");
            QssMenu.Checkbox("frenzy", "Use On Frenzy?");
            QssMenu.Checkbox("silence", "Use On Silence?");
            QssMenu.Checkbox("snare", "Use On Snare?");
            QssMenu.Checkbox("sleep", "Use On Sleep?");
            QssMenu.Checkbox("stun", "Use On Stuns?");
            QssMenu.Checkbox("supperss", "Use On Supperss?");
            QssMenu.Checkbox("slow", "Use On Slows?");
            QssMenu.Checkbox("knockup", "Use On Knock Ups?");
            QssMenu.Checkbox("knockback", "Use On Knock Backs?");
            QssMenu.Checkbox("nearsight", "Use On NearSight?");
            QssMenu.Checkbox("root", "Use On Roots?");
            QssMenu.Checkbox("tunt", "Use On Taunts?");
            QssMenu.Checkbox("poly", "Use On Polymorph?");
            QssMenu.Checkbox("poison", "Use On Poisons?");

            QssMenu.AddSeparator();
            QssMenu.AddGroupLabel("Ults Settings:");
            QssMenu.Checkbox("liss", "Use On Lissandra Ult?");
            QssMenu.Checkbox("naut", "Use On Nautilus Ult?");
            QssMenu.Checkbox("zed", "Use On Zed Ult?");
            QssMenu.Checkbox("vlad", "Use On Vlad Ult?");
            QssMenu.Checkbox("fizz", "Use On Fizz Ult?");
            QssMenu.Checkbox("fiora", "Use On Fiora Ult?");
            QssMenu.AddSeparator();
            QssMenu.Slider("hp", "Use Only When HP is Under [{0}%]", 30);
            QssMenu.Slider("human", "Humanizer Delay [{0}]", 150, 0, 1500);
            QssMenu.Slider("Rene", "[{0}] Enemies or more Near to Cast", 1, 0, 5);
            QssMenu.Slider("enemydetect", "Enemies Detect InRange [{0}]", 1000, 0, 2000);
            loaded = true;

            Obj_AI_Base.OnBuffGain += OnBuffGain;
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!loaded)
            {
                return;
            }

            if (QssMenu.GetCheckbox("enable"))
            {
                if (sender.IsMe)
                {
                    var debuff = (QssMenu.GetCheckbox("charm") && (args.Buff.Type == BuffType.Charm || Player.Instance.HasBuffOfType(BuffType.Charm)))
                                 || (QssMenu.GetCheckbox("tunt")
                                     && (args.Buff.Type == BuffType.Taunt || Player.Instance.HasBuffOfType(BuffType.Taunt)))
                                 || (QssMenu.GetCheckbox("stun") && (args.Buff.Type == BuffType.Stun || Player.Instance.HasBuffOfType(BuffType.Stun)))
                                 || (QssMenu.GetCheckbox("fear") && (args.Buff.Type == BuffType.Fear || Player.Instance.HasBuffOfType(BuffType.Fear)))
                                 || (QssMenu.GetCheckbox("silence")
                                     && (args.Buff.Type == BuffType.Silence || Player.Instance.HasBuffOfType(BuffType.Silence)))
                                 || (QssMenu.GetCheckbox("snare")
                                     && (args.Buff.Type == BuffType.Snare || Player.Instance.HasBuffOfType(BuffType.Snare)))
                                 || (QssMenu.GetCheckbox("supperss")
                                     && (args.Buff.Type == BuffType.Suppression || Player.Instance.HasBuffOfType(BuffType.Suppression)))
                                 || (QssMenu.GetCheckbox("sleep")
                                     && (args.Buff.Type == BuffType.Sleep || Player.Instance.HasBuffOfType(BuffType.Sleep)))
                                 || (QssMenu.GetCheckbox("poly")
                                     && (args.Buff.Type == BuffType.Polymorph || Player.Instance.HasBuffOfType(BuffType.Polymorph)))
                                 || (QssMenu.GetCheckbox("frenzy")
                                     && (args.Buff.Type == BuffType.Frenzy || Player.Instance.HasBuffOfType(BuffType.Frenzy)))
                                 || (QssMenu.GetCheckbox("disarm")
                                     && (args.Buff.Type == BuffType.Disarm || Player.Instance.HasBuffOfType(BuffType.Disarm)))
                                 || (QssMenu.GetCheckbox("nearsight")
                                     && (args.Buff.Type == BuffType.NearSight || Player.Instance.HasBuffOfType(BuffType.NearSight)))
                                 || (QssMenu.GetCheckbox("knockback")
                                     && (args.Buff.Type == BuffType.Knockback || Player.Instance.HasBuffOfType(BuffType.Knockback)))
                                 || (QssMenu.GetCheckbox("knockup")
                                     && (args.Buff.Type == BuffType.Knockup || Player.Instance.HasBuffOfType(BuffType.Knockup)))
                                 || (QssMenu.GetCheckbox("slow") && (args.Buff.Type == BuffType.Slow || Player.Instance.HasBuffOfType(BuffType.Slow)))
                                 || (QssMenu.GetCheckbox("poison")
                                     && (args.Buff.Type == BuffType.Poison || Player.Instance.HasBuffOfType(BuffType.Poison)))
                                 || (QssMenu.GetCheckbox("blind")
                                     && (args.Buff.Type == BuffType.Blind || Player.Instance.HasBuffOfType(BuffType.Blind)))
                                 || (QssMenu.GetCheckbox("zed") && args.Buff.Name == "zedrtargetmark")
                                 || (QssMenu.GetCheckbox("vlad") && args.Buff.Name == "vladimirhemoplaguedebuff")
                                 || (QssMenu.GetCheckbox("liss") && args.Buff.Name == "LissandraREnemy2")
                                 || (QssMenu.GetCheckbox("fizz") && args.Buff.Name == "fizzmarinerdoombomb")
                                 || (QssMenu.GetCheckbox("naut") && args.Buff.Name == "nautilusgrandlinetarget")
                                 || (QssMenu.GetCheckbox("fiora") && args.Buff.Name == "fiorarmark");
                    var enemys = QssMenu.GetSlider("Rene");
                    var hp = QssMenu.GetSlider("hp");
                    var enemysrange = QssMenu.GetSlider("enemydetect");
                    var countenemies = Helpers.CountEnemies(enemysrange);
                    var delay = QssMenu.GetSlider("human");
                    if (debuff && Player.Instance.HealthPercent <= hp && countenemies >= enemys)
                    {
                        Core.DelayAction(QssCast, delay);
                    }
                }
            }
        }

        public static void QssCast()
        {
            if (Quicksilver_Sash.IsOwned() && Quicksilver_Sash.IsReady() && QssMenu.GetCheckbox("Quicksilver"))
            {
                Quicksilver_Sash.Cast();
            }

            if (Mercurial_Scimitar.IsOwned() && Mercurial_Scimitar.IsReady() && QssMenu.GetCheckbox("Mercurial"))
            {
                Mercurial_Scimitar.Cast();
            }

            if (Dervish_Blade.IsOwned() && Dervish_Blade.IsReady() && QssMenu.GetCheckbox("Dervish_Blade"))
            {
                Dervish_Blade.Cast();
            }

            if (Cleanse != null)
            {
                if (QssMenu.GetCheckbox("Cleanse") && Cleanse.IsReady())
                {
                    Cleanse.Cast();
                }
            }
        }
    }
}