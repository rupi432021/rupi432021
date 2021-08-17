using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class BarReportsForUserController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("api/BarReportsForUser/{idUser}")]
        public HttpResponseMessage Get(int idUser) //  get count of reported tweets for each hashtage. 
        {
            try
            {
                BarReportsForUser brf = new BarReportsForUser();
                return Request.CreateResponse(HttpStatusCode.OK, brf.getBarReportsForUser(idUser));
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
        [Route("api/BarReportsForUser/BarOfAllUsers/{idUser}")]
        public HttpResponseMessage BarOfAllUsers(int idUser) //  get count of reported tweets for each hashtage. 
        {
            try
            {
                BarReportsForUser brf = new BarReportsForUser();
                return Request.CreateResponse(HttpStatusCode.OK, brf.BarOfAllUsers(idUser));
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



        public HttpResponseMessage Get() //  get count of weekly reported tweets 
        {
            try
            {
                BarReportsForUser brf = new BarReportsForUser();
                return Request.CreateResponse(HttpStatusCode.OK, brf.getBarWeeklyReports());
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