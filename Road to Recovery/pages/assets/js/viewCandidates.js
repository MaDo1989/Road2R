checkCookie();
let { convertDBDate2FrontEndDate, getHebrew_WeekDay } = GENERAL.USEFULL_FUNCTIONS;
//let { getRidePatNum4_viewCandidate } = GENERAL.RIDEPAT;


let { ajaxCall } = GENERAL.FETCH_DATA;
let thisRidePat;
let { COPYWRITE } = GENERAL;

const wiringDataTables = () => {
    //manage button clicks on tables

    //      EXAMPLE:

    //$('#datatable-morning tbody').on('click', '#candidatesBtn', function () {
    //    candidatesButton(this);
    //});

}


function handleClick() {
    fetchData4ThisRidepat();

}
$(document).ready(() => {
    $("#GetCandidatesBTN").click(handleClick);

   
    $('#rights').html(COPYWRITE());

    if (!JSON.parse(localStorage.getItem("isProductionDatabase"))) {
        $("#databaseType").text("Test database ")
    }
    else $("#databaseType").text("Production database")
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body'
    })
    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.toLowerCase().indexOf('test') != -1) {
        $("#na").css("background-color", "#ffde89");
    }
    if (window.location.href.indexOf('http://40.117.122.242/Road%20to%20Recovery/') != -1) {
        window.location.href = "notAvailable.html";
    }

    if (JSON.parse(GENERAL.USER.getIsAssistant())) {
        $("#menuType").attr("w3-include-html", "HelperMenu.html");
        $("#userName").html(GENERAL.USER.getAsistantAndCoorDisplayName());
    }
    else {
        $("#menuType").attr("w3-include-html", "menu.html");
        $("#userName").html(GENERAL.USER.getUserDisplayName());

    }
    includeHTML();//with out this there is no side bar!


    //#region ↓DATATABLES PROPERTIES↓|


    /* 
                 ============================
                 || ↓DATATABLES PROPERTIES↓||
                 ============================
    */
    candidatesTable = $('#datatable-candidates').DataTable(
        {
            "stateDuration": 60 * 60,
            "columnDefs": []
        },
        {
            "paging": false
        },
        {
            "visible": false,
            "targets": [1]
        },
        {
            "className": "dt-body-left",
            "targets": [3]
        }
    );


    superDriversTable = $('#datatable-superDrivers').DataTable(
        {
            "stateDuration": 60 * 60,
            "columnDefs": []
        },
        {
            "paging": false
        },
        {
            "visible": false,
            "targets": [1]
        },
        {
            "className": "dt-body-left",
            "targets": [3]
        }
    );



    /*
                     ============================
                     || ↑DATATABLES PROPERTIES↑||
                     ============================
    */
    //#endregion ↑DATATABLES PROPERTIES↑



});




const fetchData4ThisRidepat = () => {

    //let ridePatNum = JSON.parse(getRidePatNum4_viewCandidate());
    let ridePatNum =  $("#rideId").val();

    ajaxCall(
        'GetCandidates',
        JSON.stringify({ ridePatNum }),
        fetchData4ThisRidepat_SCB,
        fetchData4ThisRidepat_ECB
    );
}

const fetchData4ThisRidepat_SCB = (data) => {
    thisRidePat = JSON.parse(data.d); //think if this variable should be global or local & pass throw this function below

    //temp ↓
    console.log('%c ↓ R2R custom error ↓', 'background: green; color: white');

    console.log(thisRidePat);

    console.log('%c ↑ R2R custom error ↑', 'background: green; color: white');

    //temp ↑

    //useRidePatData();
}

const fetchData4ThisRidepat_ECB = (data) => {
    console.log('%c ↓ R2R custom error ↓', 'background: red; color: white');
    console.log(data);
    console.log('%c ↑ R2R custom error ↑', 'background: red; color: white');
}


const useRidePatData = () => {

    let date = convertDBDate2FrontEndDate(thisRidePat.Date);
    let dd = date.getDate();
    let mm = date.getMonth() + 1;

    let now = new Date(Date.now());

    let hours = date.getHours();
    let minutes = date.getMinutes();

    // general: מ<מוצא> ל<יעד>, <היום או יום <יום בשבוע>, <תאריך ללא שנה>> ב-<שעה> <בבוקר או רחה"צ>
    // example:  מועמדים להסעה: מתרקומיא לשיבא, היום ב6:15 בבוקר
    let ridePatCandidatesHeadLine = `מ${thisRidePat.Origin.Name} `;
    ridePatCandidatesHeadLine += `ל${thisRidePat.Destination.Name} `;
    if (dd === now.getDate() &&
        mm === now.getMonth() + 1 &&
        date.getFullYear() === now.getFullYear()
    ) {
        ridePatCandidatesHeadLine += 'היום ';
    } else {
        ridePatCandidatesHeadLine += `ב${getHebrew_WeekDay(date.getDay())}, ${dd}.${mm} ` ;
    }

    if (date.getMinutes() === 14) {
        if (date.getHours() === 19 || date.getHours() === 20 || date.getHours() === 21 || date.getHours() === 22) {
            ridePatCandidatesHeadLine += 'אחה"צ ';
        }
    } else {
        ridePatCandidatesHeadLine += ` ב-${hours}:${minutes < 10 ? '0' + minutes : minutes} `;
        ridePatCandidatesHeadLine += hours <= 11 ? 'בבוקר' : 'בצהריים'
    }

    document.getElementById('RideCandidates_ph').innerHTML = ridePatCandidatesHeadLine;
    //here I stop and went to develop sort by regions

}


//function refreshTable() {
//    //???

//}




//function manipulateTables() {

//    tableMorning = document.getElementById("datatable-morning");
//    tableAfternoon = document.getElementById("datatable-afternoon");

//}