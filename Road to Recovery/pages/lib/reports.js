// Purpose: JS code for the reports UI



// 24-Nov:  Avishai
//1.  . מספר המתנדבים השונים שהסיעו בכל חודש בשנתיים האחרונות. 



// 31-Oct-20:  Implement code after populating template_DATE_LATER_THAN, so that if radio group
// is checked, we respect it and get all volunteers.
// Set default date to 1/1/2020 


//   GetRidePatViewByTimeFilter    ==>   All RIDES  

// 020Oct : Shlomit Meler bug.
/* 
 * This is teh code that returns empty results:
 * 
 *
 ReportService.cs :   getPickupForDriver() 
    SELECT *
    FROM RPView
    WHERE  MainDriver = '18312'
    ;

 I do not recall from where I copied it.

need to read notes on what RPView is, and see where I copied it from.... 




// --------------------------------------

// Downhill Park:
// 1. Export to PDF now prints in hebrew (as I created the font file), but it displayed reversed.
// Need to look at pdfmake doc about RTL, or in data-table.
// Also, shoudl check which font is used now in the HTML, maybe that font is better ?


/* Instructions for printing PDF Hebrew 
 *  https://github.com/bpampuch/pdfmake/issues/1496
 *  
  c.customize && c.customize(a);
            a = d.pdfMake.createPdf(a);


Downloaded font from:
   https://fonts.google.com/specimen/Alef?subset=hebrew&preview.text=&preview.text_type=custom&sidebar.open=true&selection.family=Alef:wght@700


https://pdfmake.github.io/docs/fonts/custom-fonts-client-side/vfs/


Created vfs.js : 

   git clone https://github.com/bpampuch/pdfmake.git .
   cd examples/fonts/
   explorer.exe  .
   cd ..
   node build-vfs.js

and do a mnaul change to it, at teh end to regiser as .vfs
            
             * */

/* Perf analsysis
 
  http://localhost:54573/Road%20to%20Recovery/pages/WebService.asmx/GetRidePatView
  
    {"volunteerId":-2,"maxDays":-1}
 
 RidePat.cs :  GetRidePatView() 
    //VolunteerId - 1 means get ALL FUTURE ridePats // VolunteerId -2 means get ALL ridePats


pages\ridePatForm.html  loadPage :     if (JSON.parse(GENERAL.RIDEPAT.getRidePatList()).length != 0)
    Does not look like anyone is using arr_customer for its actuall array.
    But it does need to be a non-epty array for something to happen.
    So it's enough to store just a non-empty array.


 * */



// 05-07: Focus on reports content, Use UI as-is fix it later.
//  ==> finish all services is the goal.

// Park: 06-06 : working  on week picker by customizing datepicker
// see comments at end of  populate_week_field() - need to fix it
// Then we can start implement the repot of amute week


// DEBUG:  We  use אלון שדה  &  בני בורנפלד
// new report:   פר אזור: רשימת המתנדבים      "מתנדבים שבועי"
// TODO:  Implement GetReportRidesWeeklyPerRegion() to call  getVolunteerRidesPerWeek() and return result as an array of volunteers
// TODO: GetReportRidesWeeklyPerRegion() is just a copy of old code now, so need to cleanit up and set values in Vounteer structure
// TODO:  for some numeric values we need to return.

// TODO: Implement UI of teh " "מתנדבים שבועי  report

// TODO: make sure report is per requirements: add missing fields of  , ק"ם
// TODO: Fix export to PDF
// TODO: Customize print table for RTL   https://datatables.net/forums/discussion/44355/i-was-able-to-right-align-my-print-layout-how-do-i-apply-that-to-individual-columns
// TODO: Do the actual post in "Print"


/* Internal notes 
 *   getContactType --> The relationship types - Father,Sister etc...
 *  
 *  RidePat.cs :  GetRidePatView()     
 *  Escorted e = new Escorted();
 *  -		ItemArray	{object[3]}	object[]
		[0]	2123	object {int}
		[1]	"אבו בודק בדיקה1"	object {string}
		[2]	"0547298598"	object {string}

 *   Query is "select * from RidePatEscortView where RidePatNum = 2123 "

 * */

