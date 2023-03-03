const afterNoon = `אחה"צ`;
const maleInHebrew = "זכר";
const femaleInHebrew = "נקבה";
const male = "Male";
const female = "Female";
const afternoonIndicator = 14;
const patientStatus = {
    Empty: -1,
    NotFinished: 0,
    Finished: 1
};


var GENERAL = {

    USER: {
        getUserName: function () {
            return localStorage.user;
        },
        setUserName: function (userName) {
            localStorage.user = userName;
        },
        getIsAssistant: function () {
            return localStorage.isAssistant;
        },
        setIsAssistant: function (isAssistant) {
            localStorage.isAssistant = isAssistant;
        },
        getUserDisplayName: function () {
            return localStorage.userCell;
        },
        setUserDisplayName: function (userCell) {
            localStorage.userCell = userCell;
        },
        getAsistantDisplayName: function () {
            return localStorage.assistantCell;
        },
        setAsistantDisplayName: function (assistantCell) {
            localStorage.assistantCell = assistantCell;
        },
        getAsistantAndCoorDisplayName: function () {
            return localStorage.coorAndAssistant;
        },
        setAsistantAndCoorDisplayName: function (names) {
            localStorage.coorAndAssistant = names;
        },
        getPassword: function () {
            return localStorage.password;
        },
        setPassword: function (password) {
            localStorage.password = password;
        },
        getUserType: function () {
            return localStorage.userType;
        },
        setUserType: function (userType) {
            localStorage.userType = userType;
        },
        getCoorAssistant: function () {
            return localStorage.coorAssistant;
        },
        setCoorAssistant: function (coorAssistant) {
            localStorage.coorAssistant = coorAssistant;
        }
    },

    VOLUNTEERS: {
        getVolunteersList: function () {
            return LZString.decompress(localStorage.volunteersList);
        },
        setVolunteersList: function (volunteersList) {
            localStorage.volunteersList = LZString.compress(volunteersList);
        }
    },

    LOCATIONS: {
        getDestinationsList: function () {
            return LZString.decompress(localStorage.locationsList);
        },
        setLocationsList: function (locationsList) {
            localStorage.locationsList = LZString.compress(locationsList);
        }
    },

    ESCORTED: {
        getEscortedList: function () {
            return localStorage.escortedList;
        },
        setEscortedList: function (escortedList) {
            localStorage.escortedList = escortedList;
        }
    },

    RIDEPAT: {
        getRidePatList: function () {
            return LZString.decompress(localStorage.ridePatList);
        },
        setRidePatList: function (ridePatList) {
            localStorage.ridePatList = LZString.compress(ridePatList);
        },
        getRidePatNum4_viewCandidate: function () {
            return LZString.decompress(localStorage.RidePatNum4_viewCandidate);
        },
        setRidePatNum4_viewCandidate: function (RidePatNum4_viewCandidate) {
            localStorage.RidePatNum4_viewCandidate = LZString.compress(RidePatNum4_viewCandidate);
        }
    },

    PATIENTS: {
        getPatientsList: function () { //this name does not make sense since it get a json of {displayName: "", func: "new" || "edit" || "show"}
            return LZString.decompress(localStorage.patientsList);
        },
        setPatientsList: function (patientsList) {
            localStorage.patientsList = LZString.compress(patientsList);
        },
        getPatients: function () { //this will get all the exist Patients from local storage
            return LZString.decompress(localStorage.Patients);
        },
        setPatients: function (Patients) { //this will set all the exist Patients into local storage
            localStorage.Patients = LZString.compress(Patients);
        },
        GetPatientGender: function (enumNumber, isAssistant) {

            let genderAsString = '';
            if (!isAssistant) {

                switch (enumNumber) {
                    case 0:
                        genderAsString = femaleInHebrew;
                        break;
                    case 1:
                        genderAsString = maleInHebrew;
                        break;
                }
            } else {
                switch (enumNumber) {
                    case 0:
                        genderAsString = "Female";
                        break;
                    case 1:
                        genderAsString = "Male";
                        break;
                }
            }

            return genderAsString;
        },
        GetPatientStatus: function (enumNumber, isAssistant) {
            let statusAsString = '';
            if (!isAssistant) {
                switch (enumNumber) {
                    case 0:
                        statusAsString = "לא סיים";
                        break;
                    case 1:
                        statusAsString = "סיים";
                        break;
                    default:
                        statusAsString = "לא סיים";
                        break;
                }
            } else {
                switch (enumNumber) {
                    case 0:
                        statusAsString = "Not Finished";
                        break;
                    case 1:
                        statusAsString = "Finished";
                        break;
                    default:
                        statusAsString = "Not Finished";
                        break;
                }
            }

            return statusAsString;
        },

    },

    FETCH_DATA: {
        ajaxCall: function (funcNameInWebService, data, successCB, errorCB) {
            $.ajax({
                dataType: "json",
                url: `WebService.asmx/${funcNameInWebService}`,
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: data,
                success: successCB,
                error: errorCB
            });
        },
        ajaxCall_WithGzipMe: function (funcNameInWebService, data, successCB, errorCB, async = true) {
            $.ajax({
                dataType: "json",
                url: `WebService.asmx/${funcNameInWebService}`,
                contentType: "application/json; charset=utf-8",
                type: "POST",                                  /*WE ALWAYS USE POST*/
                async: async,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Content-Encoding", "gzip");
                },
                data: data,
                success: successCB,
                error: errorCB
            });
        }

    },

    USEFULL_FUNCTIONS: {

        /**
           * Gets a string of /Date(1608581640000).
           * Returns an int → 1608581640000
         */
        convert2DBDateToInt: (fullTimeStempStr) => {
            let startTrim = fullTimeStempStr.indexOf('(') + 1;
            let endTrim = fullTimeStempStr.indexOf(')');
            let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
            return parseInt(fullTimeStempNumber);

        },

        /**
          * Gets a string of /Date(1608581640000).
          * Returns a Date obj → new Date(1608581640000)
         */
        convertDBDate2FrontEndDate: (fullTimeStampStr) => { // fullTimeStempStr = this form → "/Date(1608581640000)/" OR '2022-04-02T03:00:00'

            if (typeof fullTimeStampStr === 'undefined' || !fullTimeStampStr) return "";

            if (fullTimeStampStr.toString().includes('Date')) {

                return new Date(GENERAL.USEFULL_FUNCTIONS.convert2DBDateToInt(fullTimeStampStr));
            } else if (GENERAL.USEFULL_FUNCTIONS.isFromOpenConnection(fullTimeStampStr)) {

                return GENERAL.USEFULL_FUNCTIONS.fixDate_WhichComeFromOpenConnection(fullTimeStampStr);
            } else {

                return new Date(fullTimeStampStr);
            }
        },

        fixDate_WhichComeFromOpenConnection: (date2fix, getDate = true) => {

            const ticks = new Date(date2fix) - new Date().getTimezoneOffset() * 60000;

            if (getDate) {
                return new Date(ticks);
            }

            return ticks;
        },

        isFromOpenConnection: (date) => {
            let isFromOpenCon = date.toString().includes('T');

            return isFromOpenCon;
        },

        getHebrew_WeekDay: (day) => {
            let days = [];
            days[days.length] = "יום ראשון";
            days[days.length] = "יום שני";
            days[days.length] = "יום שלישי";
            days[days.length] = "יום רביעי";
            days[days.length] = "יום חמישי";
            days[days.length] = "יום שישי";
            days[days.length] = "יום שבת";

            return days[day];

        },

        buildStringforEquipment: (hebrewArr) => {
            if (hebrewArr.length === 0) return "(None) אין";
            let str = "";
            for (var i = 0; i < hebrewArr.length; i++) {
                switch (hebrewArr[i]) {

                    case "כסא גלגלים":
                        str += `${hebrewArr[i]} (Wheelchair), `;
                        break;
                    case "קביים":
                        str += `${hebrewArr[i]} (Crutches), `;
                        break;
                }
            }
            str = str.substring(0, str.lastIndexOf(", "));
            return str;
        },

        buildStringforDriverResponsibilityEquipment: (hebrewArr) => {

            let str = '';
            for (var i = 0; i < hebrewArr.length; i++) {
                switch (hebrewArr[i]) {
                    case "כסא תינוק":
                        str += `${hebrewArr[i]} (Baby seat), `;
                        break;
                    case "בוסטר":
                        str += `${hebrewArr[i]} (Buster), `;
                        break;
                }
            }
            if (!hebrewArr.includes('כסא תינוק') && !hebrewArr.includes('בוסטר')) return "(None) אין";
            str = str.substring(0, str.lastIndexOf(", "));
            return str;
        },

        /**
            * Gets a string of israeli phone number with no "-".
            * Returns a boolean weather the phone number has 10 valid digits
        */
        validateMobileNum: (mobileNum) => {

            if (!mobileNum || isNaN(parseInt(mobileNum))) { return false; }

            return mobileNum.length === 10;
        },

        /**
            * Gets a string of israeli phone number with no "-".
            * Returns a boolean weather the phone number valid or not, true is valid
        */
        validateMobileNumFullVersion: (mobileNum) => {

            if (!mobileNum || isNaN(parseInt(mobileNum))) { return false; }

            if (mobileNum.length !== 10) { return false; }

            const num2test_arr = Array.from(mobileNum);

            if (num2test_arr[0] !== '0' || num2test_arr[1] !== '5') { return false; }

            return true;
        },

        /**
          * Gets a string of israeli phone number with no "-".
          * Returns a new string of the phone number with a string seperator
        */
        addSeperator2MobileNum: (mobileNum, Seperator = '-') => {

            let newStr = '';
            newStr = mobileNum.substring(0, 3);

            newStr += Seperator;

            newStr += mobileNum.substring(3, mobileNum.length);

            return newStr;

        },

        /**
       * Gets two dates 
       * Returns the hours gap between them.
       * notes:
       *  - The function return can be negative or positive depands on the parameter
       *        for instance if you insert values to the function like this: getHoursGap(early, late) → return will be negative and vice versa
       * this logic comes from the need to know if a ridepat is in G hours or allready passed (and how many hours passes since)
     */
        getHoursGap: (x, y) => {

            let miliSeconds_gap = x - y;
            let hours_gap = miliSeconds_gap / (1000 * 60 * 60); //1000 ms in 1 sec, 60 sec in 1m, 60 min in 1h
            return hours_gap;
        },
        /**
     * This function used when use in arr.sort(compareFunc)
     * 
    */
        compareFunc: (a, b) => {

            let x = a.Name.trim();
            let y = b.Name.trim();

            return x < y ? -1 : x > y ? 1 : 0;
        },

        showMe: ({ objName, displayName }) => {

            const func = 'edit';
            let arr_details;

            switch (objName) {
                case 'patient':
                    arr_details = { displayName, func };
                    GENERAL.PATIENTS.setPatientsList(JSON.stringify(arr_details));

                    break;
                case 'volunteer':
                    arr_details = { displayName, func };
                    GENERAL.VOLUNTEERS.setVolunteersList(JSON.stringify(arr_details));

                    break;
            }

        },
        /**
        * a and b are a pair of dateTime to compare these two general example:
        * dd.mm.yyy, hh:mm OR dd.mm.yyy, אחה"צ 
        * אחה"צ is calculated as 23:59 
        * Returns an int 
        * Ascending: → 0 if a=b, 1 if a > b, -1 if a < b
        * Descending: → 0 if a=b,-1 if a > b,  1 if a > b
        */
        datetimeCompareFunc: (a, b, isAscending) => {

            let dateAndTimeArrayof_a = $.trim(a).split(', ');
            let dateAndTimeArrayof_b = $.trim(b).split(', ');


            let ddmmyyyArr_a = dateAndTimeArrayof_a[0].split('.');
            let ddmmyyyArr_b = dateAndTimeArrayof_b[0].split('.');

            let dd_a = parseInt(ddmmyyyArr_a[0]);
            let mm_a = parseInt(ddmmyyyArr_a[1]);
            let yyyy_a = parseInt(ddmmyyyArr_a[2]);

            let dd_b = parseInt(ddmmyyyArr_b[0]);
            let mm_b = parseInt(ddmmyyyArr_b[1]);
            let yyyy_b = parseInt(ddmmyyyArr_b[2]);


            let time_a = dateAndTimeArrayof_a[1].split(':');
            let time_b = dateAndTimeArrayof_b[1].split(':');

            let hh_a = time_a[0] === afterNoon ? 23 : parseInt(time_a[0]);
            let minutes_a = time_a[0] === afterNoon ? 59 : parseInt(time_a[1]);

            let hh_b = time_b[0] === afterNoon ? 23 : parseInt(time_b[0]);
            let minutes_b = time_b[0] === afterNoon ? 59 : parseInt(time_b[1]);

            //new Date(year, monthIndex, day, hours, minutes)
            let a_dateAndTime = new Date(yyyy_a, mm_a - 1, dd_a, hh_a, minutes_a);
            let b_dateAndTime = new Date(yyyy_b, mm_b - 1, dd_b, hh_b, minutes_b);

            a = a_dateAndTime.getTime();
            b = b_dateAndTime.getTime();

            let result;
            if (isAscending) {

                result = a === b ? 0 : a > b ? 1 : -1;
            } else {//Descending

                result = a === b ? 0 : a > b ? -1 : 1;
            }
            return result;
        },

        dateCompareFunc: (a, b, isAscending) => {
            let ddmmyyyArr_a = $.trim(a).split('.');
            let ddmmyyyArr_b = $.trim(b).split('.');

            let dd_a = parseInt(ddmmyyyArr_a[0]);
            let mm_a = parseInt(ddmmyyyArr_a[1]);
            let yyyy_a = parseInt(ddmmyyyArr_a[2]);

            let dd_b = parseInt(ddmmyyyArr_b[0]);
            let mm_b = parseInt(ddmmyyyArr_b[1]);
            let yyyy_b = parseInt(ddmmyyyArr_b[2]);


            let a_date = new Date(yyyy_a, mm_a + 1, dd_a);
            let b_date = new Date(yyyy_b, mm_b + 1, dd_b);

            a = a_date.getTime();
            b = b_date.getTime();
            let result;
            if (isAscending) {

                a = isNaN(a) ? Infinity : a;
                b = isNaN(b) ? Infinity : b;

                result = a === b ? 0 : a > b ? 1 : -1;
            } else {//Descending

                a = isNaN(a) ? -Infinity : a;
                b = isNaN(b) ? -Infinity : b;

                result = a === b ? 0 : a > b ? -1 : 1;
            }
            return result;
        },

        isDate: (obj) => {

            let result = obj instanceof Date && !isNaN(obj);
            return result;
        },

        IsNullOrUndefined: (obj) => {

            let result = obj === null || obj === undefined;
            return result;
        }
    },

    COPYWRITE: () => {
        return "2022 - 2018 © כל הזכויות שמורות לעמותת בדרך להחלמה";
    },

    APP_ID: 1,
}