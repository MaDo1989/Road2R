let map;

function initMap() {
    geocoder = new google.maps.Geocoder();
    map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 31.9461, lng: 34.8516 },
        zoom: 9.3
    });


}