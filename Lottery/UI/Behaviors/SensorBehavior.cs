using FlysEngine.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class SensorBehavior : Behavior
    {
        protected override void OnSpriteSet(Sprite sprite)
        {
            base.OnSpriteSet(sprite);
            sprite.Body.IsSensor = true;
            foreach (Sprite child in sprite.Children) child.Body.IsSensor = true;
        }
    }
}
