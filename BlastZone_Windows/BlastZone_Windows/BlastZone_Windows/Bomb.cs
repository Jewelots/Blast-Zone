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

        EventTimer lifeTimer;

        int power;

        public Bomb(TileObjectManager manager, int tilePosX, int tilePosY, Texture2D tex, int power) //replace with Animation
            : base(manager, tilePosX, tilePosY)
        {
            bombTex = tex;
            Solid = true;

            lifeTimer = new EventTimer(0, 5);

            this.power = power;

            //Hook to explode when life ends
            lifeTimer.OnEnd += Explode;
            OnFireSpread += Explode;
        }

        public override void Update(GameTime gameTime)
        {
            lifeTimer.Update(gameTime);
        }

        void Explode()
        {
            RemoveThis();
            manager.level.fireManager.ExplodeFrom(tilePositionX, tilePositionY, power);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            double percentComplete = lifeTimer.GetRatio();

            //Calculate throb scale with a sin wave
            double throbScale = 0;
            throbScale = Math.Sin(lifeTimer.GetCurrentTime() * 20 * (0.2 + (percentComplete * percentComplete) * 0.8)) / 5 * 2 * (0.2 + (percentComplete) * 0.8);

            //Draw offset to center sprite
            Vector2 drawOffset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);

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
