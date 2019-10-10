using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace Config.Docker
{
    public class DockerSecretParser
    {
        private readonly IFileSystem _fileSystem;

        // TODO: replace for appsettings value
        private readonly string DOCKER_SECRET_PATH = "/run/secrets/";
        private IDictionary<string, string> _data;

        public DockerSecretParser(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> Parse()
        {
            _data.Clear();
            var secretsExists = _fileSystem.Directory.Exists(DOCKER_SECRET_PATH);
            if (!secretsExists) return _data;

            foreach (var filePath in _fileSystem.Directory.GetFiles(DOCKER_SECRET_PATH))
            {
                // Docker-secret nesting will be denoted by `_`. 
                // Convert that to dotnet config `:` nesting.
                var underscoreKey = _fileSystem.FileInfo.FromFileName(filePath).Name;
                var colonKey = underscoreKey.Replace('_', ':');

                var value = _fileSystem.File.ReadAllText(filePath);
                _data.Add(colonKey, value);
            }

            return _data;
        }
    }
}