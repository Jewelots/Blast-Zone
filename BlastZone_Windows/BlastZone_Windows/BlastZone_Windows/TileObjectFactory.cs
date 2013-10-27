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
    class TileObjectFactory
    {
        Texture2D bombTex;
        //Animation bombIdleAnimation;

        bool loaded = false;

        public void LoadContent(ContentManager Content)
        {
            bombTex = Content.Load<Texture2D>("bomb");
            loaded = true;
        }

        public Bomb CreateBomb(Vector2 tilePos)
        {
            if (!loaded) return null;

            return new Bomb(tilePos, bombTex);
        }
    }
}
