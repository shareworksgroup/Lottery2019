using FlysEngine.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class RestitutionBehavior : Behavior
    {
        public float Restitution { get; set; } = 2.5f;
        public float Friction { get; set; } = 0.0f;

        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);

            foreach (var fixture in sprite.Body.FixtureList)
            {
                fixture.Restitution = Restitution;
                fixture.Friction = Friction;
            }
        }
    }
}
