using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters.Enum;
using Tweetinvi.Parameters.V2;
using System.Security.Policy;
using Project_FOA.Models.DAL;
using Microsoft.Ajax.Utilities;
using Tweetinvi.Exceptions;
using Tweetinvi.Core.Models;
using System.Data;
using System.Runtime.InteropServices;

namespace Project_FOA.Models
{
    public class Twitter
    {
        //old tokens!!! 
        //const string apiKey = "rtAam1dOD4Cg3NOO272BgOuCI";
        //const string apiKeySecret = "qlnGvuzOVDwLtbB0zvNOQwL34hfXrSd3bxJkzlGBSTZrVIgJ10";
        //const string accessToken = "1348717792258891789-KHqssfuxrjyTIYJ7sMStc7AAJvhNla";
        //const string accessTokenSecret = "bjlznhJV9ClefxphjC5BzrcNLicIIRTxmfVZsI2ZUR84d";

        //new tokens!!!
        const string apiKey = "XhU15jfU4AysmqxI9KnZ1iRzO";
        const string apiKeySecret = "goZYjm8weo5KlGVt99buVT9y02qOXoUH7lh4q9CBNfqaxVxPhK";
        const string accessToken = "1377561861114200064-VoFL2mMXKxhz9R9f48xgIQxTrqj8zQ";
        const string accessTokenSecret = "7xUDX4w5AqSKlL40VQ6cndueWSgPiF1flC0gYX1t4TckR";

        //new tokens - 15.6 !!!
        //const string apiKey = "1yjINAmeGnoY4cH9OgdHoggRs";
        //const string apiKeySecret = "iVDfuLftiJTCrr2GEyDsOosaIQNUaD3hq2PD3P8vojqIgsh8NM";
        //const string accessToken = "AAAAAAAAAAAAAAAAAAAAADm3QgEAAAAApwk9umqIseJXZFxyFDLktHKhPFo%3DNvYdbv10O0mMiIKYiwdBTVq0ul4wFdjTifh7MK3BZUdfVloIyx";
        //const string accessTokenSecret = "bjlznhJV9ClefxphjC5BzrcNLicIIRTxmfVZsI2ZUR84d";

        public Twitter() { }

        public async Task<List<Search>> test() //get tweets from twitter by search key words
        {
            Search s = new Search();
            HelpKeyWord h = new HelpKeyWord();
            List<Search> searchList = s.getSearchForMonitoring(); 
            List<HelpKeyWord> helpKeyWordsList = h.getHelpKeyWords();
            List<string> prefixJewsWordsList = h.getPrefixJewsWords();
            List<Search> allTweets = new List<Search>();

            var tc = new TwitterClient(apiKey, apiKeySecret, accessToken, accessTokenSecret);

            // enable track and await - because of rate limit
            tc.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            string str = "";

            foreach (var item in searchList)
            {
                bool hasPrefix = false;
                foreach (var item2 in prefixJewsWordsList)
                {
                    if ((item.chkContains(item2)))
                    {
                        hasPrefix = true;
                        break;
                    }
                }

                if (!hasPrefix)
                {
                    foreach (var item2 in helpKeyWordsList) // the first help key word is "" so we can search the keySearch alone 
                    {
                        try
                        {
                            str = item.SearchKey + " " + item2.KeyWord;

                            var parameters = new SearchTweetsParameters(str)
                            {
                                Filters = TweetSearchFilters.Hashtags,
                                Lang = LanguageFilter.English
                            };

                            var tweets = await tc.Search.SearchTweetsAsync(parameters);

                            Search tweetsBySearch = new Search(item.IdSearch, item.SearchKey, tweets);

                            allTweets.Add(tweetsBySearch);

                        }
                        catch (TwitterException ex)
                        {
                            //Error err = new Error();

                            //err.Text = ex.Message;

                            //err.insert();
                        }
                    }
                }
                else {
                    try
                    {
                        var parameters = new SearchTweetsParameters(item.SearchKey)
                        {
                            Filters = TweetSearchFilters.Hashtags,
                            Lang = LanguageFilter.English
                        };

                        var tweets = await tc.Search.SearchTweetsAsync(parameters);

                        Search tweetsBySearch = new Search(item.IdSearch, item.SearchKey, tweets);

                        allTweets.Add(tweetsBySearch);

                    }
                    catch (TwitterException ex)
                    {
                        //Error err = new Error();

                        //err.Text = ex.Message;

                        //err.insert();
                    }
                }
            }
          
            return allTweets;
        }


