// Purpose: JS code for the reports UI


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

// Current startegy for refreshing the table, set per report type
var S_refresh_preview = null;

let S_HistoryTable = null;


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
    if (window.location.hostname.toString() == 'localhost' || window.location.pathname.toLowerCase().indexOf('test') != -1) {
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

    $.validator.addMethod("smallerThan",
        function (value, element, param) {
            var $otherElement = $(param);
            return parseInt(value, 10) < parseInt($otherElement.val(), 10);
        });
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


// How to refresh the table, per each report type  (Hooked into volunteer selection)
var K_strategy = {
    "rp_vl_ride_month": rp_vl_ride_month__refresh_preview,
    "rp_vl_ride_year": rp_vl_ride_year__refresh_preview,
    "rp_amuta_vls_week": rp_amuta_vls_week__refresh_preview,
    "rp_amuta_vls_per_pat": rp_amuta_vls_per_pat__refresh_preview,
    "rp_amuta_vls_km": rp_amuta_vls_km__refresh_preview,
    "rp_amuta_vls_list": rp_amuta_vls_list__refresh_preview,
    "rp_amuta_vls_per_month": rp_amuta_vls_per_month__refresh_preview,
    "rp_pil_vls_per_month": rp_pil_vls_per_month__refresh_preview,
    "rp_pil_vl_ride_month": rp_pil_vl_ride_month__refresh_preview,
    "rp_pil_vl_ride_recent_period": rp_pil_vl_ride_recent_period__refresh_preview,
    "rp_center_daily_by_month": rp_center_daily_by_month__refresh_preview,
    "rp_center_monthly_by_year": rp_center_monthly_by_year__refresh_preview,
    "rp_center_patients_rides": empty_func,
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
    ],
    "rp_pil_vls_per_month": [
        {
            id: "rp_pil_vls_per_month__year",
            template: 'div[name="template_YTD"]',
            type: "YEAR",
            post_clone: rp_pil_vls_per_month__post_clone
        }
    ],
    "rp_pil_vl_ride_month": [
        {
            id: "rp_vl_ride_month__month",
            template: 'div[name="template_MONTH"]',
            type: "MONTH",
            post_clone: rp_pil_vl_ride_month__post_clone
        }
    ],
    "rp_pil_vl_ride_recent_period": [
        {
            id: "rp_pil_vl_ride_recent_period__fields",
            template: 'div[name="template_RECENT_PERIOD"]',
            type: "RECENT_PERIOD",
            post_clone: rp_pil_vl_ride_recent_period__post_clone
        }
    ],
    "rp_center_daily_by_month": [
        {
            id: "rp_center_daily_by_month__month",
            template: 'div[name="template_MONTH"]',
            type: "MONTH",
            post_clone: rp_center_daily_by_month___post_clone
        }
    ],
    "rp_center_monthly_by_year": [
        {
            id: "rp_center_monthly_by_year__year",
            type: "YEAR",
            template: 'div[name="template_YEAR"]',
            post_clone: rp_center_monthly_by_year__post_clone
        }
    ],
    "rp_center_patients_rides": [
        {
            id: "rp_patients_rides__volunteer",
            template: 'div[name="template_VOLUNTEER"]',
            type: "VOLUNTEER",
            post_clone: field_volunteers_post_clone
        },
        {
            id: "rp_patients_rides__month_start",
            template: 'div[name="template_MONTH_START"]',
            type: "MONTH",
            post_clone: empty_func
        },
        {
            id: "rp_patients_rides__month_end",
            template: 'div[name="template_MONTH_END"]',
            type: "MONTH",
            post_clone: rp_center_patients_rides__post_clone
        },
        {
            id: "rp_patients_rides__hospital",
            template: 'div[name="template_HOSPITAL"]',
            type: "HOSPITAL",
            post_clone: field_hospitals_post_clone
        },
        {
            id: "rp_patients_rides__barrier",
            template: 'div[name="template_BARRIER"]',
            type: "BARRIER",
            post_clone: field_barriers_post_clone
        },
        {
            id: "rp_patients_rides__barrier",
            template: 'div[name="template_GENERATE_REPORT"]',
            type: "GENERATE_REPORT",
            post_clone: field_generate_report_post_clone
        }


    ]


    
 }

/* Handling hebrew in PDF: 
    Instructions for printing PDF Hebrew
  https://github.com/bpampuch/pdfmake/issues/1496
 
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

and do a manual change to it, at the end to regiser as .vfs
*/


