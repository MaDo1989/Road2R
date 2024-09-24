checkCookie();
let { getHebrew_WeekDay, addSeperator2MobileNum, showMe, convertDBDate2FrontEndDate } = GENERAL.USEFULL_FUNCTIONS;
let { getRidePatNum4_viewCandidate } = GENERAL.RIDEPAT;
let { ajaxCall } = GENERAL.FETCH_DATA;
let allCandidatedFromDB;
let { COPYWRITE } = GENERAL;
let candidatesTable;
let superDriversTable;
let regularCandidated_clientVersion = [];
let superCandidated_clientVersion = [];
let ridePatNum;


const wiringDataTables = () => {
    //manage button clicks on tables

    //#region showDocumentedCallsBtn
    $('#datatable-candidates tbody').on('click', '#showDocumentedCallsBtn', function () {
        manipulateDocumentedCallsModal(this, candidatesTable);
    });

    $('#datatable-superDrivers tbody').on('click', '#showDocumentedCallsBtn', function () {
        manipulateDocumentedCallsModal(this, superDriversTable);
    });
    //#endregion showDocumentedCallsBtn

    //#region showDocumentedRidesBtn
    $('#datatable-candidates tbody').on('click', '#showDocumentedRidesBtn', function () {
        manipulateDocumentedRidesModal(this, candidatesTable);
    });

    $('#datatable-superDrivers tbody').on('click', '#showDocumentedRidesBtn', function () {
        manipulateDocumentedRidesModal(this, superDriversTable);
    });
    //#endregion showDocumentedRidesBtn

    //#region showMe
    /*
    IMPORTANT NOTE
    the event is mouseup in order to catch all scenarios.
    */
    $('#datatable-candidates tbody').on('mouseup', '.showMe', function () {
        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);
    });

    $('#datatable-superDrivers tbody').on('mouseup', '.showMe', function () {
        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);
    });
    //#endregion showMe

}

const ConvertDBDate2UIDate = (fullTimeStempStr) => {
    if (fullTimeStempStr === undefined) return;
    let startTrim = fullTimeStempStr.indexOf('(') + 1;
    let endTrim = fullTimeStempStr.indexOf(')');
    let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
    let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));

    //Note: in getMOnth function 0=January, 1=February etc...

    let dd = fullTimeStemp.getDate();

    let mm = fullTimeStemp.getMonth() + 1;

    let yyyy = fullTimeStemp.getFullYear();

    return `${dd}.${mm}.${yyyy}`;
}

function hideCharacteristics() {
    $("#characteristics").css("visibility", "hidden");
}

function showCharacteristics() {

    let id = this.getAttribute('data-driverId');
    let c = allCandidatedFromDB[id];
    //console.log(c);
    let boldArr = ["נסיעות בציר המדויק הזה", "הסעות ביום הזה", "הסעות בחלק הנדרש של היום"];
    let txt = {
        "נסיעות בציר המדויק הזה": c.AmountOfRidesInThisPath,
        "נסיעות בציר ההפוך": c.AmountOfRidesInOppositePath,
        "נסיעות מאותה נקודה לאותו איזור": c.AmountOfRides_OriginToArea,
        "נסיעות נוספות": c.NoOfDocumentedRides - c.AmountOfRides_OriginToArea,
        "הסעות ביום הזה": c.AmountOfRidesAtThisDayWeek,
        "הסעות בימים אחרים": c.NoOfDocumentedRides - c.AmountOfRidesAtThisDayWeek,
        "הסעות בחלק הנדרש של היום": c.AmountOfRidesAtThisTime,
        "הסעות בחלקו השני של היום": c.NoOfDocumentedRides - c.AmountOfRidesAtThisTime
    };

    let str = "<h3> נתוני נסיעות בחצי שנה האחרונה </h3>";
    str += "<h3>" + c.DisplayName + "</h3>";
    for (k in txt) {
        if (txt[k] != 0)
            if (boldArr.indexOf(k) >= 0)
                str += "<p class='boldC'>" + k + " : " + txt[k] + "</p>";
            else
                str += "<p>" + k + " : " + txt[k] + "</p>";
    }
    let position = $(this).position();

    //$("#characteristics").css("left", position.left - 300);
    //$("#characteristics").css("top", position.top - 310);
    $("#characteristics").css("visibility", "visible");
    $("#characteristics").html(str);

}

