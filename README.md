# homework
code repository
This is the repository created to retrieve tweet data using Twitter APIs.
There are two solutions in this repository.
1.  TwitterSvc.sln   - This is the solution that provides the APIs.
       There are two APIs that are usable:
           https://localhost:7000/api/TweetDatas/   to get the hashtag distribution
           https://localhost:7000/api/TweetDatas/10   to get the 10 most recent tweets sampled
2.  TwitterConsole.sln - This is the console application that calls these two APIs and displays the data on the console window.

The authorization token is at line 38 in the file TwitterSvc/TwitterAccess.cs. That will have to changed to the value needed
and the project should be recompiled.

The TwitterSvc API server has unresolved locking issues when using the twitter API
            https://api.twitter.com/2/tweets/sample/stream

The data is streaming down and all the parsing is working as expected. However, the APIs do not get initialized correctly.

For this demonstration, the following is URL is used instead
            https://api.twitter.com/2/tweets/search/recent?query=%23caturday%20has%3Aimages%20-is%3Aretweet

With this URL, the APIs generate the hashtag information and the 10 most recent tweets.

Both the solutions can be downloaded from the repository https://github.com/dvsingh1/homework.git


