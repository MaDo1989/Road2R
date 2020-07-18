// Purpose: JS code for the reports UI

//  11-07:  Use 'label' in K_fiels_map to display the title



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


console.log("Shalom");


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
    "rp_amuta_vls_km": rp_amuta_vls_km__refresh_preview
}


// list of fields to be used for each report-type 
var K_fields_map = {
    "rp_vl_ride_month": [
        {
            id: "rp_vl_ride_month__name",
            type: "VOLUNTEER",
            label: "הסעות החודש",
            template: 'div[name="template_VOLUNTEER"]',
            post_clone: field_volunteers_post_clone
        },
        {
            id: "rp_vl_ride_month__month",
            template: 'div[name="template_MONTH"]',
            type: "MONTH",
            label: "מתנדבים החודש",
            post_clone: field_month_post_clone
        }
    ],
    "rp_vl_ride_year": [
        {
            id: "rp_vl_ride_year__name",
            type: "VOLUNTEER",
            label: "מתנדבים שנתי",
            template: 'div[name="template_VOLUNTEER"]',
            post_clone: field_volunteers_post_clone
        }
    ],
    "rp_amuta_vls_week": [
        {
            id: "rp_amuta_vls_week__week",
            type: "WEEK",
            label: "מתנדבים - שבועי",
            template: 'div[name="template_WEEK"]',
            post_clone: field_week_post_clone
        }
    ],
    "rp_amuta_vls_per_pat": [
        {
            id: "rp_amuta_vls_per_pat__patient",
            type: "PATIENT",
            label: "מתנדבים פר חולה",
            template: 'div[name="template_PATIENT"]',
            post_clone: field_patient_post_clone
        }
    ],
    "rp_amuta_vls_km": [
        {
            id: "rp_amuta_vls_per_km__year",
            type: "YEAR",
            label: "מתנדבים - שנתי",
            template: 'div[name="template_YEAR"]',
            post_clone: field_year_post_clone
        }
    ]
}

function populate_parameters(report_type) {
 //   $("#params_ph").text("Populating for " + report_type);
    $("#params_ph").empty();
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
    input.prop("id", input.attr("template_id"));

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


function field_year_post_clone(id) {
    var today = new Date();
    $('#select_year').val(today.getFullYear() - 1);
    $("#select_year").change(on_year_change);

    rp_amuta_vls_km__refresh_preview();
}

function on_year_change() {
    rp_amuta_vls_km__refresh_preview();
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
    // https://stackoverflow.com/a/9402266

   dt =  $("#select_week").datepicker({
        dateFormat: "yy-mm-dd",
        showOtherMonths: true,
        selectOtherMonths: true,
   });

    //@@ dt.on("show", function (e) {

        // This is working. Need to  do it for the tr and support mouseleave and hide()

        //console.log("Show", e);
        //$(document).on('mouseenter', '.datepicker-days',
        //    function () { console.log($(this)); $(this).find('td a').addClass('ui-state-hover'); });
    //@@ });

    // DEBUG: set date to March
    dt.datepicker('setDate', new Date(2020, 6));
    dt.on("changeDate", function (e) {
     refreshPreview();
  });

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
        // default date range - this year up till now
        var today = new Date();
        var end_month_date = moment(today);
        today.setMonth(0);
        today.setDate(1);
        var start_month_date = moment(today);

        refreshTable(volunteerId,
                start_month_date.format("YYYY-MM-DD"),
                end_month_date.format("YYYY-MM-DD"));
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
                    'print', 'csv', 'excel', 'pdf'
                ],
                createdRow: function (row, data, dataIndex) {
                        $(row).css('background-color', '#f1f1f1');
                },
                rowCallback: function (row, data, index) {
                }
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
                    'print', 'csv', 'excel', 'pdf'
                ],
                createdRow: function (row, data, dataIndex) {
                    $(row).css('background-color', '#f1f1f1');
                },
                rowCallback: function (row, data, index) {
                }
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
            arr_rides = JSON.parse(data.d);

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
                    'print', 'csv', 'excel', 'pdf'
                ],
                createdRow: function (row, data, dataIndex) {
                    $(row).css('background-color', '#f1f1f1');
                },
                rowCallback: function (row, data, index) {
                }
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
    allRides = [];

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
            arr_rides = JSON.parse(data.d);
            // DEBUG console.log(arr_rides);

            for (i in arr_rides) {

                //Change date format
                var temp = arr_rides[i].Date.substring(arr_rides[i].Date.indexOf("(") + 1);
                temp = temp.substring(0, temp.indexOf(")"));
                var date = new Date(parseInt(temp));

                var HEBday = getDayString(date.getDay());

                if (arr_rides[i].Drivers.length != 0) {
                    driverName = arr_rides[i].Drivers[0].DisplayName;
                }
                destinationName = arr_rides[i].Destination.Name;
                originName = arr_rides[i].Origin.Name;


                if (date.getDate() < 10) {
                    day = "0" + date.getDate();
                } else day = date.getDate();


                if (date.getMonth() + 1 < 10) {
                    month = "0" + (date.getMonth() + 1);
                } else month = date.getMonth() + 1;


                var d = new Date();
                var timezoneOffset = 0; // The difference between UTC and Israel

                if ((date.getHours() - timezoneOffset) < 10) {
                    hours = "0" + (date.getHours() - timezoneOffset);
                } else hours = (date.getHours() - timezoneOffset);

                if (date.getMinutes() < 10) {
                    minutes = "0" + date.getMinutes();
                } else minutes = date.getMinutes();

                console.log("arr_rides @", i, arr_rides[i].Pat);

                if (arr_rides[i].Pat.DisplayName.includes("אנונימי")) {
                    patDisplayName = "חולה";
                } else {
                    patDisplayName = arr_rides[i].Pat.DisplayName;
                }

                if (arr_rides[i].Pat.EscortedList.length != 0) {
                    patDisplayName += " + " + arr_rides[i].Pat.EscortedList.length;
                }

                date2 = HEBday + " " + day + "/" + month + "/" + date.getUTCFullYear() % 2000;
                time = hours + ":" + minutes;


                if (time == "22:14") { //22:14 is the default time to show afternoon אחה''צ

                    time = " אחה\"צ";
                }



                var Ride = {};

                if (arr_rides[i].Status == "ממתינה לשיבוץ") {
                    drivers = arr_rides[i].Status;
                } else {
                    drivers = arr_rides[i].Drivers[0].DisplayName;
                }

                Ride = {
                    Date: date2,
                    OriginName: arr_rides[i].Origin.Name,
                    DestinationName: arr_rides[i].Destination.Name,
                    Time: time,
                    PatDisplayName: patDisplayName,
                    Coordinator: arr_rides[i].Coordinator.DisplayName,
                    Drivers: drivers,
                    Day: date2.slice(4, 5)
                }


                allRides.push(Ride);



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
                    'print', 'csv', 'excel', 'pdf'
                ],
                createdRow: function (row, data, dataIndex) {
                    if (data.Date.includes("א") || data.Date.includes("ג") || data.Date.includes("ה") || data.Date.includes("ש"))
                        $(row).css('background-color', '#f1f1f1');
                    else $(row).css('background-color', '#ffffff')
                },
                rowCallback: function (row, data, index) {
                    if (data.Drivers == "ממתינה לשיבוץ") {
                        $(row).find('td:eq(6)').css('background-color', '#FBD4B4');
                    }
                }
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