$(document).ready(() => {
    $("#DocumentedCallsModal").attr("w3-include-html", "DocumentedCallsModal.html");
    $("#documentedRidesModal").attr("w3-include-html", "documentedRidesModal.html");

    $("#characteristics").css("visibility", "hidden");

    $(document).on("mouseover", ".c1", showCharacteristics);
    $(document).on("mouseout", ".c1", hideCharacteristics);

    candidatesTable = $('#datatable-candidates').DataTable({ data: [], destroy: true });
    regularTable = $('#datatable-RegularDrivers').DataTable({ data: [], destroy: true });
    superDriversTable = $('#datatable-superDrivers').DataTable({ data: [], destroy: true });

    ridePatNum = JSON.parse(getRidePatNum4_viewCandidate());

    getRidePat();
    //getCandidates();
    getCandidatesV2();


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

    wiringDataTables();

    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "de_date-asc": function (a, b) {
            /*a and b are a couple of dates to compare
            return value will be:
            0 if a=b
            1 if a > b
           -1 if a < b
            */
            let dateAsArr_a = $.trim(a).split('.');
            let dateAsArr_b = $.trim(b).split('.');

            let a_date = new Date(parseInt(dateAsArr_a[2]), parseInt(dateAsArr_a[1]), parseInt(dateAsArr_a[0]));
            let b_date = new Date(parseInt(dateAsArr_b[2]), parseInt(dateAsArr_b[1]), parseInt(dateAsArr_b[0]));

            a = a_date.getTime();
            b = b_date.getTime();

            return a === b ? 0 : a > b ? 1 : -1;
            //var z = ((x < y) ? -1 : ((x > y) ? 1 : 0));
            //return z;
        },
        "de_date-desc": function (a, b) {
            /*a and b are a couple of dates to compare
              return value will be:
              0 if a=b
             -1 if a > b
              1 if a < b
              */
            let dateAsArr_a = $.trim(a).split('.');
            let dateAsArr_b = $.trim(b).split('.');

            let a_date = new Date(parseInt(dateAsArr_a[2]), parseInt(dateAsArr_a[1]), parseInt(dateAsArr_a[0]));
            let b_date = new Date(parseInt(dateAsArr_b[2]), parseInt(dateAsArr_b[1]), parseInt(dateAsArr_b[0]));

            a = a_date.getTime();
            b = b_date.getTime();

            return a === b ? 0 : a > b ? -1 : 1;
            //var z = 0//((x < y) ? 1 : ((x > y) ? -1 : 0));
            //return z;
        }
    });

});


const deceideWhichTable2Show = () => {
    $('#collapse1').addClass("in");
    $('#collapsed1_aTag').removeClass('collapsed');

    $('#collapsed2_aTag').addClass('collapsed');
    $('#collapsed3_aTag').addClass('collapsed');

}

const viewCharactaristics = () => {
    window.open("viewC.html?ridePatNum=" + ridePatNum + "&dayInWeek=" + ridepatDate.getDay(), '_blank').focus();
}

const getRidePat = () => {
    let ridePatObj = localStorage.getItem(`ridePatObj_${ridePatNum}`);
    renderRidePatDetails(JSON.parse(ridePatObj));
}

