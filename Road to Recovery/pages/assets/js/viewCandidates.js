﻿checkCookie();

const wiringDataTables = () => {
    //manage button clicks on tables

    //      EXAMPLE:

    //$('#datatable-morning tbody').on('click', '#candidatesBtn', function () {
    //    candidatesButton(this);
    //});

}

$(document).ready(() => {

    let ridePatObj = JSON.parse(localStorage.getItem('ridePatObj')); //Yogev last mark, next step is write the panels dynamically
    console.log(ridePatObj);
    
    if (!JSON.parse(localStorage.getItem("isProductionDatabase"))) {
        $("#databaseType").text("Test database ")
    }
    else $("#databaseType").text("Production database")
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body'
    })
    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.indexOf('test') != -1) {
        $("#na").css("background-color", "#ffde89");
    }
    if (window.location.href.indexOf('http://40.117.122.242/Road%20to%20Recovery/') != -1) {
        window.location.href = "notAvailable.html";
    }

    if (JSON.parse(GENERAL.USER.getIsAssistant())) {
        $("#menuType").attr("w3-include-html", "HelperMenu.html");
        $("#userName").html(GENERAL.USER.getAsistantAndCoorDisplayName());
    }
    else {
        $("#menuType").attr("w3-include-html", "menu.html");
        $("#userName").html(GENERAL.USER.getUserDisplayName());

    }
    includeHTML();//with out this there is no side bar!


    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "customdatesort-pre": function (a) {
            var r, x;
            if (a === null || a === "") {
                r = false;
            } else {
                x = a.split("/");
                r = +new Date(+x[2], +x[1] - 1, +x[0]);
            }
            //console.log("[PRECALC] " + a + " becomes " + r);
            return r;
        },
        "customdatesort-asc": function (a, b) {
            if (a === false && b === false) {
                return 0;
            } else if (a === false) {
                return 1;
            } else if (b === false) {
                return -1;
            } else {
                return a - b;
            }
        },
        "customdatesort-desc": function (a, b) {
            if (a === false && b === false) {
                return 0;
            } else if (a === false) {
                return 1;
            } else if (b === false) {
                return -1;
            } else {
                return b - a;
            }
        }
    });



    /*
                 ============================
                 || ↓DATATABLES PROPERTIES↓||
                 ============================
    */
    candidatesTable = $('#datatable-candidates').DataTable(
        {
            "stateDuration": 60 * 60,
            "columnDefs": []
        },
        {
            "paging": false
        },
        {
            "visible": false,
            "targets": [1]
        },
        {
            "className": "dt-body-left",
            "targets": [3]
        }
    );


    superDriversTable = $('#datatable-superDrivers').DataTable(
        {
            "stateDuration": 60 * 60,
            "columnDefs": []
        },
        {
            "paging": false
        },
        {
            "visible": false,
            "targets": [1]
        },
        {
            "className": "dt-body-left",
            "targets": [3]
        }
    );



    /*
                 ============================
                 || ↑DATATABLES PROPERTIES↑||
                 ============================
    */



});




function refreshTable() {
    //???

}




function manipulateTables() {

    tableMorning = document.getElementById("datatable-morning");
    tableAfternoon = document.getElementById("datatable-afternoon");

}