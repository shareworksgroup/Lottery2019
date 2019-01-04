using FarseerPhysics.Common;
using Lottery2019.UI.Details;
using SharpDX;
using SharpDX.Direct2D1;
using System.Linq;
using EngineShapes = FarseerPhysics.Collision.Shapes;
using Xna = Microsoft.Xna.Framework;

namespace Lottery2019.UI.Shapes
{
    public class PolygonShape : Shape
    {
        public PolygonShape()
        {
        }

        public PolygonShape(JsonShape jsonShape, Vector2 center) : base(jsonShape, center)
        {
            Points = jsonShape.Points.Select(x => new Vector2(x[0], x[1])).ToArray();
        }

        public Vector2[] Points { get; set; }

        public override void Draw(DeviceContext renderTarget, Brush brush)
        {
            var lines = new[]
            {
                new []{ Points[0], Points[1] },
                new []{ Points[1], Points[2] },
                new []{ Points[2], Points[3] },
                new []{ Points[3], Points[0] },
            };
            foreach (var line in lines)
            {
                renderTarget.DrawLine(line[0], line[1], brush, 2.0f);
            }
        }

        public override bool TestPoint(Vector2 point)
        {
            var test = new EngineShapes.PolygonShape(1.0f)
            {
                Vertices = new Vertices(
                Points.Select(x => new Xna.Vector2(x.X, x.Y)))
            };

            var transform = new FarseerPhysics.Common.Transform();
            transform.SetIdentity();
            var mouseXna = new Xna.Vector2(point.X, point.Y);

            return test.TestPoint(ref transform, ref mouseXna);
        }

        public override EngineShapes.Shape ToEngineShape()
        {
            return new EngineShapes.PolygonShape(Density)
            {
                Vertices = new Vertices(
                Points.Select(p => (p - Center).ToXnaVector2()))
            };
        }
    }
}
