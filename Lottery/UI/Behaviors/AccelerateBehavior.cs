using FlysEngine.Sprites;
using SharpDX;
using System;

namespace Lottery2019.UI.Behaviors
{
    public class AccelerateBehavior : Behavior
    {
        public float Angle { get; set; }

        public float Force { get; set; }

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);

            sprite.Body.IsSensor = true;
            sprite.Hit += Sprite_Hit;
        }

        private void Sprite_Hit(object sender, Sprite e)
        {
            Accelerate(e);
        }

        public void Accelerate(Sprite sprite)
        {
            float radians = MathUtil.DegreesToRadians(Angle);
            sprite.Body.ApplyForce(
                new Vector2(
                    Force * (float)Math.Sin(radians), 
                    -Force * (float)Math.Cos(radians))
                .ToSimulation());
        }
    }
}
