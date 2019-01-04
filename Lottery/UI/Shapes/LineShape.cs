using FarseerPhysics.Common;
using Lottery2019.UI.Details;
using SharpDX;
using SharpDX.Direct2D1;
using EngineShapes = FarseerPhysics.Collision.Shapes;
using Xna = Microsoft.Xna.Framework;

namespace Lottery2019.UI.Shapes
{
    public class LineShape : Shape
    {
        public LineShape()
        {
        }

        public LineShape(JsonShape jsonShape, Vector2 center) : base(jsonShape, center)
        {
            var p1 = jsonShape.Points[0];
            var p2 = jsonShape.Points[1];
            Start = new Vector2(p1[0], p1[1]);
            End = new Vector2(p2[0], p2[1]);
        }

        public Vector2 Start { get; private set; }

        public Vector2 End { get; private set; }

        public override void Draw(DeviceContext renderTarget, Brush brush)
        {
            renderTarget.DrawLine(Start, End, brush, 2.0f);
        }

        public override bool TestPoint(Vector2 point)
        {
            var p = new Xna.Vector2(point.X, point.Y);
            var start = new Xna.Vector2(Start.X, Start.Y);
            var end = new Xna.Vector2(End.X, End.Y);
            var distance = LineTools.DistanceBetweenPointAndLineSegment(ref p,
                ref start, ref end);
            return distance < 1.0f;
        }

        public override EngineShapes.Shape ToEngineShape()
        {
            return new EngineShapes.EdgeShape(
                (Start + Offset - Center).ToXnaVector2(),
                (End + Offset - Center).ToXnaVector2());
        }
    }
}
