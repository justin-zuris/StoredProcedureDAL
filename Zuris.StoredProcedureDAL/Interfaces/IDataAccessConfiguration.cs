namespace Zuris.SPDAL
{
    public interface IDataAccessConfiguration
    {
        string ConnectionString { get; }

        string ProviderString { get; }
    }
}