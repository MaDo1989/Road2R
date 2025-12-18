/*
NOTICE THIS:
אחה"צ
calculation   wise: its value is 12:00 (like when we calaulate hours gap between the present and a given ridepat)
table sorting wise: its value is Infinity or negetive Infinity
*/
checkCookie();
var destinationName = "";
var originName = "";
var driverName = "";
var patientName = "";
var Origin_Dest = "";
var isAnonymous = false;
var isAssistant = false;
var refreshTime;
var refreshInterval;
var tomorrowDate = new Date()
tomorrowDate.setDate(tomorrowDate.getDate() + 1)
var hospitals = [];
var returnRidePat;
var returnRidePatMsg = ""
var dateReturn;
var returnDriverMsg;

let checkboxesRides = [];
let checkboxFlag = false;

let arrayOF_areasObj = [];
let morningRidePats = [];
let afterNoonRidePats = [];
let tomorrowMorningRidePats = [];
let tomorrowAfternoonRidePats = [];
let futureRidePats = [];
let allReadyCountedRidePats = [];
let candidate4IhaveSeen;
let timer;
let arr_rides;

const {

    validateMobileNumFullVersion,
    addSeperator2MobileNum,
    convertDBDate2FrontEndDate,
    getHoursGap,
    convert2DBDateToInt,
    getHebrew_WeekDay,
    showMe,
    dateCompareFunc,
    datetimeCompareFunc,
    isDate,
    IsNullOrUndefined

} = GENERAL.USEFULL_FUNCTIONS;
const DateMode = { Today: 0, Tomorrow: 1, Future: 2 };

const { GetPatientGender, GetPatientStatus } = GENERAL.PATIENTS;

const { ajaxCall } = GENERAL.FETCH_DATA;
const { APP_ID } = GENERAL;

const newVolunteerText = `מתנדב חדש`;
//   const noDriver = { hebrew: 'אין', english: 'None!' };

const controllersLayout = "<'row top'<'col-sm-6'l><'col-sm-5'f><'col-sm-1'B>>rt<'bottom'pi>";//https://datatables.net/reference/option/dom
const controllersLayoutForFutureTable = "<'row top'<'col-sm-2'l><'#future-table-time-interval-container.col-sm-4'><'col-sm-5'f><'col-sm-1'B>>rt<'bottom'pi>";
let drivers, driverNames;
let _ridepatToManipulate;
let _assignDriverToRideNum;
let _isDriverReplacement;
let _trToHighLight;
let _driverId;

let tMorning, tAfternoon, tTomorrowMorning, tTomorrowAfternoon, tFuture;


function wiringDataTables() {
    //manage button clicks on tables

    //#region remove ridepat
    $(`#${tableNames.MORNING} tbody`).on('click', '.remove', function () {
        confirmDelete(this, tMorning);
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('click', '.remove', function () {
        confirmDelete(this, tAfternoon);
    });

    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('click', '.remove', function () {
        confirmDelete(this, tTomorrowMorning);
    });

    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('click', '.remove', function () {
        confirmDelete(this, tTomorrowAfternoon);
    });

    $(`#${tableNames.FUTURE} tbody`).on('click', '.remove', function () {
        confirmDelete(this, tFuture);
    });
    //#endregion

    //#region edit ridepat
    $(`#${tableNames.MORNING} tbody`).on('click', '.edit', function () {
        editButton(this);
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('click', '.edit', function () {
        editButton(this);
    });


    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('click', '.edit', function () {
        editButton(this);
    });


    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('click', '.edit', function () {
        editButton(this);
    });

    $(`#${tableNames.FUTURE} tbody`).on('click', '.edit', function () {
        editButton(this);
    });
    //#endregion

    //#region copyMessage ridepat
    $(`#${tableNames.MORNING} tbody`).on('click', '.copyMessage', function () {
        copyMessageButton(this, tMorning);
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('click', '.copyMessage', function () {
        copyMessageButton(this, tAfternoon);
    });


    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('click', '.copyMessage', function () {
        copyMessageButton(this, tTomorrowMorning);
    });

    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('click', '.copyMessage', function () {
        copyMessageButton(this, tTomorrowAfternoon);
    });

    $(`#${tableNames.FUTURE} tbody`).on('click', '.copyMessage', function () {
        copyMessageButton(this, tFuture);
    });
    //#endregion

    //#region show target form
    /*
     IMPORTANT NOTE
     the event is mouseup in order to catch all scenarios.
     */
    $(`#${tableNames.MORNING} tbody`).on('mouseup', '.showMe', function () {
        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('mouseup', '.showMe', function () {
        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);
    });

    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('mouseup', '.showMe', function () {
        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);
    });

    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('mouseup', '.showMe', function () {

        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);

    });

    $(`#${tableNames.FUTURE} tbody`).on('mouseup', '.showMe', function () {

        let targetObj = {};
        targetObj.objName = $(this).attr("data-obj");
        targetObj.displayName = this.text;

        showMe(targetObj);

    });
    //#endregion

    //#region ContextMenu_dropdown
    $(`#${tableNames.MORNING} tbody`).on('mousedown', '.within2HoursSinceChange', function (e) {

        timer = setTimeout(function () {

            candidate4IhaveSeen = e.target.parentElement.id;
            $('.ContextMenu_dropdown').removeClass('hidden');
            $(".ContextMenu_dropdown").css({ position: "absolute", top: e.pageY, left: e.pageX });
        }, 500);
    }).on("mouseup mouseleave", function () {
        clearTimeout(timer);
    }).on("click", function () {//click on the td...
        $('.ContextMenu_dropdown').addClass('hidden');
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('mousedown', '.within2HoursSinceChange', function (e) {

        timer = setTimeout(function () {

            candidate4IhaveSeen = e.target.parentElement.id;
            $('.ContextMenu_dropdown').removeClass('hidden');
            $(".ContextMenu_dropdown").css({ position: "absolute", top: e.pageY, left: e.pageX });
        }, 500);
    }).on("mouseup mouseleave", function () {
        clearTimeout(timer);
    }).on("click", function () {//click on the td...
        $('.ContextMenu_dropdown').addClass('hidden');
    });

    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('mousedown', '.within2HoursSinceChange', function (e) {

        timer = setTimeout(function () {

            candidate4IhaveSeen = e.target.parentElement.id;
            $('.ContextMenu_dropdown').removeClass('hidden');
            $(".ContextMenu_dropdown").css({ position: "absolute", top: e.pageY, left: e.pageX });
        }, 500);
    }).on("mouseup mouseleave", function () {
        clearTimeout(timer);
    }).on("click", function () {//click on the td...
        $('.ContextMenu_dropdown').addClass('hidden');
    });

    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('mousedown', '.within2HoursSinceChange', function (e) {

        timer = setTimeout(function () {

            candidate4IhaveSeen = e.target.parentElement.id;
            $('.ContextMenu_dropdown').removeClass('hidden');
            $(".ContextMenu_dropdown").css({ position: "absolute", top: e.pageY, left: e.pageX });
        }, 500);
    }).on("mouseup mouseleave", function () {
        clearTimeout(timer);
    }).on("click", function () {//click on the td...
        $('.ContextMenu_dropdown').addClass('hidden');
    });


    $(`#${tableNames.FUTURE} tbody`).on('mousedown', '.within2HoursSinceChange', function (e) {

        timer = setTimeout(function () {

            candidate4IhaveSeen = e.target.parentElement.id;
            $('.ContextMenu_dropdown').removeClass('hidden');
            $(".ContextMenu_dropdown").css({ position: "absolute", top: e.pageY, left: e.pageX });
        }, 500);
    }).on("mouseup mouseleave", function () {
        clearTimeout(timer);
    }).on("click", function () {//click on the td...
        $('.ContextMenu_dropdown').addClass('hidden');
    });

    //#endregion

    //#region candidatesButton
    $(`#${tableNames.MORNING} tbody`).on('click', '.candidatesBtn', function () {
        candidatesButton(this);
    });

    $(`#${tableNames.AFTERNOON} tbody`).on('click', '.candidatesBtn', function () {
        candidatesButton(this);
    });

    $(`#${tableNames.TOMORROW_MORNING} tbody`).on('click', '.candidatesBtn', function () {
        candidatesButton(this);
    });

    $(`#${tableNames.TOMORROW_AFTERNOON} tbody`).on('click', '.candidatesBtn', function () {
        candidatesButton(this);
    });

    $(`#${tableNames.FUTURE} tbody`).on('click', '.candidatesBtn', function () {
        candidatesButton(this);
    });
    //#endregion

}

function getDrivers() {

    const driversPromise = new Promise((resolve, reject) => {

        const getDrivers_SCB = (data) => {
            drivers = JSON.parse(data.d);
            driverNames = drivers.map(d => d.DisplayName);

            return resolve(driverNames);
        }

        const getDrivers_ECB = (err) => {
            alert('Issue in getDrivers ' + JSON.stringify(err));

            return reject(err);
        }
        ajaxCall('GetDrivers', JSON.stringify({ isActive: true, isDriving: true }), getDrivers_SCB, getDrivers_ECB);
    });

    return driversPromise;
}


const assignOrUpdateAPIcall = (unityRideID, DriverId) => {
    const UserName = GENERAL.USER.getUserDisplayName();
    const request = {
        UnityRideId: unityRideID,
        DriverId: DriverId,
        isDelete: false,
        userName: UserName
    }
    ajaxCall('AssignUpdateDriverToUnityRide', JSON.stringify(request), assignDriverToRidePat_SCB, assignDriverToRidePat_ECB);

}

const assignDriverToRidePat = () => {
    $('#wait').show();
    const driverNameFromInput = $('#driver').val();

    if (typeof driverNameFromInput === 'undefined' || !driverNameFromInput || driverNameFromInput.length === 0 || !driverNames.includes(driverNameFromInput)) {

        $('#wait').hide();
        return alert('יש לבחור נהג מתוך הרשימה');
    }
    const driverIndex = driverNames.indexOf(driverNameFromInput);
    const userSelected = drivers[driverIndex];
    let request;
    let dataString;


    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetAbsenceByVolunteerId",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: userSelected.Id }),
        success: function (data) {
            $('#wait').hide();
            Absences = JSON.parse(data.d);
            let absenceStatus = false;
            for (var i = 0; i < Absences.length; i++) {
                if (Absences[i].AbsenceStatus) {
                    absenceStatus = true;
                }
            }
            if (absenceStatus) {
                swal({
                    title: "?האם הינך בטוח/ה שברצונך לשבץ נהג זה",
                    type: "warning",
                    text: 'נהג זה כרגע אינו זמין,באפשרותך לשבץ אותו בכל זאת או לבחור בנהג חלופי',
                    showCancelButton: true,
                    cancelButtonText: "בטל",
                    confirmButtonClass: 'btn-warning',
                    confirmButtonText: "שבץ",
                    closeOnConfirm: true
                }, function (userResponse) {
                    if (userResponse) {
                        assignOrUpdateAPIcall(_ridepatToManipulate, userSelected.Id);
                        setTimeout(() => {
                            colorRowWithDriver([_ridepatToManipulate]);

                        }, 750)
                        assignDriverToRidePatAbort();

                    }

                });
            }
            else {
                assignOrUpdateAPIcall(_ridepatToManipulate, userSelected.Id);
                assignDriverToRidePatAbort();

            }

        },
        error: function (err) {
            alert("Error in GetVolunteersAbsences (manageridepat.html ajax) !: " + err.responseText);
            $('#wait').hide();
        }
    });




}

const assignDriverToRidePatAbort = () => {
    removeClass('highlightRow');
}

const editPatientStatusAbort = () => {
    removeClass('highlightRow');
}

//New Api because the unity Gilad
const updateUnityridePatientStatus = () => {
    $('#wait').show();
    removeClass('highlightRow');
    const hours = $("#editPatientStatusModalHours").val();
    const minutes = $("#editPatientStatusModalMinutes").val();
    const isTreatmentFinished = !$("#editPatientStatusNotFinishedCB").is(':checked');

    if (
        isNaN(hours) || isNaN(minutes) ||
        hours === null || minutes === null
    ) {
        $('#wait').hide();
        return alert('נא לבחור שעה מהרשימות הנגללות או לבטל את הפעולה');
    }

    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    const now = new Date();
    const editTimeStamp = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hours, minutes);

    ridepatToManipulate = CustomRideObject(ridepatToManipulate);
    ridepatToManipulate.Pat.RidePatPatientStatus.Status = isTreatmentFinished;
    ridepatToManipulate.Pat.RidePatPatientStatus.EditTimeStamp = editTimeStamp;

    //update_arr_rides(ridepatToManipulate);
    const userName = GENERAL.USER.getUserDisplayName();
    const data = {
        patientId: ridepatToManipulate.Pat.Id,
        unityRideID: ridepatToManipulate.RidePatNum,
        patientStatus: isTreatmentFinished ? "Finished" : "Not Finished",
        editTimeStamp: editTimeStamp,
        userName: userName
    }
    const updatePatientStatus_SCB = () => {

        $('#wait').hide();
        $('#editPatientStatusModal').modal('hide');
    }

    const updatePatientStatus_ECB = (err) => {

        $('#wait').hide();
        $('#editPatientStatusModal').modal('hide');
        console.log(err);
        alert(err);
    }

    ajaxCall('UpdatePatientStatus_UnityRide', JSON.stringify(data), updatePatientStatus_SCB, updatePatientStatus_ECB);
}


const updateRidePatPatientStatus = () => {

    $('#wait').show();
    removeClass('highlightRow');
    const hours = $("#editPatientStatusModalHours").val();
    const minutes = $("#editPatientStatusModalMinutes").val();
    const isTreatmentFinished = !$("#editPatientStatusNotFinishedCB").is(':checked');

    if (
        isNaN(hours) || isNaN(minutes) ||
        hours === null || minutes === null
    ) {
        $('#wait').hide();
        return alert('נא לבחור שעה מהרשימות הנגללות או לבטל את הפעולה');
    }

    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    const now = new Date();

    const editTimeStamp = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hours, minutes);
    ridepatToManipulate = CustomRideObject(ridepatToManipulate);
    ridepatToManipulate.Pat.RidePatPatientStatus.Status = isTreatmentFinished;
    ridepatToManipulate.Pat.RidePatPatientStatus.EditTimeStamp = editTimeStamp;
    update_arr_rides(ridepatToManipulate);

    const data = {
        patientId: ridepatToManipulate.Pat.Id,
        ridePatId: ridepatToManipulate.RidePatNum,
        patientStatus: isTreatmentFinished ? "Finished" : "Not Finished",
        editTimeStamp: editTimeStamp
    }

    const updatePatientStatus_SCB = () => {

        $('#wait').hide();
        $('#editPatientStatusModal').modal('hide');
    }

    const updatePatientStatus_ECB = (err) => {

        $('#wait').hide();
        $('#editPatientStatusModal').modal('hide');
        console.log(err);
        alert(err);
    }
    ajaxCall('UpdatePatientStatus', JSON.stringify(data), updatePatientStatus_SCB, updatePatientStatus_ECB);


}

//Gilad change here the date to PickupTime
const updateRidePatTime = () => {

    $('#wait').show();
    let hours, minutes = null;
    const isAfterNoon = $("#editTimeAfterNoonCB").is(':checked');

    if (isAfterNoon) {
        hours = 22;
        minutes = 14;
    } else {
        hours = $("#hours").val();
        minutes = $("#minutes").val();
    }
    if (
        isNaN(hours) || isNaN(minutes) ||
        hours === null || minutes === null
    ) {
        $('#wait').hide();
        return alert('נא לבחור שעה מהרשימות הנגללות או אחה"צ');
    }

    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    let dateToManipulate = convertDBDate2FrontEndDate(ridepatToManipulate.PickupTime);
    dateToManipulate.setHours(hours, minutes);

    let data = {
        ridePatId: _ridepatToManipulate,
        dateTime: dateToManipulate
    };

    const updateRidePatTime_SCB = () => {

        $('#wait').hide();
        removeClass('highlightRow');
        //the timeout is because the render take time and we want to update the color in real time. by gilad.
        setTimeout(colorRowWithDriver, 750, [_ridepatToManipulate])
        //colorRowWithDriver([_ridepatToManipulate]);

        $('#editTimeModal').modal('hide');
    }

    const updateRidePatTime_ECB = (err) => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editTimeModal').modal('hide');
        alert(err);
    }
    ajaxCall('UpdateRidePatTime', JSON.stringify(data), updateRidePatTime_SCB, updateRidePatTime_ECB);

}

//new Api
const updateUnityRideTime = () => {

    $('#wait').show();
    let hours, minutes = null;
    const isAfterNoon = $("#editTimeAfterNoonCB").is(':checked');

    if (isAfterNoon) {
        hours = 20;
        minutes = 14;
    } else {
        hours = $("#hours").val();
        minutes = $("#minutes").val();
    }
    if (
        isNaN(hours) || isNaN(minutes) ||
        hours === null || minutes === null
    ) {
        $('#wait').hide();
        return alert('נא לבחור שעה מהרשימות הנגללות או אחה"צ');
    }

    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    let dateToManipulate = convertDBDate2FrontEndDate(ridepatToManipulate.PickupTime);
    dateToManipulate.setHours(hours, minutes);
    const userName = GENERAL.USER.getUserDisplayName();

    let data = {
        unityRideId: _ridepatToManipulate,
        pickupTime: dateToManipulate,
        userName: userName
    };

    const updateUnityRideTime_SCB = () => {

        $('#wait').hide();
        removeClass('highlightRow');
        //the timeout is because the render take time and we want to update the color in real time. by gilad.
        setTimeout(colorRowWithDriver, 750, [_ridepatToManipulate])
        //colorRowWithDriver([_ridepatToManipulate]);

        $('#editTimeModal').modal('hide');
    }

    const updateUnityRideTime_ECB = (err) => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editTimeModal').modal('hide');
        if (err.responseJSON.Message.includes('error code: -2')) {
            const patientName = ridepatToManipulate.PatientName
            const PickupTime = dateToManipulate;
            const date = formatDate(PickupTime);
            const time = formatTime(PickupTime);
            const origin = ridepatToManipulate.Origin;
            const destination = ridepatToManipulate.Destination;
            const textmessage = `החולה ${patientName} רשום להסעה בתאריך ${date} בשעה ${time} מ${origin} ל${destination}`;
            swal({
                title: "קוד שגיאה 2-",
                text: textmessage,
                type: "error",
                confirmButtonText: "אישור",
                confirmButtonClass: 'btn btn-danger',
                closeOnConfirm: true
            })
        }
        else {
            alert(err);

        }
    }
    ajaxCall('UpdateUnityRideTime', JSON.stringify(data), updateUnityRideTime_SCB, updateUnityRideTime_ECB);

}
function formatTime(date) {
    let hours = date.getHours();
    let minutes = date.getMinutes();

    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;

    return hours + ':' + minutes;
}
function formatDate(date) {
    let day = date.getDate();
    let month = date.getMonth() + 1; // Months are zero based
    let year = date.getFullYear();

    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;

    return day + '/' + month + '/' + year;
}
//new API to update remark by Gilad
const updateRemark_UnityRide = () => {
    $('#wait').show();
    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    let newRemark = $('#editRemarkTextEditor').val();

    newRemark = newRemark.replace(/\n/g, "<br>");
    const userName = GENERAL.USER.getUserDisplayName();
    let data = {
        UnityRideID: _ridepatToManipulate,
        newRemark,
        userName
    };
    const updateRemark_SCB = () => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editRemarkModal').modal('hide');
    }

    const updateRemark_ECB = (err) => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editRemarkModal').modal('hide');
        alert(err);
    }

    ajaxCall('UpdateUnityRideRemark', JSON.stringify(data), updateRemark_SCB, updateRemark_ECB);
}

