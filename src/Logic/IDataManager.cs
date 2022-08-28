namespace Logic;
public interface IDataManager : IDisposable
{
    ValueTask AddPhrase(Phrase phraise);
    Task<IEnumerable<Phrase>> GetPhrases(int count);
    ValueTask UpdatePhrase(Phrase phrase);
}