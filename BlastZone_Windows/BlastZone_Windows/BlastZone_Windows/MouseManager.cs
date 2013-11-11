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
    enum MouseButton
    {
        LEFT,
        RIGHT,
        MIDDLE
    }

    class MouseManager
    {
        static MouseState oldState;
        static MouseState State;

        private MouseManager() { }

        public static void Update()
        {
            oldState = State;
            State = Mouse.GetState();
        }

        public static bool ButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LEFT:
                    return (State.LeftButton   == ButtonState.Pressed && oldState.LeftButton   == ButtonState.Released);
                case MouseButton.RIGHT:
                    return (State.RightButton  == ButtonState.Pressed && oldState.RightButton  == ButtonState.Released);
                case MouseButton.MIDDLE:
                    return (State.MiddleButton == ButtonState.Pressed && oldState.MiddleButton == ButtonState.Released);
                default:
                    return false;
            }
        }

        public static bool ButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LEFT:
                    return (State.LeftButton   == ButtonState.Released && oldState.LeftButton   == ButtonState.Pressed);
                case MouseButton.RIGHT:
                    return (State.RightButton  == ButtonState.Released && oldState.RightButton  == ButtonState.Pressed);
                case MouseButton.MIDDLE:
                    return (State.MiddleButton == ButtonState.Released && oldState.MiddleButton == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        public static bool ButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LEFT:
                    return (State.LeftButton   == ButtonState.Pressed);
                case MouseButton.RIGHT:
                    return (State.RightButton  == ButtonState.Pressed);
                case MouseButton.MIDDLE:
                    return (State.MiddleButton == ButtonState.Pressed);
                default:
                    return false;
            }
        }
    }
}
