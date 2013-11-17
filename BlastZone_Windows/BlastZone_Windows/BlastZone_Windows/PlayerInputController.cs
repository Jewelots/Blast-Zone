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

        Buttons padBombButton;

        KeyboardState lastKeyState;
        GamePadState lastPadState;

        bool useKey = true;

        public PlayerInputController(Player ply)
        {
            controlling = ply;
            keyIdentifiers = null;
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

        public void SetJoyIdentifiers(Buttons bomb)
        {
            padBombButton = bomb;

            useKey = false;
        }

        public void GetKeyInput(KeyboardState k)
        {
            if (!useKey) return;

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

            if (KeyJustPressed(keyIdentifiers["bomb"], k))
            {
                controlling.PlaceBomb();
            }

            lastKeyState = k;
        }

        public bool KeyJustPressed(Keys key, KeyboardState k)
        {
            return k.IsKeyDown(key) && lastKeyState.IsKeyUp(key);
        }

        public void GetPadInput(GamePadState g)
        {
            if (useKey) return;

            if (g.DPad.Up == ButtonState.Pressed || g.ThumbSticks.Left.Y < 0)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP));
            }

            if (g.DPad.Down == ButtonState.Pressed || g.ThumbSticks.Left.Y > 0)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN));
            }

            if (g.DPad.Left == ButtonState.Pressed || g.ThumbSticks.Left.X < 0)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT));
            }

            if (g.DPad.Right == ButtonState.Pressed || g.ThumbSticks.Left.X > 0)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT));
            }

            if (ButtonJustPressed(Buttons.A, g))
            {
                controlling.PlaceBomb();
            }

            lastPadState = g;
        }

        public bool ButtonJustPressed(Buttons button, GamePadState g)
        {
            return g.IsButtonDown(button) && lastPadState.IsButtonUp(button);
        }
    }
}
