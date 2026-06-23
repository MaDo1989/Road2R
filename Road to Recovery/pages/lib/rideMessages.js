let days = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];




const showMessage = (arr_rides, ridePatNum) => {
    let ridepat = arr_rides.find(r => r.RidePatNum === ridePatNum);
    ridepat = CustomRideObject(ridepat);

    if (!ridepat || ridepat.Drivers.length === 0) return;

    const onlyRidepatsWithDriver = arr_rides.filter(r => r.MainDriver > 0);

    let AllRidesForThisDriver = onlyRidepatsWithDriver.filter(r => {
        const oneRide = CustomRideObject(r);
        return (
            oneRide.Origin.Name === ridepat.Origin.Name &&
            oneRide.Drivers[0].Id === ridepat.Drivers[0].Id &&
            oneRide.Date === ridepat.Date
        );
    });

    let driverName = AllRidesForThisDriver[0].DriverName.split("_")[0];

    let allDestinations = [...new Set(AllRidesForThisDriver.map(r => CustomRideObject(r).Destination.Name))];

    let message = {
        origin: AllRidesForThisDriver[0].Origin,
        destinations: allDestinations,
        driver: driverName,
        date: AllRidesForThisDriver[0].PickupTime,
        patients: [],
        totalPeople: 0,
        ridePatNum: ridePatNum
    };

    for (let i = 0; i < AllRidesForThisDriver.length; i++) {
        let ride = CustomRideObject(AllRidesForThisDriver[i]);
        let patient;

        if (ride.Pat.IsAnonymous) {
            patient = {
                isAnonymous: true,
                numOfEscorts: ride.Pat.EscortedList.length,
                dest: ride.Destination.Name
            };
        } else {
            patient = {
                isAnonymous: false,
                name: ride.Pat.DisplayName.split("_")[0],
                EnglishName: ride.Pat.EnglishName,
                numOfescorts: ride.Pat.EscortedList.length,
                cellPhone: ride.Pat.cellPhone,
                escorts: ride.Pat.EscortedList,
                GenderAsEnum: ride.Pat.GenderAsEnum,
                Age: ride.Pat.Age,
                dest: ride.Destination.Name
            };
        }

        patient.OnlyEscort = ride.OnlyEscort;

        // Important: attach the specific ride number for this patient
        patient.ridePatNum = ride.RidePatNum;

        if (patient.OnlyEscort !== true) {
            message.totalPeople += 1;
        }

        message.totalPeople += ride.Pat.EscortedList.length;

        message.patients.push(patient);
        console.log('from show message : ', message);
    }

    return message;
};



function buildMessage(message,hour,min) {
    const sep = `\n`;
    let firstName = message.driver.split(" ")[0];

    let destText = message.destinations.length === 1
        ? `ל${message.destinations[0]}`
        : `ל${message.destinations.join(" ול")}`;

    const shouldShowDestination = message.destinations.length > 1;

    let txt = `,שלום ${firstName}${sep}`;
    txt += `הסעה מ${message.origin} ${destText}${sep}`;
    txt += message.totalPeople === 1
        ? `סה"כ אדם אחד${sep}`
        : `סה"כ ${message.totalPeople} אנשים${sep}`;

    txt += messageDate(message.date,hour,min);

    for (let i = 0; i < message.patients.length; i++) {
        let p = message.patients[i];
        txt += sep + sep;
        txt += patientMessage(p, shouldShowDestination, sep);

        if (!p.isAnonymous) {
            const rideObjForPatient = findRideByNumber(p.ridePatNum);
            let phoneText = getPatientsPhonesText(p, rideObjForPatient, sep);
            if (phoneText !== ``) txt += phoneText;
        }
    }

    txt += sep + sep + "!תודה ונסיעה טובה";
    return txt;
}




const getPatientsPhonesText = (patient, rideObj, sep) => {
    let txt = ``;
    const seen = new Set();

    const digitsOnly = s => String(s || '').replace(/\D/g, '');

    const addIfValid = (raw) => {
        if (!raw) return;
        if (typeof validateMobileNumFullVersion === 'function' && !validateMobileNumFullVersion(raw)) return;

        const d = digitsOnly(raw);
        if (!d) return;
        if (seen.has(d)) return;
        seen.add(d);

        const formatted = (d.length > 3) ? `${d.slice(0, 3)}-${d.slice(3)}` : d;

        if (txt.length === 0) {
            txt += `${formatted}`;
        } else {
            txt += sep + `${formatted}`;
        }
    };

    addIfValid(patient?.cellPhone);
    addIfValid(patient?.cellPhone1);
    addIfValid(rideObj?.PatientCellPhone);
    addIfValid(rideObj?.PatientCellPhone2);
    addIfValid(rideObj?.PatientCellPhone3);

    return txt;
};