const updateRemark = () => {

    $('#wait').show();
    let ridepatToManipulate = arr_rides.find(r => r.RidePatNum === _ridepatToManipulate);
    let newRemark = $('#editRemarkTextEditor').val();

    newRemark = newRemark.replace(/\n/g, "<br>");

    let data = {
        ridePatId: _ridepatToManipulate,
        newRemark
    };

    const updateRemark_SCB = () => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editRemarkModal').modal('hide');
    }

    const updateRemark_ECB = (err) => {

        $('#wait').hide();
        removeClass('highlightRow');
        $('#editRemarkModal').modal('hide');
        alert(err);
    }

    ajaxCall('UpdateRidePatRemark', JSON.stringify(data), updateRemark_SCB, updateRemark_ECB);

}

const editTimeAbort = () => {
    removeClass('highlightRow');
}

const editRemarkAbort = () => {
    removeClass('highlightRow');
}

const removeClass = (className) => {
    _trToHighLight.classList.remove(className);
}

const assignDriverToRidePat_SCB = (data) => {
    const ur = JSON.parse(data.d);
    if (ur.RidePatNum == -5) {
        console.error('duplicated driver! --> got status -5 from the server');

        swal({
            title: " הפעולה נכשלה ,לנהג זה יש נסיעה בתאריך ובשעה שבחרת",
            timer: 3600,
            type: "error",
            showConfirmButton: false
        });

    }
    update_arr_rides(ur);
    setTimeout(() => {
        colorRowWithDriver([_ridepatToManipulate]);
    }, 750)
    $('#wait').hide();
    removeClass('highlightRow');
    $('#assignDriverModal').modal('hide');
}

const assignDriverToRidePat_ECB = (err) => {
    $('#wait').hide();
    assignDriverToRidePat('highlightRow');
    alert('Issue in assignDriverToRidePat: ' + JSON.stringify(err));
}

const UpdateDriver_SCB = (data) => {
    $('#wait').hide();
    _trToHighLight.classList.remove('highlightRow');
    $('#assignDriverModal').modal('hide');
}

const UpdateDriver_ECB = (err) => {
    $('#wait').hide();
    _trToHighLight.classList.remove('highlightRow');
    alert('Issue in UpdateDriver: ' + JSON.stringify(err));
}

const deleteDriverFromRide = () => {

    swal({
        title: "האם אתה בטוח שאתה רוצה למחוק את הנהג מההסעה?",
        type: "warning",
        text: 'בלחיצה על מחיקה הנהג יוסר מהסעה זו',
        showCancelButton: true,
        cancelButtonText: "בטל",
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "מחיקה",
        closeOnConfirm: true
    }, function (userResponse) {
        if (userResponse) {
            $('#wait').show();

            const UserName = GENERAL.USER.getUserDisplayName();

            const request = {
                UnityRideId: _ridepatToManipulate,
                DriverId: _driverId,
                isDelete: true,
                userName: UserName


            }
            $(`#${_ridepatToManipulate}`).removeClass('statusComplete');
            const dataString = JSON.stringify(request);
            ajaxCall('AssignUpdateDriverToUnityRide', dataString, deleteRideSuccessCB, deleteRideErrorCB);

        }

    });




}

const deleteRideSuccessCB = (data) => {
    $('#wait').hide();
    $('#assignDriverModal').modal('hide');
    //let td = document.getElementById(_ridepatToManipulate).getElementsByClassName('driver_td')[0]
    //td.classList.add('withIn12HoursDrive')
    let tr = document.getElementById(_ridepatToManipulate)
    tr_node2manipulate = tr;
    manipilateNodeStyle(tr_node2manipulate, _ridepatToManipulate, REMOVE_DRIVER, false, tableName2manipulate);

    assignDriverToRidePatAbort();
}

const deleteRideErrorCB = (err) => {

    $('#wait').hide();
    $('#assignDriverModal').modal('hide');
    assignDriverToRidePatAbort();
    alert(err);
}

$(document).ready(function () {

    $("#assignDriverModal").attr("w3-include-html", "assignDriverModal.html");
    $('#assignDriverModal').on('shown.bs.modal', bsModalHasShown); // triger cb function only after modal is shown
    $('#editRemarkModal').on('shown.bs.modal', editRemarkModalHasShown); // triger cb function only after modal is shown


    $("#editTimeModal").attr("w3-include-html", "editTimeModal.html");
    $("#editPatientStatusModal").attr("w3-include-html", "editPatientStatusModal.html");
    $("#editRemarkModal").attr("w3-include-html", "editRemarkModal.html");
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getHospitalView",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        //  async: false,
        data: JSON.stringify({ active: true }),
        success: function (data) {
            arr_hospitals = JSON.parse(data.d);
            for (i in arr_hospitals) {
                hospitals.push(arr_hospitals[i].Name)
            }
        },
        error: function (err) { alert("Error in getHospitalView: " + err.responseText); }
    });


    wiringDataTables();
    const { COPYWRITE } = GENERAL;
    $('#rights').html(COPYWRITE());

    if (localStorage.refreshTime == "undefined" || localStorage.refreshTime === undefined) { // Set the deafult refresh rate for 5 minutes
        localStorage.refreshTime = '300000';
        $('#refreshTime').val('300000')
    } else {
        $('#refreshTime').val(parseInt(JSON.parse(localStorage.refreshTime)));
    }
    refreshTime = parseInt(JSON.parse(localStorage.refreshTime));

    $('#refreshTime').on('change', function () {

        localStorage.refreshTime = String(this.value);
        refreshTime = parseInt(JSON.parse(localStorage.refreshTime));
        clearInterval(refreshInterval);
        refreshInterval = setInterval("refreshTable_UnityRide()", refreshTime);

    });

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
    isAssistant = JSON.parse(GENERAL.USER.getIsAssistant());

    var assistantName = "";
    if (isAssistant) {
        $("#menuType").attr("w3-include-html", "HelperMenu.html");
        var coorAssistant = GENERAL.USER.getCoorAssistant();
        assistantName = GENERAL.USER.getAsistantDisplayName();
    }
    else $("#menuType").attr("w3-include-html", "menu.html");
    includeHTML();
    if (isAssistant) {
        var userName = GENERAL.USER.getAsistantAndCoorDisplayName();
        $("#userName").html(userName);
    }
    else {
        var userName = GENERAL.USER.getUserDisplayName();
        $("#userName").html(userName);
    }


    deceideWhichTable2Show();

    jQuery.extend(jQuery.fn.dataTableExt.oSort, {

        "de_time-asc": function (a, b) {
            /*a and b are a couple of times hh:mm to compare
            return value will be:
            0 if a=b
            1 if a > b
           -1 if a < b
            */

            a = $(a)[0].querySelector('.time').innerHTML;
            b = $(b)[0].querySelector('.time').innerHTML;

            let datetime_a = $.trim(a).split(':');
            let datetime_b = $.trim(b).split(':');
            let hh_a = parseInt(datetime_a[0]);
            let mm_a = parseInt(datetime_a[1]);

            let hh_b = parseInt(datetime_b[0]);
            let mm_b = parseInt(datetime_b[1]);

            let now = new Date();

            let a_dateTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hh_a, mm_a);
            let b_dateTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hh_b, mm_b);

            a = a_dateTime.getTime();
            b = b_dateTime.getTime();

            if (isNaN(a)) { //this is for אחה"צ
                a = Infinity;
            }
            if (isNaN(b)) {
                b = Infinity;
            }

            return a === b ? 0 : a > b ? 1 : -1;

        },

        "de_time-desc": function (a, b) {
            /*a and b are a couple of times hh:mm to  compare
              return value will be:
              0 if a=b
             -1 if a > b
              1 if a < b
              */
            a = $(a)[0].querySelector('.time').innerHTML;
            b = $(b)[0].querySelector('.time').innerHTML;

            let datetime_a = $.trim(a).split(':');
            let datetime_b = $.trim(b).split(':');

            let hh_a = parseInt(datetime_a[0]);
            let mm_a = parseInt(datetime_a[1]);

            let hh_b = parseInt(datetime_b[0]);
            let mm_b = parseInt(datetime_b[1]);

            let now = new Date();

            let a_dateTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hh_a, mm_a)
            let b_dateTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hh_b, mm_b)

            a = a_dateTime.getTime();
            b = b_dateTime.getTime();

            if (isNaN(a)) {//this is for אחה"צ
                a = Infinity;
            }
            if (isNaN(b)) {
                b = Infinity;
            }

            return a === b ? 0 : a > b ? -1 : 1;
        },

        "de_date-asc": (a, b) => {

            return dateCompareFunc(a, b, true);

        },

        "de_date-desc": (a, b) => {

            return dateCompareFunc(a, b, false);
        },

        "last_modifiedSort-asc": (a, b) => {

            return datetimeCompareFunc(a, b, true);

        },

        "last_modifiedSort-desc": (a, b) => {

            return datetimeCompareFunc(a, b, false);

        },
    });

    getCoors();

    let timeInt = _timeinterval_G == 'חודש' ? 30 : _timeinterval_G == 'שבועיים' ? 14 : 7
    refreshTable_UnityRide(timeInt);
    //refreshTable()

    if (!isAssistant) {

        refreshInterval = setInterval("refreshTable_UnityRide()", refreshTime);
    }

    let allSortsSwitch = true;
    if (localStorage.getItem('allSortsSwitch') != null)
        allSortsSwitch = JSON.parse(localStorage.getItem('allSortsSwitch'));

    if (allSortsSwitch) {
        $('#allSortsSwitch').prop('checked', true);
        $('#openSortsControllers').prop('disabled', false);
        $('#allSortsLabel').html('כבה סינון');
    } else {
        $('#allSortsSwitch').prop('checked', false);
        $('#openSortsControllers').prop('disabled', true);
        $('#allSortsLabel').html('הפעל סינון');
    }



});

const assignTable2lastOne = (tableName) => {

    localStorage.setItem('lastTableWas', tableName);
}

const deceideWhichTable2Show = () => {
    /*
     #collapse1 = datatable-morning
     #collapse2 = datatable-afternoon
     #collapse3 = datatable-tomorrow-morning
     #collapse4 = ddatatable-tomorrow-afternoon
     #collapse5 = datatable-future
     */

    let table2open = localStorage.getItem('lastTableWas') ? localStorage.getItem('lastTableWas') : '';

    switch (table2open) {

        case tableNames.MORNING:
            $('#collapse1').addClass("in");
            $('#collapsed1_aTag').removeClass('collapsed');
            break;

        case tableNames.AFTERNOON:
            $('#collapse2').addClass("in");
            $('#collapsed2_aTag').removeClass('collapsed');
            break;

        case tableNames.TOMORROW_MORNING:
            $('#collapse3').addClass("in");
            $('#collapsed3_aTag').removeClass('collapsed');
            break;

        case tableNames.TOMORROW_AFTERNOON:
            $('#collapse4').addClass("in");
            $('#collapse4_aTag').removeClass('collapsed');
            break;

        case tableNames.FUTURE:
            $('#collapse5').addClass("in");
            $('#collapse5_aTag').removeClass('collapsed');
            break;

        default:
            setTable2Show_Bytime();
            break;

    }

}

const setTable2Show_Bytime = () => {

    let now = new Date();

    if (parseInt(now.getHours()) <= 12) {//show the morning table

        $('#collapse1').addClass("in");
    } else {//show the afternoon table

        $('#collapse2').addClass("in");
    }


}

function getCoors() {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getCoorList",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: false,
        //data: JSON.stringify({}),
        success: getCoorsSuccessCB,
        error: getCoorsErrorCB
    });
}

function sendMessegeFromAssistant(ridepat, cellphone, msg) {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/pushAssistant",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: false,
        data: JSON.stringify({ ridepat: ridepat, cellphone: cellphone, msg: msg }),
        error: pushAssistantErrorCB
    });
}

function pushAssistantErrorCB(error) {
    console.log(error);
}

function getCoorsSuccessCB(data) {
    var arr_coor = JSON.parse(data.d);
    var user = GENERAL.USER.getUserName();
    for (i in arr_coor) {
        if (user == arr_coor[i].UserName) {
            currentCoor = arr_coor[i].DisplayName;
            GENERAL.USER.setUserType(arr_coor[i].TypeVol)
            break;
        }
    }
}

function getCoorsErrorCB(e) {
    alert("Error in getCoors: " + e.responseText);
}

function confirmDelete(deleteBtn, table) {

    let ridePatNum = parseInt(deleteBtn.id);

    //IMMUTABLE paradigme !
    const original_deleteCandidate = arr_rides.find((r) => r.RidePatNum === ridePatNum);


    let deleteCandidate = {
        ...original_deleteCandidate,
        Date: convertDBDate2FrontEndDate(original_deleteCandidate.Date)
    }
    deleteCandidate = CustomRideObject(deleteCandidate);
    deleteCandidate.Date = new Date(convert2DBDateToInt(deleteCandidate.Date));
    let isAfterNoon = deleteCandidate.Date.getMinutes() === 14;
    let isToday = table.context[0].sTableId === "datatable-afternoon" || table.context[0].sTableId === "datatable-morning";

    const { DisplayName } = deleteCandidate.Pat;
    const { Name: from } = deleteCandidate.Origin;
    const { Name: to } = deleteCandidate.Destination;
    const { Date: date } = deleteCandidate; //Date is a reserved word for js! DO NOT ASSIGHN !!!

    let msg2user = `מחיקת ההסעה של ${DisplayName} מ: ${from} ל: ${to} `;

    msg2user += isToday ? 'היום ' :
        `יציאה בתאריך ${date.toLocaleString('he-IL', { dateStyle: 'short' })} `;

    msg2user += isAfterNoon ? 'בשעות אחה"צ ' :
        `בשעה ${date.toLocaleString('he-IL', { timeStyle: 'short' })} `;

    let addToTextWarning = ''
    addToTextWarning = (original_deleteCandidate.MainDriver == -1) && (original_deleteCandidate.IsAnonymous == true) ? '(לא יהיה ניתן לשחזר מידע את הנסיעה (תימחק לצמיתות)' : '';
    msg2user += '\n' + addToTextWarning;
    swal({
        title: "هل انت متأكد?\nהאם אתם בטוחים?",
        type: "warning",
        text: msg2user,
        showCancelButton: true,
        cancelButtonText: "الغاء\nבטל",
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "حذف\nמחיקה",
        closeOnConfirm: true
    }, function (userResponse) {
        if (userResponse) {
            // deleteRidePat(original_deleteCandidate, deleteBtn, table);
            deleteUnityRide(original_deleteCandidate, deleteBtn, table)
        }

    });


}

