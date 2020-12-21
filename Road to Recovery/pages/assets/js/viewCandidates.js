checkCookie();
let { convertDBDate2FrontEndDate } = GENERAL.USEFULL_FUNCTIONS;
let { getRidePatNum4_viewCandidate } = GENERAL.RIDEPAT;
let { ajaxCall } = GENERAL.FETCH_DATA;
let thisRidePat;

const wiringDataTables = () => {
    //manage button clicks on tables

    //      EXAMPLE:

    //$('#datatable-morning tbody').on('click', '#candidatesBtn', function () {
    //    candidatesButton(this);
    //});

}

$(document).ready(() => {
    fetchData4ThisRidepat();

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



});




const fetchData4ThisRidepat = () => {

    let ridePatNum = JSON.parse(getRidePatNum4_viewCandidate());
    ajaxCall(
        'GetRidePat',
        JSON.stringify({ ridePatNum }),
        fetchData4ThisRidepat_SCB,
        fetchData4ThisRidepat_ECB
    );
}

const fetchData4ThisRidepat_SCB = (data) => {
    thisRidePat = JSON.parse(data.d); //think if this variable should be global or local & pass throw this function below

    console.log(thisRidePat);

    useRidePatData();
}

const fetchData4ThisRidepat_ECB = (data) => {
    console.log('%c ↓ R2R custom error ↓', 'background: red; color: white');
    console.log(data);
    console.log('%c ↑ R2R custom error ↑', 'background: red; color: white');
}


const useRidePatData = () => {
    
    let date = convertDBDate2FrontEndDate(thisRidePat.Date);
    let hh = date.getHours(); 
    let mm = date.getMinutes();
    //YOGEV STOPED HERE IN 22.12.2020 1:00 AM 
    let rideCandidatesHeadLine = `מ לשיבא, היום ב6:15 בבוקר`;
    rideCandidatesHeadLine += ` ${thisRidePat.Origin.Name} `;
    rideCandidatesHeadLine += `ל`;
    rideCandidatesHeadLine += ` ${thisRidePat.Destination.Name} `;
    rideCandidatesHeadLine += ``;
    rideCandidatesHeadLine += ``;
    rideCandidatesHeadLine += ``;

    document.getElementById('RideCandidates_ph').innerHTML = rideCandidatesHeadLine;
}


//function refreshTable() {
//    //???

//}




//function manipulateTables() {

//    tableMorning = document.getElementById("datatable-morning");
//    tableAfternoon = document.getElementById("datatable-afternoon");

//}