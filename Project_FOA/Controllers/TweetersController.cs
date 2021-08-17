using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class TweetersController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get() //react - get list of antisemitic tweeters
        {
            try
            {
                Tweeter t = new Tweeter();
                return Ok(t.getAntiTweeters());
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        //POST api/<controller>
        public HttpResponseMessage Post([FromBody] List<Tweeter> allNewTweeters) //post tweeters
        {
            try
            {
                Tweeter t = new Tweeter();
                t.InsertNewTweeters(allNewTweeters);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no tweeter was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put( [FromBody] List<Tweeter> AllTweetersToUpdate) //update existing tweeters
        {
            try
            {
                Tweeter t = new Tweeter();
                t.UpdateTweeters(AllTweetersToUpdate);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no tweeter was updated")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}