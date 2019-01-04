using System;
using System.Collections.Generic;
using WIC = SharpDX.WIC;
using Direct2D1 = SharpDX.Direct2D1;

namespace Lottery2019.UI.Details
{
    public class BitmapManager : IDisposable
    {
        private readonly WIC.ImagingFactory _imagingFactory;
        private readonly Dictionary<string, Direct2D1.Bitmap1> _bmps = new Dictionary<string, Direct2D1.Bitmap1>();
        private Direct2D1.DeviceContext _renderTarget;

        public BitmapManager(WIC.ImagingFactory imagingFactory)
        {
            _imagingFactory = imagingFactory;
        }

        public void SetRenderTarget(Direct2D1.DeviceContext renderTarget)
        {
            _renderTarget = renderTarget;
        }

        public Direct2D1.Bitmap1 Get(string filename)
        {
            EnsureLoadBitmap(filename);
            return _bmps[filename];
        }

        public void EnsureLoadBitmap(string filename)
        {
            if (!_bmps.ContainsKey(filename))
            {
                _bmps[filename] = CreateD2dBitmap(
                    _imagingFactory,
                    filename,
                    _renderTarget);
            }
        }

        private static Direct2D1.Bitmap1 CreateD2dBitmap(
            WIC.ImagingFactory imagingFactory, 
            string filename, 
            Direct2D1.DeviceContext renderTarget)
        {
            var decoder = new WIC.BitmapDecoder(imagingFactory, filename, WIC.DecodeOptions.CacheOnDemand);
            var frame = decoder.GetFrame(0);

            var image = new WIC.FormatConverter(imagingFactory);
            image.Initialize(frame, WIC.PixelFormat.Format32bppPBGRA);

            return Direct2D1.Bitmap1.FromWicBitmap(renderTarget, image);
        }

        public void ReleaseDeviceResources()
        {
            foreach (var kv in _bmps)
            {
                kv.Value.Dispose();
            }
            _bmps.Clear();
        }

        public void Dispose()
        {
            ReleaseDeviceResources();
        }
    }
}
