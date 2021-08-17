using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class BarReportsForUser
    {
        DateTime dateReported;
        int countReported;

        public BarReportsForUser(DateTime dateReported, int countReported)
        {
            DateReported = dateReported;
            CountReported = countReported;
        }

        public DateTime DateReported { get => dateReported; set => dateReported = value; }
        public int CountReported { get => countReported; set => countReported = value; }

        public BarReportsForUser() { }
        public List<BarReportsForUser> getBarReportsForUser(int idUser) //  get count of reported user
        {
            DBservices dbs = new DBservices();
            return dbs.getBarReportsForUser(idUser);
        }
        public List<BarReportsForUser> BarOfAllUsers(int idUser) //  get count of all user reports
        {
            DBservices dbs = new DBservices();
            return dbs.BarOfAllUsers(idUser);
        }

        public List<BarReportsForUser> getBarWeeklyReports() //  get count of weekly reported tweets
        {
            DBservices dbs = new DBservices();
            return dbs.getBarWeeklyReports();
        }
    }
}