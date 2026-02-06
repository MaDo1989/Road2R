var cleanPhoneNumber;
var msgTable;
let { convertDBDate2FrontEndDate, datetimeCompareFunc, dateCompareFunc } = GENERAL.USEFULL_FUNCTIONS;
let columnsVisibiltyFlags = [];
const columnsVisibiltyFlagsName = 'columnsVisibiltyFlags';

checkCookie();
var allVolunteersFromDb = [];
let activeChangedFlag = false;
$(document).ready(function () {

    $('#skipPageButton').click(function () {
        var pageNumber = parseInt($('#skipPageInput').val());
        tbl.page(pageNumber - 1).draw('page');
        $('#skipPageInput').val("")
    });

    $("#DocumentedCallsModal").attr("w3-include-html", "DocumentedCallsModal.html");
    $("#documentedRidesModal").attr("w3-include-html", "documentedRidesModal.html");
    $("#DocumentedAbsenceModal").attr("w3-include-html", "documentedAbsenceModal.html");


    let { COPYWRITE } = GENERAL;
    $('#rights').html(COPYWRITE());
    const parentDiv = document.querySelector("#columnsVisibiltyContainer");
    const labels = parentDiv.querySelectorAll("label");
    const checkboxes = parentDiv.querySelectorAll("input[type='checkbox']");

    if (localStorage.hasOwnProperty(columnsVisibiltyFlagsName)) {

        columnsVisibiltyFlags = JSON.parse(localStorage.getItem(columnsVisibiltyFlagsName));

        checkboxes.forEach((checkbox) => {

            const key = parseInt(checkbox.dataset.column);
            checkbox.checked = columnsVisibiltyFlags[key];
        });

    } else {
        columnsVisibiltyFlags = [];

        labels.forEach((label) => {
            const key = parseInt(label.previousElementSibling.dataset.column);

            columnsVisibiltyFlags[key] = true;
        });

        checkboxes.forEach((checkbox) => {

            const key = parseInt(checkbox.dataset.column);
            checkbox.checked = columnsVisibiltyFlags[key];
        });


        localStorage.setItem(columnsVisibiltyFlagsName, JSON.stringify(columnsVisibiltyFlags));
    }


    if (localStorage.activeVol == "undefined" || localStorage.activeVol === undefined)
        localStorage.activeVol = true;

    if (localStorage.driveVol == "undefined" || localStorage.driveVol === undefined)
        localStorage.driveVol = true;

    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.toLowerCase().indexOf('test') != -1) {
        $("#na").css("background-color", "#ffde89");
    }
    if (window.location.href.indexOf('http://40.117.122.242/Road%20to%20Recovery/') != -1) {
        window.location.href = "notAvailable.html";
    }
    includeHTML();
    var userName = GENERAL.USER.getUserDisplayName();
    $("#userName").html(userName);
    $("#closeModal").click(function () {

    });


    refreshTable();
    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "de_date-asc": function (a, b) {

            return dateCompareFunc(a, b, true);
        },
        "de_date-desc": function (a, b) {

            return dateCompareFunc(a, b, false);
        },
        "de_datetime-asc": function (a, b) {

            return datetimeCompareFunc(a, b, true);
        }, "de_datetime-desc": function (a, b) {

            return datetimeCompareFunc(a, b, false);
        }
    });




});

const updateLsVisibilty = (checkboxId, value) => {

    if (localStorage.hasOwnProperty(columnsVisibiltyFlagsName)) {

        columnsVisibiltyFlags = JSON.parse(localStorage.getItem(columnsVisibiltyFlagsName));
        columnsVisibiltyFlags[checkboxId] = value;
        localStorage.setItem(columnsVisibiltyFlagsName, JSON.stringify(columnsVisibiltyFlags));
    }
}

function ActiveMode() {
    localStorage.activeVol = $("#activeCustomers").prop('checked');
    filterAndDisplayTable();
}

const driveMode = () => {
    localStorage.driveVol = $('#drivingVolunteers').prop('checked');
    filterAndDisplayTable();
}

