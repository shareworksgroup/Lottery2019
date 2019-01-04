using System.Collections.Generic;
using Lottery2019.UI.Sprites;
using Lottery2019.UI.Shapes;
using SharpDX;

namespace Lottery2019.UI.Behaviors
{
    public class CircleCreatorBehavior : Behavior
    {
        public CircleCreatorBehavior(Sprite sprite) : base(sprite)
        {
        }

        private Vector2 _lastCenter;

        public override void UpdateLogic(float dt)
        {
            var res = Sprite.XResource;
            var pos = res.InvertTransformPoint(res.WorldTransform, res.MouseClientPosition);
            if (Sprite.XResource.MouseState.Buttons[0] && 
                Sprite.XResource.KeyboardState.IsPressed(SharpDX.DirectInput.Key.LeftShift) && 
                pos != _lastCenter)
            {
                _lastCenter = pos;

                var child = new Sprite(Sprite.XResource);                
                child.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
                child.Position = pos;
                child.SpriteType = "Debug";
                child.Center = new Vector2(15, 15);
                child.Behaviors["default"] = new AutoBorderBehavior(child);
                child.Shapes = new List<Shape>
                {
                    new CircleShape(14.0f)
                    {
                        Center = child.Center, 
                    }
                };
                child.Frames = new[]
                {
                    "./Resources/Sprites/Nail.png"
                };
                Sprite.Children.Add(child);
            }
        }
    }
}
