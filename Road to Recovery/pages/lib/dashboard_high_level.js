// Purpose: Dashboard UI for Amuta

// next step - first note in GitHub Project Trello board - Dashboard column.


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
    // start_daily_cards();
    start_monthly_cards();
    // start_yearly_cards();
}

function start_daily_cards() {
    start_current_day_row();
}

// Initiate async ajax call. When call finishes, invoke card's on_data callback
function start_current_day_row() {


    // @@ 
    // let today = new Date(2021, 09, 12); // 12-Oct-2020
    // let today = new Date();
    let today = new Date(2021, 10, 05); // 05-Nov-2020

    $("#dsb_hl_daily_rides_todays_date").text(moment(today).format('DD.MM'));

    var query_object = {
        start_date: moment(today).format('YYYY-MM-DD'),
        end_date: moment(today).add(1, 'days').format('YYYY-MM-DD')
    }

    start_current_daily_totals(query_object);

    start_current_daily_need_drivers(query_object);

    start_current_daily_new_volunteers(query_object);

}

function start_current_daily_totals(query_object) {
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
            result = data.d;
            $("#dsb_hl_daily_volunteers_attention").text(result.Volunteers);
        },
        error: function (err) {
        }
    });
}


function render_card_rides(card_def, metric_info) {
    var ctx = document.getElementById(card_def.canvas_name).getContext('2d');
    var slice_def = slice_definitions[card_def.slice];

    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: slice_def.labels,
            datasets: [{
                label: card_def.title,
                data: [metric_info.Value1, metric_info.Value2],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

}


function render_card_patients(card_def, metric_info) {
    var ctx = document.getElementById(card_def.canvas_name).getContext('2d');
    var slice_def = slice_definitions[card_def.slice];

    var data = {
        labels: slice_def.labels,
        datasets: [{
            label: card_def.title,
            data: [metric_info.Value1, metric_info.Value2],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)'
            ],
            borderWidth: 1
        }]
    };

    var myChart = new Chart(ctx, {
        type: 'line',
        data: data,
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function render_card_engaged_volunteers(card_def, metric_info) {
    var ctx = document.getElementById(card_def.canvas_name).getContext('2d');
    var slice_def = slice_definitions[card_def.slice];

    var data = {
        labels: slice_def.labels,
        datasets: [{
            label: card_def.title,
            data: [metric_info.Value1, metric_info.Value2],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)'
            ],
            borderWidth: 1
        }]
    };

    var myChart = new Chart(ctx, {
        type: 'line',
        data: data,
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}



function render_card_new_volunteers(card_def, metric_info) {
    var ctx = document.getElementById(card_def.canvas_name).getContext('2d');
    var slice_def = slice_definitions[card_def.slice];

    var data = {
        labels: slice_def.labels,
        datasets: [{
            label: card_def.title,
            data: [metric_info.Value1, metric_info.Value2],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)'
            ],
            borderWidth: 1
        }]
    };

    var myChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
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


    /*
const card_definitions = [
    {
        name: "rides",
        title: "הסעות",
        slice: "SLICE_DEF_YTD",
        render: render_card_rides,
        canvas_name: "dsb_hl_card_rides"
    },

    {
        name: "patients",
        title: "חולים",
        slice: "SLICE_MONTH_YTD",
        render: render_card_patients,
        canvas_name: "dsb_hl_card_patients"
    },
    {
        name: "engaged_volunteers",
        title: "מתנדבים מעורבים",
        slice: "SLICE_MONTH_YTD",
        render: render_card_engaged_volunteers,
        canvas_name: "dsb_hl_card_engaged_volunteers"
    },
    {
        name: "new_volunteers",
        title: "מתנדבים חדשים",
        slice: "SLICE_MONTH_YTD",
        render: render_card_new_volunteers,
        canvas_name: "dsb_hl_card_new_volunteers"
    },
];
    */

 slice_definitions = {
    "SLICE_DEF_YTD": {
        labels: ['Prev Year to date', 'This year to date']
    },
    "SLICE_MONTH_YTD": {
        labels: ['Prev Month to date', 'This Month to date']
    }
};



// https://www.chartjs.org/docs/latest/
function display_demo_card() {

    var ctx = document.getElementById('myChart').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
            datasets: [{
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

}


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
    let dateObj = new Date();

    if (month_designator.localeCompare("prev") == 0) {
        dateObj.setMonth(dateObj.getMonth() - 1);
    }
    if (month_designator.localeCompare("yoy") == 0) {
        dateObj.setFullYear(dateObj.getFullYear() - 1);
    }

    dateObj.setDate(1);
    let endDate = new Date(dateObj.getFullYear(),dateObj.getMonth() + 1, 0);

    let result = {
        start_date: moment(dateObj).format("YYYY-MM-DD"),
        end_date: moment(endDate).format("YYYY-MM-DD")
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

    for (const card_def of month_card_definitions) {
     //@@    start_one_month_row(card_def);
    }

    start_month_graph(get_month_card("curr"));

   for (const card_def of month_card_definitions) {
      //@@   start_one_month_new_volunteers(card_def);
    }
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
                start_month_graph(next_card);   // Not really recursive - called from incoming-data callback
            }
            result = data.d;
            render_month_graph(card_def, result);

        },
        error: function (err) {
        }


    });
}


