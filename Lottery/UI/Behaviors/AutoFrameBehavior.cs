using Lottery2019.UI.Sprites;

namespace Lottery2019.UI.Behaviors
{
    public class AutoFrameBehavior : Behavior
    {
        public float Fps { get; set; } = 5.0f;
        private float _accumulateTime = 0.0f;

        public AutoFrameBehavior(Sprite sprite) :
            base(sprite)
        {
        }

        public override void UpdateLogic(float dt)
        {
            var frameTime = 1.0f / Fps;
            _accumulateTime += dt;
            while (_accumulateTime > frameTime)
            {
                Sprite.FrameId = (Sprite.FrameId + 1) % Sprite.Frames.Length;
                _accumulateTime -= frameTime;
            }
        }
    }
}
