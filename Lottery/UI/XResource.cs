using System;
using Direct2D1 = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using DWrite = SharpDX.DirectWrite;
using WIC = SharpDX.WIC;
using DirectInput = SharpDX.DirectInput;
using Animation = SharpDX.Animation;
using SharpDX;
using System.Windows.Forms;
using XnaVector2 = Duality.Vector2;
using FarseerPhysics.Dynamics;
using FlysEngine.Tools;

namespace Lottery2019.UI
{
    public class XResource2 : IDisposable
    {
        public readonly Direct2D1.Factory1 Direct2DFactory = new Direct2D1.Factory1();
        public readonly DWrite.Factory DWriteFactory = new DWrite.Factory(DWrite.FactoryType.Shared);
        public readonly WIC.ImagingFactory2 WICFactory = new WIC.ImagingFactory2();

        public readonly Animation.Manager AnimationManager = new Animation.Manager();
        public readonly Animation.TransitionLibrary TransitionLibrary = new Animation.TransitionLibrary();
        private readonly DirectInput.DirectInput DirectInput = new DirectInput.DirectInput();
        private readonly DirectInput.Keyboard Keyboard;
        public DirectInput.KeyboardState KeyboardState = new DirectInput.KeyboardState();
        private readonly DirectInput.Mouse Mouse;
        public DirectInput.MouseState MouseState = new DirectInput.MouseState();
        public Vector2 MouseClientPosition = new Vector2();
        public readonly World World = new World(new XnaVector2(0f, 9.82f));

        public DXGI.SwapChain1 SwapChain;
        public Direct2D1.DeviceContext RenderTarget;

        public const float Restitution = 1.0f;
        public const float Friction = 0.0f;

        public Matrix3x2 WorldTransform;

        public XResource2()
        {
            Keyboard = new DirectInput.Keyboard(DirectInput);
            Keyboard.Acquire();
            Mouse = new DirectInput.Mouse(DirectInput);
            Mouse.Acquire();
        }

        public Vector2 InvertTransformPoint(Matrix3x2 matrix, Vector2 point)
        {
            matrix.Invert();
            return Matrix3x2.TransformPoint(matrix, point);
        }

        public bool DeviceAvailable
            => RenderTarget != null && !RenderTarget.IsDisposed;

        public void Resize()
        {
            if (!DeviceAvailable)
                return;
            
            RenderTarget.Target = null;
            SwapChain.ResizeBuffers(0, 0, 0, DXGI.Format.Unknown, DXGI.SwapChainFlags.None);
            DirectXTools.CreateDeviceSwapChainBitmap(SwapChain, RenderTarget);
        }

        public Animation.Variable CreateAnimation(double initialValue, double finalValue, double duration)
        {
            var var = new Animation.Variable(AnimationManager, initialValue);
            AnimationManager.ScheduleTransition(var,
                TransitionLibrary.AccelerateDecelerate(duration, finalValue, 0.2, 0.8), _timeNow);
            return var;
        }

        double _timeNow = 0.0;
        public void UpdateLogic(float dt, Form form)
        {
            _timeNow += dt;
            AnimationManager.Update(_timeNow);
            if (form.Focused)
            {
                Keyboard.GetCurrentState(ref KeyboardState);
            }
            else
            {
                KeyboardState.PressedKeys.Clear();
            }
            
            Mouse.GetCurrentState(ref MouseState);
            MouseClientPosition = form.PointToClient(Cursor.Position).ToVector2();
            
            if (!new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height).Contains(
                MouseClientPosition.X, MouseClientPosition.Y))
            {
                MouseState.Z = 0;
            }
        }

        public void InitializeDevice(IntPtr windowHandle)
        {
            using (var d3device = DirectXTools.CreateD3Device())
            {
                RenderTarget = DirectXTools.CreateRenderTarget(Direct2DFactory, d3device);
                SwapChain = DirectXTools.CreateSwapChainForHwnd(d3device, windowHandle);
                DirectXTools.CreateDeviceSwapChainBitmap(SwapChain, RenderTarget);
            }
        }

        public void ReleaseDeviceResources()
        {
            SwapChain.Dispose();
            RenderTarget.Dispose();
        }

        public void Dispose()
        {
            ReleaseDeviceResources();

            Keyboard.Dispose();
            Mouse.Dispose();
            DirectInput.Dispose();

            TransitionLibrary.Dispose();
            AnimationManager.Dispose();
            
            Direct2DFactory.Dispose();
            DWriteFactory.Dispose();
            WICFactory.Dispose();
            Keyboard.Dispose();
            DirectInput.Dispose();
        }
    }
}