//gilad addition

function jsonToExcelAndDownload(data, fileName) {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Tomorrow-Ride');
    XLSX.writeFile(wb, fileName + '.xlsx');
}



const FixedHour = (DBdateString) => {
    let h = new Date(convertDBDate2FrontEndDate(DBdateString)).getHours()
    let m = new Date(convertDBDate2FrontEndDate(DBdateString)).getMinutes();
    if (h.toString().length == 1) {
        h = `0${h}`;
    }
    if (m.toString().length == 1) {
        m = `0${m}`;
    }
    return `${h}:${m}`;
}



// not in use right now.
const MessageToPalestinianCoor = () => {
    //const dest = tomorrowMorningRidePats[0].destination
    //const origin = tomorrowMorningRidePats[0].origin
    //let time = tomorrowMorningRidePats[0].time.replace('<div class="elementsInSameLine"><span class="time">', '').replace(`</span><button type='button' data-toggle="modal" data-target="#editTimeModal" onclick="prepreparationEditTimeModal(this, ${tomorrowMorningRidePats[0].ridePatNum})" class='btn btn-icon waves-effect waves-light btn-secondary' title='עריכה'><i class='ti-pencil'></i></button></div>`, '');
    //let patientName = tomorrowMorningRidePats[0].patient.split('>')[1].split('<')[0]
    $('#wait').show();
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/Get_Tomorrow_RidePatView_Gilad",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        success: (data) => {
            $('#wait').hide();
            let tomorrowListExcel = JSON.parse(data.d);
            let listToExcel = [];
            let rideNum = 1;
            tomorrowListExcel.forEach(row => {
                let rowToExcel = {
                    Row: rideNum,
                    From: row.OriginE,
                    To: row.DestinationE,
                    At: FixedHour(row.pickupTime),
                    Pateint_Name: row.patientEName,
                    Num_of_passengers: row.numOfPass,
                    Driver: row.MainDriver
                };
                rideNum++;
                listToExcel.push(rowToExcel);
            })
            const stringToCopy = buildMessageString(listToExcel);

            navigator.clipboard.writeText(stringToCopy)
                .then(function () {
                    swal({
                        title: "ההודעה מוכנה בפעולת הדבק",
                        timer: 1000,
                        type: "success",
                        showConfirmButton: false
                    });
                }, function (err) {
                    swal({
                        title: "העתקת ההודעה נכשלה",
                        timer: 1000,
                        type: "error",
                        showConfirmButton: false
                    });
                });
        },
        error: (error) => {
            console.log('from ajax to Get_Tomorrow_RidePatView_Gilad ', error);
            $('#wait').hide();
        }

    });
}

//gilad
const buildMessageString = (rideList) => {
    const d = new Date();
    const space = '\n';
    const line = '_______________________________________________________________' + space + space;
    const tab = '\t';
    const sep = `  `;
    let str = `Tomorrow-Rides${tab}${d.getDate() + 1} - ${d.getMonth() + 1} - ${d.getFullYear()}` + space;
    str += line;


    for (let i = 0; i < rideList.length; i++) {
        rideList[i].same = i + 1;

        if (i != 0 && isSameRide(rideList[i], rideList[i - 1])) {
            rideList[i].same = i + 1;
            rideList[i - 1].same = i + 1;
            //this change for cal the num of passanger each ride.
            rideList[i].Num_of_passengers = rideList[i].Num_of_passengers + rideList[i - 1].Num_of_passengers + 1;
            rideList[i - 1].Num_of_passengers = rideList[i].Num_of_passengers

        }
        else {
            rideList[i].Num_of_passengers += 1;
        }


    }
    for (let i = 0; i < rideList.length; i++) {
        let tempStr = '';
        //if (i != 0 && rideList[i].same != rideList[i - 1].same) {
        //    tempStr=line
        //}
        tempStr += `Ride ${rideList[i].same}${sep}From: ${rideList[i].From}${sep}To: ${rideList[i].To}${sep}At: ${rideList[i].At}${sep}${rideList[i].Pateint_Name}${sep} (${rideList[i].Num_of_passengers})`;
        tempStr += space;
        str += tempStr;
    }
    str += line;

    return str;

}
const isSameRide = (ride1, ride2) => {
    if (ride1.At == ride2.At && ride1.From == ride2.From && ride1.Driver == ride2.Driver) {
        return true
    }
    return false;
}

function copyMessageButton(copyMsgBtn, table) {

    let ridePatNum = parseInt(copyMsgBtn.id);
    let messageObject = showMessage(arr_rides, ridePatNum);

    if (!messageObject.date.includes('Date')) {

        const dateTicks = fixDate_WhichComeFromOpenConnection(messageObject.date, false);
        messageObject.date = repaireDateForMeesageModule(dateTicks);
    }
    let textMessageToCopy = buildMessage(messageObject);
    navigator.clipboard.writeText(textMessageToCopy)
        .then(function () {
            swal({
                title: "ההודעה מוכנה בפעולת הדבק",
                timer: 1000,
                type: "success",
                showConfirmButton: false
            });
        }, function (err) {
            swal({
                title: "העתקת ההודעה נכשלה",
                timer: 1000,
                type: "error",
                showConfirmButton: false
            });
        });
}

const repaireDateForMeesageModule = (ticks) => {
    let rawDate = `/Date(${ticks})/`;

    return rawDate;
}

const candidatesButton = (btn) => {
    let ridePatNum = btn.id;
    GENERAL.RIDEPAT.setRidePatNum4_viewCandidate(ridePatNum);

    ridePatNum = parseInt(ridePatNum);
    const thisridePatObj = arr_rides.find((r) => r.RidePatNum === ridePatNum);

    localStorage.setItem(`ridePatObj_${ridePatNum}`, JSON.stringify(thisridePatObj));

    window.open("viewCandidates.html", '_blank').focus();

    /*
    let a = document.createElement('a');

    a.target = '_blank';
    a.href = "viewCandidates.html";
    a.click();
    */
}

function editButton(ridePat) {
    arr_details = { RidePatNum: ridePat.name, func: "edit" }; //ridePat.name=RidePatNum
    GENERAL.RIDEPAT.setRidePatList(JSON.stringify(arr_details));
    location.href = "ridePatForm.html";
}

function deleteRidePat(ridePat, deleteBtn, thistable) {
    checkCookie();
    $('#wait').show();
    // var numberOfRides = 1;
    //var repeatRide = "";
    let ridePatNums2MarkDeleted = [];
    ridePatNums2MarkDeleted.push(ridePat.RidePatNum);
    let nowInUTC = new Date().toUTCString();

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/ChangeArrayOF_RidePatStatuses",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ newStatus: 'נמחקה', ridePatNums: ridePatNums2MarkDeleted, clientUTCTimeStemp: nowInUTC }),
        success: function () {
            swal({
                title: "لقد تم حذف السفرية\nההסעה נמחקה",
                timer: 1000,
                type: "success",
                showConfirmButton: false
            });


            if (thistable && deleteBtn) {
                thistable.row($(deleteBtn).parents('tr')).remove().draw();
            } else {
                location.reload();
            }

            let RidePat = {
                Date: typeof ridePat.Date === 'string' ? convertDBDate2FrontEndDate(ridePat.Date) : ridePat.Date,
                Destination: ridePat.Destination,
                Origin: ridePat.Origin,
                Pat: ridePat.Pat
            }
            //retturn ride part.
            $.ajax({
                dataType: "json",
                url: "WebService.asmx/getReturnRidePat",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                async: false,
                data: JSON.stringify({ RidePat }),
                success: function (data) {

                    returnRidePat = JSON.parse(data.d);


                    if (returnRidePat && returnRidePat.Status !== 'נמחקה') {
                        $('#wait').hide();

                        let patientName = 'חולה';
                        if (!returnRidePat.Pat.DisplayName.includes("אנונימי")) {
                            patientName = returnRidePat.Pat.DisplayName;
                        }
                        let frontEndDate = convertDBDate2FrontEndDate(returnRidePat.Date);

                        if (frontEndDate.getMinutes() === 14) {//afternoon
                            frontEndDate = `תאריך ${frontEndDate.toLocaleDateString('he-IL')} בשעות אחה"צ`;
                        } else {
                            frontEndDate = frontEndDate.toLocaleString('he-IL', { dateStyle: 'short', timeStyle: 'short' });
                        }

                        returnRidePatMsg = `האם למחוק גם את ההחזרה של ${patientName} מ${returnRidePat.Origin.Name} ל${returnRidePat.Destination.Name} ב${frontEndDate}`;
                        $('#DriverReturnRidePatMsg').html("");
                        if (returnRidePat.Drivers.length != 0) {
                            returnDriverMsg = returnRidePat.Drivers[0].DisplayName + " נרשם להחזרה הזו!";
                            $('#DriverReturnRidePatMsg').html("<h5 style='color:red; font-weight: bold;'>שימו לב!</h5><h5>" + returnRidePat.Drivers[0].DisplayName + " נרשם להחזרה הזו!</h5>");
                        }

                        $('#returnRidePatMsg').html(returnRidePatMsg);
                        $('#messagesModal').modal('toggle');
                    } else {
                        $('#wait').hide();
                    }
                },
                error: function (err) {
                    alert("Error in getReturnRidePat: " + err.responseText);
                    console.log(err);
                }
            })
        },
        error: function (err) { alert("Error in deleteRidePat:" + err.responseText); }
    });

}


const deleteUnityRide = (ridePat, deleteBtn, thistable) => {
    checkCookie();
    $('#wait').show();
    const UserName = GENERAL.USER.getUserDisplayName();
    // var numberOfRides = 1;
    //var repeatRide = "";
    let ridePatNums2MarkDeleted = [];
    ridePatNums2MarkDeleted.push(ridePat.RidePatNum);
    let nowInUTC = new Date().toUTCString();
    const listIds = []
    listIds.push(ridePat.RidePatNum);
    let request = { ListIDs: listIds, userName: UserName };
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/deleteUnityRide",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify(request),
        success: function (data) {
            swal({
                title: "لقد تم حذف السفرية\nההסעה נמחקה",
                timer: 1000,
                type: "success",
                showConfirmButton: false
            });
            if (thistable && deleteBtn) {
                let stam = thistable.row($(deleteBtn).parents('tr')).remove().draw();
            } else {
                location.reload();
            }
            $('#wait').hide();
            //retturn ride part.
            // only non anonymous and destination==hospital
            if (isThisReturnNonAnonymousRide(ridePat)) {
                $.ajax({
                    dataType: "json",
                    url: "WebService.asmx/GetReturnRideUnityRide",
                    contentType: "application/json; charset=utf-8",
                    type: "POST",
                    async: false,
                    data: JSON.stringify({ unityRideID: ridePat.RidePatNum }),
                    success: function (data) {

                        returnRidePat = JSON.parse(data.d);
                        returnRidePat = CustomRideObject(returnRidePat);
                        if (returnRidePat && returnRidePat.Status != null && returnRidePat.Status !== 'נמחקה') {
                            $('#wait').hide();

                            let patientName = 'חולה';
                            if (!returnRidePat.Pat.DisplayName.includes("אנונימי")) {
                                patientName = returnRidePat.Pat.DisplayName;
                            }
                            let frontEndDate = convertDBDate2FrontEndDate(returnRidePat.Date);

                            if (frontEndDate.getMinutes() === 14) {//afternoon
                                frontEndDate = `תאריך ${frontEndDate.toLocaleDateString('he-IL')} בשעות אחה"צ`;
                            } else {
                                frontEndDate = frontEndDate.toLocaleString('he-IL', { dateStyle: 'short', timeStyle: 'short' });
                            }

                            returnRidePatMsg = `האם למחוק גם את ההחזרה של ${patientName} מ${returnRidePat.Origin.Name} ל${returnRidePat.Destination.Name} ב${frontEndDate}`;
                            $('#DriverReturnRidePatMsg').html("");
                            if (returnRidePat.Drivers.length != 0) {
                                returnDriverMsg = returnRidePat.Drivers[0].DisplayName + " נרשם להחזרה הזו!";
                                $('#DriverReturnRidePatMsg').html("<h5 style='color:red; font-weight: bold;'>שימו לב!</h5><h5>" + returnRidePat.Drivers[0].DisplayName + " נרשם להחזרה הזו!</h5>");
                            }

                            $('#returnRidePatMsg').html(returnRidePatMsg);
                            $('#messagesModal').modal('toggle');
                        } else {
                            $('#wait').hide();
                        }
                    },
                    error: function (err) {
                        alert("Error in getReturnUnityRide***: " + err.responseText);
                        console.log(err);
                    }
                })
            }









        },
        error: function (err) {
            alert("Error in deleteUnityRide: " + err.responseText);
            $('#wait').hide();
            console.log(err);
        }
    })





}


function newRidePatForm() {
    ridePatNum = " ";
    arr_details = { ridePatNum: ridePatNum, func: "new" };
    GENERAL.RIDEPAT.setRidePatList(JSON.stringify(arr_details));
    location.href = "ridePatForm.html";
}
function deleteReturn_unityRide() {

    let thisTable = null;
    let deleteBtn = null;

    if (document.getElementById(`${returnRidePat.RidePatNum}`) !== null) { //DUE TO SORTING THE ROW IS NOT RENDERED


        deleteBtn = document.getElementById(`${returnRidePat.RidePatNum}`);

        switch ($(deleteBtn).parents("table:first")[0].id) {
            case tableNames.MORNING:
                thisTable = tMorning;
                break;
            case tableNames.AFTERNOON:
                thisTable = tAfternoon;
                break;
            case tableNames.TOMORROW_MORNING:
                thisTable = tTomorrowMorning
                break;
            case tableNames.TOMORROW_AFTERNOON:
                thisTable = tTomorrowAfternoon
                break;
            case tableNames.FUTURE:
                thisTable = tFuture;
                break;
        }

    }
    let ridepat = returnRidePat;
    returnRidePat = null;
    $('#messagesModal').modal('toggle');
    $('#DriverReturnRidePatMsg').html("");
    deleteBtn = deleteBtn.children[deleteBtn.children.length - 1].children[0].children[1]

    deleteUnityRide(ridepat, deleteBtn, thisTable);
}
function deleteReturn() {

    var RidePat = new Object();
    RidePat.RidePatNum = returnRidePat.RidePatNum;
    //   var tempDate = dateReturn; --wtf ?! YOGEV disable it
    var tempDate = "01/01/1970"; // Fictional date for deletion
    var tempDateReveresed = tempDate.split('/').reverse().join('/')
    var date3 = new Date(tempDateReveresed);
    RidePat.Date = date3;
    if (isNaN(date3.getTime())) {
        var date3 = new Date();
        var tempHour = tempDate.split(':');
        date3.setHours(tempHour[0])
        RidePat.Date = date3;
    }
    var Pat = new Object();
    Pat.DisplayName = returnRidePat.Pat.DisplayName;
    RidePat.Pat = Pat;
    var Destination = new Object();
    Destination.Name = returnRidePat.Destination.Name;
    RidePat.Destination = Destination;
    var Origin = new Object();
    Origin.Name = returnRidePat.Origin.Name;
    RidePat.Origin = Origin;

    let thisTable = null;
    let deleteBtn = null;

    if (document.getElementById(`${RidePat.RidePatNum}`) !== null) { //DUE TO SORTING THE ROW IS NOT RENDERED


        deleteBtn = document.getElementById(`${RidePat.RidePatNum}`);

        switch ($(deleteBtn).parents("table:first")[0].id) {
            case tableNames.MORNING:
                thisTable = tMorning;
                break;
            case tableNames.AFTERNOON:
                thisTable = tAfternoon;
                break;
            case tableNames.TOMORROW_MORNING:
                thisTable = tTomorrowMorning
                break;
            case tableNames.TOMORROW_AFTERNOON:
                thisTable = tTomorrowAfternoon
                break;
            case tableNames.FUTURE:
                thisTable = tFuture;
                break;
        }

    }

    returnRidePat = null;
    $('#messagesModal').modal('toggle');
    $('#DriverReturnRidePatMsg').html("");
    deleteBtn = deleteBtn.children[deleteBtn.children.length - 1].children[0].children[1]

    deleteRidePat(RidePat, deleteBtn, thisTable);
}

const isThisReturnNonAnonymousRide = (ride) => {
    if (hospitals.includes(ride.Destination) && !ride.IsAnonymous) {
        return true;
    }
    else {
        return false;
    }


}


function closeModal() {
    $('#messagesModal').modal('hide');
    $('#DriverReturnRidePatMsg').html("");

    //הסטטוס 'אין נסיעת הלוך' עושה בעיות כיוון שלא ניתן להשתבץ אליו.
    //newStatus = "אין נסיעת הלוך"


}

const showFiltersModal = () => {

    ajaxCall(
        'getAreas_AsLocationObj',
        '',
        getAreas_SCB,
        getAreas_ECB
    );

    $('#filtersModal').modal({
        backdrop: 'static', //prevent popup to close when clicking outside
        keyboard: false     //prevent popup to close when press esc on the keyboard
    })
    $('#filtersModal').modal('show');
}

const getAreas_ECB = (err) => { console.log(err); }

