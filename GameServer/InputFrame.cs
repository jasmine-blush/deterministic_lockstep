using System;
using System.Collections.Generic;

namespace GameServer {
    public class InputFrame {
        public int FrameID;
        public float GameTime;
        public byte KeyCode;

        public InputFrame(int frameID, float gameTime, byte keyCode) {
            FrameID = frameID;
            GameTime = gameTime;
            KeyCode = keyCode;
        }
    }
}
