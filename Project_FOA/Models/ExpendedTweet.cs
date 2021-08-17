using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class ExpendedTweet : Tweet
    {
        Tweeter author;
        List<string> hashtags;

        public ExpendedTweet(string idTweet, string tweetCreatedAt, string tweetText, int quoteCount, int replyCount, int retweetCount, string urlTweet, string tweetType, string attributedTweetId, int isAntisemitic, string idTweeter, double antisemitismPercentage, double finalScore, List<string> attributedTweetersNames, int searchId, int userId, Tweeter author, List<string> hashtags) : base( idTweet, tweetCreatedAt, tweetText, quoteCount, replyCount, retweetCount,  urlTweet, tweetType, attributedTweetId, isAntisemitic,idTweeter, antisemitismPercentage, finalScore, attributedTweetersNames, searchId, userId)
        {
            this.Author = author;
            this.Hashtags = hashtags;
        }

        public Tweeter Author { get => author; set => author = value; }
        public List<string> Hashtags { get => hashtags; set => hashtags = value; }


        public ExpendedTweet() { }
    }
}