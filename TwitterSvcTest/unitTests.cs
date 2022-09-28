using Microsoft.Extensions.Logging;
using TwitterSvc;
using Microsoft.Build.Utilities;
using TwitterSvc.Models;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

namespace TwitterSvcTest
{
    [TestClass]
    public class unitTests
    {

        string input1 = "{\"data\":{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow ";
        string input2 = "https://t.co/w1cnoXgixl\"} }  {\"data\":{\"id\":\"2573844196280205315\",\"text\" ";
        string input3 = ":\"@nedeat2 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"} ";
        string input4 = "}{\"data\":{\"id\":\"3573844196280205315\",\"text\" ";
        string input5 = ":\"@nedeat3  ";
        string input6 = " @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}";
        string expResp1 = "{\"data\":{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow  @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}";
        string expResp2 = "{\"data\":{\"id\":\"1573844196280205315\",\"text\" :\"@nedeat1 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}}";
        string expSub1 =  "{\"data\":{\"id\":\"2573844196280205315\",\"text\" ";
        string expSub2 = "{\"data\":{\"id\":\"2573844196280205315\",\"text\" :\"@nedeat2 @FuzzyMarth @tomtomorrow https://t.co/w1cnoXgixl\"}";

        [TestMethod]
        public void TwitterAccessTestInputWithoutProperEnding()
        {
            string subPart = "", savePart = "";
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            string resp = access.ProcessIncomingData(input1, ref subPart);
            Assert.AreEqual(resp, input1);

        }

        [TestMethod]
        public void TwitterAccessTestInputWithProperEndingAndNothingExtraAtTheEnd()
        {
            string subPart = "", savePart = "";
            string input = input1 + input6;
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            string resp = access.ProcessIncomingData(input, ref subPart);
            Assert.AreEqual(resp, expResp1);
            //The subPart should be empty
            string expectedSub = "";
            Assert.AreEqual(subPart, expectedSub);

        }

        [TestMethod]
        public void TwitterAccessTestInputWithProperEndingAndExtraAtTheEnd()
        {
            string subPart = "", savePart = "";
            string input = input1 + input2;
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            string resp = access.ProcessIncomingData(input, ref subPart);
            Assert.AreEqual(resp, expResp2);
            Assert.AreEqual(subPart, expSub1);

        }

        [TestMethod]
        public void TwitterAccessTestInputWithSubPartAndWithoutProperEnding()
        {
            string subPart = expSub1, savePart = "";
            string input = input3;
            string expResp = expSub1 + input3;
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            string resp = access.ProcessIncomingData(input, ref subPart);
            Assert.AreEqual(resp, expSub2);
            Assert.AreEqual(subPart, expSub2);

        }

        [TestMethod]
        public void TwitterAccessTestInputWithSubPartAndMultipleEntriesWithProperEnding()
        {
            string subPart = expSub2, savePart = "";
            string input = input4 + input5 + input6;
            string expResp = subPart + input;
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            string resp = access.ProcessIncomingData(input, ref subPart);
            Assert.AreEqual(resp, expResp);
            Assert.AreEqual(subPart, "");

        }

        [TestMethod]
        public void TwitterAccessTestInputUpdatedToCorrectFormat()
        {
            string subPart = expSub2, savePart = "";
            string input = expResp2;
            string expResp = "{entries : [" + input + "]}";

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            access.UpdateIncomingData(ref input);
            Assert.AreEqual(input, expResp);

        }

        [TestMethod]
        public void TwitterAccessProcessDataCount0()
        {
            RootObject data = new RootObject();
            List<CmdData> list = new List<CmdData>();
            data.entries = list;
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TwitterAccess>();
            TwitterAccess access = new TwitterAccess(logger);
            access.ProcessReceivedData(data);
            Assert.AreEqual(list.Count, 0);

        }