var K_DataTable_PDF_EXPORT = {
    extend: 'pdfHtml5',
    text: 'יצוא הדו"ח ל-PDF',
    exportOptions: {
        orthogonal: "exportpdf",
        format: {
            header: function (text, index, node) {
                // https://datatables.net/forums/discussion/48856/is-there-a-way-to-target-table-header-using-export-options-while-exporting
                return reverse_for_hebrew(text);
            }
        }
    },
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
    }
};

// Also, see styling in html file -  buttons-csv.buttons-html5 
var K_DataTable_CSV_EXPORT = {
    extend: 'csv',
    text: 'יצוא הדו"ח ל-CSV',
};


function reverse_for_hebrew(input_str) {
    // https://github.com/bpampuch/pdfmake/issues/184#issuecomment-677909980

    let hebrew_char = input_str.search(/[\u0590-\u05FF]/);
    if (hebrew_char >= 0) {
        let rtl_string = input_str.split("").reverse().join("");
        return rtl_string;
    }
    return input_str;
}


// https://datatables.net/forums/discussion/48267/can-a-column-be-defined-to-render-different-for-view-vs-printing#Comment_127032
function render_cell_in_pdf_rtl(data, type, row) {
    if (type === "exportpdf") {
        return reverse_for_hebrew(data);
    }
    return data;
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
    patients: [],
    hospitals: [],
    barriers: []
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
// called when the async ajax call to load volunteers has finished
// Used to populate UI needing the volunteers list
function populate_volunteer_field() {
    $("#select_driver").autocomplete({
        source: K_CACHE.volunteers,
        select: on_volunteer_selected
        });
}

// on_load_hospitals called when the async ajax call  has finished
// Used to populate UI needing the hospitals list
function loadHospitals(on_load_hospitals) {

    if (K_CACHE.hospitals.length > 1) {
        // One time loading already done.
        on_load_hospitals();
        return;
    }

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportHospitals",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: true,
        success: function (data) {
            var hospitals = data.d;
            hospitals.sort();
            K_CACHE.hospitals = hospitals;
            on_load_hospitals();
        },
        error: function (err) { alert("Error in loadHospitals"); }
    });
}


function on_hospital_selected(event, ui) {
    $("#" + event.target.id).val(ui.item.value);
    return true;
}

function populate_hospital_field() {
     $("#select_hospital").autocomplete({
        source: K_CACHE.hospitals,
        select: on_hospital_selected
    });
    
}


function loadBarriers(on_load_barriers) {

    if (K_CACHE.barriers.length > 1) {
        // One time loading already done.
        on_load_barriers();
        return;
    }

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportBarriers",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: true,
        success: function (data) {
            var barriers = data.d;
            barriers.sort();
            K_CACHE.barriers = barriers;
            on_load_barriers();
        },
        error: function (err) { alert("Error in loadBarriers"); }
    });
}


function on_barrier_selected(event, ui) {
    $("#" + event.target.id).val(ui.item.value);
    return true;
}

function populate_barrier_field() {
    $("#select_barrier").autocomplete({
        source: K_CACHE.barriers,
        select: on_barrier_selected
    });

}

function get_last_month_date() {
    var prev_month = new Date();
    prev_month.setDate(0); // 0 will result in the last day of the previous month
    prev_month.setDate(1); 
    return prev_month;
}


function my_show_month_field(a) {
    console.log("HERE", a);
}
function populate_month_field(is_last_month) {

    var dt = $('#select_month').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        autoclose: true
    });
    if (is_last_month) {
        // compute previous month
        dt.datepicker('setDate', get_last_month_date());
    }
    else {
        // current month
        dt.datepicker('setDate', new Date());
    }
    dt.on("changeDate", refreshPreview);
    // dt.on("show", my_show_month_field);
}

function populate_month_range_fields(start_date, end_date, refresh_callback) {

    var dt_start = $('#select_month_start').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        autoclose: true
    });
    dt_start.datepicker('setDate', start_date);
    dt_start.on("changeDate", refresh_callback);

    var dt_end = $('#select_month_end').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        autoclose: true
    });
    dt_end.datepicker('setDate', end_date);
    dt_end.on("changeDate", refresh_callback);
}


function field_month_post_clone(id) {
    populate_month_field(true);
}

