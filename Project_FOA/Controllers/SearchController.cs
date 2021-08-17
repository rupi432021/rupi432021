using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class SearchController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get() //react - get list of search key words with antisemitic tweets arrray
        {
            try
            {
                Search s = new Search();
                return Ok(s.getSearch());
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/Search/GetGeneralHashtagsNotToSearch")]
        public HttpResponseMessage GetGeneralHashtagsNotToSearch() ////get general hashtags not to search
        {
            try
            {
                Search s = new Search();
                return Request.CreateResponse(HttpStatusCode.OK, s.GetGeneralHashtagsNotToSearch());
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
        [Route("api/Search/GetSearchSystemKeys")]
        public HttpResponseMessage GetSearchSystemKeys() //get search system key
        {
            try
            {
                Search s = new Search();
                return Request.CreateResponse(HttpStatusCode.OK, s.GetSearchSystemKeys());
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



        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post(string searchToPost) //react - add search key
        {
            try
            {
                Search s = new Search();      
                return Ok(s.postSearch(searchToPost));
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }


        // POST api/<controller>
        [HttpPost]
        [Route("api/Search/PostFromExploredToSearch/{hashtagsToPost}")]
        public IHttpActionResult PostFromExploredToSearch(string hashtagsToPost) //react - post list of explored hashtags to the system search
        {
            try
            {
                Search s = new Search();
                s.PostFromExploredToSearch(hashtagsToPost);
                return Ok("success");
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }





        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(string idsSearchStr) //react - update search key words to be not active (we won't search in twitter with them anymore)
        {
            try
            {
                var idsSearchArr = idsSearchStr.Split(',');
 
                Search s = new Search();
                foreach (var item in idsSearchArr)
                {
                    int idSearch = Int32.Parse(item);
                    s.updateSearch(idSearch);
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