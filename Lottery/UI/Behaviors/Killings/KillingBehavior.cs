using Lottery2019.UI.Sprites;

namespace Lottery2019.UI.Behaviors.Killings
{
    public abstract class KillingBehavior : Behavior
    {
        public KillingBehavior(Sprite sprite) : base(sprite)
        {
        }

        public PersonSprite PersonSprite => (PersonSprite)Sprite;

        public abstract void Kill();
    }
}
