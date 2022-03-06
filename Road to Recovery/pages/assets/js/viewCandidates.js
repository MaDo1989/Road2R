checkCookie();
let { convertDBDate2FrontEndDate, getHebrew_WeekDay, addSeperator2MobileNum } = GENERAL.USEFULL_FUNCTIONS;
let { getRidePatNum4_viewCandidate } = GENERAL.RIDEPAT;
let { ajaxCall } = GENERAL.FETCH_DATA;
let allCandidatedFromDB;
let { COPYWRITE } = GENERAL;
let candidatesTable;
let regularCandidated_clientVersion = [];
let superCandidated_clientVersion = [];
let ridePatNum;


const wiringDataTables = () => {
    //manage button clicks on tables

    //      EXAMPLE:

    //$('#datatable-morning tbody').on('click', '#candidatesBtn', function () {
    //    candidatesButton(this);
    //});

}

$(document).ready(() => {

    candidatesTable   = $('#datatable-candidates').DataTable({ data: [], destroy: true });
    superDriversTable = $('#datatable-superDrivers').DataTable({ data: [], destroy: true });
    newDriversTable   = $('#datatable-newDrivers').DataTable({ data: [], destroy: true });

    ridePatNum = JSON.parse( getRidePatNum4_viewCandidate());
    getCandidates();
    getRidePat();


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
});


const deceideWhichTable2Show = () => {
    $('#collapse1').addClass("in");
    $('#collapsed1_aTag').removeClass('collapsed');
}
const viewCharactaristics = () => {
    window.open("viewC.html?ridePatNum=" + ridePatNum, '_blank').focus();
}

const getRidePat = () => {
    let ridePatObj = localStorage.getItem(`ridePatObj_${ridePatNum}`);
    renderRidePatDetails(JSON.parse(ridePatObj));
}

const renderRidePatDetails = (ridepat) => {
    let ridepatDate = convertDBDate2FrontEndDate(ridepat.Date);
    let isToday = isItToday(ridepatDate);
    let isAfterNoon = ridepatDate.getMinutes() === 14;


    let ridePatDetails = `מ`;
    ridePatDetails += ridepat.Origin.Name;
    ridePatDetails += ' ';
    ridePatDetails += `ל`;
    ridePatDetails += ridepat.Destination.Name;
    ridePatDetails += ' ';
    ridePatDetails += isToday ? 'היום' : `ב` + getHebrew_WeekDay(ridepatDate.getDay());
    ridePatDetails += ' ';
    ridePatDetails += `${ridepatDate.getDate()}.${parseInt(ridepatDate.getMonth() + 1)}`;
    ridePatDetails += ' ';
    ridePatDetails += isAfterNoon ? `אחה"צ` : `בשעה  ${ridepatDate.toLocaleString('he-IL', { timeStyle: 'short' })}`;
    ridePatDetails += ' ';
    ridePatDetails += 'מקומות: ' + parseInt(ridepat.Escorts.length + 1);
    //ridePatDetails += ridepatDate.Equipment.length > 0 && ridepatDate.Equipment.includes(''); 

    document.getElementById('RideCandidates_ph').innerHTML = ridePatDetails;
}

const isItToday = (d) => {

    let now = new Date(Date.now());
    let daysMatch = d.getDate() === now.getDate();
    let monthMatch = d.getMonth() === now.getMonth();
    let yearMatch = d.getFullYear() === now.getFullYear();

    let result = daysMatch && monthMatch && yearMatch;

    return result;
}

const getCandidates = () => {

    // BENNY
    let numOfCandidates = 10;
    let newFlag = false;

    $('#wait').show();
    ajaxCall(
        'GetCandidates',
        JSON.stringify({ ridePatNum, numOfCandidates, newFlag }),
        getCandidates_SCB,
        getCandidates_ECB
    );
}

const getCandidates_SCB = (data) => {

    $('#wait').hide();
    deceideWhichTable2Show();

    allCandidatedFromDB = JSON.parse(data.d);
    fillTableWithData();

}

const getCandidates_ECB = (data) => {

    $('#wait').hide();

    console.log('%c ↓ R2R custom error ↓', 'background: red; color: white');
    console.log(data);
    console.log('%c ↑ R2R custom error ↑', 'background: red; color: white');
}

