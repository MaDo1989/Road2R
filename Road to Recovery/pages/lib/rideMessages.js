let days = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];

const showMessage = (arr_rides, ridePatNum) => {
    const ridepat = arr_rides.find((r) => r.RidePatNum === ridePatNum);
    
    if (ridepat.Drivers.length == 0) return;
    // get all the rides going to this direction with this driver
    const onlyRidepatsWithDriver = arr_rides.filter(r => r.Drivers.length > 0);
    AllRidesForThisDriver = onlyRidepatsWithDriver.filter((r) => {
        return (
            r.Destination.Name === ridepat.Destination.Name &&
            r.Origin.Name === ridepat.Origin.Name &&
            r.Drivers[0].Id === ridepat.Drivers[0].Id &&
            r.Date === ridepat.Date
        )
    });
    // remove names that include _ used for unique display names
    let driverName = AllRidesForThisDriver[0].Drivers[0].DisplayName.split("_")[0];
    message = {
        origin: AllRidesForThisDriver[0].Origin.Name,
        destination: AllRidesForThisDriver[0].Destination.Name,
        driver: driverName,
        date: AllRidesForThisDriver[0].Date,
        patients: [],
        totalPeople: 0
    }


    for (var i = 0; i < AllRidesForThisDriver.length; i++) {
        message.totalPeople += 1;
        if (AllRidesForThisDriver[i].Pat.IsAnonymous) {
            patient = {
                isAnonymous: true,
                numOfEscorts: AllRidesForThisDriver[i].Pat.EscortedList.length
            }
        }
        else {
            patient = {
                isAnonymous: false,
                name: AllRidesForThisDriver[i].Pat.DisplayName.split("_")[0],
                EnglishName: AllRidesForThisDriver[i].Pat.EnglishName,
                numOfescorts: AllRidesForThisDriver[i].Pat.EscortedList.length,
                cellPhone: AllRidesForThisDriver[i].Pat.CellPhone,
                escorts: AllRidesForThisDriver[i].Pat.EscortedList
            }
        }
        message.totalPeople += AllRidesForThisDriver[i].Pat.EscortedList.length;
        message.patients.push(patient);
        message.ridePatNum = ridePatNum;

    }

    return message;

}

function buildMessage(message) {

    //sep = `<br/>`;
    sep = `\n`;

    //let txt = `${message.ridePatNum}` + sep;
    let txt = `שלום ${message.driver}` + sep;
    txt += `הסעה מ${message.origin} ל${message.destination}` + sep;
    if (message.totalPeople === 1)
        txt += `סה"כ אדם אחד` + sep;
    else
        txt += `סה"כ ${message.totalPeople} אנשים` + sep;
    txt += messageDate(message.date) + sep;
    if (message.patients.length === 1) {
        txt += sep;
        txt += "הפרטים:" + sep;
        txt += patientMessage(message.patients[0]);
        if (!message.patients[0].isAnonymous) {
            let phoneText = getPatientsPhonesText(message.patients[0]);
            if (phoneText != ``) {
                txt += `טלפונים:` + sep;
                txt += phoneText;
            }
        }
    }
    else {
        //txt += `הסעת ${message.patients.length} חולים` + sep;
        for (var i = 0; i < message.patients.length; i++) {
            txt += sep;
            txt += `פרטי חולה ${i + 1}:` + sep;
            txt += patientMessage(message.patients[i]);
            if (!message.patients[i].isAnonymous) {
                let phoneText = getPatientsPhonesText(message.patients[i]);
                if (phoneText != ``) {
                    txt += `טלפונים:` + sep;
                    txt += phoneText;
                }
            }
        }
    }

    //txt += `***********************************` + sep;
    //txt += `***********************************` + sep;
    return txt;
  //  $("#message").append(txt);
}

const getPatientsPhonesText = (patient) => {
    let txt = ``;
    if (validateMobileNumFullVersion(patient.cellPhone)) {
        let cellphone = patient.cellPhone.slice(0, 3) + "-" + patient.cellPhone.slice(3, 9)
        txt += `${patient.name}: ${cellphone}` + sep;
    }

    for (var i = 0; i < patient.escorts.length; i++) {
        if (patient.escorts[i].IsAnonymous == false && validateMobileNumFullVersion(patient.escorts[i].CellPhone))
            txt += `${patient.escorts[i].DisplayName}: ${patient.escorts[i].CellPhone}` + sep;
    }
    return txt;
}

validateMobileNumFullVersion = (mobileNum) => {

    if (!mobileNum || isNaN(parseInt(mobileNum))) { return false; }

    if (mobileNum.length !== 10) { return false; }

    const num2test_arr = Array.from(mobileNum);

    if (num2test_arr[0] !== '0' || num2test_arr[1] !== '5') { return false; }

    return true;
}

function messageDate(date) {
    // let jsDate = convertDBDate2FrontEndDate(date);
    let txt = ``;
    let currentDate = new Date();
    let rideDate = new Date(netDate(date));
    let dayInWeek = days[rideDate.getDay()];
    let sameDay = datesAreOnSameDay(currentDate, rideDate);
    let hour = rideDate.getHours();
    let min = rideDate.getMinutes();
    if (min == "0") min = "00";
    let afternoonRide = false;
    if (min == 14) afternoonRide = true;
    if (sameDay && !afternoonRide)
        txt = `היום בשעה ${hour}:${min}`;
    else if (sameDay && afternoonRide)
        txt = `היום אחר הצהריים`;
    else if (!sameDay && !afternoonRide)
        txt = `ביום ${dayInWeek} ה${rideDate.getDate()}/${rideDate.getMonth()} בשעה ${hour}:${min}`;
    else
        txt = `ביום ${dayInWeek} ה${rideDate.getDate()}/${rideDate.getMonth()} בשעות אחר הצהריים`;
    return txt;
}

const datesAreOnSameDay = (first, second) =>
    first.getFullYear() === second.getFullYear() &&
    first.getMonth() === second.getMonth() &&
    first.getDate() === second.getDate();

const netDate = (fullTimeStempStr) => {
    let startTrim = fullTimeStempStr.indexOf('(') + 1;
    let endTrim = fullTimeStempStr.indexOf(')');
    let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
    return parseInt(fullTimeStempNumber);
}

const patientMessage = (patient) => {
    let txt = "";
    try {
        txt = (patient.isAnonymous) ? `חולה` : `${patient.name}`;
    }
    catch
    {
        console.log("error in patientMessage");
        console.log(patient);
    }
    txt = (patient.isAnonymous) ? `חולה` : `${patient.name}`;
    txt += sep;
    numberOfEscorts = (patient.isAnonymous) ? patient.numOfEscorts : patient.escorts.length;
    if (numberOfEscorts === 0)
        txt += `בלי מלווה`;
    else if (numberOfEscorts === 1)
        txt += `עם מלווה אחד`;
    else
        txt += `עם ${numberOfEscorts} מלווים`;
    txt += sep;
    return txt;
}