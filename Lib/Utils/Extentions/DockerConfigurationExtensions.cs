using System;
using Config.Docker;
using Microsoft.Extensions.Configuration;

namespace Utils.Extentions
{
    public static class DockerConfigurationExtensions
    {
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder)
        {
            return AddDockerSecrets(builder, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, bool optional)
        {
            return AddDockerSecrets(builder, optional: optional, reloadOnChange: false);
        }


        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, bool optional, bool reloadOnChange)
        {
            var source = new DockerConfigSource
            {
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };
            builder.Add(source);
            return builder;
        }
    }
}
