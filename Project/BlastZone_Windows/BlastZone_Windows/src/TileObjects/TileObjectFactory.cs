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

using SpritesheetAnimation;

namespace BlastZone_Windows
{
    /// <summary>
    /// A factory to create tile objects and keep track of content
    /// </summary>
    class TileObjectFactory
    {
        Texture2D bombTex, blockTex;

        Texture2D[] powerupTex;

        AnimatedSprite destroySoftblockAnimation;

        SoundEffect powerupSound, bombExplosionSound;
        SoundEffectInstance powerupSoundInstance, bombExplosionSoundInstance;

        bool loaded = false;

        public void LoadContent(ContentManager Content)
        {
            bombTex = Content.Load<Texture2D>("Images/Game/bomb");
            blockTex = Content.Load<Texture2D>("Images/Game/softblocks");
            loaded = true;

            powerupTex = new Texture2D[3];

            powerupTex[0] = Content.Load<Texture2D>("Images/Game/bombup");
            powerupTex[1] = Content.Load<Texture2D>("Images/Game/fireup");
            powerupTex[2] = Content.Load<Texture2D>("Images/Game/speedup");

            AnimationSheet destroySoftblockAnimationSheet = new AnimationSheet();
            destroySoftblockAnimationSheet.Load(Content, "Spritesheets\\softblocks");
            destroySoftblockAnimation = new AnimatedSprite(destroySoftblockAnimationSheet, "Block1");

            powerupSound = Content.Load<SoundEffect>("SFX/powerup");
            bombExplosionSound = Content.Load<SoundEffect>("SFX/bombexplode");

            powerupSoundInstance = powerupSound.CreateInstance();
            bombExplosionSoundInstance = bombExplosionSound.CreateInstance();

            powerupSoundInstance.Volume = GlobalGameData.SFXVolume;
            bombExplosionSoundInstance.Volume = GlobalGameData.SFXVolume;
        }

        public Bomb CreateBomb(TileObjectManager manager, int tilePosX, int tilePosY, int power)
        {
            if (!loaded) return null;

            return new Bomb(manager, tilePosX, tilePosY, bombTex, power, bombExplosionSoundInstance);
        }

        public SoftBlock CreateSoftBlock(TileObjectManager manager, int tilePosX, int tilePosY, int softblockType)
        {
            if (!loaded) return null;

            return new SoftBlock(manager, tilePosX, tilePosY, blockTex, destroySoftblockAnimation, softblockType);
        }

        public Powerup CreatePowerup(TileObjectManager manager, int tilePosX, int tilePosY, PowerupType pType)
        {
            if (!loaded) return null;

            Texture2D pTex;

            switch (pType)
            {
                case PowerupType.BOMB_UP:
                    pTex = powerupTex[0];
                    break;
                case PowerupType.FIRE_UP:
                    pTex = powerupTex[1];
                    break;
                case PowerupType.SPEED_UP:
                    pTex = powerupTex[2];
                    break;
                default:
                    pTex = powerupTex[0];
                    break;
            }

            return new Powerup(manager, tilePosX, tilePosY, pType, pTex, powerupSoundInstance);
        }
    }
}
