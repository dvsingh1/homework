using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            timer1.Start();

            Thread.Sleep(5000);

            void HandleTimer2()
            {
                program.GetLatestTweets(url2);
            }
            System.Timers.Timer timer2 = new Timer(interval: 10000);
            timer2.Elapsed += (sender, e) => HandleTimer2();
            timer2.Start();

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
    }
}
