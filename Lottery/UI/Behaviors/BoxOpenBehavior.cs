using System.Linq;
using SharpDX;
using SharpDX.Direct2D1;
using FlysEngine.Sprites;
using FlysEngine.Sprites.Shapes;

namespace Lottery2019.UI.Behaviors
{
    public class BoxOpenBehavior : Behavior
    {
        private float _dtAll;
        private bool _started = false;

        public float Duration { get; set; }

        public override void Update(float dt)
        {
            if (!_started) return;

            if (Sprite.FrameId == Sprite.Frames.Length - 1)
            {
                _started = false;
                Sprite.SetShapes();
                return;
            }

            _dtAll += dt;
            float stageDt = Duration / Sprite.Frames.Length;
            if (_dtAll >= stageDt)
            {
                Sprite.FrameId++;
                _dtAll = _dtAll - stageDt;
            }
        }

        public void Open() => _started = true;
    }
}
