using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlastZone_Windows.MovementGrid;
using Microsoft.Xna.Framework.Input;

namespace BlastZone_Windows
{
    class PlayerInputController
    {
        Player controlling;

        Dictionary<string, Keys> keyIdentifiers;

        public PlayerInputController(Player ply)
        {
            controlling = ply;
            keyIdentifiers = new Dictionary<string, Keys>();
        }

        public void GetInput(KeyboardState k)
        {
            if (k.IsKeyDown(Keys.W) || k.IsKeyDown(Keys.Up))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP));
            }

            if (k.IsKeyDown(Keys.S) || k.IsKeyDown(Keys.Down))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN));
            }

            if (k.IsKeyDown(Keys.A) || k.IsKeyDown(Keys.Left))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT));
            }

            if (k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT));
            }
        }
    }
}
