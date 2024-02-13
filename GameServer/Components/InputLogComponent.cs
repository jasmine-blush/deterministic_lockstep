using System.Collections.Generic;

namespace GameServer.Components {
    public struct InputLogComponent {
        public List<InputFrame> InputFrames;

        public InputLogComponent(List<InputFrame> inputFrames) {
            InputFrames = inputFrames;
        }
    }
}