const getAreas_SCB = (data) => {

    let listOfAreas = JSON.parse(data.d);

    let checkBoxesStr = '';

    const arrayOF_areasObj_isEmpty = localStorage.getItem('arrayOF_areasObj') === null || localStorage.getItem('arrayOF_areasObj') === 'null';

    let areasFromLs = JSON.parse(localStorage.getItem('arrayOF_areasObj'));


    if (!arrayOF_areasObj_isEmpty && listOfAreas.length > areasFromLs.length) {
        //case when 1 or more new areas added and ls is allready exist

        for (let i = areasFromLs.length; i < listOfAreas.length; i++) { //the gap between them
            areasFromLs.push({
                hebrewName: listOfAreas[i].Area,
                englishName: listOfAreas[i].EnglishName,
                checked: false
            });
        }
        localStorage.setItem('arrayOF_areasObj', JSON.stringify(areasFromLs));
    } else if (!arrayOF_areasObj_isEmpty && listOfAreas.length < areasFromLs.length) {
        localStorage.setItem('arrayOF_areasObj', null);
        location.reload();
    }

    for (let i = 0; i < listOfAreas.length; i++) {

        if (arrayOF_areasObj_isEmpty) {

            arrayOF_areasObj[i] = { hebrewName: listOfAreas[i].Area, englishName: listOfAreas[i].EnglishName }

            if (i === listOfAreas.length - 1) {
                localStorage.setItem('arrayOF_areasObj', JSON.stringify(arrayOF_areasObj));
            }
        }

        checkBoxesStr += `<div>`;
        checkBoxesStr += `<input type="checkbox" id="${listOfAreas[i].EnglishName}" checked />`;
        checkBoxesStr += `<label class="overRideBS_label" for="${listOfAreas[i].EnglishName}">&nbsp;${listOfAreas[i].Area}</label>`;
        checkBoxesStr += `</div>`;
    }
    document.getElementById('filtersModal_placeHolder').innerHTML = checkBoxesStr;

    if (!arrayOF_areasObj_isEmpty) {

        let checkBoxesContainer = document.getElementById('filtersModal_placeHolder').children;
        let arrayOF_areasObj = JSON.parse(localStorage.getItem('arrayOF_areasObj'));

        for (let i = 0; i < checkBoxesContainer.length; i++) {
            $(`#${arrayOF_areasObj[i].englishName}`).prop('checked', arrayOF_areasObj[i].checked);
        }

    }
}

const saveFilters = () => {

    let checkBoxesContainer = document.getElementById('filtersModal_placeHolder').children;
    let arrayOF_areasObj = JSON.parse(localStorage.getItem('arrayOF_areasObj'));

    let areasToShow_arr = [];

    for (let i = 0; i < checkBoxesContainer.length; i++) {
        arrayOF_areasObj[i] = {
            ...arrayOF_areasObj[i],
            checked: $(`#${arrayOF_areasObj[i].englishName}`).prop('checked')
        };

        if ($(`#${arrayOF_areasObj[i].englishName}`).prop('checked')) {
            areasToShow_arr.push(arrayOF_areasObj[i].hebrewName);
        }
    }

    localStorage.setItem('arrayOF_areasObj', JSON.stringify(arrayOF_areasObj));
    localStorage.setItem('areasToShow_arr', JSON.stringify(areasToShow_arr));

    refreshTable_UnityRide();
}

const allSortsSwitch_changed = () => {

    let considerSorts_controllers = $('#allSortsSwitch').prop('checked');
    localStorage.setItem('allSortsSwitch', JSON.stringify(considerSorts_controllers));

    if (!considerSorts_controllers) {

        $('#allSortsLabel').html('הפעל סינון');
        $('#openSortsControllers').prop('disabled', true);
        refreshTable_UnityRide();
    } else {

        $('#allSortsLabel').html('כבה סינון');
        $('#openSortsControllers').prop('disabled', false);
        refreshTable_UnityRide();
    }

}

const classManager = {

    getSuitableClassS_4thisTime: (tableName, hh = -1, mm = -1) => {
        let suitableClass;

        switch (tableName) {
            case "datatable-morning":
                suitableClass = 'withIn12HoursDrive';
                break;
            case "datatable-afternoon":
                suitableClass = classManager.calculateSuitableClass_24HoursRange(hh, mm);
                break;
            case "datatable-tomorrow-morning":
                suitableClass = classManager.calculateSuitableClass_24HoursRange(
                    hh,
                    mm,
                    false
                );
                break;
            case "datatable-future":
                suitableClass = 'initialStyle';
                break;
        }
        return suitableClass;
    },

    calculateSuitableClass_24HoursRange: (hh, mm, isToday = true) => {

        let now = new Date();
        let ridePatTimeObj = {
            hh: parseInt(hh),
            mm: parseInt(mm),
        };
        const addons = isToday ? 0 : 1;

        let ridePatTime = new Date(
            now.getFullYear(),
            now.getMonth(),
            now.getDate() + addons,
            ridePatTimeObj.hh,
            ridePatTimeObj.mm
        );


        let hours_gap = getHoursGap(ridePatTime, now);

        if (hours_gap >= 0 && hours_gap <= 24) {

            return hours_gap <= 12 ? 'withIn12HoursDrive' : 'withIn24HoursDrive';
        }

        return 'initialStyle';
    },
};

const isWithin2HoursSinceChange = (ridePatLastModified) => {
    let now = new Date();
    const hoursGap = getHoursGap(ridePatLastModified, now);
    return (hoursGap >= -2 && hoursGap <= 0);
}

const getNext_X_DaysDateAsString = (days) => {

    if (typeof days != 'number') {
        return;
    }

    let today = new Date();
    let futureIntRepresentation = today.setDate(today.getDate() + days);
    let futureDate = new Date(futureIntRepresentation);
    let finalOutPut = futureDate.toLocaleString('he-IL', { dateStyle: "short" });

    return finalOutPut;
}

// just to check the new api gilad
const CHECK_TEST_NOT_REAL = () => {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetUnityRide",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ days: 7 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);

        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in GetRidePatView: " + err.responseText);
        }
    });
}
const refreshTable_splitRide = () => {
    //DateTime rideDate, bool isAfternoon, bool isFutureTable, int days
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetSplitRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ dateMode: DateMode.Today, isAfternoon: false, isFutureTable: false, days: 0 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);
            console.log('Gilad check this today morning -->', test)


        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in refreshTable_splitRide: " + err.responseText);
        }
    });
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetSplitRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ dateMode: DateMode.Today, isAfternoon: true, isFutureTable: false, days: 0 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);
            console.log('Gilad check this today afternoon -->', test)


        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in refreshTable_splitRide: " + err.responseText);
        }
    });
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetSplitRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ dateMode: DateMode.Tomorrow, isAfternoon: false, isFutureTable: false, days: 0 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);
            console.log('Gilad check this Tomorrow morning -->', test)


        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in refreshTable_splitRide: " + err.responseText);
        }
    });
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetSplitRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ dateMode: DateMode.Tomorrow, isAfternoon: true, isFutureTable: false, days: 0 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);
            console.log('Gilad check this Tomorrow afternoon -->', test)


        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in refreshTable_splitRide: " + err.responseText);
        }
    });
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetSplitRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ dateMode: DateMode.Future, isAfternoon: false, isFutureTable: true, days: 7 }),
        success: function (data) {
            $('#wait').hide();
            const test = JSON.parse(data.d);
            console.log('Gilad check this Future -->', test)


        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in refreshTable_splitRide: " + err.responseText);
        }
    });
}

