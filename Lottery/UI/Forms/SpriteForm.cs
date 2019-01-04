using FarseerPhysics.Dynamics;
using FlysEngine.Desktop;
using Lottery2019.UI.Sprites;
using Lottery2019.UI.Sprites.Loader;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using XnaVector2 = Duality.Vector2;
using DirectInput = SharpDX.DirectInput;

namespace Lottery2019.UI.Forms
{
    public abstract partial class SpriteForm : RenderWindow
    {
        private readonly DirectInput.DirectInput DirectInput = new DirectInput.DirectInput();
        private readonly DirectInput.Keyboard Keyboard;
        public DirectInput.KeyboardState KeyboardState = new DirectInput.KeyboardState();
        private readonly DirectInput.Mouse Mouse;
        public DirectInput.MouseState MouseState = new DirectInput.MouseState();
        public Vector2 MouseClientPosition = new Vector2();

        protected readonly List<Sprite> Sprites = new List<Sprite>();
        protected float CameraY = 0.0f;
        protected const float StageWidth = 1920.0f;
        protected const float StageHeight = 1080.0f;
        public const float MaxStage = 3200.0f - StageHeight;
        SpriteLoader _loader;
        public World World { get; } = new World(new XnaVector2(0f, 9.82f));
        public Matrix3x2 WorldTransform { get; private set; }

        public SpriteForm(string spritePath, string stagePath)
        {
            Keyboard = new DirectInput.Keyboard(DirectInput);
            Keyboard.Acquire();
            Mouse = new DirectInput.Mouse(DirectInput);
            Mouse.Acquire();

            InitializeComponent();
            _loader = new SpriteLoader(spritePath);
            ResetStage(stagePath);
        }

        public Vector2 InvertTransformPoint(Matrix3x2 matrix, Vector2 point)
        {
            matrix.Invert();
            return Matrix3x2.TransformPoint(matrix, point);
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
            World.ProcessChanges();
            Debug.Assert(World.BodyList.Count == 0);
            
            Sprites.AddRange(_loader.CreateSprites(stagePath, this));
            OnSpriteCreated();
        }

        protected abstract void OnSpriteCreated();

        protected override void OnUpdateLogic(float lastFrameTimeInSecond)
        {
            base.OnUpdateLogic(lastFrameTimeInSecond);

            if (Focused)
            {
                Keyboard.GetCurrentState(ref KeyboardState);
            }
            else
            {
                KeyboardState.PressedKeys.Clear();
            }

            Mouse.GetCurrentState(ref MouseState);
            MouseClientPosition = PointToClient(System.Windows.Forms.Cursor.Position).ToVector2();

            if (!new Rectangle(0, 0, ClientSize.Width, ClientSize.Height).Contains(
                MouseClientPosition.X, MouseClientPosition.Y))
            {
                MouseState.Z = 0;
            }

            foreach (var sprite in Sprites)
            {
                sprite.UpdateLogic(lastFrameTimeInSecond);
            }
        }

        protected override void OnDraw(DeviceContext renderTarget)
        {
            var worldScale = renderTarget.Size.Width / StageWidth;
            var worldOffset = -CameraY * worldScale;

            WorldTransform = Matrix3x2.Scaling(worldScale) * Matrix3x2.Translation(0.0f, worldOffset);
            renderTarget.Transform = WorldTransform;

            foreach (var sprite in Sprites)
            {
                sprite.Draw(renderTarget);
            }

            base.OnDraw(renderTarget);
        }
    }
}
