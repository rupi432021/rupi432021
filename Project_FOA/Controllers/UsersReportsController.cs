using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class UsersReportsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }     

        [HttpGet]
        [Route("api/GetUsersReports")]
        public IHttpActionResult GetUsersReports() //react - get list of users reports
        {
            try
            {
                UserReport ur = new UserReport();
                return Ok(ur.GetUsersReports());
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        [HttpGet]
        [Route("api/UsersReports/getUserDaily/{idUser}")]
        public HttpResponseMessage Get(int idUser) //  get count of reported tweets for each hashtage. 
        {
            try
            {
                UserReport ur = new UserReport();
                return Request.CreateResponse(HttpStatusCode.OK, ur.getUserDailyReport(idUser));
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