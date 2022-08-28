namespace Data;

public interface IDataAccess : IDisposable
{
    Task<IEnumerable<PhraseDto>> GetPhrases(int count);
    Task UpdatePhrases(IEnumerable<PhraseDto> phrases);
    Task InsertPhrase(PhraseDto phrase);
}