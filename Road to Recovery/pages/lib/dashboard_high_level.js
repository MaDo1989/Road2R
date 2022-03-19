// Purpose: Dashboard UI for Amuta


window.is_debugging_dsb = false; 
window.full_loading = true;  
window.all_graphs = true;


// a mapping of event names to the next code that should be called
// used to avoid flooding the server with dozens of queries at startup.
// determines order of what gets populated first. -  TODAY & Daily.
// search for event names in code to see usage.
let K_chained_loading_table = {}; 

function set_chain_cb_for_event(event_name, cb) {
    K_chained_loading_table[event_name] = cb;
}

function call_next_in_chain(event_name) {
    if (event_name in K_chained_loading_table) {
        cb = K_chained_loading_table[event_name];
        cb();
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
    if (window.full_loading) {
        set_chain_cb_for_event("on_daily_finished", start_yearly_cards);
        set_chain_cb_for_event("on_weekly_finished", start_monthly_cards);

        start_daily_cards();
        start_weekly_cards();
    }
    else {
        // For fast debugging
        start_weekly_cards();
        // start_month_graph(get_month_card("curr"));
        // start_one_week_row(get_week_card("curr"));
    }
}

function start_daily_cards() {
    start_current_day_row();
}

// Initiate async ajax call. When call finishes, invoke card's on_data callback
function start_current_day_row() {

    // let today = new Date(2021, 09, 12); // 12-Oct-2020
    // let today = new Date(2021, 10, 05); // 05-Nov-2020
    
    let today = new Date();

    $("#dsb_hl_daily_rides_todays_date").text(moment(today).format('DD.MM'));

    var query_object = {
        start_date: moment(today).format('YYYY-MM-DD'),
        end_date: moment(today).add(1, 'days').format('YYYY-MM-DD')
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
            result = data.d;
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
    start_one_week_row(get_week_card("curr"));

    start_week_all_graphs();

    start_one_week_new_volunteers(get_week_card("curr"));

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


function get_week_range(week_designator) {
    let end_date = moment();
    let start_date = moment();

    if (week_designator.localeCompare("curr") == 0) {
        start_date.subtract(7, 'days');
    }
    if (week_designator.localeCompare("prev") == 0) {
        start_date.subtract(14, 'days');
        end_date.subtract(7, 'days');
    }
    if (week_designator.localeCompare("2wks_ago") == 0) {
        start_date.subtract(21, 'days');
        end_date.subtract(14, 'days');
    }

    let result = {
        start_date: start_date.format("YYYY-MM-DD"),
        end_date: end_date.format("YYYY-MM-DD")
    }
    if (window.is_debugging_dsb) {
        console.log("Overriding", result);
        result = {
            start_date: "2022-02-08",
            end_date: "2022-02-15"
        }
    }
    return result;
}


// To improve performance, we query the entire last 21 days in one shot
function start_week_all_graphs() {
    let end_date = moment();
    let start_date = moment();
    start_date.subtract(21, 'days');
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



function start_one_week_row(card_def) {
    let dsg = card_def.designator;
    let label_id = "#dsb_hl_weekly_tbl_" + dsg + "_week_name";
    let week_name = get_week_name_in_hebrew(dsg);
    $(label_id).text(week_name);

    // Invok Async call, to get info for this row.

    var query_object = get_week_range(dsg);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportMonthlyDigestMetrics",
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
                start_one_week_row(next_card);   // Not really recursive - called from incoming-data callback
            }
            result = data.d;
            render_week_row(dsg, result);
        },
        error: function (err) {
        }
    });
}

function render_week_row(dsg, result) {
    let label_id = "#dsb_hl_weekly_tbl_" + dsg + "_pats";
    $(label_id).text(result.Patients);
    label_id = "#dsb_hl_weekly_tbl_" + dsg + "_rides";
    $(label_id).text(result.Rides);
    label_id = "#dsb_hl_weekly_tbl_" + dsg + "_vols";
    $(label_id).text(result.Volunteers);
}