const renderRidePatDetails = (ridepat) => {
    //console.log('what is ridepat ? ',ridepat)
    ridepatDate = convertDBDate2FrontEndDate(ridepat.PickupTime);
    let isToday = isItToday(ridepatDate);
    let isAfterNoon = ridepatDate.getMinutes() === 14;
    

    let ridePatDetails = `מ`;
    ridePatDetails += ridepat.Origin;
    ridePatDetails += ' ';
    ridePatDetails += `ל`;
    ridePatDetails += ridepat.Destination;
    ridePatDetails += ' ';
    ridePatDetails += isToday ? 'היום' : `ב` + getHebrew_WeekDay(ridepatDate.getDay());
    ridePatDetails += ' ';
    ridePatDetails += `${ridepatDate.getDate()}.${parseInt(ridepatDate.getMonth() + 1)}`;
    ridePatDetails += ' ';
    ridePatDetails += isAfterNoon ? `אחה"צ` : `בשעה  ${ridepatDate.toLocaleString('he-IL', { timeStyle: 'short' })}`;
    ridePatDetails += ' ';
    ridePatDetails += 'מקומות: ' + parseInt(ridepat.AmountOfEscorts + 1);
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
    let numOfCandidates = 25;
    let newFlag = false;
    let dayInWeek = ridepatDate.getDay();

    $('#wait').show();
    //console.log('what sent ? ', { ridePatNum, numOfCandidates, newFlag, dayInWeek })
    ajaxCall(
        'GetCandidates',
        JSON.stringify({ ridePatNum, numOfCandidates, newFlag, dayInWeek }),
        getCandidates_SCB,
        getCandidates_ECB
    );
}


const getCandidatesV2 = () => {
    $("#wait").show();
    //console.log({ RideNum: ridePatNum, mode: 3 })
    ajaxCall("GetCandidateUnityRideV2",
        JSON.stringify({ RideNum: ridePatNum, mode: 3 }),
        getCandidateV2_SCB,
        getCandidates_ECB
        
    )
}


//new version of getCandidates
const getCandidateV2_SCB = (data) => {
    $('#wait').hide();
    
    allCandidatedFromDB = JSON.parse(data.d);
    //console.log('res v2:', allCandidatedFromDB);
    //new version of fillTableWithData
    fillTableWithDataV2();

}


const getCandidates_SCB = (data) => {

    $('#wait').hide();
    deceideWhichTable2Show();

    allCandidatedFromDB = JSON.parse(data.d);
    console.log('what is the res:', allCandidatedFromDB);
    fillTableWithData();

}

const getCandidates_ECB = (data) => {

    $('#wait').hide();

    console.log('%c ↓ R2R custom error ↓', 'background: red; color: white');
    console.log(data);
    console.log('%c ↑ R2R custom error ↑', 'background: red; color: white');
}

const buildCandidateHTML = ({ Id, DisplayName }) => {

    let candidateHTML = `<a href="volunteerform.html" data-obj="volunteer" class="showMe clickable blueFont boldFont"`;
    candidateHTML += `id='${Id}'>${DisplayName}</a>`;

    return candidateHTML;
}

