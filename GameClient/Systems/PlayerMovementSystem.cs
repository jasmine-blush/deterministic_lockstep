using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using GameClient.GameStates;
using SFML.Window;
using System;
using GameClient.Managers;
using SFML.System;

namespace GameClient.Systems {
    public sealed class PlayerMovementSystem : ISystem<float> {
        public bool IsEnabled { get; set; } = true;

        private Entity _player;

        public PlayerMovementSystem(Entity player) {
            _player = player;
        }

        public void Update(float gameTime) {
            float timeSpeed = gameTime / 1000f;  //GameTime in seconds used for speed calculation
            if(KeyboardManager.Key_W) {
                float last_y = _player.Get<PositionComponent>().Position.Y;
                _player.Get<PositionComponent>().Position.Y -= _player.Get<SpeedComponent>().Speed.Y * timeSpeed;
                if(_player.Get<PositionComponent>().Position.Y < 0) {
                    _player.Get<PositionComponent>().Position.Y = last_y;
                }
            }
            if(KeyboardManager.Key_A) {
                float last_x = _player.Get<PositionComponent>().Position.X;
                _player.Get<PositionComponent>().Position.X -= _player.Get<SpeedComponent>().Speed.X * timeSpeed;
                if(_player.Get<PositionComponent>().Position.X < 0) {
                    _player.Get<PositionComponent>().Position.X = last_x;
                }
            }
            if(KeyboardManager.Key_S) {
                float last_y = _player.Get<PositionComponent>().Position.Y;
                _player.Get<PositionComponent>().Position.Y += _player.Get<SpeedComponent>().Speed.Y * timeSpeed;
                if(_player.Get<PositionComponent>().Position.Y > GameState.Instance.WorldMap.WorldArray.GetLength(0) * TileManager.TileSize) {
                    _player.Get<PositionComponent>().Position.Y = last_y;
                }
            }
            if(KeyboardManager.Key_D) {
                float last_x = _player.Get<PositionComponent>().Position.X;
                _player.Get<PositionComponent>().Position.X += _player.Get<SpeedComponent>().Speed.X * timeSpeed;
                if(_player.Get<PositionComponent>().Position.X > GameState.Instance.WorldMap.WorldArray.GetLength(0) * TileManager.TileSize) {
                    _player.Get<PositionComponent>().Position.X = last_x;
                }
            }
        }

        public void Dispose() {
           
        }
    }
}
