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
    /// A factory to create tile objects and keep track of content
    /// </summary>
    class TileObjectFactory
    {
        Texture2D bombTex, blockTex;

        bool loaded = false;

        public void LoadContent(ContentManager Content)
        {
            bombTex = Content.Load<Texture2D>("Images/Game/bomb");
            blockTex = Content.Load<Texture2D>("Images/Game/softblock");
            loaded = true;
        }

        public Bomb CreateBomb(TileObjectManager manager, int tilePosX, int tilePosY, int power)
        {
            if (!loaded) return null;

            return new Bomb(manager, tilePosX, tilePosY, bombTex, power);
        }

        public SoftBlock CreateSoftBlock(TileObjectManager manager, int tilePosX, int tilePosY)
        {
            if (!loaded) return null;

            return new SoftBlock(manager, tilePosX, tilePosY, blockTex);
        }
    }
}
