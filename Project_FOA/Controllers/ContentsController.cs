using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Project_FOA.Models;

namespace Project_FOA.Controllers
{
    public class ContentsController : ApiController
    {

        //GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }


        //POST api/<controller>
        public HttpResponseMessage Post([FromBody] List<Content> contents)
        {
            try
            {
                Content c = new Content();
                c.InsertContent(contents);
         
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    //500
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);

                }
                else if (ex.Message == "no customer was inserted")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                    //throw (ex);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }

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