
//#region Variables for real-time updates

let tableNameRidePatShouldBeRender;
let tableName2manipulate;
let thisRidePatDate;
let thisRidePatLastModified;
let row2manipulate;
let tr_nodesAsArray = [];
let node2manipulate;
let nodechilds;
let lastModified_td;
let ridePatBeforeUpdate;
let _timeinterval_G = localStorage.getItem('timeInterval') != undefined ? localStorage.getItem('timeInterval') : 'טווח זמן'
const ADD_DRIVER = 'add driver', REMOVE_DRIVER = 'remove driver', RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED = 'ridepat_patient_status_edittime_changed';
const tableNames = {
    MORNING: 'datatable-morning',
    AFTERNOON: 'datatable-afternoon',
    TOMORROW_MORNING: 'datatable-tomorrow-morning',
    TOMORROW_AFTERNOON: 'datatable-tomorrow-afternoon',
    FUTURE: 'datatable-future',
    NOT_RELEVANT: 'not-relevant'
}
let currentRidePat_EditTimeStamp;
let updatedRidePat_EditTimeStamp;
let currentRidePat_status;
let updatedRidePat_status;
let isAfterNoonRide;
let treatmentFinished;
let driverIsAssigned;
let listRowstoColor = [];
hospitals = [];
//#endregion

//#region Functions to Reuse

const update_arr_rides = (updated_ridePat) => {
    const index = arr_rides.findIndex(r => r.RidePatNum === updated_ridePat.RidePatNum);
    if (index == -1) {
        console.error("in (update_arr_rides) \n not found ride with id : ", updated_ridePat.RidePatNum)
    }
    ridePatBeforeUpdate = arr_rides[index];
    arr_rides[index] = updated_ridePat;
}

//make the flat object to complex object like the old API
const CustomRideObject = (thisRide) => {

    let CustomObj = {};
    CustomObj.RidePatNum = thisRide.RidePatNum;
    CustomObj.Status = thisRide.Status;
    CustomObj.Remark = thisRide.Remark;
    CustomObj.OnlyEscort = thisRide.OnlyEscort;
    CustomObj.LastModified = thisRide.LastModified;
    CustomObj.Date = thisRide.PickupTime;

    CustomObj.Pat = {};
    CustomObj.Pat.DisplayName = thisRide.PatientName;
    CustomObj.Pat.Id = thisRide.PatientId;
    CustomObj.Pat.DisplayNameA = ''
    CustomObj.Pat.EnglishName = ''
    CustomObj.Pat.PatientIdentity = 0;
    CustomObj.Pat.Equipment = thisRide.PatientEquipments == null ? [] : thisRide.PatientEquipments;
    CustomObj.Pat.IsAnonymous = thisRide.IsAnonymous;
    CustomObj.Pat.RidePatPatientStatus = {};
    CustomObj.Pat.RidePatPatientStatus.Status = (thisRide.PatientStatus == "" || thisRide.PatientStatus == "Not Finished") ? -1 : 1;
    CustomObj.Pat.RidePatPatientStatus.EditTimeStamp = thisRide.PatientStatusEditTime

    CustomObj.Pat.EscortedList = [];
    CustomObj.Escorts = [];
    //for (var i = 0; i < thisRide[18].Value.length; i++) {
    //    let oneEscort = { }
    //    oneEscort.Id = thisRide[18].Value[i][0].Value;
    //    oneEscort.DisplayName = thisRide[18].Value[i][1].Value;
    //    CustomObj.Escorts.push(oneEscort);
    //}
    for (var i = 0; i < thisRide.AmountOfEscorts; i++) {
        let oneEscort = {}
        oneEscort.Id = 0
        oneEscort.DisplayName = ''
        CustomObj.Pat.EscortedList.push(oneEscort);
    }

    for (var i = 0; i < thisRide.AmountOfEscorts; i++) {
        let oneEscort = {}
        oneEscort.Id = 0
        oneEscort.DisplayName = ''
        CustomObj.Escorts.push(oneEscort);
    }

    CustomObj.Drivers = [];
    let Maindriver = {};
    Maindriver.DisplayName = thisRide.DriverName;
    Maindriver.Id = thisRide.MainDriver;
    Maindriver.CellPhone = thisRide.DriverCellPhone
    CustomObj.Drivers.push(Maindriver);
    if (Maindriver.Id == null || Maindriver.Id == -1) {
        CustomObj.Drivers = [];
    }

    CustomObj.Destination = {};
    CustomObj.Destination.Name = thisRide.Destination
    CustomObj.Origin = {};
    CustomObj.Origin.Name = thisRide.Origin;

    CustomObj.Pat.Barrier = {}
    CustomObj.Pat.Hospital = {}
    CustomObj.Pat.Age = thisRide.PatientAge;
    CustomObj.Pat.cellPhone = thisRide.PatientCellPhone;
    CustomObj.Pat.GenderAsEnum = thisRide.PatientGender;
    CustomObj.Pat.Barrier.Name = CustomObj.Destination.Name;
    CustomObj.Pat.Hospital.Name = CustomObj.Origin.Name;

    CustomObj.Coordinator = {};
    CustomObj.Coordinator.DisplayName = thisRide.CoorName
    CustomObj.Coordinator.Id = thisRide.CoorId

    return CustomObj;
}