const fillTableWithData = () => {

    let thisCandidate = {};
    regularCandidated_clientVersion = [];
    superCandidated_clientVersion = [];
    newCandidated_clientVersion = [];

    let date2display;

    for (let i in allCandidatedFromDB) {

        date2display = convertDBDate2FrontEndDate(allCandidatedFromDB[i].LatestDocumentedCallDate).toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });

        thisCandidate = {
            id: i,
            displayName: allCandidatedFromDB[i].DisplayName,
            cellphone: addSeperator2MobileNum(allCandidatedFromDB[i].CellPhone, "-"),
            city: allCandidatedFromDB[i].City,// + '<br />בדיקה',
            daysSinceLastRide: allCandidatedFromDB[i].DaysSinceLastRide,
            numOfRides_last2Months: allCandidatedFromDB[i].NumOfRides_last2Months,
            daysUntilNextRide: allCandidatedFromDB[i].DaysUntilNextRide,
            latestDocumentedCallDate: date2display,
            seniorityInYears: allCandidatedFromDB[i].SeniorityInYears,
            buttons: ''
        }

        switch (allCandidatedFromDB[i].DriverLevel) {

            case 0:
                newCandidated_clientVersion.push(thisCandidate);
                break;
            case 1:
                regularCandidated_clientVersion.push(thisCandidate);
                break;
            case 2:
                superCandidated_clientVersion.push(thisCandidate);
                break;
            default:
                alert("illegal DriverType");
                break;
        }

        //allCandidatedFromDB[i].IsSuperDriver ?
        //    superCandidated_clientVersion.push(thisCandidate) :
        //    regularCandidated_clientVersion.push(thisCandidate);

        //#region ↓DATATABLES PROPERTIES↓|

        /* 
                     ============================
                     || ↓DATATABLES PROPERTIES↓||
                     ============================
        */
    }
    candidatesTable = $('#datatable-candidates').DataTable({
        data: regularCandidated_clientVersion,
        rowId: 'id',
        pageLength: 10,
        stateSave: true,
        destroy: true,
        "lengthChange": false, // for somereason this property must be string
        stateDuration: 60 * 60,
        autoWidth: false,
        columns: [
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            { data: "displayName" },                                //0
            { data: "cellphone" },                                  //1
            { data: "city" },                                       //2
            { data: "daysSinceLastRide" },                          //3
            { data: "numOfRides_last2Months" },                     //4
            { data: "daysUntilNextRide" },                          //5
            { data: "latestDocumentedCallDate" },                   //6
            { data: "seniorityInYears" },                           //7
            { data: "buttons" }                                    //8
        ],
        columnDefs: [
            { width: '20%', "targets": [0] },
            { width: '10%', "targets": [1] },
            { width: '15%', "targets": [2] },
            { width: '10%', "targets": [3] },
            { width: '5%', "targets": [4, 5, 7] },
            { width: '12%', "targets": [6, 8] }
        ]
    });

    superDriversTable = $('#datatable-superDrivers').DataTable(
        {
            data: superCandidated_clientVersion,
            rowId: 'id',
            pageLength: 10,
            stateSave: true,
            destroy: true,
            "lengthChange": false, // for somereason this property must be string
            stateDuration: 60 * 60,
            autoWidth: false,
            columns: [
                //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
                { data: "displayName" },                                //0
                { data: "cellphone" },                                  //1
                { data: "city" },                                       //2
                { data: "daysSinceLastRide" },                          //3
                { data: "numOfRides_last2Months" },                     //4
                { data: "daysUntilNextRide" },                          //5
                { data: "latestDocumentedCallDate" },                   //6
                { data: "seniorityInYears" },                           //7
                { data: "buttons" }                                    //8

            ],
            columnDefs: [
                { width: '20%', "targets": [0] },
                { width: '10%', "targets": [1] },
                { width: '15%', "targets": [2] },
                { width: '10%', "targets": [3] },
                { width: '5%', "targets": [4, 5, 7] },
                { width: '12%', "targets": [6, 8] }
            ]
        });

    newDriversTable = $('#datatable-newDrivers').DataTable(
        {
            data: newCandidated_clientVersion,
            rowId: 'id',
            pageLength: 10,
            stateSave: true,
            destroy: true,
            "lengthChange": false, // for somereason this property must be string
            stateDuration: 60 * 60,
            autoWidth: false,
            columns: [
                //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
                { data: "displayName" },                                //0
                { data: "cellphone" },                                  //1
                { data: "city" },                                       //2
                { data: "daysSinceLastRide" },                          //3
                { data: "numOfRides_last2Months" },                     //4
                { data: "daysUntilNextRide" },                          //5
                { data: "latestDocumentedCallDate" },                   //6
                { data: "seniorityInYears" },                           //7
                { data: "buttons" }                                    //8

            ],
            columnDefs: [
                { width: '20%', "targets": [0] },
                { width: '10%', "targets": [1] },
                { width: '15%', "targets": [2] },
                { width: '10%', "targets": [3] },
                { width: '5%', "targets": [4, 5, 7] },
                { width: '12%', "targets": [6, 8] }
            ]
        });

    /*
                     ============================
                     || ↑DATATABLES PROPERTIES↑||
                     ============================
    */
    //#endregion ↑DATATABLES PROPERTIES↑


}