        public async Task<object> getTweetsByCustomizedSearch(string searchKeywords) //get tweets from twitter by customized search
        {
            var searchKeywordsPairs = searchKeywords.Split(',');
            List<string> keysToSearch = new List<string>();

            for (int i = 0; i < searchKeywordsPairs.Length; i++)
            {
                string word = "";
                var splitedPair = searchKeywordsPairs[i].Split('-');
                if (splitedPair[0] == "Hashtag")
                    word = "#" + splitedPair[1];
                else
                    word = "”" + splitedPair[1] + "”";
                keysToSearch.Add(word);
            }

            List<Object> allTweets = new List<object>();

            var tc = new TwitterClient(apiKey, apiKeySecret, accessToken, accessTokenSecret);

            // enable track and await - because of rate limit
            tc.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            foreach (var item in keysToSearch)
            {
                    try
                    {
                    CustomizedSearch cs = new CustomizedSearch();
                    CustomizedSearch customizedSearch = cs.getCustomizedSearchByKey(item); //all details of keyword

                            var parameters = new SearchTweetsParameters(item)
                            {
                                Filters = TweetSearchFilters.Hashtags,
                                Lang = LanguageFilter.English
                            };

                            var tweets = await tc.Search.SearchTweetsAsync(parameters);

                    //tweets found by customized keyword
                            CustomizedSearch tweetsBySearch = new CustomizedSearch(customizedSearch.IdSearch , customizedSearch.SearchKey, 1, tweets);

                            allTweets.Add(tweetsBySearch);
                        
                    }
                    catch (TwitterException ex)
                    {
                        //Error err = new Error();

                        //err.Text = ex.Message;

                        //err.insert();
                    }
                
            }


            return allTweets;
        }



        //get the tweeters information - including the calculation of : antisemitic tweets, antisemitic friends, etc
        public async Task<TweetersArrs> getTweetersInfo(List<Tweeter> tweetersArr)
        {
            var tc = new TwitterClient(apiKey, apiKeySecret, accessToken, accessTokenSecret);

            List<FriendsTweeter> allTweetersFriends = new List<FriendsTweeter>(); //consists the friends (that exist in the db) list of every tweeter
            List<Tweeter> allNewTweeters = new List<Tweeter>(); //consists all new tweeters that we need to insert together into table tweeters_train_2021
            List<Tweeter> allTweetersToUpdate = new List<Tweeter>(); //consists all tweeters that exist in the db and we need to update them
            List<ITweet> tweetsList = new List<ITweet>();
            DBservices dbs = new DBservices();

            // enable track and await - because of rate limit
            tc.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            List<Tweeter> tList = new List<Tweeter>(); //list of tweeters that exists in the db
            dbs = dbs.getTweetersDT(); // using data table

            //check if tweeter exists in data table(from data set)
            foreach (DataRow dr in dbs.dt.Rows)
            {
                Tweeter t = new Tweeter();

                t.IdTweeter = (string)dr["idTweeter"]; ;
                t.TweeterName = (string)dr["tweeterName"];
                t.FollowersCount = Convert.ToInt32(dr["followersCount"]);
                t.FriendsCount = Convert.ToInt32(dr["friendsCount"]);
                t.TweeterCreatedAtAccount = (string)dr["tweeterCreatedAtAccount"];
                t.StatusesCountSinceCreated = Convert.ToInt32(dr["statusesCountSinceCreated"]);
                t.StatusesCountSinceChecked = Convert.ToInt32(dr["statusesCountSinceChecked"]);
                t.AntisemiticTweets = Convert.ToInt32(dr["antisemiticTweets"]);
                t.AntisemiticFriends = Convert.ToInt32(dr["antisemiticFriends"]);
                t.TweeterLocation = (string)dr["tweeterLocation"];
                t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);
                tList.Add(t);
            }

