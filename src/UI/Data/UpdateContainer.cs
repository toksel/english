namespace English;

public class UpdateContainer
{
    private object _lock = new();
    private List<Phrase> _phrases = new();

    public void Add(Phrase phrase) 
    {
        lock(_lock)
        {
            _phrases.Add(phrase);
        }
    } 

    public List<Phrase> GetAll()
    {
        List<Phrase> phrases;
        lock(_lock)
        {
            phrases = _phrases.ToList();
            _phrases.Clear();
        }
        return phrases;
    }
}