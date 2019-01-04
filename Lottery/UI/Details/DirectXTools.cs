using System;
using System.Linq;
using Direct3D = SharpDX.Direct3D;
using Direct3D11 = SharpDX.Direct3D11;
using Direct2D1 = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using WIC = SharpDX.WIC;
using SharpDX.IO;
using System.IO;

namespace Lottery2019.UI.Details
{
    public static class DirectXTools
    {
        public static Direct3D11.Device CreateD3Device()
        {
            var supportedFeatureLevels = new[]
            {
                Direct3D.FeatureLevel.Level_11_1,
                Direct3D.FeatureLevel.Level_11_0,
                Direct3D.FeatureLevel.Level_10_1,
                Direct3D.FeatureLevel.Level_10_0,
            };

            var supportedDrivers = new[]
            {
                Direct3D.DriverType.Hardware,
                Direct3D.DriverType.Warp,
            };

            foreach (var driver in supportedDrivers)
            {
                try
                {
                    return new Direct3D11.Device(
                        driver,
                        Direct3D11.DeviceCreationFlags.BgraSupport,
                        supportedFeatureLevels);
                }
                catch (SharpDX.SharpDXException)
                {
                    if (driver == supportedDrivers.Last())
                        throw;
                }
            }

            throw new NotSupportedException();
        }

        public static Direct2D1.DeviceContext CreateRenderTarget(
            Direct2D1.Factory1 factory2d,
            Direct3D11.Device device3d)
        {
            var dxgiDevice = device3d.QueryInterface<DXGI.Device>();
            using (var device2d = new Direct2D1.Device(factory2d, dxgiDevice))
            {
                return new Direct2D1.DeviceContext(
                    device2d,
                    Direct2D1.DeviceContextOptions.None);
            }
        }

        public static DXGI.SwapChain1 CreateSwapChainForHwnd(
            Direct3D11.Device device,
            IntPtr hwnd)
        {
            var dxgiDevice = device.QueryInterface<DXGI.Device>();
            var dxgiFactory = dxgiDevice.Adapter.GetParent<DXGI.Factory2>();
            var dxgiDesc = new DXGI.SwapChainDescription1
            {
                Format = DXGI.Format.B8G8R8A8_UNorm,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                Usage = DXGI.Usage.RenderTargetOutput,
                BufferCount = 2,
            };
            return new DXGI.SwapChain1(
                dxgiFactory,
                device,
                hwnd,
                ref dxgiDesc);
        }

        public static void CreateDeviceSwapChainBitmap(
            DXGI.SwapChain1 swapChain,
            Direct2D1.DeviceContext target)
        {
            using (var surface = swapChain.GetBackBuffer<DXGI.Surface>(0))
            {
                var props = new Direct2D1.BitmapProperties1
                {
                    BitmapOptions = Direct2D1.BitmapOptions.Target | Direct2D1.BitmapOptions.CannotDraw,
                    PixelFormat = new Direct2D1.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, Direct2D1.AlphaMode.Ignore)
                };
                using (var bitmap = new Direct2D1.Bitmap1(target, surface, props))
                {
                    target.Target = bitmap;
                }
            }
        }

        public static void SaveD2DBitmap(WIC.ImagingFactory wic, WIC.Bitmap wicBitmap, string filename)
        {
            using (var stream = File.Create(filename))
            using (var encoder = new WIC.BitmapEncoder(wic, WIC.ContainerFormatGuids.Png))
            {
                encoder.Initialize(stream);
                using (var frame = new WIC.BitmapFrameEncode(encoder))
                {
                    frame.Initialize();
                    frame.SetSize(wicBitmap.Size.Width, wicBitmap.Size.Height);

                    var pixelFormat = wicBitmap.PixelFormat;
                    frame.SetPixelFormat(ref pixelFormat);
                    frame.WriteSource(wicBitmap);

                    frame.Commit();
                    encoder.Commit();
                }
            }
        }

        public static WIC.FormatConverter CreateWicImage(WIC.ImagingFactory wic, string rawFile)
        {
            using (var decoder = new WIC.JpegBitmapDecoder(wic))
            using (var decodeStream = new WIC.WICStream(wic, rawFile, NativeFileAccess.Read))
            {
                decoder.Initialize(decodeStream, WIC.DecodeOptions.CacheOnDemand);
                using (var decodeFrame = decoder.GetFrame(0))
                {
                    var converter = new WIC.FormatConverter(wic);
                    converter.Initialize(decodeFrame, WIC.PixelFormat.Format32bppPBGRA);
                    return converter;
                }
            }
        }
    }
}
