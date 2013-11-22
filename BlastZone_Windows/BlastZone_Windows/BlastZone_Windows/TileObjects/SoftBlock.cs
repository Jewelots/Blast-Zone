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

using SpritesheetAnimation;

namespace BlastZone_Windows
{
    class SoftBlock : TileObject
    {
        Texture2D blockTex;
        Rectangle sourceRect;

        AnimatedSprite destroyAnimation;

        int blockType;
        bool isDead;

        public SoftBlock(TileObjectManager manager, int tilePosX, int tilePosY, Texture2D tex, AnimatedSprite destroyAnimation, int type)
            : base(manager, tilePosX, tilePosY)
        {
            blockTex = tex;
            Solid = true;
            isDead = false;

            //Hook to destroy when burnt
            OnFireSpread += Destroy;

            this.blockType = type;
            sourceRect = new Rectangle(0, type * GlobalGameData.tileSize, GlobalGameData.tileSize, GlobalGameData.tileSize);

            this.destroyAnimation = destroyAnimation;
        }

        public override void Update(GameTime gameTime)
        {
        }

        void Destroy()
        {
            isDead = true;
            manager.SpawnPowerup(tilePositionX, tilePositionY);

            //Draw offset to center sprite
            Vector2 drawOffset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);
            Vector2 drawPos = DrawPosition + drawOffset;

            AnimatedSprite newAnim = new AnimatedSprite(destroyAnimation);
            newAnim.SetAnimation("Block" + (blockType + 1));
            manager.level.floatingAnimationManager.Add(newAnim, drawPos);

            newAnim.OnAnimationEnd += RemoveThis;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Don't draw if dead
            if (isDead) return;

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