//This one take from unityRide the data *work*
function refreshTable_UnityRide(timeInterval = 7) {
    $('#wait').show();

    morningRidePats = [];
    afterNoonRidePats = [];
    tomorrowMorningRidePats = [];
    tomorrowAfternoonRidePats = [];
    futureRidePats = [];

    let areasToShow_arr = JSON.parse(localStorage.getItem('areasToShow_arr'));
    let allSortsSwitch = JSON.parse(localStorage.getItem('allSortsSwitch'));

    checkCookie();

    if (areasToShow_arr !== null && areasToShow_arr.length === 0 && allSortsSwitch) { //dont show any
        $('#wait').hide();

        tMorning = $(`#${tableNames.MORNING}`).DataTable({ data: [], destroy: true });

        tAfternoon = $(`#${tableNames.AFTERNOON}`).DataTable({ data: [], destroy: true });

        tTomorrowMorning = $(`#${tableNames.TOMORROW_MORNING}`).DataTable({ data: [], destroy: true });

        tTomorrowAfternoon = $(`#${tableNames.TOMORROW_AFTERNOON}`).DataTable({ data: [], destroy: true });

        tFuture = $(`#${tableNames.FUTURE}`).DataTable({ data: [], destroy: true });
        return;
    }
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetUnityRide",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ days: timeInterval }),
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);
            GENERAL.RIDEPAT.setRidePatList(JSON.stringify(arr_rides));

            let thisRidePatDate;
            let thisRidePat = {};
            let editBtn;
            let deleteBtn;
            let messageToCopyBtn;
            let candidateBtn;
            let btnStr = '';
            //     let lastModified = convertDBDate2FrontEndDate(arr_rides[i].LastModified).toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            let numOfRealEscorts = 0; //not anonymous
            let driverColumnContent = '';
            let englishStatus = '';
            let patientHTML;
            let driverHTML;
            let status2render;
            let timeHTML;
            let patientGender = null;
            let patientAge = null;
            let isPatientStatusShouldPaintRow = false;
            let remarkHTML = '';
            listRowstoColor = [];


            for (let i = 0; i < arr_rides.length; i++) {
                if (allSortsSwitch && areasToShow_arr !== null) {
                    if (areasToShow_arr.length !== 0) {
                        if (!areasToShow_arr.includes(arr_rides[i].Area)) {
                            continue;
                        }
                    }
                }
                thisRidePatDate = convertDBDate2FrontEndDate(arr_rides[i].PickupTime);
                timeHTML = buildTimeHTML(thisRidePatDate, arr_rides[i].RidePatNum);
                numOfEscorts = arr_rides[i].AmountOfEscorts;

                //#region adjust patient column
                patientHTML = buildPatientHTML_AfterUnited(arr_rides[i]);
                //#endregion

                //#region adjust driver column
                driverHTML = '';
                let driversArr = [];
                if (arr_rides[i].DriverName != '') {
                    //Custom obj for buildDriverHTML and buildAcionsButtonsHTML functions.
                    let MainDriver = {
                        Id: arr_rides[i].MainDriver,
                        DisplayName: arr_rides[i].DriverName,
                        EnglishFN: '',
                        EnglishLN: '',
                        CellPhone: arr_rides[i].DriverCellPhone,
                        NoOfDocumentedRides: arr_rides[i].NoOfDocumentedRides,
                        IsNewDriver: arr_rides[i].IsNewDriver

                    }
                    driversArr.push(MainDriver);
                    let RideNum = 1;
                    driverHTML = buildDriverHTML(arr_rides[i].RidePatNum, MainDriver, RideNum);
                } else {

                    driverHTML = buildDriverHTML(arr_rides[i].RidePatNum);
                }
                //#endregion adjust driver column

                //#region Status
                //custom for buildstatusString function
                let statusObj = {}
                if (arr_rides[i].PatientStatus == '' || arr_rides[i].PatientStatus == 'Not Finished') {
                    statusObj.Status = -1;
                }
                else {
                    statusObj.Status = 1;

                }
                statusObj.EditTimeStamp = arr_rides[i].PatientStatusEditTime;
                status2render = buildStatusString(statusObj, arr_rides[i].RidePatNum);
                //#endregion Status

                //#region remarkHTML
                remarkHTML = buildRemarkHTML(arr_rides[i].Remark, arr_rides[i].RidePatNum);
                //#endregion remarkHTML

                //#region patient age

                //custom object for buildPatientAgeHTML function.
                let ageObj = {}
                ageObj.PatientName = arr_rides[i].PatientName;
                ageObj.Age = arr_rides[i].PatientAge;
                ageObj.GenderAsEnum = arr_rides[i].PatientGender;
                ageObj.IsAnonymous = arr_rides[i].IsAnonymous;
                patientAge = buildPatientAgeHTML_AfterUnited(ageObj);
                //#endregion


                //#region buttons

                btnStr = buildAcionsButtonsHTML(arr_rides[i].RidePatNum, driversArr);
                //#endregion buttons

                //#region isPatientStatusShouldPaintRow
                isPatientStatusShouldPaintRow = callIfPatientStatusShouldPaintRow_AfterUnited(arr_rides[i]);
                //#endregion isPatientStatusShouldPaintRow

                // if need to add who change in the last modified col just add this line :
                // + '<br><p class="whochange" >'+'על ידי - '+arr_rides[i].CoorName+'</p>'
                //to this objcet "thisRidePat".


                thisRidePat = {
                    checkBoxesStr: `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${arr_rides[i].RidePatNum}">`,
                    ridePatNum: arr_rides[i].RidePatNum,
                    time: timeHTML,
                    //origin: isAssistant ? arr_rides[i].Origin.EnglishName : arr_rides[i].Origin.Name,
                    //destination: isAssistant ? arr_rides[i].Destination.EnglishName : arr_rides[i].Destination.Name,
                    origin: arr_rides[i].Origin,
                    destination: arr_rides[i].Destination,
                    patient: patientHTML,
                    patientAge: patientAge,
                    driver: driverHTML,
                    status: status2render,
                    lastModified: convertDBDate2FrontEndDate(arr_rides[i].LastModified).toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" }),
                    remark: remarkHTML,
                    isAnonymous: arr_rides[i].IsAnonymous == true,
                    hasDriver: arr_rides[i].MainDriver != -1,
                    buttons: btnStr,
                    isPatientStatusShouldPaintRow: isPatientStatusShouldPaintRow,
                    type: 0
                }

                if (thisRidePatDate.toLocaleDateString() === new Date().toLocaleDateString()) {                         //<---- today
                    if (thisRidePatDate.getHours() < 12) {                                                              //<---- today morning
                        thisRidePat.type = 1;
                        morningRidePats.push(thisRidePat);
                    } else {                                                                                           //<---- today afternoon
                        thisRidePat.type = 2;
                        afterNoonRidePats.push(thisRidePat);
                    }

                }
                else if (thisRidePatDate.toLocaleDateString() === tomorrowDate.toLocaleDateString()) {              //<---- tomorrow

                    if (thisRidePatDate.getHours() < 12) {                                                           //<---- tomorrow morning
                        thisRidePat.date = thisRidePatDate.toLocaleString('he-IL', { dateStyle: 'short' });
                        thisRidePat.type = 3
                        tomorrowMorningRidePats.push(thisRidePat);
                    } else {                                                                                         //<---- tomorrow afternoon
                        thisRidePat.date = thisRidePatDate.toLocaleString('he-IL', { dateStyle: 'short' });
                        thisRidePat.type = 4
                        tomorrowAfternoonRidePats.push(thisRidePat);
                    }

                }
                else if (thisRidePatDate > tomorrowDate) {                                                         //<---- future

                    thisRidePat.date = thisRidePatDate.toLocaleString('he-IL', { dateStyle: 'short' });
                    thisRidePat.type = 5;
                    futureRidePats.push(thisRidePat);
                }

                //here need to be the logic of coloring the row

                const hourRide = convertDBDate2FrontEndDate(arr_rides[i].PickupTime).getHours();
                const anonymous = arr_rides[i].PatientName.includes('אנונימי') || arr_rides[i].IsAnonymous;
                if (!anonymous && arr_rides[i].MainDriver > 0 && hourRide <= 21) {
                    listRowstoColor.push(arr_rides[i].RidePatNum);
                }


            }
            let arrayOf_IHaveSeenAllready = localStorage.getItem('arrayOf_IHaveSeenAllready') ?
                JSON.parse(localStorage.getItem('arrayOf_IHaveSeenAllready'))
                :
                [];

            //           ============================
            //           || ↓DATATABLES PROPERTIES↓||
            //           ============================

            tMorning = $(`#${tableNames.MORNING}`).DataTable({
                data: morningRidePats,
                rowId: 'ridePatNum',
                dom: controllersLayout,
                buttons: [],
                pageLength: 100,
                stateSave: true,
                destroy: true,
                autoWidth: false,
                columns: [
                    //when add column be aware of columnDefs refernces [i] IMPORTANT !!!
                    { data: "checkBoxesStr" },                          //0
                    { data: "time" },                                //1
                    { data: "origin" },                              //2
                    { data: "destination" },                         //3
                    { data: "patient" },                             //4
                    { data: "patientAge" },                          //5
                    { data: "driver" },                              //6
                    { data: "status" },                              //7
                    { data: "lastModified" },                        //8
                    { data: "remark" },                              //9
                    { data: "isAnonymous" },                         //10
                    { data: "buttons" },                             //11

                ],
                columnDefs: [
                    { "targets": [0], "orderable": false },
                    { width: '5%', "targets": [0, 1, 2, 5, 6, 7, 8] },
                    { width: '2%', "targets": [5] },
                    { width: '15%', "targets": [4] },
                    { width: '25%', "targets": [10] },
                    { targets: 1, type: 'de_time' },
                    {
                        targets: [2, 3, 4, 6, 7, 8], createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {
                            if (isAssistant) {
                                cell.className += ' ltr';
                            }

                            switch (colIndex) {
                                case 6: //driver column

                                    $(cell).addClass('driver_td');
                                    if (!rowData.hasDriver) {
                                        const suitableClass = classManager.getSuitableClassS_4thisTime("datatable-morning");
                                        $(cell).addClass(suitableClass);
                                    }
                                    break;

                                case 7://status column
                                    if (
                                        cellData.indexOf("הגענו ליעד") !== -1 ||
                                        cellData.indexOf("Reached destination") !== -1
                                    ) {
                                        cell.style.backgroundColor = "green";
                                        cell.style.color = "white";
                                    }
                                    else if (cellData.includes('שובץ נהג') || cellData.includes('Driver registered')) {

                                        if (rowData["isAnonymous"]) {
                                            $(cell.parentElement).addClass('driverAssign2AnonymousRide');
                                        }
                                    }
                                    break;
                                case 8://lastModified column

                                    const thisRidePatNum = rowData.ridePatNum;
                                    const thisridePatObj = arr_rides.find((r) => r.RidePatNum === thisRidePatNum);
                                    const intValue = convert2DBDateToInt(thisridePatObj.LastModified);//the reasone why we get the origial obj is to support pc with english date
                                    const thisRidePatLastModified = new Date(intValue)
                                    const isSeenAllReady = arrayOf_IHaveSeenAllready.includes(thisRidePatNum);

                                    if (isWithin2HoursSinceChange(thisRidePatLastModified) && !isSeenAllReady) {
                                        cell.classList.add('within2HoursSinceChange');
                                        increaseCounter(tableNames.MORNING, thisRidePatNum);
                                    }
                                    break;

                            }

                        }
                    },
                    { targets: 8, className: 'last-modified_td' },
                    { targets: 8, type: 'last_modifiedSort' },
                    { "targets": [10], visible: false },
                    { targets: [11], orderable: false },
                    { targets: [0], className: 'text-center' }

                ],
                createdRow: function (row, data, dataIndex) {

                    if (data.isPatientStatusShouldPaintRow) {

                        $(row).addClass("highlightRow-after-patientStatusModal");
                    }
                },
            });

            tAfternoon = $(`#${tableNames.AFTERNOON}`).DataTable({
                rowId: 'ridePatNum',
                dom: controllersLayout,
                buttons: [],
                pageLength: 100,
                data: afterNoonRidePats,
                stateSave: true,
                destroy: true,
                columns: [
                    //when add column be aware of columnDefs refernces [i] IMPORTANT !!!

                    { data: "checkBoxesStr" },                       //0
                    { data: "time" },                                //1
                    { data: "origin" },                              //2
                    { data: "destination" },                         //3
                    { data: "patient" },                             //4
                    { data: "patientAge" },                          //5
                    { data: "driver" },                              //6
                    { data: "status" },                              //7
                    { data: "lastModified" },                        //8
                    { data: "remark" },                              //9
                    { data: "isAnonymous" },                         //10
                    { data: "buttons" },                             //11
                    //{ data: "time" },                                //0
                    //{ data: "origin" },                              //1
                    //{ data: "destination" },                         //2
                    //{ data: "patient" },                             //3
                    //{ data: "patientAge" },                          //4
                    //{ data: "driver" },                              //5
                    //{ data: "status" },                              //6
                    //{ data: "lastModified" },                        //7
                    //{ data: "remark" },                              //8
                    //{ data: "isAnonymous" },                         //9
                    //{ data: "buttons" },                             //10
                ],
                columnDefs: [
                    { "targets": [0], "orderable": false },
                    { width: '5%', "targets": [0, 1, 2, 3, 6, 7, 8, 9] },
                    { width: '2%', "targets": [5] },
                    { width: '15%', "targets": [4] },
                    { width: '25%', "targets": [11] },
                    { targets: 1, type: 'de_time' },
                    { targets: [0], className: 'text-center' },
                    { targets: [1], createdCell: (td) => { $(td).attr('id', 'rideTiming') } },
                    {
                        targets: [2, 3, 4, 6, 7, 8], createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {
                            if (isAssistant) {
                                cell.className += ' ltr';
                            }

                            switch (colIndex) {

                                case 2: //origin
                                    $(cell).attr('id', 'OriginName');
                                    if (cellData.indexOf('מרכז') !== -1
                                        || cellData.indexOf('center') !== -1
                                        || rowData['destination'].indexOf('מרכז') !== -1
                                        || rowData['destination'].indexOf('center') !== -1
                                    ) {
                                        $(cell.parentElement).addClass('driverAssign2AnonymousRide');
                                    }
                                    break;

                                case 3://destination
                                    createdCell: (td) => { $(td).attr('id', 'DestinationName') }
                                    break;

                                case 6: //driver
                                    //this code is identical to the one in datatable-tomorrow-morning when can need to change to 1 function
                                    $(cell).addClass('driver_td');
                                    if (!rowData.hasDriver) {

                                        let ridePatTime_td;
                                        if (rowData['time'] === 'אחה"צ') {

                                            ridePatTime_td = [13, 1];
                                        } else {

                                            ridePatTime_td = $.trim(rowData['time']).split(':');
                                        }

                                        const hh = parseInt(ridePatTime_td[0]);
                                        const mm = parseInt(ridePatTime_td[1]);

                                        const suitableClass = classManager.getSuitableClassS_4thisTime("datatable-afternoon", hh, mm);
                                        $(cell).addClass(suitableClass);

                                    }
                                    break;

                                case 7://status
                                    if (cellData.indexOf("הגענו ליעד") !== -1 ||
                                        cellData.indexOf("Reached destination") !== -1) {

                                        $(cell).addClass('reached2destination');
                                    } else if (cellData.includes('שובץ נהג') || cellData.includes('Driver registered')) {

                                        if (rowData["isAnonymous"]) {

                                            $(cell.parentElement).addClass('driverAssign2AnonymousRide');
                                        }
                                    }
                                    break;
                                case 8://lastModified column

                                    const thisRidePatNum = rowData.ridePatNum;
                                    const thisridePatObj = arr_rides.find((r) => r.RidePatNum === thisRidePatNum);
                                    const intValue = convert2DBDateToInt(thisridePatObj.LastModified);//the reasone why we get the origial obj is to support pc with english date
                                    const thisRidePatLastModified = new Date(intValue)
                                    const isSeenAllReady = arrayOf_IHaveSeenAllready.includes(thisRidePatNum);

                                    if (isWithin2HoursSinceChange(thisRidePatLastModified) && !isSeenAllReady) {
                                        cell.classList.add('within2HoursSinceChange');
                                        increaseCounter(tableNames.AFTERNOON, thisRidePatNum);
                                    }
                                    break;

                            }

                        }
                    },
                    { targets: 8, className: 'last-modified_td' },
                    { targets: 8, type: 'last_modifiedSort' },
                    { targets: [10], visible: false },
                    { targets: [11], orderable: false }
                ],
                autoWidth: false,
                createdRow: function (row, data, dataIndex) {

                    if (data.isPatientStatusShouldPaintRow) {

                        $(row).addClass("highlightRow-after-patientStatusModal");
                    }
                },
            });

            tTomorrowMorning = $(`#${tableNames.TOMORROW_MORNING}`).DataTable({
                rowId: 'ridePatNum',
                dom: controllersLayout,
                buttons: [],
                data: tomorrowMorningRidePats,
                stateSave: true,
                destroy: true,
                "pageLength": 100,
                columns: [
                    //when add column be aware of columnDefs refernces [i] IMPORTANT !!!

                    { data: "checkBoxesStr" },                       //0
                    { data: "date" },                                //1
                    { data: "time" },                                //2
                    { data: "origin" },                              //3
                    { data: "destination" },                         //4
                    { data: "patient" },                             //5
                    { data: "patientAge" },                          //6
                    { data: "driver" },                              //7
                    { data: "status" },                              //8
                    { data: "lastModified" },                        //9
                    { data: "remark" },                              //10
                    { data: "isAnonymous" },                         //11
                    { data: "buttons" },                             //12

                    //{ data: "date" },                                //0
                    //{ data: "time" },                                //1
                    //{ data: "origin" },                              //2
                    //{ data: "destination" },                         //3
                    //{ data: "patient" },                             //4
                    //{ data: "patientAge" },                          //5
                    //{ data: "driver" },                              //6
                    //{ data: "status" },                              //7
                    //{ data: "lastModified" },                        //8
                    //{ data: "remark" },                              //9
                    //{ data: "isAnonymous" },                         //10
                    //{ data: "buttons" },                             //11
                ],
                "columnDefs": [
                    { targets: 2, type: 'de_time' },
                    { "targets": [0], "orderable": false },
                    { width: '4%', "targets": [1, 2] },
                    { width: '6%', "targets": [3, 4] },
                    { width: '15%', "targets": [5] },
                    { width: '2%', "targets": [6] },
                    { width: '5%', "targets": [0, 7, 8, 9, 10] },
                    { width: '0%', "targets": [11] },
                    { width: '25%', "targets": [12] },
                    { targets: [0], className: 'text-center' },
                    {
                        targets: 6, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => { //5 is driver column

                            cell.className += ' driver_td';
                            if (!rowData.hasDriver) {

                                let ridePatTime_td;
                                if (rowData['time'] === 'אחה"צ') {

                                    ridePatTime_td = [12, 1];
                                } else {

                                    ridePatTime_td = $.trim(rowData['time']).split(':');
                                }

                                const hh = parseInt(ridePatTime_td[0]);
                                const mm = parseInt(ridePatTime_td[1]);

                                const suitableClass = classManager.getSuitableClassS_4thisTime("datatable-tomorrow-morning", hh, mm);
                                $(cell).addClass(suitableClass);

                            }
                        }
                    },
                    {
                        targets: 8, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => { //6 is status column
                            if (cellData.indexOf("הגענו ליעד") !== -1 ||
                                cellData.indexOf("Reached destination") !== -1) {

                                $(cell).addClass('reached2destination');
                            } else if (cellData.includes('שובץ נהג') || cellData.includes('Driver registered')) {

                                if (rowData["isAnonymous"]) {

                                    $(cell.parentElement).addClass('driverAssign2AnonymousRide');
                                }
                            }
                        }
                    },
                    {
                        targets: 9, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {//lastModified column

                            const thisRidePatNum = rowData.ridePatNum;
                            const thisridePatObj = arr_rides.find((r) => r.RidePatNum === thisRidePatNum);
                            const intValue = convert2DBDateToInt(thisridePatObj.LastModified);//the reasone why we get the origial obj is to support pc with english date
                            const thisRidePatLastModified = new Date(intValue)
                            const isSeenAllReady = arrayOf_IHaveSeenAllready.includes(thisRidePatNum);

                            if (isWithin2HoursSinceChange(thisRidePatLastModified) && !isSeenAllReady) {
                                cell.classList.add('within2HoursSinceChange');
                                increaseCounter(tableNames.TOMORROW_MORNING, thisRidePatNum);
                            }
                        }
                    },
                    { targets: 9, className: 'last-modified_td' },
                    { targets: 9, type: 'last_modifiedSort' },
                    { targets: 10, className: 'text-right-strong' },
                    { targets: [11], visible: false },
                    { targets: [12], orderable: false }

                ],
                autoWidth: false,
                createdRow: function (row, data, dataIndex) {

                    if (data.isPatientStatusShouldPaintRow) {

                        $(row).addClass("highlightRow-after-patientStatusModal");
                    }
                },

            });

            tTomorrowAfternoon = $(`#${tableNames.TOMORROW_AFTERNOON}`).DataTable({
                rowId: 'ridePatNum',
                dom: controllersLayout,
                buttons: [],
                data: tomorrowAfternoonRidePats,
                stateSave: true,
                destroy: true,
                "pageLength": 100,
                columns: [
                    //when add column be aware of columnDefs refernces [i] IMPORTANT !!!

                    { data: "checkBoxesStr" },                       //0
                    { data: "date" },                                //1
                    { data: "time" },                                //2
                    { data: "origin" },                              //3
                    { data: "destination" },                         //4
                    { data: "patient" },                             //5
                    { data: "patientAge" },                          //6
                    { data: "driver" },                              //7
                    { data: "status" },                              //8
                    { data: "lastModified" },                        //9
                    { data: "remark" },                              //10
                    { data: "isAnonymous" },                         //11
                    { data: "buttons" },                             //12




                    //{ data: "date" },                                //0
                    //{ data: "time" },                                //1
                    //{ data: "origin" },                              //2
                    //{ data: "destination" },                         //3
                    //{ data: "patient" },                             //4
                    //{ data: "patientAge" },                          //5
                    //{ data: "driver" },                              //6
                    //{ data: "status" },                              //7
                    //{ data: "lastModified" },                        //8
                    //{ data: "remark" },                              //9
                    //{ data: "isAnonymous" },                         //10
                    //{ data: "buttons" },                             //11
                ],
                "columnDefs": [
                    { targets: 2, type: 'de_time' },
                    { width: '4%', "targets": [1, 2] },
                    { width: '6%', "targets": [3, 4] },
                    { width: '15%', "targets": [5] },
                    { width: '2%', "targets": [6] },
                    { width: '5%', "targets": [0, 7, 8, 9, 10] },
                    { width: '0%', "targets": [11] },
                    { width: '25%', "targets": [12] },
                    {
                        targets: 6, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => { //5 is driver column

                            cell.className += ' driver_td';
                            if (!rowData.hasDriver) {

                                let ridePatTime_td;
                                if (rowData['time'] === 'אחה"צ') {

                                    ridePatTime_td = [12, 1];
                                } else {

                                    ridePatTime_td = $.trim(rowData['time']).split(':');
                                }

                                const hh = parseInt(ridePatTime_td[0]);
                                const mm = parseInt(ridePatTime_td[1]);

                                const suitableClass = classManager.getSuitableClassS_4thisTime("datatable-tomorrow-morning", hh, mm);
                                $(cell).addClass(suitableClass);

                            }
                        }
                    },
                    {
                        targets: 8, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => { //6 is status column
                            if (cellData.indexOf("הגענו ליעד") !== -1 ||
                                cellData.indexOf("Reached destination") !== -1) {

                                $(cell).addClass('reached2destination');
                            } else if (cellData.includes('שובץ נהג') || cellData.includes('Driver registered')) {

                                if (rowData["isAnonymous"]) {

                                    $(cell.parentElement).addClass('driverAssign2AnonymousRide');
                                }
                            }
                        }
                    },
                    {
                        targets: 9, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {//lastModified column

                            const thisRidePatNum = rowData.ridePatNum;
                            const thisridePatObj = arr_rides.find((r) => r.RidePatNum === thisRidePatNum);
                            const intValue = convert2DBDateToInt(thisridePatObj.LastModified);//the reasone why we get the origial obj is to support pc with english date
                            const thisRidePatLastModified = new Date(intValue)
                            const isSeenAllReady = arrayOf_IHaveSeenAllready.includes(thisRidePatNum);

                            if (isWithin2HoursSinceChange(thisRidePatLastModified) && !isSeenAllReady) {
                                cell.classList.add('within2HoursSinceChange');
                                increaseCounter(tableNames.TOMORROW_MORNING, thisRidePatNum);
                            }
                        }
                    },
                    { targets: 9, className: 'last-modified_td' },
                    { targets: 9, type: 'last_modifiedSort' },
                    { targets: 10, className: 'text-right-strong' },
                    { targets: [11], visible: false },
                    { targets: [12], orderable: false },
                    { targets: [0], orderable: false },
                    { targets: [0], className: 'text-center' }

                ],
                autoWidth: false,
                createdRow: function (row, data, dataIndex) {

                    if (data.isPatientStatusShouldPaintRow) {

                        $(row).addClass("highlightRow-after-patientStatusModal");
                    }
                },

            });


            tFuture = $(`#${tableNames.FUTURE}`).DataTable({
                rowId: 'ridePatNum',
                dom: controllersLayoutForFutureTable,
                fnInitComplete: function (x) {
                    buildHtmlSelectTimeInterval();
                },
                buttons: [],
                data: futureRidePats,
                stateSave: true,
                destroy: true,
                columns: [
                    //when add column be aware of columnDefs refernces [i] IMPORTANT !!!

                    { data: "checkBoxesStr" },                       //0
                    { data: "date" },                                //1
                    { data: "time" },                                //2
                    { data: "origin" },                              //3
                    { data: "destination" },                         //4
                    { data: "patient" },                             //5
                    { data: "patientAge" },                          //6
                    { data: "driver" },                              //7
                    { data: "status" },                              //8
                    { data: "lastModified" },                        //9
                    { data: "remark" },                              //10
                    { data: "isAnonymous" },                         //11
                    { data: "buttons" },                             //12


                    //{ data: "date" },                                //0
                    //{ data: "time" },                                //1
                    //{ data: "origin" },                              //2
                    //{ data: "destination" },                         //3
                    //{ data: "patient" },                             //4
                    //{ data: "patientAge" },                          //5
                    //{ data: "driver" },                              //6
                    //{ data: "status" },                              //7
                    //{ data: "lastModified" },                        //8
                    //{ data: "remark" },                              //9
                    //{ data: "isAnonymous" },                         //10
                    //{ data: "buttons" },                             //11
                ],
                pageLength: 100,
                columnDefs: [
                    { targets: 1, type: 'de_date' },
                    { targets: 2, type: 'de_time' },
                    { targets: 8, type: 'last_modifiedSort' },
                    { width: '4%', "targets": [1, 2] },
                    { width: '6%', "targets": [3, 4] },
                    { width: '10%', "targets": [5] },
                    { width: '2%', "targets": [6] },
                    { width: '5%', "targets": [0, 7, 8, 9, 10] },
                    { width: '0%', "targets": [11] },
                    { width: '25%', "targets": [12] },
                    { targets: 7, className: 'driver_td' },
                    {
                        targets: 9, createdCell: (cell, cellData, rowData, rowIndex, colIndex) => {//lastModified column

                            const thisRidePatNum = rowData.ridePatNum;
                            const thisridePatObj = arr_rides.find((r) => r.RidePatNum === thisRidePatNum);
                            const intValue = convert2DBDateToInt(thisridePatObj.LastModified);
                            const thisRidePatLastModified = new Date(intValue)
                            const isSeenAllReady = arrayOf_IHaveSeenAllready.includes(thisRidePatNum);

                            if (isWithin2HoursSinceChange(thisRidePatLastModified) && !isSeenAllReady) {
                                cell.classList.add('within2HoursSinceChange');
                                increaseCounter(tableNames.FUTURE, thisRidePatNum);
                            }
                        }
                    },
                    { targets: 9, className: 'last-modified_td' },
                    { targets: 9, type: 'last_modifiedSort' },
                    { targets: [11], visible: false },
                    { targets: [12], orderable: false },
                    { targets: [0], orderable: false },
                    { targets: [0], className: 'text-center' }
                ],
                autoWidth: false,
                createdRow: function (row, data, dataIndex) {

                    if (data.isPatientStatusShouldPaintRow) {

                        $(row).addClass("highlightRow-after-patientStatusModal");
                    }
                },
            });

            //            ============================
            //            || ↑DATATABLES PROPERTIES↑||
            //            ============================

            //colorRowWithDriver(listRowstoColor);
            colorRowBeige(listRowstoColor);
            $(".dataTables_empty").html("אין הסעות لا توجد سفريات No drives");

            $('.checkboxClass').prop('checked', false);



        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in GetRidePatView: " + err.responseText);
        }
    });
}
function findRideByNumber(rideNumber) {
    // Iterate through each ride object in the array
    for (let i = 0; i < arr_rides.length; i++) {
        const ride = arr_rides[i];
        // Check if the ride number matches the provided rideNumber
        if (ride.RidePatNum === rideNumber) {
            // Return the ride object if found
            return ride;
        }
    }
    // If the ride is not found, return null or handle it as needed
    console.warn(`Ride with number ${rideNumber} not found.`);
    return null;
}

