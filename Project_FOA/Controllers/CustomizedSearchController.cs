using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class CustomizedSearchController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("api/CustomizedSearch/{idUser}")]
        public HttpResponseMessage Get(int idUser) //  if the word already exists - insert only to customizedSearchOfUser_2021, if not - insert also to customizedSearch_2021
        {
            try
            {
                CustomizedSearch cs = new CustomizedSearch();
                return Request.CreateResponse(HttpStatusCode.OK, cs.getCustomizedSearchOfUser(idUser));
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
        [Route("api/Search/GetCustomized/{idUser}")]
        public HttpResponseMessage GetCustomized(int idUser) //get search system key
        {
            try
            {
                CustomizedSearch cs = new CustomizedSearch();
                return Request.CreateResponse(HttpStatusCode.OK, cs.GetCustomized(idUser));
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
        [HttpGet]
        [Route("api/CustomizedSearch/GetCustomizedSearch/{word}/{idUser}/{searchOption}")] //get search key og user + add new search
        public HttpResponseMessage GetCustomizedSearch(string word, int idUser, string searchOption) //  if the word already exists - insert only to customizedSearchOfUser_2021, if not - insert also to customizedSearch_2021
        {
            try
            {
                CustomizedSearch cs = new CustomizedSearch();
                return Request.CreateResponse(HttpStatusCode.OK, cs.getCustomizedSearch(word, idUser, searchOption));
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

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody] int searchId) //put customized search - the search key was added to the system search, so it no longer needed in the customized search
        {
            try
            {
                CustomizedSearch cs = new CustomizedSearch();
                cs.UpdateCustomizedSearch(searchId);
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


    }
}