function render_month_graph(card_def, data)
{
    let prepared_data = {
        labels: data.map(function (obj) { return obj.Day; }),
        rides: data.map(function (obj) { return obj.Rides; }),
        volunteers: data.map(function (obj) { return obj.Volunteers; }),
        patients: data.map(function (obj) { return obj.Patients; })
    };

    var myChart = null;
    // Find the chart, if exists
    Chart.helpers.each(Chart.instances, function (instance) {
        if (instance.chart.canvas.id.localeCompare("dsb_hl_monthly_graph") == 0) {
            myChart = instance;
        }
    })

    if (myChart) {
        add_to_month_graph(myChart, prepared_data, card_def);
    }
    else {
        create_month_graph(prepared_data);
    }
}

const r2rHTMLLegend = {
    id: 'r2rHTMLLegend',
    afterUpdate(chart, args) {
        console.log("r2rHTMLLegend::afterUpdate", chart, args);
    }
};

function create_month_graph(prepared_data)
{
    // We use version 2.1.4 of chart.js
    Chart.pluginService.register(r2rHTMLLegend);

    var ctx = document.getElementById('dsb_hl_monthly_graph').getContext('2d');
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
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}


function add_to_month_graph(myChart, prepared_data, card_def) {
    window.myd = myChart;
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


function start_yearly_cards() {

    start_one_year_row("ytd");
    start_one_year_row("yoy");

    start_year_graph();
}

function get_year_range(year_designator) {
    let today = new Date();
    let start = new Date(today.getFullYear(), 0, 1); // 01-Jan

    if (year_designator.localeCompare("yoy") == 0) {
        today.setFullYear(today.getFullYear() - 1);
        start.setFullYear(start.getFullYear() - 1);
    }

    if (year_designator.localeCompare("12months") == 0) {
        start = new Date();
        start.setFullYear(start.getFullYear() - 1);
    }

    let result = {
        start_date: moment(start).format("YYYY-MM-DD"),
        end_date: moment(today).format("YYYY-MM-DD")
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


function start_year_graph() {
    var query_object = get_year_range("12months");

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
            result = data.d;
            render_year_graph(result);
        },
        error: function (err) {
        }
    });
}

function render_year_graph(data) {
    // We get from DB the months in 1-12 order.
    // We need to order as - last 12 months
    let today = new Date();
    let curr_month = today.getMonth() + 1;
    let last_year = data.slice(curr_month);
    data = last_year.concat(data.slice(0, curr_month));

    let labels = data.map(function (obj) { return obj.Day; });
    let rides = data.map(function (obj) { return obj.Rides; });
    let volunteers = data.map(function (obj) { return obj.Volunteers; });
    let patients = data.map(function (obj) { return obj.Patients; });

    var ctx = document.getElementById('dsb_hl_yearly_graph').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'הסעות',
                    data: rides,
                    fill: false,
                    borderColor: CHART_COLORS.green,
                    backgroundColor: CHART_COLORS.green,
                    borderWidth: 1
                },
                {
                    label: 'חולים',
                    data: patients,
                    fill: false,
                    borderColor: CHART_COLORS.purple,
                    backgroundColor: CHART_COLORS.purple,
                    borderWidth: 1
                },
                {
                    label: 'מתנדבים',
                    data: volunteers,
                    fill: false,
                    borderColor: CHART_COLORS.orange,
                    backgroundColor: CHART_COLORS.orange,
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}