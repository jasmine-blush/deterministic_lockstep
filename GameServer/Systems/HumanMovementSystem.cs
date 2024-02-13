using DefaultEcs;
using DefaultEcs.System;
using GameServer.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Systems {
    public sealed class HumanMovementSystem : ISystem<World> {
        public bool IsEnabled { get; set; } = true;

        /*Handle the movement of each human by going through their InputLogs*/
        /*KeyCode = 0.0.0.0.0(Key_D).0(Key_S).0(Key_A).0(Key_W)*/
        public void Update(World world) {
            foreach(Entity human in world.GetEntities().With<HumanFlag>().AsEnumerable()) {
                foreach(InputFrame frame in human.Get<InputLogComponent>().InputFrames) {
                    if(frame.FrameID > human.Get<HumanFlag>().LastFrame) {
                        float timeSpeed = frame.GameTime / 1000f;  //GameTime in seconds used for speed calculation
                        if((frame.KeyCode & 1) == 1) {
                            float last_y = human.Get<PositionComponent>().Position.Y;
                            human.Get<PositionComponent>().Position.Y -= human.Get<SpeedComponent>().Speed.Y * timeSpeed;
                            if(human.Get<PositionComponent>().Position.Y < 0) {
                                human.Get<PositionComponent>().Position.Y = last_y;
                            }
                        }
                        if((frame.KeyCode & 2) == 2) {
                            float last_x = human.Get<PositionComponent>().Position.X;
                            human.Get<PositionComponent>().Position.X -= human.Get<SpeedComponent>().Speed.X * timeSpeed;
                            if(human.Get<PositionComponent>().Position.X < 0) {
                                human.Get<PositionComponent>().Position.X = last_x;
                            }
                        }
                        if((frame.KeyCode & 4) == 4) {
                            float last_y = human.Get<PositionComponent>().Position.Y;
                            human.Get<PositionComponent>().Position.Y += human.Get<SpeedComponent>().Speed.Y * timeSpeed;
                            //if(human.Get<PositionComponent>().Position.Y > GameState.Instance.WorldMap.WorldArray.GetLength(0) * TileManager.TileSize) {
                            if(human.Get<PositionComponent>().Position.Y > 32000f) {  //TODO: Retreive WorldMap Size
                                human.Get<PositionComponent>().Position.Y = last_y;
                            }
                        }
                        if((frame.KeyCode & 8) == 8) {
                            float last_x = human.Get<PositionComponent>().Position.X;
                            human.Get<PositionComponent>().Position.X += human.Get<SpeedComponent>().Speed.X * timeSpeed;
                            if(human.Get<PositionComponent>().Position.X > 32000f) {
                                human.Get<PositionComponent>().Position.X = last_x;
                            }
                        }
                        human.Get<HumanFlag>().LastFrame = frame.FrameID;
                    }
                }
                human.Get<InputLogComponent>().InputFrames.Clear();
            }
        }

        public void Dispose() {

        }
    }
}
