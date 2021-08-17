using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class BarAntiTweetsByTypesController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get() //react - get bar chart details - type counts of antisemitic tweets
        {
            try
            {
                BarAntiTweetsByType at = new BarAntiTweetsByType();
                return Ok(at.getBarAntiTweetsByType());
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