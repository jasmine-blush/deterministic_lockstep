using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Managers {
    public static class KeyboardManager {
        public static bool Key_W = false;
        public static bool Key_S = false;
        public static bool Key_A = false;
        public static bool Key_D = false;

        public static void KeyPressed(object sender, KeyEventArgs e) {
            if(e.Code == Keyboard.Key.W)
                Key_W = true;
            if(e.Code == Keyboard.Key.S)
                Key_S = true;
            if(e.Code == Keyboard.Key.A)
                Key_A = true;
            if(e.Code == Keyboard.Key.D)
                Key_D = true;
        }

        public static void KeyReleased(object sender, KeyEventArgs e) {
            if(e.Code == Keyboard.Key.W)
                Key_W = false;
            if(e.Code == Keyboard.Key.S)
                Key_S = false;
            if(e.Code == Keyboard.Key.A)
                Key_A = false;
            if(e.Code == Keyboard.Key.D)
                Key_D = false;
        }
    }
}
