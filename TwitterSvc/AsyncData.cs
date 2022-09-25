using System.Collections;

namespace TwitterSvc
{
    public class AsyncData
    {
        public ManualResetEvent oSignalEvent;
        public Queue dataQueue;
    }
}
