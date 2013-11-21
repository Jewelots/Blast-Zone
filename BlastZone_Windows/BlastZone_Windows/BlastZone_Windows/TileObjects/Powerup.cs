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
    enum PowerupType
    {
        BOMB_UP,
        FIRE_UP,
        SPEED_UP
    }

    class Powerup : TileObject
    {
        Texture2D powerupTex;
        PowerupType pType;
        SoundEffectInstance powerupSoundInstance;

        public Powerup(TileObjectManager manager, int tilePosX, int tilePosY, PowerupType pType, Texture2D tex, SoundEffectInstance powerupSound)
            : base(manager, tilePosX, tilePosY)
        {
            this.pType = pType;
            this.powerupTex = tex;
            this.powerupSoundInstance = powerupSound;

            Solid = false;

            //Hook to destroy when burnt
            OnFireSpread += Destroy;
            OnPlayerCollision += PlayerCollect;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public PowerupType GetPowerType()
        {
            return pType;
        }

        void Destroy()
        {
            RemoveThis();
            //Spawn effect
        }

        void PlayerCollect(Player p)
        {
            switch (pType)
            {
                case PowerupType.BOMB_UP:
                    p.BombUp();
                    break;
                case PowerupType.FIRE_UP:
                    p.FireUp();
                    break;
                case PowerupType.SPEED_UP:
                    p.SpeedUp();
                    break;
            }

            powerupSoundInstance.Play();

            RemoveThis();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw offset to center sprite
            Vector2 drawOffset = new Vector2(GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio, GlobalGameData.tileSize / 2 * GlobalGameData.drawRatio);
            Vector2 drawPos = DrawPosition + drawOffset;

            drawPos.X = (int)Math.Floor(drawPos.X);
            drawPos.Y = (int)Math.Floor(drawPos.Y);

            spriteBatch.Draw(
                powerupTex, //Texture
                drawPos, //Position
                null, //Source Rect
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
