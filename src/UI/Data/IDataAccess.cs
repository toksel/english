namespace English;

public interface IDataAccess : IDisposable
{
    Task<IEnumerable<Phrase>> GetEntities(int count);
    Task UpdateEntities(IEnumerable<Phrase> phrases);
    Task InsertEntity(Phrase phrase);
}