/* Documentation:
 * For each report, we have different input fields.
 * 'K_fields_map' is a global map that lists the fields to be used for each report.
 * 
 * When a report is selected, we create the fields per the map
 * The creation is done by cloning a field template from the HTML into the fields div
 * 
 * In some cases, we do not need to show a field but as it's assumed to have a default value
 * e.g. "Rides this year" assumes the month field is January-this-year.
 * In this case, we create a hidden field with the same id as if it was visible.
 * See 'rp_vl_ride_year__month'
 * 
 */

// Current startegy for refreshing teh table, set per report type
var S_refresh_preview = null;

function init_reports_page() {
    includeHTML();
    set_banner_debug_data();
    process_permissions();

    init_components();
}


function set_banner_debug_data() {
    if (!JSON.parse(localStorage.getItem("isProductionDatabase"))) {
        $("#databaseType").text("Test database ")
    }
    else {
        $("#databaseType").text("Production database")
    }
    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.indexOf('test') != -1) {
        $("#na").css("background-color", "#ffde89");
    }
}

//* Purpose show UI that is available to specific users
function process_permissions() {
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetCurrentUserEntitlements",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: true,
        success: function (data) {
            var permissions = data.d;
            for (a_permission of permissions) {
                if (a_permission == "Record_NI_report") {
                    $("#rp_special_list").show();
                }
            }
        },
        error: function (err) { alert("Error in process_permissions"); }
    });

}

function init_components() {
    // Datatables date-time plugin
    $.fn.dataTable.moment('DD/MM/YYYY');
}
// Handle a click event on one of the reports in the Reports-Tree
function on_report_click(event) {

    if (S_refresh_preview == null) {
        // here for quick debuggign something on first click
       }


    var report_type = event.target.id;
    populate_parameters(report_type);
    S_refresh_preview = K_strategy[report_type];
 }


// How to refresh the table, per each report type
var K_strategy = {
    "rp_vl_ride_month": rp_vl_ride_month__refresh_preview,
    "rp_vl_ride_year": rp_vl_ride_year__refresh_preview,
    "rp_amuta_vls_week": rp_amuta_vls_week__refresh_preview,
    "rp_amuta_vls_per_pat": rp_amuta_vls_per_pat__refresh_preview,
    "rp_amuta_vls_km": rp_amuta_vls_km__refresh_preview,
    "rp_amuta_vls_list": rp_amuta_vls_list__refresh_preview,
    "rp_amuta_vls_per_month": rp_amuta_vls_per_month__refresh_preview
}


// list of fields to be used for each report-type 
var K_fields_map = {
    "rp_vl_ride_month": [
        {
            id: "rp_vl_ride_month__name",
            type: "VOLUNTEER",
            template: 'div[name="template_VOLUNTEER"]',
            post_clone: field_volunteers_post_clone
        },
        {
            id: "rp_vl_ride_month__month",
            template: 'div[name="template_MONTH"]',
            type: "MONTH",
            post_clone: field_month_post_clone
        }
    ],
    "rp_vl_ride_year": [
        {
            id: "rp_vl_ride_year__name",
            type: "VOLUNTEER",
            template: 'div[name="template_VOLUNTEER"]',
            post_clone: field_volunteers_post_clone
        },
        {
            id: "rp_vl_ride_year__year",
            type: "YEAR",
            template: 'div[name="template_YEAR"]',
            post_clone: rp_vl_ride_year__field_year_post_clone
        }

    ],
    "rp_amuta_vls_week": [
        {
            id: "rp_amuta_vls_week__week",
            type: "WEEK",
            template: 'div[name="template_WEEK"]',
            post_clone: field_week_post_clone
        }
    ],
    "rp_amuta_vls_per_pat": [
        {
            id: "rp_amuta_vls_per_pat__patient",
            type: "PATIENT",
            template: 'div[name="template_PATIENT"]',
            post_clone: field_patient_post_clone
        }
    ],
    "rp_amuta_vls_km": [
        {
            id: "rp_amuta_vls_per_km__year",
            type: "YEAR",
            template: 'div[name="template_YEAR"]',
            post_clone: rp_amuta_vls_per_km_field_year_post_clone
        }
    ],
    "rp_amuta_vls_list": [
        {
            id: "rp_amuta_vls_list__radio",
            type: "YEAR",
            template: 'div[name="template_VOLUNTEER_LIST_RADIO"]',
            post_clone: rp_amuta_vls_list_field_radio_post_clone
        }
    ],
    "rp_amuta_vls_per_month": [
        {
            id: "rp_vl_ride_month__month",
            template: 'div[name="template_PER_MONTH"]',
            type: "MONTH",
            post_clone: rp_amuta_vls_per_month__refresh_preview
        }
    ]

}

