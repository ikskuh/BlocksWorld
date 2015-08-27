using System;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System.Linq;

namespace BlocksWorld
{
    internal class InputDriver : IGameInputDriver
    {
        private Game game;
        private readonly Dictionary<Key, bool> pressedButtons = new Dictionary<Key, bool>();
        private readonly Dictionary<Key, bool> releasedButtons = new Dictionary<Key, bool>();
        private readonly Dictionary<MouseButton, bool> pressedMouse = new Dictionary<MouseButton, bool>();
        private readonly Dictionary<MouseButton, bool> releasedMouse = new Dictionary<MouseButton, bool>();

        public InputDriver(Game game)
        {
            this.game = game;

            this.Mouse.Move += Mouse_Move;
            this.Mouse.ButtonDown += (s, e) =>
            {
                this.pressedMouse[e.Button] = true;
            };
            this.Mouse.ButtonUp += (s, e) =>
            {
                this.releasedMouse[e.Button] = true;
            };

            this.Keyboard.KeyDown += (s, e) =>
            {
                this.pressedButtons[e.Key] = true;
            };
            this.Keyboard.KeyDown += (s, e) =>
            {
                this.releasedButtons[e.Key] = true;
            };

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (pressedButtons.ContainsKey(key)) continue;
                pressedButtons.Add(key, false);
                releasedButtons.Add(key, false);
            }

            foreach (MouseButton btn in Enum.GetValues(typeof(MouseButton)))
            {
                if (pressedMouse.ContainsKey(btn)) continue;
                pressedMouse.Add(btn, false);
                releasedMouse.Add(btn, false);
            }
        }

        private void Mouse_Move(object sender, MouseMoveEventArgs e)
        {
            this.MouseMovement += new Vector2(e.XDelta, e.YDelta);
        }

        public KeyboardDevice Keyboard
        {
            get { return this.game.Keyboard; }
        }

        public MouseDevice Mouse
        {
            get { return this.game.Mouse; }
        }

        public Vector2 MouseMovement { get; private set; }

        public bool GetButton(Key key)
        {
            return this.Keyboard[key];
        }

        public bool GetButtonDown(Key key)
        {
            return this.pressedButtons[key];
        }

        public bool GetButtonUp(Key key)
        {
            return this.releasedButtons[key];
        }

        internal void Reset()
        {
            this.SetFalse(this.pressedMouse);
            this.SetFalse(this.pressedButtons);

            this.SetFalse(this.releasedMouse);
            this.SetFalse(this.releasedButtons);

            this.MouseMovement = Vector2.Zero;
        }

        void SetFalse<T>(Dictionary<T, bool> dict)
        {
            foreach(var key in dict.Keys.ToArray())
            {
                dict[key] = false;
            }
        }

        public bool GetMouse(MouseButton btn)
        {
            return this.Mouse[btn];
        }

        public bool GetMouseDown(MouseButton btn)
        {
            return this.pressedMouse[btn];
        }

        public bool GetMouseUp(MouseButton btn)
        {
            return this.releasedMouse[btn];
        }
    }
}