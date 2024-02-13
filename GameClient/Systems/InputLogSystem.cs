using DefaultEcs.System;
using GameClient.GameStates;
using GameClient.Networking;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Threading;
using GameClient.Managers;

namespace GameClient.Systems {
    public sealed class InputLogSystem : ISystem<float> {
        public bool IsEnabled { get; set; } = true;

        private static readonly object _lck = new object();
        private static List<InputFrame> _inputFrames = new List<InputFrame>();

        /*Saves a list of inputs with frameID and gameTime every frame*/
        /*KeyCode = 0.0.0.0.0(Key_D).0(Key_S).0(Key_A).0(Key_W)*/
        public void Update(float gameTime) {
            if(IsEnabled) {
                byte keyCode = 0;
                if(KeyboardManager.Key_W) {
                    keyCode += 1;
                }
                if(KeyboardManager.Key_A) {
                    keyCode += 2;
                }
                if(KeyboardManager.Key_S) {
                    keyCode += 4;
                }
                if(KeyboardManager.Key_D) {
                    keyCode += 8;
                }
                if(keyCode > 0) {
                    lock(_lck) {
                        //TODO: Implement InputLog packet batching
                        /*if(_inputFrames.Count > 0) {
                            InputFrame lastInputFrame = _inputFrames[_inputFrames.Count - 1];
                            if(lastInputFrame.KeyCode == keyCode) {
                                lastInputFrame.FrameID = GameState.Instance.FrameID;
                                lastInputFrame.GameTime += gameTime;
                                Console.Write(gameTime);
                            } else {
                                _inputFrames.Add(new InputFrame(GameState.Instance.FrameID, gameTime, keyCode));
                            }
                        } else {*/
                            _inputFrames.Add(new InputFrame(GameState.Instance.FrameID, gameTime, keyCode));
                        //}
                    }
                    
                }
            }
        }

        public static List<InputFrame> AcquireInputFrames() {
            Monitor.Enter(_lck);
            return _inputFrames;
        }

        public static void ReleaseInputFrames() {
            Monitor.Exit(_lck);
        }

        public static void UpdateLastInputFrame(int frameID, float gameTime) {
            lock(_lck) {

            }
        }

        public static void AddInputFrame(InputFrame inputFrame) {
            lock(_lck) {
                _inputFrames.Add(inputFrame);
            }
        }

        /*Removes all inputframes in the string[] frame list*/
        public static void RemoveInputFrames(string[] frames) {
            lock(_lck) {
                foreach(string frameID in frames) {
                    int currentFrameID = Int32.Parse(frameID);
                    foreach(InputFrame item in _inputFrames) {
                        if(item.FrameID == currentFrameID) {
                            _inputFrames.Remove(item);
                            break;
                        }
                    }
                }
            }
        }

        public static void Clear() {
            lock(_lck) {
                _inputFrames.Clear();
            }
        }

        /*Removes all input frames up to the frameID*/
        public static void ClearToFrameID(int frameID) {
            lock(_lck) {
                int frameIndex = -1;
                for(int i = 0; i < _inputFrames.Count; i++) {
                    if(_inputFrames[i].FrameID == frameID) {
                        frameIndex = i;
                        break;
                    }
                }
                if(frameIndex != -1)
                    _inputFrames.RemoveRange(0, frameIndex + 1);
            }
        }

        public void Dispose() {

        }
    }
}
