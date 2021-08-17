using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class ReportsController : ApiController
    {
        [HttpGet]
        [Route("api/Reports/{idUser}")]
        public HttpResponseMessage Get(int idUser) //  get count of reported tweets for each hashtage. 
        {
            try
            {
                Report r = new Report();
                return Request.CreateResponse(HttpStatusCode.OK, r.getMonthReported(idUser));
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
        public HttpResponseMessage Post([FromBody] Report reportObj)
        {
            try
            {
                Report r = new Report();         
                return Request.CreateResponse(HttpStatusCode.OK, r.InsertReports(reportObj));
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    //500
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
                else if (ex.Message == "no report was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                    //throw (ex);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
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