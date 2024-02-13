using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using GameClient.Networking;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Systems {
    public sealed class SynchronizeSystem : ISystem<World> {
        public bool IsEnabled { get; set; } = true;

        /*Go through all synchronize data from the server and either create humans or synchronize positions*/
        public void Update(World world) {
            List<PositionData> positionList = SynchronizeData.AcquirePositionList();
            /*go through all synchronize data*/
            foreach(PositionData positionData in positionList) {
                bool humanExists = false;
                /*go through all humans to check if the humanID of current positionData matches*/
                foreach(Entity currentHuman in world.GetEntities().With<HumanFlag>().AsEnumerable()) {
                    if(positionData.HumanID == currentHuman.Get<HumanFlag>().HumanID) {
                        humanExists = true;
                        /*if human is player, check StateBufferSystem for last positions, otherwise just synchronize human*/
                        if(currentHuman.Has<PlayerFlag>()) {
                            if(StateBufferSystem.HasFrame(positionData.FrameID)) {
                                if(positionData.Position != StateBufferSystem.GetPositionAtFrame(positionData.FrameID)) {
                                    currentHuman.Get<PositionComponent>().Position = positionData.Position;
                                    StateBufferSystem.Clear();
                                } else {
                                    StateBufferSystem.ClearToFrameID(positionData.FrameID);
                                }
                            } else if(positionData.FrameID == -1){
                                currentHuman.Get<PositionComponent>().Position = positionData.Position;
                            }
                        } else {
                            currentHuman.Get<PositionComponent>().Position = positionData.Position;
                        }
                    }
                }

                /*if human doesn't exist yet, create it immediately so it can be used for next positionData*/
                if(!humanExists) {
                    Entity newHuman = world.CreateEntity();
                    newHuman.Set(new HumanFlag(positionData.HumanID));
                    newHuman.Set(new PositionComponent(positionData.Position));
                    newHuman.Set(new CircleComponent(12, Color.Red, positionData.Position));
                }
            }
            positionList.Clear();
            SynchronizeData.ReleasePositionList();
        }

        public void Dispose() {

        }
    }
}
