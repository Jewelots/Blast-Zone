using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlastZone_Windows.MovementGrid;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using SpritesheetAnimation;
using Microsoft.Xna.Framework.Audio;

namespace BlastZone_Windows
{
    class Player
    {
        GridNodeMover movement;
        AnimatedSprite playerAnimations;

        Func<int, int, int, int, bool> placeBombFunc;

        int power;
        int playerIndex;
        int bombCount;

        public bool IsDead;
        bool gameIsOver;

        SoundEffect footstepSound, bombPlaceSound;
        SoundEffectInstance footstepSoundInstance;

        public Player(GridNodeMap map, int playerIndex, Func<int, int, int, int, bool> placeBombFunc)
        {
            movement = new GridNodeMover(map);
            this.placeBombFunc = placeBombFunc;
            this.playerIndex = playerIndex;
        }

        public void Reset(int gx, int gy)
        {
            movement.SetPosition(gx, gy);
            movement.Reset();
            bombCount = 1;
            power = 1;
            IsDead = false;
            gameIsOver = false;
            footstepSoundInstance = footstepSound.CreateInstance();
            footstepSoundInstance.Volume = GlobalGameData.SFXVolume * 0.5f; //Quiet enough to not annoy hopefully
            footstepSoundInstance.IsLooped = true;
        }

        public void Move(MoveEvent moveEvent)
        {
            //Don't move if dead
            if (IsDead) return;

            if (!gameIsOver)
            {
                //Play footstep sound
                footstepSoundInstance.Play();
            }

            movement.QueueEvent(moveEvent);

            switch (moveEvent.moveEvent)
            {
                case MoveEvent.MoveEventType.MOVE_UP:
                    playerAnimations.ContinueAnimation("WalkUp");
                    break;
                case MoveEvent.MoveEventType.MOVE_DOWN:
                    playerAnimations.ContinueAnimation("WalkDown");
                    break;
                case MoveEvent.MoveEventType.MOVE_LEFT:
                    playerAnimations.ContinueAnimation("WalkLeft");
                    break;
                case MoveEvent.MoveEventType.MOVE_RIGHT:
                    playerAnimations.ContinueAnimation("WalkRight");
                    break;
            }
        }

        public void StopMove()
        {
            footstepSoundInstance.Stop();
        }

        public void GameOver()
        {
            gameIsOver = true;
            footstepSoundInstance.Stop();
        }

        public void LoadContent(ContentManager Content)
        {
            AnimationSheet playerSheet = new AnimationSheet();
            playerSheet.Load(Content, "Spritesheets\\players");
            playerAnimations = new AnimatedSprite(playerSheet, "StandDown");
            playerAnimations.SetTexture("player" + (playerIndex + 1));

            footstepSound = Content.Load<SoundEffect>("SFX/footsteps");
            bombPlaceSound = Content.Load<SoundEffect>("SFX/bombplace");
        }

        public void Update(GameTime gameTime)
        {
            //Don't update if dead
            if (IsDead) return;

            if (movement.IsEmpty())
            {
                playerAnimations.Stop();
            }

            movement.Update(gameTime);
            playerAnimations.position = movement.GetPosition() - new Vector2(0, 10);
            playerAnimations.Update(gameTime);
        }

        public void GetGridPosition(out int gx, out int gy)
        {
            movement.GetGridPosition(out gx, out gy);
        }

        public Vector2 GetPosition()
        {
            return movement.GetPosition();
        }

        public void PlaceBomb()
        {
            //Don't place bomb if dead
            if (IsDead) return;

            int gx, gy;

            movement.GetGridPosition(out gx, out gy);

            if (placeBombFunc(playerIndex, gx, gy, this.power))
            {
                SoundEffectInstance bombSoundInstance = bombPlaceSound.CreateInstance();
                bombSoundInstance.Volume = GlobalGameData.SFXVolume;
                bombSoundInstance.Play();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Don't draw if dead
            if (IsDead) return;

            playerAnimations.Draw(spriteBatch, GlobalGameData.drawRatio);
        }

        public int GetMaxBombs()
        {
            return bombCount;
        }

        public void BombUp()
        {
            bombCount += 1;
        }

        public void FireUp()
        {
            power += 1;
        }

        public void SpeedUp()
        {
            movement.AddSpeed();
        }
    }
}
