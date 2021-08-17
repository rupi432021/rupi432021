using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class ActiveAntisemiticTweeters
    {
        string tweeterName;
        int cntReports;

        public ActiveAntisemiticTweeters(string tweeterName, int cntReports)
        {
            TweeterName = tweeterName;
            CntReports = cntReports;
        }

        public string TweeterName { get => tweeterName; set => tweeterName = value; }
        public int CntReports { get => cntReports; set => cntReports = value; }

        public ActiveAntisemiticTweeters() { }

        public List<ActiveAntisemiticTweeters> getActiveAntisemiticTweeters() //get top 5 active tweeters (with the most reported antisemitic tweets) 
        {
            
            DBservices dbs = new DBservices();
            return dbs.getActiveAntisemiticTweeters();
        }
    }
}