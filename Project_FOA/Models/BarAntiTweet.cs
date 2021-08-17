using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models.DAL
{
    public class BarAntiTweet
    {
        string numTweet;
        string tweetText;
        double antisemitismPercentage;
      
        public BarAntiTweet(string numTweet, string tweetText, double antisemitismPercentage)
        {
            NumTweet = numTweet;
            TweetText = tweetText;
            AntisemitismPercentage = antisemitismPercentage;
        }

        public string NumTweet { get => numTweet; set => numTweet = value; }
        public string TweetText { get => tweetText; set => tweetText = value; }
        public double AntisemitismPercentage { get => antisemitismPercentage; set => antisemitismPercentage = value; }


        public BarAntiTweet() { }

        public List<BarAntiTweet> getBarAntiTweets() 
        {
            DBservices dbs = new DBservices();
            List<BarAntiTweet> BarAntiTweetsList = dbs.getBarAntiTweets();
            return BarAntiTweetsList;
        }

    }
}