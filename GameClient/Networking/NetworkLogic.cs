using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DefaultEcs;
using GameClient.Components;
using GameClient.GameStates;
using GameClient.Systems;
using SFML.System;
using GameClient.Managers;

namespace GameClient.Networking {
    public static class NetworkLogic {
        public static async void RunNetworkLogic(UdpUser udpClient, Entity player) {
            await Task.Factory.StartNew(async () => {
                try {
                    udpClient.SendMessage(128);  //Initial Connect Message, ending bit signals player connection: 1.0.0.0.0.0.0.0

                    bool hasReceivedID = false;  //wait for server to send player ID
                    do {
                        var receivedMessage = await udpClient.AsyncReceiveMessage();
                        if(receivedMessage.Length == 1) {
                            if(receivedMessage[0] >= 128) {  //Server sends: 1.0.0.0.0.0.0.0 + humanID, ex: humanID = 3: 1.0.0.0.0.0.1.1
                                player.Get<HumanFlag>().HumanID = receivedMessage[0] - 128;
                                Console.WriteLine("ID: " + player.Get<HumanFlag>().HumanID);
                                hasReceivedID = true;
                            }
                        }
                    } while(!hasReceivedID);

                    Clock networkClock = new Clock();
                    bool gameEnded = false;
                    float timeStep = 1f / 60f;  //60 ticks per second
                    GameState.Instance.EnableInputLogging();  //Enable input logging after ID has been received

                    //debug
                    float gameTime = 0f;
                    float count = 0;
                    int dataLength = 0;
                    //------

                    DoReceive(udpClient);
                    while(!gameEnded) {

                        /*Send InputLog data to server every tick*/
                        List<InputFrame> inputFrames = InputLogSystem.AcquireInputFrames();
                        if(inputFrames.Count > 0) {
                            List<byte> inputData = new List<byte>() { 2 }; //0.0.0.0.0.0.1.0 + { InputFrame}
                            foreach(InputFrame item in inputFrames) {
                                inputData.AddRange(item.ToBytes());
                            }
                            udpClient.SendMessage(inputData.ToArray());
                            dataLength += inputData.Count;
                            InputLogSystem.Clear();
                        }
                        InputLogSystem.ReleaseInputFrames();



                        while(networkClock.ElapsedTime.AsSeconds() < timeStep) {
                            //wait or do some other stuff (sleep method takes 15ms, so bad)
                        }

                        //debug
                        gameTime += networkClock.ElapsedTime.AsSeconds();
                        count++;
                        if(gameTime >= 1f) {
                            Console.WriteLine(gameTime + " | " + count + " | " + dataLength);
                            gameTime = 0f;
                            count = 0;
                            dataLength = 0;
                        }
                        //------

                        networkClock.Restart();
                        if(StateManager.GameEnded)
                            gameEnded = true;
                    }

                    //Server sends tickNr at exact server time and client syncs with it
                    //If client doesn't send input for a few ticks, server sends new state
                    //Client sends input every tick
                    //Server sends other players input every tick
                    //ticks receive input for past frames, what do??

                    //Server sends full state regularly
                    //Server should run behind. It's either the server runs behind or client runs behind because two-way travel time

                    //Clients just send inputs to server asap (doesn't matter which tick they were on), server sends inputs back asap
                    //server also sends full state regularly to fix ping inconsistencies


                    //synchronize ticks with server

                    //collect list of player inputs every frame


                    //every tick:
                    //  send list to server
                    //  save which inputs were sent
                    //  if server send ack: clear all inputs that are accepted in ack

                    //await server sends new game state
                    //update game state
                } catch(Exception ex) {
                    Debug.Write(ex);
                }
            });
        }

        private static async void DoReceive(UdpUser udpClient) {
            bool ended = false;
            while(!ended) {
                byte[] receivedMessage = await udpClient.AsyncReceiveMessage();
                if(receivedMessage.Length > 0) {
                    if(receivedMessage[0] == 1) {  //0.0.0.0.0.0.0.1 = Synchronize data
                        int index = 1;
                        while(index < receivedMessage.Length - 1) {
                            PositionData positionData = new PositionData();
                            positionData.HumanID = receivedMessage[index];
                            positionData.FrameID = BitConverter.ToInt32(receivedMessage, index + 1);
                            positionData.Position = new Vector2f(
                                    BitConverter.ToSingle(receivedMessage, index + 5),
                                    BitConverter.ToSingle(receivedMessage, index + 9)
                            );

                            SynchronizeData.AddPositionData(positionData);

                            index += 13;
                        }
                    }
                } else {
                    throw new Exception("Message was 0");
                }

                if(StateManager.GameEnded)
                    ended = true;
            }
        }
    }
}
