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
    /// <summary>
    /// Abstract class used for objects that snap to the grid and can be solid or not
    /// </summary>
    abstract class TileObject
    {
        protected int tilePositionX, tilePositionY;
        private TileObjectManager manager;

        public bool Solid { get; protected set; }
        public Vector2 Position
        {
            get
            {
                return new Vector2(tilePositionX, tilePositionY);
            }

            private set
            {
                tilePositionX = (int)value.X; tilePositionY = (int)value.Y;
            }
        }

        protected Vector2 DrawPosition { get { return Position * GlobalGameData.tileSize * GlobalGameData.drawRatio; } }

        public delegate void PlayerCollisionHandler();
        public delegate void FireSpreadHandler();
        /// <summary>
        /// Event called when player moves to the tile this TileObject resides in
        /// </summary>
        public event PlayerCollisionHandler OnPlayerCollision;
        /// <summary>
        /// Event called when fire spreads to the tile this TileObject resides in
        /// </summary>
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

        public TileObject(TileObjectManager manager, int tilePosX, int tilePosY)
        {
            this.manager = manager;

            tilePositionX = tilePosX;
            tilePositionY = tilePosY;
        }

        protected void RemoveThis()
        {
            manager.RemoveAt(tilePositionX, tilePositionY);
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
