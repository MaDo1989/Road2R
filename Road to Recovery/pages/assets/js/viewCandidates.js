checkCookie();
let { convertDBDate2FrontEndDate, getHebrew_WeekDay, addSeperator2MobileNum } = GENERAL.USEFULL_FUNCTIONS;
let { getRidePatNum4_viewCandidate } = GENERAL.RIDEPAT;
let { ajaxCall } = GENERAL.FETCH_DATA;
let { preDefinedContents } = GENERAL.DOCUMENTEDCALLS;
let allCandidatedFromDB;
let { COPYWRITE } = GENERAL;
let candidatesTable;
let regularCandidated_clientVersion = [];
let superCandidated_clientVersion = [];
let ridePatNum;
let documentedCallsTable;



const wiringDataTables = () => {
    //manage button clicks on tables

    $('#datatable-candidates tbody').on('click', '#showDocumentedCallsBtn', function () {

        $('#wait').show();

        let rowData = candidatesTable.row($(this).parents('tr')).data();

        let childrenof_td = this.parentElement.children;

        for (let i = 0; i < childrenof_td.length; i++) {
            if (childrenof_td[i].id.includes('badgeOf_')) {
                spanBadgeId = childrenof_td[i].id;
            }
        }

        thisVolunteerId = parseInt(rowData.id); //this variable is goobal to this page & used also in documentAcall2DB !!!

        $('#DocumentedCallsTitle').text("שיחות עם " + rowData.displayName)

        $.ajax({
            dataType: "json",
            url: "WebService.asmx/GetDocumentedCallsByDriverId",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            data: JSON.stringify({ driverId: thisVolunteerId }),
            success: function (data) {
                data = JSON.parse(data.d)
                if (documentedCallsTable != null) {
                    documentedCallsTable.destroy();
                }
                documentedCallsTable = $('#DocumentedCallsTable').DataTable({
                    order: [[4, "desc"]],
                    pageLength: 5,
                    data: data,
                    columns: [
                        {
                            data: (data) => {
                                return ConvertDBDate2UIDate(data.CallRecordedDate);
                            }
                        },
                        {
                            data: (data) => {
                                if (data.CallRecordedTime === undefined) return;
                                let time = data.CallRecordedTime;

                                let hh = time.Hours < 10 && time.Hours.toString().length === 1 ? "0" + time.Hours : time.Hours;
                                hh += ":";
                                let mm = time.Minutes < 10 && time.Minutes.toString().length === 1 ? "0" + time.Minutes : time.Minutes;
                                return hh + mm;
                            }
                        },
                        {
                            data: "CoordinatorName"
                        },
                        {
                            data: "CallContent"
                        }
                        ,
                        {
                            data: (data) => {
                                return convertDBDate2FrontEndDate(data.FullDateStemp);
                            }
                        }
                    ],
                    columnDefs: [
                        { "targets": [0], type: 'de_date' },
                        /*
                         Amir wanted a seperated columns to date and time but still sort by the full time stemps (or the acctual ticks)
                         so my (Yogev) soolution was to
                         1. fetch the fulltime stemp from the back-end
                         2. render and sort by this column
                         3. not showing it to the user
                          ↓*/
                        { "targets": [4], visible: false },
                        //↑

                    ]

                });
                $('#wait').hide();

            },
            error: function (err) {
                alert("Error in GetVolunteersRideHistory: " + err.responseText);
                $('#wait').hide();
            }
        });


    });

    $('#datatable-superDrivers tbody').on('click', '#showDocumentedCallsBtn', function () {

        $('#wait').show();

        let rowData = superDriversTable.row($(this).parents('tr')).data();

        let childrenof_td = this.parentElement.children;

        for (let i = 0; i < childrenof_td.length; i++) {
            if (childrenof_td[i].id.includes('badgeOf_')) {
                spanBadgeId = childrenof_td[i].id;
            }
        }

        thisVolunteerId = parseInt(rowData.id); //this variable is goobal to this page & used also in documentAcall2DB !!!

        $('#DocumentedCallsTitle').text("שיחות עם " + rowData.displayName)

        $.ajax({
            dataType: "json",
            url: "WebService.asmx/GetDocumentedCallsByDriverId",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            data: JSON.stringify({ driverId: thisVolunteerId }),
            success: function (data) {
                data = JSON.parse(data.d)
                if (documentedCallsTable != null) {
                    documentedCallsTable.destroy();
                }
                documentedCallsTable = $('#DocumentedCallsTable').DataTable({
                    order: [[4, "desc"]],
                    pageLength: 5,
                    data: data,
                    columns: [
                        {
                            data: (data) => {
                                return ConvertDBDate2UIDate(data.CallRecordedDate);
                            }
                        },
                        {
                            data: (data) => {
                                if (data.CallRecordedTime === undefined) return;
                                let time = data.CallRecordedTime;

                                let hh = time.Hours < 10 && time.Hours.toString().length === 1 ? "0" + time.Hours : time.Hours;
                                hh += ":";
                                let mm = time.Minutes < 10 && time.Minutes.toString().length === 1 ? "0" + time.Minutes : time.Minutes;
                                return hh + mm;
                            }
                        },
                        {
                            data: "CoordinatorName"
                        },
                        {
                            data: "CallContent"
                        }
                        ,
                        {
                            data: (data) => {
                                return convertDBDate2FrontEndDate(data.FullDateStemp);
                            }
                        }
                    ],
                    columnDefs: [
                        { "targets": [0], type: 'de_date' },
                        /*
                         Amir wanted a seperated columns to date and time but still sort by the full time stemps (or the acctual ticks)
                         so my (Yogev) soolution was to
                         1. fetch the fulltime stemp from the back-end
                         2. render and sort by this column
                         3. not showing it to the user
                          ↓*/
                        { "targets": [4], visible: false },
                        //↑

                    ]

                });
                $('#wait').hide();

            },
            error: function (err) {
                alert("Error in GetVolunteersRideHistory: " + err.responseText);
                $('#wait').hide();
            }
        });


    });

 
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
    let boldArr = ["נסיעות בציר המדויק הזה", "הסעות ביום הזה", "הסעות בחלק הנדרש של היום"];
    let txt = {
        "נסיעות בציר המדויק הזה": c.AmmountOfPathMatch[4],
        "נסיעות בציר ההפוך": c.AmmountOfPathMatch[3],
        "נסיעות מאותה נקודה לאותו איזור": c.AmmountOfPathMatch[2],
        "נסיעות בין אותם איזורים": c.AmmountOfPathMatch[1],
        "נסיעות נוספות": c.AmmountOfPathMatch[0],
        "הסעות ביום הזה": c.AmmountOfMatchByDay,
        "הסעות בימים אחרים": c.AmmountOfDisMatchByDay,
        "הסעות בחלק הנדרש של היום": c.AmmountOfMatchDayPart,
        "הסעות בחלקו השני של היום": c.AmmountOfDisMatchDayPart
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
    $("#characteristics").css("visibility", "hidden");

    $(document).on("mouseover", ".c1", showCharacteristics);
    $(document).on("mouseout", ".c1", hideCharacteristics);

    candidatesTable = $('#datatable-candidates').DataTable({ data: [], destroy: true });
    superDriversTable = $('#datatable-superDrivers').DataTable({ data: [], destroy: true });
    //newDriversTable   = $('#datatable-newDrivers').DataTable({ data: [], destroy: true });

    ridePatNum = JSON.parse(getRidePatNum4_viewCandidate());
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
    let btnStr;
    for (let i in allCandidatedFromDB) {
        btnStr = '';
        date2display = convertDBDate2FrontEndDate(allCandidatedFromDB[i].LatestDocumentedCallDate).toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });

        let showDocumentedCallsBtn = '';
        showDocumentedCallsBtn += `<div class='btnWrapper-left'><span id="badgeOf_${allCandidatedFromDB[i].Id}" class="badge badge-pill badge-default">${allCandidatedFromDB[i].NoOfDocumentedCalls}</span>`;
        showDocumentedCallsBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id ="showDocumentedCallsBtn" title="שיחות מתועדות" data-backdrop="static" data-keyboard="false" data-toggle="modal" data-target="#DocumentedCallsModal"><i class="fa fa-phone" aria-hidden="true"></i></button></div>';

        btnStr += showDocumentedCallsBtn;

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
        rowId: 'id',
        pageLength: 10,
        stateSave: true,
        destroy: true,
        "lengthChange": false, // for somereason this property must be string
        stateDuration: 60 * 60,
        autoWidth: false,
        columns: [
            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
            {
                data: "displayName",
                render: function (data, type, row, meta) {
                    let did = "data-driverId='" + row.id + "'";
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
            { width: '10%', "targets": [6, 9] }
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
                {
                    data: "displayName",
                    render: function (data, type, row, meta) {
                        let did = "data-driverId='" + row.id + "'";
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
                { width: '10%', "targets": [6, 9] }
            ]
        });

    //newDriversTable = $('#datatable-newDrivers').DataTable(
    //    {
    //        data: newCandidated_clientVersion,
    //        rowId: 'id',
    //        pageLength: 10,
    //        stateSave: true,
    //        destroy: true,
    //        "lengthChange": false, // for somereason this property must be string
    //        stateDuration: 60 * 60,
    //        autoWidth: false,
    //        columns: [
    //            //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
    //            {
    //                data: "displayName",
    //                render: function (data, type, row, meta) {
    //                    let did = "data-driverId='" + row.id + "'";
    //                    return '<p class="c1" ' + did + '>' + data + ' </p>';
    //                }
    //            },                                                      //0
    //            { data: "cellphone" },                                  //1
    //            { data: "city" },                                       //2
    //            { data: "daysSinceLastRide" },                          //3
    //            { data: "numOfRides_last2Months" },                     //4
    //            { data: "daysUntilNextRide" },                          //5
    //            { data: "latestDocumentedCallDate" },                   //6
    //            { data: "seniorityInYears" },                           //7
    //            { data: "score" },                                      //8
    //            { data: "buttons" }                                     //9

    //        ],
    //        columnDefs: [
    //            { width: '20%', "targets": [0] },
    //            { width: '10%', "targets": [1] },
    //            { width: '15%', "targets": [2] },
    //            { width: '10%', "targets": [3] },
    //            { width: '5%', "targets": [4, 5, 7] },
    //            { width: '4%', "targets": [8] },
    //            { width: '10%', "targets": [6, 9] }
    //        ]
    //    });

    /*
                     ============================
                     || ↑DATATABLES PROPERTIES↑||
                     ============================
    */
    //#endregion ↑DATATABLES PROPERTIES↑


}


const openDocumentAcallModal = () => {
    $('#DocumentAcallsModal').modal('show');

    $('.closeBtn_DocumentAcallsModal').prop('disabled', true);
    $('#saveCallBtn').prop('disabled', false);
    $('#cancellCallBtn').prop('disabled', false);


    //get a list of all coordinators ()
    let allCordinatorsFromDB = [];
    let loggedInUser = {};
    loggedInUser.DisplayName = localStorage.getItem('userCell');
    loggedInUser.UserType = localStorage.getItem('userType');
    $('#DocumentAcall_writeContent').val('');
    $('#DocumentAcall_choooseContent').prop('disabled', false);
    $('#DocumentAcall_writeContent').prop('disabled', false);


    $('#DocumentAcall_contentErrorMsg').hide();
    $('#DocumentAcall_CoordinatorErrorMsg').hide();


    $('#wait').show();
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getCoordinatorsList_version_02",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        success: function (data) {
            $('#wait').hide();
            allCordinatorsFromDB = JSON.parse(data.d);
            let cordinatorsOptions = '<option value="not selected">בחר.י רכז.ת</option>';

            for (var i = 0; i < allCordinatorsFromDB.length; i++) {
                cordinatorsOptions += `<option value="${allCordinatorsFromDB[i].Id}"`;
                cordinatorsOptions += loggedInUser.UserType === 'רכז' &&
                    allCordinatorsFromDB[i].DisplayName === loggedInUser.DisplayName ? 'selected>' : '>';
                cordinatorsOptions += `${allCordinatorsFromDB[i].DisplayName}</option >`;
            }
            document.getElementById('DocumentAcall_choooseCoordinator').innerHTML = cordinatorsOptions;


            let contentOptions = `<option value="${preDefinedContents[0].key}" selected>${preDefinedContents[0].value}</option>`;
            //could be also hidden if needed ↑
            for (var i = 1; i < preDefinedContents.length; i++) {
                contentOptions += `<option value="${preDefinedContents[i].key}">${preDefinedContents[i].value}</option>`;
            }
            document.getElementById('DocumentAcall_choooseContent').innerHTML = contentOptions;



            let now = new Date();
            document.getElementById('DocumentAcallDatePicker').valueAsDate = now; //sets the default date for today

            let time = new Object();
            time.hours = now.getHours() < 10 ? '0' + now.getHours() : now.getHours();
            time.minutes = now.getMinutes() < 10 ? '0' + now.getMinutes() : now.getMinutes();

            document.getElementById('DocumentAcallTime').value = `${time.hours}:${time.minutes}`;


            $('#documentAcallPlusBtn').prop('disabled', true);
        },
        error: function (err) {
            alert("Error in GetCoordinatorsList: " + err.responseText);
            $('#wait').hide();
        }
    });
}

