using Lottery2019.UI.Sprites;
using SharpDX;

namespace Lottery2019.UI.Behaviors
{
    public class ChomperBehavior : StagesBehavior
    {
        public override float[] Stages { get; set; } = new[] { 0.5f, 2.0f, 0.5f, 0.5f };
        public float Distance { get; set; } = 50.0f;

        public override float Length => Distance;

        public ChomperBehavior(Sprite sprite) :
            base(sprite)
        {
        }

        protected override void ByDx(float dx)
        {
            var oldPos = Sprite.Position;
            switch (StageId)
            {
                case 0: // down
                    Sprite.Position = new Vector2(oldPos.X, oldPos.Y + dx);
                    break;
                case 1: // down-steady
                    Sprite.Body.Enabled = false;
                    Sprite.Alpha = 0.0f;
                    break;
                case 2: // up
                    Sprite.Body.Enabled = true;
                    Sprite.Alpha = 1.0f;
                    Sprite.Position = new Vector2(oldPos.X, oldPos.Y - dx);
                    break;
                case 3: // up-steady
                    break;                
            }
        }
    }
}