function refreshTable() {
    var activeVol = JSON.parse(localStorage.activeVol);
    $("#activeCustomers").prop('checked', activeVol);

    let driveVol = JSON.parse(localStorage.driveVol);
    $("#drivingVolunteers").prop('checked', driveVol);


    allVolunteersFromDb = [];
    $('#wait').show();
    checkCookie();

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getVolunteers_Gilad",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ active: true }),
        success: function (data) {

            $('#wait').hide();
            var arr_volunteers = JSON.parse(data.d);

            GENERAL.VOLUNTEERS.setVolunteersList(JSON.stringify(arr_volunteers));

            allVolunteersFromDb = [];

            for (i in arr_volunteers) {
                var btnStr = '';
                var editBtn = "<div class='btnWrapper-right'><button type='button' class='btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5' id='edit' title='עריכה'><i class='ti-pencil'></i></button></div>";

                btnStr += editBtn;
                let deleteBtn = '';
                if (arr_volunteers[i].IsActive == false) {
                    btnStr = "";
                    var reactiveBtn = "<div class='btnWrapper-right'><button type='button' class='btn btn-icon waves-effect waves-light btn-warning btn-sm m-b-5' id='reactive' title='שחזור'><i class='fa fa-undo'></i></button></div>";
                    btnStr += " " + reactiveBtn;
                }
                else {
                    deleteBtn = "<div class='btnWrapper-left'><button  type='button' class='btn btn-icon waves-effect waves-light btn-danger btn-sm m-b-5' id='remove' title='מחיקה'><i class='fa fa-remove'></i></button></div>";
                    var messagesBtn = "<div class='btnWrapper-right'><button type='button' disabled class='btn btn-icon waves-effect waves-light btn-light btn-sm m-b-5' id='messages' title='הודעות' data-toggle='modal' data-target='#messagesModal'><i class='fa fa-envelope-o'></i></button></div>";
                }
                var cellphone2 = "";
                if (arr_volunteers[i].CellPhone2 != "") {
                    cellphone2 = arr_volunteers[i].CellPhone2;
                }
                else cellphone2 = arr_volunteers[i].HomePhone;

                let showDocumentedRidesBtn = '';
                showDocumentedRidesBtn += `<div class='btnWrapper-left'><span class="badge badge-default">${arr_volunteers[i].No_of_rides}</span>`;
                showDocumentedRidesBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id="showDocumentedRidesBtn" title="תיעוד הסעות" data-toggle="modal" data-target="#documentedRidesModal"><i class="fa fa-car" aria-hidden="true"></i></button></div>';
                btnStr += showDocumentedRidesBtn;




                let showDocumentedCallsBtn = '';
                showDocumentedCallsBtn += `<div class='btnWrapper-left'><span id="badgeOf_${arr_volunteers[i].Id}" class="badge badge-pill badge-default">${arr_volunteers[i].NoOfDocumentedCalls}</span>`;
                showDocumentedCallsBtn += '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id="showDocumentedCallsBtn" title="שיחות מתועדות" data-backdrop="static" data-keyboard="false" data-toggle="modal" data-target="#DocumentedCallsModal"><i class="fa fa-phone" aria-hidden="true"></i></button></div>';
                if (arr_volunteers[i].AbsenceStatus) {
                    showDocumentedCallsBtn = `<div class='btnWrapper-left'><span id="badgeOf_${arr_volunteers[i].Id}" class="badge badge-pill badge-default">${arr_volunteers[i].NoOfDocumentedCalls}</span>`;
                    showDocumentedCallsBtn += '<button style="background-color:#efa834 !important;border: 1px solid #efa834 !important;" type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5" id="showDocumentedCallsBtn" title="שיחות מתועדות" data-backdrop="static" data-keyboard="false" data-toggle="modal" data-target="#DocumentedCallsModal"><i class="fa fa-phone" aria-hidden="true"></i></button></div>';
                }
                btnStr += showDocumentedCallsBtn;


                btnStr += " " + deleteBtn;

                let lastModified = convertDBDate2FrontEndDate(arr_volunteers[i].DateTime_LastModified);
                lastModified = lastModified.toLocaleString('he-IL', { dateStyle: 'short', timeStyle: 'short' });


                let joinDate = convertDBDate2FrontEndDate(arr_volunteers[i].JoinDate);
                joinDate = joinDate.toLocaleString('he-IL', { dateStyle: 'short' });

                let latestDrive = convertDBDate2FrontEndDate(arr_volunteers[i].LatestDrive);
                latestDrive = latestDrive === null ? '' : latestDrive.toLocaleString('he-IL', { dateStyle: 'short' });



                let volunteerTypeAndGender = buildvolunteerTypeAndGenderHTML(arr_volunteers[i].TypeVol, arr_volunteers[i].Gender);
                let englishNameVolunteer = ''
                if (arr_volunteers[i].EnglishName != '') {
                    englishNameVolunteer = ' <br> ' + arr_volunteers[i].EnglishName;
                }

                if (volunteerTypeAndGender == "רכזת") {
                    volunteerTypeAndGender = "מתאמת"
                }
                if (volunteerTypeAndGender == "רכז") {
                    volunteerTypeAndGender = "מתאם"
                }

                var Volunteer = {
                    Id: arr_volunteers[i].Id,
                    DisplayName: arr_volunteers[i].DisplayName + englishNameVolunteer,
                    TypeVol: volunteerTypeAndGender,
                    CellPhone: arr_volunteers[i].CellPhone,
                    CellPhone2: cellphone2,
                    City: arr_volunteers[i].City,
                    Remarks: arr_volunteers[i].Remarks,
                    Buttons: btnStr,
                    LastModified: lastModified,
                    NumOfRides_last2Months: arr_volunteers[i].NumOfRides_last2Months,
                    LatestDrive: latestDrive,
                    MostCommonPath: arr_volunteers[i].MostCommonPath,
                    NearestBigCity: arr_volunteers[i].NearestBigCity,
                    JoinDate: joinDate,
                    AvailableSeats: arr_volunteers[i].AvailableSeats == 0 ? 'לא ידוע' : arr_volunteers[i].AvailableSeats,
                    isBooster: arr_volunteers[i].IsBooster ? '✅' : '❌',
                    isBabySeat: arr_volunteers[i].IsBabySeat ? '✅' : '❌',
                    IsActive: arr_volunteers[i].IsActive,
                    IsDriving: arr_volunteers[i].IsDriving
                }
                allVolunteersFromDb.push(Volunteer);
            }

            filterAndDisplayTable();

            if (activeChangedFlag == false) {
                $('#DocumentedCallsModal .modal-body')[0].innerHTML += `<button onclick="ManageAbsenceToggle(this)" class="AbsenceBtn btn waves-effect btn-primary">ניהול היעדרויות</button>`;
            }
        },
        error: function (err) {
            alert("Error in getVolunteers: " + err.responseText);
            $('#wait').hide();
        }
    });

    document.querySelectorAll('input.toggle-vis').forEach((el) => {
        el.addEventListener('change', function (e) {
            e.preventDefault();
            let columnIdx = e.target.getAttribute('data-column');
            let column = tbl.column(columnIdx);

            column.visible(e.target.checked);
        });
    });
}