function populate_parameters(report_type) {
 //   $("#params_ph").text("Populating for " + report_type);
    $("#params_ph").empty();
    let report_link = $("#" + report_type);

    hide_all_tables();
    // Mark the current in bold
    $(".report_link_cls").css("font-weight", "normal");
    report_link.css("font-weight", "bold");
    $("#report_title").html(report_link.text());
    var fields = rp_get_fields(report_type);
    if (fields) {
        fields.forEach(rp_add_one_parameter);
    }
}

// returns a list of fields per a report type
function rp_get_fields(report_type) {

    return K_fields_map[report_type];

}

// Makes sure we remove name from clone and set an id
function clone_template(template, parent_id) {
    var result = $(template).clone().prop("id", parent_id + "_field");
    result.removeAttr("name");

    // assign unique id to the input element
    var input = result.find("input");
    input.each(function () {
        var new_id = $(this).attr("template_id");
        if (new_id) {
            $(this).prop("id", new_id);
        }
    })
    var select = result.find("select");
    select.prop("id", select.attr("template_id"));

    // if has "template_name", assign it to its name
    var select = result.find("input[type=radio]");
    select.prop("name", select.attr("template_name"));


     
    result.appendTo("#" + parent_id);
    result.removeClass("report_template");
    return result;
}

var K_CACHE = {
    volunteers: [],
    patients: []
};



// on_load_volunteers called when the async ajax call  has finished
// Used to populate UI needing the volunteers list
function loadVolunteers(on_load_volunteers) {

    if (K_CACHE.volunteers.length > 1) {
        // One time loading already done.
        on_load_volunteers();
        return;
    }

    if (false) {
        console.log("loadVolunteers: speeding up by adding one entry only in debugging")
        var debug_entry = { label: "tt", id: 14535 };
        K_CACHE.volunteers.push(debug_entry);
        debug_entry = { label: "beny", id: 14430 };
        K_CACHE.volunteers.push(debug_entry);
        on_load_volunteers();
        return;
    }

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getVolunteers",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: true,
        data: JSON.stringify({ active: true }),
        success: function (data) {
            var arr_drivers = JSON.parse(data.d);
            arr_drivers.sort();
            for (i in arr_drivers) {
                var entry = { label: arr_drivers[i].DisplayName, id: arr_drivers[i].Id };
                K_CACHE.volunteers.push(entry);
            }

            var debug_entry = { label: "tt", id: 14535 };
            K_CACHE.volunteers.push(debug_entry);

            on_load_volunteers();
        },
        error: function (err) { alert("Error in loadVolunteers"); }
    });
}

// Called when a volunteer is selected in auto-complete
function on_volunteer_selected(event, ui) {
    // store teh selected value on the element
    $("#" + event.target.id).attr("itemID", ui.item.id);
    refreshPreview();
    return true;
}
// called when the async ajax call to laod volunteers has finished
// Used to populate UI needing the volunteers list
function populate_volunteer_field() {
    $("#select_driver").autocomplete({
        source: K_CACHE.volunteers,
        select: on_volunteer_selected
        });
}

function get_last_month_date() {
    var prev_month = new Date();
    prev_month.setDate(0); // 0 will result in the last day of the previous month
    prev_month.setDate(1); 
    return prev_month;
}

function populate_month_field() {
    
    var dt = $('#select_month').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        onClose: function () { console.log("populate_month_field()::onClose; Does not work, maybe due to duplicate IDS??"); },
        autoclose: true
    });
    // compute previous month
    dt.datepicker('setDate', get_last_month_date());
    dt.on("changeDate", refreshPreview);
}

function field_month_post_clone(id) {
    populate_month_field();
}

function empty_func(id) {

}

