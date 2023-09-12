using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Hosting
{
    public abstract class Job : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        protected readonly ILogger<Job> Logger;

        protected Job(ILogger<Job> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            Logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Logger.LogInformation("Starting Job: {type}", GetType().Name);

                await RunAsync(stoppingToken);

                Logger.LogInformation("Completed Job: {type}", GetType().Name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error running job - {ex}", ex.ToString());
                Environment.ExitCode = 1;
                throw;
            }

            _hostApplicationLifetime.StopApplication();
        }

        protected abstract Task RunAsync(CancellationToken cancellationToken);
    }
}