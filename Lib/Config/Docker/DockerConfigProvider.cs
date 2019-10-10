using System;
using Microsoft.Extensions.Configuration;

namespace Config.Docker
{
    public class  DockerConfigProvider: ConfigurationProvider, IConfigurationProvider
    {
        public DockerConfigSource Source {get;}

        public DockerConfigProvider(DockerConfigSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            Source = source;
        }


        public override void Load()
        {
            var parser = new DockerSecretParser();
            Data = parser.Parse();
        }
    }
}