function rp_amuta_vls_per_km_field_year_post_clone(id) {
    var today = new Date();
    $('#select_year').val(today.getFullYear() - 1);
    $("#select_year").change(rp_amuta_vls_km__refresh_preview);

    rp_amuta_vls_km__refresh_preview();
}

function rp_amuta_vls_list_field_radio_post_clone(id) {
    var today = new Date();
    $('#select_date_later').val("2020-01-01");
    $("#select_date_later").change(rp_amuta_vls_list__refresh_preview);

    $("#radio_start_date").change(rp_amuta_vls_list__refresh_preview);
    $("#radio_all").change(rp_amuta_vls_list__refresh_preview);

    rp_amuta_vls_list__refresh_preview();
}


function rp_vl_ride_year__field_year_post_clone(id) {
    var today = new Date();
    $('#select_year').val(today.getFullYear());
    $("#select_year").change(rp_vl_ride_year__refresh_preview);

}


// Given a defintition of field  build the UI for it in the Parameters panel
// The UI is coped from a template in the page. Each field type has its own template
function rp_add_one_parameter(def) {
    $("#params_ph").append($("<div>", { id: def.id }));
    var field_elem = $("#" + def.id);
    // field_elem.append(def.label + " XYZ");
    var clone = clone_template(def.template, def.id);
    if (def.post_clone) {
        def.post_clone(def.id);
    }
}

function field_volunteers_post_clone(id) {
    loadVolunteers(populate_volunteer_field);
}


function populate_week_field() {

    $("#select_week").change(function () {
        refreshPreview();
    });

    // https://stackoverflow.com/a/9402266

    /*
   dt =  $("#select_week").datepicker({
        dateFormat: "yy-mm-dd",
        showOtherMonths: true,
        selectOtherMonths: true,
   });

*/
    //@@ dt.on("show", function (e) {

        // This is working. Need to  do it for the tr and support mouseleave and hide()

        //console.log("Show", e);
        //$(document).on('mouseenter', '.datepicker-days',
        //    function () { console.log($(this)); $(this).find('td a').addClass('ui-state-hover'); });
    //@@ });

    // DEBUG: set date to March
    /* 
    dt.datepicker('setDate', new Date(2020, 6));
    dt.on("changeDate", function (e) {
     refreshPreview();
  });

*/
}

function field_week_post_clone(id) {
    populate_week_field();
}


function field_patient_post_clone(id) {
    loadPatients(populate_patient_field)
}


// Called when a volunteer is selected in auto-complete
function on_patient_selected(event, ui) {
    // store the selected value on the element
    $("#" + event.target.id).attr("itemID", ui.item.id);
    refreshPreview();
    return true;
}
// called when the async ajax call to laod volunteers has finished
// Used to populate UI needing the volunteers list
function populate_patient_field() {
    $("#select_patient").autocomplete({
        source: K_CACHE.patients,
        select: on_patient_selected
    });
}


// on_load_patients called when the async ajax call  has finished
// Used to populate UI needing the patients list
function loadPatients(on_load_patients) {

    if (K_CACHE.patients.length > 1) {
        // One time loading already done.
        on_load_patients();
        return;
    }

    if (false) {
        console.log("loadPatients: speeding up by adding one entry only in debugging")
        var debug_entry = { label: "tt", id: 22 };
        K_CACHE.patients.push(debug_entry);
        debug_entry = { label: "ganem", id: 21 };
        K_CACHE.patients.push(debug_entry);
        on_load_patients();
        return;
    }

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetPatientsDisplayNames",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: true,
        data: JSON.stringify({ active: true }),
        success: function (data) {
            var arr_patients = JSON.parse(data.d);
            for (i in arr_patients) {
                var entry = { label: arr_patients[i].Name, id: arr_patients[i].ID };
                K_CACHE.patients.push(entry);
            }

            var debug_entry = { label: "tt", id: 14535 };
            K_CACHE.patients.push(debug_entry);

            on_load_patients();
        },
        error: function (err) { alert("Error in loadVolunteers"); }
    });
}

function refreshPreview() {
    S_refresh_preview();
}   

