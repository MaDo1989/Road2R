// Purpose: Dashboard UI for Amuta


window.ConfigFlags = {
    is_debugging_dsb: false,
    full_loading: true,
    all_graphs: true,
    load_rows: true,
};

window.DebugFlags = {
    is_debugging_dsb: true,
    full_loading: false,
    all_graphs: false,
    load_rows: true,
};

// Set this (and tweak DebugFlags) to get only specific web-requests done
// window.ConfigFlags = window.DebugFlags;  

// a mapping of event names to the next code that should be called
// used to avoid flooding the server with dozens of queries at startup.
// determines order of what gets populated first. -  TODAY & Daily.
// search for event names in code to see usage.
let K_chained_loading_table = {}; 

function set_chain_cb_for_event(event_name, cb, data) {
    K_chained_loading_table[event_name] = { callback: cb, user_data: data };
}

function call_next_in_chain(event_name) {
    if (event_name in K_chained_loading_table) {
        entry = K_chained_loading_table[event_name];

        // remove it so will not be called again when section combo is updated
        delete K_chained_loading_table[event_name];
        entry.callback(entry.user_data);
    }
}

const CHART_COLORS = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};


function dashboard_hl_init() {
    $("#reports_content_div").hide();
    $("#dsb_hl_content_div").show();
    if (window.ConfigFlags.full_loading) {
        set_chain_cb_for_event("on_daily_finished", start_yearly_cards, null);
        set_chain_cb_for_event("on_weekly_finished", start_monthly_cards_this_month, null);

        start_daily_cards();
        start_weekly_cards();
    }
    else {
        // For fast debugging
        // start_daily_cards();
        // start_weekly_cards();
        // start_monthly_cards_this_month();
        start_all_months_rows(moment());

        // start_month_graph(moment(), get_month_card("curr_and_prev"));
        // setup_monthly_choose_combo();

        // start_all_years_rows();
    }
}

function start_monthly_cards_this_month() {
    start_monthly_cards(moment());
}
 
function on_daily_date_change() {
    let selected_day = moment($("#dsb_select_day").val(), K_DateFormat_Moment);
    let today = moment();
    if (selected_day.isAfter(today)) {
        selected_day = today;
    }

    $(".dsb_daily_num").text("--");  // reset all the weekly number fields.

    start_current_day_row(selected_day);
}

function setup_daily_choose_combo() {
    var dt = $('#dsb_select_day').datepicker({
        format: K_DateFormat_DatePicker,
        autoclose: true
    });
    let today = new Date();
    dt.datepicker('setDate', today);
    dt.on("changeDate", on_daily_date_change);
}

function start_daily_cards() {
    start_current_day_row(new Date());
    setup_daily_choose_combo();

}

// Initiate async ajax call. When call finishes, invoke card's on_data callback
function start_current_day_row(the_date) {

    $("#dsb_hl_daily_rides_todays_date").text(moment(the_date).format('DD.MM'));

    var query_object = {
        start_date: moment(the_date).format('YYYY-MM-DD'),
        end_date: moment(the_date).add(1, 'days').format('YYYY-MM-DD')
    }

    start_current_daily_totals(query_object);

    start_current_daily_need_drivers(query_object);
}

function start_current_daily_totals(query_object) {
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportDailyDigestMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d[0];
            // Update card numeric value
            $("#dsb_hl_daily_rides_total").text(result.Rides);
            $("#dsb_hl_daily_patients_total").text(result.Patients);
            $("#dsb_hl_daily_volunteers_total").text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}

function start_current_daily_need_drivers(query_object) {
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportRangeNeedDriversMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            // Update card numeric value
            $("#dsb_hl_daily_rides_attention").text(result.Value1);
            $("#dsb_hl_daily_patients_attention").text(result.Value2);

            start_current_daily_new_volunteers(query_object);
        },
        error: function (err) {
        }
    });
}



function start_current_daily_new_volunteers(query_object) {
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportNewDriversInRange",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            call_next_in_chain("on_daily_finished");
            result = data.d;
            $("#dsb_hl_daily_volunteers_attention").text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}




const daily_card_definitions = [

    {
        name: "daily_rides",
        title: "הסעות",
        div_id: "dsb_hl_daily_rides"
    },
    {
        name: "daily_patients",
        title: "חולים",
        div_id: "dsb_hl_daily_patients"
    },
    {
        name: "daily_volunteers",
        title: "מתנדבים",
        div_id: "dsb_hl_daily_volunteers"
    }
];

 

/*  ==================   WEEK  related code     =========================== */

function start_weekly_cards() {
    if (window.ConfigFlags.full_loading) {
        start_all_weeks_rows(moment());

        start_week_all_graphs(moment());

        start_one_week_new_volunteers(get_week_card("curr"), moment());
    }
    else {
        // For debugging 
        // start_week_all_graphs(moment());
        start_all_weeks_rows(moment());
        //start_one_week_new_volunteers(get_week_card("curr"), moment());

    }

    setup_weekly_choose_combo();
    $("#dsb_hl_weekly_tbl_curr_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_weekly_tbl_prev_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_weekly_tbl_2wks_ago_show").click(toggle_month_week_graph_datasets);
}


