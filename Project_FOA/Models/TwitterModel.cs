using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters.Enum;
using Tweetinvi.Parameters.V2;

namespace bennybennytwitter.Models
{
    public class TwitterModel
    {

        /*--------------------------Consumer Keys-----------------------------*/
        /*--------------------------------------------------------------------*/

        /*-------------------------API key & secret---------------------------*/
        string _API_key = "4abw2WrnZai51xVqd4SmGH0vB";
        string _API_key_secret = "S3QGjfSVV1U06ccwTmEp8qL1pnfr0eqhcrVi59mZOEvaZCBgm1";

        /*-------------------------Authentication Tokens----------------------*/
        /*--------------------------------------------------------------------*/


        /*-------------------------Bearer token-------------------------------*/
        string bearer_token = "AAAAAAAAAAAAAAAAAAAAALMXKAEAAAAAdpdIqJgLf8tO0ss5lWUvOXZa%2BS4%3DHZoGPkRYz3lcxirURCYHK4ebsY3ER9YRGxTDq0oTsUkZh371G7";


        /*--------------------Access token & secret--------------------------*/
        string access_token = "1330919520593195009-YCOWydZCH3ZmW7DcZbBBQdqJLlAGXb";
        string access_token_secret = "oAHMxVohrClhMMUnmyQBhHMPNycl7rL5daiQ4FeZG8xay";



        public async Task<object> test()
        {
            var tc = new TwitterClient(_API_key, _API_key_secret, access_token, access_token_secret);
            var userParameters = new GetUserByNameV2Parameters("netanyahu" )
            {
                Expansions = { UserResponseFields.Expansions.PinnedTweetId },
                TweetFields = { UserResponseFields.Tweet.Attachments, UserResponseFields.Tweet.Entities },
                UserFields = UserResponseFields.User.ALL
            };
            var userResponse = tc.UsersV2.GetUserByNameAsync(userParameters);
            var user = userResponse;
            return user;

        }
    }
}