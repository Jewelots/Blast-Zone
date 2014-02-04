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
            useKey = true;
        }

        public void SetJoyIdentifiers()
        {
            useKey = false;
        }

        public void GetKeyInput(KeyboardState k)
        {
            if (!useKey) return;

            if (keyIdentifiers == null) return; //No keys set

            bool hasMoved = false;

            if (k.IsKeyDown(keyIdentifiers["up"]))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP));
                hasMoved = true;
            }

            if (k.IsKeyDown(keyIdentifiers["down"]))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN));
                hasMoved = true;
            }

            if (k.IsKeyDown(keyIdentifiers["left"]))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT));
                hasMoved = true;
            }

            if (k.IsKeyDown(keyIdentifiers["right"]))
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT));
                hasMoved = true;
            }

            if (KeyJustPressed(keyIdentifiers["bomb"], k))
            {
                controlling.PlaceBomb();
            }

            lastKeyState = k;

            //Player didn't move this frame
            if (!hasMoved)
            {
                controlling.StopMove();
            }
        }

        public bool KeyJustPressed(Keys key, KeyboardState k)
        {
            return k.IsKeyDown(key) && lastKeyState.IsKeyUp(key);
        }

        public void GetPadInput(GamePadState g)
        {
            if (useKey) return;

            bool hasMoved = false;

            if (g.DPad.Up == ButtonState.Pressed)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP, Math.Abs(150)));
                hasMoved = true;
            }

            if (g.DPad.Down == ButtonState.Pressed)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN, Math.Abs(150)));
                hasMoved = true;
            }

            if (g.DPad.Left == ButtonState.Pressed)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT, Math.Abs(150)));
                hasMoved = true;
            }

            if (g.DPad.Right == ButtonState.Pressed)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT, Math.Abs(150)));
                hasMoved = true;
            }

            if (g.ThumbSticks.Left.Y > 0.25)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_UP, Math.Abs(g.ThumbSticks.Left.Y * 150)));
                hasMoved = true;
            }

            if (g.ThumbSticks.Left.Y < -0.25)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_DOWN, Math.Abs(g.ThumbSticks.Left.Y * 150)));
                hasMoved = true;
            }

            if (g.ThumbSticks.Left.X < -0.25)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_LEFT, Math.Abs(g.ThumbSticks.Left.X * 150)));
                hasMoved = true;
            }

            if (g.ThumbSticks.Left.X > 0.25)
            {
                controlling.Move(MoveEvent.MakeEvent(MoveEvent.MoveEventType.MOVE_RIGHT, Math.Abs(g.ThumbSticks.Left.X * 150)));
                hasMoved = true;
            }

            if (ButtonJustPressed(Buttons.A, g) || ButtonJustPressed(Buttons.B, g) || ButtonJustPressed(Buttons.LeftShoulder, g) || ButtonJustPressed(Buttons.RightShoulder, g) || ButtonJustPressed(Buttons.LeftTrigger, g) || ButtonJustPressed(Buttons.RightTrigger, g))
            {
                controlling.PlaceBomb();
            }

            lastPadState = g;

            //Player didn't move this frame
            if (!hasMoved)
            {
                controlling.StopMove();
            }
        }

        public bool ButtonJustPressed(Buttons button, GamePadState g)
        {
            return g.IsButtonDown(button) && lastPadState.IsButtonUp(button);
        }
    }
}