function field_month_current_post_clone(id) {
    populate_month_field(false);
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
    $('#select_date_later').val("2021-01-01");
    $("#select_date_later").change(rp_amuta_vls_list__refresh_preview);

    $("#ck_only_with_rides").change(rp_amuta_vls_list__refresh_preview);

    $("#radio_start_date").change(rp_amuta_vls_list__refresh_preview);
    $("#radio_all").change(rp_amuta_vls_list__refresh_preview);

    $("#commit_to_ni_db").click(rp_amuta_vls_list__commit_to_ni_db);
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

function field_hospitals_post_clone(id) {
    loadHospitals(populate_hospital_field);
}

function field_barriers_post_clone(id) {
    loadBarriers(populate_barrier_field);
}

function field_generate_report_post_clone(id) {
    $("#generate_report_period").click(rp_center_patients_rides__refresh_preview);
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


function convert_year_to_start_end_dates(year_selector) {
    var end_date;
    var selected_year = new Date($(year_selector).val());
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
    return { start_date: start_date, end_date: end_date };
}

function rp_vl_ride_year__refresh_preview() {
    var volunteerId = $("#select_driver").attr("itemID");
    if (volunteerId) {

        obj = convert_year_to_start_end_dates("#select_year");
        
        refreshTable(volunteerId,
                obj.start_date.format("YYYY-MM-DD"),
                obj.end_date.format("YYYY-MM-DD"));
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

function rp_amuta_vls_list__query_object() {

    var selected_date = $('#select_date_later').val();
    var only_with_rides = $("#ck_only_with_rides").is(":checked");
    if ($("#radio_all").is(":checked")) {
        selected_date = "NONE";
    }
    return {
        start_date: selected_date,
        only_with_rides: only_with_rides
    };
}



function rp_amuta_vls_list__refresh_preview() {
    var query_object = rp_amuta_vls_list__query_object();

    refresh_amuta_vls_list_Table(query_object);
}   

function rp_amuta_vls_per_month__refresh_preview() {
//    var selected_date = $('#select_date_later').val();
//    var config = "start_date";

    refresh_amuta_vls_per_month_Table("2019-01-01");
}   


// Checks if all fields are filled. If so refresh the report
function rp_pil_vl_ride_month__refresh_preview() {
    var selected_date = Date.parse($("#select_month").val());
    if (selected_date) {
        var start_month_date = moment(selected_date);
        var end_month_date = start_month_date.clone().add(1, 'months');


        refresh_pil_vl_ride_month_Table(
                start_month_date.format("YYYY-MM-DD"),
                end_month_date.format("YYYY-MM-DD"));
    }
}   


// MDN  https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/parseInt
function filterInt(value) {
    if (/^[-+]?\d+$/.test(value)) {
        return Number(value)
    } else {
        return NaN
    }
}

function rp_pil_vl_ride_recent_period__refresh_preview() {

    $("#div_table_pil_vl_ride_recent_period").hide();

    let is_valid = false;

    let begin_str = $("#input_period_begin").val();
    let begin_num = filterInt(begin_str);
    if (begin_num > 0) {
        let end_str = $("#input_period_end").val();
        let end_num = filterInt(end_str);
        if (end_num > 0 && end_num < begin_num) {
            is_valid = true;
        }
    }

    if (is_valid) {
        $("#generate_report_period").attr("disabled", false);
    }
    else {
        $("#generate_report_period").attr("disabled", true);
    }
}   


function rp_pil_vl_ride_recent_period__generate()
{
    var start_str = $("#input_period_begin").val();
    var end_str = $("#input_period_end").val();
    refresh_pil_vl_ride_recent_period_Table(start_str, end_str);

}


function rp_pil_vls_per_month__post_clone(id)
{
    $("#select_year_ytd").change(rp_pil_vls_per_month__refresh_preview);
    rp_pil_vls_per_month__refresh_preview();
}


function rp_pil_vl_ride_month__post_clone(id) {
    populate_month_field(true);
    rp_pil_vl_ride_month__refresh_preview();
}


function rp_pil_vl_ride_recent_period__post_clone(id) {
    $("#input_period_begin").keyup(rp_pil_vl_ride_recent_period__refresh_preview);
    $("#input_period_end").keyup(rp_pil_vl_ride_recent_period__refresh_preview);
    $("#generate_report_period").click(rp_pil_vl_ride_recent_period__generate);
    rp_pil_vl_ride_recent_period__refresh_preview();
}


function rp_center_daily_by_month___post_clone(id) {
    populate_month_field(false);
    rp_center_daily_by_month__refresh_preview();
}

function rp_center_monthly_by_year__post_clone(id) {
    var today = new Date();
    $('#select_year').val(today.getFullYear());
    $("#select_year").change(rp_center_monthly_by_year__refresh_preview);

    rp_center_monthly_by_year__refresh_preview();
}

function rp_center_patients_rides__post_clone(id) {

    // range - last 12 month.
    let start_date = new Date();
    start_date.setFullYear(start_date.getFullYear() - 1);
    let end_date = new Date();
    
    populate_month_range_fields(start_date, end_date, empty_func);
}


function rp_pil_vls_per_month__refresh_preview() {
    var start_date, end_date;
    var selected_year = $('#select_year_ytd').val();

    if (selected_year.startsWith("20")) {
        start_date = selected_year + "-01-01";
        var this_year = new Date().getFullYear();
        if (this_year == selected_year) {
            end_date = moment().format('YYYY-MM-DD');
        }
        else {
            end_date = selected_year + "-12-31";
        }
    }
    else {
        start_date = moment().subtract(1, 'years').format('YYYY-MM-DD');
        end_date = moment().format('YYYY-MM-DD');
    }


    refresh_pil_vls_per_month_Table(start_date, end_date);
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
                "language": {
                    "search": "חיפוש:"
                },

                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    {
                        data: "Region",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Volunteer",
                        render: render_cell_in_pdf_rtl
                    }

                ],
                dom: 'Bfrtip',
                buttons: [
                    K_DataTable_PDF_EXPORT
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
            records = JSON.parse(data.d);

            $('#div_table_amuta_vls_km').show();
            tbl = $('#table_amuta_vls_km').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: records,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    {
                        data: "Date",
                        render: function (data, type, row) {
                            if (type == "display") {
                                return build_date_with_dow_string(data);
                            }
                            return data;
                        }
                    },
                    {
                        data: "Volunteer",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Patient",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Origin",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Destination",
                        render: render_cell_in_pdf_rtl
                    }

                ],
                dom: 'Bfrtip',
                buttons: [
                    K_DataTable_PDF_EXPORT,
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
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
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Year" },
                    { data: "Month" },
                    { data: "Count" }

                ],
                dom: 'Bfrtip',
 

                buttons: [
                    K_DataTable_CSV_EXPORT
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
function refresh_pil_vls_per_month_Table(start_date, end_date) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportSliceVolunteerPerMonth",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            arr_rides = data.d;

            $('#div_table_pil_vls_per_month').show();
            tbl = $('#table_pil_vls_per_month').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "DisplayName" },
                    { data: "City" },
                    { data: "CellPhone" },
                    { data: "JoinDate" },
                    { data: "Jan" },
                    { data: "Feb" },
                    { data: "Mar" },
                    { data: "Apr" },
                    { data: "May" },
                    { data: "Jun" },
                    { data: "Jul" },
                    { data: "Aug" },
                    { data: "Sep" },
                    { data: "Oct" },
                    { data: "Nov" },
                    { data: "Dec" }

                ],
                dom: 'Bfrtip',


                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
        }


    });

}