            for (int i = 0; i < tweetersArr.Count; i++)
            {
                //check if tweeter exists
                Tweeter t = null;
                int antisemiticTweeter = 0;
                int antisemiticTweets = 0;
                t = tList.Find(tweeterObj => tweetersArr[i].IdTweeter == tweeterObj.IdTweeter); //find twetter

                if (t != null) //tweeter exists
                {
                    antisemiticTweeter = t.IsAntisemitic; //tweeter is anitsemitic(yes\no)
                    antisemiticTweets = t.AntisemiticTweets;
                }
                tweetersArr[i].FriendsCount = 0;
                tweetersArr[i].AntisemiticFriends = 0;
                try
                {
                    var friendIds = await tc.Users.GetFriendIdsAsync(tweetersArr[i].TweeterName); //all friends ids of tweeter
                    int cntAntisemiticFriends = 0;

                    for (int j = 0; j < friendIds.Length; j++)  // 1. count antisemitic friends 2. connection with friend (suspected or not)
                    {
                        string friendId = friendIds[j].ToString();
                        int antisemiticFriendTweeter = 0;
                        Tweeter friendTweeter = tList.Find(tweeterObj => friendId == tweeterObj.IdTweeter); //find twetter friend
                        if (friendTweeter != null) //friend exists in db
                        {
                            antisemiticFriendTweeter = friendTweeter.IsAntisemitic; //friend tweeter is anitsemitic (yes\no)
                            if (antisemiticFriendTweeter != 0) // friend is antisemitic
                                cntAntisemiticFriends++;

                            FriendsTweeter ft = new FriendsTweeter();//object to insert into friendsOfTweeters_2021 table

                            if (t != null) // tweeter exists
                            {
                                if (antisemiticTweeter != 0 || antisemiticFriendTweeter != 0) // tweeter is antisemitic or friend antisemitic 
                                    ft = new FriendsTweeter(tweetersArr[i].IdTweeter, friendId, 1, false);//connection is suspected
                                else // tweeter exists but not antisemitic, and friend exists but not antismeitic
                                    ft = new FriendsTweeter(tweetersArr[i].IdTweeter, friendId, 0, false); //connection is not suspected
                            }
                            else // tweeter is new 
                            {
                                if (antisemiticFriendTweeter != 0) // tweeter is new, friend is antisemitic
                                    ft = new FriendsTweeter(tweetersArr[i].IdTweeter, friendId, 1, true);
                                else // tweeter is new, friend is not antisemitic
                                    ft = new FriendsTweeter(tweetersArr[i].IdTweeter, friendId, 0, true);//later when we make a decision about the new tweeter - if we decide that he is anti-Semitic then the connection is suspected
                            }
                            allTweetersFriends.Add(ft);
                        }
                    }

                    tweetersArr[i].FriendsCount = friendIds.Length;
                    tweetersArr[i].AntisemiticFriends = cntAntisemiticFriends;
                }
                catch (TwitterException ex)
                {
                    Error err = new Error();

                    err.Text = ex.Message.Substring(0, 100);

                    err.insert();
                }
                try {
                    if (antisemiticTweets > 0) //followers after tweeter - if the tweeter has at least 1 antisemitic tweets then we will ask 2 more tweets from twitter api
                    {
                        string tweeterName = tweetersArr[i].TweeterName;
                        var tweets = await tc.Timelines.GetUserTimelineAsync(tweeterName);
                        var tweetsBytweeter = tweets.Take(2);
                        tweetsList = tweetsList.Concat(tweetsBytweeter).ToList<ITweet>();
                    }

                    if (t == null) // tweeter is new 
                        allNewTweeters.Add(tweetersArr[i]); //add the new tweeter into the array of all new tweeters
                    else // tweeter exists
                    {
                        allTweetersToUpdate.Add(tweetersArr[i]);
                        //update some fields of tweeter - friendsCount, followersCount.. 
                    }
                }
                
                catch (TwitterException ex)
                {
                    Error err = new Error();

                    err.Text = ex.Message.Substring(0,100);

                    err.insert();
                }
            }

            var tweetsArray = tweetsList.ToArray();
            TweetersArrs ta = new TweetersArrs(allNewTweeters, allTweetersFriends, allTweetersToUpdate, tweetsArray);
            return ta;
        }

