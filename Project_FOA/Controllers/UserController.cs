using Project_FOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Project_FOA.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet]
        [Route("api/User/{email}/{password}")]
        public HttpResponseMessage LoginUser(string email, string password)
        {
            try
            {
                User u = new User();
                return Request.CreateResponse(HttpStatusCode.OK, u.loginUser(email, password));
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
        [Route("api/GetUsers")]
        public IHttpActionResult GetUsers() //react - get list of users
        {
            try
            {
                User u = new User();
                return Ok(u.GetUsers());
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/User/forgotPassword/{email}/")] //forgot password
        public HttpResponseMessage forgotPassword(string email)
        {
            try
            {
                User u = new User();               
                return Request.CreateResponse(HttpStatusCode.OK, u.forgotPassword(email));
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
        public HttpResponseMessage Post([FromBody] User user)
        {
            try
            {
                string Cname = user.Insert();
                return Request.CreateResponse(HttpStatusCode.OK, Cname);
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
        public HttpResponseMessage Put([FromBody] User user)
        {
            try
            {
                user.UpdateUserDetails(user);
                return Request.CreateResponse(HttpStatusCode.OK, "success");
            }
            catch (Exception ex)
            {
                if (ex.Message == "failed to connect to the server")
                {
                    //500
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);

                }
                else if (ex.Message == "no user was updated")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                    //throw (ex);
                }
                else return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }

        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/PutUserNotActive/{idsUsersStr}")]
        public IHttpActionResult PutUserNotActive(string idsUsersStr) //react - update user to be not active 
        {
            try
            {
                var idsUsersArr = idsUsersStr.Split(',');

                User u = new User();
                foreach (var item in idsUsersArr)
                {
                    int idUser = Int32.Parse(item);
                    u.updateUser(idUser);
                }

                return Ok("success");
            }
            catch (Exception e)
            {
                //return BadRequest(e.Message);
                return Content(HttpStatusCode.BadRequest, e);
            }
        }



        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(User user ) // search keys of user to delete
        {
            try
            {
                User u = new User();
                return Request.CreateResponse(HttpStatusCode.OK, u.deleteCustomizedSearchOfUser(user));
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