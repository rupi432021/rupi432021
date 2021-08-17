using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class BarAntiTweetsByType
    {
        string tweetType;
        int cntAntiTweets;

        public BarAntiTweetsByType(string tweetType, int cntAntiTweets)
        {
            TweetType = tweetType;
            CntAntiTweets = cntAntiTweets;
        }

        public string TweetType { get => tweetType; set => tweetType = value; }
        public int CntAntiTweets { get => cntAntiTweets; set => cntAntiTweets = value; }

        public BarAntiTweetsByType() { }

        public List<BarAntiTweetsByType> getBarAntiTweetsByType()
        {
            DBservices dbs = new DBservices();
            List<BarAntiTweetsByType> BarAntiTweetsByTypeList = dbs.getBarAntiTweetsByType();
            return BarAntiTweetsByTypeList;
        }
    }
}