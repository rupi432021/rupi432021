using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class PieAntiFriendsController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get() //react - get bar chart details - top 10 tweeters with antisemitic friends
        {
            try
            {
                PieAntiFriend af = new PieAntiFriend();
                return Ok(af.getPieAntiFriends());
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
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