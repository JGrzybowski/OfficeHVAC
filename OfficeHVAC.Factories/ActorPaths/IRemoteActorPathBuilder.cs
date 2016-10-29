namespace OfficeHVAC.Factories.ActorPaths
{
    public interface IRemoteActorPathBuilder : IActorPathBuilder
    {
        string ServerAddress { get; set; }

        int? ServerPort { get; set; }

        string CompanyActorName { get; set; }
    }
}