//gilad
const colorRowWithDriver = (rideArray) => {

    if (rideArray.length == 0) {
        return;
    }
    rideArray.forEach(rideID => {
        if (document.getElementById(rideID) == null) {
            return
        }
        let timespan = ''
        let PatientNameSpan = '';
        //if (document.getElementById(rideID).children[0].children.length == 0) {
        //    timespan = document.getElementById(rideID).children[1].children[0].children[0].innerHTML
        //    PatientNameSpan = document.getElementById(rideID).children[4].children[0].innerHTML


        //}
        //else {
        //    timespan = document.getElementById(rideID).children[2].children[0].children[0].innerHTML;
        //    PatientNameSpan = document.getElementById(rideID).children[3].children[0].innerHTML

        //}
        let thisRide = findRideByNumber(rideID);

        const isAfterNoonRide = convertDBDate2FrontEndDate(thisRide.PickupTime).getMinutes() === afternoonIndicator;
        timespan = isAfterNoonRide ? 'אחה"צ' : 'בוקר';
        PatientNameSpan = thisRide.IsAnonymous ? 'חולה' : thisRide.PatientName;
        if (timespan != 'אחה"צ' && PatientNameSpan != 'חולה' && thisRide.MainDriver != -1) {
            $(`#${rideID}`).addClass('statusComplete');
        }
        else if (timespan == 'אחה"צ' || PatientNameSpan == 'חולה' || thisRide.MainDriver == -1) {
            $(`#${rideID}`).removeClass('statusComplete');

        }
    })
}
const colorRowBeige = (rideArray) => {
    rideArray.forEach(rideID => {
        $(`#${rideID}`).addClass('statusComplete');
    })
}

const callIfPatientStatusShouldPaintRow_AfterUnited = (ride) => {

    let result = false;
    //const status = ride.Pat.RidePatPatientStatus.Status;
    const status = ride.PatientStatus == 'Finished' ? 1 : -1;
    treatmentFinished = status === patientStatus.Finished;

    //const driverIsAssigned = ride.Drivers.length > 0;
    const driverIsAssigned = ride.MainDriver != -1;

    const thisRidePatDate = convertDBDate2FrontEndDate(ride.PickupTime);
    let isAfterNoonRide = false;

    if (typeof thisRidePatDate !== undefined) {

        if (isDate(thisRidePatDate)) {
            isAfterNoonRide = thisRidePatDate.getMinutes() === afternoonIndicator;
        }
    }

    if (treatmentFinished && ((!driverIsAssigned) || (driverIsAssigned && isAfterNoonRide))) {

        result = true;
    }

    return result;
}

const callIfPatientStatusShouldPaintRow = (ride) => {

    let result = false;
    const status = ride.Pat.RidePatPatientStatus.Status;
    treatmentFinished = status === patientStatus.Finished;

    const driverIsAssigned = ride.Drivers.length > 0;

    const thisRidePatDate = convertDBDate2FrontEndDate(ride.Date);
    let isAfterNoonRide = false;

    if (typeof thisRidePatDate !== undefined) {

        if (isDate(thisRidePatDate)) {
            isAfterNoonRide = thisRidePatDate.getMinutes() === afternoonIndicator;
        }
    }

    if (treatmentFinished && ((!driverIsAssigned) || (driverIsAssigned && isAfterNoonRide))) {

        result = true;
    }

    return result;
}


const buildPatientAgeHTML_AfterUnited = (patient) => {

    let age = '';
    const boy = 'בן ', girl = 'בת ';
    const gender = GetPatientGender(patient.GenderAsEnum, isAssistant)
    const isMaleHebrew = gender === maleInHebrew;
    const isMaleEnglish = gender === male;

    if (gender.length === 0 && IsNullOrUndefined(patient.Age)) {
        return '';
    }

    if (IsNullOrUndefined(patient.Age)) {

        let result = '';

        if (!isAssistant) {

            result = isMaleHebrew ? boy : girl;
        } else {

            result = isMaleEnglish ? male : female;
        }

        return result;
    }

    if (patient.IsAnonymous !== true) {
        const isGenderEmpty = gender === '';
        age = '<span>'

        if (!isAssistant) {

            age += isMaleHebrew ? boy : gender === femaleInHebrew ? girl : '';
        }
        else {

            age += isMaleEnglish ? 'his age ' : gender === female ? 'her age ' : '';
        }

        age += patient.Age;
        age += '</span>';
    }
    //let ageNumber = patient.Age == 0 ? '' : patient.Age;
    if (patient.GenderAsEnum == -1 && !patient.IsAnonymous) {


        age = `<span>בגיל ${patient.Age}</span>`;

    }

    if (patient.Age == 1) {

        if (patient.GenderAsEnum == -1) {
            age = `<span>בגיל שנה</span>`;
        }
        else if (patient.GenderAsEnum == 0) {
            age = `<span>בת שנה</span>`;
        }
        else if (patient.GenderAsEnum == 1) {
            age = `<span>בן שנה</span>`;
        }

    }
    if (patient.Age == 0 || patient.IsAnonymous) {
        age = `<span></span>`;
    }

    return age;
}
const buildPatientAgeHTML = (patient) => {
    let age = '';
    const boy = 'בן ', girl = 'בת ';
    const gender = GetPatientGender(patient.GenderAsEnum, isAssistant)
    const isMaleHebrew = gender === maleInHebrew;
    const isMaleEnglish = gender === male;

    if (gender.length === 0 && IsNullOrUndefined(patient.Age)) {
        return '';
    }

    if (IsNullOrUndefined(patient.Age)) {

        let result = '';

        if (!isAssistant) {

            result = isMaleHebrew ? boy : girl;
        } else {

            result = isMaleEnglish ? male : female;
        }

        return result;
    }

    if (patient.IsAnonymous !== 'True') {
        const isGenderEmpty = gender === '';
        age = '<span>'

        if (!isAssistant) {

            age += isMaleHebrew ? boy : gender === femaleInHebrew ? girl : '';
        }
        else {

            age += isMaleEnglish ? 'his age ' : gender === female ? 'her age ' : '';
        }

        age += patient.Age;
        age += '</span>';
    }


    return age;
}

const buildHtmlSelectTimeInterval = () => {

    const container = document.querySelector('#future-table-time-interval-container');
    const intervals = ["טווח זמן", "שבוע", "שבועיים", "חודש"];
    const selectList = document.createElement("select");
    selectList.id = 'future-table-time-interval-select';
    $(selectList).addClass('select-box');

    for (var i = 0; i < intervals.length; i++) {
        const option = document.createElement("option");
        option.value = intervals[i];
        option.text = intervals[i];
        selectList.appendChild(option);
    }
    selectList.onchange = loadFutureTable;
    container.appendChild(selectList);
    selectList.value = _timeinterval_G;

}

const loadFutureTable = () => {

    const selectList = document.querySelector('#future-table-time-interval-select');
    let timeInterval = selectList.value;
    _timeinterval_G = timeInterval;
    switch (timeInterval) {
        case 'שבוע':
            timeInterval = 7;
            break;
        case 'שבועיים':
            timeInterval = 14;
            break;
        case 'חודש':
            timeInterval = 30;
            break;
        case 'טווח זמן':
            timeInterval = 7;
            break;
        default:
            timeInterval = 7;
            break
    }
    localStorage.setItem('timeInterval', _timeinterval_G);

    refreshTable_UnityRide(timeInterval);
}




const buildDriverHTML = (ridePatNum, driver, rideNum) => {
    let driverWrapper = '';
    let isDriverReplacement;
    if (typeof driver !== 'undefined' && typeof rideNum !== 'undefined') {

        isDriverReplacement = true;
        let driverInnerContent = '';
        const { Id, DisplayName, EnglishFN, EnglishLN, CellPhone, NoOfDocumentedRides, IsNewDriver } = driver;

        if (!isAssistant) {                         //hebrew

            driverInnerContent = DisplayName;
        } else {                                    //english

            driverInnerContent = EnglishFN.length + EnglishLN.length > 0 ?
                `${EnglishFN} ${EnglishLN}`
                :
                DisplayName
        }

        driverWrapper = IsNewDriver ? `<span class='purpleFont boldFont'>${newVolunteerText}</span>` : ``;
        driverWrapper += `<a href="volunteerform.html" data-obj="volunteer" class="showMe driverName clickable blueFont boldFont"`;
        driverWrapper += `id='${Id}'>${driverInnerContent}</a>`;
        driverWrapper += `<span class="driverCellPhone">${addSeperator2MobileNum(CellPhone, '-')}</span>`;
        driverWrapper += `<button onclick="prepreparationAssignDriverModal(this, ${ridePatNum}, ${isDriverReplacement}, ${rideNum}, ${Id})" data-toggle="modal" data-target="#assignDriverModal" class='btn add-driver'>עדכון נהג</button>`;
        driverWrapper += '</div>';
    } else {

        isDriverReplacement = false;
        driverWrapper = `<div class='elementsInSameLine'>`;
        //let nodriverSpan = '<span class="verticalAllign">';
        //nodriverSpan += isAssistant ? noDriver.english : noDriver.hebrew;
        //nodriverSpan += '</span>';
        //driverWrapper += nodriverSpan;
        driverWrapper += `<buttond onclick="prepreparationAssignDriverModal(this, ${ridePatNum}, ${isDriverReplacement})"  data-toggle="modal" data-target="#assignDriverModal" class='btn add-driver'>שיבוץ נהג</button></div>`;
    }

    return driverWrapper;
}

//this is for check after gilad changes to united tabels with new api 3/12
const buildPatientHTML_AfterUnited = (ridepat) => {
    let patientPhone2render = "";
    let numOfEscorts = ridepat.AmountOfEscorts;
    let numOfPassengers = '<span class="numOfPassengers">נוסע יחיד</span>';
    let numOfPassengers_English =
        '<span class="numOfPassengers">single passenger</span>';
    let escortsPhone2render = "";
    let equipmentsAndnumOfPassengers = "";
    if (ridepat.PatientCellPhone != "0500000000") {
        if (validateMobileNumFullVersion(ridepat.PatientCellPhone)) {
            patientPhone2render = `<span class="patientPhone2render">${addSeperator2MobileNum(
                ridepat.PatientCellPhone,
                "-"
            )}</span>`
        }
        if (ridepat.PatientCellPhone != ridepat.PatientCellPhone2 && ridepat.PatientCellPhone2!=null) {
            patientPhone2render += ridepat.PatientCellPhone2 != "0" ? `<p class="phones-patient-p">${ridepat.PatientCellPhone2.replace(/(\d{3})(\d+)/, "$1-$2")}</p>` : '';
        }
        if (ridepat.PatientCellPhone2 != ridepat.PatientCellPhone3 && ridepat.PatientCellPhone != ridepat.PatientCellPhone3 && ridepat.PatientCellPhone3!=null) {
            patientPhone2render += ridepat.PatientCellPhone3 != null ? `<p class="phones-patient-p">${ridepat.PatientCellPhone3.replace(/(\d{3})(\d+)/, "$1-$2")}</p>` : '';

        }
    }



    if (numOfEscorts > 0) {

        numOfPassengers = `<span class="numOfPassengers">${numOfEscorts + (ridepat.OnlyEscort ? 0 : 1)//+1 is the patient himself
            } נוסעים</span>`;
        numOfPassengers_English = `<span class="numOfPassengers">${numOfEscorts + (ridepat.OnlyEscort ? 0 : 1)
            } passengers</span>`;

        numOfRealEscorts = 0;
        if (numOfRealEscorts === 0) escortsPhone2render = "";
    } else {
        numOfPassengers = `<span class="numOfPassengers">נוסע יחיד</span>`;
        numOfPassengers_English = `<span class="numOfPassengers">single pasnger</span>`;
    }

    if (ridepat.PatientEquipments != null) {
        equipmentsAndnumOfPassengers = `<div class="equipmentsAndnumOfPassengers">`;
        equipmentsAndnumOfPassengers += isAssistant
            ? numOfPassengers_English
            : numOfPassengers;
        equipmentsAndnumOfPassengers += `<div>`;
        ridepat.PatientEquipments.map((eq) => {
            equipmentsAndnumOfPassengers += `<img class="equipments-img" src="../../Media/${eq}.png" />`;
        });
        equipmentsAndnumOfPassengers += `</div>`;
        equipmentsAndnumOfPassengers += `</div>`;
    } else {
        equipmentsAndnumOfPassengers = `<div class="equipmentsAndnumOfPassengers block">`;
        equipmentsAndnumOfPassengers += isAssistant
            ? numOfPassengers_English
            : numOfPassengers;
        equipmentsAndnumOfPassengers += `</div>`;
    }

    let patientAddons =
        equipmentsAndnumOfPassengers + patientPhone2render + escortsPhone2render;
    let patientName2render = "";
    let patient_English_Name2render = "";

    if (ridepat.PatientName.includes("אנונימי")) {
        patientName2render = "<span class='boldFont'>חולה</span>";
        patient_English_Name2render = "<span class='boldFont'>patient</span>";
    } else {
        patientName2render = `<a href="patientform.html" data-obj="patient" class='showMe blueFont clickable boldFont' id='${ridepat.PatientId}'>${ridepat.PatientName}</a>`;
        patient_English_Name2render = `<a href="patientform.html" data-obj="patient" class='showMe blueFont clickable boldFont' id='${ridepat.PatientId}'>${ridepat.PatientName}</a>`;
    }


    let result = null;
    if (isAssistant) {

        result = patient_English_Name2render;
    } else {

        result = patientName2render
    }

    result = result.concat(' ' + patientAddons);

    return result;
};

