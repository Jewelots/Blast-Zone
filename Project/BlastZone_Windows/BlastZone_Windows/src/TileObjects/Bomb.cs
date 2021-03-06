﻿using System;
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
        SoundEffectInstance explodeSoundInstance;

        int power;

        public Bomb(TileObjectManager manager, int tilePosX, int tilePosY, Texture2D tex, int power, SoundEffectInstance explodeSound)
            : base(manager, tilePosX, tilePosY)
        {
            bombTex = tex;
            Solid = true;

            this.explodeSoundInstance = explodeSound;

            lifeTimer = new EventTimer(0, 4);

            this.power = power;

            //Hook to explode when life ends
            lifeTimer.OnEnd += Explode;
            OnFireSpread += Explode;
        }

        public override void Update(GameTime gameTime)
        {
            lifeTimer.Update(gameTime);

            Vector2 levelOffset = new Vector2(GlobalGameData.windowWidth / 2 - GlobalGameData.levelSizeX / 2, GlobalGameData.windowHeight / 2 - GlobalGameData.levelSizeY / 2);
            //Managers.ParticleManager.Emit("BombSmoke", levelOffset + DrawPosition + new Vector2(48, 2));
            Managers.ParticleManager.AddEmissionPoint("BombSmoke", levelOffset + DrawPosition + new Vector2(48, 2));
        }

        void Explode()
        {
            explodeSoundInstance.Play();
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
                Color.Lerp(Color.White, new Color(255, 128, 128), (float)Math.Pow((float)percentComplete, 2)), //Color
                0, //Rotation
                new Vector2(GlobalGameData.tileSize / 2 - 0.5f, GlobalGameData.tileSize / 2 - 0.5f), //Offset
                GlobalGameData.drawRatio + (float)throbScale, //Scale
                SpriteEffects.None, //Sprite Effect (Flip)
                1f //Layer Depth
            );
        }
    }
}
