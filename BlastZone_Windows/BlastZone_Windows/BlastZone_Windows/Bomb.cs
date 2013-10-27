using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BlastZone_Windows
{
    class Bomb : TileObject
    {
        Texture2D bombTex;

        double life;
        double maxLife = 5f;

        public Bomb(TileObjectManager manager, int tilePosX, int tilePosY, Texture2D tex) //replace with Animation
            : base(manager, tilePosX, tilePosY)
        {
            bombTex = tex;
            Solid = true;
            life = 0f;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize, Vector2 levelPos)
        {
            life += gameTime.ElapsedGameTime.TotalSeconds;

            if (life > maxLife) RemoveThis();

            double ratioAlong = life/maxLife;

            double throbScale = 0;
            throbScale = Math.Sin(life * 20 * (0.2 + (ratioAlong * ratioAlong) * 0.8)) / 5 * 2 * (0.2 + (ratioAlong) * 0.8);

            spriteBatch.Draw(bombTex, DrawPosition + levelPos, null, Color.White, 0, new Vector2(tileSize / 2 - 0.5f, tileSize / 2 - 0.5f), drawRatio + (float)throbScale, SpriteEffects.None, 1f);
        }
    }
}
