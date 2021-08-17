using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Text;
using Project_FOA.Models;
using System.Web.UI;
using System.Threading.Tasks;
using WebGrease.Css.Ast.Selectors;
using Tweetinvi.Models.Entities;
using System.Net.Configuration;
using Tweetinvi.Models;

namespace Project_FOA.Models.DAL
{
    /// <summary>
    /// DBServices is a class created by me to provides some DataBase Services
    /// </summary>
    public class DBservices
    {
        public SqlDataAdapter da;
        public DataTable dt;

        public DBservices()
        {
        }

        //--------------------------------------------------------------------------------------------------
        // This method creates a connection to the database according to the connectionString name in the web.config 
        //--------------------------------------------------------------------------------------------------
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        //--------------------------------------------------------------------------------------------------
        // This method inserts content to the content table 
        //--------------------------------------------------------------------------------------------------
        public void InsertContent(List<Content> contents)
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }

            foreach (var content in contents)
            {
                Content c = getTweetById(content.IdStrTweet, con);
                if (c == null) //new
                {
                    String cStr = BuildInsertContentCommand(content);      // helper method to build the insert string
                    cmd = CreateCommand(cStr, con);             // create the command 

                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected != 1)
                            throw new Exception("could not insert all the contents");
                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);
                    }
                }
            }

            if (con != null)
            {
                con.Close();
            }
        }

        //--------------------------------------------------------------------
        // Build the Insert content command String
        //--------------------------------------------------------------------
        private String BuildInsertContentCommand(Content content)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}')", content.IdStrTweet, content.TextTweet, content.UrlTweet, content.IdStrUser);
            String prefix = "INSERT INTO TweetsContent_2021 " + "(idStrTweet, textTweet, urlTweet, idStrUser)";
            command = prefix + sb.ToString();

            return command;
        }

        //--------------------------------------------------------------------------------------------------
        // This method inserts friends of tweeter to the friendsOfTweeter_2021 table 
        //--------------------------------------------------------------------------------------------------
        public void InsertFriendsOfTweeter(List<FriendsTweeter> allTweetersFriends)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            foreach (var item in allTweetersFriends)
            {
                bool combinationExists = false;
            
                    combinationExists = getCombinationFriendWithTweeter(item.IdTweeter, item.IdFriendTweeter, con);
      
                if (combinationExists == false)  // tweeter is new - so all his connections are new and need to be inserted, or tweeter exists but the combination not exists so we need to insert
                {
                    String cStr = BuildInsertFriendsOfTweeterCommand(item);      // helper method to build the insert string
                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected != 1)
                            throw new Exception("could not insert all the combination");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            if (con != null)
            {
                con.Close();
            }
        }

        //--------------------------------------------------------------------
        // Build the Insert friends of tweeter command String
        //--------------------------------------------------------------------
        private String BuildInsertFriendsOfTweeterCommand(FriendsTweeter ft)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}', '{1}', {2})", ft.IdTweeter, ft.IdFriendTweeter, ft.SuspectedConnection);
            String prefix = "INSERT INTO friendsOfTweeters_2021 " + "(idTweeter, idFriendTweeter, suspectedConnection)";
            command = prefix + sb.ToString();

            return command;
        }

        //--------------------------------------------------------------------------------------------------
        // This method inserts tweeters array to the tweeters_train_2021 table 
        //--------------------------------------------------------------------------------------------------

        public void InsertTweetersArr(List<Tweeter> tweeters)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            foreach (var tweeter in tweeters) // foreach tweeter in the array do the insert
            {

                Tweeter chkTweeter = getTweeterById(tweeter.IdTweeter);
                if (chkTweeter == null)
                {
                    String cStr = BuildInsertTweeterCommand(tweeter);      // helper method to build the insert string
                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected != 1)
                            throw new Exception("could not insert all the tweeters");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            if (con != null)
            {
                con.Close();
            }
        }

        //--------------------------------------------------------------------
        // Build the Insert tweeter command String
        //--------------------------------------------------------------------
        private String BuildInsertTweeterCommand(Tweeter tweeter)
        {
            String command;

            StringBuilder sb = new StringBuilder();

            string tweeterLocation = $"N'{tweeter.TweeterLocation}'";

            // use a string builder to create the dynamic string
            sb.AppendFormat(" Values('{0}', '{1}', {2},{3},'{4}',{5},{6},{7},{8},{9},{10},{11})", tweeter.IdTweeter, tweeter.TweeterName, tweeter.FollowersCount, tweeter.FriendsCount, tweeter.TweeterCreatedAtAccount, tweeter.StatusesCountSinceCreated, tweeter.StatusesCountSinceChecked, tweeter.AntisemiticTweets, tweeter.AntisemiticFriends, tweeterLocation, tweeter.IsAntisemitic,tweeter.AntisemitismPercentage);
            String prefix = "INSERT INTO Tweeters_train_2021 " + "(idTweeter,tweeterName, followersCount,friendsCount,tweeterCreatedAtAccount,statusesCountSinceCreated,statusesCountSinceChecked,antisemiticTweets,antisemiticFriends,tweeterLocation,isAntisemitic,antisemitismPercentage)";
            command = prefix + sb.ToString();

            return command;
        }

        //--------------------------------------------------------------------------------------------------
        // This method inserts tweets into table tweets_train_2021
        //--------------------------------------------------------------------------------------------------
        public List<Tweet> InsertTweets(List<Tweet> allTweets)
        {
            SqlConnection con;
            SqlCommand cmd;
            List<string> tweetersNotAntiToUpdate = new List<string>();//list of ids to update in db- statusesCountSinceChecked
            List<string> tweetersAntiToUpdate = new List<string>();      //list of ids to update in db - statusesCountSinceChecked +antisemiticTweets
            List<Tweet> antisemiticTweets = new List<Tweet>();
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            DBservices dbs2 = getProbwordsDT();

            foreach (var item in allTweets)
            {
                bool validInsert = true;//check if the tweet has attributed tweet that exists in the list
                Tweet chkTweet = getChkTweetById(item.IdTweet, con);
                Tweeter tweeter = getTweeterById(item.IdTweeter);
                if (tweeter != null)
                {
                    if (chkTweet == null)  // tweet is new
                    {
                        if (item.AttributedTweetId != null)
                        {
                            validInsert = allTweets.Exists(x => x.IdTweet == item.AttributedTweetId);
                            if (!validInsert)
                                item.AttributedTweetId = "";

                        }
                        BayesAlgorithm ba = calc_NaiveBayes(item.TweetText, dbs2.dt);
                        item.AntisemitismPercentage = ba.AntisemitismPercentage;
                        item.IsAntisemitic = ba.IsAntisemitic;
                        if (ba.IsAntisemitic == 1)
                        {
                            antisemiticTweets.Add(item);
                        }


                        String cStr = BuildInsertTweetsCommand(item);      // helper method to build the insert string
                        cmd = CreateCommand(cStr, con);             // create the command 
                        try
                        {
                            int numEffected = cmd.ExecuteNonQuery(); // execute the command
                            if (numEffected != 1)
                                throw new Exception("could not insert all the tweets");
                        }
                        catch (Exception ex)
                        {
                            throw (ex);


                        }

                        if (item.AttributedTweetersNames.Count > 0) //there are attributed tweeters
                        {
                            foreach (var item2 in item.AttributedTweetersNames)
                            {
                                string idAttributedTweeter = getTweeterByName(item2, con); //get the id of the attributed tweeter by his username

                                if (idAttributedTweeter != null)
                                {
                                    cStr = BuildAttributesOfTweetsCommand(item.IdTweet, idAttributedTweeter); // insert into table attributesOfTweets_2021
                                    cmd = CreateCommand(cStr, con);        // create the command 
                                    try
                                    {
                                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                                        if (numEffected != 1)
                                            throw new Exception("could not insert all the combination");
                                    }
                                    catch (Exception ex)
                                    {
                                        throw (ex);
                                    }

                                }
                            }
                        }
                        //if (item.SearchId != -1) //we check bacause there is no hashtag exists for attributedTweetId (when searchId = -1 it means that we didn't found the tweeter by a search key)
                        //{
                        //    insertTweetBySearch(item.IdTweet, item.SearchId, con);
                        //}
                        if (ba.IsAntisemitic == 0)//create list of ids to update columns 
                        {
                            tweetersNotAntiToUpdate.Add(item.IdTweeter);
                        }
                        else
                            tweetersAntiToUpdate.Add(item.IdTweeter);
                    }
                    else //tweet is not new, we need to check if it is antisemitic
                    {
                        bool userAlreadyReported = getChkTweetReportedByUser(item, con);
                        //check if user already reported it
                        if (chkTweet.IsAntisemitic == 1 && userAlreadyReported == false) //the tweet is antisemitic and the user didn't report the tweet
                            antisemiticTweets.Add(chkTweet);
                    }
                }
            }
            if (tweetersNotAntiToUpdate.Count > 0 | tweetersAntiToUpdate.Count > 0) //if at least one of them has values
            {
                
                DBservices dbs3 = getTweetersDT();
                dbs3.dt = updateTweetersAfterScoringTweets(dbs3.dt, tweetersNotAntiToUpdate, tweetersAntiToUpdate);
                dbs3.Update();
            }

            //calculation - final score of amtisemitism
            antisemiticTweets= calc_FinalScore(antisemiticTweets, con);

            if (con != null)
            {
                con.Close();
            }

            return antisemiticTweets;
        }


        //update tweeter
        public List<Tweet> calc_FinalScore(List<Tweet> antisemiticTweets, SqlConnection con)
        {
            SqlCommand cmd;
            

            foreach (var tweet in antisemiticTweets)
            {
                Tweeter tweeter = getTweeterById(tweet.IdTweeter);

                double finalScore;

                if (tweeter.IsAntisemitic == 1)
                    finalScore = (0.8 * tweet.AntisemitismPercentage) + (0.2 * tweeter.AntisemitismPercentage);
                else
                    finalScore = 0.8 * tweet.AntisemitismPercentage;

                tweet.FinalScore = finalScore;

                String selectSTR = "update tweets_train_2021 set finalScore = " + finalScore + "where idTweet = '" + tweet.IdTweet + "'";
          
                cmd = CreateCommand(selectSTR, con);
                try
                {
                    int numEffected = cmd.ExecuteNonQuery();
                    if (numEffected != 1)
                        throw new Exception("could not update all the tweeters");
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
            
            if (con != null)
            {
                con.Close();
            }
            return antisemiticTweets;
        }







        //--------------------------------------------------------------------------------------------------
        // This method inserts tweets by search to the tweetsBySearch_2021 table 
        //--------------------------------------------------------------------------------------------------

        public void InsertTweetsBySearch(List<Tweet> allTweets)
        {
            SqlConnection con;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            foreach (var item in allTweets) // foreach tweet
            {
                Tweet chkTweet = getChkTweetById(item.IdTweet, con);
                if (chkTweet == null)  // tweet is new
                {
                    if (item.SearchId != -1) //we check bacause there is no hashtag exists for attributedTweetId (when searchId = -1 it means that we didn't found the tweeter by a search key)
                    {
                        insertTweetBySearch(item.IdTweet, item.SearchId, con);
                    }
                }
                else // tweet is not new, we want to check if we found him with a new hashtag
                {
                    bool chkHashtagExist = getCheckIfCombinationTweetSearchExists(item.IdTweet, item.SearchId, con);//check if the combination of idsearch and idtweet exists already(table-tweetsBySearch)
                    if (chkHashtagExist == false & item.SearchId != -1) //combination not exists, and it came from search(SearchId!=-1) 
                    {
                        insertTweetBySearch(item.IdTweet, item.SearchId, con);
                    }
                }
            }
            if (con != null)
            {
                con.Close();
            }
        }



        //--------------------------------------------------------------------------------------------------
        // This method inserts tweets by search to the customizedSearchOfUser_2021 table 
        //--------------------------------------------------------------------------------------------------

        public void InsertTweetsByCustomizedSearch(List<Tweet> allTweets)
        {
            SqlConnection con;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            foreach (var item in allTweets) // foreach tweet
            {
                Tweet chkTweet = getChkTweetById(item.IdTweet, con);
                if (chkTweet == null)  // tweet is new
                {
                    if (item.SearchId != -1) //we check bacause there is no hashtag exists for attributedTweetId (when searchId = -1 it means that we didn't found the tweeter by a search key)
                    {
                        insertTweetByCustomizedSearch(item.IdTweet, item.SearchId, item.UserId, con);
                    }
                }
                else // tweet is not new, we want to check if we found him with a new hashtag
                {
                    bool chkHashtagExist = getCheckIfCombinationTweetCustoimizedSearchExists(item.IdTweet, item.SearchId, item.UserId, con);//check if the combination of idsearch and idtweet exists already(table-tweetsBySearch)
                    if (chkHashtagExist == false & item.SearchId != -1) //combination not exists, and it came from search(SearchId!=-1) 
                    {
                        insertTweetByCustomizedSearch(item.IdTweet, item.SearchId, item.UserId, con);
                    }
                }
            }
            if (con != null)
            {
                con.Close();
            }
        }



        //check if the combination of idsearch and idtweet exists already(table-tweetsBySearch)
        private bool getCheckIfCombinationTweetSearchExists(string IdTweet, int SearchId, SqlConnection con)
        {
            String selectSTR = "select * from tweetsBySearch_2021 where idTweet = '" + IdTweet + "' and idSearch = " + SearchId;
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {  
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        //check if the combination of idsearch and idtweet exists already(table-tweetsByCustomizedSearch)
        private bool getCheckIfCombinationTweetCustoimizedSearchExists(string IdTweet, int SearchId, int UserId, SqlConnection con)
        {
            String selectSTR = "select * from tweetsByCustomizedSearch_2021 where idTweet = '" + IdTweet + "' and idCustomizedSearch = " + SearchId + " and idUser = " + UserId;
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        //check if the tweet was already reported by the user
        private bool getChkTweetReportedByUser(Tweet tweet, SqlConnection con)
        {
            String selectSTR = "select * from reports_2021 where idUser = " + tweet.UserId + " and idTweet = '" + tweet.IdTweet + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //insert into table tweetBySearch the combination of tweetId + searchId
        private void insertTweetBySearch(string IdTweet, int SearchId, SqlConnection con) {
            
           string cStr = BuildInsertTweetsBySearchCommand(IdTweet,SearchId);      // helper method to build the insert string         
            SqlCommand cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not insert all the combination");
            }
            catch (SqlException ex)
            {
               Error err = new Error();

               err.Text = ex.Message;

               err.insert();      
            }
        }




        //insert into table tweetByCustomizedSearch the combination of tweetId + searchId
        private void insertTweetByCustomizedSearch(string IdTweet, int SearchId, int UserId, SqlConnection con)
        {

            string cStr = BuildInsertTweetsByCustomizedSearchCommand(IdTweet, SearchId, UserId);      // helper method to build the insert string         
            SqlCommand cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not insert all the combination");
            }
            catch (SqlException ex)
            {
                Error err = new Error();

                err.Text = ex.Message;

                err.insert();
            }
        }



        //____________________________________________________
        //update the tweeters that wrote anti and not anti tweets- statusesCountSinceChecked +antisemiticTweets
        //____________________________________________________-_
        private DataTable updateTweetersAfterScoringTweets(DataTable dt, List<string> tweetersNotAntiToUpdate,List<string> tweetersAntiToUpdate)
        {
            SqlConnection con;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            ScoreTweeter st;
            foreach (DataRow dr in dt.Rows)
            {
                foreach (var item in tweetersNotAntiToUpdate) //tweeters of non-antisemitic tweets
                {
                    if (item.Equals(dr["idTweeter"]))//id tweeter in data table (tweeters-2021)
                    {
                        dr["statusesCountSinceChecked"] = Convert.ToInt32(dr["statusesCountSinceChecked"]) + 1;
                        st = scoringAntisemiticTweeter(item, Convert.ToInt32(dr["statusesCountSinceChecked"]), Convert.ToInt32(dr["antisemiticTweets"])); // calc tweeter score                  
                        dr["isAntisemitic"] = st.IsAntisemitic;
                        dr["antisemitismPercentage"] = st.AntisemitismPercentage;
                        if (st.IsAntisemitic == 1)
                            UpdateSuspectedConnection(item, con);  //suspected connection
                        break;                        
                    }
                }
                foreach (var item in tweetersAntiToUpdate)  //tweeters of antisemitic tweets
                {
                    if (item.Equals(dr["idTweeter"]))
                    {
                        dr["statusesCountSinceChecked"] = Convert.ToInt32(dr["statusesCountSinceChecked"]) + 1;
                        dr["antisemiticTweets"] = Convert.ToInt32(dr["antisemiticTweets"]) + 1;
                        st = scoringAntisemiticTweeter(item, Convert.ToInt32(dr["statusesCountSinceChecked"]), Convert.ToInt32(dr["antisemiticTweets"])); // calc tweeter score             
                        dr["isAntisemitic"] = st.IsAntisemitic;
                        dr["antisemitismPercentage"] = st.AntisemitismPercentage;
                        if (st.IsAntisemitic == 1)
                            UpdateSuspectedConnection(item, con);  //suspected connection
                        break;
                    }
                }
            }
            if (con != null)
            {
                con.Close();
            }

            return dt;
        }

        public ScoreTweeter scoringAntisemiticTweeter(string idTweeter, int statusesCountSinceChecked, int antisemiticTweets)
        {
            SqlConnection con;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            int suspectedConnections = getSuspectedConnectionsTweeter(idTweeter, con);
            int friendsCount = getFriendsCountTweeter(idTweeter, con);

            try
            {
                int avgSuspectedConnection = getAvgSuspectedConnection(con);
                int avgInfluenceFollowersCount = getAvgInfluenceFollowersCount(con);
                int avgInfluenceFriendsCount = getAvgInfluenceFriendsCount(con);
                if (avgSuspectedConnection == -1 | avgInfluenceFollowersCount == -1 | avgInfluenceFriendsCount == -1)
                    throw new Exception("could not calculate average");
                Tweeter t = getTweeterById(idTweeter);

                double ratioTweetsChk;
                if (statusesCountSinceChecked != 0)
                    ratioTweetsChk = (double)antisemiticTweets / (double)statusesCountSinceChecked;
                else
                    ratioTweetsChk = 0;

                double ratioTweets = 0;
                double ratioFriends = 0;
                double ratioSuspectedConnection = 0;
                double ratioInfluenceFollowersCount = 0;
                double ratioInfluenceFriendsCount = 0;

                if (ratioTweetsChk == 1)
                { ratioTweets = ratioTweetsChk * 0.05; // ratio antisemiticTweets / allTweets
                    if (friendsCount != 0)
                        ratioFriends = ((double)t.AntisemiticFriends / (double)friendsCount) * 0.475; // ratio antisemiticFriends / allFriends
                    else
                        ratioFriends = 0;
                  ratioSuspectedConnection = ratioCalc(suspectedConnections, avgSuspectedConnection, 0.375); //ratio suspectedConnection                                                                                                      //Influence Followers calc
                           
                  ratioInfluenceFollowersCount = ratioCalc(t.FollowersCount, avgInfluenceFollowersCount, 0.05);
                  //InfluenceFriends calc
                  ratioInfluenceFriendsCount = ratioCalc(t.FriendsCount, avgInfluenceFriendsCount, 0.05);
                }
                else
                {
                    ratioTweets = ratioTweetsChk * 0.4;
                    if (friendsCount != 0)
                        ratioFriends = ((double)t.AntisemiticFriends / (double)friendsCount) * 0.3; // ratio antisemiticFriends / allFriends
                    else ratioFriends = 0;
                    ratioSuspectedConnection = ratioCalc(suspectedConnections, avgSuspectedConnection, 0.2); //ratio suspectedConnection                                                                                                      //Influence Followers calc
                    ratioInfluenceFollowersCount = ratioCalc(t.FollowersCount, avgInfluenceFollowersCount, 0.05);
                    //InfluenceFriends calc
                    ratioInfluenceFriendsCount = ratioCalc(t.FriendsCount, avgInfluenceFriendsCount, 0.05);
                }
                //final calc - 
                double calcAntisemitismTweeterPercentage = ratioTweets + ratioFriends + ratioSuspectedConnection + ratioInfluenceFollowersCount + ratioInfluenceFriendsCount;
                if ((ratioTweets == 0 & ratioFriends == 0 & ratioSuspectedConnection == 0))//if the tweeter doesn't have any antisemitic signs
                    calcAntisemitismTweeterPercentage = 0; 
                 
                int isAntisemitic = 0;
                double antisemitismPercentage = 0;
                if (calcAntisemitismTweeterPercentage > 0)
                {
                    isAntisemitic = 1;
                    antisemitismPercentage = Math.Sqrt(calcAntisemitismTweeterPercentage);
                }
                else
                {
                    antisemitismPercentage = Math.Sqrt(-calcAntisemitismTweeterPercentage);
                }

                ScoreTweeter st = new ScoreTweeter(isAntisemitic, antisemitismPercentage);

                return st;
            }
            catch (Exception ex) {
                throw new Exception("failed to calculate average", ex);
            }

        }
        public double ratioCalc(int numerator, int denominator,double percent)
        {
            double ratio = 0;
            double midCalc = (double)numerator / (double)denominator;
            if (midCalc > 1)
            {
                ratio = ((double)(numerator - denominator) / (double)(numerator + denominator)) * percent;
            }
            else
            {
                ratio = -(midCalc * percent);
            }
            return ratio;

        }

        //--------------------------------------------------------------------
        // Build the Insert tweets command String
        //--------------------------------------------------------------------
        private String BuildInsertTweetsCommand(Tweet t)
        {
            String command;

            StringBuilder sb = new StringBuilder();

            string prefix;
            string tweetText = $"N'{t.TweetText}'";

            if (t.AttributedTweetId == null | t.AttributedTweetId == "") //the sql will insert the default value (null)
            {
                sb.AppendFormat(" Values('{0}', '{1}', {2}, {3}, {4}, {5},'{6}','{7}',{8},'{9}',{10})", t.IdTweet, t.TweetCreatedAt, tweetText, t.QuoteCount, t.ReplyCount, t.RetweetCount, t.UrlTweet, t.TweetType, t.IsAntisemitic, t.IdTweeter, t.AntisemitismPercentage);
                prefix = "INSERT INTO tweets_train_2021 " + "(idTweet, tweetCreatedAt, tweetText, quoteCount, replyCount, retweetCount, urlTweet , tweetType, isAntisemitic, idTweeter, antisemitismPercentage )";
            }
            else
            {
                sb.AppendFormat(" Values('{0}', '{1}', {2}, {3}, {4}, {5},'{6}','{7}','{8}',{9},'{10}',{11})", t.IdTweet, t.TweetCreatedAt, tweetText, t.QuoteCount, t.ReplyCount, t.RetweetCount, t.UrlTweet, t.TweetType, t.AttributedTweetId, t.IsAntisemitic, t.IdTweeter, t.AntisemitismPercentage);
                prefix = "INSERT INTO tweets_train_2021 " + "(idTweet, tweetCreatedAt, tweetText, quoteCount, replyCount, retweetCount, urlTweet , tweetType, attributedTweetId, isAntisemitic, idTweeter, antisemitismPercentage )";
            }
            command = prefix + sb.ToString();
            return command;
        }

        //--------------------------------------------------------------------
        // Build the Insert attributes of tweets command String
        //--------------------------------------------------------------------
        private String BuildAttributesOfTweetsCommand(string idTweet, string idAttributedTweeter)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values('{0}', '{1}')", idTweet, idAttributedTweeter);
            String prefix = "INSERT INTO attributesOfTweets_2021 " + "(idTweet, idTweeter)";
            command = prefix + sb.ToString();

            return command;
        }


        //--------------------------------------------------------------------
        // Build the Insert tweets by search command String
        //--------------------------------------------------------------------
        private String BuildInsertTweetsBySearchCommand(string idTweet, int idSearch)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0}, '{1}')", idSearch, idTweet);
            String prefix = "INSERT INTO tweetsBySearch_2021 " + "(idSearch , idTweet)";
            command = prefix + sb.ToString();

            return command;
        }



        //--------------------------------------------------------------------
        // Build the Insert tweets by customized search command String
        //--------------------------------------------------------------------
        private String BuildInsertTweetsByCustomizedSearchCommand(string idTweet, int idSearch, int idUser)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0}, '{1}', {2})", idSearch, idTweet, idUser);
            String prefix = "INSERT INTO tweetsByCustomizedSearch_2021 " + "(idCustomizedSearch , idTweet, idUser)";
            command = prefix + sb.ToString();

            return command;
        }

        //data reader get suspected connections of tweeter 
        public int getSuspectedConnectionsTweeter(string idTweeter, SqlConnection con)
        {
            try
            {
                String selectSTR = "select t.suspectedConnections from( select idTweeter,count(suspectedConnection) as suspectedConnections from friendsOfTweeters_2021 where suspectedConnection=1 and idTweeter='" + idTweeter + "' group by idTweeter) t";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row

                    return Convert.ToInt32(dr["suspectedConnections"]);

                }
                else
                    return 0; //tweeter doesn't exist in table friends_of_tweeter
             
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        //data reader get friends count of tweeter
        public int getFriendsCountTweeter(string idTweeter, SqlConnection con)
        {
            try
            {
                String selectSTR = "select t.countFriends from (select idTweeter,count(idFriendTweeter) as countFriends from friendsOfTweeters_2021 where idTweeter='" + idTweeter + "' group by idTweeter) t";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    return Convert.ToInt32(dr["countFriends"]);
                }
                else
                    return 0; //tweeter doesn't exist in table friends_of_tweeter

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //data reader get suspected connections of tweeter 
        public int getAvgSuspectedConnection(SqlConnection con) 
        {
            try
            {
                String selectSTR = "select avg(t.suspectedConnections) as avgSuspectedConnection from (select idTweeter,count(suspectedConnection) as suspectedConnections from friendsOfTweeters_2021 group by idTweeter) t";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    int avgSuspectedConnection = Convert.ToInt32(dr["avgSuspectedConnection"]);
                    return avgSuspectedConnection;
                }
                else
                    return -1;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        //data reader get Influence Followers Count
        public int getAvgInfluenceFollowersCount(SqlConnection con)
        {
            try
            {
                String selectSTR = "select avg(followersCount) as avgFollowersCount from Tweeters_train_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    int avgFollowersCount = Convert.ToInt32(dr["avgFollowersCount"]);
                    return avgFollowersCount;
                }
                else
                    return -1;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //data reader get Influence Friends Count
        public int getAvgInfluenceFriendsCount(SqlConnection con)
        {
            try
            {
                String selectSTR = "select avg(friendsCount) as avgFriendsCount from Tweeters_train_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    int avgFriendsCount = Convert.ToInt32(dr["avgFriendsCount"]);
                    return avgFriendsCount;
                }
                else
                    return -1;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //--------------------------------------------------------------------------------------------------
        // This method insertsnew search key to the search_2021 table 
        //--------------------------------------------------------------------------------------------------

        public string postSearch(string searchKey)
        {
            SqlConnection con;
            SqlCommand cmd;
            string strReturnValue = "";
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            int chkSearchExists = getSearchByKeyWord(searchKey, con); // check if the word exists and active
            String cStr = "";
            if (chkSearchExists == -1) // the word does not exists 
            {
                cStr = BuildInsertSearchCommand(searchKey);      // helper method to build the insert string
                cmd = CreateCommand(cStr, con);             // create the command 
                try
                {
                    int numEffected = cmd.ExecuteNonQuery(); // execute the command
                    if (numEffected != 1)
                        throw new Exception("could not insert search key");
                    strReturnValue = "Search key was added successfuly!";
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
            else if (chkSearchExists == 1)//the word exists and active
                strReturnValue = "Search key already exists!";
            else //the word exists and not active
            {
                cStr = "update search_2021 set isActive = 1 where searchKey = '" + searchKey + "'";
                cmd = CreateCommand(cStr, con);             // create the command 
                try
                {
                    int numEffected = cmd.ExecuteNonQuery(); // execute the command
                    if (numEffected != 1)
                        throw new Exception("could not update search key to be not active");
                    strReturnValue = "Search key was added successfuly!";
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
            if (con != null)
            {
                con.Close();
            }
            return strReturnValue;
        }



        //--------------------------------------------------------------------
        // Build the Insert tweeter command String
        //--------------------------------------------------------------------
        private String BuildInsertSearchCommand(string searchKey)
        {
            String command;

            StringBuilder sb = new StringBuilder();

            // use a string builder to create the dynamic string
            sb.AppendFormat(" Values('{0}')", searchKey);
            String prefix = "INSERT INTO search_2021 " + "(searchKey)";
            command = prefix + sb.ToString();

            return command;
        }


        //---------------------------------------------------------------------------------
        // Create the SqlCommand
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

            return cmd;
        }

        //data reader check if tweet exists by id 
        public Tweet getChkTweetById(string idTweet, SqlConnection con)
        {

            String selectSTR = "SELECT * FROM tweets_train_2021 where idTweet = '" + idTweet + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);

            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    Tweet t = new Tweet();

                    t.IdTweet = (string)dr["idTweet"];
                    t.TweetCreatedAt = (string)dr["tweetCreatedAt"];
                    t.TweetText = (string)dr["tweetText"];
                    t.QuoteCount = Convert.ToInt32(dr["quoteCount"]);
                    t.ReplyCount = Convert.ToInt32(dr["replyCount"]);
                    t.RetweetCount = Convert.ToInt32(dr["retweetCount"]);
                    t.UrlTweet = (string)dr["urlTweet"];
                    t.TweetType = (string)dr["tweetType"];
                    t.AttributedTweetId = (dr["attributedTweetId"] != DBNull.Value) ? (string)dr["attributedTweetId"] : "";
                    t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);
                    t.IdTweeter = (string)dr["idTweeter"];
                    t.AntisemitismPercentage = Convert.ToDouble(dr["antisemitismPercentage"]);
                    t.FinalScore = Convert.ToDouble(dr["finalScore"]);
                    return t;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //data reader check if tweeter exists by username
        public string getTweeterByName(string tweeterName, SqlConnection con)
        {
            String selectSTR = "SELECT idTweeter FROM Tweeters_train_2021 where tweeterName = '" + tweeterName + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {  
                    string idTweeter = (string)dr["idTweeter"];
                    return idTweeter;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        //data reader check if search key exists (for react)
        public int getSearchByKeyWord(string searchKey, SqlConnection con)
        {
            String selectSTR = "SELECT isActive FROM search_2021 where searchKey = '" + searchKey + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    int isActive = Convert.ToInt32(dr["isActive"]);
                    return isActive;
                }
                else
                    return -1;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        //data reader check if tweet exists in content table by id (for python)
        public Content getTweetById(string idStrTweet, SqlConnection con)
        {
            String selectSTR = "SELECT * FROM TweetsContent_2021 where idStrTweet = '" + idStrTweet + "'";
            SqlCommand cmd = new SqlCommand(selectSTR, con); 
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    Content c = new Content();

                    c.IdStrTweet = (string)dr["idStrTweet"];
                    c.TextTweet = (string)dr["textTweet"];
                    c.UrlTweet = (string)dr["urlTweet"];
                    c.IdStrUser = (string)dr["idStrUser"];
             
                    return c;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        //data reader check if combination of tweeter and a friend exists
        public bool getCombinationFriendWithTweeter(string idTweeter, string idFriendTweeter, SqlConnection con)
        {
            SqlCommand cmd;
            try
            {
                String selectSTR = "select * from friendsOfTweeters_2021 where idTweeter = '" + idTweeter + "' and idFriendTweeter = '"+ idFriendTweeter + "'" ;
                 cmd = new SqlCommand(selectSTR, con);
          
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   
                    return true; //read - combination exists!
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
        }

        //data reader check if tweeter exists
        public Tweeter getTweeterById(string idTweeter)
        {
            SqlConnection con = null;

            try
                {
                 con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
                }
           catch (Exception ex)
                {
                  throw (ex);
                }
            try
            {
                String selectSTR = "select * from Tweeters_train_2021 where idTweeter = '" + idTweeter + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   
                    Tweeter t = new Tweeter();

                    t.IdTweeter = (string)dr["idTweeter"]; 
                    t.TweeterName = (string)dr["tweeterName"];
                    t.FollowersCount = Convert.ToInt32(dr["followersCount"]);
                    t.FriendsCount = Convert.ToInt32(dr["friendsCount"]);
                    t.TweeterCreatedAtAccount = (string)dr["tweeterCreatedAtAccount"];
                    t.StatusesCountSinceCreated = Convert.ToInt32(dr["statusesCountSinceCreated"]);
                    t.StatusesCountSinceChecked = Convert.ToInt32(dr["statusesCountSinceChecked"]);
                    t.AntisemiticTweets = Convert.ToInt32(dr["antisemiticTweets"]);
                    t.AntisemiticFriends = Convert.ToInt32(dr["antisemiticFriends"]);
                    t.TweeterLocation = (string)dr["tweeterLocation"];
                    t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);
                    t.AntisemitismPercentage = Convert.ToDouble(dr["antisemitismPercentage"]);
                    return t;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally {
                if (con != null)
                    con.Close();
            }
        }


        //data reader get Learning System Customized
        public List<LearningSystem> getLearningSystemCustomized()
        {
            SqlConnection con = null;
            List<LearningSystem> LearningSystemCustomizedList = new List<LearningSystem>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select tcs.idCustomizedSearch as idSearch,c.customizedSearchKey as SearchKey,count(r.idTweet) as countTweet from customizedSearch_2021 c inner join tweetsByCustomizedSearch_2021 tcs on c.idCustomizedSearch=tcs.idCustomizedSearch inner join reports_2021 r on tcs.idTweet=r.idTweet ";
                selectSTR += "inner join decisions_2021 d on r.idDecision=d.idDecision where d.decision=1 group by tcs.idCustomizedSearch ,c.customizedSearchKey";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {
                    LearningSystem lsc = new LearningSystem();
                    lsc.IdSearch= Convert.ToInt32(dr["idSearch"]);
                    lsc.CountTweet= Convert.ToInt32(dr["countTweet"]);
                    lsc.SearchKey= (string)dr["SearchKey"];
                    
                    LearningSystemCustomizedList.Add(lsc);
                }
                return LearningSystemCustomizedList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }



        //data reader get Learning System Customized for manager
        public List<string> GetMeassagesManager()
        {
            SqlConnection con = null;
            List<string> MeassagesManagerList = new List<string>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select messageManager from messagesToManager_2021";
        
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                { 
                    string messageManager = (string)dr["messageManager"];

                    MeassagesManagerList.Add(messageManager);
                }
                return MeassagesManagerList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        //data reader get Learning System Customized for user
        public List<string> GetMeassagesUser()
        {
            SqlConnection con = null;
            List<string> MeassagesUserList = new List<string>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select messageUser from messagesToUser_2021";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {
                    string messageUser = (string)dr["messageUser"];

                    MeassagesUserList.Add(messageUser);
                }
                return MeassagesUserList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        //data reader get Learning System
        public List<LearningSystem> getLearningSystem()
        {
            SqlConnection con = null;
            List<LearningSystem> LearningSystemList = new List<LearningSystem>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select tbs.idSearch as idSearch,s.searchKey as SearchKey,count(r.idTweet) as countTweet from search_2021 s inner join tweetsBySearch_2021 tbs on s.idSearch=tbs.idSearch inner join reports_2021 r on tbs.idTweet=r.idTweet inner join decisions_2021 d ";
                selectSTR += "on r.idDecision=d.idDecision where d.decision=0 group by tbs.idSearch,s.searchKey";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {
                    LearningSystem lsc = new LearningSystem();
                    lsc.IdSearch = Convert.ToInt32(dr["idSearch"]);
                    lsc.CountTweet = Convert.ToInt32(dr["countTweet"]);
                    lsc.SearchKey = (string)dr["SearchKey"];

                    LearningSystemList.Add(lsc);
                }
                return LearningSystemList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        //data reader Users Reports
        public List<UserReport> getUsersReports()
        {
            SqlConnection con = null;
            List<UserReport> UserReportList = new List<UserReport>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select r.createdAt,u.firstName + ' ' + u.lastName as fullname,t.tweetText,t.urlTweet, case when d.decision = 1 then 'Antisemitic' else 'Not antisemitic' end as decision, d.reason from";
                selectSTR += " reports_2021 r inner join decisions_2021 d on r.idDecision = d.idDecision inner join users_2021 u on r.idUser = u.idUser";
                selectSTR += " inner join tweets_train_2021 t on r.idTweet = t.idTweet";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {
                    UserReport us = new UserReport();
                    us.CreatedAt= Convert.ToDateTime(dr["createdAt"]);
                    us.Fullname= (string)dr["fullname"];
                    us.TweetText = (string)dr["tweetText"];
                    us.UrlTweet = (string)dr["urlTweet"];
                    us.Decision = (string)dr["decision"];
                    us.Reason = (string)dr["reason"];
                    UserReportList.Add(us);
                }
                return UserReportList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        //data reader Users
        public List<User> getUsers()
        {
            SqlConnection con = null;
            List<User> UsersList = new List<User>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select * from users_2021 where isActive=1";
        
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {
                    User u = new User();

                    u.Iduser= Convert.ToInt32(dr["iduser"]);
                    u.Email= (string)dr["email"];
                    u.UserPassword= (string)dr["userPassword"];
                    u.FirstName= (string)dr["firstName"];
                    u.LastName= (string)dr["lastName"];
                    u.Phone= (string)dr["phone"];
                    u.IsActive= Convert.ToInt32(dr["isActive"]);

                    UsersList.Add(u);
                }
                return UsersList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        //data reader check if tweeter is antisemitic by id
        public Tweeter getAntisemiticTweeterById(string idTweeter)
        {
            SqlConnection con = null;
                try
                {
                    con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            try
               {
                String selectSTR = "select* from Tweeters_train_2021 where isAntisemitic = 1 and idTweeter = '" + idTweeter + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                { 
                    Tweeter t = new Tweeter();

                    t.IdTweeter = (string)dr["idTweeter"];;
                    t.TweeterName = (string)dr["tweeterName"];
                    t.FollowersCount = Convert.ToInt32(dr["followersCount"]);
                    t.FriendsCount = Convert.ToInt32(dr["friendsCount"]);
                    t.TweeterCreatedAtAccount = (string)dr["tweeterCreatedAtAccount"];
                    t.StatusesCountSinceCreated = Convert.ToInt32(dr["statusesCountSinceCreated"]);
                    t.StatusesCountSinceChecked = Convert.ToInt32(dr["statusesCountSinceChecked"]);
                    t.AntisemiticTweets = Convert.ToInt32(dr["antisemiticTweets"]);
                    t.AntisemiticFriends = Convert.ToInt32(dr["antisemiticFriends"]);
                    t.TweeterLocation = (string)dr["tweeterLocation"];
                    t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);

                    return t;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
                {
                if (con != null)
                    con.Close();
               }
        }

        //data reader all ngrams
        public List<Search> getSearch()
        {
            SqlConnection con = null;
            List<Search> searchList = new List<Search>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "SELECT * FROM search_2021 where isActive = 1";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Search s = new Search();
                    s.IdSearch = Convert.ToInt32(dr["idSearch"]);
                    s.SearchKey = (string)dr["searchKey"];
                    searchList.Add(s);
                }

                return searchList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get random 2 search key words for monitoring
        public List<Search> getSearchForMonitoring()
        {
            SqlConnection con = null;
            List<Search> searchList = new List<Search>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "SELECT TOP 2 * FROM search_2021 where isActive = 1 ORDER BY NEWID()";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Search s = new Search();
                    s.IdSearch = Convert.ToInt32(dr["idSearch"]);
                    s.SearchKey = (string)dr["searchKey"];
                    searchList.Add(s);
                }

                return searchList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        //data reader all search key for timer
        public List<int> getSearchForTimer(SqlConnection con)
        {
            
            List<int> searchForTimerList = new List<int>();

            try
            {
                String selectSTR = "SELECT idSearch FROM search_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    
                    int idSearch = Convert.ToInt32(dr["idSearch"]);
                    searchForTimerList.Add(idSearch);
                }

                return searchForTimerList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
           
        }


        //data reader all antisemitic tweets by search key
        public List<Tweet> getTweetsBySearch(int idSearch)
        {
 
            List<Tweet> tweetsBySearchList = new List<Tweet>();
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            try
            {
                String selectSTR = "select t.* from tweets_train_2021 t inner join tweetsBySearch_2021 ts on t.idTweet = ts.idTweet where t.isAntisemitic = 1 and ts.idSearch = " + idSearch;
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Tweet t = new Tweet();

                    t.IdTweet = (string)dr["idTweet"];
                    t.TweetCreatedAt = (string)dr["tweetCreatedAt"];
                    t.TweetText = (string)dr["tweetText"];
                    t.QuoteCount = Convert.ToInt32(dr["QuoteCount"]);
                    t.ReplyCount = Convert.ToInt32(dr["replyCount"]);
                    t.RetweetCount = Convert.ToInt32(dr["retweetCount"]);
                    t.UrlTweet = (string)dr["urlTweet"];
                    t.TweetType = (string)dr["tweetType"];
                    t.AttributedTweetId = (dr["attributedTweetId"] != DBNull.Value) ? (string)dr["attributedTweetId"] : "";
                    t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);
                    t.IdTweeter = (string)dr["idTweeter"];
                    t.AntisemitismPercentage = Convert.ToDouble(dr["antisemitismPercentage"]);
                    t.FinalScore = Convert.ToDouble(dr["finalScore"]);

                    tweetsBySearchList.Add(t);
                }
                return tweetsBySearchList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
              finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }




        //data reader all help key words
        public List<HelpKeyWord> getHelpKeyWords()
        {
            SqlConnection con = null;
            List<HelpKeyWord> helpKeyWordsList = new List<HelpKeyWord>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "SELECT * FROM helpKeyWords_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    HelpKeyWord h = new HelpKeyWord();
                    h.IdHelpKey = Convert.ToInt32(dr["idHelpKey"]);
                    h.KeyWord = (string)dr["keyWord"];

                    helpKeyWordsList.Add(h);
                }
                return helpKeyWordsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader all prefix words
        public List<string> getPrefixJewsWords()
        {
            SqlConnection con = null;
            List<string> prefixJewsWordsList = new List<string>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "SELECT * FROM prefixJewsWords_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    string p = (string)dr["prefix"];

                    prefixJewsWordsList.Add(p);
                }
                return prefixJewsWordsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //update tweeter
        public void UpdateTweeter(List<Tweeter> AllTweetersToUpdate)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            foreach (var tweeter in AllTweetersToUpdate)
            {
             String selectSTR = "update Tweeters_train_2021 set[followersCount] = "+ tweeter.FollowersCount;
             selectSTR += ",[friendsCount] ="+ tweeter.FriendsCount + " ,[statusesCountSinceCreated] = "+ tweeter.StatusesCountSinceCreated ;
             selectSTR += ",[antisemiticFriends]="+tweeter.AntisemiticFriends +",[tweeterLocation]= '"+ tweeter.TweeterLocation +"'";
             selectSTR += " where idTweeter = '" +tweeter.IdTweeter+"'";

             cmd = CreateCommand(selectSTR, con);
            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                    if (numEffected != 1)
                        throw new Exception("could not update all the tweeters");
                }
            catch (Exception ex)
            {
                throw (ex);
            }
            }
            if (con != null)
             {
              con.Close();
             }
        }


        //update suspected connection of tweeter in friendsOfTweeters_2021
        public void UpdateSuspectedConnection(string idTweeter, SqlConnection con)
        {
            SqlCommand cmd;

            String selectSTR = "update friendsOfTweeters_2021 set suspectedConnection = 1 where idTweeter = '" + idTweeter + "' or idFriendTweeter = '" + idTweeter + "'";
                cmd = CreateCommand(selectSTR, con);
                try
                {
                    int numEffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
          
        }




        //---------------------------------------------------------------------------------

        // Update
        //---------------------------------------------------------------------------------

        public void updateSearch(int idSearch)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String cStr = "update search_2021 set isActive = 0 where idSearch = " + idSearch;
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not update search key to be not active");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        public void UpdateCustomizedSearch(int idSearch) //update customize search key to be not active (because we added it to the system search)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String cStr = "update customizedSearch_2021 set isActive = 0 where idCustomizedSearch = " + idSearch;
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not update search key to be not active");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }




        //update user details
        public void UpdateUserDetails(User user)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String cStr = "update users_2021 set userPassword = '" + user.UserPassword + "' , firstName = '" + user.FirstName + "' , lastName = '" + user.LastName + "' , phone = '" + user.Phone + "' where idUser = " + user.Iduser;
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not update search key to be not active");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //update search key to no active
        public void updateSearchKeyToNoActive()
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            List<int>idsSearchList=getSearchForTimer(con);//get all ids search key
            List<Search> SearchActiveList = getSearch();//get all active search key
            int indexId;
            int idsSearchToBeActive;

            String cStr = "update search_2021 set isActive = 0 where isActive = 1"; //update all active to be not active
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            for (int i = 0; i < SearchActiveList.Count; i++)
            {
               indexId=idsSearchList.FindIndex(x => x == SearchActiveList[i].IdSearch);//index for search key that is active
                if(indexId== (idsSearchList.Count-1)) { indexId = 0; }
                else indexId++;
                idsSearchToBeActive = idsSearchList[indexId];
                 cStr = "update search_2021 set isActive = 1 where idSearch = " + idsSearchToBeActive;
                cmd = CreateCommand(cStr, con);             // create the command 
                try
                {
                    int numEffected = cmd.ExecuteNonQuery(); // execute the command
                    if (numEffected != 1)
                        throw new Exception("could not update search key to be not active");
                }
                catch (Exception ex)
                {
                    throw (ex);
                }

            }

                if (con != null)
                {
                    con.Close();
                }
            
        }


        //---------------------------------------------------------------------------------

        // Update user to be not active 
        //---------------------------------------------------------------------------------

        public void updateUser(int idUser)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String cStr = "update users_2021 set isActive = 0 where idUser = " + idUser;
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("could not update user to be not active");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }




        //---------------------------------------------------------------------------------

        // Data set
        //---------------------------------------------------------------------------------

        //data set to get all the information about the tweeter - from Tweeters_train_2021
        public DBservices getTweetersDT()
        {
            SqlConnection con = null;

            try
            {
                // connect
                con = connect("DBConnectionString");
                // create a dataadaptor
                da = new SqlDataAdapter("select * from Tweeters_train_2021", con);
                // automatic build the commands
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                // create a DataSet
                DataSet ds = new DataSet();
                // Fill the Dataset
                da.Fill(ds);
                // keep the table in a field
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return this;
        }

        //data set to get all the information about probability of words from NaiveBayes_2021
        public DBservices getProbwordsDT()
        {
            SqlConnection con = null;
            try
            {
                // connect
                con = connect("DBConnectionString");
                // create a dataadaptor
                da = new SqlDataAdapter("select * from NaiveBayes_2021", con); 
                // automatic build the commands
                SqlCommandBuilder builder = new SqlCommandBuilder(da);
                // create a DataSet
                DataSet ds = new DataSet();
                // Fill the Dataset
                da.Fill(ds);
                // keep the table in a field
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return this;
        }
        //--------------------------------------------------------------------------------------------------

        public BayesAlgorithm calc_NaiveBayes(String tweetsText, DataTable dt) //algorithm to calculate the anitsemitism of tweet (if it's antisemitic or not)
        {
            string word;
            double pWordWithAnti;
            double pWordWithNotAnti;
            double CalcprobAnti = 1;
            double CalcprobNotAnti = 1;
            double PAnti = 0.3784255362824592;
            double PNnotAnti = 0.6215744637175408;

            tweetsText = tweetsText.Replace("\n", " ").ToLower();
            var arrWords = tweetsText.Split(' ');
            foreach (var item in arrWords)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    word = (string)dr["word"];
                    if (item == word)
                    {
                        pWordWithAnti = Convert.ToDouble(dr["pWordWithAnti"]);
                        CalcprobAnti = CalcprobAnti * pWordWithAnti;
                        pWordWithNotAnti = Convert.ToDouble(dr["pWordWithNotAnti"]);
                        CalcprobNotAnti = CalcprobNotAnti * pWordWithNotAnti;
                        break;
                    }
                }
            }

            double CalcDenominator = (CalcprobAnti * PAnti) + (CalcprobNotAnti * PNnotAnti);

            if (CalcDenominator == 1)
                CalcDenominator = 2.7182818284590451; // ln e is 1

            CalcprobAnti = (CalcprobAnti * PAnti) / CalcDenominator;
            CalcprobNotAnti = (CalcprobNotAnti * PNnotAnti) / CalcDenominator; // calculation of bayes - not antisemitic

            if (CalcprobAnti > CalcprobNotAnti)
            {
                BayesAlgorithm baAnti = new BayesAlgorithm(1, CalcprobAnti);
                return baAnti; //text is antisemitic
            }
            else
            {
                BayesAlgorithm baNotAnti = new BayesAlgorithm(0, CalcprobNotAnti);
                return baNotAnti; //text is not anti
            }
        }



        //data reader get all tweets for the volunteer
        public List<Tweet> getTweets(int idUser)
        {
            SqlConnection con = null;
            List<Tweet> TweetsList = new List<Tweet>();
            try
            {
                con = connect("DBConnectionString");

                //String selectSTR = "select * from tweets_train_2021 where isAntisemitic = 1 and tweetType != 'retweet' and idTweet not in (select idTweet from reports_2021 where idUser = " + idUser + " or idTweet in (select cntByIdTweet.idTweet ";
                //selectSTR = " from (select b.idTweet, COUNT(r.idReport) as cntIdReport from reports_2021 r right join tweets_train_2021 b on r.idTweet = b.idTweet group by b.idTweet having COUNT(idReport) > 5) cntByIdTweet))";

                string selectSTR = "select * from tweets_train_2021 where isAntisemitic = 1 and tweetType != 'retweet' and idTweet not in (select idTweet from reports_2021 where idUser = " + idUser + " or idTweet in (select cntByIdTweet.idTweet from (select b.idTweet, COUNT(r.idReport) as cntIdReport from reports_2021 r right join tweets_train_2021 b on r.idTweet = b.idTweet group by b.idTweet having COUNT(idReport) > 5) cntByIdTweet))";
                selectSTR += " and idTweet not in (select idTweet from tweetsByCustomizedSearch_2021 where idUser != " + idUser + ")";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row
                    Tweet t = new Tweet();
                    t.IdTweet = (string)dr["idTweet"];
                    t.TweetText = (string)dr["tweetText"];
                    t.UrlTweet = (string)dr["urlTweet"];
                    t.FinalScore = Convert.ToDouble(dr["finalScore"]);
                    TweetsList.Add(t);
                }
                return TweetsList;
            } 
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //data reader get all Decisions for the volunteer page
        public List<Decision> getDecisions()
        {
            SqlConnection con = null;
            List<Decision> DecisionsList = new List<Decision>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select * from decisions_2021";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Decision d = new Decision();
                    d.IdDecision = Convert.ToInt32(dr["idDecision"]);
                    d.Reason = (string)dr["reason"];
                    d.DecisionBit = Convert.ToInt32(dr["decision"]);
                    DecisionsList.Add(d);
                }
                return DecisionsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //data reader get all antisemitic tweets by tweeter
        public List<Tweet> getAntiTweets(string idTweeter)
        {
            SqlConnection con = null;
            List<Tweet> antiTweetsList = new List<Tweet>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select * from tweets_train_2021 where isAntisemitic = 1 and idTweeter = '" + idTweeter + "'";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
              
                while (dr.Read())
                {   // Read till the end of the data into a row
                    Tweet t = new Tweet();
                    t.IdTweet = (string)dr["idTweet"];
                    t.TweetCreatedAt = (string)dr["tweetCreatedAt"];
                    t.TweetText = (string)dr["tweetText"];
                    t.QuoteCount = Convert.ToInt32(dr["QuoteCount"]);
                    t.ReplyCount = Convert.ToInt32(dr["replyCount"]);
                    t.RetweetCount = Convert.ToInt32(dr["retweetCount"]);
                    t.UrlTweet = (string)dr["urlTweet"];
                    t.TweetType = (string)dr["tweetType"];
                    t.AttributedTweetId = (dr["attributedTweetId"] != DBNull.Value) ? (string)dr["attributedTweetId"] : "";
                    t.IsAntisemitic = Convert.ToInt32(dr["isAntisemitic"]);
                    t.IdTweeter = (string)dr["idTweeter"];
                    t.AntisemitismPercentage = Convert.ToDouble(dr["antisemitismPercentage"]);
                    t.FinalScore = Convert.ToDouble(dr["finalScore"]);
                    antiTweetsList.Add(t);
                }
                return antiTweetsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        //data reader get pai-search details (search key + count of its tweets)
        public List<PieSearch> getPieSearch()
        {
            SqlConnection con = null;
            List<PieSearch> paiSearchList = new List<PieSearch>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select top 5 s.searchKey, ts.tweetsCount from (select idSearch, count(a.idTweet) as tweetsCount from tweetsBySearch_2021 a inner join tweets_train_2021 b on a.idTweet = b.idTweet";
                selectSTR += " where b.isAntisemitic = 1 group by idSearch ) ts inner join search_2021 s on ts.idSearch = s.idSearch order by ts.tweetsCount desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    PieSearch ps = new PieSearch();
                    ps.SearchKey = (string)dr["searchKey"];                 
                    ps.TweetsCount = Convert.ToInt32(dr["tweetsCount"]);

                    paiSearchList.Add(ps);
                }
                return paiSearchList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get pie chart details - top 10 tweeters with antisemitic friends
        public List<PieAntiFriend> getPieAntiFriends()
        {
            SqlConnection con = null;
            List<PieAntiFriend> pieAntiFriendsList = new List<PieAntiFriend>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select top 10 tweeterName, antisemiticFriends from Tweeters_train_2021 order by antisemiticFriends desc";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    PieAntiFriend af = new PieAntiFriend();
                    af.TweeterName = (string)dr["tweeterName"];
                    af.AntisemiticFriends = Convert.ToInt32(dr["antisemiticFriends"]);

                    pieAntiFriendsList.Add(af);
                }
                return pieAntiFriendsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get bar chart details - top 10 tweet with the highest antisemitism percentage - manager
        public List<BarAntiTweet> getBarAntiTweets()
        {
            SqlConnection con = null;
            List<BarAntiTweet> BarAntiTweetsList = new List<BarAntiTweet>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select top 10 cast(ROW_NUMBER() OVER (ORDER BY finalScore desc) as nvarchar(10)) AS numTweet, REPLACE(tweetText, '''','' ) as tweetText, finalScore from tweets_train_2021 where isAntisemitic = 1 and tweetType != 'retweet' and tweetType != 'reply' ";
              
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarAntiTweet at = new BarAntiTweet();
                    at.NumTweet = (string)dr["numTweet"];
                    at.TweetText = (string)dr["tweetText"];
                    at.AntisemitismPercentage = Convert.ToDouble(dr["finalScore"]);

                    BarAntiTweetsList.Add(at);

                }
                return BarAntiTweetsList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get bar chart details - number of reports on antisemitic sweets (annual)
        public List<BarAntiReportsAnnual> getBarAntiReportsAnnual()
        {
            SqlConnection con = null;
            List<BarAntiReportsAnnual> BarAntiReportsAnnualList = new List<BarAntiReportsAnnual>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "declare @lastyear datetime declare @now datetime set @now = getdate() set @lastyear = dateadd(Year,-1,@now) ";
                selectSTR += " select cast(MONTH(convert(date, r.createdAt)) as int) as  monthReport , count( MONTH(convert(date, r.createdAt))) as countReports";
                selectSTR += " from reports_2021 r inner join decisions_2021 d on r.idDecision = d.idDecision where d.decision = 1 and r.createdAt BETWEEN @lastyear AND @now group by cast(MONTH(convert(date, r.createdAt)) as int)";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarAntiReportsAnnual ba = new BarAntiReportsAnnual();
                    ba.MonthReport = Convert.ToInt32(dr["monthReport"]);
                    ba.CountReports = Convert.ToInt32(dr["countReports"]);

                    BarAntiReportsAnnualList.Add(ba);

                }
                return BarAntiReportsAnnualList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get top 5 active tweeters (with the most reported antisemitic tweets) 
        public List<ActiveAntisemiticTweeters> getActiveAntisemiticTweeters()
        {
            SqlConnection con = null;
            List<ActiveAntisemiticTweeters> ActiveAntisemiticTweetersList = new List<ActiveAntisemiticTweeters>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select top 5 tr.tweeterName, count(r.idReport) cntReports from reports_2021 r inner join tweets_train_2021 t on r.idTweet=t.idTweet inner join Tweeters_train_2021 tr on tr.idTweeter = t.idTweeter where r.idDecision >= 9 group by tr.tweeterName order by count(r.idReport) desc";
           
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    ActiveAntisemiticTweeters at = new ActiveAntisemiticTweeters();
                    at.TweeterName = (string)dr["tweeterName"];
                    at.CntReports = Convert.ToInt32(dr["cntReports"]);

                    ActiveAntisemiticTweetersList.Add(at);

                }
                return ActiveAntisemiticTweetersList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get number of reports today
        public int getReportedToday()
        {
            SqlConnection con = null;
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = " select t.cntReports from (select Convert(date,createdAt ) as today, count(idReport) as cntReports from reports_2021 where  Convert(date,createdAt ) = Convert(date,getdate() ) group by  Convert(date,createdAt )) t";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    int cntReports = Convert.ToInt32(dr["cntReports"]);
                    return cntReports;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //data reader get bar chart details - antisemitic tweets - types counts - manager
        public List<BarAntiTweetsByType> getBarAntiTweetsByType()
        {
            SqlConnection con = null;
            List<BarAntiTweetsByType> BarAntiTweetsByTypeList = new List<BarAntiTweetsByType>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select tweetType , COUNT(idTweet) as cntAntiTweets from tweets_train_2021 where isAntisemitic = 1 group by tweetType";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarAntiTweetsByType at = new BarAntiTweetsByType();
                    at.TweetType = (string)dr["tweetType"];
                    at.CntAntiTweets = Convert.ToInt32(dr["cntAntiTweets"]);

                    BarAntiTweetsByTypeList.Add(at);

                }
                return BarAntiTweetsByTypeList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //getBarReportsForUser - for volunteer 
        public List<BarReportsForUser> getBarReportsForUser(int iduser)
        {
            SqlConnection con = null;
            List<BarReportsForUser> BarReportsForUserList = new List<BarReportsForUser>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "declare @lastweek datetime declare @now datetime set @now = getdate() set @lastweek = dateadd(day,-7,@now)";
                selectSTR += "SELECT convert(date, createdAt) as dateReported , COUNT(convert(date, createdAt)) AS countReported FROM reports_2021 WHERE createdAt BETWEEN @lastweek AND @now and idUser="+iduser+" GROUP BY convert(date, createdAt) ORDER BY convert(date, createdAt)";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarReportsForUser bru = new BarReportsForUser();
                    bru.DateReported = Convert.ToDateTime(dr["dateReported"]);
                    bru.CountReported = Convert.ToInt32(dr["countReported"]);

                    BarReportsForUserList.Add(bru);

                }
                return BarReportsForUserList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //getBarReportsallUser - for volunteer - if he doesnt have reports in the last week.
        public List<BarReportsForUser> BarOfAllUsers(int iduser)
        {
            SqlConnection con = null;
            List<BarReportsForUser> BarReportsForUserList = new List<BarReportsForUser>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "declare @lastweek datetime declare @now datetime set @now = getdate() set @lastweek = dateadd(day,-7,@now)";
                selectSTR += "SELECT convert(date, createdAt) as dateReported , COUNT(convert(date, createdAt)) AS countReported FROM reports_2021 WHERE createdAt BETWEEN @lastweek AND @now and idUser!=" + iduser + " GROUP BY convert(date, createdAt) ORDER BY convert(date, createdAt)";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarReportsForUser bru = new BarReportsForUser();
                    bru.DateReported = Convert.ToDateTime(dr["dateReported"]);
                    bru.CountReported = Convert.ToInt32(dr["countReported"]);

                    BarReportsForUserList.Add(bru);

                }
                return BarReportsForUserList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //get Bar weekly Reported tweets - for manager
        public List<BarReportsForUser> getBarWeeklyReports()
        {
            SqlConnection con = null;
            List<BarReportsForUser> BarReportsForManagerList = new List<BarReportsForUser>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "declare @lastweek datetime declare @now datetime set @now = getdate() set @lastweek = dateadd(day,-7,@now) SELECT convert(date, createdAt) as dateReported , COUNT(convert(date, createdAt)) AS countReported ";
                selectSTR += " FROM reports_2021 WHERE createdAt BETWEEN @lastweek AND @now GROUP BY convert(date, createdAt) ORDER BY convert(date, createdAt)";

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    BarReportsForUser bru = new BarReportsForUser();
                    bru.DateReported = Convert.ToDateTime(dr["dateReported"]);
                    bru.CountReported = Convert.ToInt32(dr["countReported"]);

                    BarReportsForManagerList.Add(bru);

                }
                return BarReportsForManagerList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        //get daily repoerts of user - for home page
        public int getUserDailyReport(int idUser)
        {
            SqlConnection con = null;
          
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select t.cntReports from (select r.idUser, count(r.idReport) as cntReports from reports_2021 r inner join users_2021 u on r.idUser = u.idUser where u.idUser = " + idUser + " and convert(date, r.createdAt) = convert(date, GETDATE()) group by r.idUser ) t";
          
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    int cntReports = Convert.ToInt32(dr["cntReports"]);

                    return cntReports;
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //getBarReportsForUser - for volunteer 
        public int getMonthReported(int iduser)
        {
            SqlConnection con = null;
           
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "declare @lastmonth datetime declare @now datetime set @now = getdate() set @lastmonth = dateadd(MONTH,-1,@now)";
                selectSTR += "SELECT COUNT(convert(date, createdAt)) AS countReportedOfMonth FROM reports_2021 WHERE createdAt BETWEEN @lastmonth AND @now and idUser="+iduser;

                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                
                


                if (dr.Read())
                {   // Read till the end of the data into a row

                    return Convert.ToInt32(dr["countReportedOfMonth"]);

                }
                else
                    return 0; //tweeter doesn't exist in table friends_of_tweeter
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //data reader get all antisemitic tweeters 
        public List<Tweeter> getAntiTweeters()
        {
            SqlConnection con = null;
            List<Tweeter> antiTweetersList = new List<Tweeter>();

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file

                String selectSTR = "select * from tweeters_train_2021 where isAntisemitic = 1 and idTweeter in (select idTweeter from tweets_train_2021 where isAntisemitic = 1)";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                while (dr.Read())
                {   // Read till the end of the data into a row
                    Tweeter t = new Tweeter();

                    t.IdTweeter = (string)dr["idTweeter"]; ;
                    t.TweeterName = (string)dr["tweeterName"];
                    t.FollowersCount = Convert.ToInt32(dr["followersCount"]);
                    t.FriendsCount = Convert.ToInt32(dr["friendsCount"]);
                    t.TweeterCreatedAtAccount = (string)dr["tweeterCreatedAtAccount"];
                    t.StatusesCountSinceCreated = Convert.ToInt32(dr["statusesCountSinceCreated"]);
                    t.StatusesCountSinceChecked = Convert.ToInt32(dr["statusesCountSinceChecked"]);
                    t.AntisemiticTweets = Convert.ToInt32(dr["antisemiticTweets"]);
                    t.AntisemiticFriends = Convert.ToInt32(dr["antisemiticFriends"]);
                    t.TweeterLocation = (string)dr["tweeterLocation"];
                    t.AntisemitismPercentage = Convert.ToDouble(dr["antisemitismPercentage"]);

                    antiTweetersList.Add(t);
                }
                return antiTweetersList;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //data reader-get search list of user
        public List<string> getCustomizedSearchOfUser(int idUser)
        {
            SqlConnection con = null;
            List<string> searchListOfUser = new List<string>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select b.CustomizedSearchKey from customizedSearchOfUser_2021 a inner join customizedSearch_2021 b on a.idCustomizedSearch = b.idCustomizedSearch where a.idUser = " + idUser+" and b.isActive=1";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row
                     string customizedSearchKey = (string)dr["customizedSearchKey"];
                     searchListOfUser.Add(customizedSearchKey);
                }
                return searchListOfUser;
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }

      
        //data reader-get ExploredHashtags list of user
        public List<HashtagsToUser> getExploredHashtags(int idUser)
        {
            SqlConnection con = null;
            List<HashtagsToUser> ExploredHashtagsListOfUser = new List<HashtagsToUser>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select * from ExploredHashtagsToUser_2021 where idUser =" + idUser;
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row
                    HashtagsToUser htu = new HashtagsToUser();                 
                    htu.IdHashUser= Convert.ToInt32(dr["idHashUser"]);
                    htu.IdUser = Convert.ToInt32(dr["idUser"]);
                    htu.Hashtag= (string)dr["hashtag"];
                    ExploredHashtagsListOfUser.Add(htu);
                }
                return ExploredHashtagsListOfUser;
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }


        //login.html
        //data reader-get all user object by Email and password
        public User loginUser(string email, string password)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "Select * from users_2021 where email = '" + email + "' and isActive = 1";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                if (dr.Read())
                {   // Read till the end of the data into a row
                    User c = new User();
                    c.Iduser = Convert.ToInt32(dr["iduser"]);
                    c.Email = (string)dr["email"];
                    c.UserPassword = (string)dr["userPassword"];
                    c.FirstName = (string)dr["firstName"];
                    c.LastName = (string)dr["lastName"];
                    c.Phone = (string)dr["phone"];
                    c.IsActive = Convert.ToInt32(dr["isActive"]);
                    c.IsManager = Convert.ToInt32(dr["isManager"]);
                    c.CustomizedSearchListOfUser = getCustomizedSearchOfUser(c.Iduser);
                    return c;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }

        //login.html
        //data reader- forgot user password
        public string forgotPassword(string email)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "Select userPassword from users_2021 where email = '" + email + "' and isActive = 1";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                if (dr.Read())
                {   // Read till the end of the data into a row
                    string password = (string)dr["userPassword"];
                    return password;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }




        //data reader- get customized search by keyword
        public CustomizedSearch getCustomizedSearchByKey(string searchKey)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select * from customizedSearch_2021 where customizedSearchKey = '" + searchKey + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                if (dr.Read())
                {   // Read till the end of the data into a row      
                    CustomizedSearch cs = new CustomizedSearch();
                    cs.IdSearch = Convert.ToInt32(dr["idCustomizedSearch"]);
                    cs.SearchKey = (string)dr["customizedSearchKey"];
                    cs.IsActive = Convert.ToInt32(dr["isActive"]); 
                    return cs;
                }
                else
                    return null;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }

        


        //CustomizedSearch - if the word already exists - insert only to customizedSearchOfUser_2021, if not - insert also to customizedSearch_2021
        public string getCustomizedSearch(string word, int idUser)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }           
            try
            {
                CustomizedSearch wordExists = checkCustomizedSearchExists(word, con); // check if the word already exists
                string cStr;
                SqlCommand cmd;
                int idCustomizedSearch;
                bool combinationExists = false; // word + user
                if (wordExists == null) //word does not exist
                {
                    cStr = BuildInsertCustomizedSearchCommand(word); // insert into table customizedSearch_2021
                    cmd = CreateCommand(cStr, con);   // create the command 

                    try
                    {
                        object result = cmd.ExecuteScalar();
                        result = (result == DBNull.Value) ? null : result;
                        idCustomizedSearch = Convert.ToInt32(result);
                        if (idCustomizedSearch == 0)
                            throw new Exception("could not insert customized search into db");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }

                    cStr = BuildInsertCustomizedSearchOfUserCommand(idUser, idCustomizedSearch); // insert into table customizedSearchOfUser_2021
                    cmd = CreateCommand(cStr, con);  // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery();
                        if (numEffected != 1)
                            throw new Exception("could not insert customized search of user into db");
                        return word;
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }

                }
                else //word exists
                {
                    combinationExists = checkCombinationSearchOfUserExists(word, idUser, con);
                    idCustomizedSearch = wordExists.IdSearch;

                    if (combinationExists & wordExists.IsActive == 1)
                        return "Search key is already exists in your list!";
                    else if (wordExists.IsActive == 0)
                    {               
                        cStr = "update customizedSearch_2021 set isActive = 1 where customizedSearchKey = '" + word + "'";
                        cmd = CreateCommand(cStr, con);             // create the command 
                        try
                        {
                            int numEffected = cmd.ExecuteNonQuery();
                            if (numEffected != 1)
                                throw new Exception("could not update customized search key to be active");
                        }
                        catch (Exception ex)
                        {
                            throw (ex);
                        }

                        cStr = BuildInsertCustomizedSearchOfUserCommand(idUser, idCustomizedSearch); // insert into table customizedSearchOfUser_2021
                        cmd = CreateCommand(cStr, con);  // create the command 
                        try
                        {
                            int numEffected = cmd.ExecuteNonQuery();
                            if (numEffected != 1)
                                throw new Exception("could not insert customized search of user into db");                         
                        }
                        catch (Exception ex)
                        {
                            throw (ex);
                        }
                       
                    }
                    return word;
                }          
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }


        //data reader- check if customized search key exists
        public CustomizedSearch checkCustomizedSearchExists(string word, SqlConnection con)
        {
            String selectSTR = "select * from customizedSearch_2021 where customizedSearchKey = '" + word + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                if (dr.Read())
                {   // Read till the end of the data into a row      
                    CustomizedSearch cs = new CustomizedSearch();
                    cs.IdSearch = Convert.ToInt32(dr["idCustomizedSearch"]);
                    cs.SearchKey = (string)dr["customizedSearchKey"];
                    cs.IsActive = Convert.ToInt32(dr["isActive"]); ;
                    return cs;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }      

        }


        //data reader- check if combination of user + word exists
        public bool checkCombinationSearchOfUserExists(string word, int idUser, SqlConnection con)
        {
            String selectSTR = "select a.* from customizedSearchOfUser_2021 a inner join customizedSearch_2021 b on a.idCustomizedSearch = b.idCustomizedSearch where a.idUser = " + idUser + " and b.CustomizedSearchKey = '" + word + "'";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                if (dr.Read())
                {   // Read till the end of the data into a row      
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

        }

        //data reader- get general hashtags not to search
        public List<string> GetGeneralHashtagsNotToSearch()
        {
            SqlConnection con = null;
            List<string> generalHashtagsNotToSearch = new List<string>(); 
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select hashtag from GeneralHashtagsNotToSearch";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row      
                    string hashtag = (string)dr["hashtag"];
                    generalHashtagsNotToSearch.Add(hashtag);                 
                }
               
                return generalHashtagsNotToSearch;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }

        //data reader- get serch key 
        public List<string> GetSearchSystemKeys()
        {
            SqlConnection con = null;
            List<string> searchSystemKeys = new List<string>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select searchKey from search_2021 where isActive = 1";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row      
                    string searchKey = (string)dr["searchKey"];
                    searchSystemKeys.Add(searchKey);
                }

                return searchSystemKeys;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }


        //data reader- get serch key 
        public List<string> GetCustomized(int idUser)
        {
            SqlConnection con = null;
            List<string> customizeList = new List<string>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select c.customizedSearchKey from customizedSearch_2021 c inner join customizedSearchOfUser_2021 cu on c.idCustomizedSearch = cu.idCustomizedSearch where cu.idUser ="+idUser;

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                {   // Read till the end of the data into a row      
                    string customizedSearchKey = (string)dr["customizedSearchKey"];
                    customizeList.Add(customizedSearchKey);
                }

                return customizeList;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }

        //data reader- get list objects of Hashtags To Manager
        public List<HashtagsToManager> getHashtagsToManager()
        {
            SqlConnection con = null;
            List<HashtagsToManager> hashtagsToManagerList = new List<HashtagsToManager>();
            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String selectSTR = "select * from ExploredHashtagsToManager_2021";

            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end
                while (dr.Read())
                    
                {   // Read till the end of the data into a row      
                    HashtagsToManager htm = new HashtagsToManager();
                    htm.IdHashManager = Convert.ToInt32(dr["idHashManager"]);
                    htm.Hashtag = (string)dr["hashtag"];
                    hashtagsToManagerList.Add(htm);
                }

                return hashtagsToManagerList;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }


        //data reader get exploredHashtag by id from exploredHashtagsToManager_2021
        public HashtagsToManager getExploredHashtagById(int idHashtag, SqlConnection con)
        {

            String selectSTR = "SELECT * FROM ExploredHashtagsToManager_2021 where idHashManager = " + idHashtag;
            SqlCommand cmd = new SqlCommand(selectSTR, con);

            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                    HashtagsToManager htm = new HashtagsToManager();

                    htm.IdHashManager = Convert.ToInt32(dr["idHashManager"]);
                    htm.Hashtag = (string)dr["hashtag"];
              
                    return htm;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }




        //data reader get exploredHashtag by id from exploredHashtagsToManager_2021
        public bool chkIfWordShouldBeActive(string customizedWord, SqlConnection con)
        {

            String selectSTR = "select * from customizedSearchOfUser_2021 where idCustomizedSearch in (select idCustomizedSearch from customizedSearch_2021 where customizedSearchKey = '" + customizedWord + "')";
            SqlCommand cmd = new SqlCommand(selectSTR, con);

            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {   // Read till the end of the data into a row
                   
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        //--------------------------------------------------------------------
        // Build the Insert customized Search command String
        //--------------------------------------------------------------------
        private String BuildInsertCustomizedSearchCommand(string word)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ('{0}')",word);
            String prefix = "INSERT INTO  customizedSearch_2021 " + "(customizedSearchKey)";
            command = prefix + sb.ToString() + " select SCOPE_IDENTITY()"; ;

            return command;
        }


        //--------------------------------------------------------------------
        // Build the Insert customized Search of user command String
        //--------------------------------------------------------------------
        private String BuildInsertCustomizedSearchOfUserCommand(int idUser, int idCustomizedSearch)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ({0},{1})", idUser, idCustomizedSearch);
            String prefix = "INSERT INTO  customizedSearchOfUser_2021 " + "(idUser,idCustomizedSearch)";
            command = prefix + sb.ToString();

            return command;
        }

        //--------------------------------------------------------------------------------------------------
        // This method insert error to the error table 
        //--------------------------------------------------------------------------------------------------
        public void insertError(Error err)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            String cStr = BuildInsertErrorCommand(err);      // helper method to build the insert string
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                if (numEffected != 1)
                    throw new Exception("could not write error into db");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //--------------------------------------------------------------------
        // Build the Insert report command String
        //--------------------------------------------------------------------
        private String BuildInsertErrorCommand(Error err)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ('{0}')", err.Text);
            String prefix = "INSERT INTO  try_twitter " + "([text])";
            command = prefix + sb.ToString();

            return command;
        }



        //--------------------------------------------------------------------------------------------------
        // This method insert hashtags to the search_2021 table 
        //--------------------------------------------------------------------------------------------------
        public void PostFromExploredToSearch(string hashtagsToPost)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            var exploredHashtagsArr = hashtagsToPost.Split(',');
            foreach (var item in exploredHashtagsArr)
            {
                int idHashtag = Convert.ToInt32(item);
                HashtagsToManager htm = getExploredHashtagById(idHashtag, con);
            String cStr = BuildInsertFromExploredToSearchCommand(htm.Hashtag);      // helper method to build the insert string
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                if (numEffected != 1)
                    throw new Exception("could not insert hashtag into db");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
                deleteExploredHashtags(idHashtag); //delete from explored hashtags table, because we added it to the search table
            }
                if (con != null)
                {
                    con.Close();
                }
            
        }

        //--------------------------------------------------------------------
        // Build the Insert report command String
        //--------------------------------------------------------------------
        private String BuildInsertFromExploredToSearchCommand(string hashtag)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ('{0}')", hashtag);
            String prefix = "INSERT INTO  search_2021 " + "(searchKey)";
            command = prefix + sb.ToString();

            return command;
        }



        //-------------------------------------------------------------------
        //delete
        //-------------------------------------------------------------------

        public List<string> deleteCustomizedSearchOfUser(User user)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            try
            {
                for (int i = 0; i < user.CustomizedSearchListOfUser.Count; i++)
                {
                    String cStr = "delete from customizedSearchOfUser_2021 where idUser = " + user.Iduser + " and idCustomizedSearch in (select idCustomizedSearch from customizedSearch_2021 where customizedSearchKey = '" + user.CustomizedSearchListOfUser[i] + "')";
                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery();
                        if (numEffected != 1)
                            throw new Exception("could not delete the search key from db");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                    bool chkWordShouldBeActive = chkIfWordShouldBeActive(user.CustomizedSearchListOfUser[i], con);

                    if (!chkWordShouldBeActive) //no volunteer has this word in his customized list
                    {
                        cStr = "update customizedSearch_2021 set isActive = 0 where customizedSearchKey = '" + user.CustomizedSearchListOfUser[i] + "'";
                        cmd = CreateCommand(cStr, con);             // create the command 
                        try
                        {
                            int numEffected = cmd.ExecuteNonQuery();
                            if (numEffected != 1)
                                throw new Exception("could not update the search key to be not active in db");
                        }
                        catch (Exception ex)
                        {
                            throw (ex);
                        }
                    }

                }
                return getCustomizedSearchOfUser(user.Iduser);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public void deleteExploredHashtags(int idExploredHashtags)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            try
            {
                
                    String cStr = "delete from ExploredHashtagsToManager_2021 where idHashManager="+ idExploredHashtags;
                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery();
                        if (numEffected != 1)
                            throw new Exception("could not write error into db");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
               
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        public List<HashtagsToUser> deleteHashtagsToUser(List<HashtagsToUser> hashtagsToUser)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            try
            {
                for (int i = 0; i < hashtagsToUser.Count; i++)
               
              
                {

                    String cStr = "delete from ExploredHashtagsToUser_2021 where idHashUser =" + hashtagsToUser[i].IdHashUser + "and idUser =" +hashtagsToUser[i].IdUser; 
                     cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery();
                        if (numEffected != 1)
                            throw new Exception("could not write error into db");
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }

                }
                return getExploredHashtags(hashtagsToUser[0].IdUser);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        //-------------------------------------------------------------------
        //data set
        //-------------------------------------------------------------------





        //--------------------------------------------------------------------------------------------------
        // This method insert report to the report table 
        //--------------------------------------------------------------------------------------------------
        public int InsertReports(Report reportObj)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

             String cStr = BuildInsertReportCommand(reportObj);      // helper method to build the insert string
             cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int idReport = Convert.ToInt32(cmd.ExecuteScalar());
                updateToNonAntiAfterReport(); //after reporting, we update all the tweets that have been reported more than 3 times that they are not antisemitic (we update them to isAntisemitic = 0)
                return idReport;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //insert to message to manager 
     
        public void InsertMessageToManager(string type,string message)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
          
           string cStr = BuildInsertMessageToManagerCommand(type,message);      // helper method to build the insert string
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {           
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                if (numEffected != 1)
                    throw new Exception("no message was inserted");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //insert to message to manager 
        public void DeleteMessagesToManager()
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            String cStr = "delete from messagesToManager_2021";
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }
        //--------------------------------------------------------------------
        // Build the Insert message to manager command string 
        //--------------------------------------------------------------------
        private String BuildInsertMessageToManagerCommand(string type,String message)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ('{0}','{1}')", type, message);
            String prefix = "INSERT INTO  messagesToManager_2021 " + "(typeMessageManager,messageManager)";
            command = prefix + sb.ToString();

            return command;
        }

        //insert to message to user 

        public void InsertMessageToUser(string type, string message)
        {
            SqlConnection con;
            SqlCommand cmd;
            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }
            String cStr = "delete from messagesToUser_2021";
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int numEffected = cmd.ExecuteNonQuery();
                
            }
            catch (Exception ex)
            {
                throw (ex);
            }

             cStr = BuildInsertMessageToUserCommand(type, message);      // helper method to build the insert string
            cmd = CreateCommand(cStr, con);             // create the command 
            try
            {
                int idReport = Convert.ToInt32(cmd.ExecuteScalar());


            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //--------------------------------------------------------------------
        // Build the Insert message to users command string 
        //--------------------------------------------------------------------
        private String BuildInsertMessageToUserCommand(string type, String message)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ('{0}','{1}')", type, message);
            String prefix = "INSERT INTO  messagesToUser_2021 " + "(typeMessageUser, messageUser)";
            command = prefix + sb.ToString();

            return command;
        }


        // update all the tweets that have been reported more than 3 times that they are not antisemitic
        public void updateToNonAntiAfterReport()
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create a connection to the database using the connection String defined in the web config file
            }
            catch (Exception ex)
            {
                throw new Exception("failed to connect to the server", ex);
            }

            try
            {
                String selectSTR = "update tweets_train_2021 set isAntisemitic = 0 where idTweet in (select t.idTweet from (select idTweet, count(idReport) cnt from reports_2021 r inner join decisions_2021 d on r.idDecision = d.idDecision where d.decision = 0 group by idTweet having count(idReport) >= 3) t)";
              
                cmd = CreateCommand(selectSTR, con);

                int numEffected = cmd.ExecuteNonQuery();
   
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        // This method inserts a user to the users_2021 table     
        public string InsertUser(User user)
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }
            string cStr;
            try {
                User userExistsNotActive = getCheckUserExistsNotActive(user.Email, con);
                if (userExistsNotActive != null) //user exists but is not active so we need to change it to active
                {
                    cStr = "update users_2021 set isActive = 1 where email = '" + user.Email + "'";
                    cmd = CreateCommand(cStr, con);
                    int numEffected = cmd.ExecuteNonQuery();
                    if (numEffected != 1)
                        throw new Exception("could not update user to be active");
                    user.Iduser = userExistsNotActive.Iduser;
                    UpdateUserDetails(user);
                }
                else //user does not exist in the system at all
                {
                    cStr = BuildInsertUserCommand(user);      // helper method to build the insert string

                    cmd = CreateCommand(cStr, con);             // create the command 

                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected == 0)
                            throw new Exception("no user was inserted");                   
                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);

                    }             
                }
                return user.FirstName;
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }
           
        }
        // Build the Insert user command String
        private String BuildInsertUserCommand(User user)
        {
            String command;
            String prefix;
            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            if (user.IsManager == 0)
            {
                sb.AppendFormat("Values('{0}', '{1}', '{2}','{3}','{4}')", user.Email, user.UserPassword, user.FirstName, user.LastName, user.Phone);
                prefix = "INSERT INTO users_2021 " + "(email, userPassword, firstName, lastName,phone)";
            }
            else
            {
                sb.AppendFormat("Values('{0}', '{1}', '{2}','{3}','{4}',{5})", user.Email, user.UserPassword, user.FirstName, user.LastName, user.Phone,user.IsManager);
                prefix = "INSERT INTO users_2021 " + "(email, userPassword, firstName, lastName,phone,isManager)";
            }
           
            command = prefix + sb.ToString();

            return command;
        }



        //check if the combination of idsearch and idtweet exists already(table-tweetsBySearch)
        private User getCheckUserExistsNotActive(string Email, SqlConnection con)
        {
            String selectSTR = "select * from users_2021 where email = '" + Email + "' and isActive = 0";
            SqlCommand cmd = new SqlCommand(selectSTR, con);
            try
            {
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    User u = new User();

                    u.Iduser = Convert.ToInt32(dr["iduser"]);
                    u.Email = (string)dr["email"];
                    u.UserPassword = (string)dr["userPassword"];
                    u.FirstName = (string)dr["firstName"];
                    u.LastName = (string)dr["lastName"];
                    u.Phone = (string)dr["phone"];
                    u.IsActive = Convert.ToInt32(dr["isActive"]);

                    return u;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }




        // This method inserts hashtags to manager - table ExploredHashtagsToManager_2021 
        public void InsertHashtagsToManager(List<string> hashtagsToManager)
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }
            foreach (var item in hashtagsToManager)
            {
                bool hashtagsToManagerExists = gethashtagsToManager(item,con);

                if (!hashtagsToManagerExists)
                {
                    String cStr = BuildInsertHashtagsToManagerCommand(item);      // helper method to build the insert string

                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected == 0)
                            throw new Exception("no hashtagToManager was inserted");

                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);

                    }
                }

            }

            if (con != null)
                {
                    // close the db connection
                    con.Close();
                }          
        }
        //Build the Insert Hashtags To Manager command String
        private String BuildInsertHashtagsToManagerCommand(string hashtagToManager)
        {
            String command;
            string HashtagToManager = $"N'{hashtagToManager}'";
            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0})", HashtagToManager);
            String prefix = "INSERT INTO ExploredHashtagsToManager_2021 " + "(hashtag)";
            command = prefix + sb.ToString();

            return command;
        }


        // This method inserts hashtags to manager - table ExploredHashtagsToManager_2021 
        public List<HashtagsToUser> PostFromExploredToPersonalList(List<HashtagsToUser> hashtagsToUser) //אפרת ושני
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }
            try
            {
                foreach (var item in hashtagsToUser)
                {
                    int idCustomizedSearch;
                    String cStr = BuildInsertCustomizedSearchCommand(item.Hashtag);      // helper method to build the insert string

                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        object result = cmd.ExecuteScalar();
                        result = (result == DBNull.Value) ? null : result;
                        idCustomizedSearch = Convert.ToInt32(result);
                        if (idCustomizedSearch == 0)
                            throw new Exception("no hashtagToManager was inserted");

                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);

                    }

                    String cStr1 = BuildInsertCustomizedSearchOfUserCommand(item.IdUser, idCustomizedSearch);      // helper method to build the insert string

                    cmd = CreateCommand(cStr1, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery();
                        if (numEffected != 1)
                            throw new Exception("could not insert hashtags to CustomizedSearchOfUserCommand table");


                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);

                    }
                }
                deleteHashtagsToUser(hashtagsToUser);
                return getExploredHashtags(hashtagsToUser[0].IdUser);
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);

            }
            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }
        }


        //data reader check if hashtags To Manager Exists
        public bool gethashtagsToManager(string hashtag, SqlConnection con)
        {
            SqlCommand cmd;
            try
            {
                String selectSTR = "select * from ExploredHashtagsToManager_2021 where hashtag = '"+hashtag+"'";
                cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    return true; //read -  exists!
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
        }



        //---------------//
        //user//
        //-----------------//
        // This method inserts hashtags to manager - table ExploredHashtagsToManager_2021 
        public void InsertHashtagsToUser(HashtagsToUser hashtagsToUser)
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw new Exception("failed to connect to the server", ex);
            }
            foreach (var item in hashtagsToUser.Hashtags)
            {
                bool hashtagsToUserExists = gethashtagsToUser(item, hashtagsToUser.IdUser, con);

                if (!hashtagsToUserExists)
                {
                    String cStr = BuildInsertHashtagsToUserCommand(item, hashtagsToUser.IdUser);      // helper method to build the insert string

                    cmd = CreateCommand(cStr, con);             // create the command 
                    try
                    {
                        int numEffected = cmd.ExecuteNonQuery(); // execute the command
                        if (numEffected == 0)
                            throw new Exception("no hashtagToUser was inserted");

                    }
                    catch (Exception ex)
                    {
                        // write to log
                        throw (ex);

                    }
                }

            }

            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
        //Build the Insert Hashtags To User command String
        private String BuildInsertHashtagsToUserCommand(string hashtagToUser,int idUser)
        {
            String command;
            string HashtagToUser = $"N'{hashtagToUser}'";
            StringBuilder sb = new StringBuilder();
            // use a string builder to create the dynamic string
            sb.AppendFormat("Values({0},{1})", idUser, HashtagToUser);
            String prefix = "INSERT INTO ExploredHashtagsToUser_2021 " + "(idUser,hashtag)";
            command = prefix + sb.ToString();

            return command;
        }


        //data reader check if hashtags To User Exists
        public bool gethashtagsToUser(string hashtag,int idUser, SqlConnection con)
        {
            SqlCommand cmd;
            try
            {
                String selectSTR = "select * from ExploredHashtagsToUser_2021 where idUser ="+idUser+"and hashtag = '"+hashtag+"'";
                cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); // CommandBehavior.CloseConnection: the connection will be closed after reading has reached the end

                if (dr.Read())
                {
                    return true; //read -  exists!
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }
        }


        //--------------------------------------------------------------------
        // Build the Insert report command String
        //--------------------------------------------------------------------
        private String BuildInsertReportCommand(Report reportObj)
        {
            String command;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" Values ({0}, '{1}', {2})", reportObj.Iduser, reportObj.Idtweet, reportObj.Idecision);
            String prefix = "INSERT INTO  reports_2021 " + "(idUser, idTweet, idDecision)";
            command = prefix + sb.ToString() + " select SCOPE_IDENTITY()";

            return command;
        }

        //-------------------------------------------------------------------
        //data set
        //-------------------------------------------------------------------


        public void Update()
        {
            da.Update(dt);
        }
    }
}


