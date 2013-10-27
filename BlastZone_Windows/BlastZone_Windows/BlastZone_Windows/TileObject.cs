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
    abstract class TileObject
    {
        public bool Solid { get; protected set; }
        public Vector2 Position { get; private set; }

        protected int tileSize = 16;
        protected int drawRatio = 3;

        protected Vector2 DrawPosition { get { return Position * tileSize * drawRatio; } }

        public delegate void PlayerCollisionHandler();
        public delegate void FireSpreadHandler();
        public event PlayerCollisionHandler OnPlayerCollision;
        public event FireSpreadHandler OnFireSpread;

        protected void PlayerCollision()
        {
            if (OnPlayerCollision != null)
                OnPlayerCollision();
        }

        protected void FireSpread()
        {
            if (OnFireSpread != null)
                OnFireSpread();
        }

        public TileObject(Vector2 tilePos)
        {
            Position = tilePos;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize, Vector2 levelPos);
    }
}
