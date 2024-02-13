using DefaultEcs;
using DefaultEcs.System;
using GameClient.Components;
using SFML.Graphics;
using SFML.System;
using System;
using GameClient.Managers;

namespace GameClient.Systems {
    public sealed class HumanDrawSystem : ISystem<World> {
        public bool IsEnabled { get; set; } = true;

        Entity _player;

        public HumanDrawSystem(Entity player) {
            _player = player;
        }

        public void Update(World world) {
            foreach(var human in world.GetEntities().With<HumanFlag>().AsEnumerable()) {
                CircleShape circle = human.Get<CircleComponent>().Circle;

                if(human.Has<PlayerFlag>()) {
                    StateManager.Instance.Window.Draw(circle);
                } else {
                    Vector2f playerPosition = _player.Get<PositionComponent>().Position;
                    Vector2f position = new Vector2f(
                        human.Get<PositionComponent>().Position.X - (playerPosition.X - (StateManager.Instance.Window.Size.X / 2f)),
                        human.Get<PositionComponent>().Position.Y - (playerPosition.Y - (StateManager.Instance.Window.Size.Y / 2f)));
                    circle.Position = position;
                    StateManager.Instance.Window.Draw(circle);
                }
            }
        }

        public void Dispose() {
            
        }
    }
}
