using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class Tweet 
    {
        string idTweet;
        string tweetCreatedAt;
        string tweetText;
        int quoteCount;
        int replyCount;
        int retweetCount;
        string urlTweet;
        string tweetType;
        string attributedTweetId;
        int isAntisemitic;
        string idTweeter;
        double antisemitismPercentage;
        double finalScore;
        List<string> attributedTweetersNames; // we have table attributesOfTweets (many to many)
        int searchId; // we have table tweetsBySearch (many to many)
        int userId;

        public Tweet(string idTweet, string tweetCreatedAt, string tweetText, int quoteCount, int replyCount, int retweetCount, string urlTweet, string tweetType, string attributedTweetId, int isAntisemitic, string idTweeter, double antisemitismPercentage, double finalScore, List<string> attributedTweetersNames, int searchId, int userId)
        {
            IdTweet = idTweet;
            TweetCreatedAt = tweetCreatedAt;
            TweetText = tweetText;
            QuoteCount = quoteCount;
            ReplyCount = replyCount;
            RetweetCount = retweetCount;
            UrlTweet = urlTweet;
            TweetType = tweetType;
            AttributedTweetId = attributedTweetId;
            IsAntisemitic = isAntisemitic;
            IdTweeter = idTweeter;
            AntisemitismPercentage = antisemitismPercentage;
            FinalScore = finalScore;
            AttributedTweetersNames = attributedTweetersNames;
            SearchId = searchId;
            UserId = userId;
        } 
        public string IdTweet { get => idTweet; set => idTweet = value; }
        public string TweetCreatedAt { get => tweetCreatedAt; set => tweetCreatedAt = value; }
        public int QuoteCount { get => quoteCount; set => quoteCount = value; }
        public int ReplyCount { get => replyCount; set => replyCount = value; }
        public int RetweetCount { get => retweetCount; set => retweetCount = value; }
        public string UrlTweet { get => urlTweet; set => urlTweet = value; }
        public string TweetType { get => tweetType; set => tweetType = value; }
        public string AttributedTweetId { get => attributedTweetId; set => attributedTweetId = value; }
        public int IsAntisemitic { get => isAntisemitic; set => isAntisemitic = value; }
        public List<string> AttributedTweetersNames { get => attributedTweetersNames; set => attributedTweetersNames = value; }
        public string TweetText { get => tweetText; set => tweetText = value; }
        public int SearchId { get => searchId; set => searchId = value; }
        public int UserId { get => userId; set => userId = value; }
        public string IdTweeter { get => idTweeter; set => idTweeter = value; }
        public double AntisemitismPercentage { get => antisemitismPercentage; set => antisemitismPercentage = value; }
        public double FinalScore { get => finalScore; set => finalScore = value; }

        public Tweet() { }

        public List<Tweet> InsertTweets(List<Tweet> allTweets) //insert tweets
        {
            DBservices dbs = new DBservices();
            return dbs.InsertTweets(allTweets);
        }

        public void InsertTweetsBySearch(List<Tweet> allTweets) //insert tweets by search
        {
            DBservices dbs = new DBservices();
            dbs.InsertTweetsBySearch(allTweets);
        }

        public void InsertTweetsByCustomizedSearch(List<Tweet> allTweets) //insert tweets by search
        {
            DBservices dbs = new DBservices();
            dbs.InsertTweetsByCustomizedSearch(allTweets);
        }

        public List<Tweet> getTweets(int idUser) //get all tweets for the volunteer page
        {
            DBservices dbs = new DBservices();
            List<Tweet> TweetsList = dbs.getTweets(idUser);
            return TweetsList;
        }

        public List<Tweet> getAntiTweets(string idTweeter) //get antisemitic tweets by id tweeter - for react
        {
            DBservices dbs = new DBservices();
            List<Tweet> antiTweetsList = dbs.getAntiTweets(idTweeter);
            return antiTweetsList;
        }

        public List<Tweet> getTweetsBySearch(int idSearch) //get all antisemitic tweets by id search - for react
        {
            DBservices dbs = new DBservices();
            List<Tweet> TweetsList = dbs.getTweetsBySearch(idSearch);
            return TweetsList;
        }
    }
}