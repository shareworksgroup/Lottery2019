using Lottery2019.UI.Sprites;
using Lottery2019.UI.Sprites.Loader;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lottery2019.UI.Forms
{
    public abstract partial class SpriteForm : Direct2DForm
    {
        protected readonly List<Sprite> Sprites = new List<Sprite>();
        protected float CameraY = 0.0f;
        protected const float StageWidth = 1920.0f;
        protected const float StageHeight = 1080.0f;
        public const float MaxStage = 3200.0f - StageHeight;
        SpriteLoader _loader;

        public SpriteForm(string spritePath, string stagePath)
        {
            InitializeComponent();
            _loader = new SpriteLoader(spritePath);
            ResetStage(stagePath);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected void ResetStage(string stagePath)
        {
            foreach (var sprite in Sprites)
                sprite.Dispose();
            Sprites.Clear();
            XResource.World.ProcessChanges();
            Debug.Assert(XResource.World.BodyList.Count == 0);
            
            Sprites.AddRange(_loader.CreateSprites(stagePath, XResource));
            OnSpriteCreated();
        }

        protected abstract void OnSpriteCreated();

        protected override void UpdateLogic(float dt)
        {
            foreach (var sprite in Sprites)
            {
                sprite.UpdateLogic(dt);
            }
        }

        protected override void Draw(DeviceContext renderTarget)
        {
            var worldScale = renderTarget.Size.Width / StageWidth;
            var worldOffset = -CameraY * worldScale;

            XResource.WorldTransform = Matrix3x2.Scaling(worldScale) * Matrix3x2.Translation(0.0f, worldOffset);
            renderTarget.Transform = XResource.WorldTransform;

            foreach (var sprite in Sprites)
            {
                sprite.Draw(renderTarget);
            }
            base.Draw(renderTarget);
        }
    }
}
