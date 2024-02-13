using DefaultEcs;
using DefaultEcs.System;
using GameServer.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Systems {
    public sealed class HandleMessagesSystem : ISystem<World> {
        public bool IsEnabled { get; set; } = true;

        /*Go through all current messages and then clear the list*/
        public void Update(World world) {
            List<ReceivedMessage> currentMessages = GameState.Instance.AcquireCurrentMessages();
            foreach(ReceivedMessage message in currentMessages) {
                if(message.Message.Length > 0) {
                    if(message.Message[0] == 128) {  //Player has connected: 1.0.0.0.0.0.0.0
                        AddHuman(world, message);
                    } else if(message.Message[0] == 2) {  //Player sends input: 0.0.0.0.0.0.1.0
                        AddInputFrames(world, message);
                    }
                } else {
                    throw new Exception("Message was 0");
                }
            }
            currentMessages.Clear();
            GameState.Instance.ReleaseCurrentMessages();
        }

        /*Add a human to the world if they don't already exist, checked by IP*/
        private void AddHuman(World world, ReceivedMessage receivedMessage) {
            /*Checks IP Address to see if human has connected before*/
            EntitySet humans = world.GetEntities().With<HumanFlag>().AsSet();
            foreach(Entity currentHuman in humans.GetEntities()) {
                HumanFlag humanFlag = currentHuman.Get<HumanFlag>();
                if(receivedMessage.IPAddr.Equals(humanFlag.IPAddr)) {
                    Program.Server.SendMessage((byte)(128 + humanFlag.HumanID), humanFlag.IPAddr);  //1.0.0.0.0.0.0.0 + humanID, ex: humanID = 3: 1.0.0.0.0.0.1.1
                    currentHuman.Get<HumanFlag>().LastFrame = -1;
                    return;
                }
            }

            //TODO: Streamline creation of humans
            /*Creates human if necessary*/
            Entity human = world.CreateEntity();
            int humanID = humans.Count;
            human.Set(new HumanFlag(humanID, receivedMessage.IPAddr));
            human.Set(new PositionComponent(600f, 300f)); //Set initial player position and override init code of GameClient (600, 300)
            human.Set(new SpeedComponent(100f, 100f));
            human.Set(new InputLogComponent(new List<InputFrame>()));

            Program.Server.SendMessage((byte)(128 + humanID), receivedMessage.IPAddr);  //1.0.0.0.0.0.0.0 + humanID, ex: humanID = 3: 1.0.0.0.0.0.1.1

            humans.Dispose();
        }

        /*Add received input frames to the InputLog of a human*/
        /* 0.0.0.0.0.0.1.0 + {InputFrame}
         * 1 byte 		   + 9 bytes
         * 
         * InputFrame = {frameID} + {gameTime} + {KeyCodes}
				        4 bytes   + 4 bytes    + 1 byte
         * KeyCode = 0.0.0.0.0(Key_D).0(Key_S).0(Key_A).0(Key_W)
         */
        private void AddInputFrames(World world, ReceivedMessage receivedMessage) {
            /*Check if a human with the IP exists*/
            Entity human = new Entity();
            bool humanExists = false;
            foreach(Entity currHuman in world.GetEntities().With<HumanFlag>().AsEnumerable()) {
                if(receivedMessage.IPAddr.Equals(currHuman.Get<HumanFlag>().IPAddr)) {
                    human = currHuman;
                    humanExists = true;
                    break;
                }
            }

            if(humanExists) {
                /*Parse each sent input frame and attach it to the humans InputLog*/
                InputLogComponent inputLog = human.Get<InputLogComponent>();

                int index = 1;
                while(index < receivedMessage.Message.Length - 1) {
                    int frameID = BitConverter.ToInt32(receivedMessage.Message, index);
                    float gameTime = BitConverter.ToSingle(receivedMessage.Message, index + 4);
                    byte keyCode = receivedMessage.Message[index + 8];
                    inputLog.InputFrames.Add(new InputFrame(frameID, gameTime, keyCode));

                    index += 9;
                }
            } else {
                Console.WriteLine("human {0} doesn't exist", receivedMessage.IPAddr);
            }
        }

        public void Dispose() {

        }
    }
}
