using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class ExpendedTweetsTweetersArr
    {
        List<ExpendedTweet> expendedTweets;
        TweetersArrs tweetersArrays;

        public ExpendedTweetsTweetersArr(List<ExpendedTweet> expendedTweets, TweetersArrs tweetersArrays)
        {
            ExpendedTweets = expendedTweets;
            TweetersArrays = tweetersArrays;
        }

        public List<ExpendedTweet> ExpendedTweets { get => expendedTweets; set => expendedTweets = value; }
        public TweetersArrs TweetersArrays { get => tweetersArrays; set => tweetersArrays = value; }

        public ExpendedTweetsTweetersArr() { }
    }
}