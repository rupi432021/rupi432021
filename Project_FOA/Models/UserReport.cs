using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models.DAL
{
    public class UserReport
    {
        DateTime createdAt;
        string fullname;
        string tweetText;
        string urlTweet;
        string decision;
        string reason;

        public UserReport(DateTime createdAt, string fullname, string tweetText, string urlTweet, string decision, string reason)
        {
            CreatedAt = createdAt;
            Fullname = fullname;
            TweetText = tweetText;
            UrlTweet = urlTweet;
            Decision = decision;
            Reason = reason;
        }

        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        public string Fullname { get => fullname; set => fullname = value; }
        public string TweetText { get => tweetText; set => tweetText = value; }
        public string UrlTweet { get => urlTweet; set => urlTweet = value; }
        public string Decision { get => decision; set => decision = value; }
        public string Reason { get => reason; set => reason = value; }

        public UserReport() { }

        public List<UserReport> GetUsersReports() //get users reports
        {
            DBservices dbs = new DBservices();
            List<UserReport> usersReportsList = dbs.getUsersReports();
            return usersReportsList;
        }

        public int getUserDailyReport(int idUser) //get daily reports of user
        {
            DBservices dbs = new DBservices();
            return dbs.getUserDailyReport(idUser);
        }
    }
}