using System;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using Rectangle = SharpDX.Rectangle;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace KappaUtility.Common.Texture
{
    public class ChampionTexture
    {
        public ChampionTexture(AIHeroClient Champion, List<SharpDX.Direct3D9.Texture> Texture, List<Image> Images, Sprite hp, Sprite mp, Sprite xp, Sprite emp, Sprite recall, Sprite teleport)
        {
            this.Hero = Champion;

            var Texturearry = Texture.ToArray();
            var imagesarray = Images.ToArray();

            this.HeroIcon = new Sprite(Texturearry[0])
            { Rectangle = new Rectangle(0, 0, imagesarray[0].Width, imagesarray[0].Height) };

            this.HeroIconDead = new Sprite(Texturearry[1])
            { Rectangle = new Rectangle(0, 0, imagesarray[1].Width, imagesarray[1].Height) };

            var q = new Sprite(Texturearry[2]) { Rectangle = new Rectangle(0, 0, imagesarray[2].Width, imagesarray[2].Height) };
            var qnr = new Sprite(Texturearry[3]) { Rectangle = new Rectangle(0, 0, imagesarray[3].Width, imagesarray[3].Height) };
            this.Q = new SpriteSlot(q, qnr, SpellSlot.Q);

            var w = new Sprite(Texturearry[4]) { Rectangle = new Rectangle(0, 0, imagesarray[4].Width, imagesarray[4].Height) };
            var wnr = new Sprite(Texturearry[5]) { Rectangle = new Rectangle(0, 0, imagesarray[5].Width, imagesarray[5].Height) };
            this.W = new SpriteSlot(w, wnr, SpellSlot.W);

            var e = new Sprite(Texturearry[6]) { Rectangle = new Rectangle(0, 0, imagesarray[6].Width, imagesarray[6].Height) };
            var enr = new Sprite(Texturearry[7]) { Rectangle = new Rectangle(0, 0, imagesarray[7].Width, imagesarray[7].Height) };
            this.E = new SpriteSlot(e, enr, SpellSlot.E);

            var r = new Sprite(Texturearry[8]) { Rectangle = new Rectangle(0, 0, imagesarray[8].Width, imagesarray[8].Height) };
            var rnr = new Sprite(Texturearry[9]) { Rectangle = new Rectangle(0, 0, imagesarray[9].Width, imagesarray[9].Height) };
            this.R = new SpriteSlot(r, rnr, SpellSlot.R);

            var sum1 = new Sprite(Texturearry[10]) { Rectangle = new Rectangle(0, 0, imagesarray[10].Width, imagesarray[10].Height) };
            var sum1nr = new Sprite(Texturearry[11]) { Rectangle = new Rectangle(0, 0, imagesarray[11].Width, imagesarray[11].Height) };
            this.Summoner1 = new SpriteSlot(sum1, sum1nr, SpellSlot.Summoner1);

            var sum2 = new Sprite(Texturearry[12]) { Rectangle = new Rectangle(0, 0, imagesarray[12].Width, imagesarray[12].Height) };
            var sum2nr = new Sprite(Texturearry[13]) { Rectangle = new Rectangle(0, 0, imagesarray[13].Width, imagesarray[13].Height) };
            this.Summoner2 = new SpriteSlot(sum2, sum2nr, SpellSlot.Summoner2);

            int barwidth = imagesarray[2].Width + imagesarray[0].Width;
            int barheight = (int)(imagesarray[0].Height * 0.07);

            this.HPBar = new Sprite(() => hp.Texture) { Rectangle = new Rectangle(0, 0, barwidth, barheight) };
            this.MPBar = new Sprite(() => mp.Texture) { Rectangle = new Rectangle(0, 0, barwidth, barheight) };
            this.XPBar = new Sprite(() => xp.Texture) { Rectangle = new Rectangle(0, 0, barwidth, barheight) };
            this.EmptyBar = new Sprite(() => emp.Texture) { Rectangle = new Rectangle(0, 0, barwidth, barheight) };

            int recallwidth = (int)(barheight * 1.3f);
            int recallheight = (int)(imagesarray[0].Height * 0.988f); // Same as hero icon Height
            this.Recall = new Sprite(() => recall.Texture) { Rectangle = new Rectangle(0, 0, recallwidth, recallheight) };
            this.Teleport = new Sprite(() => teleport.Texture) { Rectangle = new Rectangle(0, 0, recallwidth, recallheight) };

            this.Spells = new[] { this.R, this.Summoner1, this.Summoner2 };
            this.AllSpells = new[] { this.Q, this.W, this.E, this.R, this.Summoner1, this.Summoner2 };

            foreach(var i in Images)
                i.Dispose();
        }

        public AIHeroClient Hero;
        public Sprite HeroIcon;
        public Sprite HeroIconDead;
        public SpriteSlot Q;
        public SpriteSlot W;
        public SpriteSlot E;
        public SpriteSlot R;
        public SpriteSlot Summoner1;
        public SpriteSlot Summoner2;
        public SpriteSlot[] Spells;
        public SpriteSlot[] AllSpells;
        public Sprite HPBar;
        public Sprite MPBar;
        public Sprite XPBar;
        public Sprite EmptyBar;
        public Sprite Recall;
        public Sprite Teleport;
    }

    public class SpriteSlot
    {
        public SpriteSlot(Sprite Rdyspr, Sprite NRspr, SpellSlot slot)
        {
            this.ReadySprite = Rdyspr;
            this.NotReadySprite = NRspr;
            this.Slot = slot;
        }
        public Sprite ReadySprite;
        public Sprite NotReadySprite;
        public SpellSlot Slot;
    }
}
