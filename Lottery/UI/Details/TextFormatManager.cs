using System;
using System.Collections.Generic;
using DWrite = SharpDX.DirectWrite;

namespace Lottery2019.UI.Details
{
    public class TextFormatManager : IDisposable
    {
        private readonly Dictionary<string, DWrite.TextFormat> formatMap = new Dictionary<string, DWrite.TextFormat>();
        private readonly DWrite.Factory _factory;

        public TextFormatManager(DWrite.Factory factory)
        {
            _factory = factory;
        }

        public DWrite.TextFormat Get(float fontSize, string fontFamilyName = "Consolas")
        {
            var key = $"{fontFamilyName}:{fontSize}";
            if (!formatMap.ContainsKey(key))
            {
                formatMap[key] = new DWrite.TextFormat(_factory, fontFamilyName, fontSize);
            }

            return formatMap[key];
        }

        public void Dispose()
        {
            foreach (var format in formatMap.Values)
            {
                format.Dispose();
            }
        }
    }
}