function refresh_pil_vl_ride_month_Table(start_date, end_date) {
    hide_all_tables();
    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportSliceVolunteersCountInMonth",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            var records = data.d;

            $('#div_table_pil_vl_ride_month').show();
            tbl = $('#table_pil_vl_ride_month').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: records,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Volunteer" },
                    { data: "Count" },
                ],
                dom: 'Bfrtip',

                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
        }

    });

}


function get_buttons_pil_vl_ride_recent_period() {
    let showDocumentedRidesAllBtn = '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5 showDocumentedRidesAllBtn" title="תיעוד הסעות" data-toggle="modal" data-target="#documentedRidesModal"><i class="fa fa-car" aria-hidden="true"></i></button>';
    let showDocumentedRidesInPeriodBtn = '<button type="button" class="btn btn-icon waves-effect waves-light btn-primary btn-sm m-b-5 showDocumentedRidesInPeriodBtn" title="תיעוד הסעות בתקופה" style = "margin-right: 0.5em" data-toggle="modal" data-target="#documentedRidesModal">';
    showDocumentedRidesInPeriodBtn += '<div class="fa-stack" style="width:1em; height:1em;line-height:1em;"> <i class="fa fa-car"></i>';
    showDocumentedRidesInPeriodBtn += '<i class="fa fa-filter" style="position:absolute;right:-1em;font-size: 0.7em;color:yellow"></i></div > ';
    showDocumentedRidesInPeriodBtn += '</button > ';
    return '<div>' + showDocumentedRidesAllBtn + showDocumentedRidesInPeriodBtn + '</div>';
}

function refresh_pil_vl_ride_recent_period_Table(start_number, end_number) {
    hide_all_tables();

    let csv_export_button = {
        exportOptions: {
            columns: [0, 1,2 ] // do not export the Buttons column
        }
    };

    Object.assign(csv_export_button, K_DataTable_CSV_EXPORT);  // Copy fields from reusable button

    $('#wait').show();
    var query_object = {
        start_number: start_number,
        end_number: end_number
    };



    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportSliceVolunteersInPeriod",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            var records = data.d;

            let buttons_html = get_buttons_pil_vl_ride_recent_period();
            // Add buttons to the table
            for (a_rec of records) {
                a_rec.Buttons = buttons_html;
            }

            $('#div_table_pil_vl_ride_recent_period').show();
            tbl = $('#table_pil_vl_ride_recent_period').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: records,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    { data: "Volunteer" },
                    { data: "CityCityName" },
                    { data: "CellPhone" },
                    { data: "Buttons" }
                ],
                dom: 'Bfrtip',
                buttons: [ 
                    csv_export_button
                ]
            });
            register_table_button_events();
        },
        error: function (err) {
            $('#wait').hide();
        }

    });

}