// Checks if all fields are filled. If so refresh the report
function rp_vl_ride_month__refresh_preview() {
    var selected_date = Date.parse($("#select_month").val());
    if (selected_date) {
        var start_month_date = moment(selected_date);
        var end_month_date = start_month_date.clone().add(1, 'months');

        var volunteerId = $("#select_driver").attr("itemID");
        if (volunteerId) {
            refreshTable(volunteerId,
                start_month_date.format("YYYY-MM-DD"),
                end_month_date.format("YYYY-MM-DD"));
        }
    }
}   


function rp_vl_ride_year__refresh_preview() {
    var volunteerId = $("#select_driver").attr("itemID");
    if (volunteerId) {
        var end_date;
        var selected_year = new Date( $("#select_year").val());
        var start_date = moment(selected_year);
        
        // this year end today, not on 31-Dec 
        var today = new Date();
        if (today.getFullYear() == selected_year.getFullYear()) {
            // this year end today, not on 31-Dec 
            end_date = moment(today);
        }
        else {
            selected_year.setMonth(11);
            selected_year.setDate(31);
            end_date = moment(selected_year);
        }

        refreshTable(volunteerId,
                start_date.format("YYYY-MM-DD"),
                end_date.format("YYYY-MM-DD"));
    }
}   


function rp_amuta_vls_week__refresh_preview() {
    var selected_date = Date.parse($("#select_week").val());
    var m = moment(selected_date);
    var start_week_date = m.startOf('week').format("YYYY-MM-DD");
    var end_week_date = m.endOf('week').format("YYYY-MM-DD");


    refresh_amuta_vls_week_Table(
        start_week_date, end_week_date);
  
}   

function rp_amuta_vls_km__refresh_preview() {
    var selected_date = Date.parse($("#select_year").val());
    var m = moment(selected_date);
    var start_week_date = m.startOf('year').format("YYYY-MM-DD");
    var end_week_date = m.endOf('year').format("YYYY-MM-DD");

    refresh_amuta_vls_km_Table(
        start_week_date, end_week_date);
}   

function rp_amuta_vls_list__refresh_preview() {
    var selected_date = $('#select_date_later').val();
    var config = "start_date";
    if ($("#radio_all").is(":checked")) {
        config = "all";
    }

    refresh_amuta_vls_list_Table(selected_date, config);
}   

function rp_amuta_vls_per_month__refresh_preview() {
//    var selected_date = $('#select_date_later').val();
//    var config = "start_date";

    refresh_amuta_vls_per_month_Table("2019-01-01");
}   


// 'start_date' :  a date formatted as YYYY-MM-DD
// 'end_date'   :  a date formatted as YYYY-MM-DD
function refresh_amuta_vls_week_Table(start_date, end_date) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteerWeekly",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);

            $('#div_table_amuta_vls_week').show();
            tbl = $('#table_amuta_vls_week').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Region" },
                    { data: "Volunteer" }

                ],
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'pdfHtml5',
                        orientation: 'landscape',
                        pageSize: 'LEGAL',
                        customize: function (doc) {
                            pdfMake.fonts = {
                                hebrewFont: {
                                    normal: 'Alef-Regular.ttf',
                                    bold: 'Alef-Bold.ttf',
                                    italics: 'Alef-Regular.ttf',
                                    bolditalics: 'Alef-Bold.ttf'
                                }
                            };

                            doc.defaultStyle = {
                                font: 'hebrewFont'
                            };
                            console.log(doc);
                        } 
                    }
                ],

                xbuttons: [
                    'csv', 'excel', 'pdf'
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}

// 'start_date' :  a date formatted as YYYY-MM-DD
// 'end_date'   :  a date formatted as YYYY-MM-DD
function refresh_amuta_vls_km_Table(start_date, end_date) {

    

    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteersKM",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);

            $('#div_table_amuta_vls_km').show();
            tbl = $('#table_amuta_vls_km').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Date" },
                    { data: "Volunteer" },
                    { data: "Patient" },
                    { data: "Origin" },
                    { data: "Destination" }

                ],
                dom: 'Bfrtip',
                buttons: [
                   'csv', 'excel', 'pdf'
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}



// 'start_date' :  a date formatted as YYYY-MM-DD
function refresh_amuta_vls_per_month_Table(start_date) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteerPerMonth",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);

            $('#div_table_amuta_vls_per_month').show();
            tbl = $('#table_amuta_vls_month').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Year" },
                    { data: "Month" },
                    { data: "Count" }

                ],
                dom: 'Bfrtip',
 

                buttons: [
                    'csv', 'excel', 
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}


