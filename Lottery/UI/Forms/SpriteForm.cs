using FarseerPhysics.Dynamics;
using Lottery2019.UI.Sprites.Loader;
using SharpDX;
using System.Diagnostics;
using XnaVector2 = Duality.Vector2;
using FlysEngine.Sprites;
using SharpDX.Direct2D1;
using System.Linq;

namespace Lottery2019.UI.Forms
{
    public abstract partial class SpriteForm : SpriteWindow
    {        
        protected float CameraY = 0.0f;
        protected const float StageWidth = 1920.0f;
        protected const float StageHeight = 1080.0f;
        public const float MaxStage = 2500.0f - StageHeight;
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
            
            AddSprites(
                _loader.CreateSprites(stagePath, this)
                .ToArray());
            
            OnSpriteCreated();
        }

        protected override void OnDraw(DeviceContext renderTarget)
        {
            var worldScale = renderTarget.Size.Width / StageWidth;
            var worldOffset = -CameraY * worldScale;

            GlobalTransform = Matrix3x2.Scaling(worldScale) * Matrix3x2.Translation(0.0f, worldOffset);

            base.OnDraw(renderTarget);

            if (KeyboardState.IsPressed(SharpDX.DirectInput.Key.LeftShift))
            {
                DrawDiagnostics(renderTarget);
            }
        }

        private void DrawDiagnostics(DeviceContext renderTarget)
        {
            renderTarget.Transform = Matrix3x2.Identity;
            Matrix3x2 wt = GlobalTransform;
            wt.Invert();
            var worldPos = Matrix3x2.TransformPoint(wt, MouseClientPosition);

            renderTarget.DrawText(
                $"FPS: {RenderTimer.FramesPerSecond:0}\r\nFrameTime: {RenderTimer.DurationSinceLastFrame}\r\n({worldPos.X:0}, {worldPos.Y:0})",
                XResource.TextFormats[11],
                new RectangleF(0, 0, 100, 30),
                XResource.GetColor(Color.White));
        }

        protected abstract void OnSpriteCreated();
    }
}
