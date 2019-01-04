using System;
using System.Collections.Generic;
using Direct2D1 = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;

namespace Lottery2019.UI.Details
{
    public class TextLayoutManager : IDisposable
    {
        private readonly DWrite.Factory _dwriteFactory;
        private readonly TextFormatManager _textManager;
        private readonly SolidBrushManager _brushManager;
        private readonly Dictionary<string, DWrite.TextLayout> _bmps = new Dictionary<string, DWrite.TextLayout>();
        private Direct2D1.DeviceContext _renderTarget;

        public TextLayoutManager(DWrite.Factory dwriteFactory, SolidBrushManager brushManager, TextFormatManager textManager)
        {
            _dwriteFactory = dwriteFactory;
            _brushManager = brushManager;
            _textManager = textManager;
        }

        public void SetRenderTarget(Direct2D1.DeviceContext renderTarget)
        {
            _renderTarget = renderTarget;
        }

        public DWrite.TextLayout Get(string text, float fontSize)
        {
            var key = $"{text}:{fontSize}";
            if (!_bmps.ContainsKey(key))
            {
                _bmps[key] = new DWrite.TextLayout(_dwriteFactory,
                    text,
                    _textManager.Get(fontSize),
                    float.MaxValue, fontSize * 2.0f);
            }

            return _bmps[key];
        }

        public void Remove(string text, float fontSize)
        {
            var key = $"{text}:{fontSize}";
            if (_bmps.ContainsKey(key))
            {
                var val = _bmps[key];
                val.Dispose();
                _bmps.Remove(key);
            }
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