const week_card_definitions = [
    {
        designator: "curr",
        next: "prev"
    },
    {
        designator: "prev",
        borderDash: [8, 8],
        next: "2wks_ago"
    },
    {
        designator: "all_3weeks",
        borderDash: [8, 8],
        next: null
    },
    {
        designator: "2wks_ago",
        borderDash: [10, 4],
        next: null
    }
];

function get_week_card(dsg) {
    for (const card_def of week_card_definitions) {
        if (card_def.designator.localeCompare(dsg) == 0) {
            return card_def;
        }
    }
    return null;
}

function get_week_name_in_hebrew(week_designator) {
    if (week_designator.localeCompare("prev") == 0) {
        return "השבוע הקודם";
    }
    if (week_designator.localeCompare("2wks_ago") == 0) {
        return "השבוע שלפניו";
    }

    return "השבוע";
}

// move ahead the date to the Saturday of that week.  
// May return a day that in the future
function bump_week_end_range(end_date) {
    if (end_date.weekday() == 0) {
        end_date.add(6, 'days');  // Sunday + 6 days = Saturday
    }
    else {
        end_date.endOf('isoWeek').subtract(1, 'days');
    }
}

function dbg_validate_bump_week_end_range() {
    let k = moment();
    for (let idx = 0; idx < 16; ++idx) {
        let d = moment(k);
        bump_week_end_range(d);
        console.log(moment(k).format(K_DateFormat_Debug_Moment), "==>", d.format(K_DateFormat_Debug_Moment));
        k.subtract(1, 'days');
    }
}

function get_all_weeks_range(end_date) {
    bump_week_end_range(end_date);
    end_date.add(1, 'days'); // This is needed for DB query <= @end_date
    let start_date = moment(end_date).subtract(21, 'days');
    let result = {
        start_date: start_date.format("YYYY-MM-DD"),
        end_date: end_date.format("YYYY-MM-DD"),
        prev_start: "NA",
        prev_end: "NA",
        span: "WEEK"
    }
    return result;
}

// Soon to be obsolete ...
function get_week_range(week_designator, end_date) {
    bump_week_end_range(end_date);
    end_date.add(1, 'days'); // This is needed for DB query <= @end_date
    let start_date = moment(end_date);

    if (week_designator.localeCompare("curr") == 0) {
        start_date.subtract(7, 'days');
    }
    if (week_designator.localeCompare("prev") == 0) {
        start_date.subtract(14, 'days');
        end_date.subtract(7, 'days');
    }
    if (week_designator.localeCompare("all_3weeks") == 0) {
        start_date.subtract(21, 'days');
    }

    if (week_designator.localeCompare("2wks_ago") == 0) {
        start_date.subtract(21, 'days');
        end_date.subtract(14, 'days');
    }

    let result = {
        start_date: start_date.format("YYYY-MM-DD"),
        end_date: end_date.format("YYYY-MM-DD"),
        prev_start: "NA",
        prev_end: "NA",
        span:   "WEEK"
    }
    return result;
}


function on_weekly_date_change() {
    let selected_day = moment($("#dsb_select_week").val(), K_DateFormat_Moment);
    // make sure it ends on Saturday,
    bump_week_end_range(selected_day);
    // unless in current week
    let today = moment();
    if (selected_day.isAfter(today)) {
        selected_day = today;
    }

    reset_graph("dsb_hl_weekly_graph");
    $(".dsb_weekly_num").text("--");  // reset all the weekly number fields.

    start_week_all_graphs(selected_day);
    start_all_weeks_rows(moment(selected_day));
    set_chain_cb_for_event("on_weekly_rows_finished", delayed_start_week_new_volunteers, selected_day);

}

function delayed_start_week_new_volunteers(selected_day) {
    console.log("delayed_start_week_new_volunteers", selected_day);
    start_one_week_new_volunteers(get_week_card("curr"), moment(selected_day));
}
function setup_weekly_choose_combo() {
    var dt = $('#dsb_select_week').datepicker({
        format: K_DateFormat_DatePicker,
        autoclose: true
    });
    let today = new Date();
    dt.datepicker('setDate', today);
    dt.on("changeDate", on_weekly_date_change);
}

// To improve performance, we query the entire range of last 3 weeks in one shot
function start_week_all_graphs(end_date) {
    let start_date = moment(end_date);
    // The week-range in the graph always start on Sunday
    start_date.startOf('isoWeek'); // Avoid L10N problems - ISO says week start on Monday.
    start_date.subtract(15, 'days'); // two weeks back + 1 day from Mon-->Sun

    let query_object = {
        start_date: start_date.format("YYYY-MM-DD"),
        end_date: end_date.format("YYYY-MM-DD")
    }
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportWeeklyGraphMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            render_week_all_graphs(result, query_object);
        },
        error: function (err) {
        }


    });

}



