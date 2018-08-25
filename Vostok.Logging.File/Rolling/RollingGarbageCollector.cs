﻿using System;
using System.Linq;

namespace Vostok.Logging.File.Rolling
{
    internal class RollingGarbageCollector : IRollingGarbageCollector
    {
        private readonly IFileSystem fileSystem;
        private readonly Func<int> filesToKeepProvider;

        public RollingGarbageCollector(IFileSystem fileSystem, Func<int> filesToKeepProvider)
        {
            this.fileSystem = fileSystem;
            this.filesToKeepProvider = filesToKeepProvider;
        }

        public void RemoveStaleFiles(string[] allFiles)
        {
            var filesToKeep = filesToKeepProvider();
            if (filesToKeep < 1)
                return;

            foreach (var file in allFiles.Take(allFiles.Length - filesToKeep))
                fileSystem.TryRemoveFile(file);
        }
    }
}