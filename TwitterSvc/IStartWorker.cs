using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace TwitterSvc
{
    public interface IStartWorker
    {
        ManualResetEvent GetManualResetEvent();

        Queue GetQueue();

        Task DoWork(CancellationToken cancellationToken);
    }
}
