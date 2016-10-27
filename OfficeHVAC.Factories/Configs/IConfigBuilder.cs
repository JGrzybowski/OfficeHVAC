using Akka.Configuration;

namespace OfficeHVAC.Factories.Configs
{
    public interface IConfigBuilder
    {
        int? Port { get; set; }
        Config Config();
    }
}