const getTableName2Manipulate = (ridePatNum) => {
    let tableName2manipulate;
    if (typeof tMorning.row(`#${ridePatNum}`).data() !== 'undefined') {
        tableName2manipulate = tableNames.MORNING;
    } else if (typeof tAfternoon.row(`#${ridePatNum}`).data() !== 'undefined') {
        tableName2manipulate = tableNames.AFTERNOON;
    } else if (typeof tTomorrowMorning.row(`#${ridePatNum}`).data() !== 'undefined') {
        tableName2manipulate = tableNames.TOMORROW_MORNING;
    } else if (typeof tTomorrowAfternoon.row(`#${ridePatNum}`).data() !== 'undefined') {
        tableName2manipulate = tableNames.TOMORROW_AFTERNOON;
    } else if (typeof tFuture.row(`#${ridePatNum}`).data() !== 'undefined') {
        tableName2manipulate = tableNames.FUTURE;
    } else {
        tableName2manipulate = 'table did not found...';
    }
    return tableName2manipulate;
}

const getTableNameToBeRenderedIn = (ridepatDate) => {

    let tableName = '';


    if (ridepatDate.toLocaleDateString() === new Date().toLocaleDateString()) {//<---- today

        if (ridepatDate.getHours() < 12 && ridepatDate.getMinutes() != 14) {

            tableName = tableNames.MORNING;
        } else {

            tableName = tableNames.AFTERNOON;
        }

    } else {

        tableName = tableNames.NOT_RELEVANT;
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        if (ridepatDate.toLocaleDateString() === tomorrow.toLocaleDateString()) { //<---- tomorrow
            if (ridepatDate.getHours() < 12 && ridepatDate.getMinutes() != 14) {

                tableName = tableNames.TOMORROW_MORNING;
            } else {

                tableName = tableNames.TOMORROW_AFTERNOON;
            }
        }
    }
    return tableName;
}

const fixDate_WhichComeFromOpenConnection = (date2fix, getDate = true) => {

    const ticks = new Date(date2fix) - new Date().getTimezoneOffset() * 60000;

    if (getDate) {

        let fixedDate = new Date(ticks);
        return fixedDate;
    }

    return ticks;
}

const manipilateNodeStyle = (tr_node2manipulate, updated_ridePat, functionlity, isFutureTable = false, tableName = "") => {

    nodechilds = Array.from($(tr_node2manipulate).children());
    lastModified_td = nodechilds.find(x => x.className.includes('last-modified_td'));
    driver_td = nodechilds.find(x => x.className.includes('driver_td'));
    $(lastModified_td).addClass('within2HoursSinceChange');
    $(driver_td).removeClass('withIn12HoursDrive withIn24HoursDrive');

    switch (functionlity) {

        case ADD_DRIVER:

            //if (!isFutureTable && updated_ridePat.Pat.IsAnonymous === 'True') {
            //    $(tr_node2manipulate).addClass('driverAssign2AnonymousRide');
            //}

            break;

        case REMOVE_DRIVER:

            $(tr_node2manipulate).removeClass('driverAssign2AnonymousRide');

            let hh, mm;
            const rideDate = fixDate_WhichComeFromOpenConnection(updated_ridePat.Date);

            if (rideDate.getMinutes() === afternoonIndicator) {
                hh = 12, mm = 0;
            } else {
                hh = rideDate.getHours(), mm = rideDate.getMinutes();
            }

            const suitableClass = classManager.getSuitableClassS_4thisTime(tableName, hh, mm);
            $(driver_td).addClass(suitableClass);
            break;

        case RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED:
            $(tr_node2manipulate).addClass('highlightRow-after-patientStatusModal');

            break;

    }
}