        //get tweeter information from twitter
        public async Task<List<Tweeter>> getTweeter(string tweetersToExplore)
        {
            var tc = new TwitterClient(apiKey, apiKeySecret, accessToken, accessTokenSecret);

            var tweetersArray = tweetersToExplore.Split(',');

            List<Tweeter> allUsers = new List<Tweeter>();

            // enable track and await - because of rate limit
            tc.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

            foreach (var item in tweetersArray)
            {
                try
                {
                    var userResponse = await tc.UsersV2.GetUserByNameAsync(item);
                    var user = userResponse.User;
                    if (user != null) // it can be null if the user is suspended in twitter
                    {
                        Tweeter tweeter = new Tweeter(user.Id, user.Username, user.PublicMetrics.FollowersCount, 0, user.CreatedAt.ToString(), user.PublicMetrics.TweetCount, 0, 0, 0, user.Location, 0,0, null);
                        allUsers.Add(tweeter);
                    }
                }
                catch (TwitterException e)
                {
                    Error err = new Error();

                    err.Text = e.Message.Substring(0, 100);

                    err.insert();
                }
            }
            return allUsers;
        }

        //get tweets information from twitter by id - and get the information about their tweeters too
        public async Task<List<ExpendedTweet>> getTweets(string tweetsToExplore)
        {
            var tc = new TwitterClient(apiKey, apiKeySecret, accessToken, accessTokenSecret);

            var str = tweetsToExplore.Split(',');

            List<ExpendedTweet> AllExpendedTweets = new List<ExpendedTweet>();
            
            foreach (var item in str)
            {
                try
                {
                    var tweetResponse = await tc.TweetsV2.GetTweetAsync(item);
                    var tweetApi = tweetResponse.Tweet;
                    if (tweetApi != null) //the tweet is not removed from twitter
                    {
                        var tweetAuthor = tweetResponse.Includes.Users[0]; //include information about the tweeter of the tweet
                        if (tweetAuthor != null)
                        {
                            string urlTweet = "https://twitter.com/" + tweetAuthor.Username + "/status/" + tweetApi.Id;
                            string tweetType = "";
                            string attributedTweetId = null;

                            if (tweetApi.ReferencedTweets == null)
                                tweetType = "ordinary";
                            else
                            {
                                attributedTweetId = tweetApi.ReferencedTweets[0].Id;
                                switch (tweetApi.ReferencedTweets[0].Type)
                                {
                                    case "quoted":
                                        tweetType = "quote";
                                        break;
                                    case "replied_to":
                                        tweetType = "reply";
                                        break;
                                    default:
                                        tweetType = "retweeted";
                                        break;
                                }
                            }

                            List<string> attributedTweetersNames = new List<string>(); //user mentions 
                            List<string> hashtags = new List<string>(); //hashtags to explore later

                            if (tweetApi.Entities != null)
                            {
                                if (tweetApi.Entities.Mentions != null) // there are user mentions
                                {
                                    for (int i = 0; i < tweetApi.Entities.Mentions.Length; i++)
                                    {
                                        attributedTweetersNames.Add(tweetApi.Entities.Mentions[i].Username);
                                    }
                                }
                                if (tweetApi.Entities.Hashtags != null) // there are hashtags
                                {
                                    for (int i = 0; i < tweetApi.Entities.Hashtags.Length; i++)
                                    {
                                        hashtags.Add(tweetApi.Entities.Hashtags[i].Tag);
                                    }
                                }
                            }
                            Tweeter tweeter = new Tweeter(tweetAuthor.Id, tweetAuthor.Username, tweetAuthor.PublicMetrics.FollowersCount, 0, tweetAuthor.CreatedAt.ToString(), tweetAuthor.PublicMetrics.TweetCount, 0, 0, 0, tweetAuthor.Location, 0, 0, null);
                            ExpendedTweet et = new ExpendedTweet(tweetApi.Id, tweetApi.CreatedAt.ToString(), tweetApi.Text, tweetApi.PublicMetrics.QuoteCount, tweetApi.PublicMetrics.ReplyCount, tweetApi.PublicMetrics.RetweetCount, urlTweet, tweetType, attributedTweetId, 0, tweetAuthor.Id, 0, 0, attributedTweetersNames, -1, 0, tweeter, hashtags);
                            AllExpendedTweets.Add(et);
                        }
                    }
                }
                catch (TwitterException e)
                {
                    //Error err = new Error();

                    //err.Text = e.Message;

                    //err.insert();
                }
            }
            return AllExpendedTweets;
        }

    }
}