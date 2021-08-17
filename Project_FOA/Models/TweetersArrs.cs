using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tweetinvi.Models;

namespace Project_FOA.Models
{
    public class TweetersArrs
    {
        List<Tweeter> allNewTweeters;
        List<FriendsTweeter> allTweetersFriend;
        List<Tweeter> allTweetersToUpdate;
        ITweet[] tweetsArray;

        public TweetersArrs(List<Tweeter> allNewTweeters, List<FriendsTweeter> allTweetersFriend, List<Tweeter> allTweetersToUpdate, ITweet[] tweetsArray)
        {
            AllNewTweeters = allNewTweeters;
            AllTweetersFriend = allTweetersFriend;
            AllTweetersToUpdate = allTweetersToUpdate;
            TweetsArray = tweetsArray;
        }

        public List<Tweeter> AllNewTweeters { get => allNewTweeters; set => allNewTweeters = value; }
        public List<FriendsTweeter> AllTweetersFriend { get => allTweetersFriend; set => allTweetersFriend = value; }

        public List<Tweeter> AllTweetersToUpdate { get => allTweetersToUpdate; set => allTweetersToUpdate = value; }
        public ITweet[] TweetsArray { get => tweetsArray; set => tweetsArray = value; }

        public TweetersArrs() { }
    }
}