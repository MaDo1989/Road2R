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
        }

    },

    ORDERS: {
        getOrdersList: function () {
            return localStorage.ordersList;
        },
        setOrdersList: function (ordersList) {
            localStorage.ordersList = ordersList;
        }
    },

    CUSTOMERS: {
        getCustomersList: function () {
            return localStorage.customersList;
        },
        setCustomersList: function (customersList) {
            localStorage.customersList = customersList;
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
    ESCORTED: {
        getEscortedList: function () {
            return localStorage.escortedList;
        },
        setEscortedList: function (escortedList) {
            localStorage.escortedList = escortedList;
        }
    },

    PATIENTS: {
        getPatientsList: function () {
            return localStorage.patientsList;
        },
        setPatientsList: function (patientsList) {
            localStorage.patientsList = patientsList;
        }
    },


    SERVICES: {
        getServicesList: function () {
            return localStorage.servicesList;
        },
        setServicesList: function (servicesList) {
            localStorage.servicesList = servicesList;
        }
    },

    PRICELISTS: {
        getPriceListList: function () {
            return localStorage.priceListList;
        },
        setPriceListList: function (priceListList) {
            localStorage.priceListList = priceListList;
        }
    },

    TRUCKS: {
        getTrucksList: function () {
            return localStorage.trucksList;
        },
        setTrucksList: function (trucksList) {
            localStorage.trucksList = trucksList;
        }
    },

    DRIVERS: {
        getDriversList: function () {
            return localStorage.driversList;
        },
        setDriversList: function (driversList) {
            localStorage.driversList = driversList;
        }
    },

    TRUCKHANDLINGS: {
        getTruckHandlingsList: function () {
            return localStorage.truckHandlingsList;
        },
        setTruckHandlingsList: function (truckHandlingsList) {
            localStorage.truckHandlingsList = truckHandlingsList;
        }
    },


    DDLTruckType: {
        getTruckType: function () {
            return localStorage.DDLTruckType;
        },
        setTruckType: function (TruckType) {
            localStorage.DDLTruckType = TruckType;
        }
    },
    DOCUMENTS: {
        getDocumentsList: function () {
            return localStorage.documentsList;
        },
        setDocumentsList: function (documentsList) {
            localStorage.documentsList = documentsList;
        }
    }

}

//function sayHello(name) {
//    return "hello " + name;
//}


//function buildCalc($elem) {


//}