const shiftRidePatToSuitableTable = (tableNameRidePatIsNowIn, tableNameRidePatShouldBeRender, updated_ridePat) => {

    removeRidePatFromOldTable(tableNameRidePatIsNowIn, updated_ridePat);

    rePaintRidePatToNewTable(tableNameRidePatShouldBeRender, updated_ridePat);
}

const removeRidePatFromOldTable = (tableNameRidePatIsNowIn, updated_ridePat) => {
    switch (tableNameRidePatIsNowIn) {
        case tableNames.MORNING:
            row2manipulate = tMorning.row(`#${updated_ridePat.RidePatNum}`).data();
            tMorning.row(`#${updated_ridePat.RidePatNum}`).remove().draw(false);
            break;
        case tableNames.AFTERNOON:
            row2manipulate = tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();
            tAfternoon.row(`#${updated_ridePat.RidePatNum}`).remove().draw(false);
            break;

        case tableNames.TOMORROW_MORNING:
            row2manipulate = tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data();
            tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).remove().draw(false);
            break;
        case tableNames.TOMORROW_AFTERNOON:
            row2manipulate = tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();
            tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).remove().draw(false);
            break;
    }
}

const rePaintRidePatToNewTable = (tableNameRidePatShouldBeRender, updated_ridePat) => {

    let ridePat2Draw = {};
    ridePat2Draw.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${updated_ridePat.RidePatNum}">`;
    ridePat2Draw.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
    ridePat2Draw.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
    ridePat2Draw.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
    ridePat2Draw.patient = buildPatientHTML(updated_ridePat);
    ridePat2Draw.patientAge = buildPatientAgeHTML_AfterUnited(updated_ridePat.Pat);
    ridePat2Draw.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RidePatNum);
    ridePat2Draw.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
    ridePat2Draw.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
    ridePat2Draw.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);
    ridePat2Draw.isAnonymous = updated_ridePat.Pat.IsAnonymous;
    ridePat2Draw.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

    switch (tableNameRidePatShouldBeRender) {

        case tableNames.MORNING:

            tMorning.row.add(
                {
                    checkBoxesStr: ridePat2Draw.checkBoxesStr,
                    ridePatNum: updated_ridePat.RidePatNum,
                    time: ridePat2Draw.time,
                    origin: ridePat2Draw.origin,
                    destination: ridePat2Draw.destination,
                    patient: ridePat2Draw.patient,
                    patientAge: ridePat2Draw.patientAge,
                    driver: ridePat2Draw.driver,
                    status: ridePat2Draw.status,
                    lastModified: ridePat2Draw.lastModified,
                    remark: ridePat2Draw.remark,
                    isAnonymous: ridePat2Draw.isAnonymous,
                    buttons: ridePat2Draw.buttons
                }
            ).draw(false);

            tr_nodesAsArray = Array.from(tMorning.rows().nodes());
            break;

        case tableNames.AFTERNOON:

            tAfternoon.row.add(
                {
                    checkBoxesStr: ridePat2Draw.checkBoxesStr,
                    ridePatNum: updated_ridePat.RidePatNum,
                    time: ridePat2Draw.time,
                    origin: ridePat2Draw.origin,
                    destination: ridePat2Draw.destination,
                    patient: ridePat2Draw.patient,
                    patientAge: ridePat2Draw.patientAge,
                    driver: ridePat2Draw.driver,
                    status: ridePat2Draw.status,
                    lastModified: ridePat2Draw.lastModified,
                    remark: ridePat2Draw.remark,
                    isAnonymous: ridePat2Draw.isAnonymous,
                    buttons: ridePat2Draw.buttons
                }
            ).draw(false);

            tr_nodesAsArray = Array.from(tAfternoon.rows().nodes());
            break;

        case tableNames.TOMORROW_MORNING:

            tTomorrowMorning.row.add(
                {
                    checkBoxesStr: ridePat2Draw.checkBoxesStr,
                    ridePatNum: updated_ridePat.RidePatNum,
                    time: ridePat2Draw.time,
                    date: thisRidePatDate.toLocaleDateString('he-IL'),
                    origin: ridePat2Draw.origin,
                    destination: ridePat2Draw.destination,
                    patient: ridePat2Draw.patient,
                    patientAge: ridePat2Draw.patientAge,
                    driver: ridePat2Draw.driver,
                    status: ridePat2Draw.status,
                    lastModified: ridePat2Draw.lastModified,
                    remark: ridePat2Draw.remark,
                    isAnonymous: ridePat2Draw.isAnonymous,
                    buttons: ridePat2Draw.buttons
                }
            ).draw(false);

            tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());
            break;

        case tableNames.TOMORROW_AFTERNOON:

            tTomorrowAfternoon.row.add(
                {
                    checkBoxesStr: ridePat2Draw.checkBoxesStr,
                    ridePatNum: updated_ridePat.RidePatNum,
                    time: ridePat2Draw.time,
                    date: thisRidePatDate.toLocaleDateString('he-IL'),
                    origin: ridePat2Draw.origin,
                    destination: ridePat2Draw.destination,
                    patient: ridePat2Draw.patient,
                    patientAge: ridePat2Draw.patientAge,
                    driver: ridePat2Draw.driver,
                    status: ridePat2Draw.status,
                    lastModified: ridePat2Draw.lastModified,
                    remark: ridePat2Draw.remark,
                    isAnonymous: ridePat2Draw.isAnonymous,
                    buttons: ridePat2Draw.buttons
                }
            ).draw(false);

            tr_nodesAsArray = Array.from(tTomorrowAfternoon.rows().nodes());
            break;
    }

    if (tr_nodesAsArray) {
        tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);

        updatedRidePat_status = updated_ridePat.Pat.RidePatPatientStatus.Status;
        treatmentFinished = updatedRidePat_status === patientStatus.Finished;
        driverIsAssigned = updated_ridePat.Drivers.length > 0;

        if (!driverIsAssigned && treatmentFinished) {
            manipilateNodeStyle(tr_node2manipulate, updated_ridePat, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
        }

    }
}

