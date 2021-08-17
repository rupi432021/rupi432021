$(document).ready(function () {   
    PageName = "VolunteerReport";
    CounterCustomizedSearch = 0;
    ajaxCall("GET", "../api/Decisions", "", getSuccessDecision, error);   //get reasons
    buttonEvents();
    $('input:checkbox').click(function () { //onclick on checkbox
        $('input:checkbox').not(this).prop('checked', false);  //only one checkbox can be checked 
        if (this.checked) { //if the checkbox is checked
            $("#reasonsOptions").show();
            $("#reason").show();
        }
        else { //if the checkbox is unchecked
            $("#reasonsOptions").hide();
            $("#reason").hide();
        }
    });

    $('#DescModal').on('hidden.bs.modal', function () { //on close modal - reset
        $('input[type=checkbox]').prop('checked', false);
        $("#reasonsOptions").hide();
        $("#reason").hide();
        if ($('#message').length)  // if the message of submit exists
            $("#message").remove(); // remove it            
        $("button#submitBtn").show(); //if it was closed after submit (because we hide it on successful submit)            
    })
});

function initVolunteerReport(navPage) {
    PageName = navPage;
    LogedUser = "";
    if (localStorage.getItem("LogedUser") !== null) {
        //  $("#btnLog").html("Log Out");
        LogedUser = JSON.parse(localStorage.getItem("LogedUser"));
        $("#navbarDropdowUserName").text(LogedUser.FirstName);
    }
    // once the document is ready we  we print all tweets from the server 
    let api = "../api/Tweets/GetTweets/" + LogedUser.Iduser;
    ajaxCall("GET", api, "", getSuccessRenderDataTable, error);
}


//report button
function buttonEvents() {
    $(document).on("click", ".viewBtn", function () {
        markSelected(this);
        console.log(this);
        $("#cancelSaveBTN").prop("disabled", false);// view mode: disable all controls in the form לבדוק
        populateFields(this.getAttribute('data-tweetId')); //fill the fields in the pop up 
        $('#DescModal').modal("show"); //popup     
    });
    $(document).on("click", "#submitBtn", onSubmitFunc);
}

// mark the selected row
function markSelected(btn) {
    $("#tweetsTable tr").removeClass("selected"); // remove seleced class from rows that were selected before
    row = (btn.parentNode).parentNode; // button is in TD which is in Row
    row.className = 'selected'; // mark as selected
}

//post report
function onSubmitFunc() {
    sel = document.getElementById("reasonsOptions");
    reasonId = sel.options[sel.selectedIndex].id; //it can never choose the first option "choose a reason"
    if (reasonId != "") { //if no reason was picked then we won't submit
        let reportObj = {
            Idtweet: tweetIdReport,
            Iduser: LogedUser.Iduser, 
            Idecision: reasonId
        }
        ajaxCall("POST", "../api/Reports", JSON.stringify(reportObj), insertSuccess, error);
    }
    else
        alert("You must decide and give a reason for your decision.");
}

//success of post report
function insertSuccess(data) {
    console.log(data);
    $("button#submitBtn").hide();
    str = "<p id='message'> Report number " + data + " was submitted successfully </p>";
    $(".modal-footer").append(str);
    tweets = tweets.filter(item => item.IdTweet == tweetIdReport);
    if (localStorage.getItem("tweetsdata") !== null) {
        tweetsdata = JSON.parse(localStorage.getItem("tweetsdata"));
        tweetsdata = tweetsdata.filter(item => item.IdTweet != tweetIdReport);
    }
    if (PageName != "")//volunteer page
        location.reload();
    else {//customized page
        
        redrawTable(tbl, tweetsdata);
    }
}


function insertValToReasons(chk) {
    if (chk == "yes") {
        decision = 1; //yes to report anti                            
    }
    if (chk == "no") {
        decision = 0; //no - report not anti
    }
    str = '<option value="" selected="true" disabled="disabled">Choose a reason</option>'; //when we open the options - it's disabled
    for (var i = 0; i < reasons.length; i++) {
        if (reasons[i].DecisionBit == decision) {
            str += '<option id="' + reasons[i].IdDecision + '" value="' + reasons[i].IdDecision + '">' + reasons[i].Reason + '</option>';
        }
    }
    $("#reasonsOptions").html(str);

    $("#reasonsOptions").show();
    $("#reason").show();

    $('input:checkbox').click(function () { //onclick on checkbox
        $('input:checkbox').not(this).prop('checked', false);  //only one checkbox can be checked 
        if (this.checked) { //if the checkbox is checked
            $("#reasonsOptions").show();
            $("#reason").show();
        }
        else { //if the checkbox is unchecked
            $("#reasonsOptions").hide();
            $("#reason").hide();
        }
    });

    $('#DescModal').on('hidden.bs.modal', function () { //on close modal - reset
        $('input[type=checkbox]').prop('checked', false);
        $("#reasonsOptions").hide();
        $("#reason").hide();
        if ($('#message').length)  // if the message of submit exists
            $("#message").remove(); // remove it            
        $("button#submitBtn").show(); //if it was closed after submit (because we hide it on successful submit)            
    })

}