function filterAndDisplayTable() {
    var activeVol = JSON.parse(localStorage.activeVol);
    let driveVol = JSON.parse(localStorage.driveVol);

    let filteredData = allVolunteersFromDb.filter(volunteer => {
        let passActiveFilter = true;
        let passDriveFilter = true;

        if (activeVol === true) {
            passActiveFilter = volunteer.IsActive === true;
        }

        if (driveVol === true) {
            passDriveFilter = volunteer.IsDriving === true;
        }

        return passActiveFilter && passDriveFilter;
    });

    if (typeof tbl !== 'undefined' && tbl !== null) {
        tbl.clear();
        tbl.rows.add(filteredData);
        tbl.draw();
    } else {
        tbl = $('#datatable-buttons').DataTable({
            data: filteredData,
            stateSave: true,
            rowId: 'Id',
            "autoWidth": false,
            "destroy": true,
            dom: 'Bfrtip',
            buttons: [
                'excel',
            ],
            columnDefs: [
                {
                    targets: 5,
                    className: 'dt-body-left'
                },
                {
                    targets: 3,
                    className: 'text-left-strong'
                },
                { "targets": [14], "orderable": false },
                { targets: 6, type: 'de_datetime' },
                { targets: 8, type: 'de_date' },
                { targets: 10, type: 'de_date' },
                {
                    targets: 11,
                    render: function (data, type, row) {
                        if (type === 'sort') {
                            if (data === "לא ידוע") {
                                return 999999;
                            }
                            return parseInt(data, 10) || 0;
                        }
                        return data;
                    },
                    type: 'numeric'
                }
            ],
            columns: [
                { data: "DisplayName", width: "1%" },
                { data: "TypeVol", width: "1%" },
                { data: "CellPhone", width: "2%" },
                { data: "City", width: "2%" },
                { data: "NearestBigCity", width: "4%" },
                { data: "Remarks", width: "7%" },
                { data: "LastModified", width: "7%" },
                { data: "NumOfRides_last2Months", width: "2%" },
                { data: "LatestDrive", width: "2%" },
                { data: "MostCommonPath", width: "3%" },
                { data: "JoinDate", width: "4%" },
                { data: "AvailableSeats", width: "4%" },
                { data: "isBooster", width: "1%" },
                { data: "isBabySeat", width: "1%" },
                { data: "Buttons", width: "25%" },
            ],
        });

        buttonsEvents();
    }
}