function filter_by_period(data, start_date, end_date) {
    if (start_date == null || end_date == null) {
        return data;
    }

    let result = data.filter(a_rec => { let this_date = moment(ConvertDBDate2UIFullStempDate(a_rec.Date)); return this_date.isBetween(start_date, end_date); } );
    return result;
}

function show_rides_history(element, start_date, end_date) {

    $('#wait').show();

    let rowData = tbl.row(element.parents('tr')).data();
    let title_str = "תיעוד הסעות " + rowData.Volunteer + " ";
    if (start_date != null) {
        title_str += "בתקופה  ";
    }
    $('#documentedRidesTitle').text(title_str);


    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetVolunteersDocumentedRides",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ volunteerId: rowData.Id }),
        success: function (data) {
            orig_data = JSON.parse(data.d)
            data = filter_by_period(orig_data, start_date, end_date);


            if (S_HistoryTable != null) {
                S_HistoryTable.destroy();
            }

            let now = new Date();
            let earliyestFuture = { date: new Date(2100, 11, 31), row: null };

            S_HistoryTable = $('#documentedRidesTable').DataTable({
                order: [[5, "desc"]],
                pageLength: 10,
                data: data,
                columns: [
                    {
                        data: (data) => {
                            // if (data.Date === undefined) return;
                            return ConvertDBDate2UIDate(data.Date);
                        }
                    },
                    {
                        data: (data) => {
                            if (data.Date === undefined) return;

                            let fullTimeStempStr = data.Date;
                            let startTrim = fullTimeStempStr.indexOf('(') + 1;
                            let endTrim = fullTimeStempStr.indexOf(')');
                            let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
                            let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));

                            if (fullTimeStemp.getMinutes() === 14) {
                                if (fullTimeStemp.getHours() === 19 || fullTimeStemp.getHours() === 20 || fullTimeStemp.getHours() === 21 || fullTimeStemp.getHours() === 22) {
                                    return 'אחה"צ';
                                }
                            }

                            let hh = fullTimeStemp.getHours() < 10 ? "0" + fullTimeStemp.getHours() : fullTimeStemp.getHours();
                            hh += ":";
                            let mm = fullTimeStemp.getMinutes() < 10 ? "0" + fullTimeStemp.getMinutes() : fullTimeStemp.getMinutes();
                            return hh + mm;
                        }
                    },
                    {
                        data: (data) => {
                            if (data.Origin === undefined || data.Destination === undefined) return;

                            let fullPath = data.Origin.Name + " ← " + data.Destination.Name;
                            return fullPath;
                        }
                    },
                    { data: "Pat.DisplayName" },
                    { data: "Remark" },
                    {
                        data: (data) => {
                            return ConvertDBDate2UIFullStempDate(data.Date);
                        }
                    },

                ],
                columnDefs: [
                    { "targets": [0], type: 'de_date' },
                    /*
                     Amir wanted a seperated columns to date and time but still sort by the full time stemps (or the acctual ticks)
                     so my (Yogev) soolution was to
                     1. fetch the fulltime stemp from the back-end
                     2. render and sort by this column
                     3. not showing it to the user
                      ↓*/
                    { "targets": [5], visible: false },
                    { "targets": 0, width: "10%" },
                    { "targets": 1, width: "10%" },
                    { "targets": 2, width: "20%" },
                    { "targets": 3, width: "25%" },
                    { "targets": 4, width: "35%" },
                ],
                createdRow: function (row, data, index) {
                    let the_date = ConvertDBDate2UIFullStempDate(data.Date);

                    if (the_date > now) {
                        $(row).addClass('futureRide'); 

                        // We want to apply special border to the last future ride - i.e the earliest
                        // earliyestFuture are used to record teh earliest date we observe, and it's row
                        if (the_date < earliyestFuture.date) {
                            earliyestFuture.date = the_date;
                            earliyestFuture.row = $(row);
                        }
                    }
                    else {
                        // We are done with all future rides, tag the earliest and forget it
                        if (earliyestFuture.row) {
                            earliyestFuture.row.addClass('earliestFuture');
                            earliyestFuture.row = null;
                        }
                    }
                }

            });
            $('#wait').hide();

        },
        error: function (err) {
            alert("Error in GetVolunteersRideHistory: " + err.responseText);
            $('#wait').hide();
        }
    });


}


