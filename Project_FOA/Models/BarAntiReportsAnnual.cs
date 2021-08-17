using Project_FOA.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_FOA.Models
{
    public class BarAntiReportsAnnual
    {
        int monthReport;
        int countReports;

        public BarAntiReportsAnnual(int monthReport, int countReports)
        {
            MonthReport = monthReport;
            CountReports = countReports;
        }

        public int MonthReport { get => monthReport; set => monthReport = value; }
        public int CountReports { get => countReports; set => countReports = value; }

        public BarAntiReportsAnnual() { }

        public List<BarAntiReportsAnnual> getBarAntiReportsAnnual() //  get count of reported tweets for each hashtage.
        {
            DBservices dbs = new DBservices();
            return dbs.getBarAntiReportsAnnual();
        }
    }
}