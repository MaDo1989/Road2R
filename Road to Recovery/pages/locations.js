
$(document).ready(function () {

    //$("#resolveBTN").click(resolve);
    $("#readLocationsBTN").click(readLocations);
    $("#updateBTN").click(update);
    objBuffer = [];
    maxBuffer = 10;
    stop = false;
});


function update() {
    index = 0;
    let timeInterval = parseInt($("#timeInterval").val());
    geocoder = new google.maps.Geocoder();
    codeAddress();
    h = setInterval(codeAddress, timeInterval);
}

function readLocations() {

// get the cities from the server
    
    //names = ["חדרה", "עין שריד", "אין עיר כזו", "ג'לג'וליה"];
    //scb(names);

    let locType = $("#locationTypeDDL").val();

    $.ajax({
        dataType: "json",
        url: "WebService.asmx/readLocationsNamesByType",
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: false,
        data: JSON.stringify({ locationType: locType }),
        success: scb,
        error: function (err) {
            alert("Error in readLocationsNamesByType: " + err.responseText);
        }
    });

}


function scb(data) {
    names = JSON.parse(data.d);
    console.log(names);
    //names = ["סנט ג'ון", "איכילוב"];
    $("#updateDBDiv").css("visibility", "visible");
    $("#ph").html(`got ${names.length} locations`);
    numLocations = names.length;
}


function ecb(err) {
    alert("error from the server");
}


        function resolve() {
            let city = document.getElementById("cityNameTB").value;
            codeAddress(city);
}

function buildGoogleObject(name, googleObject) {

    objBuffer.push(googleObject);

    if (objBuffer.length === maxBuffer || index === numLocations) {
        // writeToServer
        $("#updateMessage").html("updating...");
        let jsonData = { googleLocations :  objBuffer };
        $.ajax({
            dataType: "json",
            url: "WebService.asmx/writeGoogleLocations",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            async: true,
            data: JSON.stringify(jsonData),
            success: writeSCB,
            error: function (err) {
                alert("Error in writeGoogleLocations: " + err.responseText);
            }
        });
        //writeSCB(objBuffer.length);
        objBuffer = [];
    }
}

function writeSCB(data) {
    let num = JSON.parse(data.d);
    console.log(`****** wrote ${num} locations into the database *********`);
    $("#updateMessage").html(`wrote ${num} locations into the database`);
    if (stop) {
        clearInterval(h);
        alert("finished");
    }

}

function codeAddress() {
    if (stop)
        return;
    let address = names[index];
    index++;

    let country = $("#countryDDL").val();

    geocoder.geocode({
            address: address,
            componentRestrictions: {
                country: country
            }
        }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            let r = results[0];
            if (r.formatted_address !== "Israel") {
                let googleObject = {
                    Name: address,
                    Lat: r.geometry.location.lat(),
                    Lng: r.geometry.location.lng()
                };

                console.log(googleObject);

                buildGoogleObject(address, googleObject);
            }
            else
                console.log(`${address} was not detected`);

        } else {
            console.log(address, 'Geocode was not successful for the following reason: ' + status);
            $("#errors").append(`<p>${address}</p>`);
        }
            $("#counter").html(`${index}/${numLocations}`);
    });


    if (index === numLocations) {
        stop = true;
    }
}
