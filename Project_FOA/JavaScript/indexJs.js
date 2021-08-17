function activate() {
    ajaxCall('GET', '../api/Twitter', '', scb, ecb);

    $("#btnGo").prop("disabled", true);
}

$(document).ajaxStop(function () { //when all the ajax call requests ended - do the post of tweets
    postTweets();
});
var contents = [];
var hashtagsToExplore = [];
var attributeedTweetersInTweets = [];
var Alltweets = [];
var AlltweetersFromApi = [];
var AlltweetsFromApi = [];
var TweetersToExplore = [];
var TweetsToExplore = [];
var AntisemiticTweetsForHashtags = []; // antisemitic tweets we posted, for hashtags to explore
var GeneralHashtagsNotToSearch = []; 
var SearchSystemKeys = [];
var CustomizedSearch=[]
var counterCustomizedIndication = 1;
var counter2CustomizedIndication = 1;
var isWholeProcessTweeters = true; //if we need to do the whole process (find all the info about the tweeters or/and attributes + post), else (we need to do half of the process) - we already have the info, we just need to do post
const regex = /["'“$%&!↵↵()*+,-./:;<=>?^_`[\]{|}~→]/g;
var checkNeedToExplore = false;
var fromProcess = "fromSearch";
var fullOrCustomized = "";
CounterCustomizedSearch = 0;

// data = all tweets list including the search word
function scb(data) {
    console.log(data);
    for (var i = 0; i < data.length; i++) {
        if (data[i].TweetsArray.length > 0) {
            checkNeedToExplore = true;
            data[i].TweetsArray = data[i].TweetsArray.slice(0, 10); //the number in the slice can be changed
            tweetsFullInfo(data[i]);
        }
    }    
    nextToExplore("full");
}
function tweetsFullInfo(tweetsBySearchObj) {  //tweets found by search or by exploring potential antisemitic tweeter   
    if (fullOrCustomized == "customized" && counterCustomizedIndication == 1)
        indicationUser("Found tweets..");
    for (var j = 0; j < tweetsBySearchObj.TweetsArray.length; j++) {
        let idTweet = tweetsBySearchObj.TweetsArray[j].IdStr;
        let tweetCreatedAt = tweetsBySearchObj.TweetsArray[j].CreatedAt;
        let quoteCount = tweetsBySearchObj.TweetsArray[j].QuoteCount;
        let replyCount = tweetsBySearchObj.TweetsArray[j].ReplyCount;
        let retweetCount = tweetsBySearchObj.TweetsArray[j].RetweetCount;
        let urlTweet = tweetsBySearchObj.TweetsArray[j].Url;
        tweetsBySearchObj.TweetsArray[j].Text = tweetsBySearchObj.TweetsArray[j].Text.replace(regex, "''");
        let tweetText = tweetsBySearchObj.TweetsArray[j].Text;
        let tweetType;
        let attributedTweetId = null; //attributed tweet id 
        if (tweetsBySearchObj.TweetsArray[j].InReplyToStatusIdStr !== null) {
            tweetType = "reply";
            attributedTweetId = tweetsBySearchObj.TweetsArray[j].InReplyToStatusIdStr;
        } else if (tweetsBySearchObj.TweetsArray[j].IsRetweet) {
            tweetType = "retweet";
            attributedTweetId = tweetsBySearchObj.TweetsArray[j].RetweetedTweet.IdStr;
        } else if (tweetsBySearchObj.TweetsArray[j].QuotedStatusIdStr !== null) {
            tweetType = "quote";
            attributedTweetId = tweetsBySearchObj.TweetsArray[j].QuotedStatusIdStr;
        } else {
            tweetType = "ordinary";
        }
        if (attributedTweetId != null)
            TweetsToExplore.push(attributedTweetId);

        let isAntisemitic = 0;
        var attributedTweetersNames = [];

        for (var k = 0; k < tweetsBySearchObj.TweetsArray[j].UserMentions.length; k++) {
            attributedTweetersNames.push(tweetsBySearchObj.TweetsArray[j].UserMentions[k].screen_name); // user mentions- to remember to insert later in table attributesOfTweets_2021
            TweetersToExplore.push(tweetsBySearchObj.TweetsArray[j].UserMentions[k].screen_name); // user mentions - to explore in API
        }
        if (fromProcess == "fromSearch") {
            TweetersToExplore.push(tweetsBySearchObj.TweetsArray[j].CreatedBy.ScreenName) //tweeter of tweet - to explore in API
            searchId = tweetsBySearchObj.IdSearch;
        }
        else {
            searchId = -1;
        }
        let HashtagsArr;
        if (fromProcess == "fromSearch") {
            if (tweetType == "retweet")//if it's a retweet,then the hashtags in the retweetedTweet .hastags to explore - without the search word
                HashtagsArr = tweetsBySearchObj.TweetsArray[j].RetweetedTweet.Hashtags.filter(item => ("#" + item.text).toLowerCase() !== tweetsBySearchObj.SearchKey.toLowerCase());
            else//hastags to explore - without the search word
                HashtagsArr = tweetsBySearchObj.TweetsArray[j].Hashtags.filter(item => ("#" + item.text).toLowerCase() != tweetsBySearchObj.SearchKey.toLowerCase());
        }
        else
            HashtagsArr = tweetsBySearchObj.TweetsArray[j].Hashtags;


        idTweeter = tweetsBySearchObj.TweetsArray[j].CreatedBy.IdStr;
        if (HashtagsArr.length > 0) {
            for (var i = 0; i < HashtagsArr.length; i++) {
                HashtagsArr[i] = HashtagsArr[i].text;
            }
            let obj = {
                Hashtags: HashtagsArr,
                IdTweet: idTweet,
                IdTweeter: idTweeter
            }

            hashtagsToExplore.push(obj);
        }
        let tweet = {
            IdTweet: idTweet,
            TweetCreatedAt: tweetCreatedAt,
            TweetText: tweetText,
            QuoteCount: quoteCount,
            ReplyCount: replyCount,
            RetweetCount: retweetCount,
            UrlTweet: urlTweet,
            TweetType: tweetType,
            AttributedTweetId: attributedTweetId,
            IsAntisemitic: isAntisemitic,
            IdTweeter: idTweeter,
            AntisemitismPercentage: 0,
            FinalScore: 0,
            AttributedTweetersNames: attributedTweetersNames, //we will insert it into table attributesOfTweets_2021 (idTweet, idTweeter) --> in order to insert it, we need to insert the tweeter first!
            SearchId: searchId, // we will insert it into table tweetsBySearch_2021 (idTweet, searchId)
            UserId: 0
        }
        Alltweets.push(tweet);
    }
}
function nextToExplore(typeOfSearch) {
    fullOrCustomized = typeOfSearch;
    TweetersToExplore = [...new Set(TweetersToExplore)]; // remove duplicates
    TweetsToExplore = [...new Set(TweetsToExplore)]; // remove duplicates
    console.log(TweetersToExplore);
    console.log(TweetsToExplore);
    if (checkNeedToExplore)
        callTweetersToExplore();
    else if (fromProcess == "fromSearch") {
        {
            $("button#btnSearchKeywords").attr("disabled", false);
            swal("No tweets were found", "You should try another search key..", "info");     
            inputValue = true; 
            $("#loadingTwitter").hide();
            indicationUser("");
        }

    }
}



//tweeters to explore (the whole process - get info + post)
function callTweetersToExplore() {
    AlltweetersFromApi = [];
    isWholeProcessTweeters = true;
    var tweetersStr = "";
    TweetersToExplore = TweetersToExplore.filter(tweeterName => ((tweeterName != "YouTube") && (tweeterName != "CNN")));
    for (var i = 0; i < TweetersToExplore.length; i++) {
        tweetersStr += TweetersToExplore[i] + ","; //user mentions of tweet - to explore in API
        if ((i % 10 == 0 || i == TweetersToExplore.length - 1) && (i != 0 || TweetersToExplore.length == 1)) {
            tweetersStr = tweetersStr.slice(0, -1);
            let api = '../api/Twitter/getTweetersByName/' + tweetersStr;
            ajaxCall('GET', api, "", getSuccessTweeters, getError);
            tweetersStr = "";
        }
    }
}

//success of tweeters to explore
function getSuccessTweeters(data) {

    AlltweetersFromApi = AlltweetersFromApi.concat(data);

    chkIfFinished = chkFinishExplore(TweetersToExplore, AlltweetersFromApi) //we need to check if we finished to find all the tweeters we needed to explore
    if (chkIfFinished) // finished to find all tweeters
    {
        TweetersToExplore = [];
        createTweeters();
    }
}


function chkFinishExplore(arrNeededToExplore, arrFromApi) {
    let x = 0;
    //calculate the number of times we needed to explore all the tweeters/tweets (depends on the arr parameter)- in order to know when we found all of them
    if (arrNeededToExplore.length % 10 == 0)
        x = Math.floor(arrNeededToExplore.length / 10);
    else {
        if ((arrNeededToExplore.length % 10 == 1) && (arrNeededToExplore.length != 1))
            x = Math.floor((arrNeededToExplore.length - 1) / 10);
        else
            x = Math.floor(arrNeededToExplore.length / 10) + 1;
    }

    if (arrFromApi.length == x)
        return true;
    else
        return false;
}

function getError(err) {
    console.log(err);
}


//do the insert of new tweeters + update of existing tweeters + insert connections of friends
function createTweeters() {
    console.log("tweeters explored that need to be inserted/updated:");
    console.log(AlltweetersFromApi);

    if (fullOrCustomized == "customized" && counterCustomizedIndication == 1)
        indicationUser("Found tweeters..");

    var allNewTweeters = [];
    var allTweetersToUpdate = [];

    for (var i = 0; i < AlltweetersFromApi.length; i++) {
        allNewTweeters = allNewTweeters.concat(AlltweetersFromApi[i].AllNewTweeters);
        allTweetersToUpdate = allTweetersToUpdate.concat(AlltweetersFromApi[i].AllTweetersToUpdate);
    }


    allNewTweeters = [...new Set(allNewTweeters)]; // remove duplicates
    allTweetersToUpdate = [...new Set(allTweetersToUpdate)]; // remove duplicates
    const regex = /["']/g;

    for (var i = 0; i < allNewTweeters.length; i++) {
        if (allNewTweeters[i].TweeterLocation!=null)
        allNewTweeters[i].TweeterLocation = allNewTweeters[i].TweeterLocation.replace(regex, "''");
    }
    for (var i = 0; i < allTweetersToUpdate.length; i++) {
        if (allTweetersToUpdate[i].TweeterLocation!=null)
        allTweetersToUpdate[i].TweeterLocation = allTweetersToUpdate[i].TweeterLocation.replace(regex, "''");
    }

    console.log("start ajax of tweeters - new + update");

    if (allNewTweeters.length > 0) { //if we have new tweeters then we post them, then we update friends
        ajaxCall('POST', "../api/Tweeters", JSON.stringify(allNewTweeters), postSuccessAndFriends, ecb);
        if (allTweetersToUpdate.length > 0)// and if we also have existing tweeters we update them
            ajaxCall('PUT', "../api/Tweeters", JSON.stringify(allTweetersToUpdate), postSuccess, ecb);
    }
    else //if we don't have new tweeters, then we only do update to existing tweeters and then update friends
        ajaxCall('PUT', "../api/Tweeters", JSON.stringify(allTweetersToUpdate), postSuccessAndFriends, ecb);
}

//success of insert tweeters or success of update tweeters (if there are no new tweeters)
function postSuccessAndFriends(data) {
    console.log("success");
    var allTweetersFriend = [];
    var allTweetsByTweeter = []
    console.log("start ajax of tweeters - friends");

    for (var i = 0; i < AlltweetersFromApi.length; i++) {
        allTweetersFriend = allTweetersFriend.concat(AlltweetersFromApi[i].AllTweetersFriend);
        allTweetsByTweeter = allTweetsByTweeter.concat(AlltweetersFromApi[i].TweetsArray);

    }

    allTweetersFriend = [...new Set(allTweetersFriend)]; // remove duplicates

    if (allTweetersFriend.length > 0) {
        ajaxCall('POST', "../api/FriendsTweeters", JSON.stringify(allTweetersFriend), postSuccess, ecb);
    }
    allTweetsByTweeter = [...new Set(allTweetsByTweeter)]; // remove duplicates
    let obj = {
        TweetsArray: allTweetsByTweeter
    }
    fromProcess = "fromTweeters";

    checkNeedToExplore = false;
    tweetsFullInfo(obj);
    counterCustomizedIndication++;
    nextToExplore(fullOrCustomized);
  
    if (fullOrCustomized == "customized" && counterCustomizedIndication == 1)
        indicationUser("Collecting more data..");

    if (isWholeProcessTweeters) //if not - we just wanted to post the tweeters. we don't need to post tweets.
        callTweets(); // when we finish posting all tweeters, we do ajax call of tweets
}

//ajax call of tweets- get all the information about tweets we needed to explore + the information about their tweeters
function callTweets() {
    var tweetsToExplore = "";
    AlltweetsFromApi = [];
    console.log("start ajax - get tweets");
    TweetsToExplore = [...new Set(TweetsToExplore)]; // remove duplicates
    CopyTweetsToExplore = TweetsToExplore.slice();
    for (var i = 0; i < TweetsToExplore.length; i++) {
        let found = Alltweets.find(item => item.IdTweet == TweetsToExplore[i]);
        if (found != undefined)
            CopyTweetsToExplore = CopyTweetsToExplore.filter(tweet => tweet != found.IdTweet);
    }
    TweetsToExplore = CopyTweetsToExplore.slice();
    if (TweetsToExplore.length > 0) { //there are tweets to explore
        for (var i = 0; i < TweetsToExplore.length; i++) {
            tweetsToExplore += TweetsToExplore[i] + ","; //tweets ids to explore in API
            if ((i % 10 == 0 || i == TweetsToExplore.length - 1) && (i != 0 || TweetsToExplore.length == 1)) {
                tweetsToExplore = tweetsToExplore.slice(0, -1);
                let api = '../api/Twitter/getTweetsByIds/' + tweetsToExplore;
                ajaxCall('GET', api, "", getSuccessTweetsByIds, ecb);
                tweetsToExplore = "";
            }
        }
    }
}

// success of explored tweets
function getSuccessTweetsByIds(data) {

    AlltweetsFromApi = AlltweetsFromApi.concat(data);
    chkIfFinished = chkFinishExplore(TweetsToExplore, AlltweetsFromApi) //we need to check if we finished to find all the tweets we needed to explore
    if (chkIfFinished) // finished to find all tweets
        createTweets();
}

//create objects of tweets and check the chain
function createTweets() {

    console.log("tweets explored:");
    console.log(AlltweetsFromApi);

    TweetsToExplore = [];
    AlltweetersFromApi = [];

    for (var i = 0; i < AlltweetsFromApi.length; i++) {
        for (var j = 0; j < AlltweetsFromApi[i].ExpendedTweets.length; j++) {
            if (AlltweetsFromApi[i].ExpendedTweets[j].Hashtags.length > 0) { // the tweet has hashtags
                let obj = {
                    Hashtags: AlltweetsFromApi[i].ExpendedTweets[j].Hashtags,
                    IdTweet: AlltweetsFromApi[i].ExpendedTweets[j].IdTweet,
                    IdTweeter: AlltweetsFromApi[i].ExpendedTweets[j].IdTweeter
                }
                hashtagsToExplore.push(obj);
            }
            if (AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetersNames.length > 0) // the tweet has attributed tweeters
                TweetersToExplore = TweetersToExplore.concat(AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetersNames);
            if (AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetId != null) // the tweet has attributed tweet
                TweetsToExplore.push(AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetId);

            AlltweetsFromApi[i].ExpendedTweets[j].TweetText = AlltweetsFromApi[i].ExpendedTweets[j].TweetText.replace(regex, "''");

            let tweet = {
                IdTweet: AlltweetsFromApi[i].ExpendedTweets[j].IdTweet,
                TweetCreatedAt: AlltweetsFromApi[i].ExpendedTweets[j].TweetCreatedAt,
                TweetText: AlltweetsFromApi[i].ExpendedTweets[j].TweetText,
                QuoteCount: AlltweetsFromApi[i].ExpendedTweets[j].QuoteCount,
                ReplyCount: AlltweetsFromApi[i].ExpendedTweets[j].ReplyCount,
                RetweetCount: AlltweetsFromApi[i].ExpendedTweets[j].RetweetCount,
                UrlTweet: AlltweetsFromApi[i].ExpendedTweets[j].UrlTweet,
                TweetType: AlltweetsFromApi[i].ExpendedTweets[j].TweetType,
                AttributedTweetId: AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetId,
                IsAntisemitic: AlltweetsFromApi[i].ExpendedTweets[j].IsAntisemitic,
                IdTweeter: AlltweetsFromApi[i].ExpendedTweets[j].IdTweeter,
                AntisemitismPercentage: AlltweetsFromApi[i].ExpendedTweets[j].AntisemitismPercentage,
                FinalScore: AlltweetsFromApi[i].ExpendedTweets[j].FinalScore,
                AttributedTweetersNames: AlltweetsFromApi[i].ExpendedTweets[j].AttributedTweetersNames, //we will insert it into table attributesOfTweets_2021 (idTweet, idTweeter) --> in order to insert it, we need to insert the tweeter first!
                SearchId: AlltweetsFromApi[i].ExpendedTweets[j].SearchId, // we will insert it into table tweetsBySearch_2021 (idTweet, searchId) 
                UserId: AlltweetsFromApi[i].ExpendedTweets[j].UserId
            }
            Alltweets.push(tweet);
        }
        AlltweetersFromApi = AlltweetersFromApi.concat(AlltweetsFromApi[i].TweetersArrays);
    }

    TweetersToExplore = [...new Set(TweetersToExplore)]; // remove duplicates
    TweetsToExplore = [...new Set(TweetsToExplore)]; // remove duplicates
    isWholeProcessTweeters = false;

    createTweeters(); // we have the all the tweeters(who wrote the tweets) info, we just need to do the post - half process

    if (fullOrCustomized == "customized" && counter2CustomizedIndication == 1)
        indicationUser("calculating antisemitism..");
    counter2CustomizedIndication++;
    if (TweetersToExplore.length > 0 || TweetsToExplore.length > 0) {
        chkLenTweetersToExplore = 0 //TweetersToExplore.length = 0     
        if (TweetersToExplore.length > 0) { //two cases : 1. there are tweeters to explore + tweets to explore. 2. there are tweeters to explore but not tweets to explore.
            callTweetersToExplore(); // only attributed tweeters - we need the whole process 
            chkLenTweetersToExplore = 1 //TweetersToExplore.length > 0
        }
        if (chkLenTweetersToExplore == 0 && TweetsToExplore.length > 0) // one case : there are tweets to explore but not tweeters to explore
            callTweets();
    }
}

//post all the tweets - the whole chain
function postTweets() {
    if (Alltweets.length > 0) { //there are tweets to post
        if (fullOrCustomized == "customized")
            indicationUser("almost there..");
        Alltweets = getUnique();
        Alltweets.sort((a, b) => (a.IdTweet > b.IdTweet) ? 1 : -1) // sort allTweets by idTweet from min to max(old to new)
        console.log(Alltweets);
           if (fullOrCustomized != "full") {
               for (var i = 0; i < Alltweets.length; i++) {
                   Alltweets[i].UserId = LogedUser.Iduser;
               }
           }
        ajaxCall('POST', '../api/Tweets', JSON.stringify(Alltweets), postSuccessTweets, ecb);
    }
}

function getUnique() {

    // store the comparison  values in array
    const unique = Alltweets.map(e => e['IdTweet'])

        // store the indexes of the unique objects
        .map((e, i, final) => final.indexOf(e) === i && i)

        // eliminate the false indexes & return unique objects
        .filter((e) => Alltweets[e]).map(e => Alltweets[e]);

    return unique;
}



function postSuccessTweets(data) {
     AntisemiticTweetsForHashtags = data;
    console.log("finished to post all tweets successfully");
    if (fullOrCustomized == "full")
        ajaxCall('POST', '../api/Tweets/PostTweetsBySearch', JSON.stringify(Alltweets), postSuccessTweetsBy, ecb);
    else {
        
        ajaxCall('POST', '../api/Tweets/PostTweetsByCustomizedSearch', JSON.stringify(Alltweets), postSuccessTweetsBy, ecb);
    }
}

//error function
function ecb(data) {
    console.log(data);
}

//for dateset in python
function postContents() {
    const regex = /["']/g;
    for (var i = 0; i < data.length; i++) {
        idStrTweet = data[i].IdStr;
        data[i].Text = data[i].Text.replace(regex, "''");
        textTweet = data[i].Text;
        urlTweet = data[i].Url;
        IdStrUser = data[i].CreatedBy.IdStr;

        let content = {
            idStrTweet,
            textTweet,
            urlTweet,
            IdStrUser
        }
        contents.push(content);
    }
    ajaxCall('POST', '../api/Contents', JSON.stringify(contents), postConSuccess, ecb);
}

function postSuccess(data) {
    console.log(data);
}

function postSuccessTweetsBy(data) {
    Alltweets = [];
    getGeneral();
}



function getGeneral() {
    ajaxCall('GET', '../api/Search/GetGeneralHashtagsNotToSearch', "", getSuccessGeneral, ecb);
}

function getSuccessGeneral(data) {
    GeneralHashtagsNotToSearch = data;
    ajaxCall('GET', '../api/Search/GetSearchSystemKeys', "", getSuccessSearchSystemKeys, ecb);
}

function getSuccessSearchSystemKeys(data) {
    SearchSystemKeys = data;
    if (fullOrCustomized == "full") {
        let HashtagsOfAntisemiticTweets = handleHashtagsToExplore();
        ajaxCall('POST', '../api/HashtagsToManager', JSON.stringify(HashtagsOfAntisemiticTweets), postSuccess, ecb);
    }
        
    else {
        LogedUser = JSON.parse(localStorage.getItem("LogedUser"));
        ajaxCall('GET', '../api/Search/GetCustomized/' + LogedUser.Iduser, "", getSuccessCustomized, ecb);
    }    
}
function getSuccessCustomized(data) {
    CustomizedSearch = data;
    let HashtagsOfAntisemiticTweets = handleHashtagsToExplore();
    if (HashtagsOfAntisemiticTweets.length > 0) {
        for (var i = 0; i < CustomizedSearch.length; i++) {
            HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.filter(item => ("#" + item) !== CustomizedSearch[i]);
        }
    }
    if (HashtagsOfAntisemiticTweets.length > 0) {
        let obj = {
            IdUser: LogedUser.Iduser,
            Hashtags: HashtagsOfAntisemiticTweets
        }
        ajaxCall('POST', '../api/HashtagsToUser', JSON.stringify(obj), postSuccessHashtagsToUser, ecb);
    }
    else {
        postSuccessHashtagsToUser();
    }
    
}


function handleHashtagsToExplore() {
    let HashtagsOfAntisemiticTweets = [];
    for (var i = 0; i < hashtagsToExplore.length; i++) {
        let foundAntisemiticTweet = AntisemiticTweetsForHashtags.find(item => item.IdTweet == hashtagsToExplore[i].IdTweet);
        if (foundAntisemiticTweet !== undefined) { //hashtag object of antisemitic tweet
            for (var j = 0; j < hashtagsToExplore[i].Hashtags.length; j++) {
                HashtagsOfAntisemiticTweets.push(hashtagsToExplore[i].Hashtags[j].toLowerCase()) //all hashtags of antisemitic tweets
            }
        }
    }

    ////list of dictionary - count how many times each hashtag appears
    let countOccurrencesHashtags = HashtagsOfAntisemiticTweets.reduce((prev, curr) => (prev[curr] = ++prev[curr] || 1, prev), {});

    HashtagsOfAntisemiticTweets = [...new Set(HashtagsOfAntisemiticTweets)]; // remove duplicates

    for (var key in countOccurrencesHashtags) {
        var value = countOccurrencesHashtags[key];
        if ((fullOrCustomized == "full" && value < 5) || (fullOrCustomized == "customized" && value < 2))
            HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.filter(item => item !== key); //remove hashtag from array (because it doesn't appear many times)
    }
    if (HashtagsOfAntisemiticTweets.length > 0) {
        for (var i = 0; i < GeneralHashtagsNotToSearch.length; i++) {//check if exists in general list of hashtags
            HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.filter(item => ("#" + item) !== GeneralHashtagsNotToSearch[i]);
        }
        for (var i = 0; i < SearchSystemKeys.length; i++) {//check if exists in search list of hashtags(is active =1)
            HashtagsOfAntisemiticTweets = HashtagsOfAntisemiticTweets.filter(item => ("#" + item) !== SearchSystemKeys[i]);
        }
    }
    return HashtagsOfAntisemiticTweets;

}

//success post content
function postConSuccess(data) {
    console.log(data);
    $("#btnGo").prop("disabled", false);
}

function postSuccessHashtagsToUser(data) {
    if (AntisemiticTweetsForHashtags.length > 0) {
        CounterCustomizedSearch++;
        if (fullOrCustomized == "customized")
            indicationUser("");

        getSuccessRenderDataTable(AntisemiticTweetsForHashtags);

        //else redrawTable(tbl, AntisemiticTweetsForHashtags);
    }
    else {
        redrawTable("", "");
        document.getElementById("tweetsTable").innerHTML = "";
        swal("No suspicious tweets were found", "You should try another search key", "info");
        $("button#btnSearchKeywords").attr("disabled", false);
        inputValue = true;
        $("#loadingTwitter").hide();
        $("#indication").hide();
        //indicationUser("No suspicious tweets were found");
        //$("#resultCustomized").text("No suspicious tweets were found.")
    }
   
}
