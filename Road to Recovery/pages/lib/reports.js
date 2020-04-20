// Purpose: JS code for the reports UI


// TODO: find some names (via dbveawer ) that should have old rides
// TODO:  add mont UI, and respect in service.


// Handle a click event on one of the reports in the Reports-Tree
function on_report_click(event) {
    var report_type = event.target.id;
    populate_parameters(report_type);
   

}


function populate_parameters(report_type) {
    $("#params_ph").text("Populating for " + report_type);
    var fields = rp_get_fields(report_type);
    if (fields) {
        fields.forEach(rp_add_one_parameter);
    }
}

// returns a list of fields per a report type
function rp_get_fields(report_type) {

    return [
        {
            id: "rp_vl_ride_month__name",
            type: "VOLUNTEER",
            template: 'div[name="template_VOLUNTEER"]',
            post_clone: field_volunteers_post_clone
        },
        {
            id: "rp_vl_ride_month__month",
            type: "MONTH",
            post_clone: null
        }
    ];

}

// Makes sure we remove name from clone and set an id
function clone_template(template, new_id) {
    var result = $(template).clone().prop("id", new_id);
    result.removeAttr("name");
    return result;
}

var K_CACHE = { volunteers: [] };



// on_load_volunteers called when the async ajax call  has finished
// Used to populate UI needing the volunteers list
function loadVolunteers(on_load_volunteers) {

    if (K_CACHE.volunteers.length > 1) {
        // One time loading already done.
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
            on_load_volunteers();
        },
        error: function (err) { alert("Error in loadVolunteers"); }
    });
}

// Called when a volunteer is selected in auto-complete
function on_volunteer_selected(event, ui) {
    refreshTable(ui.item.id);
    return true;
}
// called when the async ajax call to laod volunteers has finished
// Used to populate UI needing the volunteers list
function populate_volunteer_field() {
    $("#driver").autocomplete({
        source: K_CACHE.volunteers,
        select: on_volunteer_selected
        });
}

// Given a defintition of field  build the UI for it in the Parameters panel
// The UI is coped from a template in the page. Each field type has its own template
function rp_add_one_parameter(def) {
    $("#params_ph").append($("<div>", { id: def.id }));
    var field_elem = $("#" + def.id);
    field_elem.append(def.label + " XYZ");
    var clone = clone_template(def.template, "KKK");
    clone.appendTo("#" + def.id);
    if (def.post_clone) {
        def.post_clone(def.id);
    }
}

function field_volunteers_post_clone(id) {
    loadVolunteers(populate_volunteer_field);
}


function refreshTable(volunteerId) {
    ridesToShow = [];
    allRides = [];

    $('#wait').show();
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetReportVolunteerRides",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Content-Encoding", "gzip");
        },
        type: "POST",
        data: JSON.stringify({ volunteerId: volunteerId, maxDays: -2 }),// -2 means this week rides
        success: function (data) {
            $('#wait').hide();
            arr_rides = JSON.parse(data.d);
            console.log(arr_rides);

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
                    { data: "OriginName" },
                    { data: "DestinationName" },
                    { data: "Time" },
                    { data: "PatDisplayName" },
                    { data: "Coordinator" },
                    { data: "Drivers" },

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