const buildvolunteerTypeAndGenderHTML = (type, gender) => {

    let output = type + ' ' + gender;
    const isMale = gender === 'מתנדב';
    const taf = 'ת';

    if (isMale) {

        output = type;
    } else {

        output = type + taf;
    }

    return output;
}

const ConvertDBDate2UIDate = (fullTimeStempStr) => {
    if (fullTimeStempStr === undefined) return;
    let startTrim = fullTimeStempStr.indexOf('(') + 1;
    let endTrim = fullTimeStempStr.indexOf(')');
    let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
    let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));

    let dd = fullTimeStemp.getDate();

    let mm = fullTimeStemp.getMonth() + 1;

    let yyyy = fullTimeStemp.getFullYear();

    return `${dd}.${mm}.${yyyy}`;
}

const ConvertDBDate2UIFullStempDate = (dateAsStr) => {
    if (dateAsStr === undefined) return;
    let startTrim = dateAsStr.indexOf('(') + 1;
    let endTrim = dateAsStr.indexOf(')');
    let fullTimeStempNumber = dateAsStr.substring(startTrim, endTrim);
    let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));
    return fullTimeStemp;
}

const ConvertDBDate2PickerFormat = (dateStr) => {
    let str = ConvertDBDate2UIDate(dateStr);
    str = str.replace(".", "-").replace(".", "-");
    str = ReverseString(str);

    let split = str.split("-");
    if (split[1].length == 1) {
        split[1] = '0' + split[1];
    }
    if (split[2].length == 1) {
        split[2] = '0' + split[2];
    }
    return `${split[0]}-${split[1]}-${split[2]}`;

}

function ReverseString(str) {
    return str.split('-').reverse().join('-')
}