const buildPatientHTML = (ridepat) => {

    let patientPhone2render = "";
    let numOfEscorts = ridepat.Escorts.length;
    let numOfPassengers = '<span class="numOfPassengers">נוסע יחיד</span>';
    let numOfPassengers_English =
        '<span class="numOfPassengers">single passenger</span>';
    let escortsPhone2render = "";
    let equipmentsAndnumOfPassengers = "";

    if (validateMobileNumFullVersion(ridepat.Pat.CellPhone)) {
        patientPhone2render = `<span class="patientPhone2render">${addSeperator2MobileNum(
            ridepat.Pat.CellPhone,
            "-"
        )}</span>`;
    }

    if (numOfEscorts > 0) {

        numOfPassengers = `<span class="numOfPassengers">${numOfEscorts + (ridepat.OnlyEscort ? 0 : 1)//+1 is the patient himself
            } נוסעים</span>`;
        numOfPassengers_English = `<span class="numOfPassengers">${numOfEscorts + (ridepat.OnlyEscort ? 0 : 1)
            } passengers</span>`;



        // Gilad comment out it --> avraham doesnt want the phone Number of Escorts its no needed !!
        //escortsPhone2render = '<span class="escortsPhone2render block">מלווים: ';
        numOfRealEscorts = 0;

        //ridepat.Escorts.map((es, i) => {
        //    if (validateMobileNumFullVersion(es.CellPhone)) {
        //        numOfRealEscorts++;
        //        escortsPhone2render += `${addSeperator2MobileNum(
        //            es.CellPhone,
        //            "-"
        //        )}, `;

        //        if (i === numOfEscorts - 1) {
        //            escortsPhone2render = escortsPhone2render.substring(
        //                0,
        //                escortsPhone2render.length - 2
        //            );
        //            escortsPhone2render += "</span>";
        //        }

        //    }
        //});
        if (numOfRealEscorts === 0) escortsPhone2render = "";
    } else {
        numOfPassengers = `<span class="numOfPassengers">נוסע יחיד</span>`;
        numOfPassengers_English = `<span class="numOfPassengers">single pasnger</span>`;
    }

    if (ridepat.Pat.Equipment.length > 0) {
        equipmentsAndnumOfPassengers = `<div class="equipmentsAndnumOfPassengers">`;
        equipmentsAndnumOfPassengers += isAssistant
            ? numOfPassengers_English
            : numOfPassengers;
        equipmentsAndnumOfPassengers += `<div>`;
        ridepat.Pat.Equipment.map((eq) => {
            equipmentsAndnumOfPassengers += `<img class="equipments-img" src="../../Media/${eq}.png" />`;
        });
        equipmentsAndnumOfPassengers += `</div>`;
        equipmentsAndnumOfPassengers += `</div>`;
    } else {
        equipmentsAndnumOfPassengers = `<div class="equipmentsAndnumOfPassengers block">`;
        equipmentsAndnumOfPassengers += isAssistant
            ? numOfPassengers_English
            : numOfPassengers;
        equipmentsAndnumOfPassengers += `</div>`;
    }

    let patientAddons =
        equipmentsAndnumOfPassengers + patientPhone2render + escortsPhone2render;
    let patientName2render = "";
    let patient_English_Name2render = "";

    if (ridepat.Pat.DisplayName.includes("אנונימי")) {
        patientName2render = "<span class='boldFont'>חולה</span>";
        patient_English_Name2render = "<span class='boldFont'>patient</span>";
    } else {
        patientName2render = `<a href="patientform.html" data-obj="patient" class='showMe blueFont clickable boldFont' id='${ridepat.Pat.Id}'>${ridepat.Pat.DisplayName}</a>`;
        patient_English_Name2render = `<a href="patientform.html" data-obj="patient" class='showMe blueFont clickable boldFont' id='${ridepat.Pat.Id}'>${ridepat.Pat.EnglishName}</a>`;
    }


    let result = null;
    if (isAssistant) {

        result = patient_English_Name2render;
    } else {

        result = patientName2render
    }

    result = result.concat(' ' + patientAddons);

    return result;
};

const buildStatusString = (statusObj, ridePatNum) => {


    let statusWrapper = `<div class="elementsInSameLine">`;
    const status = GetPatientStatus(statusObj.Status, isAssistant);
    const editTimeStamp = convertDBDate2FrontEndDate(statusObj.EditTimeStamp);
    const editBtn = `<button type='button' data-toggle="modal" data-target="#editPatientStatusModal" onclick="prepreparationEditPatientStatusModal(this, ${ridePatNum})" class='btn btn-icon waves-effect waves-light btn-secondary' title='עריכה'><i class='ti-pencil'></i></button>`;

    statusHTML = `<span>${status}</span>`;

    statusWrapper += '<div class="column">';
    statusWrapper += statusHTML;
    if (statusObj.Status === patientStatus.Finished) {

        statusWrapper += editTimeStamp ? '<div class="timeUnderStatus">' + editTimeStamp.toLocaleTimeString('he-IL', { timeStyle: 'short' }) + '</div>' : '';
    } else {

    }

    statusWrapper += `</div>`;

    statusWrapper += editBtn;

    statusWrapper += `</div>`;

    return statusWrapper;
}

const buildRemarkHTML = (remark, ridePatNum) => {

    let wrapper = '<div class="ToRight elementsInSameLine">';

    const editBtn = `<button type='button' data-toggle="modal" onclick="prepreparationEditRemark(this, ${ridePatNum})" data-target="#editRemarkModal" class='fix-flex-scretch btn btn-icon waves-effect waves-light btn-secondary' title='עריכה'><i class='ti-pencil'></i></button>`;

    wrapper += editBtn;
    wrapper += `<div class="m-l-10">${remark}</div>`;

    wrapper += '</div>';

    return wrapper;
}

const buildAcionsButtonsHTML = (ridePatNum, drivers) => {

    const editBtn = `<button type='button' class='edit                   btn btn-icon waves-effect waves-light btn-secondary' id='${ridePatNum}'  name='${ridePatNum}' title='עריכה'>         <i class='ti-pencil'></i></button>`;
    const deleteBtn = `<button type='button' class='remove               btn btn-icon waves-effect waves-light btn-secondary' id='${ridePatNum}'  name='${ridePatNum}' title='מחיקה'>         <i class='fa fa-remove'></i></button>`;
    const messageToCopyBtn = `<button type='button' class='copyMessage   btn btn-icon waves-effect waves-light btn-secondary' id='${ridePatNum}'  name='${ridePatNum}' title='העתקת הודעה'>   <i class='fa fa-envelope'></i></button>`;
    const candidateBtn = `<button type='button' title="מועמדים להסעה"   class='candidatesBtn btn btn-icon waves-effect waves-light btn-secondary'     id='${ridePatNum}'  name='${ridePatNum}' title='מועמדים להסעה'> <i class='fa fa-car'></i></button>`;
    let thisRide = findRideByNumber(ridePatNum);
    let timeNum = thisRide.PickupTime.replace('/', '').replace('/', '').replace('Date(', '').replace(')', '');
    timeNum = parseInt(timeNum);
    let time = new Date(timeNum);
    let buttonsWrapper = `<div class='elementsInSameLine'>${editBtn} ${deleteBtn} ${(drivers.length > 0) ? messageToCopyBtn : candidateBtn}</div>`;
    //let buttonsWrapper = `<div class='elementsInSameLine'>${editBtn} ${deleteBtn} ${(drivers.length > 0 && time.getHours() <= 23) ? messageToCopyBtn : candidateBtn}</div>`;

    return buttonsWrapper;
}

const buildTimeHTML = (thisRidePatDate, ridePatNum) => {

    let timeWrapper = `<div class="elementsInSameLine">`;

    let time = '<span class="time">';
    time += thisRidePatDate.getMinutes() === 14 ? 'אחה"צ' : thisRidePatDate.toLocaleTimeString('he-IL', { timeStyle: 'short' });
    time += '</span>';
    timeWrapper += time;

    const editBtn = `<button type='button' data-toggle="modal" data-target="#editTimeModal" onclick="prepreparationEditTimeModal(this, ${ridePatNum})" class='btn btn-icon waves-effect waves-light btn-secondary' title='עריכה'><i class='ti-pencil'></i></button>`;
    timeWrapper += editBtn;
    timeWrapper += `</div>`;

    return timeWrapper;
}

function prepreparationAssignDriverModal(thisBtn, ridePatNum, isDriverReplacement, rideNum, driverId) {
    _ridepatToManipulate = ridePatNum;
    _assignDriverToRideNum = rideNum
    _isDriverReplacement = isDriverReplacement;
    _driverId = driverId
    highLightRow(thisBtn);

    if (typeof driverId === 'undefined' || !driverId || driverId === null) {
        $('#removeDriverBtn').prop("disabled", true);
    } else {
        $('#removeDriverBtn').prop("disabled", false);
    }

    if (isDriverReplacement) {
        $('#driverModalTitle').text('עדכון נהג');
    } else {
        $('#driverModalTitle').text('שיבוץ נהג');
    }
}

//gilad change here the date to PickupTime
function prepreparationEditTimeModal(thisBtn, ridepatNum) {
    _ridepatToManipulate = ridepatNum
    highLightRow(thisBtn);

    $('#editTimeModalTitle').text('עדכון שעת הסעה');
    BuildTimeDDLs('hours', 'minutes');

    const ridepatObject = arr_rides.find(r => r.RidePatNum === ridepatNum);
    const ridePatTime = convertDBDate2FrontEndDate(ridepatObject.PickupTime);

    let hoursAsDoubleDigits = createDoubleDigit(ridePatTime.getHours());
    let minutesAsDoubleDigits = createDoubleDigit(ridePatTime.getMinutes());
    let isAfterNoon = parseInt(minutesAsDoubleDigits) === afternoonIndicator;
    let $hoursSelect = $('#hours');
    let $minutesSelect = $('#minutes');

    if (!isAfterNoon) {

        $hoursSelect.val(hoursAsDoubleDigits);
        $minutesSelect.val(minutesAsDoubleDigits);
        checkeditTimeAfterNoonCB(false);
    } else {

        retrieveDDLToInitial('#hours');
        retrieveDDLToInitial('#minutes');
        checkeditTimeAfterNoonCB(true);
    }
}

const BuildTimeDDLs = (hoursId, minutesId) => {


    const minutesArr = [0, 15, 30, 45];
    let $hoursSelect = $(`#${hoursId}`);
    let $minutesSelect = $(`#${minutesId}`);
    let doubleDigitsValue = '';


    // Clear existing options
    $hoursSelect.empty();
    $minutesSelect.empty();
    $minutesSelect.html('<option disabled="" selected="">דקות</option>');
    $hoursSelect.html('<option disabled="" selected="">שעות</option>')
    for (let i = 0; i < 46; i++) {

        if (i < 24) {

            doubleDigitsValue = createDoubleDigit(i);
            $('<option>', { value: doubleDigitsValue, text: doubleDigitsValue }).appendTo($hoursSelect);
            if (minutesArr.includes(i)) {
                $('<option>', { value: doubleDigitsValue, text: doubleDigitsValue }).appendTo($minutesSelect);
            }
        } else if (minutesArr.includes(i)) {
            doubleDigitsValue = createDoubleDigit(i);
            $('<option>', { value: doubleDigitsValue, text: doubleDigitsValue }).appendTo($minutesSelect);
        }
    }
    $hoursSelect.val('');
    $minutesSelect.val('');



}

function prepreparationEditPatientStatusModal(thisBtn, ridepatNum, EditTimeStamp, status) {
    _ridepatToManipulate = ridepatNum
    highLightRowSpecifcCSSClass(thisBtn, 'highlightRow');

    $('#editPatientStatusModalTitle').text('עדכון סטטוס חולה');
    BuildTimeDDLs("editPatientStatusModalHours", "editPatientStatusModalMinutes");
    let $hoursSelect = $('#editPatientStatusModalHours');
    let $minutesSelect = $('#editPatientStatusModalMinutes');

    let ridepatObject = arr_rides.find(r => r.RidePatNum === ridepatNum);
    ridepatObject = CustomRideObject(ridepatObject);
    const ridePatPatientStatus = ridepatObject.Pat.RidePatPatientStatus.Status;
    let editTimeStamp = ridepatObject.Pat.RidePatPatientStatus.EditTimeStamp;
    if (ridePatPatientStatus != -1) {

        editTimeStamp = convertDBDate2FrontEndDate(editTimeStamp);
        let hoursAsDoubleDigits = createDoubleDigit(editTimeStamp.getHours());
        let minutesAsDoubleDigits = createDoubleDigit(editTimeStamp.getMinutes());
        $hoursSelect.val(hoursAsDoubleDigits);
        $minutesSelect.val(minutesAsDoubleDigits);
        if (ridePatPatientStatus == -1) {
            $hoursSelect.val('');
            $minutesSelect.val('');
        }
    } else {

        retrieveDDLToInitial('#editPatientStatusModalMinutes');
        retrieveDDLToInitial('#editPatientStatusModalHours');
        checkeditTimeNotFinishedCB(true);

    }
}

function prepreparationEditRemark(thisBtn, ridepatNum) {
    _ridepatToManipulate = ridepatNum
    highLightRow(thisBtn);

    const ridepatObject = arr_rides.find(r => r.RidePatNum === ridepatNum);
    const remarkToRender = ridepatObject.Remark;
    $('#editRemarkTextEditor').val(remarkToRender);
}

const editRemarkModalHasShown = () => {

    let inputField = document.getElementById('editRemarkTextEditor');
    let textLength = inputField.value.length;
    let endOfRemark = 0;

    if (textLength > 0) {

        inputField.value = inputField.value.trim();
        inputField.value = inputField.value += '\n';
        textLength = inputField.value.length;
        endOfRemark = textLength;
        inputField.value = inputField.value.replace(/<br>/g, "\n");
    }

    inputField.selectionStart = endOfRemark;
    inputField.selectionEnd = endOfRemark;

    focusOnElement('editRemarkTextEditor');
}

const checkeditTimeNotFinishedCB = (NotFinishedIsChecked) => {

    $('#editPatientStatusNotFinishedCB').prop('checked', NotFinishedIsChecked);
    $('#editPatientStatusModalHours').prop('disabled', NotFinishedIsChecked);
    $('#editPatientStatusModalMinutes').prop('disabled', NotFinishedIsChecked);
}

const checkeditTimeAfterNoonCB = (afterNoonIsChecked) => {

    $('#editTimeAfterNoonCB').prop('checked', afterNoonIsChecked);
    $('#hours').prop('disabled', afterNoonIsChecked);
    $('#minutes').prop('disabled', afterNoonIsChecked);
}

const retrieveDDLToInitial = (id) => {
    $(id).get(0).selectedIndex = 0;
}

const createDoubleDigit = (value) => {

    let result = value < 10 ? '0' + value : value;

    return result;
}

function highLightRow(thisBtn) {
    _trToHighLight = thisBtn.closest('tr');
    _trToHighLight.classList.add('highlightRow');
}

function highLightRowSpecifcCSSClass(thisBtn, className) {
    _trToHighLight = thisBtn.closest('tr');
    _trToHighLight.classList.add(className);
}

const add2IhaveSeenAllready = () => {

    $('.ContextMenu_dropdown').addClass('hidden');//hide floating button
    $($(`#${candidate4IhaveSeen}`).children('.last-modified_td')[0]).removeClass('within2HoursSinceChange');//remove marker

    increaseCounter($($(`#${candidate4IhaveSeen}`)).closest('table')[0].id, candidate4IhaveSeen, true);

    let arrayOf_IHaveSeenAllready = JSON.parse(localStorage.getItem('arrayOf_IHaveSeenAllready'));

    if (arrayOf_IHaveSeenAllready) {

        arrayOf_IHaveSeenAllready.push(parseInt(candidate4IhaveSeen));
        localStorage.setItem('arrayOf_IHaveSeenAllready', JSON.stringify(arrayOf_IHaveSeenAllready));
    } else {

        let newArr = [];
        newArr.push(parseInt(candidate4IhaveSeen))
        localStorage.setItem('arrayOf_IHaveSeenAllready', JSON.stringify(newArr));
    }

}

const increaseCounter = (tableName, ridePatNum, isDecrease = false) => {

    if (!allReadyCountedRidePats.includes(ridePatNum)) {

        let counterValue_asTxt = $(`#${tableName}_lastModified_counter`).text();
        let previosValue = parseInt(counterValue_asTxt);
        isDecrease ? previosValue-- : previosValue++;
        $(`#${tableName}_lastModified_counter`).html(previosValue)

        allReadyCountedRidePats.push(ridePatNum);
    }
}

const sortTable_LastModifiedFirst = (tableName) => {
    switch (tableName) {
        case tableNames.MORNING:
            tMorning.order([6, 'desc']).draw();
            break;
        case tableNames.AFTERNOON:
            tAfternoon.order([6, 'desc']).draw();
            break;
        case tableNames.TOMORROW_MORNING:
            tTomorrowMorning.order([7, 'desc']).draw();
            break;
        case tableNames.FUTURE:
            tFuture.order([7, 'desc']).draw();
            break;
    }
}

const editTimeModal_bsModalHasShown = () => {

}

const bsModalHasShown = async () => {
    await activateAutoComplteToModalsInput();

    const thisRideDriver = drivers.find(d => d.Id === _driverId);
    if (typeof thisRideDriver === 'undefined' || !thisRideDriver) {
        $('#driver').val('');
    } else {
        $('#driver').val(thisRideDriver.DisplayName);
    }

    focusOnElement('driver');
}

const activateAutoComplteToModalsInput = async () => {

    if (typeof driverNames === 'undefined' || !driverNames) {
        driverNames = await getDrivers();
    }

    $('#driver').autocomplete({ source: driverNames });
}

