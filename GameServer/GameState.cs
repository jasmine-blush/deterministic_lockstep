using DefaultEcs;
using DefaultEcs.System;
using GameServer.Components;
using GameServer.Systems;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer {
    class GameState {
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

        private static readonly object _msgLck = new object();
        private List<ReceivedMessage> _currentMessages;

        private World _world;

        private SequentialSystem<World> _worldSystems;

        #region Important Methods

        public void Initialize() {
            _currentMessages = new List<ReceivedMessage>();  //Stores all message received since last tick (threadsafe through locks)
            
            _world = new World();

            _worldSystems = new SequentialSystem<World>(
                new HandleMessagesSystem(),  //Handles all the messages received since last tick
                new HumanMovementSystem(),  //Handles all the human movement
                new SynchronizeSystem());  //Sends the current human positions to every human
        }

        public void LoadContent() {

        }

        public void Update() {
            _worldSystems.Update(_world);
        }

        public void UnloadContent() {
            _world.Dispose();
        }

        #endregion
        #region Utility

        public void AddCurrentMessage(ReceivedMessage received) {
            lock(_msgLck) {
                _currentMessages.Add(received);
            }
        }

        public List<ReceivedMessage> AcquireCurrentMessages() {
            Monitor.Enter(_msgLck);
            return _currentMessages;
        }

        public void ReleaseCurrentMessages() {
            Monitor.Exit(_msgLck);
        }

        #endregion
    }
}