function buttonsEvents() {

    $('#datatable-buttons tbody').on('click', '#edit', function () {
        var data = tbl.row($(this).parents('tr')).data();
        arr_details = { displayName: data.DisplayName, func: "edit" };
        GENERAL.VOLUNTEERS.setVolunteersList(JSON.stringify(arr_details));
        location.href = "volunteerForm.html";
    });

    $('#datatable-buttons tbody').on('click', '#show', function () {
        var data = tbl.row($(this).parents('tr')).data();
        arr_details = { displayName: data.DisplayName, func: "show" };
        GENERAL.VOLUNTEERS.setVolunteersList(JSON.stringify(arr_details));
        location.href = "volunteerForm.html";
    });

    $('#datatable-buttons tbody').on('click', '#remove', function () {
        var data = tbl.row($(this).parents('tr')).data();
        swal({
            title: "האם אתם בטוחים?",
            type: "warning",
            text: "הפיכת המתנדב " + data.DisplayName.split('<br>')[0] + " ללא פעיל",
            showCancelButton: true,
            cancelButtonText: "בטל",
            confirmButtonClass: 'btn-warning',
            confirmButtonText: "מחיקה",
            closeOnConfirm: false
        }, function () {
            deactivateVolunteer(data.DisplayName, 'false');
            setTimeout(function () { refreshTable() }, 1001);
        });
    });

    $('#datatable-buttons tbody').on('click', '#reactive', function () {
        var data = tbl.row($(this).parents('tr')).data();
        swal({
            title: "האם אתם בטוחים?",
            type: "warning",
            text: "שחזור המתנדב " + data.DisplayName.split("<br>")[0],
            showCancelButton: true,
            cancelButtonText: "בטל",
            confirmButtonClass: 'btn-warning',
            confirmButtonText: "שחזר",
            closeOnConfirm: false
        }, function () {
            deactivateVolunteer(data.DisplayName, 'true');
            swal({
                title: "המתנדב הפך לפעיל",
                timer: 1000,
                type: "success",
                showConfirmButton: false
            });
            setTimeout(function () { refreshTable() }, 1001);
        });

    });

    $('#datatable-buttons tbody').on('click', '#showDocumentedCallsBtn', function () {

        manipulateDocumentedCallsModal(this, tbl);

    });

    $('#datatable-buttons tbody').on('click', '#showDocumentedRidesBtn', function () {

        manipulateDocumentedRidesModal(this, tbl);

    });

    $('#datatable-buttons tbody').on('click', '#messages', function () {

        $('#wait').show();

        var rowData = tbl.row($(this).parents('tr')).data();


        $('#messagesModalTitle').text("היסטוריית הודעות - " + rowData.DisplayName.split('<br>')[0])
        $.ajax({
            dataType: "json",
            url: "WebService.asmx/getMessages",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            data: JSON.stringify({ displayName: rowData.DisplayName.split('<br>')[0] }),
            success: function (data) {
                var data = JSON.parse(data.d)
                if (msgTable != null) {

                    msgTable.destroy();
                }

                msgTable = $('#messagesTable').DataTable({
                    columnDefs: [
                        { "targets": 3, className: 'text-left-strong' },
                    ],
                    order: [[1, "desc"]],
                    pageLength: 5,
                    data: data,
                    columns: [
                        { data: "DateTime" },
                        { data: "MsgID" },
                        { data: "ParentID" },
                        { data: "Type" },
                        { data: "Title" },
                        { data: "MsgContent" },
                        { data: "Sender" },
                    ]
                });
                $('#wait').hide();

            },
            error: function (err) {
                alert("Error in getVolunteer: " + err.responseText);
                $('#wait').hide();
            }
        });

    });
}

function deactivateVolunteer(displayName, active) {
    checkCookie();

    displayName = displayName.split('<br>')[0];
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/SetVolunteerIsActive",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ displayName: displayName, active: active }),
        success: function (data) {

            const response = JSON.parse(data.d);

            if (response.IsSuccess) {
                swal({
                    title: active != "true" ? 'המתנדב הפך ללא פעיל' : 'המתנדב הפך לפעיל',
                    timer: 1000,
                    type: "success",
                    showConfirmButton: false
                });
            } else {
                swal({
                    title: "הפעולה לא בוצעה " + response.Reason,
                    type: "warning",
                });
            }

            $('#wait').hide();
        },
        error: function (err) { alert("Error in deactivateVolunteer: " + err.responseText); }
    });

}

function newVolunteerForm() {
    displayName = " ";
    arr_details = { displayName: displayName, func: "new" };
    GENERAL.VOLUNTEERS.setVolunteersList(JSON.stringify(arr_details));
    location.href = "volunteerForm.html";
}

const beforeDeactiveCheckFutureRides = (volunteer) => {
    $('#wait').show();
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/CheckFutureRides",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: volunteer.Id }),
        success: function (data) {

            const response = JSON.parse(data.d);

            if (response == 0) {
                deactivateVolunteer(volunteer.DisplayName, 'false');
                setTimeout(function () { refreshTable() }, 1001);
            }
            else if (response == 1) {
                swal({
                    title: "שימו לב !",
                    type: "warning",
                    text: "לתשומת ליבך למתנדב זה קיימות נסיעות עתידיות, האם עדיין ברצונך להמשיך בפעולה?",
                    showCancelButton: true,
                    cancelButtonText: "בטל",
                    confirmButtonClass: 'btn-warning',
                    confirmButtonText: "מחיקה",
                    closeOnConfirm: false
                }, function () {
                    deactivateVolunteer(volunteer.DisplayName, 'false');
                    setTimeout(function () { refreshTable() }, 1001);
                });
            }


            $('#wait').hide();
        },
        error: function (err) {
            alert("Error in beforeDeactiveCheckFutureRides: " + err.responseText);
            $('#wait').hide();
        }
    });
}

const ManageAbsenceToggle = (btn) => {
    AbsenceBtn = btn;
    $('#DocumentedCallsModal').modal('toggle');
    $('#DocumentedAbsenceModal').modal('toggle');
}