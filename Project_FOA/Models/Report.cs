using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class Report
    {
        int idReoprt;
        DateTime cratedAt;   
        string idtweet;
        int iduser;
        int idecision; //id of a reason

        public Report(int idReoprt, DateTime cratedAt, string idtweet, int iduser, int idecision)
        {
            IdReoprt = idReoprt;
            CratedAt = cratedAt;
            Idtweet = idtweet;
            Iduser = iduser;
            Idecision = idecision;
        }

        public int IdReoprt { get => idReoprt; set => idReoprt = value; }
        public DateTime CratedAt { get => cratedAt; set => cratedAt = value; }
        public string Idtweet { get => idtweet; set => idtweet = value; }
        public int Iduser { get => iduser; set => iduser = value; }
        public int Idecision { get => idecision; set => idecision = value; }

        public Report() { }

        public int InsertReports(Report reportObj)
        {
            DBservices dbs = new DBservices();
            return dbs.InsertReports(reportObj);
        }
        public int getMonthReported(int idUser) //  get count of reported tweets for each hashtage.
        {
            DBservices dbs = new DBservices();
            return dbs.getMonthReported(idUser);
        }

     
    }
}