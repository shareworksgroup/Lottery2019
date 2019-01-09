using SharpDX;
using FlysEngine.Sprites;
using FlysEngine.Sprites.Shapes;

namespace Lottery2019.UI.Behaviors
{
    public class CircleCreatorBehavior : Behavior
    {
        private Vector2 _lastCenter;

        public override void Update(float dt)
        {
            var res = Sprite.XResource;
            var win = Sprite.Window;

            var pos = res.InvertTransformPoint(win.GlobalTransform, win.MouseClientPosition);
            if (win.MouseState.Buttons[0] &&
                win.KeyboardState.IsPressed(SharpDX.DirectInput.Key.LeftShift) && 
                pos != _lastCenter)
            {
                _lastCenter = pos;

                var child = new Sprite(win);
                child.Body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
                child.Position = pos;
                child.Name = "Debug";
                child.Center = new Vector2(15, 15);
                child.AddBehavior(new AutoBorderBehavior());
                child.SetShapes(new CircleShape(14.0f)
                {
                    Center = child.Center,
                });
                child.Frames = new[]
                {
                    "./Resources/Sprites/Nail.png"
                };
                Sprite.Children.Add(child);
            }
        }
    }
}
