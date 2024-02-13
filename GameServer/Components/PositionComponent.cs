using SFML.System;

namespace GameServer.Components {
    public struct PositionComponent {
        public Vector2f Position;

        public PositionComponent(Vector2f position) {
            Position = position;
        }

        public PositionComponent(float x, float y) {
            Position.X = x;
            Position.Y = y;
        }
    }
}