// Copied from viewVolunteer.html:  buttonsEvents 
function register_table_button_events() {

    $(".showDocumentedRidesAllBtn").click(function () {
        show_rides_history($(this), null, null);
    });


    $(".showDocumentedRidesInPeriodBtn").click(function () {
        var start_str = $("#input_period_begin").val();
        var end_str = $("#input_period_end").val();
        let start_date = moment().subtract(+start_str, 'd');
        let end_date = moment().subtract(+end_str, 'd');

        show_rides_history($(this), start_date, end_date);
    });



}

// Copied from viewVolunteer.html
// The iput timestamp is  "/Date(1616560200000)/"
const ConvertDBDate2UIDate = (fullTimeStempStr) => {
    if (fullTimeStempStr === undefined) return;
    let startTrim = fullTimeStempStr.indexOf('(') + 1;
    let endTrim = fullTimeStempStr.indexOf(')');
    let fullTimeStempNumber = fullTimeStempStr.substring(startTrim, endTrim);
    let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));

    //Note: in getMOnth function 0=January, 1=February etc...

    let dd = fullTimeStemp.getDate();

    let mm = fullTimeStemp.getMonth() + 1;

    let yyyy = fullTimeStemp.getFullYear();

    return `${dd}.${mm}.${yyyy}`;
}

// Copied from viewVolunteer.html
const ConvertDBDate2UIFullStempDate = (dateAsStr) => {
    if (dateAsStr === undefined) return;
    let startTrim = dateAsStr.indexOf('(') + 1;
    let endTrim = dateAsStr.indexOf(')');
    let fullTimeStempNumber = dateAsStr.substring(startTrim, endTrim);
    let fullTimeStemp = new Date(parseInt(fullTimeStempNumber));
    return fullTimeStemp;
}




function refresh_amuta_vls_list_Table(query_object) {
    hide_all_tables();
    $('#wait').show();

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
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [

                    { data: "FirstNameH" },
                    { data: "LastNameH" },
                    { data: "VolunteerIdentity" },
                    { data: "Email" },
                    { data: "Address" },
                    { data: "CityCityName" },
                    { data: "JoinDate" },
                    { data: "CellPhone"}
                ],
                dom: 'Bfrtip',


                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}




// Change Date field in every object, to have a string & index
function update_date_field_for_sort(arr) {
    for (let entry of arr) {
        entry.Date = {
            str: build_date_with_dow_string(entry.Date),
            timestamp : new moment(entry.Date, "DD/MM/YYYY", true).valueOf()
        }
    }
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

            update_date_field_for_sort(arr_rides);

            $('#div_table_amuta_vls_per_pat').show();
            tbl = $('#table_amuta_vls_per_pat').DataTable({
                pageLength: 500,
                bLengthChange: false,
                data: arr_rides,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 1], "targets": 0 }],
                columns: [
                    {
                        data: "Date",
                        render: function (data, type, row) {
                            if (type == "sort") {
                                return (data.timestamp);
                            }
                            return data.str;
                        }
                    },
                    {
                        data: "Volunteer",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Origin",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "Destination",
                        render: render_cell_in_pdf_rtl
                    }

                ],
                dom: 'Bfrtip',
                buttons: [
                    K_DataTable_PDF_EXPORT,
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

}

function build_date_with_dow_string(in_date) {
    var date = moment(in_date, "DD/MM/YYYY", true);
    var HEBday = getDayString(date.day());

    var result = HEBday + " " + date.format("DD/MM/YY");
    return result;
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
                date2 = { str: HEBday + " " + date.format("DD/MM/YY"), timestamp: date.valueOf()};
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
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0, 3], "targets": 0 }],
                columns: [
                    {
                        data: "Date",
                        render: function (data, type, row) {
                            if (type == "sort") {
                                return (data.timestamp);
                            }
                            return data.str;
                        }
                    },
                    { data: "Time" },
                    {
                        data: "OriginName",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "DestinationName",
                        render: render_cell_in_pdf_rtl
                    },
                    {
                        data: "PatDisplayName",
                        render: render_cell_in_pdf_rtl
                    },

                ],
                dom: 'Bfrtip',
                buttons: [
                    K_DataTable_PDF_EXPORT,
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
            // @@ alert("Error in GetRidePatView: " + err.responseText);
        }


    });

 }