//#endregion

//#region Hubs Connections

const notification = $.connection.ridePatHub;
const notification2 = $.connection.unityRideHub;

$.connection.hub.start().done();

//#endregion

//#region Callbacks Function From Server to Clients

notification.client.ridePatUpdated = function (updated_ridePat) {

    /*   *********************
         * !!! IMPORTANT !!! *
         ********↓↓↓**********/
    //update the specific ride in arr_rides:
    update_arr_rides(updated_ridePat);
    /*  *********↑↑↑*********
        * !!! IMPORTANT !!! *
        *********************/
    tableName2manipulate = getTableName2Manipulate(updated_ridePat.RidePatNum);
    thisRidePatDate = fixDate_WhichComeFromOpenConnection(updated_ridePat.Date);
    thisRidePatLastModified = fixDate_WhichComeFromOpenConnection(updated_ridePat.LastModified);
    tableNameRidePatShouldBeRender = getTableNameToBeRenderedIn(thisRidePatDate);

    if (tableName2manipulate !== tableNameRidePatShouldBeRender && tableNameRidePatShouldBeRender !== tableNames.NOT_RELEVANT) {

        shiftRidePatToSuitableTable(tableName2manipulate, tableNameRidePatShouldBeRender, updated_ridePat);
    } else {

        switch (tableName2manipulate) {

            case tableNames.MORNING:

                row2manipulate = tMorning.row(`#${updated_ridePat.RidePatNum}`).data();

                row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
                row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML(updated_ridePat);
                row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
                row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);
                row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

                tMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.MORNING, updated_ridePat.RidePatNum);

                tr_nodesAsArray = Array.from(tMorning.rows().nodes());
                break;

            case tableNames.AFTERNOON:

                row2manipulate = tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

                row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
                row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML(updated_ridePat);
                row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
                row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
                row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

                tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.AFTERNOON, updated_ridePat.RidePatNum);

                tr_nodesAsArray = Array.from(tAfternoon.rows().nodes());
                break;

            case tableNames.TOMORROW_MORNING:

                row2manipulate = tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data();

                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
                row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML(updated_ridePat);
                row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
                row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
                row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

                tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.TOMORROW_MORNING, updated_ridePat.RidePatNum);

                tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());

                break;

            case tableNames.TOMORROW_AFTERNOON:

                row2manipulate = tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
                row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML(updated_ridePat);
                row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
                row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
                row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

                tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.TOMORROW_AFTERNOON, updated_ridePat.RidePatNum);

                tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());

                break;


            case tableNames.FUTURE:

                row2manipulate = tFuture.row(`#${updated_ridePat.RidePatNum}`).data();

                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
                row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML(updated_ridePat);
                row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
                row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
                row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

                tFuture.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.FUTURE, updated_ridePat.RidePatNum);

                tr_nodesAsArray = Array.from(tFuture.rows().nodes());
                tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);
                manipilateNodeStyle(tr_node2manipulate, updated_ridePat, ADD_DRIVER, true);

                break;

            default:
                console.log('system recognized a tempt of the server to post real-time data but failed to found the rellevant row to redraw');
                tr_nodesAsArray = null;
                break;
        }

        if (tr_nodesAsArray) {
            tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);

            updatedRidePat_status = updated_ridePat.Pat.RidePatPatientStatus.Status;
            treatmentFinished = updatedRidePat_status === patientStatus.Finished;
            isAfterNoonRide = thisRidePatDate.getMinutes() === afternoonIndicator;
            driverIsAssigned = updated_ridePat.Drivers.length > 0;

            if (treatmentFinished) {

                if (!driverIsAssigned) {

                    manipilateNodeStyle(tr_node2manipulate, updated_ridePat, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
                } else if (driverIsAssigned && isAfterNoonRide) {

                    manipilateNodeStyle(tr_node2manipulate, updated_ridePat, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
                } else {

                    $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
                }
            } else {

                $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
            }
        }
    }
}

