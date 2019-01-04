using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;
using Direct2D1 = SharpDX.Direct2D1;

namespace Lottery2019.UI.Details
{
    public class SolidBrushManager : IDisposable
    {
        Direct2D1.RenderTarget _renderTarget;

        Dictionary<SharpDX.Color, Direct2D1.SolidColorBrush> _brushMap
            = new Dictionary<SharpDX.Color, Direct2D1.SolidColorBrush>();

        public void Dispose()
        {
            ReleaseDeviceResources();
        }

        public void ReleaseDeviceResources()
        {
            foreach (var brush in _brushMap.Values)
            {
                brush.Dispose();
            }
            _brushMap.Clear();
        }

        public Direct2D1.SolidColorBrush Get(Color color)
        {
            Debug.Assert(_renderTarget != null);

            if (!_brushMap.ContainsKey(color))
            {
                _brushMap[color] = new Direct2D1.SolidColorBrush(_renderTarget, color.ToColor4());
            }
            return _brushMap[color];
        }

        public Direct2D1.SolidColorBrush Get(Color color, float opacity)
        {
            color.A = (byte)(255 * opacity);
            return Get(color);
        }

        public void SetRenderTarget(Direct2D1.RenderTarget renderTarget)
        {
            _renderTarget = renderTarget;
        }

        public void ReleaseAllColor3(Color3 color)
        {
            var colorsToRemove = _brushMap.Keys
                .Where(x => x.ToColor3() == color)
                .ToList();
            foreach (var c in colorsToRemove)
                _brushMap.Remove(c);
        }
    }
}