function rp_amuta_vls_list__commit_to_ni_db() {

    var msg = ".זהירות, פעולה זו תמנע הצגת מתנדבים אלו בדוחות הבאים"
    if (!window.confirm(msg)) {
        return;
    }

    var query_object = rp_amuta_vls_list__query_object();
    $('#wait').show();

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/CommitReportedVolunteerListToNI_DB",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
        },
        error: function (err) {
            $('#wait').hide();
            alert("Error in rp_amuta_vls_list__commit_to_ni_db: " + err.responseText);
        }

    });

}

// Checks if all fields are filled. If so refresh the report
function rp_center_daily_by_month__refresh_preview() {
    var selected_date = Date.parse($("#select_month").val());
    if (selected_date) {
        var start_month_date = moment(selected_date);
        var end_month_date = start_month_date.clone().add(1, 'months');

        rp_center_daily_by_month__refresh_Table(
            start_month_date.format("YYYY-MM-DD"),
            end_month_date.format("YYYY-MM-DD"));
    }
}   



function rp_center_daily_by_month__refresh_Table(start_date, end_date) {
    hide_all_tables();

    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };



    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportCenterDailybyMonth",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            var records = data.d;
            records = rp_center_daily_by_month__fix_records(records, start_date);

            $('#div_table_center_daily_by_month').show();
            tbl = $('#table_center_daily_by_month').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: records,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0], "type": "num", "targets": 0 }],
                columns: [
                    {
                        data: "Date",
                        render: function (data, type, row) {
                            if (type == "sort") {
                                return data.Index;
                            }
                            return data.Str;
                        }

                    },
                    { data: "VolunteerCount" },
                    { data: "PatientCount" }
                ],
                dom: 'Bfrtip',

                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
        }

    });

}

function rp_center_daily_by_month__fix_records(records, start_date_str) {
    // Change date format, inject empty rows, add total row

    daysArr = { 0: "יום א", 1: "יום ב", 2: "יום ג", 3: "יום ד", 4: "יום ה", 5: "יום ו", 6: "יום שבת", };

    // First, we create an array with every entry being 0 count
    let curr_date = moment(start_date_str, "YYYY-MM-DD");
    let days_in_month = curr_date.daysInMonth()
    let result = new Array();
    for (i = 0; i < days_in_month; i++) {
        let new_date_str = `${daysArr[curr_date.day()]} - ${i + 1}`;
        result.push({ Date: { Str: new_date_str, Index: i}, VolunteerCount: 0, PatientCount: 0 });
        curr_date = curr_date.add(1, 'days');
    }

    let total_vols = 0, total_pats = 0;

    for (a_rec of records) {
        let d = moment(a_rec.Date.split(" ")[0], "DD/MM/YYYY");
        let day_in_month = d.date();
        let new_date_str = `${daysArr[d.day()]} - ${day_in_month}`;
        a_rec.Date = { Str: new_date_str, Index: day_in_month - 1 };

        result[day_in_month - 1] = a_rec; //override actual value in placeholder
        total_vols = total_vols + +a_rec.VolunteerCount;
        total_pats = total_pats + +a_rec.PatientCount;
    }

    result.push({ Date: { Str: 'סה"כ', Index: 1001 }, VolunteerCount: total_vols, PatientCount: total_pats });

    return result;
}

function rp_center_monthly_by_year__refresh_preview() {

    obj = convert_year_to_start_end_dates("#select_year");

    rp_center_monthly_by_year__refresh_table(
        obj.start_date.format("YYYY-MM-DD"),
        obj.end_date.format("YYYY-MM-DD"));
}

function rp_center_monthly_by_year__refresh_table(start_date, end_date) {
    hide_all_tables();

    $('#wait').show();
    var query_object = {
        start_date: start_date,
        end_date: end_date
    };



    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportCenterMonthlyByYear",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            var records = data.d;

            records = rp_center_monthly_by_year__fix_records(records);

            $('#div_table_rp_center_monthly_by_year').show();
            tbl = $('#table_rp_center_monthly_by_year').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: records,
                destroy: true,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0], "type": "num", "targets": 0 }],
                columns: [
                    {
                        data: "People"
                    },
                    { data: "1", "defaultContent": ""  },
                    { data: "2", "defaultContent": ""  },
                    { data: "3", "defaultContent": ""  },
                    { data: "4", "defaultContent": ""  },
                    { data: "5", "defaultContent": "" },
                    { data: "6", "defaultContent": "" },
                    { data: "7", "defaultContent": ""  },
                    { data: "8", "defaultContent": ""  },
                    { data: "9", "defaultContent": ""  },
                    { data: "10", "defaultContent": ""  },
                    { data: "11", "defaultContent": ""  },
                    { data: "12", "defaultContent": ""  },
                    { data: "Total" },
                ],
                dom: 'Bfrtip',

                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
        }

    });


}

