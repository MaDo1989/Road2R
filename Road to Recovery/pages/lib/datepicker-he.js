﻿
function datePicker() {
    jQuery('#dateRide').datepicker({
        toggleActive: true,
        clearBtn: true,
        autoclose: true
    });
 
    jQuery('#joinDate').datepicker({
        toggleActive: true,
        clearBtn: true,
        autoclose: true
    });
   // $.datepicker.setDefaults($.datepicker.regional[""]);
    
    jQuery('#birthDate').datepicker({
        toggleActive: true,
        clearBtn: true,
        autoclose: true
    });


  //  $.datepicker.setDefaults($.datepicker.regional[""]);

    $.datepicker.setDefaults($.datepicker.regional[""]);
    $("dateRide").datepicker($.datepicker.regional["he"]);

    $.datepicker.setDefaults($.datepicker.regional[""]);
    $("birthDate").datepicker($.datepicker.regional["he"]);

    $.datepicker.setDefaults($.datepicker.regional[""]);
    $("joinDate").datepicker($.datepicker.regional["he"]);


    /* Hebrew initialisation for the UI Datepicker extension. */
    /* Written by Amir Hardon (ahardon at gmail dot com). */
    (function (factory) {
        if (typeof define === "function" && define.amd) {

            // AMD. Register as an anonymous module.
            define(["../widgets/datepicker"], factory);
        } else {

            // Browser globals
            factory(jQuery.datepicker);
        }
    }(function (datepicker) {

        datepicker.regional.he = {
            closeText: "סגור",
            prevText: "&#x3C;הקודם",
            nextText: "הבא&#x3E;",
            currentText: "היום",
            monthNames: ["ינואר", "פברואר", "מרץ", "אפריל", "מאי", "יוני",
                "יולי", "אוגוסט", "ספטמבר", "אוקטובר", "נובמבר", "דצמבר"],
            monthNamesShort: ["ינו", "פבר", "מרץ", "אפר", "מאי", "יוני",
                "יולי", "אוג", "ספט", "אוק", "נוב", "דצמ"],
            dayNames: ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"],
            dayNamesShort: ["א'", "ב'", "ג'", "ד'", "ה'", "ו'", "שבת"],
            dayNamesMin: ["א'", "ב'", "ג'", "ד'", "ה'", "ו'", "שבת"],
            weekHeader: "Wk",
            dateFormat: "dd/mm/yy",
            firstDay: 0,
            isRTL: true,
            showMonthAfterYear: false,
            yearSuffix: ""
        };
        datepicker.setDefaults(datepicker.regional.he);

        return datepicker.regional.he;

    }));
}