validateMobileNumFullVersion = (mobileNum) => {
    if (!mobileNum || isNaN(parseInt(mobileNum))) {
        return false;
    }

    if (mobileNum.length !== 10) {
        return false;
    }

    const num2test_arr = Array.from(mobileNum);

    if (num2test_arr[0] !== "0" || num2test_arr[1] !== "5") {
        return false;
    }

    return true;
};

function messageDate(date,Hour,Min) {
    let txt = ``;
    let currentDate = new Date();

    let normalizedDate = date;
    let ticks = parseInt(normalizedDate.replace(/\D/g, ''));
    let rideDate = new Date(ticks);

    let dayInWeek = days[rideDate.getDay()];
    let sameDay = datesAreOnSameDay(currentDate, rideDate);

    let hour = Hour;
    let min = Min;

    if (min == 0) min = "00";

    let afternoonRide = (min == 14);

    if (sameDay && !afternoonRide) {
        txt = `היום בשעה ${hour}:${min}`;
    } else if (sameDay && afternoonRide) {
        txt = `היום אחר הצהריים`;
    } else if (!sameDay && !afternoonRide) {
        txt = `ביום ${dayInWeek} ${rideDate.getDate()}.${rideDate.getMonth() + 1} בשעה ${hour}:${min}`;
    } else {
        txt = `ביום ${dayInWeek} ${rideDate.getDate()}.${rideDate.getMonth() + 1} בשעות אחר הצהריים`;
    }

    return txt;
}

const datesAreOnSameDay = (first, second) =>
    first.getFullYear() === second.getFullYear() &&
    first.getMonth() === second.getMonth() &&
    first.getDate() === second.getDate();

const netDate = (fullTimeStempStr) => {
    let startTrim = fullTimeStempStr.indexOf("(") + 1;
    let endTrim = fullTimeStempStr.indexOf(")");
    let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
    return parseInt(fullTimeStempNumber);
};

const patientMessage = (patient, shouldShowDestination, sep) => {
    let txt = "";
    let agePrefix = ``;

    if (patient.GenderAsEnum == 0) {
        agePrefix = `בת`;
        if (patient.Age < 1) agePrefix = `תינוקת`;
    }
    else if (patient.GenderAsEnum == 1) {
        agePrefix = `בן`;
        if (patient.Age < 1) agePrefix = `תינוק`;
    }
    else {
        agePrefix = `בגיל`;
        if (patient.Age == 0) agePrefix = ``;
    }

    if (patient.Age == 1) agePrefix += ` שנה `;
    else if (patient.Age == 2) agePrefix += ` שנתיים `;
    else if (patient.Age > 2) agePrefix += ` ${patient.Age}`;

    txt = patient.isAnonymous ? `חולה` : `${patient.name}, ${agePrefix}`.trim();

    if (shouldShowDestination && patient.dest) {
        txt += ` (ל${patient.dest})`;
    }

    txt += sep;

    let numberOfEscorts = patient.isAnonymous
        ? patient.numOfEscorts
        : patient.escorts.length;

    if (numberOfEscorts === 0) txt += `בלי מלווה`;
    else if (numberOfEscorts === 1) {
        if (patient.OnlyEscort === false) txt += `עם מלווה אחד`;
        else txt += `רק המלווה`;
    }
    else {
        if (patient.OnlyEscort === false) txt += `עם ${numberOfEscorts} מלווים`;
        else txt += `רק ${numberOfEscorts} המלווים`;
    }

    txt += sep;
    return txt;
};


function parseMSDateNoOffset(dateInput) {
    let d;

    if (typeof dateInput === 'string' && dateInput.startsWith('/Date(')) {
        // פורמט מקורי: /Date(1779660840000)/
        const ticks = parseInt(dateInput.replace('/Date(', '').replace(')/', ''));
        d = new Date(ticks);
        // שימוש ב-UTC getters כי ה-ticks מגיעים "naive" מהשרת
        return `/Date(${new Date(
            d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate(),
            d.getUTCHours(), d.getUTCMinutes(), d.getUTCSeconds()
        ).getTime()})/`;

    } else if (typeof dateInput === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(dateInput)) {
        // פורמט ISO 8601: 2026-06-09T07:00:00+03:00 או 2026-06-09T07:00:00
        // קורעים את ה-offset ולוקחים את הזמן המקומי כפשוטו
        const withoutOffset = dateInput.replace(/([+-]\d{2}:\d{2}|Z)$/, '');
        d = new Date(withoutOffset); // ייפורס כ-local time ללא offset
        return `/Date(${d.getTime()})/`;

    } else {
        throw new Error(`parseMSDateNoOffset: פורמט לא מוכר: ${dateInput}`);
    }
}