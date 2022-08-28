namespace English;

public class UpdateService : BackgroundService
{
    private readonly ILogger<UpdateService> _logger;
    private readonly IDataAccess _dataAccess;
    private readonly UpdateContainer _container;
    private readonly int _waitInterval = 3000;

    public UpdateService(ILogger<UpdateService> logger, IDataAccess dataAccess,
        UpdateContainer container)
    {
        _logger = logger;
        _dataAccess = dataAccess;
        _container = container;
    }

    public override void Dispose()
    {
        _dataAccess.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var updatePhrases = _container.GetAll();
            if (updatePhrases?.Any() ?? false)
            {
                await _dataAccess.UpdateEntities(updatePhrases);
            }
            await Task.Delay(_waitInterval);
        }
    }
}