function start_one_week_new_volunteers(card_def) {
    let dsg = card_def.designator;
    var query_object = get_week_range(dsg);

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
                start_one_week_new_volunteers(next_card);   // Not really recursive - called from incoming-data callback
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

function prepare_one_week_span_data(the_date, dict) {
    let prepared_data = {
        labels: [],
        rides: [],
        volunteers: [],
        patients: []
    }

    for (i = 0; i < 7; ++i) {
        let hebrew_day_name = the_date.toDate().toLocaleString("he", { weekday: 'long' });
        prepared_data.labels.push(hebrew_day_name);

        // check if we have info on this specific day:
        let ddmm = the_date.format("MM-DD");
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


function render_week_all_graphs(data, range) {
    // we want to "rescale" the 3 weeks on the same 7 days range

    // building a reverse index - dictionary
    let dict = {};
    data.map(function (obj) {
        dict[obj.Day] = obj;
    });

    //First span - this week (7-days-ago ==> end_date)
    let the_date = moment(range.end_date, "YYYY-MM-DD");
    the_date.subtract(7, "days");
    let prepared_data = prepare_one_week_span_data(the_date, dict);

    let element_id = "dsb_hl_weekly_graph";
    create_month_week_graph(prepared_data, element_id);

    // Compute the data for the previous week
    the_date = moment(range.end_date, "YYYY-MM-DD");
    the_date.subtract(14, "days");
    prepared_data = prepare_one_week_span_data(the_date, dict);

    var myChart = find_chart_by_id(element_id);
    add_to_month_week_graph(myChart, prepared_data, get_week_card("prev"));

    // Compute the data for two weeks ago
    the_date = moment(range.end_date, "YYYY-MM-DD");
    the_date.subtract(21, "days");
    prepared_data = prepare_one_week_span_data(the_date, dict);

    add_to_month_week_graph(myChart, prepared_data, get_week_card("2wks_ago"));
}



/*  ==================   MONTH  related code     =========================== */

function get_month_name_in_hebrew(month_designator) {
    let dateObj = new Date();

    if (month_designator.localeCompare("prev") == 0) {
        dateObj.setMonth(dateObj.getMonth() - 1);
    }
    if (month_designator.localeCompare("yoy") == 0) {
        dateObj.setFullYear(dateObj.getFullYear() - 1);
    }

    return dateObj.toLocaleString("he", { year: 'numeric', month: "long" });
}


function get_month_range(month_designator) {
    let endDate, dateObj = new Date();
    dateObj.setDate(1);

    if (month_designator.localeCompare("curr") == 0) {
        // Bound end date with current date
        endDate = new Date();
        endDate.setDate(endDate.getDate() + 1);
    }
    if (month_designator.localeCompare("prev") == 0) {
        dateObj.setMonth(dateObj.getMonth() - 1);
        // set end date to be first day of next month
        endDate = new Date(dateObj.getFullYear(), dateObj.getMonth() + 1, 1);
    }
    if (month_designator.localeCompare("yoy") == 0) {
        dateObj.setFullYear(dateObj.getFullYear() - 1);
        // set end date to be first day of next month
        endDate = new Date(dateObj.getFullYear(), dateObj.getMonth() + 1, 1);
    }

    let result = {
        start_date: moment(dateObj).format("YYYY-MM-DD"),
        end_date: moment(endDate).format("YYYY-MM-DD")
    }
    if (window.is_debugging_dsb) {
        result = {
            start_date: "2021-10-01",
            end_date: "2021-10-31"
        }
    }
    return result;
}

const month_card_definitions = [
    {
        designator: "curr",
        next: "prev"
    },
    {
        designator: "prev",
        borderDash: [8,8],
        next: "yoy"
    },
    {
        designator: "yoy", 
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
function start_monthly_cards() {

   start_one_month_row(get_month_card("curr"));

   start_month_graph(get_month_card("curr"));

   start_one_month_new_volunteers(get_month_card("curr"));


    $("#dsb_hl_monthly_tbl_curr_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_monthly_tbl_prev_show").click(toggle_month_week_graph_datasets);
    $("#dsb_hl_monthly_tbl_yoy_show").click(toggle_month_week_graph_datasets);
}



function start_one_month_row(card_def) {
    let dsg = card_def.designator;
    let label_id = "#dsb_hl_monthly_tbl_" + dsg + "_month_name";
    let month_name = get_month_name_in_hebrew(dsg);
    $(label_id).text(month_name);

    // Invok Async call, to get info for this row.

    var query_object = get_month_range(dsg);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportMonthlyDigestMetrics",
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
                start_one_month_row(next_card);   // Not really recursive - called from incoming-data callback
            }
            result = data.d;
            render_month_row(dsg, result);
        },
        error: function (err) {
        }
    });
}

function render_month_row(dsg, result) {
    let label_id = "#dsb_hl_monthly_tbl_" + dsg + "_pats";
    $(label_id).text(result.Patients);
    label_id = "#dsb_hl_monthly_tbl_" + dsg + "_rides";
    $(label_id).text(result.Rides);
    label_id = "#dsb_hl_monthly_tbl_" + dsg + "_vols";
    $(label_id).text(result.Volunteers);
}


function start_one_month_new_volunteers(card_def) {
    let dsg = card_def.designator;
    var query_object = get_month_range(dsg);

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
                start_one_month_new_volunteers(next_card);   // Not really recursive - called from incoming-data callback
            }

            result = data.d;
            label_id = "#dsb_hl_monthly_tbl_" + dsg + "_new";
            $(label_id).text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}