const fillTableWithData = () => {

    let thisCandidate = {};
    regularCandidated_clientVersion = [];
    superCandidated_clientVersion = [];
    newCandidated_clientVersion = [];

    let date2display;
    let btnStr;
    let showDocumentedCallsBtn;
    let showDocumentedRidesBtn;
    let linkableNameToRender;


    for (let i in allCandidatedFromDB) {
        btnStr = `<div class='elementsInSameLine'>`;
        date2display = convertDBDate2FrontEndDate(allCandidatedFromDB[i].LatestDocumentedCallDate).toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });

        showDocumentedCallsBtn = '';
        showDocumentedCallsBtn += `<div class='btnWrapper-left'><span id="badgeOf_${allCandidatedFromDB[i].Id}" class="badge badge-pill badge-default">${allCandidatedFromDB[i].NoOfDocumentedCalls}</span>`;
        showDocumentedCallsBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id ="showDocumentedCallsBtn" title="שיחות מתועדות" data-backdrop="static" data-keyboard="false" data-toggle="modal" data-target="#DocumentedCallsModal"><i class="fa fa-phone" aria-hidden="true"></i></button></div>';

        btnStr += showDocumentedCallsBtn;

        showDocumentedRidesBtn = '';
        showDocumentedRidesBtn += `<div><span class="badge badge-default">${allCandidatedFromDB[i].NoOfDocumentedRides}</span>`;
        showDocumentedRidesBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id ="showDocumentedRidesBtn" title="תיעוד הסעות" data-toggle="modal" data-target="#documentedRidesModal"><i class="fa fa-car" aria-hidden="true"></i></button></div>';

        btnStr += showDocumentedRidesBtn;
        btnStr += '</div>';

        linkableNameToRender = buildCandidateHTML(allCandidatedFromDB[i]);

        thisCandidate = {
            Id: i,
            LinkableDisplayName: linkableNameToRender,
            DisplayName: allCandidatedFromDB[i].DisplayName,
            cellphone: addSeperator2MobileNum(allCandidatedFromDB[i].CellPhone, "-"),
            city: allCandidatedFromDB[i].City,// + '<br />בדיקה',
            daysSinceLastRide: allCandidatedFromDB[i].DaysSinceLastRide,
            numOfRides_last2Months: allCandidatedFromDB[i].NumOfRides_last2Months,
            daysUntilNextRide: allCandidatedFromDB[i].DaysUntilNextRide,
            latestDocumentedCallDate: date2display,
            seniorityInYears: allCandidatedFromDB[i].SeniorityInYears,
            score: parseInt(allCandidatedFromDB[i].Score),
            buttons: btnStr
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
        rowId: 'Id',
        pageLength: 10,
        stateSave: true,
        destroy: true,
        "lengthChange": false, // for somereason this property must be string
        stateDuration: 60 * 60,
        autoWidth: false,
        columns: [
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            {
                data: "LinkableDisplayName",
                render: function (data, type, row, meta) {
                    let did = "data-driverId='" + row.Id + "'";
                    return '<p class="c1" ' + did + '>' + data + ' </p>';
                }
            },                                                      //0
            { data: "cellphone" },                                  //1
            { data: "city" },                                       //2
            { data: "daysSinceLastRide" },                          //3
            { data: "numOfRides_last2Months" },                     //4
            { data: "daysUntilNextRide" },                          //5
            { data: "latestDocumentedCallDate" },                   //6
            { data: "seniorityInYears" },                           //7
            { data: "score" },                                      //8
            { data: "buttons" }                                     //9
        ],
        columnDefs: [
            { width: '20%', "targets": [0] },
            { width: '10%', "targets": [1] },
            { width: '15%', "targets": [2] },
            { width: '10%', "targets": [3] },
            { width: '5%', "targets": [4, 5, 7] },
            { width: '4%', "targets": [8] },
            { width: '10%', "targets": [6, 9] },
            { targets: [9], orderable: false }
        ]
    });

    superDriversTable = $('#datatable-superDrivers').DataTable(
        {
            data: superCandidated_clientVersion,
            rowId: 'Id',
            pageLength: 10,
            stateSave: true,
            destroy: true,
            "lengthChange": false, // for somereason this property must be string
            stateDuration: 60 * 60,
            autoWidth: false,
            columns: [
                //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
                {
                    data: "LinkableDisplayName",
                    render: function (data, type, row, meta) {
                        let did = "data-driverId='" + row.Id + "'";
                        return '<p class="c1" ' + did + '>' + data + ' </p>';
                    }
                },                                                      //0
                { data: "cellphone" },                                  //1
                { data: "city" },                                       //2
                { data: "daysSinceLastRide" },                          //3
                { data: "numOfRides_last2Months" },                     //4
                { data: "daysUntilNextRide" },                          //5
                { data: "latestDocumentedCallDate" },                   //6
                { data: "seniorityInYears" },                           //7
                { data: "score" },                                      //8
                { data: "buttons" }                                     //9

            ],
            columnDefs: [
                { width: '20%', "targets": [0] },
                { width: '10%', "targets": [1] },
                { width: '15%', "targets": [2] },
                { width: '10%', "targets": [3] },
                { width: '5%', "targets": [4, 5, 7] },
                { width: '4%', "targets": [8] },
                { width: '10%', "targets": [6, 9] },
                { targets: [9], orderable: false }
            ]
        });


    /*datatable - RegularDrivers*/
    /*
                     ============================
                     || ↑DATATABLES PROPERTIES↑||
                     ============================
    */
    //#endregion ↑DATATABLES PROPERTIES↑


}







