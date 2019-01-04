using FlysEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lottery2019.UI.Sprites.Loader
{
    public class JsonSpriteWatcher
    {
        private readonly string _path;
        private readonly XResource _xResource;
        private readonly FileSystemWatcher _watcher;

        public JsonSpriteWatcher(string path, XResource xResource)
        {
            _path = path;
            _xResource = xResource;
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));
            _watcher.EnableRaisingEvents = true;
            _watcher.NotifyFilter = 
                NotifyFilters.Security |
                NotifyFilters.CreationTime |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.Attributes |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName;
            _watcher.Changed += Changed;
        }

        public event Action<IEnumerable<Sprite>> OnNewSprites; 

        private void Changed(object sender, FileSystemEventArgs e)
        {
            RaiseEvent();
        }

        public IEnumerable<Sprite> Poll()
        {
            //return SpriteLoader.CreateSprites(_path, _xResource);
            throw new NotImplementedException();
        }

        void RaiseEvent()
        {
            try
            {
                OnNewSprites?.Invoke(Poll());
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
