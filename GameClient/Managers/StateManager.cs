using System;
using SFML.Graphics;
using SFML.Window;
using GameClient.GameStates;


namespace GameClient.Managers {
    class StateManager {
        //------------------------------------------------------------
        //You can see this construct in multiple other classes. This is purely an implementation of the singleton pattern.
        private static readonly object _lck = new object();
        private static StateManager _instance;
        public static StateManager Instance {
            get {
                if(_instance == null) {
                    lock(_lck) {
                        if(_instance == null) {
                            _instance = new StateManager();
                        }
                    }
                }
                return _instance;
            }
        }
        //------------------------------------------------------------

        public static bool GameEnded = false;
        public RenderWindow Window;

        private static bool _windowHasFocus = true;
        private static IState _currentState;  //The State the StateManager currently runs
        private FramesPerSecondCounter _fpsCounter;
        private Text _fps;  //The StateManager and not the States themselves display the fps every frame

        #region Important Methods

        /*The Initialize method's main purpose is to initialize all the important variables*/
        public void Initialize() {
            //---------------------------------------
            //Creation and Tweaking of the window
            Window = new RenderWindow(new VideoMode(1280, 800), "GameClient", Styles.Close);  //Resolution, Title, WindowStyle, (ContextSettings)
            Window.SetVerticalSyncEnabled(false);
            Window.GainedFocus += new EventHandler(delegate { _windowHasFocus = true; });
            Window.LostFocus += new EventHandler(delegate { _windowHasFocus = false; });
            Window.Closed += new EventHandler(delegate { GameEnded = true; });
            Window.SetVisible(true);  //Show the window
            Window.SetKeyRepeatEnabled(false);
            //Window.SetFramerateLimit(144);
            //---------------------------------------

            Console.WriteLine("OpenGL Version: " + Window.Settings.MajorVersion + "." + Window.Settings.MinorVersion);  //Display the OpenGL Context

            //TODO: Make cleaner so gamestate isn't instanced in the initialize but when it's actually relevant
            _currentState = GameState.Instance;  //The State the engine launches in on startup
            _currentState.Initialize();  //Since we are in the Initialize method, call the same method for first loaded State

            //TODO: Clean up fps counter system and fontmanager
            //Initializing the FPS Counter
            _fpsCounter = new FramesPerSecondCounter();
            _fps = new Text(_fpsCounter.GetFPS(), FontManager.Instance.Arial, 12);  //for FontManager see FontManager.cs
            _fps.FillColor = Color.Black;
        }


        /*The LoadContent method's main purpose is to load things like textures from files*/
        public void LoadContent() {
            _currentState.LoadContent();
        }


        public void Update(float gameTime) {
            Window.DispatchEvents();  //Calls the EventHandlers defined on Window Initializiation if queued
            //if(_windowHasFocus) {
                _fpsCounter.Update(gameTime);
                MouseManager.UpdateStart();  //SeeMouseHandler.cs
                _currentState.Update(gameTime);
                MouseManager.UpdateEnd();  //SeeMouseHandler.cs
            //}
        }


        public void Draw() {
            Window.Clear(Color.White);
            _currentState.Draw();

            /*See FramesPerSecondCounter.cs to understand this if-clause*/
            if(_fpsCounter.CurrentGameTime == 0) _fps.DisplayedString = _fpsCounter.GetFPS();
            _fps.Font = FontManager.Instance.Arial;
            Window.Draw(_fps);

            Window.Display();  //Swap the Buffer
        }


        /*The UnloadContent method's main purpose is to dispose everything that can be disposed at the end of runtime*/
        public void UnloadContent() {
            Window.Close();
            Window.Dispose();

            /*Unload all the States that have an Instance (have been called at least once)*/
            if(GameState.HasInstance()) GameState.Instance.UnloadContent();

            _fps.Dispose();
            FontManager.Instance.Dispose();
        }

        #endregion
        #region Utility

        //TODO: make better system
        /*This Method is called by the States themselves to switch to a different State*/
        public static void ChangeState(States newState) {
            bool init = false;
            switch(newState) {
                case States.Game:
                    init = !GameState.HasInstance();
                    _currentState = GameState.Instance;
                    break;
                default:
                    break;
            }

            /*Initialize and Load the new State since the game-loop only cycles through Update and Draw*/
            if(init) {
                _currentState.Initialize();
                _currentState.LoadContent();
            }
        }

        #endregion
    }
}
