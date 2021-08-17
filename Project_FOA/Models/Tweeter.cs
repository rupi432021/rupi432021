using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class Tweeter
    {
        
        string idTweeter;
        string tweeterName;
        int followersCount;
        int friendsCount;
        string tweeterCreatedAtAccount;
        int statusesCountSinceCreated;
        int statusesCountSinceChecked;
        int antisemiticTweets;
        int antisemiticFriends;
        string tweeterLocation;
        int isAntisemitic;
        double antisemitismPercentage;
        List<FriendsTweeter> idsFriendsTweeter;

        public Tweeter(string idTweeter, string tweeterName, int followersCount, int friendsCount, string tweeterCreatedAtAccount, int statusesCountSinceCreated, int statusesCountSinceChecked, int antisemiticTweets, int antisemiticFriends, string tweeterLocation, int isAntisemitic, double antisemitismPercentage, List<FriendsTweeter> idsFriendsTweeter)
        {
            IdTweeter = idTweeter;
            TweeterName = tweeterName;
            FollowersCount = followersCount;
            FriendsCount = friendsCount;
            TweeterCreatedAtAccount = tweeterCreatedAtAccount;
            StatusesCountSinceCreated = statusesCountSinceCreated;
            StatusesCountSinceChecked = statusesCountSinceChecked;
            AntisemiticTweets = antisemiticTweets;
            AntisemiticFriends = antisemiticFriends;
            TweeterLocation = tweeterLocation;
            IsAntisemitic = isAntisemitic;
            AntisemitismPercentage = antisemitismPercentage;
            IdsFriendsTweeter = idsFriendsTweeter;
        }

        public string IdTweeter { get => idTweeter; set => idTweeter = value; }
        public string TweeterName { get => tweeterName; set => tweeterName = value; }
        public int FollowersCount { get => followersCount; set => followersCount = value; }
        public int FriendsCount { get => friendsCount; set => friendsCount = value; }
        public string TweeterCreatedAtAccount { get => tweeterCreatedAtAccount; set => tweeterCreatedAtAccount = value; }
        public int StatusesCountSinceCreated { get => statusesCountSinceCreated; set => statusesCountSinceCreated = value; }
        public int StatusesCountSinceChecked { get => statusesCountSinceChecked; set => statusesCountSinceChecked = value; }
        public int AntisemiticTweets { get => antisemiticTweets; set => antisemiticTweets = value; }
        public int AntisemiticFriends { get => antisemiticFriends; set => antisemiticFriends = value; }
        public string TweeterLocation { get => tweeterLocation; set => tweeterLocation = value; }
        public int IsAntisemitic { get => isAntisemitic; set => isAntisemitic = value; }
        public double AntisemitismPercentage { get => antisemitismPercentage; set => antisemitismPercentage = value; }

        public List<FriendsTweeter> IdsFriendsTweeter { get => idsFriendsTweeter; set => idsFriendsTweeter = value; }

        public Tweeter() { }

        public Tweeter getAntisemiticTweeterById(string idTwetter) //get antisemitic tweeter by id
        {
            DBservices dbs = new DBservices();
            Tweeter antisemiticTwitter = dbs.getAntisemiticTweeterById(idTwetter);
            return antisemiticTwitter;
        }

        public void InsertNewTweeters(List<Tweeter> allNewTweeters) //insert new tweeters
        {
            DBservices dbs = new DBservices();
            dbs.InsertTweetersArr(allNewTweeters);
        }

        public void UpdateTweeters(List<Tweeter> AllTweetersToUpdate) //update existing tweeters
        {
            DBservices dbs = new DBservices();
            dbs.UpdateTweeter(AllTweetersToUpdate);
        }

        public List<Tweeter> getAntiTweeters() //get antisemitic tweets by id tweeter
        {
            DBservices dbs = new DBservices();
            List<Tweeter> antiTweetersList = dbs.getAntiTweeters();
            return antiTweetersList;
        }
    }
}