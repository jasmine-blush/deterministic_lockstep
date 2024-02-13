using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using SFML.Graphics;
using GameClient.Systems;
using SFML.System;
using SFML.Window;
using GameClient.Networking;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net;
using GameClient.Managers;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameClient.GameStates {
    class GameState : IState {
        //---------------------------
        //You can see this construct in multiple other classes. This is purely an implementation of the singleton pattern.
        private static readonly object _lck = new object();
        private static GameState _instance;
        public static GameState Instance {
            get {
                if(_instance == null) {
                    lock(_lck) {
                        if(_instance == null) {
                            _instance = new GameState();
                        }
                    }
                }
                return _instance;
            }
        }
        //---------------------------

        public int FrameID;
        public WorldMap WorldMap;

        private World _world;
        private SequentialSystem<float> _updateSystems;
        private ISystem<float> _inputLogSystem;
        private SequentialSystem<World> _networkSystems;
        private ISystem<World> _cameraSystem;
        private ISystem<World> _cameraDrawSystem;
        private SequentialSystem<World> _drawSystems;

        private UdpUser _udpClient;

        #region Important Methods

        public void Initialize() {
            _world = new World();

            //Main Player Init
            Entity player = _world.CreateEntity();
            Vector2f initialPlayerPosition = new Vector2f(600f, 600f);
            player.Set<PlayerFlag>();
            player.Set<HumanFlag>();
            player.Set(new PositionComponent(initialPlayerPosition));
            player.Set(new SpeedComponent(100f, 100f));
            player.Set(new CircleComponent(10f, Color.Black, new Vector2f(StateManager.Instance.Window.Size.X / 2, StateManager.Instance.Window.Size.Y / 2)));
            //------------------

            //Second Player Init
            /*
            Entity opponent = _world.CreateEntity();
            Vector2f initialOpponentPosition = new Vector2f(800, 600);
            opponent.Set<HumanFlag>();
            opponent.Set(new PositionComponent(initialOpponentPosition));
            opponent.Set(new CircleComponent(12, Color.Red, initialOpponentPosition));*/
            //------------------

            _inputLogSystem = new InputLogSystem();
            _inputLogSystem.IsEnabled = false;
            _updateSystems = new SequentialSystem<float>(
                _inputLogSystem,
                new PlayerMovementSystem(player));
            _networkSystems = new SequentialSystem<World>(
                new StateBufferSystem(player),
                new SynchronizeSystem());
            _cameraSystem = new CameraSystem(player);

            _cameraDrawSystem = new CameraDrawSystem(player);
            _drawSystems = new SequentialSystem<World>(
                new HumanDrawSystem(player));

            StateManager.Instance.Window.KeyPressed += KeyboardManager.KeyPressed;
            StateManager.Instance.Window.KeyReleased += KeyboardManager.KeyReleased;

            _udpClient = new UdpUser(new IPEndPoint(IPAddress.Parse("192.168.137.144"), 48620));  //IP and Port of the Client
            try {
                NetworkLogic.RunNetworkLogic(_udpClient, player);
            } catch( ObjectDisposedException e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            FrameID = 0;
        }

        public void LoadContent() {
            TileManager.Instance.LoadContent();
            LoadWorldMap();
        }

        public void Update(float gameTime) {
            _updateSystems.Update(gameTime);
            _networkSystems.Update(_world);
            _cameraSystem.Update(_world);

            FrameID++;
            if(Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                StateManager.GameEnded = true;
        }

        public void Draw() {
            _cameraDrawSystem.Update(_world);
            _drawSystems.Update(_world);
        }

        public void UnloadContent() {
            //TODO: Properly send server disconnect message
            try { _udpClient.Close(); } catch(Exception e) {}

            //TODO: Dispose SFML objects
            _world.Dispose();
        }

        #endregion
        #region Utility

        public void EnableInputLogging() {
            _inputLogSystem.IsEnabled = true;
        }

        private void LoadWorldMap() {
            byte[] file = File.ReadAllBytes(@"Assets\WorldMap.dat");  //Read WorldMap file

            /*Get world size and create array*/
            int worldSize = BitConverter.ToInt32(file, 0);
            byte[,] worldArray = new byte[worldSize, worldSize];

            /*Copy byte stream from file starting index 4 (first 4 bits are int32 worldsize)*/
            Buffer.BlockCopy(file, 4, worldArray, 0, worldSize * worldSize);

            WorldMap = new WorldMap(worldArray);
        }

        #endregion
        #region Setter/Getters

        public static bool HasInstance() {
            return _instance != null;
        }

        #endregion



        /*Method to generate the world array and safe it to file*/
        public void GenerateWorldArray() {
            int worldSize = 1000; //1000x1000 tiles

            Random rng = new Random();

            using(var fileStream = new FileStream(@"Assets\WorldMap.dat", FileMode.Create)) {
                fileStream.Write(BitConverter.GetBytes(worldSize));
                for(int i = 0; i < worldSize; i++) {
                    for(int j = 0; j < worldSize; j++) {
                        fileStream.WriteByte((byte)rng.Next(0, (int)Tiles.Grass + 1));
                    }
                }
                fileStream.Dispose();
            }
        }
    }
}
