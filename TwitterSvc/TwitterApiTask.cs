using TwitterSvc;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterSvc
{
    public class TwitterApiTask : BackgroundService
    {
        private readonly IStartWorker startWorker;

        public TwitterApiTask(IStartWorker startWorker)
        {
            this.startWorker = startWorker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await startWorker.DoWork(stoppingToken);
        }
    }
}
