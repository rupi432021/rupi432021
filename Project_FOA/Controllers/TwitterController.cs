using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Project_FOA.Models;

namespace Project_FOA.Controllers
{
    public class TwitterController : ApiController
    {
        // GET api/<controller>
        public async Task<Object> Get() //get tweets by search key words
        {
            Twitter tm = new Twitter();
            return await tm.test();
        }

   
        [HttpGet]
        [Route("api/Twitter/getTweetersByName/{tweetersStr}")] //get tweeters by username
        public async Task<Object> Get(string tweetersStr)
        {
            Twitter tm = new Twitter();

            List<Tweeter> Allusers = await tm.getTweeter(tweetersStr); //information from twitter

            return await tm.getTweetersInfo(Allusers); //whole information about the tweeters (including calculations)
        }

        [HttpGet]
        [Route("api/Twitter/getTweetsByCustomizedSearch/{searchKeywords}")] //get customized search
        public async Task<Object> getTweetsByCustomizedSearch(string searchKeywords) //get tweets by search key words
        {
            Twitter tm = new Twitter();
            return await tm.getTweetsByCustomizedSearch(searchKeywords);
        }

        [HttpGet]
        [Route("api/Twitter/getTweetsByIds/{tweetsToExplore}")] //get tweets by id
        public async Task<Object> getTweetsByIds(string tweetsToExplore)
        {
            Twitter tm = new Twitter();
            List<ExpendedTweet> expendedTweetsArr = await tm.getTweets(tweetsToExplore); //information from twitter (about the tweet + the tweeter)
            List<Tweeter> tweetersToGetMoreInfo = new List<Tweeter>(); 
            for (int i = 0; i < expendedTweetsArr.Count; i++)
            {
                tweetersToGetMoreInfo.Add(expendedTweetsArr[i].Author);
            }

            TweetersArrs tweetersArrs = await tm.getTweetersInfo(tweetersToGetMoreInfo); //whole information about the tweeters (including calculations)

            ExpendedTweetsTweetersArr etta = new ExpendedTweetsTweetersArr(expendedTweetsArr, tweetersArrs); // all information about tweets + tweeters

            return etta;
        }

        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}