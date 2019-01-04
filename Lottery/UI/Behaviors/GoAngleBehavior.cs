using Lottery2019.UI.Sprites;
using SharpDX;

namespace Lottery2019.UI.Behaviors
{
    public class GoAngleBehavior : StagesBehavior
    {
        public override float[] Stages { get; set; } = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
        public float Rotation { get; set; }

        public override float Length => MathUtil.DegreesToRadians(Rotation);

        public GoAngleBehavior(Sprite sprite) : base(sprite)
        {
        }

        protected override void ByDx(float dx)
        {
            switch (StageId)
            {
                case 0:
                    Sprite.Rotation += dx;
                    break;
                case 1:
                case 3:
                    break;
                case 2:
                    Sprite.Rotation -= dx;
                    break;
            }
        }
    }
}