// 'start_date' :  a date formatted as YYYY-MM-DD
function refresh_amuta_vls_list_Table(start_date, config) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date,
        config: config
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteerList",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);

            $('#div_table_amuta_vls_list').show();
            tbl = $('#table_amuta_vls_list').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [

                    { data: "FirstNameH" },
                    { data: "LastNameH" },
                    { data: "VolunteerIdentity" },
                    { data: "Email" },
                    { data: "Address" },
                    { data: "CityCityName" },
                    { data: "JoinDate" }
                ],
                dom: 'Bfrtip',


                buttons: [
                    'csv', 'excel',
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}





function rp_amuta_vls_per_pat__refresh_preview() {
    var patient = $("#select_patient").attr("itemID");
    refresh_amuta_vls_per_pat_Table(patient);
}

function refresh_amuta_vls_per_pat_Table(patient) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        patient: patient
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteersPerPatient",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = data.d;

            $('#div_table_amuta_vls_per_pat').show();
            tbl = $('#table_amuta_vls_per_pat').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Date" },
                    { data: "Volunteer" },
                    { data: "Origin" },
                    { data: "Destination" }

                ],
                dom: 'Bfrtip',
                buttons: [
                     'csv', 'excel', 'pdf'
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}


// 'start_date' :  a date formatted as YYYY-MM-DD
// 'end_date'   :  a date formatted as YYYY-MM-DD
function refreshTable(volunteerId, start_date, end_date) {
    ridesToShow = [];

    $('#wait').show();
    hide_all_tables();
    $('#div_weeklyRides').show();
    var query_object = {
        volunteerId: volunteerId,
        start_date: start_date,
        end_date: end_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportVolunteerRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = data.d
            // DEBUG console.log(arr_rides);

            for (i in arr_rides) {

                let obj = arr_rides[i];

                var date = moment(obj.Date, "DD/MM/YYYY HH:mm:ss", true);
                date.add(3, 'hours'); // TimeZone differen between UTC and Israel
                var HEBday = getDayString(date.day());

                var d = new Date();

                if (obj.PatDisplayName.includes("אנונימי")) {
                    patDisplayName = "חולה";
                } else {
                    patDisplayName = obj.PatDisplayName;
                }

//@@                if (arr_rides[i].Pat.EscortedList.length != 0) {
//@@                    patDisplayName += " + " + arr_rides[i].Pat.EscortedList.length;
//@@                 }

               // date2 = HEBday + " " + day + "/" + month + "/" + date.getUTCFullYear() % 2000;
                date2 = HEBday + " " + date.format("DD/MM/YY");
                time = date.format("HH:mm");

                if (time == "22:14") { //22:14 is the default time to show afternoon אחה''צ
                    time = " אחה\"צ";
                }


                var Ride = {};

                Ride = {
                    Date: date2,
                    OriginName: obj.OriginName,
                    DestinationName: obj.DestinationName,
                    Time: time,
                    PatDisplayName: patDisplayName
                }
                ridesToShow.push(Ride);
           }

            tbl = $('#weeklyRides').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: ridesToShow,
                destroy: true,
                columnDefs: [
                    { "orderData": [0, 3], "targets": 0 }],
                columns: [
                    { data: "Date" },
                    { data: "Time" },
                    { data: "OriginName" },
                    { data: "DestinationName" },
                    { data: "PatDisplayName" },

                ],
                dom: 'Bfrtip',
                buttons: [
                     'csv', 'excel', 'pdf'
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

 }

function hide_all_tables() {
    $('#div_weeklyRides').hide();
    $('#div_table_amuta_vls_week').hide();
    $('#div_table_amuta_vls_per_pat').hide();
    $('#div_table_amuta_vls_km').hide();
    $('#div_table_amuta_vls_list').hide();
    $('#div_table_amuta_vls_per_month').hide();
 }


function load_location() {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/getLocations",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: "",
        success: function (data) {
            let fullList = JSON.parse(data.d)
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

            locations = {
                South: southLocations,
                Center: centerLocations,
                North: northLocations
            }

        }, error: function (error) {
            console.log(error);
        }
    });

}