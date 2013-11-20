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
    class SoftBlock : TileObject
    {
        Texture2D blockTex;
        Rectangle sourceRect;

        public SoftBlock(TileObjectManager manager, int tilePosX, int tilePosY, Texture2D tex, int type)
            : base(manager, tilePosX, tilePosY)
        {
            blockTex = tex;
            Solid = true;

            //Hook to destroy when burnt
            OnFireSpread += Destroy;

            sourceRect = new Rectangle(0, type * GlobalGameData.tileSize, GlobalGameData.tileSize, GlobalGameData.tileSize);
        }

        public override void Update(GameTime gameTime)
        {
        }

        void Destroy()
        {
            RemoveThis();
            manager.SpawnPowerup(tilePositionX, tilePositionY);
            //Spawn effect
            //manager.level.fireManager.ExplodeFrom(tilePositionX, tilePositionY, 3);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw offset to center sprite
            Vector2 drawOffset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);
            Vector2 drawPos = DrawPosition + drawOffset;

            drawPos.X = (int)Math.Floor(drawPos.X);
            drawPos.Y = (int)Math.Floor(drawPos.Y);

            spriteBatch.Draw(
                blockTex, //Texture
                drawPos, //Position
                sourceRect, //Source Rect
                Color.White, //Color
                0, //Rotation
                new Vector2(GlobalGameData.tileSize / 2, GlobalGameData.tileSize / 2), //Offset
                GlobalGameData.drawRatio, //Scale
                SpriteEffects.None, //Sprite Effect (Flip)
                1f //Layer Depth
            );
        }
    }
}
