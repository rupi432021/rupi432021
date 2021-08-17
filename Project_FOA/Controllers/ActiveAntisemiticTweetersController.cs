using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class ActiveAntisemiticTweetersController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get() //react - get top 5 active tweeters (with the most reported antisemitic tweets) 
        {
            try
            {
                ActiveAntisemiticTweeters at = new ActiveAntisemiticTweeters();
                return Ok(at.getActiveAntisemiticTweeters());
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

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
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