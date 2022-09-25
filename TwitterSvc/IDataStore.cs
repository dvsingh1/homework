using TwitterSvc.Models;

namespace TwitterSvc
{
    public interface IDataStore
    {
        TweetData GetTweetEntry(int index);
        int GetTweetCount();
        void ProcessData(TweetData data);
        IDictionary<string, int> GetPopularHashtagsData(int count);
    }
}
