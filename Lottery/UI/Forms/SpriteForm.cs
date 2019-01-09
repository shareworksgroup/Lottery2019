using FarseerPhysics.Dynamics;
using Lottery2019.UI.Sprites.Loader;
using SharpDX;
using System.Diagnostics;
using XnaVector2 = Duality.Vector2;
using FlysEngine.Sprites;
using SharpDX.Direct2D1;

namespace Lottery2019.UI.Forms
{
    public abstract partial class SpriteForm : SpriteWindow
    {        
        protected float CameraY = 0.0f;
        protected const float StageWidth = 1920.0f;
        protected const float StageHeight = 1080.0f;
        public const float MaxStage = 3200.0f - StageHeight;
        SpriteLoader _loader;

        public SpriteForm(string spritePath, string stagePath)
        {
            World.Gravity = new XnaVector2(0, 9.82f);

            InitializeComponent();
            _loader = new SpriteLoader(spritePath);
            ResetStage(stagePath);
        }

        public Vector2 InvertTransformPoint(Matrix3x2 matrix, Vector2 point)
        {
            matrix.Invert();
            return Matrix3x2.TransformPoint(matrix, point);
        }

        protected void ResetStage(string stagePath)
        {
            foreach (var sprite in Sprites.Values)
                sprite.Dispose();
            Sprites.Clear();
            World.ProcessChanges();
            Debug.Assert(World.BodyList.Count == 0);

            foreach (var sprite in _loader.CreateSprites(stagePath, this))
            {
                Sprites.Add(sprite.Id, sprite);
            }
            
            OnSpriteCreated();
        }

        protected override void OnDraw(DeviceContext renderTarget)
        {
            var worldScale = renderTarget.Size.Width / StageWidth;
            var worldOffset = -CameraY * worldScale;

            GlobalTransform = Matrix3x2.Scaling(worldScale) * Matrix3x2.Translation(0.0f, worldOffset);

            base.OnDraw(renderTarget);
        }

        protected abstract void OnSpriteCreated();
    }
}
