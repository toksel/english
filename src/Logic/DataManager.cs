using System.Threading.Channels;
using Data;

namespace Logic;
public class DataManager : IDataManager
{
    private readonly IDataAccess _dataAccess;
    private readonly Channel<Phrase> _updateQueue;
    private CancellationTokenSource _cts;
    private readonly int _waitInterval = 5000;

    public DataManager(string connectionString)
    {
        _dataAccess = new DataAccess(connectionString);
        _updateQueue = Channel.CreateUnbounded<Phrase>();
        _cts = new CancellationTokenSource();
        RunUpdater();
    }

    private void RunUpdater()
    {
        Task.Factory.StartNew(async () =>
        {
            var phrases = new List<PhraseDto>();
            while(!_cts.Token.IsCancellationRequested)
            {
                while (_updateQueue.Reader.Count > 0)
                {
                    var phrase = await _updateQueue.Reader.ReadAsync();
                    phrases.Add(MapToPhraseDto(phrase));
                }
                await _dataAccess.UpdatePhrases(phrases);
                phrases.Clear();
                await Task.Delay(_waitInterval);
            }
        }, _cts.Token);
    }

    private PhraseDto MapToPhraseDto(Phrase phrase) =>
        new PhraseDto
        {
            Id = phrase.Id,
            Rank = phrase.Rank,
            Text = phrase.Text,
            Reference = phrase.Reference
        };

    private Phrase FromPhraseDto(PhraseDto phrase) =>
        new Phrase
        {
            Id = phrase.Id,
            Rank = phrase.Rank,
            Text = phrase.Text,
            Reference = phrase.Reference
        };

    public async ValueTask AddPhrase(Phrase phraise)
    {
        var dto = MapToPhraseDto(phraise);
        await _dataAccess.InsertPhrase(dto);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts.Cancel();
            _dataAccess.Dispose();
        }
    }

    public async Task<IEnumerable<Phrase>> GetPhrases(int count)
    {
        var dtos = await _dataAccess.GetPhrases(count);
        return dtos.Select(d => FromPhraseDto(d));
    }

    public async ValueTask UpdatePhrase(Phrase phrase) =>
        await _updateQueue.Writer.WriteAsync(phrase);

    ~DataManager() => Dispose(false);
}