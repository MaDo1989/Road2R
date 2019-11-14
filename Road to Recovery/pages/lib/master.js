
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
            localStorage.escortedList =escortedList;
        }
    },

    RIDEPAT: {
        getRidePatList: function () {
            return LZString.decompress(localStorage.ridePatList);
        },
        setRidePatList: function (ridePatList) {
            localStorage.ridePatList = LZString.compress(ridePatList);
        }
    },

    PATIENTS: {
        getPatientsList: function () {
            return LZString.decompress(localStorage.patientsList);
        },
        setPatientsList: function (patientsList) {
            localStorage.patientsList = LZString.compress(patientsList);
        }
    }
};








