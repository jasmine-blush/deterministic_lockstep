using System;
using GameClient.Managers;

namespace GameClient {
    class Program {
        //If Windows COM has to be used (e.g. System Dialogs):
        //[STAThread]
        static void Main() {
            StateManager game = StateManager.Instance;  //StateManager and all States are Singletons

            game.Initialize();
            game.LoadContent();

            float gameTime;
            SFML.System.Clock gameClock = new SFML.System.Clock();  //This is the clock that measures deltatime between frames

            /*StateManager's "Ended" variable becomes true when the program is supposed to quit*/
            while(!StateManager.GameEnded) { 
                //TODO: Remove Division for performance
                gameTime = gameClock.Restart().AsMicroseconds() / 1000f; //Deltatime of each loop (aka frame) taken as Microseconds and converted to Miliseconds

                game.Update(gameTime);
                game.Draw();
            }

            gameClock.Dispose();
            game.UnloadContent();

            Environment.Exit(0);
        }
    }
}
