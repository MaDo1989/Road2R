// Purpose: Dashboard UI for Amuta


function dashboard_hl_init() {
    $("#reports_content_div").hide();
    $("#dsb_hl_content_div").show();
    start_cards();
}

function start_cards() {
    // display_demo_card();

    for (const card_def of card_definitions) {
        start_one_card(card_def);
    }
}

// Initiate async ajax call. When call finishes, invoke card's on_data callback
function start_one_card(card_def) {
    var query_object = {
        metric_name: card_def.name,
        start_date1: "01-01-2020",
        end_date1: "31-12-2020",
        start_date2: "01-01-2020",
        end_date2: "31-12-2020"
    };

    $.ajax({
        dataType: "json",
        url: "ReportsWebService.asmx/GetReportMetrics",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify(query_object),
        success: function (data) {
            // $('#wait').hide();
            result = data.d;
            card_def.render(card_def, result);
        },
        error: function (err) {
            // $('#wait').hide();
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