function start_all_weeks_rows(end_date) {
    update_day_rows_names();
    // Invok Async call, to get info for this row.

    var query_object = get_all_weeks_range(moment(end_date));

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportWithPeriodDigestMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            render_weeks_rows(result);
            call_next_in_chain("on_weekly_rows_finished");
        },
        error: function (err) {
        }
    });
}

function render_weeks_rows(result) {
    console.log("render_weeks_rows", result);
    render_on_week_row("2wks_ago", result[0]);
    render_on_week_row("prev", result[1]);
    render_on_week_row("curr", result[2]);
}

function render_on_week_row(dsg, entry) {
    let label_id = "#dsb_hl_weekly_tbl_" + dsg + "_pats";
    $(label_id).text(entry.Patients);
    label_id = "#dsb_hl_weekly_tbl_" + dsg + "_rides";
    $(label_id).text(entry.Rides);
    label_id = "#dsb_hl_weekly_tbl_" + dsg + "_vols";
    $(label_id).text(entry.Volunteers);
}

function update_day_rows_names() {
    update_one_day_row_name("curr");
    update_one_day_row_name("prev");
    update_one_day_row_name("2wks_ago");
}

function update_one_day_row_name(dsg) {
    let label_id = "#dsb_hl_weekly_tbl_" + dsg + "_week_name";
    let week_name = get_week_name_in_hebrew(dsg);
    $(label_id).text(week_name);
}


function start_one_week_new_volunteers(card_def, end_date) {
    let dsg = card_def.designator;
    var query_object = get_week_range(dsg, moment(end_date));

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportNewDriversInRange",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            // Schedule fetch for next data-set if needed.
            let next_card = get_week_card(card_def.next);
            if (next_card) {
                start_one_week_new_volunteers(next_card, moment(end_date));   // Not really recursive - called from incoming-data callback
            }
            else {
                call_next_in_chain("on_weekly_finished");
            }

            result = data.d;
            label_id = "#dsb_hl_weekly_tbl_" + dsg + "_new";
            $(label_id).text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}

function prepare_one_week_span_data(start_date, dict) {
    let prepared_data = {
        labels: [],
        rides: [],
        volunteers: [],
        patients: []
    }

    let the_date = moment(start_date); // avoid mutating input

    for (i = 0; i < 7; ++i) {
        let hebrew_day_name = the_date.toDate().toLocaleString("he", { weekday: 'long' });
        prepared_data.labels.push(hebrew_day_name);

        // check if we have info on this specific day:
        let ddmm = the_date.format("YYYY-MM-DD");
        if (ddmm in dict) {
            let dayinfo = dict[ddmm];
            prepared_data.rides.push(dayinfo.Rides);
            prepared_data.volunteers.push(dayinfo.Volunteers);
            prepared_data.patients.push(dayinfo.Patients);
        }
        else {
            prepared_data.rides.push(0);
            prepared_data.volunteers.push(0);
            prepared_data.patients.push(0);
        }
        the_date.add(1, "days");
    }

    return prepared_data;
}

function dbg_compare_scalar_arrays(lhs, rhs) {
    if (lhs.length === rhs.length &&
        lhs.every(function (value, index) { return value === rhs[index] })) {
        return true;
    }
    console.error("dbg_compare_scalar_arrays() - arrays are different", lhs, rhs);
    return false;
}


function dbg_validate_weekly_graph(myChart, server_data) {
    // confirm we display what we expect
    // Scan Graph datasets from end, and for each patients dataset, add it to test array
    let patients_arr = new Array();
    for (let idx of [7, 4, 1]) {   
        let dataset = myChart.data.datasets[idx];
        if (dataset.label.localeCompare("חולים") == 0) {
            for (elem of dataset.data) {
                if (elem) { // If not a zero value
                    patients_arr.push(elem);
                }
            }
        }
    }

    // confirm now that the "column" of Patients in server_data is the same
    let server_patients = server_data.map(
        function (elem) { return elem.Patients; });

    if (!dbg_compare_scalar_arrays(patients_arr, server_patients)) {
        console.error("dbg_validate_weekly_graph() FAILED");
        console.table(server_data);
        console.log(patients_arr);
        console.log(server_patients);
    }
}