function rp_center_monthly_by_year__fix_records(records) {
    let drivers = { "People": "מתנדבים", Total: 0 };
    let patients = { "People": "חולים", Total: 0 };
    
    for (a_rec of records) {
        let obj = patients;
        if (a_rec.Type == "DRIVER") {
            obj = drivers;
        }
        obj[a_rec.Month] = a_rec.Count;
        obj.Total += +a_rec.Count;
    }
    return new Array(patients, drivers);
}


// Checks if all fields are filled. If so refresh the report
function rp_center_patients_rides__refresh_preview() {
    var volunteerId = $("#select_driver").attr("itemID");
    if (!volunteerId) {
        volunteerId = "*";
    }
    var hospital = $("#select_hospital").val();
    if (!hospital) {
        hospital = "*";
    }
    var barrier = $("#select_barrier").val();
    if (!barrier) {
        barrier = "*";
    }

    var start_date = Date.parse($("#select_month_start").val());
    if (start_date) {
        let end_date = Date.parse($("#select_month_end").val());
        let start_moment = moment(start_date);
        let end_moment = moment(end_date);
        rp_center_patients_rides__refresh_Table(volunteerId, 
            start_moment.format("YYYY-MM-DD"),
            end_moment.format("YYYY-MM-DD"),
            hospital, barrier
        );
    }
}

function rp_center_patients_rides__fix_records(arr) {
    for (let entry of arr) {
        let orig_str = entry.Month;
        entry.Month = {
            str: orig_str,
            timestamp: new moment(orig_str, "DD/MM/YYYY", true).valueOf()
        }
    }
}

function rp_center_patients_rides__footer_row(row, data, start, end, display) {
  //    console.log("footerCallback", row, start, end, display);
    var api = this.api();
    // Total over this page
    pageTotal = api
        .column(3, { page: 'current' })
        .data()
        .reduce(function (a, b) {
            return +a + +b;
        }, 0);
    $("#center_patients_rides_footer_total").html(pageTotal);
}


function rp_center_patients_rides__refresh_Table(volunteerId, start_date, end_date, hospital, barrier) {
    hide_all_tables();

    $('#wait').show();
    var query_object = {
        volunteer: volunteerId,
        start_date: start_date,
        end_date: end_date,
        hospital: hospital,
        barrier: barrier
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportCenterPatientsRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            $('#wait').hide();
            var records = data.d;

            rp_center_patients_rides__fix_records(records);

            $('#div_table_center_patients_rides').show();
            tbl = $('#table_center_patients_rides').DataTable({
                pageLength: 100,
                bLengthChange: false,
                data: records,
                destroy: true,
                "footerCallback": rp_center_patients_rides__footer_row,
                "language": {
                    "search": "חיפוש:"
                },
                columnDefs: [
                    { "orderData": [0], "type": "num", "targets": 0 }],
                columns: [
                    {
                        data: "Month",
                        render: function (data, type, row) {
                            if (type == "sort") {
                                return (data.timestamp);
                            }
                            return data.str;
                        }
                    },
                    { data: "Volunteer" },
                    { data: "Origin" },
                    { data: "Destination" },
                    { data: "Hospital" },
                    { data: "Barrier" },
                    { data: "Count" },
                ],
                dom: 'Bfrtip',

                buttons: [
                    K_DataTable_CSV_EXPORT
                ]
            });
        },
        error: function (err) {
            $('#wait').hide();
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
    $("#div_table_pil_vls_per_month").hide();
    $("#div_table_pil_vl_ride_month").hide();
    $("#div_table_pil_vl_ride_recent_period").hide();
    $("#div_table_center_daily_by_month").hide();
    $("#div_table_rp_center_monthly_by_year").hide();
    $("#div_table_center_patients_rides").hide();
 }



/* Good code not used for now:
 
 Validation in Bootstrap:

     let v =  $("#params_ph").validate({
        // Specify validation rules
        rules: {
            input_period_begin: {
                required: true,
                digits: true,
                min: 1
            },
            input_period_end: {
                required: true,
                digits: true,
                min: 1,
                smallerThan: "#input_period_begin"
            }
        },
        // Specify validation error messages
        messages: {
            input_period_begin: "",
            input_period_end: {
                required: "הכנס מספר חיובי",
                smallerThan: "הכנס מספר קטן מהמספר שלמעלה",
            }
        }
    });

    if (v.form()) {
        $("#generate_report_period").attr("disabled", false);
    }
    else {
        $("#generate_report_period").attr("disabled", true);
    }

*/
