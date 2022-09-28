using System.Text.RegularExpressions;
using TwitterSvc.Models;

namespace TwitterSvc
{
    public interface IDataStore
    {
        TweetData GetTweetEntry(int index);
        int GetTweetCount();
        void ProcessData(TweetData data);
        IDictionary<string, int> GetPopularHashtagsData(int count);
        void UpdateData(TweetData data);
        void UpdateHashtags(TweetData data);
        void UpdateHashtagCount(IList<MatchCollection> listMatches);
        void ParseTweetData(TweetData data, IList<MatchCollection> listMatches);
    }
}