function reset_graph(element_id) {
    var myChart = find_chart_by_id(element_id);
    if (myChart) {
        myChart.data.datasets = new Array();  // reset content of Chart
        myChart.update();
    }
}
function render_week_all_graphs(server_data, range) {
    let element_id = "dsb_hl_weekly_graph";
    var myChart = find_chart_by_id(element_id);

    // we want to "rescale" the 3 weeks on the same 7 days range

    // building a reverse index - dictionary
    let dict = {};
    server_data.map(function (obj) {
        dict[obj.Day] = obj;
    });

    //First span - this week (last Sunday ==> end_date)
    let the_date = moment(range.end_date, "YYYY-MM-DD");
    the_date.startOf('isoWeek'); // << Monday
    the_date.subtract(1, "days");
    let prepared_data = prepare_one_week_span_data(the_date, dict);

    if (!myChart) {
        create_month_week_graph(prepared_data, element_id);
        myChart = find_chart_by_id(element_id);
    }
    else {
        add_to_month_week_graph(myChart, prepared_data, get_week_card("curr"),
            !$("#dsb_hl_weekly_tbl_curr_show").is(':checked') );
    }

    // Compute the data for the previous week
    the_date.subtract(7, "days");
    prepared_data = prepare_one_week_span_data(the_date, dict);

    var myChart = find_chart_by_id(element_id);
    add_to_month_week_graph(myChart, prepared_data, get_week_card("prev"),
        !$("#dsb_hl_weekly_tbl_prev_show").is(':checked') );

    // Compute the data for two weeks ago
    the_date.subtract(7, "days");
    prepared_data = prepare_one_week_span_data(the_date, dict);

    add_to_month_week_graph(myChart, prepared_data, get_week_card("2wks_ago"),
        !$("#dsb_hl_weekly_tbl_2wks_ago_show").is(':checked') );

    if (window.ConfigFlags.is_debugging_dsb) {
        dbg_validate_weekly_graph(myChart, server_data);
    }
}



/*  ==================   MONTH  related code     =========================== */

function get_month_name_in_hebrew(month, month_designator) {
    let dateObj = moment(month).toDate();

    if (month_designator.localeCompare("prev") == 0) {
        dateObj.setMonth(dateObj.getMonth() - 1);
    }
    if (month_designator.localeCompare("yoy") == 0) {
        dateObj.setFullYear(dateObj.getFullYear() - 1);
    }

    return dateObj.toLocaleString("he", { year: 'numeric', month: "long" });
}


function get_month_range(month, month_designator) {
    // Just like in weeks, we provide the range from 1 - end of month
    let endDate, inMonth = moment(month).toDate();
    inMonth.setDate(1);

    if (month_designator.localeCompare("curr") == 0) {
        endDate = moment(month);
        endDate.endOf('month').add(1, 'days');
        // Bound end date with current date
        if (endDate.isAfter(moment())) {
           endDate = moment().add(1, 'days');;
        }
    }
    if (month_designator.localeCompare("prev") == 0) {
        inMonth.setMonth(inMonth.getMonth() - 1);
        // set end date to be first day of next month
        endDate = new Date(inMonth.getFullYear(), inMonth.getMonth() + 1, 1);
    }
    if (month_designator.localeCompare("curr_and_prev") == 0) {
        endDate = moment(month);
        endDate.endOf('month').add(1, 'days');
        // Bound end date with current date
        if (endDate.isAfter(moment())) {
            endDate = moment().add(1, 'days');;
        }
        inMonth.setMonth(inMonth.getMonth() - 1);
    }
    if (month_designator.localeCompare("yoy") == 0) {
        inMonth.setFullYear(inMonth.getFullYear() - 1);
        // set end date to be first day of next month
        endDate = new Date(inMonth.getFullYear(), inMonth.getMonth() + 1, 1);
    }

    let result = {
        start_date: moment(inMonth).format("YYYY-MM-DD"),
        end_date: moment(endDate).format("YYYY-MM-DD"),
        prev_start: "NA",
        prev_end: "NA",
        span: "MONTH"
    }
    return result;
}

function get_all_months_range(month) {
    // Just like in weeks, we provide the range from 1 - end of month
    let endDate, inMonth = moment(month).toDate();
    inMonth.setDate(1);
    endDate = moment(month);
    endDate.endOf('month').add(1, 'days');
    // Bound end date with current date
    if (endDate.isAfter(moment())) {
        endDate = moment().add(1, 'days');;
    }

    let prev_start = new Date(inMonth);
    let prev_end = new Date(inMonth.getFullYear(), inMonth.getMonth() + 1, 1);

    inMonth.setMonth(inMonth.getMonth() - 1);

    prev_start.setFullYear(prev_start.getFullYear() - 1);
    prev_end.setFullYear(prev_end.getFullYear() - 1);

    let result = {
        start_date: moment(inMonth).format("YYYY-MM-DD"),
        end_date: moment(endDate).format("YYYY-MM-DD"),
        prev_start: moment(prev_start).format("YYYY-MM-DD"),
        prev_end: moment(prev_end).format("YYYY-MM-DD"),
        span: "MONTH"
    }

    console.log("get_all_months_range()", month.format("YYYY-MM-DD"), "==>", result);
    return result;
}





