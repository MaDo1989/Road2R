daysArr = { 0: "יום א'", 1: "יום ב'", 2: "יום ג'", 3: "יום ד'", 4: "יום ה'", 5: "יום ו'", 6: "יום ש'", };


function initLocalStorage() {
    lsArr = ["north", "center", "south", "sunday", "monday", "tuesday", "wednesday","thursday", "friday", "saturday"];
    for (var i = 0; i < lsArr.length; i++) {
        if (localStorage.getItem(lsArr[i]) === null)
            localStorage[lsArr[i]] = true;
    }
}


function fillLocations(data) {
    let fullList = JSON.parse(data.d);
    for (loc of fullList) {
        if (loc.Area.includes("דרום") || loc.Area.includes("ארז")) {
            southLocations.push(loc.Name);
        }
        if (loc.Area.includes("מרכז") || loc.Area.includes("תרקומיא") || loc.Area.includes("ירושלים")) {
            centerLocations.push(loc.Name);
        }
        if (loc.Area.includes("צפון")) {
            northLocations.push(loc.Name);
        }
    }

    let locations = {
        South: southLocations,
        Center: centerLocations,
        North: northLocations
    };

    return locations;
}


function initCheckBoxes() {

    initCB(localStorage.south,     '#south');
    initCB(localStorage.center,    '#center');
    initCB(localStorage.north,     '#north');
    initCB(localStorage.sunday,    '#sunday');
    initCB(localStorage.monday,    '#monday');
    initCB(localStorage.tuesday,   '#tuesday');
    initCB(localStorage.wednesday, '#wednesday');
    initCB(localStorage.thursday,  '#thursday');
    initCB(localStorage.friday,    '#friday');
    initCB(localStorage.saturday,  '#saturday');
}


function setHebDay(lsday, weekdays,hebLetter) {
    let d = JSON.parse(lsday);
    if (d)
        weekdays.push(hebLetter);
}

function saveDays(weekdays) {
    saveDay("sunday",    weekdays, "א");
    saveDay("monday",    weekdays, "ב");
    saveDay("tuesday",   weekdays, "ג");
    saveDay("wednesday", weekdays, "ד");
    saveDay("thursday",  weekdays, "ה");
    saveDay("friday",    weekdays, "ו");
    saveDay("saturday",  weekdays, "ש");
}

function saveDay(cntrlId, weekdays, hebLetter) {

    localStorage[cntrlId] = $("#" + cntrlId).prop('checked');
    setHebDay(localStorage[cntrlId], weekdays, hebLetter);
}





function HebrewWeekDays(ls, weekdays) {

    setHebDay(ls.sunday,    weekdays, "א");
    setHebDay(ls.monday,    weekdays, "ב");
    setHebDay(ls.tuesday,   weekdays, "ג");
    setHebDay(ls.wednesday, weekdays, "ד");
    setHebDay(ls.thursday,  weekdays, "ה");
    setHebDay(ls.friday,    weekdays, "ו");
    setHebDay(ls.saturday,  weekdays, "ש");
}


function initCB(ls,cntrlId) {

    if (ls == "undefined" || ls === undefined) {
        ls = true;
        $(cntrlId).prop('checked', true);
    } else {
        $(cntrlId).prop('checked', JSON.parse(ls));
    }

}


function getWeeklyThanks(rides) {
    thanks = [];
    for (var i = 0; i < rides.length; i++) {
        if (rides[i].Status == "ממתינה לשיבוץ") continue;
        var coordinator = rides[i].Coordinator.DisplayName;
        var driver = rides[i].Drivers[0].DisplayName;
        if (thanks[coordinator] == undefined) {
            thanks[coordinator] = [];
            thanks[coordinator][driver] = 1;
        }
        else {
            if (thanks[coordinator][driver] == undefined) {
                thanks[coordinator][driver] = 1;
            }
            else {
                thanks[coordinator][driver]++;
            }
        }
    }

    return thanks;
}

function showThanks() {
    var thanks = getWeeklyThanks(arr_rides);
    str = "";
    for (c in thanks) {
        str += "<h3>" + c + "</h3>";
        for (d in thanks[c]) {
            str += "<span>" + d + ", </span>";
        }
        //str = str.substring(0, str.length - 1);
    }
    document.getElementById("report").style.visibility = "visible";
    document.getElementById("thanks").innerHTML = str;

}

function closeReport() {
    document.getElementById("report").style.visibility = "hidden";
}
