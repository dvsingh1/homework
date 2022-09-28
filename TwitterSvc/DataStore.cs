using System.Text.RegularExpressions;
using TwitterSvc.Models;

namespace TwitterSvc
{
    public class DataStore : IDataStore
    {
        private readonly ILogger<DataStore> logger;
        private IList<TweetData> listTweetData = new List<TweetData>();
        private IDictionary<string, int> dictHashtags = new Dictionary<string, int>();

        public DataStore(ILogger<DataStore> logger)
        {
            this.logger = logger;
        }

        public int GetTweetCount()
        {
            return listTweetData.Count;
        }

        public TweetData GetTweetEntry(int index)
        {
            return listTweetData[index];
        }

        public void ProcessData(TweetData data)
        {
            UpdateData(data);
            UpdateHashtags(data);
            return;
        }

        public IDictionary<string, int> GetPopularHashtagsData(int count)
        {
            IDictionary<string, int> dictHdata = new Dictionary<string, int>();
            int index = 0;
            foreach (KeyValuePair<string, int> hashtag in dictHashtags.OrderByDescending(x => x.Value))
            {
                if (index < count)
                {
                    dictHdata.Add(hashtag.Key, hashtag.Value);
                    index++;
                }
                else
                {
                    break;
                }
            }
            return dictHdata;
        }

        public void UpdateData(TweetData data)
        {
            try
            {
                if (data != null)
                {
                    listTweetData.Add(data);
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Exception updating the data: {ex.Message}");
            }
            return;
        }

        public void UpdateHashtags(TweetData data)
        {
            IList<MatchCollection> listMatches = new List<MatchCollection>();
            ParseTweetData(data,listMatches);
            //update the hashtag count
            UpdateHashtagCount(listMatches);

            return;
        }

        public void UpdateHashtagCount(IList<MatchCollection> listMatches)
        {
            foreach (MatchCollection coll in listMatches)
            {
                foreach (Match match in coll)
                {
                    int result=0;
                    if (dictHashtags.TryGetValue(match.Value, out result))
                    {
                        result++;
                    }
                    else
                    {
                        result = 1;
                    }
                    dictHashtags[match.Value] = result;
                    logger.LogInformation($"hashtag: {match.Value}, count: {result}");
                }
            }
            return;
        }

        public void ParseTweetData(TweetData data, IList<MatchCollection> listMatches)
        {
            try
            {
                if (data != null)
                {
                    //pattern to pick any word that has # in it
                    string pattern = @"(\w+[\#]\w+\b) | ([\#]\w+\b)";
                    var rx = new Regex(pattern, RegexOptions.Compiled |
                        RegexOptions.IgnoreCase);

                    if (data.text != null)
                    {
                        var matches = rx.Matches(data.text);
                        logger.LogInformation($"count of words containing #: {matches.Count}");

                        foreach (Match match in matches)
                        {
                            //This pattern is to pick only those words that start with #
                            string pattern1 = @"[\#]\w+";
                            var rx1 = new Regex(pattern1, RegexOptions.Compiled |
                                RegexOptions.IgnoreCase);

                            MatchCollection matches1 = rx1.Matches(match.Value);
                            listMatches.Add(matches1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Exception parsing the data: {ex.Message}");
            }
            return;
        }
    }
}