const month_card_definitions = [
    {
        designator: "curr",
        checkbox: "#dsb_hl_monthly_tbl_curr_show",
        next: "prev"
    },
    {
        designator: "prev",
        checkbox: "#dsb_hl_monthly_tbl_prev_show",
        borderDash: [8,8],
        next: "yoy"
    },
    {
        designator: "curr_and_prev",
        checkbox: "#dsb_hl_monthly_tbl_curr_show",
        next: "yoy"
    },
    {
        designator: "yoy", 
        checkbox: "#dsb_hl_monthly_tbl_yoy_show",
        borderDash: [10, 4],
        next: null
    }
];

function get_month_card(dsg) {
    for (const card_def of month_card_definitions) {
        if (card_def.designator.localeCompare(dsg) == 0) {
            return card_def;
        }
    }
    return null;
}

function on_monthly_date_change() {
    let selected_day = moment($("#dsb_select_month").val(), K_DateFormat_Moment);
    let today = moment();
    if (selected_day.isAfter(today)) {
        selected_day = today;
    }

    reset_graph("dsb_hl_monthly_graph");
    $(".dsb_monthly_num").text("--");  // reset all the weekly number fields.


    start_month_graph(selected_day, get_month_card("curr_and_prev"));
    if (window.ConfigFlags.load_rows) {
        start_all_months_rows(selected_day);
        start_one_month_new_volunteers(selected_day, get_month_card("curr"));
    }

}

function setup_monthly_choose_combo() {
    var dt = $('#dsb_select_month').datepicker({
        format: K_DateFormat_DatePicker,
        autoclose: true
    });
    let today = new Date();
    dt.datepicker('setDate', today);
    dt.on("changeDate", on_monthly_date_change);
}


function start_monthly_cards(month) {

   start_all_months_rows(month);

   start_month_graph(month, get_month_card("curr_and_prev"));

   start_one_month_new_volunteers(month, get_month_card("curr"));

    setup_monthly_choose_combo();
    $("#dsb_hl_monthly_tbl_curr_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_monthly_tbl_prev_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_monthly_tbl_yoy_show").click(toggle_month_week_graph_datasets);
}

function start_all_months_rows(month) {
    update_all_month_rows_names(month);

    var query_object = get_all_months_range(month);

    // Invok Async call, to get info for this row.
    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportWithPeriodDigestMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            render_month_rows(result);
        },
        error: function (err) {
        }
    });
}

function render_month_rows(result) {
    console.log("render_month_row ==>", result);

    render_one_month_row("yoy", result[0]); // sorted ASC
    render_one_month_row("prev", result[1]);  
    render_one_month_row("curr", result[2]);  
}

function render_one_month_row(dsg, entry) {
    let label_id = "#dsb_hl_monthly_tbl_" + dsg + "_pats";
    $(label_id).text(entry.Patients);
    label_id = "#dsb_hl_monthly_tbl_" + dsg + "_rides";
    $(label_id).text(entry.Rides);
    label_id = "#dsb_hl_monthly_tbl_" + dsg + "_vols";
    $(label_id).text(entry.Volunteers);
}

function update_month_rows_names(month, dsg) {
    if (dsg.localeCompare("curr_and_prev") == 0) {
        update_one_month_row_name(month, "curr");
        let prev = moment(month).subtract(1, 'days');
        update_one_month_row_name(prev, "prev");
    }
    else {
        update_one_month_row_name(month, dsg);
    }
}

function update_all_month_rows_names(month) {
    update_one_month_row_name("yoy", month);
    update_one_month_row_name("prev", month);
    update_one_month_row_name("curr", month);
}

function update_one_month_row_name(dsg, month) {
    let label_id = "#dsb_hl_monthly_tbl_" + dsg + "_month_name";
    let month_name = get_month_name_in_hebrew(month, dsg);
    $(label_id).text(month_name);
}


function start_one_month_new_volunteers(month, card_def) {
    let dsg = card_def.designator;
    var query_object = get_month_range(month, dsg);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportNewDriversInRange",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            // Schedule fetch for next data-set if needed.
            let next_card = get_month_card(card_def.next);
            if (next_card) {
                start_one_month_new_volunteers(moment(month), next_card);   // Not really recursive - called from incoming-data callback
            }

            result = data.d;
            label_id = "#dsb_hl_monthly_tbl_" + dsg + "_new";
            $(label_id).text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}


function start_month_graph(month, card_def) {
    var query_object = get_month_range(month, card_def.designator);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportWeeklyGraphMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            // Schedule fetch for next data-set if needed.
            let next_card = get_month_card(card_def.next);
            if (next_card) {
                if (window.ConfigFlags.full_loading || window.ConfigFlags.all_graphs) {
                    start_month_graph(moment(month), next_card);   // Not really recursive - called from incoming-data callback
                }
            }
            result = data.d;
            render_months_graph_data(card_def, query_object, result, "dsb_hl_monthly_graph");

        },
        error: function (err) {
        }


    });
}


function find_chart_by_id(chart_id) {
    var result  = null;
    Chart.helpers.each(Chart.instances, function (instance) {
        if (instance.ctx.canvas.id.localeCompare(chart_id) == 0) {
            result = instance;
        }
    })

    return result;
}

