using System;
using System.Collections.Generic;
using System.Text;
using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using SFML.System;

namespace GameClient.Systems {
    public struct StateData {
        public int FrameID;
        public Vector2f Position;
    }

    public sealed class StateBufferSystem : ISystem<World> {
        public bool IsEnabled { get; set; }

        private Entity _player;
        private static List<StateData> _stateDataList = new List<StateData>();

        public StateBufferSystem(Entity player) {
            _player = player;
        }

        //TODO: Doesn't need world
        /*Saves the position of the player every frame*/
        public void Update(World world) {
            StateData data = new StateData() {
                FrameID = GameStates.GameState.Instance.FrameID,
                Position = _player.Get<PositionComponent>().Position
            };
            _stateDataList.Add(data);
        }

        public void Dispose() {

        }

        public static int GetLastFrameID() {
            if(_stateDataList.Count > 0) {
                return _stateDataList[_stateDataList.Count - 1].FrameID;
            }
            return -1;
        }

        /*Checks if the frameID exists in the saved positions*/
        public static bool HasFrame(int frameID) {
            foreach(StateData data in _stateDataList) {
                if(data.FrameID == frameID)
                    return true;
            }
            return false;
        }

        /*Gets the position at a specific frameID*/
        public static Vector2f GetPositionAtFrame(int frameID) {
            foreach(StateData data in _stateDataList) {
                if(data.FrameID == frameID)
                    return data.Position;
            }
            return new Vector2f();
        }

        /*Called if the client and server were unsynchronized, clears inputlog as well so the client doesn't send old and wrong frames*/
        public static void Clear() {
            InputLogSystem.Clear();
            _stateDataList.Clear();
        }

        /*Clears states and inputLog up to frameID, called when client and server were synchronized up to the frame*/
        public static void ClearToFrameID(int frameID) {
            InputLogSystem.ClearToFrameID(frameID);

            int frameIndex = -1;
            for(int i = 0; i < _stateDataList.Count; i++) {
                if(_stateDataList[i].FrameID == frameID) {
                    frameIndex = i;
                    break;
                }
            }
            if(frameIndex != -1)
                _stateDataList.RemoveRange(0, frameIndex + 1);
        }
    }
}