        [TestMethod]
        public void DataStoreParseTestWithNull()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = null;
            store.ParseTweetData(data, listMatches);
            int count = listMatches.Count;
            Assert.AreEqual(listMatches.Count, 0);
        }

        [TestMethod]
        public void DataStoreParseTestWithNoHashInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "abcd";
            store.ParseTweetData(data, listMatches);
            Assert.AreEqual(listMatches.Count, 0);
        }

        [TestMethod]
        public void DataStoreParseTestWithHashButNoHashtagInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "ab#cd";
            store.ParseTweetData(data, listMatches);
            Assert.AreEqual(listMatches.Count, 0);
            store.UpdateHashtagCount(listMatches);
            IDictionary<string, int> resp = store.GetPopularHashtagsData(3);
            int count = resp.Count;
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void DataStoreParseTestWithHashtagAtEndInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "ab #cd";
            store.ParseTweetData(data, listMatches);
            Assert.AreEqual(listMatches.Count, 1);
        }

        [TestMethod]
        public void DataStoreParseTestWithHashtagNotAtEndInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "ab #cd ef";
            store.ParseTweetData(data, listMatches);
            Assert.AreEqual(listMatches.Count, 1);
        }

        [TestMethod]
        public void DataStoreParseTestWithHashtagAndHashCharacterInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "a#b #cd ef";
            store.ParseTweetData(data, listMatches);
            Assert.AreEqual(listMatches.Count, 3);
        }

        [TestMethod]
        public void DataStoreParseTestWithTwoUniqueHashtagInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "abcd #b #cd ef";
            store.ParseTweetData(data, listMatches);
            int count = listMatches.Count;
            Assert.AreEqual(listMatches.Count, 2);
            IDictionary<string, int> resp = store.GetPopularHashtagsData(3);
            count = resp.Count;
            Assert.AreEqual(count, 0);
            store.UpdateHashtagCount(listMatches);
            resp = store.GetPopularHashtagsData(3);
            count = resp.Count;
            Assert.AreEqual(count, 2);

            int index = 0;
            foreach (KeyValuePair<string, int> hashtag in resp)
            {
                if (index > 0)
                {
                    Assert.AreEqual(hashtag.Key,"#cd");
                    Assert.AreEqual(hashtag.Value, 1);
                }
                else
                {
                    Assert.AreEqual(hashtag.Key, "#b");
                    Assert.AreEqual(hashtag.Value, 1);
                }
                index++;
            }

        }

        [TestMethod]
        public void DataStoreParseTest1WithNonUniqueHashtagInputData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "abcd #b #cd #b #b ef";
            store.ParseTweetData(data, listMatches);
            int count = listMatches.Count;
            Assert.AreEqual(listMatches.Count, 2);
            IDictionary<string, int> resp = store.GetPopularHashtagsData(3);
            count = resp.Count;
            Assert.AreEqual(count, 0);
            store.UpdateHashtagCount(listMatches);
            resp = store.GetPopularHashtagsData(3);
            count = resp.Count;
            Assert.AreEqual(count, 2);

            int index = 0;
            foreach (KeyValuePair<string, int> hashtag in resp)
            {
                if (index > 0)
                {
                    Assert.AreEqual(hashtag.Key, "#cd");
                    Assert.AreEqual(hashtag.Value, 1);
                }
                else
                {
                    Assert.AreEqual(hashtag.Key, "#b");
                    Assert.AreEqual(hashtag.Value, 3);
                }
                index++;
            }

        }

        [TestMethod]
        public void DataStoreUpdateDataWithNull()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            TweetData data = null;
            store.UpdateData(data);
            int count = store.GetTweetCount();
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void DataStoreUpdateDataWithSingleInputNoData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            store.UpdateData(data);
            int count = store.GetTweetCount();
            Assert.AreEqual(count, 1);
        }

        [TestMethod]
        public void DataStoreUpdateDataWithSingleInputWithNoHashtagData()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "abcd";
            //data.text = "abcd #b #cd ef";
            store.UpdateData(data);
            int count = store.GetTweetCount();
            Assert.AreEqual(count, 1);
            TweetData resp = store.GetTweetEntry(0);
            Assert.AreEqual(resp.id, data.id);
            Assert.AreEqual(resp.text, data.text);
        }

        [TestMethod]
        public void DataStoreUpdateDataWithSingleInputWithNoHashtagData1()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DataStore>();
            DataStore store = new DataStore(logger);
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            TweetData data = new TweetData();
            data.id = 1;
            data.text = "abcd";
            //data.text = "abcd #b #cd ef";
            store.UpdateData(data);
            int count = store.GetTweetCount();
            Assert.AreEqual(count, 1);
            TweetData resp = store.GetTweetEntry(0);
            Assert.AreEqual(resp.id, data.id);
            Assert.AreEqual(resp.text, data.text);
        }
    }
}