// dump given array to csv file
// Works for small files (less than 1,000 rows)
// https://stackoverflow.com/a/14966131
function dbg_dump_rows_as_csv(rows, file_name) {
    let csvContent = "data:text/csv;charset=utf-8,";

    rows.forEach(function (rowArray) {
        let row = rowArray.join(",");
        csvContent += row + "\r\n";
    });

    let encodedUri = encodeURI(csvContent);
    let link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", file_name);
    document.body.appendChild(link); // Required for FF

    link.click(); // This will download the data file 
}

// Call from console as    dbg_dump_graph("dsb_hl_monthly_graph") 
function dbg_dump_graph(chart_id) {
    let myChart = find_chart_by_id(chart_id);
    let data = myChart.data;

    let rows = new Array();
    data.labels.forEach(function add_row(label, index) {
        let a_row = new Array();
        a_row.push(label);
        for (a_dataset of data.datasets) {
            a_row.push(a_dataset.data[index]);
        }
        rows.push(a_row);
    });

    let selected_day = moment($("#dsb_select_month").val(), K_DateFormat_Moment);
    let file_name = selected_day.format("DD-MMM-YYYY") + "__gen_at_v2_" + moment().format("HH_mm_SS__DD-MMM-YYYY") + ".csv";
    // console.table(rows)
    dbg_dump_rows_as_csv(rows, file_name);
}

// Call from console with class of labels e.g.   dbg_dump_card_rows("month")
function dbg_dump_card_rows(designator) {
    let row_class = "dsb_monthly_num", select_field = "#dsb_select_month";
    if (designator.localeCompare("week") == 0) {
        row_class = "dsb_weekly_num";
        select_field = "#dsb_select_week";
    }

    let rows = new Array();
    $("." + row_class).each(function (index) { rows.push([$(this).attr("id"), $(this).text()]); });
    let selected_day = moment($(select_field).val(), K_DateFormat_Moment);
    let file_name = row_class +  "__" + selected_day.format("DD-MMM-YYYY") + "__gen_at_v2_" + moment().format("HH_mm_SS__DD-MMM-YYYY") + ".csv";
    dbg_dump_rows_as_csv(rows, file_name);
    console.table(rows);
}

function prepare_one_month_span_data(server_data, year_and_month_prefix) {
    // building a reverse index - dictionary
    let key, dict = {};
    server_data.map(function (obj) {
        dict[obj.Day] = obj;    // day is YYYY-MM-DD
    });

    let prepared_data = {
        labels: [],
        rides: [],
        volunteers: [],
        patients: []
    }

    for (i = 1; i < 32; ++i) {
        let day_num = i.toString();
        prepared_data.labels.push(day_num);
        if (i < 10) {
            key = year_and_month_prefix + "0" + i;   // Build key that is YYYY-MM-DD
        }
        else {
            key = year_and_month_prefix + i;
        }
        // check if we have info on this specific day:
        if (key in dict) {
            let dayinfo = dict[key];
            prepared_data.rides.push(dayinfo.Rides);
            prepared_data.volunteers.push(dayinfo.Volunteers);
            prepared_data.patients.push(dayinfo.Patients);
        }
        else {
            prepared_data.rides.push(0);
            prepared_data.volunteers.push(0);
            prepared_data.patients.push(0);
        }
    }

    return prepared_data;
}

// The data returned from server  may contain more info that this month needs (due to reduce-server-query optimization)
// So we use the range and only take a slice of the data, identified using 'year_and_month_prefix'
function render_months_graph_data(card_def, query_object, server_data, element_id) {

    if (card_def.designator.localeCompare("curr_and_prev") == 0) {
        // We can render also the 'curr' month using the given data
        let t = moment(query_object.start_date, "YYYY-MM-DD");
        t.add(1, 'months');
        let year_and_month_prefix = t.format("YYYY-MM-");
        console.log("render_months_graph_data", query_object, "==>", year_and_month_prefix);
        let prepared_data = prepare_one_month_span_data(server_data, year_and_month_prefix);
        render_one_month_graph(card_def, prepared_data, element_id);
    }

    let year_and_month_prefix = query_object.start_date.substring(0, 8);  // YYYY-MM-
    console.log("render_months_graph_data", query_object, "==>", year_and_month_prefix);
    let prepared_data = prepare_one_month_span_data(server_data, year_and_month_prefix);
    render_one_month_graph(card_def, prepared_data, element_id);

}

function render_one_month_graph(card_def, prepared_data, element_id)
{
    var myChart = find_chart_by_id(element_id); 

    if (myChart) {
        // window.myd = myChart;
        let is_hidden = !$(card_def.checkbox).is(':checked');
        add_to_month_week_graph(myChart, prepared_data, card_def, is_hidden );
    }
    else {
        // make sure labels end with 31
        window.dbg = prepared_data;
        create_month_week_graph(prepared_data, element_id);
    }
}

