using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class HashtagsToManagerController : ApiController
    {
        public IHttpActionResult Get() //react - get list objects of Hashtags To Manager
        {
            try
            {
                HashtagsToManager htm = new HashtagsToManager();
                return Ok(htm.getHashtagsToManager());
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
        public HttpResponseMessage Post([FromBody] List<string> hashtagsToManager) //post hashtages to manager
        {
            try
            {
                HashtagsToManager htm = new HashtagsToManager();
                htm.InsertHashtagsToManager(hashtagsToManager);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no hashtags to manager were inserted")
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
        public IHttpActionResult Delete(string idsExploredHashtagsStr) //react - update search key words to be not active (we won't search in twitter with them anymore)
        {
            try
            {
                var idsExploredHashtagsArr = idsExploredHashtagsStr.Split(',');

                HashtagsToManager s = new HashtagsToManager();
                foreach (var item in idsExploredHashtagsArr)
                {
                    int idExploredHashtags = Int32.Parse(item);
                    s.deleteExploredHashtags(idExploredHashtags);
                }

                return Ok("success");
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }
    }
}