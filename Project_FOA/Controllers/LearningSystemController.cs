using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class LearningSystemController : ApiController
    {
        // GET api/<controller>
        public HttpResponseMessage Get() //  get count of reported tweets for each hashtage. 
        {
            try
            {
                LearningSystem lsc = new LearningSystem();
                return Request.CreateResponse(HttpStatusCode.OK, lsc.getLearningSystemCustomized());
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
        [Route("api/LearningSystem/GetLearningSystem")]
        public HttpResponseMessage GetLearningSystem() //  get count of reported tweets for each hashtage. 
        {
            try
            {
                LearningSystem lsc = new LearningSystem();
                return Request.CreateResponse(HttpStatusCode.OK, lsc.GetLearningSystem());
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
        [Route("api/LearningSystem/GetMeassagesManager")]
        public HttpResponseMessage GetMeassagesManager() //  get count of reported tweets for each hashtage. 
        {
            try
            {
                LearningSystem lsc = new LearningSystem();
                return Request.CreateResponse(HttpStatusCode.OK, lsc.GetMeassagesManager());
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
        [Route("api/LearningSystem/GetMeassagesUser")]
        public HttpResponseMessage GetMeassagesUser() //get list of words that removed from customer hashtags list, and was added to the system list. we found that this word is important to our search engine

        {
            try
            {
                LearningSystem lsc = new LearningSystem();
                return Request.CreateResponse(HttpStatusCode.OK, lsc.GetMeassagesUser());
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
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}