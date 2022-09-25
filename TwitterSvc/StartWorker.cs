using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using TwitterSvc.Models;
using System.Configuration;

namespace TwitterSvc
{
    public class StartWorker : IStartWorker
    {
        private readonly ILogger<StartWorker> logger;
        private readonly ITwitterAccess twitterAccess;
        private readonly IDataStore dataStore;
        private int number = 0;
        private ManualResetEvent oSignalEvent = new ManualResetEvent(false);
        private Queue dataQueue = new Queue();
        public StartWorker(ILogger<StartWorker> logger, ITwitterAccess access, IDataStore dataStore)
        {
            this.logger = logger;
            this.twitterAccess = access;
            this.dataStore = dataStore;
        }

        public ManualResetEvent GetManualResetEvent()
        {
            return oSignalEvent;
        }

        public Queue GetQueue()
        {
            return dataQueue;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            twitterAccess.Init(oSignalEvent, dataQueue);
            AsyncData asyncData = new AsyncData();
            asyncData.dataQueue = dataQueue;
            asyncData.oSignalEvent = oSignalEvent;
            Thread thread = new Thread(ProcessFunc);
            thread.Start(asyncData);

            oSignalEvent.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                oSignalEvent.WaitOne(TimeSpan.FromSeconds(1));
                twitterAccess.GetData();
                //Interlocked.Increment(ref number);
                //logger.LogInformation($"Worker printing number: {number}");
                //oSignalEvent.Reset();
                await Task.Delay(1000 * 1);
            }
        }

        private void ProcessFunc(object asyncData)
        {
            oSignalEvent.Set();
            while (true)
            {
                IDictionary<string, int> dict;
                oSignalEvent.WaitOne(TimeSpan.FromSeconds(5));
                int count = dataQueue.Count;
                while (dataQueue.Count > 0)
                {
                   TweetData data = (TweetData)dataQueue.Dequeue();
                    dataStore.ProcessData(data);
                }
                logger.LogInformation($"new data count in worker: {count}");
                dict = dataStore.GetPopularHashtagsData(10);
                oSignalEvent.Reset();
                //oSignalEvent.WaitOne(TimeSpan.FromSeconds(5));
                //Thread.Sleep(5000);
            }

        }
    }
}
