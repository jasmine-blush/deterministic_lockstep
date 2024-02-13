using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer {
    class Program {
        private const int _serverPort = 48620;  //48620 - 48652 unassigned under IANA https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.xhtml?search=48620

        public static UdpServer Server = new UdpServer(new IPEndPoint(IPAddress.Parse("192.168.0.97"), _serverPort));  //Create the udp server

        private static SFML.System.Clock gameClock;

        public static int DataLength = 0;

        static void Main() {
            //debug
            float gameTime = 0f;
            float count = 0;
            //------


            bool gameHasEnded = false;  //If true, program ends
            float timeStep = 1f / 60f;  //60 ticks per second

            GameState.Instance.Initialize();
            GameState.Instance.LoadContent();

            /*start listening for messages and copy the messages back to the client*/
            Task.Factory.StartNew(async () => {
                while(!gameHasEnded) {  //Listen as long as the main loop is running
                    try {
                        var receivedMessage = await Server.AsyncReceiveMessageBytes();  //Listen for a message

                        GameState.Instance.AddCurrentMessage(receivedMessage);  //If a message is received, add it to Game so it can handle it at next tick
                    }catch(Exception e) {
                        Console.WriteLine(e.Message);
                        //Console.WriteLine(e.StackTrace);
                    }
                }
            });

            gameClock = new SFML.System.Clock();  //Create Clock for counting gameTime (starts automatically)
            while(!gameHasEnded) {  //Main Game Loop

                GameState.Instance.Update();  //Update each tick

                /*if the current tick is shorter than the timeStep, wait*/
                while(gameClock.ElapsedTime.AsSeconds() < timeStep) {
                    //wait or do some other stuff (sleep method takes >15ms, so not a good alternative)
                }


                //debug
                gameTime += gameClock.ElapsedTime.AsSeconds();
                count++;
                if(gameTime >= 1f) {
                    Console.WriteLine(gameTime + " | " + count + " | " + DataLength);
                    gameTime = 0f;
                    count = 0;
                    DataLength = 0;
                }
                //------


                gameClock.Restart();
            }

            Server.CloseServer();
            GameState.Instance.UnloadContent();

            Environment.Exit(0);
        }
    }
}
