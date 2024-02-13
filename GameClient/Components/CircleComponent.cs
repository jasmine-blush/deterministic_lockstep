using SFML.Graphics;
using SFML.System;


namespace GameClient.Components {
    public struct CircleComponent {
        public CircleShape Circle;

        public CircleComponent(float radius, Color color, Vector2f position) {
            Circle = new CircleShape(radius);
            Circle.FillColor = color;
            Circle.Position = position;
            Circle.Origin = new Vector2f(radius, radius);
        }
    }
}
