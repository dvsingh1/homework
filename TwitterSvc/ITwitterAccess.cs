using System.Collections;

namespace TwitterSvc
{
    public interface ITwitterAccess
    {
        void Init(ManualResetEvent oSignalEvent, Queue dataQueue);
        string GetData();
    }
}
