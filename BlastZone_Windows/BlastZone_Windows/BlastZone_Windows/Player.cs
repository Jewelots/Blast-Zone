using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlastZone_Windows.MovementGrid;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using SpritesheetAnimation;

namespace BlastZone_Windows
{
    class Player
    {
        GridNodeMover movement;
        AnimatedSprite playerAnimations;

        Action<int, int, int, int> placeBombFunc;

        int power;
        int playerIndex;
        int bombCount;

        public bool IsDead;

        public Player(GridNodeMap map, int playerIndex, Action<int, int, int, int> placeBombFunc)
        {
            movement = new GridNodeMover(map);
            this.placeBombFunc = placeBombFunc;
            this.playerIndex = playerIndex;
        }

        public void Reset(int gx, int gy)
        {
            movement.SetPosition(gx, gy);
            bombCount = 1;
            power = 1;
            IsDead = false;
        }

        public void Move(MoveEvent moveEvent)
        {
            //Don't move if dead
            if (IsDead) return;

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

        public void LoadContent(ContentManager Content)
        {
            AnimationSheet playerSheet = new AnimationSheet();
            playerSheet.Load(Content, "Spritesheets\\players");
            playerAnimations = new AnimatedSprite(playerSheet, "StandDown");
        }

        public void Update(GameTime gameTime)
        {
            //Don't update if dead
            if (IsDead) return;

            movement.Update(gameTime);
            playerAnimations.position = movement.GetPosition() - new Vector2(0, 10);
            playerAnimations.Update(gameTime);
        }

        public void GetGridPosition(out int gx, out int gy)
        {
            movement.GetGridPosition(out gx, out gy);
        }

        public void PlaceBomb()
        {
            //Don't place bomb if dead
            if (IsDead) return;

            int gx, gy;

            movement.GetGridPosition(out gx, out gy);

            placeBombFunc(playerIndex, gx, gy, this.power); //x, y, power
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

        public void GetAllOccupiedPositions(out int[] tx, out int[] ty)
        {
            movement.GetAllOccupiedPositions(out tx, out ty);
        }
    }
}
