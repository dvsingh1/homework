using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using TwitterSvc.Models;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Primitives;

namespace TwitterSvc
{
    public class TwitterAccess : ITwitterAccess
    {
        private readonly ILogger<TwitterAccess> logger;
        private HttpClient client = new HttpClient();
        private ManualResetEvent oSignalEvent;
        private Queue dataQueue;
        string apiurl;
        public TwitterAccess(ILogger<TwitterAccess> logger)
        {
            this.logger = logger;
        }

        public void Init(ManualResetEvent oSignalEvent, Queue dataQueue)
        {
            this.oSignalEvent = oSignalEvent;
            this.dataQueue = dataQueue;
            apiurl = Environment.GetEnvironmentVariable("apiurl");
            if (apiurl == null)
            {
                apiurl = "https://api.twitter.com/2/tweets/sample/stream";
            }
            client.BaseAddress = new Uri(apiurl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            string token = Environment.GetEnvironmentVariable("token");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            client.Timeout = Timeout.InfiniteTimeSpan;


        }

        public string GetData()
        {
            var task = GetDataAsync();
            var result = task.Result;
            oSignalEvent.Set();
            return result;
        }

        public async Task<string> GetDataAsync()
        {
            string resp="";

            try
            {
                using (HttpResponseMessage response = await client.GetAsync(apiurl,
                    HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        //use this section when using the stream URL above
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var buffer = new byte[4096];
                            int readCount;
                            string subPart = "";
                            string whole = "";
                            while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                //The buffer read may contain partial JSON data. So this should be properly
                                //handled
                                string str = Encoding.ASCII.GetString(buffer, 0, readCount);
                                try
                                {
                                    whole = ProcessIncomingData(str, ref subPart);
                                    UpdateIncomingData(ref whole);
                                    RootObject respData = JsonConvert.DeserializeObject<RootObject>(whole);
                                    ProcessReceivedData(respData);
                                    logger.LogInformation($"new tweet data added to the queue: {respData}");
                                }
                                catch (Exception ex)
                                {
                                    logger.LogInformation($"ERROR: Unable to process the received buffer: {ex.Message}");
                                }
                            }

                        }
                        //
                    }
                    else
                    {
                        logger.LogInformation($"Bad response code");

                    }
                }
            } 
            catch(Exception ex)
            {
                logger.LogInformation($"Unable to communicate with the Twitter API server: {ex.Message}");
            }

            return resp;
        }

        public string ProcessIncomingData(string inStr,ref string subPart)
        {
            string newSubPart = "";
            string pattern1 = @"(}\s+}) | (}}\s+)";
            string newPattern1 = "}}";
            string pattern2 = @"}\s+";
            string newPattern2 = "}";
            string whole = "";

            whole = subPart + inStr;
            whole = Regex.Replace(whole, pattern1, newPattern1);
            whole = Regex.Replace(whole, pattern2, newPattern2);
            int endIdx = whole.LastIndexOf(newPattern1);
            if (endIdx > -1)
            {
                string savePart = whole.Substring(0, endIdx + 2);
                int len = whole.Length - (endIdx + 2);
                if (len > 0)
                {
                    newSubPart = whole.Substring(endIdx + 2, len);
                }
                else
                {
                    newSubPart = "";
                }
                subPart = newSubPart;
                whole = savePart;
            }
            else
            {
                subPart = whole;
            }

            return whole;
        }

        public void UpdateIncomingData(ref string whole)
        {
            string pattern = "}}{";
            string newPattern = "}},{";

            //modify the string to make it look like a serialized JSON string for RootObject
            whole = Regex.Replace(whole, pattern, newPattern);

            whole = "{entries : [" + whole + "]}";
            return;
        }

        public void ProcessReceivedData(RootObject data)
        {
            List<CmdData> lstData = data.entries;

            if ((lstData != null) && (lstData.Count > 0))
            {
                oSignalEvent.Set();
                for (int i = 0; i < lstData.Count; i++)
                {
                    TweetData tweet = lstData[i].data;
                    dataQueue.Enqueue(tweet);
                }
                logger.LogInformation($"new data count in TwitterAccess: {lstData.Count}");
            }

        }

        public Queue GetDataQueue()
        {
            return dataQueue;
        }
    }
}

