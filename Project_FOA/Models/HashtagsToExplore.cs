using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tweetinvi.Models.Entities;

namespace Project_FOA.Models
{
    public class HashtagsToExplore
    {
        List<string> hashtags;
        string idTweet;
        string idTweeter;

        public HashtagsToExplore(List<string> hashtags, string idTweet, string idTweeter)
        {
            Hashtags = hashtags;
            IdTweet = idTweet;
            IdTweeter = idTweeter;
        }

        public List<string> Hashtags { get => hashtags; set => hashtags = value; }
        public string IdTweet { get => idTweet; set => idTweet = value; }
        public string IdTweeter { get => idTweeter; set => idTweeter = value; }

        public HashtagsToExplore() { }
    }
}