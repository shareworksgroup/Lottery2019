using System;
using System.Windows.Forms;
using DXGI = SharpDX.DXGI;
using Direct2D1 = SharpDX.Direct2D1;
using SharpDX;
using Color = SharpDX.Color;
using System.Diagnostics;
using SharpDX.Windows;
using System.Threading;

namespace Lottery2019.UI.Forms
{
    public abstract partial class Direct2DForm : RenderForm
    {
        protected readonly XResource XResource = new XResource();

        public Direct2DForm()
        {
            InitializeComponent();

            Resize += Form1_Resize;
            FormClosed += Form1_FormClosed;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            XResource.Dispose();
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;
            XResource.Resize();
        }

        public void Render()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Thread.Sleep(1);
                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            if (!XResource.DeviceAvailable)
                XResource.InitializeDevice(Handle);

            var dt = UpdateFpsGetDt();
            if (dt < 0.2f)
            {
                XResource.UpdateLogic(dt, this);
                UpdateLogic(dt);
            }
            RenderInternal(sw);

            _frameTimeMs = sw.ElapsedMilliseconds;
            ++_frames;
        }

        private void RenderInternal(Stopwatch stopwatch)
        {
            try
            {
                XResource.RenderTarget.BeginDraw();
                XResource.RenderTarget.Clear(Color.CornflowerBlue.ToColor4());
                XResource.RenderTarget.Transform = Matrix3x2.Identity;
                Draw(XResource.RenderTarget);
                XResource.RenderTarget.EndDraw();
                stopwatch.Stop();
                XResource.SwapChain.Present(1, DXGI.PresentFlags.None);
            }
            catch (SharpDXException e)
            {
                unchecked
                {
                    const int DeviceRemoved = (int)0x887a0005;
                    const int DeviceReset = (int)0x887A0007;
                    if (e.ResultCode == DeviceRemoved || e.ResultCode == DeviceReset)
                    {
                        XResource.ReleaseDeviceResources();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

        }

        int _frames = 0;
        long _lastFpsTick = Stopwatch.GetTimestamp();
        long _lastTick = Stopwatch.GetTimestamp();
        long _frameTimeMs = 0;
        protected float _fps = 0;
        private float UpdateFpsGetDt()
        {
            var currentTs = Stopwatch.GetTimestamp();
            var duration = currentTs - _lastFpsTick;
            if (duration > Stopwatch.Frequency)
            {
                _fps = 1.0f * _frames / duration * Stopwatch.Frequency;
                _lastFpsTick = currentTs;
                _frames = 0;
            }
            var dt = 1.0f * (currentTs - _lastTick) / Stopwatch.Frequency;
            _lastTick = currentTs;
            return dt;
        }

        private void DrawDiagnostics(Direct2D1.DeviceContext renderTarget)
        {
            var wt = XResource.WorldTransform;
            wt.Invert();
            var worldPos = Matrix3x2.TransformPoint(wt, XResource.MouseClientPosition);

            renderTarget.DrawText(
                $"FPS: {_fps:0}\r\nFrameTime: {_frameTimeMs}\r\n({worldPos.X:0}, {worldPos.Y:0})",
                XResource.TextFormats.Get(11),
                new RectangleF(0, 0, 100, 30),
                XResource.Brushes.Get(Color.White));
        }

        protected virtual void UpdateLogic(float dt)
        {

        }

        protected virtual void Draw(Direct2D1.DeviceContext renderTarget)
        {
            if (XResource.KeyboardState.IsPressed(SharpDX.DirectInput.Key.LeftShift))
            {
                renderTarget.Transform = Matrix3x2.Identity;
                DrawDiagnostics(renderTarget);
            }
        }
    }
}
