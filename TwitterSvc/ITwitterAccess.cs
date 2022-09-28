using System.Collections;
using TwitterSvc.Models;

namespace TwitterSvc
{
    public interface ITwitterAccess
    {
        void Init(ManualResetEvent oSignalEvent, Queue dataQueue);
        string GetData();
        string ProcessIncomingData(string inStr, ref string subPart);
        void UpdateIncomingData(ref string whole);
        void ProcessReceivedData(RootObject data);
        Queue GetDataQueue();

    }
}