const fillTableWithDataV2 = () => {


    //{
    //    "Id": 18143,
    //        "DisplayName": "ירון שיפוני",
    //            "CellPhone": "0526015432",
    //                "JoinDate": "01/01/2019 00:00:00",
    //                    "CityCityName": "בית קמה",
    //                        "AvailableSeats": 4,
    //                            "NoOfDocumentedRides": 0,
    //                                "SeniorityInYears": 5.728767,
    //                                    "LastCallDateTime": "19/08/2023 13:27:00",
    //                                        "Vtype": "NEWBIS",
    //                                            "LastRideInDays": null,
    //                                                "NextRideInDays": null,
    //                                                    "NumOfRidesLast2Month": 0,
    //                                                        "AmountOfRidesInThisPath": 0,
    //                                                            "AmountOfRidesInOppositePath": 0,
    //                                                                "AmountOfRides_OriginToArea": 0,
    //                                                                    "AmountOfRidesAtThisTime": 0,
    //                                                                        "AmountOfRidesAtThisDayWeek": 0,
    //                                                                            "SumOfKM": 223.152466,
    //                                                                                "Score": 2.300149
    //}

    let thisCandidate = {};
    regularCandidated_clientVersion = [];
    superCandidated_clientVersion = [];
    newCandidated_clientVersion = [];

    let date2display;
    let btnStr;
    let showDocumentedCallsBtn;
    let showDocumentedRidesBtn;
    let linkableNameToRender;


    for (let i in allCandidatedFromDB) {
        btnStr = `<div class='elementsInSameLine'>`;
        date2display = allCandidatedFromDB[i].LastCallDateTime

        showDocumentedCallsBtn = '';
        showDocumentedCallsBtn += `<div class='btnWrapper-left'><span id="badgeOf_${allCandidatedFromDB[i].Id}" class="badge badge-pill badge-default">${allCandidatedFromDB[i].NoOfDocumentedCalls}</span>`;
        showDocumentedCallsBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id ="showDocumentedCallsBtn" title="שיחות מתועדות" data-backdrop="static" data-keyboard="false" data-toggle="modal" data-target="#DocumentedCallsModal"><i class="fa fa-phone" aria-hidden="true"></i></button></div>';

        btnStr += showDocumentedCallsBtn;

        showDocumentedRidesBtn = '';
        showDocumentedRidesBtn += `<div><span class="badge badge-default">${allCandidatedFromDB[i].NoOfDocumentedRides}</span>`;
        showDocumentedRidesBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id ="showDocumentedRidesBtn" title="תיעוד הסעות" data-toggle="modal" data-target="#documentedRidesModal"><i class="fa fa-car" aria-hidden="true"></i></button></div>';

        btnStr += showDocumentedRidesBtn;
        btnStr += '</div>';

        linkableNameToRender = buildCandidateHTML(allCandidatedFromDB[i]);

        thisCandidate = {
            Id: allCandidatedFromDB[i].Id,
            LinkableDisplayName: linkableNameToRender,
            DisplayName: allCandidatedFromDB[i].DisplayName,
            cellphone: addSeperator2MobileNum(allCandidatedFromDB[i].CellPhone, "-"),
            city: allCandidatedFromDB[i].CityCityName,// + '<br />בדיקה',
            daysSinceLastRide: allCandidatedFromDB[i].LastRideInDays == null ? "אין" : allCandidatedFromDB[i].LastRideInDays,
            numOfRides_last2Months: allCandidatedFromDB[i].NumOfRidesLast2Month,
            daysUntilNextRide: allCandidatedFromDB[i].NextRideInDays == null ? "אין" : allCandidatedFromDB[i].NextRideInDays,
            latestDocumentedCallDate: date2display,
            seniorityInYears: allCandidatedFromDB[i].SeniorityInYears.toFixed(1),
            score: parseFloat(allCandidatedFromDB[i].Score.toFixed(2)),
            buttons: btnStr
        }

        switch (allCandidatedFromDB[i].Vtype) {

            case "NEWBIS":
                newCandidated_clientVersion.push(thisCandidate);
                break;
            case "REGULAR":
                regularCandidated_clientVersion.push(thisCandidate);
                break;
            case "SUPER":
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
        data: newCandidated_clientVersion,
        rowId: 'Id',
        pageLength: 10,
        stateSave: true,
        destroy: true,
        "lengthChange": false, // for somereason this property must be string
        stateDuration: 60 * 60,
        autoWidth: false,
        columns: [
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            {
                data: "LinkableDisplayName",
                render: function (data, type, row, meta) {
                    let did = "data-driverId='" + row.Id + "'";
                    return '<p class="c1" ' + did + '>' + data + ' </p>';
                }
            },                                                      //0
            { data: "cellphone" },                                  //1
            { data: "city" },                                       //2
            { data: "daysSinceLastRide" },                          //3
            { data: "numOfRides_last2Months" },                     //4
            { data: "daysUntilNextRide" },                          //5
            { data: "latestDocumentedCallDate" },                   //6
            { data: "seniorityInYears" },                           //7
            { data: "score" },                                      //8
            { data: "buttons" }                                     //9
        ],
        columnDefs: [
            { width: '20%', "targets": [0] },
            { width: '10%', "targets": [1] },
            { width: '15%', "targets": [2] },
            { width: '10%', "targets": [3] },
            { width: '5%', "targets": [4, 5, 7] },
            { width: '4%', "targets": [8] },
            { width: '10%', "targets": [6, 9] },
            { targets: [9], orderable: false }
        ]
    });

    regularTable = $('#datatable-RegularDrivers').DataTable({
        data: regularCandidated_clientVersion,
        rowId: 'Id',
        pageLength: 10,
        stateSave: true,
        destroy: true,
        "lengthChange": false, // for somereason this property must be string
        stateDuration: 60 * 60,
        autoWidth: false,
        columns: [
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            {
                data: "LinkableDisplayName",
                render: function (data, type, row, meta) {
                    let did = "data-driverId='" + row.Id + "'";
                    return '<p class="c1" ' + did + '>' + data + ' </p>';
                }
            },                                                      //0
            { data: "cellphone" },                                  //1
            { data: "city" },                                       //2
            { data: "daysSinceLastRide" },                          //3
            { data: "numOfRides_last2Months" },                     //4
            { data: "daysUntilNextRide" },                          //5
            { data: "latestDocumentedCallDate" },                   //6
            { data: "seniorityInYears" },                           //7
            { data: "score" },                                      //8
            { data: "buttons" }                                     //9
        ],
        columnDefs: [
            { width: '20%', "targets": [0] },
            { width: '10%', "targets": [1] },
            { width: '15%', "targets": [2] },
            { width: '10%', "targets": [3] },
            { width: '5%', "targets": [4, 5, 7] },
            { width: '4%', "targets": [8] },
            { width: '10%', "targets": [6, 9] },
            { targets: [9], orderable: false }
        ]
    });

    superDriversTable = $('#datatable-superDrivers').DataTable(
        {
            data: superCandidated_clientVersion,
            rowId: 'Id',
            pageLength: 10,
            stateSave: true,
            destroy: true,
            "lengthChange": false, // for somereason this property must be string
            stateDuration: 60 * 60,
            autoWidth: false,
            columns: [
                //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
                {
                    data: "LinkableDisplayName",
                    render: function (data, type, row, meta) {
                        let did = "data-driverId='" + row.Id + "'";
                        return '<p class="c1" ' + did + '>' + data + ' </p>';
                    }
                },                                                      //0
                { data: "cellphone" },                                  //1
                { data: "city" },                                       //2
                { data: "daysSinceLastRide" },                          //3
                { data: "numOfRides_last2Months" },                     //4
                { data: "daysUntilNextRide" },                          //5
                { data: "latestDocumentedCallDate" },                   //6
                { data: "seniorityInYears" },                           //7
                { data: "score" },                                      //8
                { data: "buttons" }                                     //9

            ],
            columnDefs: [
                { width: '20%', "targets": [0] },
                { width: '10%', "targets": [1] },
                { width: '15%', "targets": [2] },
                { width: '10%', "targets": [3] },
                { width: '5%', "targets": [4, 5, 7] },
                { width: '4%', "targets": [8] },
                { width: '10%', "targets": [6, 9] },
                { targets: [9], orderable: false }
            ]
        });


    /*datatable - RegularDrivers*/
    /*
                     ============================
                     || ↑DATATABLES PROPERTIES↑||
                     ============================
    */
    //#endregion ↑DATATABLES PROPERTIES↑


}
