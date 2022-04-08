function wireModals() {
    $(document).on("click", ".regBTN", function () {
        selectedRide = allRides[$(this).attr("id")];
        verifyAvailability(selectedRide.RidePatNum);
    });

    $("#existsForm").submit(function () {
        init();
        return false;
    });

    $("#regForm").submit(function () {
        GetVolunteerByMobile();
        return false;
    });

    $("#nameConfForm").submit(function () {
        registerDriver();
        return false;
    });
}

function verifyAvailability(ridePatNum) {
    $.ajax({
        dataType: "json",
        url: "WebService.asmx/GetRidePat",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        data: JSON.stringify({ ridePatNum: ridePatNum }),
        success: function (data) {
            let status = (JSON.parse(data.d)).Status;
            exists = false;
            if (status.indexOf('שובץ נהג') == -1) { //false
                $("#regModal").modal();
                buidMessage(selectedRide);
            }
            else {
                exists = true;
                handleExisting();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function buidMessage(ride) {
    var when = "<p>" + ride.Date + ", בשעה " + ride.Time + "</p>";
    var where = "<p>" + " מ" + ride.OriginName + " ל" + ride.DestinationName + "</p>";
    var who = "<p>" + " של  " + ride.PatDisplayName + "</p>";
    var str = when + where + who;
    $("#approveMsg").html(str);
}

function handleExisting() {
    $("#existsModal").modal();
}

function GetVolunteerByMobile() {
    var request = {
        cellphone: $("#phoneTB").val()
    };
    var dataString = JSON.stringify(request);

    $.ajax({ 
        url: 'WebService.asmx/GetVolunteerByMobile',  
        data: dataString,                          
        type: 'POST',                            
        dataType: 'json',                          
        contentType: 'application/json; charset = utf-8',
        success: GetVolunteerByMobileSCB,                
        error: GetVolunteerByMobileErrorCB
    }); 

    return false;

}

function GetVolunteerByMobileSCB(data) {
    $('#regModal').modal('hide');
    var v = JSON.parse(data.d);
    var text = "הנהג " + v.DisplayName + " מעוניין להרשם להסעה";
    $("#nameConfirmationModal").modal();
    $("#nameConfMsg").html(text);
}

function registerDriver() {

    var request = {
        ridePatId: selectedRide.RidePatNum,
        mobile: $("#phoneTB").val(),
        fromDevice: "web"
    };

    var dataString = JSON.stringify(request);

    $.ajax({ // ajax call starts
        url: 'WebService.asmx/AssignRideToRidePatWithMobile',   // server side web service method
        data: dataString,                          // the parameters sent to the server
        type: 'POST',                              // can be also GET
        dataType: 'json',                          // expecting JSON datatype from the server
        contentType: 'application/json; charset = utf-8', // sent to the server
        success: registerDriverSuccessCB,                // data.d id the Variable data contains the data we get from serverside
        error: registerDriverErrorCB
    }); // end of ajax call

    return false;

}

function registerDriverSuccessCB() {
    setTimeout("init()", 2100);
    swal({
        title: "נרשמת לנסיעה,תודה רבה",
        type: "success",
        timer: 2000,
        showConfirmButton: false
    });
    $("#nameConfirmationModal").modal('hide');
}

function GetVolunteerByMobileErrorCB(err) {
    if (err.responseJSON.Message === "volunteer not found") {
        swal({
            title: "מספר הנייד שהזנת אינו משויך למתנדב פעיל",
            type: "warning",
            showConfirmButton: true
        });
        return;
    }

    if (err.responseJSON.Message === "not active or not driving or both") {
        swal({
            title: "המתנדב אינו פעיל/אינו נוהג או שניהם",
            type: "warning",
            showConfirmButton: true
        });
        return;
    }
    $('#regModal').modal('hide');
}