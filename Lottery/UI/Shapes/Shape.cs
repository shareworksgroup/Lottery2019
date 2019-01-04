using FarseerPhysics.Dynamics;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using EngineShapes = FarseerPhysics.Collision.Shapes;
using Direct2D1 = SharpDX.Direct2D1;

namespace Lottery2019.UI.Shapes
{
    public abstract class Shape
    {
        public static float Restitution = 1.0f;
        public static float Friction = 0.0f;

        public Vector2 Center { get; set; }

        public Vector2 Offset { get; set; }

        public abstract bool TestPoint(Vector2 point);

        public abstract EngineShapes.Shape ToEngineShape();

        public Shape()
        {
        }

        public Shape(JsonShape jsonShape, Vector2 center)
        {
            Center = center;
            Offset = new Vector2(jsonShape.Offset[0], jsonShape.Offset[1]);
        }

        public abstract void Draw(
            Direct2D1.DeviceContext renderTarget, 
            Direct2D1.Brush brush);

        public const float Density = 1.0f;

        public static bool TestPoint(IEnumerable<Shape> shapes, Vector2 point)
        {
            return shapes.Any(shape => shape.TestPoint(point));
        }

        public static void CreateFixtures(
            IEnumerable<Shape> shapes, 
            Body body)
        {
            foreach (var shape in shapes)
            {
                var engineShape = shape.ToEngineShape();
                var fixture = body.CreateFixture(engineShape);
                fixture.Restitution = Restitution;
                fixture.Friction = Friction;
            }
        }
    }
}