const cancelCallDocument = () => {
    $('#saveCallBtn').prop('disabled', true);
    $('#cancellCallBtn').prop('disabled', true);
    $('#DocumentAcallsModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAcallPlusBtn').prop('disabled', false);

}

const DocumentedAcall_writeContent_clicked = () => {
    $('#DocumentAcall_contentErrorMsg').hide();
    $('#saveCallBtn').prop('disabled', false);
}

const DocumentAcall_writeContent_blured = () => {

    let content = $('#DocumentAcall_writeContent').val();

    if (content.length > 0) {
        $('#DocumentAcall_choooseContent').prop('disabled', true)
    } else {
        $('#DocumentAcall_choooseContent').prop('disabled', false);
    }
}

const DocumentAcall_choooseCoordinator_changed = () => {

    if ($('#DocumentAcall_choooseCoordinator').val() !== "not selected") {
        $('#DocumentAcall_CoordinatorErrorMsg').hide();
        $('#saveCallBtn').prop('disabled', false);
    }
}

const DocumentAcall_choooseContent_changed = () => {

    let preDefinedContents_key = parseInt($('#DocumentAcall_choooseContent').val());

    if (preDefinedContents_key > 0) {
        $('#DocumentAcall_writeContent').prop('disabled', true)
        $('#DocumentAcall_contentErrorMsg').hide();
        $('#saveCallBtn').prop('disabled', false);
    } else {
        $('#DocumentAcall_writeContent').prop('disabled', false);

    }
}

