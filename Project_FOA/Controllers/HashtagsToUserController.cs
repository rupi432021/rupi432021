using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class HashtagsToUserController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("api/HashtagsToUser/{idUser}")]
        public HttpResponseMessage Get(int idUser) // get all explored ExploredHashtags by user
        {
            try
            {
                HashtagsToUser htu = new HashtagsToUser();
                return Request.CreateResponse(HttpStatusCode.OK, htu.getExploredHashtags(idUser));
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


        public HttpResponseMessage Post([FromBody] HashtagsToUser hashtagsToUser) //post hashtages to user
        {
            try
            {
                HashtagsToUser htm = new HashtagsToUser();
                htm.InsertHashtagsToUser(hashtagsToUser);
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

        // POST api/<controller>
        [HttpPost]
        [Route("api/HashtagsToUser/PostFromExploredToPersonalList")]
        public HttpResponseMessage PostFromExploredToPersonalList([FromBody] List<HashtagsToUser> hashtagsToUser) //post hashtages to user
        {
            try
            {
                HashtagsToUser htm = new HashtagsToUser();                
                return Request.CreateResponse(HttpStatusCode.OK, htm.PostFromExploredToPersonalList(hashtagsToUser));
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
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(List<HashtagsToUser> hashtagsToUser) // search keys of user to delete
        {
            try
            {
                HashtagsToUser htu = new HashtagsToUser();
                return Request.CreateResponse(HttpStatusCode.OK, htu.deleteHashtagsToUser(hashtagsToUser));
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
    }
}