using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace TwitterSvc
{
    public interface IStartWorker
    {
        ManualResetEvent GetManualResetEvent();

        public void setManualResetEvent();

        Queue GetQueue();

        Task DoWork(CancellationToken cancellationToken);
    }
}
