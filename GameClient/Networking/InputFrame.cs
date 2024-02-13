using System;
using System.Collections.Generic;
using GameClient.Systems;

namespace GameClient.Networking {
    public class InputFrame {
        public int FrameID;
        public float GameTime;
        public byte KeyCode;

        public InputFrame(int frameID, float gameTime, byte keyCode) {
            FrameID = frameID;
            GameTime = gameTime;
            KeyCode = keyCode;
        }

        /*{frameID} + {gameTime} + {KeyCodes}
         * 4 bytes  + 4 bytes    + 1 byte*/
        public byte[] ToBytes() {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(FrameID));
            bytes.AddRange(BitConverter.GetBytes(GameTime));
            bytes.Add(KeyCode);
            return bytes.ToArray();
        }
    }
}
