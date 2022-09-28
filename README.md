# homework
code repository
This is the repository created to retrieve tweet data using Twitter APIs.
There are three solutions in this repository.
1.  TwitterSvc.sln   - This is the solution that provides the APIs.
       There are two APIs that are usable:
           https://localhost:7000/api/TweetDatas/   to get the hashtag distribution
           https://localhost:7000/api/TweetDatas/10   to get the 10 most recent tweets sampled
2.  TwitterConsole.sln - This is the console application that calls these two APIs and displays the data on the console window.
3. TwitterSvcTest.sln - This is the solution with unit tests for TwitterSvc classes

The authorization token is at line 38 in the file TwitterSvc/TwitterAccess.cs. That will have to changed to the value needed
and the project should be recompiled.

In the TwitterSvc solution, the launchSettings.json has the following environment variables:
        "apiurl": "https://api.twitter.com/2/tweets/sample/stream",
        "token": "AAAAAAAAAAAAAAAAAAAAAGg1hQEAAAAA2zLf9JmtVqdDpOG1iiH0f1J3fPE%3DHv1f53hJyZsexBHopnPIPxc297uscuf4Q9xLov6n3CxXAG4QoI"

The value for the apiurl variable is the URL for the Twitter API.
The variable "token" is used to specify the authorization token value. This should be changed to the value that is correct for the user.

The solutions can be downloaded from the repository https://github.com/dvsingh1/homework.git. All the solutions are compiled using visual studio 2022



