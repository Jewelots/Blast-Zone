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

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 drawOffset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);
            life += gameTime.ElapsedGameTime.TotalSeconds;

            if (life > maxLife) RemoveThis();

            double ratioAlong = life/maxLife;

            double throbScale = 0;
            throbScale = Math.Sin(life * 20 * (0.2 + (ratioAlong * ratioAlong) * 0.8)) / 5 * 2 * (0.2 + (ratioAlong) * 0.8);

            spriteBatch.Draw(
                bombTex, //Texture
                DrawPosition + drawOffset, //Position
                null, //Source Rect
                Color.White, //Color
                0, //Rotation
                new Vector2(GlobalGameData.tileSize / 2 - 0.5f, GlobalGameData.tileSize / 2 - 0.5f), //Offset
                GlobalGameData.drawRatio + (float)throbScale, //Scale
                SpriteEffects.None, //Sprite Effect (Flip)
                1f //Layer Depth
            );
        }
    }
}
