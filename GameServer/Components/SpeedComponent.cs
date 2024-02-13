using SFML.System;

namespace GameServer.Components {
    public struct SpeedComponent {
        public Vector2f Speed;

        public SpeedComponent(Vector2f speed) {
            Speed = speed;
        }

        public SpeedComponent(float x, float y) {
            Speed.X = x;
            Speed.Y = y;
        }
    }
}
