using FarseerPhysics.Common;
using Lottery2019.UI.Details;
using SharpDX;
using SharpDX.Direct2D1;
using EngineShapes = FarseerPhysics.Collision.Shapes;

namespace Lottery2019.UI.Shapes
{
    public class RectangleShape : Shape
    {
        public RectangleShape()
        {
        }

        public RectangleShape(JsonShape jsonShape, Vector2 center) : base(jsonShape, center)
        {
            Size = new Vector2(jsonShape.Size[0], jsonShape.Size[1]);
        }

        public Vector2 Size { get; set; }

        public override void Draw(DeviceContext renderTarget, Brush brush)
        {
            var test = new RectangleF(
                        Offset.X,
                        Offset.Y,
                        Size.X,
                        Size.Y);
            renderTarget.DrawRectangle(test,
                brush,
                2.0f);
        }

        public override bool TestPoint(Vector2 point)
        {
            return new RectangleF(
                        Offset.X,
                        Offset.Y,
                        Size.X,
                        Size.Y).Contains(point);
        }

        public override EngineShapes.Shape ToEngineShape()
        {
            var offset = Offset - Center;
            return new EngineShapes.PolygonShape(Density)
            {
                Vertices = new Vertices(new[]
                {
                    (offset).ToXnaVector2(),
                    (offset + new Vector2(Size.X, 0)).ToXnaVector2(),
                    (offset + Size).ToXnaVector2(),
                    (offset + new Vector2(0, Size.Y)).ToXnaVector2(),
                })
            };
        }
    }
}
