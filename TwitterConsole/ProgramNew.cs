using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TwitterConsole.Models;
using Timer = System.Timers.Timer;

namespace TwitterConsole
{
    class Program
    {
        public static HttpClient client1 = new HttpClient();
        public static HttpClient client2 = new HttpClient();

        static void Main(string[] args)
        {
            string url1 = "https://localhost:7000/api/TweetDatas/";
            string url2 = "https://localhost:7000/api/TweetDatas/10";
        //client1 is used to call the API that gets the hashtag information
            client1.BaseAddress = new Uri(url1);
            client1.DefaultRequestHeaders.Accept.Clear();
            client1.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //client2 is used to call the API that gets the latest 10 tweets
            client2.BaseAddress = new Uri(url2);
            client2.DefaultRequestHeaders.Accept.Clear();
            client2.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            Program program = new Program();

            void HandleTimer1()
            {
                program.GetHashtag(url1);
            }
            System.Timers.Timer timer1 = new Timer(interval: 10000);
            timer1.Elapsed += (sender, e) => HandleTimer1();
            //timer1.Start();

            //Thread.Sleep(5000);

            void HandleTimer2()
            {
                program.GetLatestTweets(url2);
            }
            System.Timers.Timer timer2 = new Timer(interval: 10000);
            timer2.Elapsed += (sender, e) => HandleTimer2();
            //timer2.Start();


            /*var objectToSerialize = new RootObject();
            objectToSerialize.items = new List<Item> 
                              {
                                 new Item { name = "test1", index = "index1" },
                                 new Item { name = "test2", index = "index2" }
                              };

            string test = JsonConvert.SerializeObject(objectToSerialize); */
            //string input = $"{{\"data\":{{\"id\":\"1573844196280205315\",\"text\":\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}}} , {{\"data\":{{\"id\":\"2573844196280205315\",\"text\":\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}}}";
            //string input = $"{{\"items\": [{{\"data\":{{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}}} , {{\"data\":{{\"id\":\"2573844196280205315\",\"text\":\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}}}]}}";
            /*string input = $"{{\"items\":[{{\"name\":\"test1\",\"index\":\"index1\",\"optional\":null}},{{ \"name\":\"test2\",\"index\":\"index2\",\"optional\":null}}]}}";

            string input1 = "{{\"data\":{{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow ";
            string input2 = "https://t.co/w1cnoXgixl\"}} }}  {{\"data\":{{\"id\":\"2573844196280205315\",\"text\" ";
            string input3 = ":\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}";
            string input4 = "}}{{\"data\":{{\"id\":\"3573844196280205315\",\"text\" ";
            string input5 = ":\"@nedeat1  ";
            string input6 = " @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}}}";
            */
            string input1 = "{\"data\":{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow ";
            string input2 = "https://t.co/w1cnoXgixl\"} }  {\"data\":{\"id\":\"2573844196280205315\",\"text\" ";
            string input3 = ":\"@nedeat2 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"} ";
            string input4 = "}{\"data\":{\"id\":\"3573844196280205315\",\"text\" ";
            string input5 = ":\"@nedeat3  ";
            string input6 = " @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}";
            string subPart ="", savePart="", whole="";
            string newSubPart = "";
            subPart = input1;
            IList<string> strings = new List<string>();
            strings.Add(input2);
            strings.Add(input3);
            strings.Add(input4);
            strings.Add(input5);
            strings.Add(input6);
            string pattern1 = @"(}\s+}) | (}}\s+)";
            string newPattern1 = "}}";
            string pattern2 = @"}\s+";
            string newPattern2 = "}";
            foreach (string s in strings)
            {
                whole = subPart + s;
                whole = Regex.Replace(whole, pattern1, newPattern1);
                whole = Regex.Replace(whole, pattern2, newPattern2);
                int endIdx = whole.LastIndexOf(newPattern1);
                if ( endIdx > -1)
                {
                    savePart = whole.Substring(0, endIdx+2);
                    int len = whole.Length - (endIdx+2);
                    if (len > 0)
                    {
                        newSubPart = whole.Substring(endIdx+2, len);
                    }
                    else
                    {
                        //whole = subPart + savePart;
                        newSubPart = "";
                    }
                    subPart = newSubPart;
                    whole = savePart;
                }
                else
                {
                    subPart = whole;
                }
            }

            RootObject respData = JsonConvert.DeserializeObject<RootObject>(whole);
            IList<CmdData> items = respData.items;
            //IList<Item> items = respData.items;
            //List<CmdData> respData = JsonConvert.DeserializeObject<List<CmdData>>(input);
            //var list = JsonSerializer.Deserialize<List<CmdData>>(input);


            Console.WriteLine("Enter any key to exit the application");
            Console.ReadLine();
        }

        private void GetHashtag(string url)
        {
            HttpResponseMessage webResponse = client1.GetAsync(url).Result;
            string str = webResponse.Content.ReadAsAsync<string>().Result;
            HashtagData respData = JsonConvert.DeserializeObject<HashtagData>(str);
            IDictionary<string, int> dict = respData.hashtagCount;
            Console.WriteLine("==================================");
            Console.WriteLine("Hashtag usage data:");
            Console.WriteLine("==================================");
            Console.WriteLine($"tweet count: {respData.tweetCount} ");
            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.WriteLine("Hashtag                 count");
            Console.WriteLine("==================================");
            foreach (var hash in dict)
            {
                Console.WriteLine($"{hash.Key}    {hash.Value}");
            }
            Console.WriteLine("==================================");
            Console.WriteLine();
        }

        private void GetLatestTweets(string url)
        {
            HttpResponseMessage webResponse = client1.GetAsync(url).Result;
            string str = webResponse.Content.ReadAsAsync<string>().Result;
            List<TweetData> respData = JsonConvert.DeserializeObject<List<TweetData>>(str);
            Console.WriteLine("==================================");
            Console.WriteLine("Tweet text data:");
            Console.WriteLine("==================================");
            foreach (TweetData data in respData)
            {
                Console.WriteLine($"id: {data.id} text: {data.text}");
            }
            Console.WriteLine("==================================");
            Console.WriteLine();
        }

        private int ProcessIncomingData(string inData)
        {
            int endIdx = inData.LastIndexOf("}}");
            if ((endIdx > -1) && (endIdx < inData.Length - 3))
            {
                endIdx++;
                endIdx++;
            }
            return endIdx;
        }

    }
}
