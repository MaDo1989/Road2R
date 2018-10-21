var GENERAL = {

    USER: {
        getUserName: function () {
            return localStorage.user;
        },
        setUserName: function (userName) {
            localStorage.user = userName;
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
        }

    },

    VOLUNTEERS: {
        getVolunteersList: function () {
            return localStorage.volunteersList;
        },
        setVolunteersList: function (volunteersList) {
            localStorage.volunteersList = volunteersList;
        }
    },
    LOCATIONS: {
        getDestinationsList: function () {
            return localStorage.locationsList;
        },
        setLocationsList: function (locationsList) {
            localStorage.locationsList = locationsList;
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
            return localStorage.ridePatList;
        },
        setRidePatList: function (ridePatList) {
            localStorage.ridePatList = ridePatList;
        }
    },

    PATIENTS: {
        getPatientsList: function () {
            return localStorage.patientsList;
        },
        setPatientsList: function (patientsList) {
            localStorage.patientsList = patientsList;
        }
    }
}








