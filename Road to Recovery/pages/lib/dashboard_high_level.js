// Purpose: Dashboard UI for Amuta

// next step - first note in GitHub Project Trello orad - Dashboard column.


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
    start_daily_cards();
    start_monthly_cards();
}

function start_daily_cards() {
    start_current_day_row();
}

// Initiate async ajax call. When call finishes, invoke card's on_data callback
function start_current_day_row() {

    // @@ 
    let hack_day = new Date(2021, 09, 12); // 12-Oct-2020
    var query_object = {
        start_date: moment(hack_day).format('YYYY-MM-DD'),
        end_date: moment(hack_day).add(1, 'days').format('YYYY-MM-DD')
    }

    start_current_daily_totals(query_object);

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

    // Hack alert, using last year data as TEST DB is empty
    //        dateObj.setFullYear(dateObj.getFullYear() - 1);

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

    // Hack alert, using last year data as TEST DB is empty
    //      dateObj.setFullYear(dateObj.getFullYear() - 1);

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
        designator: "curr"
    },
    {
        designator: "prev"
    },
    {
        designator: "yoy"
    }

];


function start_monthly_cards() {

    for (const card_def of month_card_definitions) {
        start_one_month_row(card_def);
    }

   start_month_graph();

   for (const card_def of month_card_definitions) {
        start_one_month_new_volunteers(card_def);
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



function start_month_graph() {

    var query_object = get_month_range("curr");

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
            result = data.d;
            render_month_graph(result);
        },
        error: function (err) {
        }


    });
}

function render_month_graph(data) {

    let labels = data.map(function (obj) { return obj.Day; });
    let rides = data.map(function (obj) { return obj.Rides; });
    let volunteers = data.map(function (obj) { return obj.Volunteers; });
    let patients = data.map(function (obj) { return obj.Patients; });

    var ctx = document.getElementById('dsb_hl_monthly_graph').getContext('2d');
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