//Gilad addition use signalR
notification2.client.UnityRideUpdated = function (updatedUnityRide) {
    /*   *********************
         * !!! IMPORTANT !!! *
         ********↓↓↓**********/
    //update the specific ride in arr_rides:
    update_arr_rides(updatedUnityRide);
    /*  *********↑↑↑*********
        * !!! IMPORTANT !!! *
        *********************/

    let customRide = CustomRideObject(updatedUnityRide); // to ajust the object

    tableName2manipulate = getTableName2Manipulate(customRide.RidePatNum);
    thisRidePatDate = fixDate_WhichComeFromOpenConnection(customRide.Date);
    thisRidePatLastModified = fixDate_WhichComeFromOpenConnection(customRide.LastModified);
    tableNameRidePatShouldBeRender = getTableNameToBeRenderedIn(thisRidePatDate);
    if (tableName2manipulate !== tableNameRidePatShouldBeRender && tableNameRidePatShouldBeRender !== tableNames.NOT_RELEVANT) {

        shiftRidePatToSuitableTable(tableName2manipulate, tableNameRidePatShouldBeRender, customRide);
    } else {

        switch (tableName2manipulate) {

            case tableNames.MORNING:

                row2manipulate = tMorning.row(`#${customRide.RidePatNum}`).data();
                row2manipulate.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${customRide.RidePatNum}">`;
                row2manipulate.time = buildTimeHTML(thisRidePatDate, customRide.RidePatNum);
                row2manipulate.origin = !isAssistant ? customRide.Origin.Name : customRide.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? customRide.Destination.Name : customRide.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML_AfterUnited(updatedUnityRide);
                row2manipulate.driver = buildDriverHTML(customRide.RidePatNum, customRide.Drivers[0], customRide.RidePatNum);
                row2manipulate.status = buildStatusString(customRide.Pat.RidePatPatientStatus, customRide.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(customRide.Remark, customRide.RidePatNum);
                row2manipulate.buttons = buildAcionsButtonsHTML(customRide.RidePatNum, customRide.Drivers);
                tMorning.row(`#${customRide.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.MORNING, customRide.RidePatNum);

                tr_nodesAsArray = Array.from(tMorning.rows().nodes());

                if (customRide.Status == 'נמחקה') {
                    tMorning.row(`#${customRide.RidePatNum}`).remove().draw()
                }
                break;

            case tableNames.AFTERNOON:

                row2manipulate = tAfternoon.row(`#${customRide.RidePatNum}`).data();
                row2manipulate.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${customRide.RidePatNum}">`;
                row2manipulate.time = buildTimeHTML(thisRidePatDate, customRide.RidePatNum);
                row2manipulate.origin = !isAssistant ? customRide.Origin.Name : customRide.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? customRide.Destination.Name : customRide.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML_AfterUnited(updatedUnityRide);
                row2manipulate.driver = buildDriverHTML(customRide.RidePatNum, customRide.Drivers[0], customRide.RidePatNum);
                row2manipulate.status = buildStatusString(customRide.Pat.RidePatPatientStatus, customRide.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(customRide.Remark, customRide.RidePatNum);
                row2manipulate.buttons = buildAcionsButtonsHTML(customRide.RidePatNum, customRide.Drivers);

                tAfternoon.row(`#${customRide.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.AFTERNOON, customRide.RidePatNum);

                tr_nodesAsArray = Array.from(tAfternoon.rows().nodes());
                if (customRide.Status == 'נמחקה') {
                    tAfternoon.row(`#${customRide.RidePatNum}`).remove().draw()
                }
                break;

            case tableNames.TOMORROW_MORNING:

                row2manipulate = tTomorrowMorning.row(`#${customRide.RidePatNum}`).data();
                row2manipulate.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${customRide.RidePatNum}">`;
                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, customRide.RidePatNum);
                row2manipulate.origin = !isAssistant ? customRide.Origin.Name : customRide.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? customRide.Destination.Name : customRide.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML_AfterUnited(updatedUnityRide);
                row2manipulate.driver = buildDriverHTML(customRide.RidePatNum, customRide.Drivers[0], customRide.RidePatNum);
                row2manipulate.status = buildStatusString(customRide.Pat.RidePatPatientStatus, customRide.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(customRide.Remark, customRide.RidePatNum);
                row2manipulate.buttons = buildAcionsButtonsHTML(customRide.RidePatNum, customRide.Drivers);

                tTomorrowMorning.row(`#${customRide.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.TOMORROW_MORNING, customRide.RidePatNum);

                tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());

                if (customRide.Status == 'נמחקה') {
                    tTomorrowMorning.row(`#${customRide.RidePatNum}`).remove().draw()
                }
                break;

            case tableNames.TOMORROW_AFTERNOON:

                row2manipulate = tTomorrowAfternoon.row(`#${customRide.RidePatNum}`).data();
                row2manipulate.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${customRide.RidePatNum}">`;
                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, customRide.RidePatNum);
                row2manipulate.origin = !isAssistant ? customRide.Origin.Name : customRide.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? customRide.Destination.Name : customRide.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML_AfterUnited(updatedUnityRide);
                row2manipulate.driver = buildDriverHTML(customRide.RidePatNum, customRide.Drivers[0], customRide.RidePatNum);
                row2manipulate.status = buildStatusString(customRide.Pat.RidePatPatientStatus, customRide.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(customRide.Remark, customRide.RidePatNum);
                row2manipulate.buttons = buildAcionsButtonsHTML(customRide.RidePatNum, customRide.Drivers);

                tTomorrowAfternoon.row(`#${customRide.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.TOMORROW_AFTERNOON, customRide.RidePatNum);

                tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());

                if (customRide.Status == 'נמחקה') {
                    tTomorrowAfternoon.row(`#${customRide.RidePatNum}`).remove().draw()
                }
                break;


            case tableNames.FUTURE:

                row2manipulate = tFuture.row(`#${customRide.RidePatNum}`).data();
                row2manipulate.checkBoxesStr = `<input type="checkbox" class="checkboxClass" onchange="ChangeCheckboxRides(this)" id="check${customRide.RidePatNum}">`;
                row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
                row2manipulate.time = buildTimeHTML(thisRidePatDate, customRide.RidePatNum);
                row2manipulate.origin = !isAssistant ? customRide.Origin.Name : customRide.Origin.EnglishName;
                row2manipulate.destination = !isAssistant ? customRide.Destination.Name : customRide.Destination.EnglishName;
                row2manipulate.patient = buildPatientHTML_AfterUnited(customRide);
                row2manipulate.driver = buildDriverHTML(customRide.RidePatNum, customRide.Drivers[0], customRide.RidePatNum);
                row2manipulate.status = buildStatusString(customRide.Pat.RidePatPatientStatus, customRide.RidePatNum);
                row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
                row2manipulate.remark = buildRemarkHTML(customRide.Remark, customRide.RidePatNum);;
                row2manipulate.buttons = buildAcionsButtonsHTML(customRide.RidePatNum, customRide.Drivers);

                tFuture.row(`#${customRide.RidePatNum}`).data(row2manipulate).draw('page');
                increaseCounter(tableNames.FUTURE, customRide.RidePatNum);

                tr_nodesAsArray = Array.from(tFuture.rows().nodes());
                tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${customRide.RidePatNum}`);
                manipilateNodeStyle(tr_node2manipulate, customRide, ADD_DRIVER, true);
                if (customRide.Status == 'נמחקה') {
                    tFuture.row(`#${customRide.RidePatNum}`).remove().draw()
                }
                if (customRide.RidePatNum < 0) {
                    tFuture.row(`#${customRide.RidePatNum * -1}`).remove().draw()
                }

                break;

            default:
                console.log('system recognized a tempt of the server to post real-time data but failed to found the rellevant row to redraw');
                tr_nodesAsArray = null;
                //location.reload();
                break;
        }

        if (tr_nodesAsArray) {
            tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${customRide.RidePatNum}`);

            updatedRidePat_status = customRide.Pat.RidePatPatientStatus.Status;
            treatmentFinished = updatedRidePat_status === patientStatus.Finished;
            isAfterNoonRide = thisRidePatDate.getMinutes() === afternoonIndicator;
            driverIsAssigned = customRide.Drivers.length > 0;

            if (treatmentFinished) {
                if (!driverIsAssigned) {

                    manipilateNodeStyle(tr_node2manipulate, customRide, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
                } else if (driverIsAssigned && isAfterNoonRide) {

                    manipilateNodeStyle(tr_node2manipulate, customRide, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
                } else {

                    $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
                }
            } else {
                manipilateNodeStyle(tr_node2manipulate, customRide, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
                $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
            }
        }
    }
}

notification.client.driverHasAssigned2RidePat = function (updated_ridePat) {

    /*   *********************
         * !!! IMPORTANT !!! *
         ********↓↓↓**********/
    //update the specific ride in arr_rides:
    update_arr_rides(updated_ridePat);
    /*  *********↑↑↑*********
        * !!! IMPORTANT !!! *
        *********************/

    tableName2manipulate = getTableName2Manipulate(updated_ridePat.RidePatNum);
    thisRidePatDate = fixDate_WhichComeFromOpenConnection(updated_ridePat.Date);
    thisRidePatLastModified = fixDate_WhichComeFromOpenConnection(updated_ridePat.LastModified);

    switch (tableName2manipulate) {

        case tableNames.MORNING:

            row2manipulate = tMorning.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
            row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
            row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
            row2manipulate.patient = buildPatientHTML(updated_ridePat);
            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            //row2manipulate.englishOriginAndDestination = updated_ridePat.Origin.EnglishName + " - " + updated_ridePat.Destination.EnglishName;
            row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.MORNING, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tMorning.rows().nodes());
            break;

        case tableNames.AFTERNOON:

            row2manipulate = tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
            row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
            row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
            row2manipulate.patient = buildPatientHTML(updated_ridePat);
            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            //row2manipulate.englishOriginAndDestination = updated_ridePat.Origin.EnglishName + " - " + updated_ridePat.Destination.EnglishName;
            row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.AFTERNOON, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tAfternoon.rows().nodes());
            break;

        case tableNames.TOMORROW_MORNING:

            row2manipulate = tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
            row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
            row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
            row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
            row2manipulate.patient = buildPatientHTML(updated_ridePat);
            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            //row2manipulate.englishOriginAndDestination = updated_ridePat.Origin.EnglishName + " - " + updated_ridePat.Destination.EnglishName;
            row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.TOMORROW_MORNING, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());
            break;

        case tableNames.TOMORROW_AFTERNOON:

            row2manipulate = tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
            row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
            row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
            row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
            row2manipulate.patient = buildPatientHTML(updated_ridePat);
            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            //row2manipulate.englishOriginAndDestination = updated_ridePat.Origin.EnglishName + " - " + updated_ridePat.Destination.EnglishName;
            row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.TOMORROW_AFTERNOON, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());
            break;

        case tableNames.FUTURE:

            row2manipulate = tFuture.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.date = thisRidePatDate.toLocaleDateString('he-IL');
            row2manipulate.time = buildTimeHTML(thisRidePatDate, updated_ridePat.RidePatNum);
            row2manipulate.origin = !isAssistant ? updated_ridePat.Origin.Name : updated_ridePat.Origin.EnglishName;
            row2manipulate.destination = !isAssistant ? updated_ridePat.Destination.Name : updated_ridePat.Destination.EnglishName;
            row2manipulate.patient = buildPatientHTML(updated_ridePat);
            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers[0], updated_ridePat.RideNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            //row2manipulate.englishOriginAndDestination = updated_ridePat.Origin.EnglishName + " - " + updated_ridePat.Destination.EnglishName;
            row2manipulate.remark = buildRemarkHTML(updated_ridePat.Remark, updated_ridePat.RidePatNum);;
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tFuture.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.FUTURE, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tFuture.rows().nodes());
            tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);
            manipilateNodeStyle(tr_node2manipulate, updated_ridePat, ADD_DRIVER, true);
            // tr_nodesAsArray = null;
            break;

        default:
            console.log('system recognized a tempt of the server to post real-time data but failed to found the rellevant row to redraw');
            tr_nodesAsArray = null;
            break;
    }
    if (tr_nodesAsArray) {

        tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);
        manipilateNodeStyle(tr_node2manipulate, updated_ridePat, ADD_DRIVER);

        updatedRidePat_status = updated_ridePat.Pat.RidePatPatientStatus.Status;
        isAfterNoonRide = thisRidePatDate.getMinutes() === afternoonIndicator;
        treatmentFinished = updatedRidePat_status === patientStatus.Finished;

        if (isAfterNoonRide && treatmentFinished) {

            manipilateNodeStyle(tr_node2manipulate, updated_ridePat, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
        } else if (treatmentFinished && !isAfterNoonRide) {

            $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
        }
    }
}

notification.client.driverHasRemovedFromRidePat = function (updated_ridePat) {
    /*********************
     * !!! IMPORTANT !!! *
     ********↓↓↓**********/
    //update the specific ride in arr_rides:
    update_arr_rides(updated_ridePat);
    /*********↑↑↑*********
     * !!! IMPORTANT !!! *
     *********************/
    tableName2manipulate = getTableName2Manipulate(updated_ridePat.RidePatNum);
    thisRidePatDate = fixDate_WhichComeFromOpenConnection(updated_ridePat.Date);
    thisRidePatLastModified = fixDate_WhichComeFromOpenConnection(updated_ridePat.LastModified);

    switch (tableName2manipulate) {

        case tableNames.MORNING:

            row2manipulate = tMorning.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.MORNING, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tMorning.rows().nodes());
            break;

        case tableNames.AFTERNOON:

            row2manipulate = tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.AFTERNOON, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tAfternoon.rows().nodes());
            break;

        case tableNames.TOMORROW_MORNING:

            row2manipulate = tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tTomorrowMorning.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.TOMORROW_MORNING, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());
            break;

        case tableNames.TOMORROW_AFTERNOON:

            row2manipulate = tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tTomorrowAfternoon.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.TOMORROW_AFTERNOON, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tTomorrowMorning.rows().nodes());
            break;



        case tableNames.FUTURE:

            row2manipulate = tFuture.row(`#${updated_ridePat.RidePatNum}`).data();

            row2manipulate.driver = buildDriverHTML(updated_ridePat.RidePatNum);
            row2manipulate.status = buildStatusString(updated_ridePat.Pat.RidePatPatientStatus, updated_ridePat.RidePatNum);
            row2manipulate.lastModified = thisRidePatLastModified.toLocaleString('he-IL', { dateStyle: "short", timeStyle: "short" });
            row2manipulate.buttons = buildAcionsButtonsHTML(updated_ridePat.RidePatNum, updated_ridePat.Drivers);

            tFuture.row(`#${updated_ridePat.RidePatNum}`).data(row2manipulate).draw('page');
            increaseCounter(tableNames.FUTURE, updated_ridePat.RidePatNum);

            tr_nodesAsArray = Array.from(tFuture.rows().nodes());
            tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);
            manipilateNodeStyle(tr_node2manipulate, updated_ridePat, REMOVE_DRIVER, true, tableName2manipulate);
            //tr_nodesAsArray = null;
            break;

        default:

            console.log('system recognized a tempt of the server to post real-time data but failed to found the rellevant row to redraw');
            tr_nodesAsArray = null;
            break;
    }

    if (tr_nodesAsArray) {

        tr_node2manipulate = tr_nodesAsArray.find(node => node.id === `${updated_ridePat.RidePatNum}`);
        manipilateNodeStyle(tr_node2manipulate, updated_ridePat, REMOVE_DRIVER, false, tableName2manipulate);

        updatedRidePat_status = updated_ridePat.Pat.RidePatPatientStatus.Status;
        treatmentFinished = updatedRidePat_status === patientStatus.Finished;

        if (treatmentFinished) {

            manipilateNodeStyle(tr_node2manipulate, updated_ridePat, RIDEPAT_PATIENT_STATUS_EDITTIME_CHANGED);
        } else {

            $(tr_node2manipulate).removeClass('highlightRow-after-patientStatusModal');
        }

    }
}

//#endregion

