
$(document).ready(function () {

    //$("#resolveBTN").click(resolve);
    $("#readCitiesBTN").click(readCities);
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

function readCities() {

// get the cities from the server
    
    //names = ["חדרה", "עין שריד", "אין עיר כזו", "ג'לג'וליה"];
    //scb(names);


    let mode = $("#rewrite").val();
    let url = "WebService.asmx/getUnmappedCities";
    if (mode === "true") {
        url = "WebService.asmx/getCities";
    }


    $.ajax({
        dataType: "json",
        url: url,
        contentType: "application/json; charset=utf-8",
        type: "POST",
        async: false,
        data: "",
        success: scb,
        error: function (err) {
            alert("Error in getLocations: " + err.responseText);
        }
    });

}


function scb(data) {
    cities = JSON.parse(data.d);
    //names = ["גלעד"];
    $("#updateDBDiv").css("visibility", "visible");
    $("#ph").html(`got ${cities.length} cities`);
    numCities = cities.length;
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

    if (objBuffer.length === maxBuffer || index === numCities) {
        // writeToServer
        $("#updateMessage").html("updating...");
        let jsonData = { googleCities :  objBuffer };
        $.ajax({
            dataType: "json",
            url: "WebService.asmx/writeGoogleCities",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            async: true,
            data: JSON.stringify(jsonData),
            success: writeSCB,
            error: function (err) {
                alert("Error in writeCities: " + err.responseText);
            }
        });
        //writeSCB(objBuffer.length);
        objBuffer = [];
    }
}

function writeSCB(data) {
    let num = JSON.parse(data.d);
    console.log(`****** wrote ${num} cities into the database *********`);
    $("#updateMessage").html(`wrote ${num} cities into the database`);
    if (stop) {
        clearInterval(h);
        alert("finished");
    }

}

function codeAddress() {
    if (stop)
        return;
    let address = cities[index].CityName;
    index++;

    geocoder.geocode({
            address: address,
            componentRestrictions: {
                country: "IL"
            }
        }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            let r = results[0];
            if (r.formatted_address !== "Israel") {
                let googleObject = {
                    CityName: address,
                    Lat: r.geometry.location.lat(),
                    Lng: r.geometry.location.lng()
                };

                console.log(googleObject);

                buildGoogleObject(address, googleObject);
            }

            else {
                console.log(`${address} was not found`);
            }

        } else {
            console.log(address, 'Geocode was not successful for the following reason: ' + status);
            $("#errors").append(`<p>${address}</p>`);
        }
        $("#counter").html(`${index}/${numCities}`);
    });

    if (index === numCities) {
        stop = true;
    }
}
