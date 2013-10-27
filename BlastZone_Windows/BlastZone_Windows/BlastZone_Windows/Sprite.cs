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
using System.Xml;
using System.Xml.Linq;

namespace BlastZone_Windows
{
    class Sprite
    {
        protected Texture2D tex;

        public Sprite()
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Color tint)
        {
            spriteBatch.Draw(tex, position, tint);
        }
    }
}