function start_month_graph(card_def) {
    var query_object = get_month_range(card_def.designator);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportMonthlyGraphMetrics",
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
                if (window.full_loading || window.all_graphs) {
                    start_month_graph(next_card);   // Not really recursive - called from incoming-data callback
                }
            }
            result = data.d;
            render_month_week_graph(card_def, result, "dsb_hl_monthly_graph");

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

function render_month_week_graph(card_def, data, element_id)
{
    let prepared_data = {
        labels: data.map(function (obj) { return obj.Day; }),
        rides: data.map(function (obj) { return obj.Rides; }),
        volunteers: data.map(function (obj) { return obj.Volunteers; }),
        patients: data.map(function (obj) { return obj.Patients; })
    };

    var myChart = find_chart_by_id(element_id); 

    if (myChart) {
        // window.myd = myChart;
        add_to_month_week_graph(myChart, prepared_data, card_def);
    }
    else {
        create_month_week_graph(prepared_data, element_id);
    }
}

const r2rHTMLLegend = {
    id: 'r2rHTMLLegend',
    afterUpdate(chart, args) {
        console.log("r2rHTMLLegend::afterUpdate", chart, args);
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


function add_to_month_week_graph(myChart, prepared_data, card_def) {
    let updated = myChart.data.datasets.concat(
        [
            {
                label: 'הסעות',
                data: prepared_data.rides,
                fill: false,
                borderColor: CHART_COLORS.green,
                backgroundColor: CHART_COLORS.green,
                borderWidth: 1,
                borderDash: card_def.borderDash
            },
            {
                label: 'חולים',
                data: prepared_data.patients,
                fill: false,
                borderColor: CHART_COLORS.purple,
                backgroundColor: CHART_COLORS.purple,
                borderWidth: 1,
                borderDash: card_def.borderDash
            },
            {
                label: 'מתנדבים',
                data: prepared_data.volunteers,
                fill: false,
                borderColor: CHART_COLORS.orange,
                backgroundColor: CHART_COLORS.orange,
                borderWidth: 1,
                borderDash: card_def.borderDash
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

    start_one_year_row("ytd");
    start_one_year_row("yoy");

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


function start_one_year_row(dsg) {
    var query_object = get_year_range(dsg);

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportMonthlyDigestMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            result = data.d;
            render_year_row(dsg, result);
        },
        error: function (err) {
        }
    });
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