const documentAcall2DB = () => {
    $('#saveCallBtn').prop('disabled', true);

    let documentedCall = {};
    documentedCall.DriverId = thisVolunteerId;
    documentedCall.CoordinatorId = parseInt($('#DocumentAcall_choooseCoordinator').val());
    documentedCall.CallRecordedDate = $('#DocumentAcallDatePicker').val();
    documentedCall.CallRecordedTime = $('#DocumentAcallTime').val();

    let preDefinedContents_key = parseInt($('#DocumentAcall_choooseContent').val());
    let content2pass2DB = '';
    if (preDefinedContents_key === 0) {
        content2pass2DB = $('#DocumentAcall_writeContent').val();
    } else {

        content2pass2DB += preDefinedContents[preDefinedContents_key].value;
    }
    documentedCall.CallContent = content2pass2DB;

    if (!documentedCall.CoordinatorId) {
        $('#DocumentAcall_CoordinatorErrorMsg').show()
        $('#DocumentAcall_contentErrorMsg').hide();
        return;
    }

    if (documentedCall.CallContent.length === 0) {
        $('#DocumentAcall_contentErrorMsg').show();
        $('#DocumentAcall_CoordinatorErrorMsg').hide()
        return;
    }

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/DocumentNewCall",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ documentedCall }),
        success: function (data) {
            // ↓ front-end cheating ↓

            //step 1: show the change on the grid right away

            /*
             in order to add row to this dataTable you should

             1. convert callRecordedDate to the form of → "/Date(1607810400000)/"
             2. convert callRecordedTime to the form of → time object with hours and minutes properties

               or else you will suffer from various errors
             */
            let documentedCallsTable = $('#DocumentedCallsTable').DataTable();
            documentedCall.CoordinatorName = $("#DocumentAcall_choooseCoordinator option:selected").text();



            let timeOfCall = documentedCall.CallRecordedTime;
            let dateOfCall = documentedCall.CallRecordedDate;

            let convertable_CallRecordedFullDateStemp = new Date(`${dateOfCall}, ${timeOfCall}`);
            convertable_CallRecordedFullDateStemp = convertable_CallRecordedFullDateStemp.getTime();
            documentedCall.FullDateStemp = `/Date(${convertable_CallRecordedFullDateStemp})/`;

            let convertable_CallRecordedDate = new Date(documentedCall.CallRecordedDate);
            convertable_CallRecordedDate = convertable_CallRecordedDate.getTime();
            documentedCall.CallRecordedDate = `/Date(${convertable_CallRecordedDate})/`;

            let convertable_CallRecordedTime = {
                Hours: documentedCall.CallRecordedTime[0] + documentedCall.CallRecordedTime[1],
                Minutes: documentedCall.CallRecordedTime[3] + documentedCall.CallRecordedTime[4]
            };


            let addRowData = {
                CallRecordedDate: documentedCall.CallRecordedDate,
                CallRecordedTime: convertable_CallRecordedTime,
                CoordinatorName: documentedCall.CoordinatorName,
                CallContent: documentedCall.CallContent,
                FullDateStemp: documentedCall.FullDateStemp
            }

            documentedCallsTable.row.add(addRowData).draw(false);

            //step 2: add +1 to cals badge

            document.getElementById(spanBadgeId).innerHTML = parseInt(document.getElementById(spanBadgeId).innerHTML) + 1;

            // ↑ front-end cheating ↑

        },
        error: function (err) { alert("Error in DocumentNewCall: " + err.responseText); }
    });

    $('#DocumentAcallsModal').modal('toggle');
    $('.closeBtn_DocumentAcallsModal').prop('disabled', false);
    $('#documentAcallPlusBtn').prop('disabled', false);

}