const r2rHTMLLegend = {
    id: 'r2rHTMLLegend',
    afterUpdate(chart, args) {
        console.log("r2rHTMLLegend::afterUpdate", chart, args);
    }
};

// https://stackoverflow.com/a/71382202
const ChartJScustomTitle = {
    id: 'customTitle',
    beforeLayout: (chart, args, opts) => {
        const {
            display,
            font
        } = opts;
        if (!display) {
            return;
        }
        const {
            ctx
        } = chart;
        ctx.font = font || '11px sans-serif'

        const {
            fontBoundingBoxAscent,
            fontBoundingBoxDescent
        } = ctx.measureText(opts.text);
        chart.options.layout.padding.top = fontBoundingBoxAscent + fontBoundingBoxDescent + 20;
    },
    afterDraw: (chart, args, opts) => {
        const {
            font,
            text,
            color
        } = opts;
        const {
            ctx,
            chartArea: {
                top,
                bottom,
                left,
                right
            }
        } = chart;
        if (opts.display) {
            ctx.fillStyle = color || Chart.defaults.color
            ctx.font = font || '11px sans-serif'
            const {
                width,
                fontBoundingBoxAscent,
                fontBoundingBoxDescent
            } = ctx.measureText(text);
            ctx.fillText(text, width, fontBoundingBoxAscent + fontBoundingBoxDescent);
        }
    }
};

function create_month_week_graph(prepared_data, graph_id)
{
    // We use version 2.1.4 of chart.js
    //FUTURE  Chart.pluginService.register(r2rHTMLLegend);

    var ctx = document.getElementById(graph_id).getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: prepared_data.labels,
            datasets: [
                {
                    label: 'הסעות',
                    data: prepared_data.rides,
                    fill: false,
                    borderColor: CHART_COLORS.green,
                    backgroundColor: CHART_COLORS.green,
                    borderWidth: 1
                },
                {
                    label: 'חולים',
                    data: prepared_data.patients,
                    fill: false,
                    borderColor: CHART_COLORS.purple,
                    backgroundColor: CHART_COLORS.purple,
                    borderWidth: 1
                },
                {
                    label: 'מתנדבים',
                    data: prepared_data.volunteers,
                    fill: false,
                    borderColor: CHART_COLORS.orange,
                    backgroundColor: CHART_COLORS.orange,
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: false,
            plugins: {
                legend: {
                    display: false
                },
                customTitle: {
                    display: true,
                    text: 'מספר האנשים / הסעות',
                    color: 'black'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                }
            }
        },
        plugins: [ChartJScustomTitle]
    });
}


function add_to_month_week_graph(myChart, prepared_data, card_def, is_hidden) {
    let updated = myChart.data.datasets.concat(
        [
            {
                label: 'הסעות',
                data: prepared_data.rides,
                fill: false,
                borderColor: CHART_COLORS.green,
                backgroundColor: CHART_COLORS.green,
                borderWidth: 1,
                borderDash: card_def.borderDash,
                hidden: is_hidden
            },
            {
                label: 'חולים',
                data: prepared_data.patients,
                fill: false,
                borderColor: CHART_COLORS.purple,
                backgroundColor: CHART_COLORS.purple,
                borderWidth: 1,
                borderDash: card_def.borderDash,
                hidden: is_hidden
            },
            {
                label: 'מתנדבים',
                data: prepared_data.volunteers,
                fill: false,
                borderColor: CHART_COLORS.orange,
                backgroundColor: CHART_COLORS.orange,
                borderWidth: 1,
                borderDash: card_def.borderDash,
                hidden: is_hidden
            }
        ]);
    myChart.data.datasets = updated;
    myChart.update();
}

function toggle_month_week_graph_datasets(event) {
    let chart_id = event.target.getAttribute("r2chart")
    let chart = find_chart_by_id(chart_id);
    if (chart) {
        let indices = event.target.getAttribute("r2data").split(",");
        for (const idx of indices) {
            let v = parseInt(idx, 10);
            if (event.target.checked) {
                chart.show(v);
            }
            else {
                chart.hide(v);
            }
        }
    }
}

function start_yearly_cards() {
    start_all_years_rows();
    start_year_graph("12months");
}

function get_year_range(year_designator) {
    let end = new Date();
    let start = new Date(end.getFullYear(), 0, 1); // 01-Jan

    if (year_designator.localeCompare("yoy") == 0) {
        end.setFullYear(end.getFullYear() - 1);
        start.setFullYear(start.getFullYear() - 1);
    }

    if (year_designator.localeCompare("12months") == 0) {
        start = new Date();
        start.setFullYear(start.getFullYear() - 1);
        // Avoid counting teh rides of the same month ayear ago, under this month
        // e.g. if today is 17-Nov-2021, we do not want to count rides of range
        // 01-Nov-2020 --> 17-Nov-2020 in November (due to GROUP BY MONTH(PickupTime))
        start.setMonth(start.getMonth() + 1);
        start.setDate(1);
    }

    if (year_designator.localeCompare("prev12months") == 0) {
        start = new Date();
        start.setFullYear(start.getFullYear() - 2);
        start.setMonth(start.getMonth() + 1);
        start.setDate(1);
        end.setFullYear(end.getFullYear() - 1);
        end.setMonth(end.getMonth() + 1);
        end.setDate(1);
    }

    let result = {
        start_date: moment(start).format("YYYY-MM-DD"),
        end_date: moment(end).format("YYYY-MM-DD")
    }
    return result;
}

