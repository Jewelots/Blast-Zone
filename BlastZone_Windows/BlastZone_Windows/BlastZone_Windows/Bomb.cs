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
        //Animation idleAnimation;
        Texture2D bombTex;

        public Bomb(Vector2 tilePos, Texture2D tex) //replace with Animation
            : base(tilePos)
        {
            bombTex = tex;

            Solid = true;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize, Vector2 levelPos)
        {
            //idleAnimation.Draw(spriteBatch, gameTime, DrawPosition + levelPos, Color.White);
            spriteBatch.Draw(bombTex, DrawPosition + levelPos, null, Color.White, 0, new Vector2(), drawRatio, SpriteEffects.None, 1f);
        }
    }
}
