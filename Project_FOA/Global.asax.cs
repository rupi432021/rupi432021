using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;
using System.Timers;
using Project_FOA.Models.DAL;
using System.Threading;
using System.Web.UI;
using Project_FOA.Models;
using System.Threading.Tasks;

namespace Project_FOA
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static Thread keepAliveThread = new Thread(KeepAlive);
        static Thread keepAliveThread2 = new Thread(KeepAlive2);
        
        protected void Application_Start()
        {
   
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            keepAliveThread.Start();
            keepAliveThread2.Start();

        }
        static async void MonitoringTimer()
        {

            //var allTweets = Monitoring.activate();
            Twitter tm = new Twitter();
            List<Search> allTweets = await tm.test();
            Monitoring.scb(allTweets);
            
        }


        static void LearningSystemTimer()//for manager
        {
            DBservices dbs = new DBservices();
            List<LearningSystem> LearningSystemList = dbs.getLearningSystem();
            string messageToManager;
            dbs.DeleteMessagesToManager();
            for (var i = 0; i < LearningSystemList.Count; i++)
            {
                if (LearningSystemList[i].CountTweet > 0)//change number!!!! -check count of reported as not anti tweets
                {
                    dbs.updateSearch(LearningSystemList[i].IdSearch);
                    messageToManager = "search key: " + LearningSystemList[i].SearchKey + " was removed from the system monitoring list";
                    dbs.InsertMessageToManager("hashtagDeleted", messageToManager);
                    
                }
            }
            int cntReports = dbs.getReportedToday();
            messageToManager = "Today " + cntReports + " reports of suspicious tweets were made by users";
            dbs.InsertMessageToManager("reportedToday", messageToManager);
        }

        static void LearningSystemCustomizedTimer()//for user
        {
            DBservices dbs = new DBservices();
            List<LearningSystem> LearningSystemCustomizedList = dbs.getLearningSystemCustomized();
            string messageToUser;
            for (var i = 0; i < LearningSystemCustomizedList.Count; i++)
            {
                if (LearningSystemCustomizedList[i].CountTweet > 0)//change the number!!!
                {
                    string strPost = dbs.postSearch(LearningSystemCustomizedList[i].SearchKey);
                    dbs.UpdateCustomizedSearch(LearningSystemCustomizedList[i].IdSearch);
                    if (strPost != "search key already exists")
                    {
                        messageToUser = "search key: " + LearningSystemCustomizedList[i].SearchKey + " was added to the system monitoring list";
                        dbs.InsertMessageToUser("hashtagAdded", messageToUser);
                    }
                }

            }

        }

        static void KeepAlive()
        {

            while (true)
            {
                try
                {
                    MonitoringTimer();
                    Thread.Sleep(172800000);

                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        static void KeepAlive2()
        {

            while (true)
            {
                try
                {
                    
                    LearningSystemTimer();
                    LearningSystemCustomizedTimer();
                    Thread.Sleep(120000);

                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

    }
}

  
