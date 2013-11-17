using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlastZone_Windows.MovementGrid;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace BlastZone_Windows
{
    class Player
    {
        GridNodeMover movement;
        Texture2D playerTex; //replace with animation

        Action<int, int, int> placeBombFunc;

        int power = 1;

        public Player(GridNodeMap map, int gx, int gy, Action<int, int, int> placeBombFunc)
        {
            movement = new GridNodeMover(map, gx, gy);
            this.placeBombFunc = placeBombFunc;
        }

        public void Move(MoveEvent moveEvent)
        {
            movement.QueueEvent(moveEvent);
        }

        public void LoadContent(ContentManager Content)
        {
            playerTex = Content.Load<Texture2D>("Images/Game/bomb");
        }

        public void Update(GameTime gameTime)
        {
            movement.Update(gameTime);
        }

        public void PlaceBomb()
        {
            int gx, gy;

            movement.GetGridPosition(out gx, out gy);

            placeBombFunc(gx, gy, this.power); //x, y, power
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(playerTex, movement.GetPosition(), null, Color.White, 0f, new Vector2(GlobalGameData.tileSize / 2 - 0.5f, GlobalGameData.tileSize / 2 - 0.5f), GlobalGameData.drawRatio, SpriteEffects.None, 1f);
        }
    }
}