// fill the form fields in pop up
function populateFields(tweetId) {
    tweet = getTweet(tweetId);
    console.log(tweet);
    $("#urlTweet").attr("href", tweet.UrlTweet);
    $("#urlTweet").text(tweet.UrlTweet);
    tweetIdReport = tweetId; //global     
}

//get a tweet according to its Id  
function getTweet(id) {
    for (i in tweets) {
        if (tweets[i].IdTweet == id)
            return tweets[i];
    }
    console.log("No tweet was found with the following ID " + id);
    return null;
}

// redraw a datatable with new data
function redrawTable(tbl, data) {
    if (data != "") {
    tbl.clear();
        for (var i = 0; i < data.length; i++) {
            data[i].FinalScore = ((data[i].FinalScore * 100).toString()).slice(0, 5) + "%";
        }
        for (var i = 0; i < data.length; i++) {
            tbl.row.add(data[i]);
        }

        tbl.draw();
    }
}

//print the data table of tweets - success of get tweets
function getSuccessRenderDataTable(tweetsdata) {
  
    if (CounterCustomizedSearch > 1) {
        redrawTable(tbl, tweetsdata)
    }
    else {
        if (PageName !== "Home") {
            str = "<h1 class='titleStyle'>Tweets for Monitoring</h1>";
            document.getElementById("titleTable").innerHTML = str;
        }
        str = "<thead><tr><th>Num</th><th>Content</th><th>Percentage of certainty</th><th id='report'>Report tweet</th></tr></thead>";
        document.getElementById("tweetsTable").innerHTML = str;
        console.log(tweetsdata);
        if (PageName == "customized") {
            localStorage.setItem("tweetsdata", JSON.stringify(tweetsdata));

            setTimeout(function () {
                $("button#btnSearchKeywords").attr("disabled", false);
            }, 9000);

        }
        for (var i = 0; i < tweetsdata.length; i++) {
            tweetsdata[i].FinalScore = ((tweetsdata[i].FinalScore * 100).toString()).slice(0, 5) + "%";
        }

        tweets = tweetsdata; // keep the tweets array in a global variable;   
        columnDefs=true;
        if (PageName != "Home") {
            columnDefs = false;
        }
        
        try {
          
            tbl = $('#tweetsTable').DataTable({
                data: tweetsdata,
                pageLength: 20,
                "order": [[1, 'asc']],
                "columnDefs": [{
                    "searchable": columnDefs,
                    "orderable": columnDefs,
                    "targets": 0
                }],
                columns: [
                    { data: "IdTweet" },
                    { data: "TweetText" },
                    {
                        data: "FinalScore"
                    },
                    {
                        render: function (data, type, row, meta) {
                            let dataTweets = "data-tweetId='" + row.IdTweet + "'";
                            viewBtn = "<button type='button' class = 'viewBtn' data-toggle='modal' " + dataTweets + " data-target='#DescModal'></button>";
                            return viewBtn;
                        }
                    }
                ],
                "pagingType": "simple",
                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
            });
            tbl.on('order.dt search.dt', function () {
                tbl.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
            }).draw();
            tbl.on('page.dt', function () {
                $('html, body').animate({
                    scrollTop: $("#tweetsTable").offset().top
                }, 'slow');
            });
        }
        catch (err) {
            console.log(err);
        }
         
        if (PageName == "Home") {
            document.getElementById("Mform").classList.add("HomeDT");
            let str = "<div class='col-2'><p class='linkCenter'><a id='goFull' href='VolunteerReports.html'>GO TO FULL LIST</a></p></div>";
            document.getElementById("homeDTRow").innerHTML += str;
        }

        if (PageName == "VolunteerReport") {
            $(".loadingPage").css("display", "none");
            $(".waitGet").css("display", "block");
        }
        else {
            $(".waitGet").css("display", "flex");
            $(".form-inline").css("display", "flex");
            $(".form-inline").css("font-size", "smaller");
        }
    }
}

function getSuccessDecision(data) {
    reasons = data;
}

//error function
function error(err) {
    swal("Error: " + err);
}

function logOut() {
    localStorage.removeItem("LogedUser");
    window.location.href = "Login.html"
}


function indicationUser(data) {
    //$("#indication").html(data);
    if (data != "") {
        document.getElementById("indication").innerHTML = data;
    }
    else {
        $("#loadingTwitter").hide();
        $("#indication").hide();
    }
    //alert(data);
}