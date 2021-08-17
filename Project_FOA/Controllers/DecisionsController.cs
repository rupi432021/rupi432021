using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class DecisionsController : ApiController
    {

        //get Decisions for volunteer page
        public HttpResponseMessage Get()
        {
            try
            {
                Decision d = new Decision();
                List<Decision> DecisionsList =d.getDecisions();
                return Request.CreateResponse(HttpStatusCode.OK, DecisionsList);
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