using SFML.System;
using SFML.Window;
using System;

namespace GameClient.Managers {
    class MouseManager {
        //True if the button is being held down at the moment
        public static bool LeftButtonPressed = false;
        public static bool RightButtonPressed = false;
        public static bool LeftButtonReleased = false;
        public static bool RightButtonReleased = false;
        //True if the button has started to be pressed in this frame
        public static bool LeftButtonJustPressed = false;
        public static bool RightButtonJustPressed = false;
        public static bool LeftButtonJustReleased = false;
        public static bool RightButtonJustReleased = false;

        public static Vector2f Position; //Mouse position

        private static bool _lastFrameLeftButtonPressed = false; //true = pressed
        private static bool _lastFrameRightButtonPressed = false; //true = pressed

        #region Important Methods

        //Updates the current mouse states
        public static void UpdateStart() {
            //Left Mouse Button
            bool leftButtonPressed = Mouse.IsButtonPressed(Mouse.Button.Left);

            LeftButtonJustPressed = !_lastFrameLeftButtonPressed && leftButtonPressed;
            LeftButtonJustReleased = _lastFrameLeftButtonPressed && !leftButtonPressed;
            LeftButtonPressed = leftButtonPressed;
            LeftButtonReleased = !leftButtonPressed;
            //-------------------------------

            //Right Mouse Button
            bool rightButtonPressed = Mouse.IsButtonPressed(Mouse.Button.Right);

            RightButtonJustPressed = !_lastFrameRightButtonPressed && rightButtonPressed;
            RightButtonJustReleased = _lastFrameRightButtonPressed && !rightButtonPressed;
            RightButtonPressed = rightButtonPressed;
            RightButtonReleased = !rightButtonPressed;
            //-------------------------------

            Position = (Vector2f)Mouse.GetPosition(StateManager.Instance.Window); //SFML Method
        }

        //Updates the mouse states preemptively for the next frame
        public static void UpdateEnd() {
            //Left Mouse Button
            _lastFrameLeftButtonPressed = LeftButtonPressed;
            LeftButtonJustPressed = false;
            LeftButtonJustReleased = false;
            //-------------------------------

            //Right Mouse Button
            _lastFrameRightButtonPressed = RightButtonPressed;
            RightButtonJustPressed = false;
            RightButtonJustReleased = false;
            //-------------------------------
        }

        #endregion
    }
}