function get_years_range() {
    let end = new Date();
    let start = new Date(end.getFullYear(), 0, 1); // 01-Jan

    let yoy_start = new Date(start);
    let yoy_end = new Date(end);
    yoy_end.setFullYear(end.getFullYear() - 1);
    yoy_start.setFullYear(start.getFullYear() - 1);

    let result = {
        start_date: moment(start).format("YYYY-MM-DD"),
        end_date: moment(end).format("YYYY-MM-DD"),
        prev_start: moment(yoy_start).format("YYYY-MM-DD"),
        prev_end: moment(yoy_end).format("YYYY-MM-DD"),
        span: "YEAR"
    }
    return result;
}

function start_all_years_rows() {
    var query_object = get_years_range();

    console.log("start_all_years_rows", query_object);


    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportWithPeriodDigestMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            render_years_rows(result);
        },
        error: function (err) {
        }
    });
}

function render_years_rows(result) {
    render_year_row("ytd", result[1]);
    render_year_row("yoy", result[0]);
}



function render_year_row(dsg, result) {
    let label_id = "#dsb_hl_yearly_tbl_" + dsg + "_pats";
    $(label_id).text(result.Patients);
    label_id = "#dsb_hl_yearly_tbl_" + dsg + "_rides";
    $(label_id).text(result.Rides);
    label_id = "#dsb_hl_yearly_tbl_" + dsg + "_vols";
    $(label_id).text(result.Volunteers);
}


function start_year_graph(dsg) {
    var query_object = get_year_range(dsg);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportYearlyGraphMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            if (dsg.localeCompare("12months") == 0) {
                start_year_graph("prev12months"); // Schedule fetch of prev year data
            }
            result = data.d;
            render_year_graph(dsg, result);
        },
        error: function (err) {
        }
    });
}


function render_year_graph(dsg, data) {
    // We get from DB the months in 1-12 order.
    // We need to order as - last 12 months
    let today = new Date();
    let curr_month = today.getMonth() + 1;
    let last_year = data.slice(curr_month);
    data = last_year.concat(data.slice(0, curr_month));


    let prepared_data = {
        labels: data.map(function (obj) { return obj.Day; }),
        rides: data.map(function (obj) { return obj.Rides; }),
        volunteers: data.map(function (obj) { return obj.Volunteers; }),
        patients: data.map(function (obj) { return obj.Patients; })
   };

    var myChart = null;
    var myChart = find_chart_by_id("dsb_hl_yearly_graph");

    if (myChart) {
        add_to_year_graph(myChart, prepared_data, dsg);
    }
    else {
        create_year_graph(prepared_data);
    }
}


function create_year_graph(data) {
 
    var ctx = document.getElementById('dsb_hl_yearly_graph').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'הסעות',
                    data: data.rides,
                    fill: false,
                    borderColor: CHART_COLORS.green,
                    backgroundColor: CHART_COLORS.green,
                    borderWidth: 1
                },
                {
                    label: 'חולים',
                    data: data.patients,
                    fill: false,
                    borderColor: CHART_COLORS.purple,
                    backgroundColor: CHART_COLORS.purple,
                    borderWidth: 1
                },
                {
                    label: 'מתנדבים',
                    data: data.volunteers,
                    fill: false,
                    borderColor: CHART_COLORS.orange,
                    backgroundColor: CHART_COLORS.orange,
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: false,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function add_to_year_graph(myChart, prepared_data, dsg) {
    let updated = myChart.data.datasets.concat(
        [
            {
                label: 'הסעות',
                data: prepared_data.rides,
                fill: false,
                borderColor: CHART_COLORS.green,
                backgroundColor: CHART_COLORS.green,
                borderWidth: 1,
                borderDash: [10, 10]
            },
            {
                label: 'חולים',
                data: prepared_data.patients,
                fill: false,
                borderColor: CHART_COLORS.purple,
                backgroundColor: CHART_COLORS.purple,
                borderWidth: 1,
                borderDash: [10, 10]
            },
            {
                label: 'מתנדבים',
                data: prepared_data.volunteers,
                fill: false,
                borderColor: CHART_COLORS.orange,
                backgroundColor: CHART_COLORS.orange,
                borderWidth: 1,
                borderDash: [10, 10]
            }
        ]);
    myChart.data.datasets = updated;
    myChart.update();
}