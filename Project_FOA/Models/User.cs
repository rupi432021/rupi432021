using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace Project_FOA.Models
{
    public class User
    {
        int iduser;
        string email;
        string userPassword;
        string firstName;
        string lastName;
        string phone;
        int isActive;
        int isManager;
        List<string> customizedSearchListOfUser;

        public User(int iduser, string email, string userPassword, string firstName, string lastName, string phone, int isActive, int isManager, List<string> customizedSearchListOfUser)
        {
            Iduser = iduser;
            Email = email;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            IsActive = isActive;
            IsManager = isManager;
            CustomizedSearchListOfUser = customizedSearchListOfUser;
        }

        public int Iduser { get => iduser; set => iduser = value; }
        public string UserPassword { get => userPassword; set => userPassword = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Email { get => email; set => email = value; }
        public string Phone { get => phone; set => phone = value; }
        public int IsActive { get => isActive; set => isActive = value; }
        public int IsManager { get => isManager; set => isManager = value; }

        public List<string> CustomizedSearchListOfUser { get => customizedSearchListOfUser; set => customizedSearchListOfUser = value; }

        public User() { }
        //login
        public User loginUser(string email, string password)
        {
            DBservices dbs = new DBservices();
            return dbs.loginUser(email, password);
        }

   
        public string Insert()
        {
            DBservices dbs = new DBservices();
            return dbs.InsertUser(this);
        }

        public void UpdateUserDetails(User user)
        {
            DBservices dbs = new DBservices();
            dbs.UpdateUserDetails(this);
        }

        public List<string> deleteCustomizedSearchOfUser(User user)
        {
            DBservices dbs = new DBservices();
            return dbs.deleteCustomizedSearchOfUser(user);
        }

        public List<User> GetUsers() //get users
        {
            DBservices dbs = new DBservices();
            List<User> usersList = dbs.getUsers();
            return usersList;
        }


        public string forgotPassword(string email) //forgot password
        {
            DBservices dbs = new DBservices();
            string password = dbs.forgotPassword(email);
            if (password == null)
                return "The email address is not registered. \n Please try again.";
            // Gmail Address from where you send the mail 
            var fromAddress = "foa.twitter.fp@gmail.com";
            // any address where the email will be sending       
            var toAddress = email;
            //Password of your gmail address 
            const string fromPassword = "foa12345";
            // Passing the values and make a email formate to display 
            string subject = "Password Recovery";
            string body = "From: Twiser System" + "\n";
            body += "From Email: " + fromAddress + "\n";
            body += "Subject: " + subject + "\n";
            body += "Your password is: " + password + "\n";
            // smtp settings 
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587; smtp.EnableSsl = true;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtp.Timeout = 20000;
            }

            smtp.Send(fromAddress, toAddress, subject, body);

            return "Password recovery request was sent successfuly. \n Please check your email to recover your password.";
        }


        public void updateUser(int idUser) //update user to be not active (manager remove the volunteer)
        {
            DBservices dbs = new DBservices();
            dbs.updateUser(idUser);
        }

    }
}