const focusOnElement = (id) => {
    $(`#${id}`).focus();
}
const transformData = (hebrewData, englishMappings) => {
    return hebrewData.map(item => {
        const ridePatNum = item.RidePatNum.toString();
        if (englishMappings[ridePatNum]) {
            return {
                ...item,
                PatientName: englishMappings[ridePatNum].patientEnglishName,
                Origin: englishMappings[ridePatNum].OriginEnglishName,
                Destination: englishMappings[ridePatNum].DestEnglishName
            };
        }
        return item;
    });
};

const deleteArrayOfRides = (arrayOfRides) => {
    checkCookie();
    $('#wait').show();
    let nowInUTC = new Date().toUTCString();
    const listIds = arrayOfRides
    const UserName = GENERAL.USER.getUserDisplayName();
    let request = { ListIDs: listIds, userName: UserName };

    let tablesDataArray = [];
    for (var i = 0; i < arrayOfRides.length; i++) {
        tablesDataArray.push({
            id: arrayOfRides[i],
            row: $(`#${arrayOfRides[i]}`),
            table: $(`#${arrayOfRides[i]}`).parents('table').DataTable(),
        })
    }
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/deleteUnityRide",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify(request),
        success: function (data) {
            const detectedReturnRides = data.d.filter((ur) => { return ur.RidePatNum != 0 });
            swal({
                title: `لقد تم حذف السفرية\n נמחקו ${listIds.length} הסעות `,
                type: "success",
                showConfirmButton: true
            });

            for (var i = 0; i < tablesDataArray; i++) {
                const deletedRideObject = tablesDataArray[i];
                if (deletedRideObject.table && deletedRideObject.row) {
                    let d = deletedRideObject.table.row(deletedRideObject.row).remove().draw();


                }
                else {
                    //location.reload();
                }
            }
            //reset the checkboxes
            checkboxesRides = [];
            $('.checkboxClass').prop('checked', false);
            $('#NumofRideChecksText').text(0);
            $('.toggleTitle').hide();
            $('#deleteFewRidesButton').prop('disabled', true);
            $('#wait').hide();
            if (detectedReturnRides.length > 0) {
                let textToDisplay = `האם תרצו למחוק גם את הסעות החזרה של ההסעות ?`;
                textToDisplay += '\n';
                textToDisplay += `להלן הפירוט: \n\n`;
                let ReturnlistIdsToRemove = [];
                detectedReturnRides.forEach((ur) => {
                    textToDisplay += `הסעה של חולה : ${ur.PatientName} \n`;
                    textToDisplay += `בתאריך: ${convertToShortDateString(convertDBDate2FrontEndDate(ur.PickupTime))} \n`;
                    textToDisplay += `מוצא: ${ur.Origin} \n`;
                    textToDisplay += `יעד: ${ur.Destination} \n\n`;
                    ReturnlistIdsToRemove.push(ur.RidePatNum);
                })
                swal({
                    title: 'המערכת זיהתה שיש הסעות החזרה להסעות שמחקת הרגע',
                    type: 'info',
                    text: (textToDisplay || '').trim(),
                    showCancelButton: true,
                    confirmButtonText: "حذف\nמחיקה",
                    cancelButtonText: "الغاء\nבטל",
                    closeOnConfirm: false,
                    showLoaderOnConfirm: true,
                    confirmButtonColor: '#ff6d5d'
                }, function (isConfirm) {
                    if (!isConfirm) return;
                    const returnRequest = { ListIDs: ReturnlistIdsToRemove, userName: UserName }
                    $.ajax({
                        dataType: "json",
                        url: "WebService.asmx/deleteUnityRide",
                        contentType: "application/json; charset=utf-8",
                        type: "POST",
                        data: JSON.stringify(returnRequest),
                        success: function (data) {
                            swal({
                                title: 'لقد تم حذف السفرية',
                                text: 'נמחקו ' + listIds.length + ' הסעות',
                                type: 'success',
                                timer: 1200,
                                showConfirmButton: false
                            });
                            let tablesDataArray = [];
                            for (var i = 0; i < ReturnlistIdsToRemove.length; i++) {
                                tablesDataArray.push({
                                    id: ReturnlistIdsToRemove[i],
                                    row: $(`#${ReturnlistIdsToRemove[i]}`),
                                    table: $(`#${ReturnlistIdsToRemove[i]}`).parents('table').DataTable(),
                                })
                            }
                            for (var i = 0; i < (tablesDataArray ? tablesDataArray.length : 0); i++) {
                                var deletedRideObject = tablesDataArray[i];
                                if (deletedRideObject && deletedRideObject.table && deletedRideObject.row != null) {
                                    deletedRideObject.table.row(deletedRideObject.row).remove().draw();
                                } else {
                                    // שים לב: reload יסגור מיד את המודאל. השאר רק אם באמת חייבים.
                                    // location.reload();
                                }
                            }
                            checkboxesRides = [];
                            $('.checkboxClass').prop('checked', false);
                            $('#NumofRideChecksText').text(0);
                            $('.toggleTitle').hide();
                            $('#deleteFewRidesButton').prop('disabled', true);
                            $('#wait').hide();
                        },
                        error: function (err) {
                            $('#wait').hide();
                            console.error(err);
                            swal({
                                title: 'שגיאה במחיקה',
                                text: (err && err.responseText) ? err.responseText : 'שגיאה לא ידועה',
                                type: 'error'
                            });
                        }
                    });
                });

            }



        },
        error: function (err) {
            alert("Error in deleteArrayOfRides -> deleteUnityRide: " + err.responseText);
            $('#wait').hide();
            console.log(err);
        }
    })
}


const messageForPalCoor = () => {
    const choosenRides = arr_rides.filter(ride => checkboxesRides.includes(ride.RidePatNum));
    const dates = [];
    choosenRides.forEach(ride => {
        dates.push(convertToShortDateString(convertDBDate2FrontEndDate(ride.PickupTime)));
    })
    const uniqueDates = [...new Set(dates)];
    if (uniqueDates.length > 1) {
        swal({
            title: "לא ניתן לשלוח הודעה לרכז על הסעות בתאריכים שונים",
            timer: 2000,
            type: "error",
            showConfirmButton: false
        });
        return;
    }

    $('#wait').show();

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetEnglishNamesByIds",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ Ids: checkboxesRides }),

        success: function (data) {

            const dicById = JSON.parse(data.d);
            translatedData = transformData(choosenRides, dicById);

            const groupedObj = groupByOriginTimeAndDriver(translatedData);

            const lineBreak = '\n';
            let message = `Rides for ${convertToShortDateString(convertDBDate2FrontEndDate(choosenRides[0].PickupTime))} Morning.${lineBreak}`;
            let index = 0;

            const groups = Object.values(groupedObj).sort((a, b) => {
                if (a.timeKey !== b.timeKey) return a.timeKey.localeCompare(b.timeKey);
                return a.origin.localeCompare(b.origin);
            });

            for (const group of groups) {
                const rides = group.rides;

                const destList = Array.from(group.destinations);
                let destStr = '';
                if (destList.length === 1) {
                    destStr = destList[0];
                } else if (destList.length === 2) {
                    destStr = `${destList[0]} and ${destList[1]}`;
                } else {
                    destStr = `${destList.slice(0, -1).join(', ')} and ${destList.slice(-1)}`;
                }

                const timeLocal = convertDBDate2FrontEndDate(rides[0].PickupTime)
                    .toLocaleTimeString('he-IL', { timeStyle: 'short' });

                message += `${lineBreak}From ${group.origin} to ${destStr} at ${timeLocal} ${lineBreak}`;

                const ridesByDriver = {};
                rides.forEach(ride => {
                    const driverId = ride.MainDriver ?? 'NO_DRIVER';
                    if (!ridesByDriver[driverId]) ridesByDriver[driverId] = [];
                    ridesByDriver[driverId].push(ride);
                });

                for (const driverId in ridesByDriver) {
                    index++;
                    const driverRides = ridesByDriver[driverId];

                    if (driverRides.length === 1) {
                        const ride = driverRides[0];
                        if (ride.OnlyEscort) {
                            message += `    {blanks} of ${ride.PatientName} (${ride.AmountOfEscorts}) ${lineBreak}`;
                        } else {
                            message += `    ${ride.PatientName} (${(ride.AmountOfEscorts || 0) + 1}) ${lineBreak}`;
                        }
                    } else {
                        const joined = driverRides.map(ride => {
                            if (ride.OnlyEscort) {
                                return `{blanks} of ${ride.PatientName} (${ride.AmountOfEscorts || 0})`;
                            } else {
                                return `${ride.PatientName} (${(ride.AmountOfEscorts || 0) + 1})`;
                            }
                        }).join(' + ');
                        message += `    ${joined}${lineBreak}`;
                    }
                }
            }

            $('#wait').hide();

            navigator.clipboard.writeText(message)
                .then(function () {
                    swal({
                        title: "ההודעה מוכנה בפעולת הדבק",
                        timer: 2000,
                        type: "success",
                        showConfirmButton: false
                    });
                }, function (err) {
                    swal({
                        title: "העתקת ההודעה נכשלה",
                        timer: 2000,
                        type: "error",
                        showConfirmButton: false
                    });
                });

        },
        error: function (err) {
            console.log("Error in messageForPalCoor ->  " + err.responseText, err);
        }

    })









}


// this is the function that rememeber and collect the checked checkboxes for any functionality.
const ChangeCheckboxRides = (checkbox) => {
    let arrlocal = [];
    if (checkbox.checked && checkbox.id != 'AllChecks') {
        checkbox.parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important')
        //checkbox.parentNode.parentNode.parnetNode.parnetNode.style.setProperty('border-collapse', 'none', 'important')

    }
    else {
        checkbox.parentNode.parentNode.style.setProperty('border', 'none', 'important')
    }
    if (checkbox.id == 'AllChecks') {
        let tableName = checkbox.parentNode.parentNode.parentNode.parentNode.id;
        if (tableName == 'datatable-morning') {
            for (var i = 0; i < morningRidePats.length; i++) {
                $(`#check${morningRidePats[i].ridePatNum}`).prop("checked", checkbox.checked);
                checkbox.checked ? $(`#check${morningRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important') : $(`#check${morningRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', 'none', 'important')
            }

        }
        if (tableName == 'datatable-afternoon') {
            for (var i = 0; i < afterNoonRidePats.length; i++) {
                $(`#check${afterNoonRidePats[i].ridePatNum}`).prop("checked", checkbox.checked)
                checkbox.checked ? $(`#check${afterNoonRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important') : $(`#check${afterNoonRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', 'none', 'important')

            }

        }

        if (tableName == 'datatable-tomorrow-morning') {
            for (var i = 0; i < tomorrowMorningRidePats.length; i++) {
                $(`#check${tomorrowMorningRidePats[i].ridePatNum}`).prop("checked", checkbox.checked)
                checkbox.checked ? $(`#check${tomorrowMorningRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important') : $(`#check${tomorrowMorningRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', 'none', 'important')

            }

        }

        if (tableName == 'datatable-tomorrow-afternoon') {
            for (var i = 0; i < tomorrowAfternoonRidePats.length; i++) {
                $(`#check${tomorrowAfternoonRidePats[i].ridePatNum}`).prop("checked", checkbox.checked)
                checkbox.checked ? $(`#check${tomorrowAfternoonRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important') : $(`#check${tomorrowAfternoonRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', 'none', 'important')

            }

        }



        if (tableName == 'datatable-future') {
            var table = $('#datatable-future').DataTable();

            // מקבל את השורות בעמוד הנוכחי
            var currentPageNodes = table.rows({ page: 'current' }).nodes();

            $(currentPageNodes).each(function (index, node) {
                for (var i = 0; i < futureRidePats.length; i++) {
                    var checkboxId = `#check${futureRidePats[i].ridePatNum}`;
                    var rowCheckbox = $(node).find(checkboxId);

                    if (rowCheckbox.length) {
                        rowCheckbox.prop("checked", checkbox.checked);
                        if (checkbox.checked) {
                            rowCheckbox[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important');
                        } else {
                            rowCheckbox[0].parentNode.parentNode.style.setProperty('border', 'none', 'important');
                        }
                    }
                }
            });
            if (checkbox.checked) {
                swal({
                    title: "שימו לב",
                    text: 'כי הבחירה היא רק על ההסעות בעמוד הנוכחי (הסעות בעמודים האחרים לא נכללות בבחירה)',
                    timer: 2200,
                    type: "info",
                    showConfirmButton: false
                });
            }



        }




        // in this way all the checkboxes in all pages will be mark !!!! but i dont know if it is good idea
        //if (tableName == 'datatable-future') {
        //    var table = $('#datatable-future').DataTable();

        //    // עובר על כל השורות בטבלה, לא משנה באיזה עמוד הן נמצאות
        //    table.rows().every(function () {
        //        var rowNode = this.node();
        //        var rowData = this.data();

        //        // מחפש את ה-checkbox בשורה הנוכחית
        //        var rowCheckbox = $(rowNode).find(`#check${rowData.ridePatNum}`);

        //        if (rowCheckbox.length) {
        //            // מעדכן את מצב ה-checkbox
        //            rowCheckbox.prop("checked", checkbox.checked);

        //            // מעדכן את הבורדר
        //            if (checkbox.checked) {
        //                rowCheckbox[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important');
        //            } else {
        //                rowCheckbox[0].parentNode.parentNode.style.setProperty('border', 'none', 'important');
        //            }
        //        }
        //    });
        //}



        // the old way for future rides
        //if (tableName == 'datatable-future') {
        //    for (var i = 0; i < futureRidePats.length; i++) {
        //        $(`#check${futureRidePats[i].ridePatNum}`).prop("checked", checkbox.checked)
        //        checkbox.checked ? $(`#check${futureRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', '2px solid #7152d3', 'important') : $(`#check${futureRidePats[i].ridePatNum}`)[0].parentNode.parentNode.style.setProperty('border', 'none', 'important')

        //    }

        //}

    }


    $('.checkboxClass').each(function () {
        if (this.checked && this.id != 'AllChecks') {
            arrlocal.push(parseInt(this.id.replace("check", "")));
        }
    });
    checkboxesRides = arrlocal;
    if (checkboxesRides.length == 0) {
        $('.toggleTitle').hide();
        $('#deleteFewRidesButton').prop('disabled', true);
        $('#messageCoorBTN').prop('disabled', true);

    }
    else {
        $('.toggleTitle').show();
        $('#deleteFewRidesButton').prop('disabled', false);
        $('#messageCoorBTN').prop('disabled', false);

    }
    $('#NumofRideChecksText').text(checkboxesRides.length);

}


const DeleteMarkedRides = () => {
    if (checkboxesRides.length == 0) {
        swal('לא נבחרו הסעות', 'יש לבחור הסעות לפני המחיקה', 'warning');
    }
    else {
        let msg2user = `האם למחוק את ה${checkboxesRides.length} ההסעות שמסומנות
                    במידה והחולה אנונימי לא יהיה ניתן לשחזר את ההסעה לאחר המחיקה !`;
        swal({
            title: `هل انت متأك?\nהאם אתם בטוחים? \n${checkboxesRides.length} הסעות ימחקו`,
            type: "warning",
            text: msg2user,
            showCancelButton: true,
            cancelButtonText: "الغاء\nבטל",
            confirmButtonClass: 'btn-warning',
            confirmButtonText: "حذف\nמחיקה",
            closeOnConfirm: true
        }, function (userResponse) {
            if (userResponse) {
                deleteArrayOfRides(checkboxesRides);
            }

        });
    }
}

function groupByOriginAndTime(rides) {
    return rides.reduce((acc, ride) => {
        const m = String(ride.PickupTime).match(/\d+/);
        const ts = m ? new Date(parseInt(m[0], 10)) : new Date(ride.PickupTime);
        const timeKey = ts.toISOString().substr(11, 5); // HH:MM

        const key = `${ride.Origin}__${timeKey}`;
        if (!acc[key]) {
            acc[key] = {
                origin: ride.Origin,
                timeKey,         
                timeDate: ts,    
                destinations: new Set(),
                rides: []
            };
        }
        acc[key].destinations.add(ride.Destination);
        acc[key].rides.push(ride);
        return acc;
    }, {});
}
function groupByOriginTimeAndDriver(rides) {
    return rides.reduce((acc, ride) => {
        const m = String(ride.PickupTime).match(/\d+/);
        const ts = m ? new Date(parseInt(m[0], 10)) : new Date(ride.PickupTime);
        const timeKey = ts.toISOString().substr(11, 5); // HH:MM

        const driverKey = ride.MainDriver ?? 'NO_DRIVER';
        const key = `${ride.Origin}__${timeKey}__${driverKey}`;

        if (!acc[key]) {
            acc[key] = {
                origin: ride.Origin,
                timeKey,
                timeDate: ts,
                driverId: driverKey,
                destinations: new Set(),
                rides: []
            };
        }

        acc[key].destinations.add(ride.Destination);
        acc[key].rides.push(ride);

        return acc;
    }, {});
}

function convertToShortDateString(dateString) {
    // Create a Date object from the input string
    const date = new Date(dateString);

    // Extract the day, month, and year
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
    const year = date.getFullYear();

    // Format the date as dd/mm/yyyy
    return `${day}/${month}/${year}`;
}
