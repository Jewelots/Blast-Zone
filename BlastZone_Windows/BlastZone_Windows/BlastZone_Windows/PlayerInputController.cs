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
        Dictionary<string, GamePadButtons> joyIdentifiers;

        bool useKey = true;

        public PlayerInputController(Player ply)
        {
            controlling = ply;
            keyIdentifiers = null;
            joyIdentifiers = null;
        }

        public void SetKeyIdentifiers(Keys up, Keys down, Keys left, Keys right, Keys bomb)
        {
            keyIdentifiers = new Dictionary<string, Keys>();
            keyIdentifiers["up"] = up;
            keyIdentifiers["down"] = down;
            keyIdentifiers["left"] = left;
            keyIdentifiers["right"] = right;
            keyIdentifiers["bomb"] = bomb;
        }

        public void SetJoyIdentifiers(GamePadButtons up, GamePadButtons down, GamePadButtons left, GamePadButtons right, GamePadButtons bomb)
        {
            joyIdentifiers = new Dictionary<string, GamePadButtons>();
            joyIdentifiers["up"] = up;
            joyIdentifiers["down"] = down;
            joyIdentifiers["left"] = left;
            joyIdentifiers["right"] = right;
            joyIdentifiers["bomb"] = bomb;

            useKey = false;
        }

        public void GetInput(KeyboardState k)
        {
            if (useKey)
            {
                if (keyIdentifiers == null) return; //No keys set

                if (k.IsKeyDown(keyIdentifiers["up"]))
                {
                    controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP));
                }

                if (k.IsKeyDown(keyIdentifiers["down"]))
                {
                    controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN));
                }

                if (k.IsKeyDown(keyIdentifiers["left"]))
                {
                    controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT));
                }

                if (k.IsKeyDown(keyIdentifiers["right"]))
                {
                    controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT));
                }
            }
        }
    }
}
