using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class TweetsController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("api/Tweets/GetTweetsByTweeter/{idTweeter}")]
        public IHttpActionResult GetTweetsByTweeter(string idTweeter) //react - get list of antisemitic tweets by tweeter id
        {
            try
            {
                Tweet t = new Tweet();
                return Ok(t.getAntiTweets(idTweeter));
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/Tweets/GetTweets/{idUser}")]
        public HttpResponseMessage Get(int idUser) //get all tweets for volunteer page
        {
            try
            {
                Tweet t = new Tweet();
                List<Tweet> TweetsList = t.getTweets(idUser);
                return Request.CreateResponse(HttpStatusCode.OK, TweetsList);
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/Tweets/GetTweetsBySearch/{idSearch}")]
        public IHttpActionResult GetTweetsBySearch(int idSearch) //react - get list of search key words with antisemitic tweets arrray
        {
            try
            {
                Tweet t = new Tweet();
                return Ok(t.getTweetsBySearch(idSearch));
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] List<Tweet> allTweets) //post tweets
        {
            try
            {
                Tweet t = new Tweet();              
                return Request.CreateResponse(HttpStatusCode.OK, t.InsertTweets(allTweets));
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no tweet was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/Tweets/PostTweetsBySearch")]
        // POST api/<controller>
        public HttpResponseMessage PostTweetsBySearch([FromBody] List<Tweet> allTweets) //post tweets
        {
            try
            {
                Tweet t = new Tweet();
                t.InsertTweetsBySearch(allTweets);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no tweet was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpPost]
        [Route("api/Tweets/PostTweetsByCustomizedSearch")]
        // POST api/<controller>
        public HttpResponseMessage PostTweetsByCustomizedSearch([FromBody] List<Tweet> allCustomizedTweets) //post tweets
        {
            try
            {
                Tweet t = new Tweet();
                t.InsertTweetsByCustomizedSearch(allCustomizedTweets);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no tweet was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }



        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}