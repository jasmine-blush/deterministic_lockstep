using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using GameClient.GameStates;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Systems {
    public sealed class CameraSystem : ISystem<World> {
        public bool IsEnabled { get; set; }

        private Entity _player;

        public CameraSystem(Entity player) {
            _player = player;
        }

        public void Update(World world) {
            /*if the last position in state buffer does not equals that of current frame, set update rendertexture to true*/
            if(StateBufferSystem.GetPositionAtFrame(GameState.Instance.FrameID - 1) != _player.Get<PositionComponent>().Position) {
                GameState.Instance.WorldMap.NeedsUpdate = true;
            }
        }

        public void Dispose() {

        }
    }
}
