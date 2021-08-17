using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters.Enum;
using Tweetinvi.Exceptions;
using Tweetinvi.Models.Entities;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace Project_FOA.Models
{
    public static class Monitoring
    {
        public static bool checkNeedToExplore = false;
        public static string[] regex = new string[] { "'", "“", "$", "%", "&", "!", "↵", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "^", "_", "`", "[", "]", "{", "|", "}", "~", "→", "\"" };
        public static List<string> TweetsToExplore = new List<string>();
        public static List<string> TweetersToExplore = new List<string>();
        public static string fromProcess = "fromSearch";
        public static List<HashtagsToExplore> hashtagsToExplore = new List<HashtagsToExplore>();
        public static List<Tweet> Alltweets = new List<Tweet>();
        public static TweetersArrs AlltweetersFromApi = new TweetersArrs();
        public static bool isWholeProcessTweeters = true;
        public static List<FriendsTweeter> allTweetersFriend = new List<FriendsTweeter>(); //consists the friends (that exist in the db) list of every tweeter
        public static List<Tweeter> allNewTweeters = new List<Tweeter>(); //consists all new tweeters that we need to insert together into table tweeters_train_2021
        public static List<Tweeter> allTweetersToUpdate = new List<Tweeter>();
        public static ExpendedTweetsTweetersArr AlltweetsFromApi = new ExpendedTweetsTweetersArr();
        public static List<Tweet> AntisemiticTweetsForHashtags = new List<Tweet>();
        public static List<string> GeneralHashtagsNotToSearch = new List<string>();
        public static List<string> SearchSystemKeys = new List<string>();
        public static string str = "";
        public static void scb(List<Search> allTweets)
        {
            System.Diagnostics.Debug.WriteLine("inside scb");

            foreach (var item in allTweets)
            {           
                if (item.TweetsArray.Length > 0)
                {
                    checkNeedToExplore = true;
                    item.TweetsArray = item.TweetsArray.Take(1).ToArray(); //the number in the slice can be changed
                    tweetsFullInfo(item);
                }
            }
            nextToExplore();
        }
        public static void tweetsFullInfo(Search tweetsBySearchObj)
        {
            System.Diagnostics.Debug.WriteLine("fromProcess:" + fromProcess);

            for (var j = 0; j < tweetsBySearchObj.TweetsArray.Length; j++)
            {         
                string idTweet = tweetsBySearchObj.TweetsArray[j].IdStr;               
                string tweetCreatedAt = tweetsBySearchObj.TweetsArray[j].CreatedAt.ToString();
                int quoteCount = Convert.ToInt32(tweetsBySearchObj.TweetsArray[j].QuoteCount);
                int replyCount = Convert.ToInt32(tweetsBySearchObj.TweetsArray[j].ReplyCount);
                int retweetCount = Convert.ToInt32(tweetsBySearchObj.TweetsArray[j].RetweetCount);
                string urlTweet = tweetsBySearchObj.TweetsArray[j].Url;
                string tweetText = tweetsBySearchObj.TweetsArray[j].Text;
                for (int i = 0; i < regex.Length; i++)
                {
                    if (tweetText.Contains(regex[i]))
                    {
                        tweetText = tweetText.Replace(regex[i], " ");
                    }
                }

                string tweetType;
                string attributedTweetId = null; //attributed tweet id 
                if (tweetsBySearchObj.TweetsArray[j].InReplyToStatusIdStr != null)
                {
                    tweetType = "reply";
                    attributedTweetId = tweetsBySearchObj.TweetsArray[j].InReplyToStatusIdStr;
                }
                else if (tweetsBySearchObj.TweetsArray[j].IsRetweet)
                {
                    tweetType = "retweet";
                    attributedTweetId = tweetsBySearchObj.TweetsArray[j].RetweetedTweet.IdStr;
                }
                else if (tweetsBySearchObj.TweetsArray[j].QuotedStatusIdStr != null)
                {
                    tweetType = "quote";
                    attributedTweetId = tweetsBySearchObj.TweetsArray[j].QuotedStatusIdStr;
                }
                else
                {
                    tweetType = "ordinary";
                }
                if (attributedTweetId != null)
                    TweetsToExplore.Add(attributedTweetId);

                int isAntisemitic = 0;
                List<string> attributedTweetersNames = new List<string>();


                for (var k = 0; k < tweetsBySearchObj.TweetsArray[j].UserMentions.Count; k++)
                {
                    attributedTweetersNames.Add(tweetsBySearchObj.TweetsArray[j].UserMentions[k].ScreenName); // user mentions- to remember to insert later in table attributesOfTweets_2021
                    TweetersToExplore.Add(tweetsBySearchObj.TweetsArray[j].UserMentions[k].ScreenName); // user mentions - to explore in API
                }
                int searchId = -1;
                if (fromProcess == "fromSearch")
                {
                    TweetersToExplore.Add(tweetsBySearchObj.TweetsArray[j].CreatedBy.ScreenName); //tweeter of tweet - to explore in API
                    searchId = tweetsBySearchObj.IdSearch;
                }

                List<IHashtagEntity> HashtagsArr = new List<IHashtagEntity>();
                if (fromProcess == "fromSearch")
                {
                    if (tweetType == "retweet")//if it's a retweet,then the hashtags in the retweetedTweet .hastags to explore - without the search word
                        HashtagsArr = tweetsBySearchObj.TweetsArray[j].RetweetedTweet.Hashtags.Where(item => ("#" + item.Text).ToLower() != tweetsBySearchObj.SearchKey.ToLower()).ToList();
                    else//hastags to explore - without the search word
                        HashtagsArr = tweetsBySearchObj.TweetsArray[j].Hashtags.Where(item => ("#" + item.Text).ToLower() != tweetsBySearchObj.SearchKey.ToLower()).ToList();
                }
                else
                    HashtagsArr = tweetsBySearchObj.TweetsArray[j].Hashtags;
                string idTweeter = tweetsBySearchObj.TweetsArray[j].CreatedBy.IdStr;
                List<string> HashtagsList = new List<string>();
                if (HashtagsArr.Count > 0)
                {
                    for (var i = 0; i < HashtagsArr.Count; i++)
                    {
                        HashtagsList.Add(HashtagsArr[i].Text);
                    }
                    HashtagsToExplore ha = new HashtagsToExplore(HashtagsList, idTweet, idTweeter);
                  
                    hashtagsToExplore.Add(ha);
                }
                Tweet tweet = new Tweet(idTweet, tweetCreatedAt, tweetText, quoteCount, replyCount, retweetCount, urlTweet, tweetType, attributedTweetId, isAntisemitic, idTweeter, 0, 0, attributedTweetersNames, searchId, 0);

                Alltweets.Add(tweet);


            }
           
        }

        public static async void nextToExplore()
        {
            System.Diagnostics.Debug.WriteLine("inside nextToExplore");
            TweetersToExplore = TweetersToExplore.Distinct().ToList();

            System.Diagnostics.Debug.WriteLine("length tweeterToExplore: " + TweetersToExplore.Count);
            foreach (var item in TweetersToExplore)
            {
                System.Diagnostics.Debug.WriteLine("tweeterToExplore: " + item);
            }
            TweetsToExplore = TweetsToExplore.Distinct().ToList();

            System.Diagnostics.Debug.WriteLine("length tweetToExplore: " + TweetsToExplore.Count);

            foreach (var item in TweetsToExplore)
            {
                System.Diagnostics.Debug.WriteLine("tweetToExplore: " + item);
            }

            System.Diagnostics.Debug.WriteLine("checkNeedToExplore: " + checkNeedToExplore);

            if (checkNeedToExplore)
                await callTweetersToExplore();
          
        }
        public static async Task callTweetersToExplore()
        {
            System.Diagnostics.Debug.WriteLine("callTweetersToExplore");
            AlltweetersFromApi =null;
            isWholeProcessTweeters = true;
            var tweetersStr = "";
            TweetersToExplore = TweetersToExplore.Where(tweeterName => ((tweeterName != "YouTube") && (tweeterName != "CNN"))).ToList();
            for (var i = 0; i < TweetersToExplore.Count; i++)
            {
                tweetersStr += TweetersToExplore[i] + ","; //user mentions of tweet - to explore in API               
            }
            tweetersStr = tweetersStr.Substring(0, tweetersStr.Length-1);
            Twitter tm = new Twitter();

            List<Tweeter> Allusers = await tm.getTweeter(tweetersStr); //information from twitter

            AlltweetersFromApi = await tm.getTweetersInfo(Allusers);
            TweetersToExplore.Clear();
            createTweeters();
            tweetersStr = "";
        }
        public static void createTweeters()
        {
            System.Diagnostics.Debug.WriteLine("createTweeters");
            allNewTweeters.Clear();
            allTweetersToUpdate.Clear();

            allNewTweeters = AlltweetersFromApi.AllNewTweeters;
            allTweetersToUpdate = AlltweetersFromApi.AllTweetersToUpdate;
           

            allNewTweeters = allNewTweeters.Distinct().ToList();// remove duplicates

            foreach (var item in allNewTweeters)
            {
                System.Diagnostics.Debug.WriteLine("NewTweeter: " + item.IdTweeter);
            }

            allTweetersToUpdate = allTweetersToUpdate.Distinct().ToList();// remove duplicates

            foreach (var item in allTweetersToUpdate)
            {
                System.Diagnostics.Debug.WriteLine("updateTweeter: " + item.IdTweeter);
            }


            //const regex = /["']/g;
            string[] regexToLocation = new string[] { "'","\"" };


            for (var i = 0; i < allNewTweeters.Count; i++)
            {
                if (allNewTweeters[i].TweeterLocation != null)
                {
                    for (int j = 0; j < regexToLocation.Length; j++)
                    {
                        if (allNewTweeters[i].TweeterLocation.Contains(regex[j]))
                        {
                            allNewTweeters[i].TweeterLocation = allNewTweeters[i].TweeterLocation.Replace(regexToLocation[j], " ");
                        }
                    }
                }
                
            }
            for (var i = 0; i < allTweetersToUpdate.Count; i++)
            {
                if (allTweetersToUpdate[i].TweeterLocation != null)
                {
                    for (int j = 0; j < regexToLocation.Length; j++)
                    {
                        if (allTweetersToUpdate[i].TweeterLocation.Contains(regex[j]))
                        {
                            allTweetersToUpdate[i].TweeterLocation = allTweetersToUpdate[i].TweeterLocation.Replace(regexToLocation[j], "");
                        }
                    }
                    
                }
            }
            Tweeter t = new Tweeter();
            if (allNewTweeters.Count > 0)
            { //if we have new tweeters then we post them, then we update friends
                t.InsertNewTweeters(allNewTweeters);              
            }
            if (allTweetersToUpdate.Count > 0)
            {// and if we also have existing tweeters we update them                   
                t.UpdateTweeters(allTweetersToUpdate);
            }
            postSuccessAndFriends();
            

        }

        public static void postSuccessAndFriends()
        {
            System.Diagnostics.Debug.WriteLine("postSuccessAndFriends");

            allTweetersFriend.Clear();
            ITweet[] allTweetsByTweeter;                 
            allTweetersFriend = AlltweetersFromApi.AllTweetersFriend;
            allTweetsByTweeter =AlltweetersFromApi.TweetsArray;

            allTweetersFriend = allTweetersFriend.Distinct().ToList();// remove duplicates
         
            if (allTweetersFriend.Count > 0)
            {
                FriendsTweeter ft = new FriendsTweeter();
                ft.InsertTweetersFriend(allTweetersFriend);
            }
            
            allTweetsByTweeter = allTweetsByTweeter.Distinct().ToArray();// remove duplicates
            Search s = new Search(-1, "", allTweetsByTweeter);
            fromProcess = "fromTweeters";
            checkNeedToExplore = false;

            tweetsFullInfo(s);           
            nextToExplore();
  
            if (isWholeProcessTweeters) //if not - we just wanted to post the tweeters. we don't need to post tweets.
             callTweets(); // when we finish posting all tweeters, we do ajax call of tweets
        }
        public static async void callTweets()
        {
            System.Diagnostics.Debug.WriteLine("callTweets");
            var tweetsToExplore = "";
            AlltweetsFromApi = null;           
            TweetsToExplore = TweetsToExplore.Distinct().ToList();// remove duplicates

            List<string> CopyTweetsToExplore = TweetsToExplore.ToList();            
            for (var i = 0; i < TweetsToExplore.Count; i++)
            {
                Tweet found = Alltweets.Find(item => item.IdTweet == TweetsToExplore[i]);
                if (found != null)
                    CopyTweetsToExplore = CopyTweetsToExplore.Where(tweet => tweet != found.IdTweet).ToList();
            }
            TweetsToExplore = CopyTweetsToExplore.ToList();
            if (TweetsToExplore.Count > 0)
            { //there are tweets to explore
                for (var i = 0; i < TweetsToExplore.Count; i++)
                {
                    tweetsToExplore += TweetsToExplore[i] + ","; //tweets ids to explore in API                        
                  
                }
                
                tweetsToExplore = tweetsToExplore.Substring(0, tweetsToExplore.Length - 1);

                System.Diagnostics.Debug.WriteLine("tweetsToExplore(getting extended info): " + tweetsToExplore);
                Twitter tm = new Twitter();
                List<ExpendedTweet> expendedTweetsArr = await tm.getTweets(tweetsToExplore); //information from twitter (about the tweet + the tweeter)
                List<Tweeter> tweetersToGetMoreInfo = new List<Tweeter>();
                for (int i = 0; i < expendedTweetsArr.Count; i++)
                {
                    tweetersToGetMoreInfo.Add(expendedTweetsArr[i].Author);
                }


                TweetersArrs tweetersArrs = await tm.getTweetersInfo(tweetersToGetMoreInfo); //whole information about the tweeters (including calculations)

                AlltweetsFromApi = new ExpendedTweetsTweetersArr(expendedTweetsArr, tweetersArrs); // all information about tweets + tweeters
                tweetsToExplore = "";
                createTweets();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("no tweets to explore.");
                System.Diagnostics.Debug.WriteLine("check if there are tweeters to explore: ");
                foreach (var item in TweetersToExplore)
                {
                    System.Diagnostics.Debug.WriteLine("(there is a problem here) tweeterToExplore: " + item);
                }
                System.Diagnostics.Debug.WriteLine("post tweets");
                postTweets();
            }
        }
        public static async void createTweets()
        {
            System.Diagnostics.Debug.WriteLine("createTweets");
            TweetsToExplore.Clear();
            AlltweetersFromApi = null;
            for (var j = 0; j < AlltweetsFromApi.ExpendedTweets.Count; j++)
            {

                if (AlltweetsFromApi.ExpendedTweets[j].Hashtags.Count > 0)
                { // the tweet has hashtags
                    HashtagsToExplore hs = new HashtagsToExplore(AlltweetsFromApi.ExpendedTweets[j].Hashtags, AlltweetsFromApi.ExpendedTweets[j].IdTweet, AlltweetsFromApi.ExpendedTweets[j].IdTweeter);
                    hashtagsToExplore.Add(hs);
                }

                if (AlltweetsFromApi.ExpendedTweets[j].AttributedTweetersNames.Count > 0) // the tweet has attributed tweeters
                    TweetersToExplore = TweetersToExplore.Concat(AlltweetsFromApi.ExpendedTweets[j].AttributedTweetersNames).ToList();
                if (AlltweetsFromApi.ExpendedTweets[j].AttributedTweetId != null) // the tweet has attributed tweet
                    TweetsToExplore.Add(AlltweetsFromApi.ExpendedTweets[j].AttributedTweetId);

                string tweetText = AlltweetsFromApi.ExpendedTweets[j].TweetText;
                for (int i = 0; i < regex.Length; i++)
                {
                    if (tweetText.Contains(regex[i]))
                    {
                        tweetText = tweetText.Replace(regex[i], " ");
                    }
                }

                Tweet tweet = new Tweet(AlltweetsFromApi.ExpendedTweets[j].IdTweet, AlltweetsFromApi.ExpendedTweets[j].TweetCreatedAt, tweetText, AlltweetsFromApi.ExpendedTweets[j].QuoteCount, AlltweetsFromApi.ExpendedTweets[j].ReplyCount, AlltweetsFromApi.ExpendedTweets[j].RetweetCount, AlltweetsFromApi.ExpendedTweets[j].UrlTweet, AlltweetsFromApi.ExpendedTweets[j].TweetType, AlltweetsFromApi.ExpendedTweets[j].AttributedTweetId, AlltweetsFromApi.ExpendedTweets[j].IsAntisemitic, AlltweetsFromApi.ExpendedTweets[j].IdTweeter, AlltweetsFromApi.ExpendedTweets[j].AntisemitismPercentage, AlltweetsFromApi.ExpendedTweets[j].FinalScore, AlltweetsFromApi.ExpendedTweets[j].AttributedTweetersNames, AlltweetsFromApi.ExpendedTweets[j].SearchId, AlltweetsFromApi.ExpendedTweets[j].UserId);
                Alltweets.Add(tweet);
            }
            AlltweetersFromApi = AlltweetsFromApi.TweetersArrays;
            TweetersToExplore = TweetersToExplore.Distinct().ToList();// remove duplicates
            TweetsToExplore = TweetsToExplore.Distinct().ToList();
            isWholeProcessTweeters = false;

            foreach (var item in TweetersToExplore)
            {
                System.Diagnostics.Debug.WriteLine("(tweetersToExplore: " + item);
            }

            foreach (var item in TweetsToExplore)
            {
                System.Diagnostics.Debug.WriteLine("(tweetsToExplore: " + item);
            }

            createTweeters(); // we have the all the tweeters(who wrote the tweets) info, we just need to do the post - half process
            if (TweetersToExplore.Count > 0 || TweetsToExplore.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("(TweetersToExplore or TweetsToExplore have length > 0)");
                int chkLenTweetersToExplore = 0; //TweetersToExplore.length = 0     
                if (TweetersToExplore.Count > 0)
                { //two cases : 1. there are tweeters to explore + tweets to explore. 2. there are tweeters to explore but not tweets to explore.
                     await callTweetersToExplore(); // only attributed tweeters - we need the whole process 
                    chkLenTweetersToExplore = 1; //TweetersToExplore.length > 0
                }
                if (chkLenTweetersToExplore == 0 && TweetsToExplore.Count > 0) // one case : there are tweets to explore but not tweeters to explore
                    callTweets();
            }

            System.Diagnostics.Debug.WriteLine("(length tweeters to explore: " + TweetersToExplore.Count + ", length tweets to explore: " + TweetsToExplore.Count);

            if (TweetersToExplore.Count == 0 && TweetsToExplore.Count == 0)
            {
                postTweets();
            }
        }
        public static void postTweets()
        {
            System.Diagnostics.Debug.WriteLine("postTweets");

            if (Alltweets.Count > 0)
            { //there are tweets to post


                Alltweets= Alltweets.DistinctBy(x => x.IdTweet).Distinct().ToList();
                Alltweets = Alltweets.OrderBy(o => o.IdTweet).ToList();

                foreach (var item in Alltweets)
                {
                    System.Diagnostics.Debug.WriteLine("Alltweets: " + item.IdTweet);
                }

                Tweet t = new Tweet();
                int b = 10;
                AntisemiticTweetsForHashtags=t.InsertTweets(Alltweets);

                //ajaxCall('POST', '../api/Tweets', JSON.stringify(Alltweets), postSuccessTweets, ecb);               
                t.InsertTweetsBySearch(Alltweets);
                Alltweets.Clear();
                Search s = new Search();
                GeneralHashtagsNotToSearch = s.GetGeneralHashtagsNotToSearch();
                SearchSystemKeys = s.GetSearchSystemKeys();
                List<string> HashtagsOfAntisemiticTweets = handleHashtagsToExplore();
                HashtagsToManager htm = new HashtagsToManager();
                htm.InsertHashtagsToManager(HashtagsOfAntisemiticTweets);
            }

            System.Diagnostics.Debug.WriteLine("length Alltweets: " + Alltweets.Count);
        }
        public static List<string> handleHashtagsToExplore()
        {
            List<string>HashtagsOfAntisemiticTweets = new List<string>();
            for (var i = 0; i < hashtagsToExplore.Count; i++)
            {
                Tweet foundAntisemiticTweet = AntisemiticTweetsForHashtags.Find(item => item.IdTweet == hashtagsToExplore[i].IdTweet);
                if (foundAntisemiticTweet != null)
                { //hashtag object of antisemitic tweet
                    for (var j = 0; j < hashtagsToExplore[i].Hashtags.Count; j++)
                    {
                        HashtagsOfAntisemiticTweets.Add(hashtagsToExplore[i].Hashtags[j].ToLower()); //all hashtags of antisemitic tweets
                    }
                }
            }

            //list of dictionary - count how many times each hashtag appears
            var countOccurrencesHashtags = HashtagsOfAntisemiticTweets.GroupBy(v => v);
            
            HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.Distinct().ToList();// remove duplicates
            foreach (var group in countOccurrencesHashtags)
            {
                if(group.Count()<2)
                {
                    HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.Where(item => item != group.Key).ToList(); //remove hashtag from array (because it doesn't appear many times)

                }
            }

            if (HashtagsOfAntisemiticTweets.Count > 0)
            {
                for (var i = 0; i < GeneralHashtagsNotToSearch.Count; i++)
                {//check if exists in general list of hashtags
                    HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.Where(item => ("#" + item) != GeneralHashtagsNotToSearch[i]).ToList();
                }
                for (var i = 0; i < SearchSystemKeys.Count; i++)
                {//check if exists in search list of hashtags(is active =1)
                    HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.Where(item => ("#" + item) != SearchSystemKeys[i]).ToList();
                }
            }
            return HashtagsOfAntisemiticTweets;

        }











    }
}


