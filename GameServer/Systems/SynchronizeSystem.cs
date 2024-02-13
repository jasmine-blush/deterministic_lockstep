using DefaultEcs;
using DefaultEcs.System;
using GameServer.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Systems {
    public sealed class SynchronizeSystem : ISystem<World> {
        public bool IsEnabled { get; set; } = true;

        /*Sends synchronization data to humans, doesn't send anything if no humans*/
        public void Update(World world) {
            EntitySet humans = world.GetEntities().With<HumanFlag>().AsSet();

            List<byte> message = new List<byte>() { 1 };  //0.0.0.0.0.0.0.1 + {humanID} + {frameID} + {Position.X} + {Position.Y}
                                                          //1 byte          + 1 byte    + 4 bytes   + 4 bytes      + 4 bytes

            foreach(Entity human in humans.GetEntities()) {
                message.Add((byte)human.Get<HumanFlag>().HumanID);
                message.AddRange(BitConverter.GetBytes(human.Get<HumanFlag>().LastFrame));
                message.AddRange(BitConverter.GetBytes(human.Get<PositionComponent>().Position.X));
                message.AddRange(BitConverter.GetBytes(human.Get<PositionComponent>().Position.Y));
            }
            foreach(Entity human in humans.GetEntities()) {
                Program.Server.SendMessage(message.ToArray(), human.Get<HumanFlag>().IPAddr);
                Program.DataLength += message.Count;
            }

            humans.Dispose();
        }

        public void Dispose() {

        }
    }
}
