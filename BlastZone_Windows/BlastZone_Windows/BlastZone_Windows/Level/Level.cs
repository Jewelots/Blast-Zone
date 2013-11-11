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

namespace BlastZone_Windows.Level
{
    /// <summary>
    /// Encapsulates the aesthetic tiles and all entities that interact.
    /// The game itself.
    /// </summary>
    class Level
    {
        /// <summary>
        /// 2D array of bools to designate what's solid and what's not
        /// </summary>
        bool[,] solidArea;

        TileObjectManager tileObjectManager;
        public FireManager fireManager;

        LevelAesthetics aesthetics;

        public Level()
        {
            int gridSizeX = GlobalGameData.gridSizeX;
            int gridSizeY = GlobalGameData.gridSizeY;

            aesthetics = new LevelAesthetics(gridSizeX, gridSizeY);

            tileObjectManager = new TileObjectManager(gridSizeX, gridSizeY, this);
            fireManager = new FireManager(gridSizeX, gridSizeY, tileObjectManager);

            solidArea = new bool[gridSizeX, gridSizeY];
        }

        public void LoadContent(ContentManager Content)
        {
            aesthetics.LoadContent(Content);
            aesthetics.GenerateTiles(solidArea);

            tileObjectManager.LoadContent(Content);
            ///////////////////////////////////////
            fireManager.LoadContent(Content); /////REPLACE LATER
            ///////////////////////////////////////

            fireManager.SetSolidArea(solidArea);
        }

        public void Reset()
        {
            tileObjectManager.Reset();
        }

        public void Update(GameTime gameTime)
        {
            tileObjectManager.Update(gameTime);

            fireManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Begin sprite batch with nearest neighbor interpolation state enabled (no filtering, stops muddy pixels)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            aesthetics.Draw(spriteBatch, gameTime);
            tileObjectManager.Draw(spriteBatch, gameTime);

            /////////////////////////
            fireManager.Draw(spriteBatch, gameTime); /////REPLACE LATER
            /////////////////////////

            spriteBatch.End();
        }
    }
}
