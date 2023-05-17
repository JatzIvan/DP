export function load_map() {
    let map = L.map('map').setView({ lon: 17.099710, lat: 48.138736}, 16);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);

    var drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);
    var drawControl = new L.Control.Draw({
        edit: {
            featureGroup: drawnItems,
            edit: false, // Disable default edit toolbar
            remove: false // Disable default remove toolbar
        },
        draw: {
            polygon: false, // Enable polygon drawing mode
            polyline: false,
            circle: false,
            rectangle: true,
            marker: false
        },
    });
    map.addControl(drawControl);

    var currentShape = null;
    map.on(L.Draw.Event.CREATED, function (event) {

        if (currentShape) {
            drawnItems.removeLayer(currentShape);
        }

        var layer = event.layer;
        currentShape = layer;
        drawnItems.addLayer(layer);

        var boundaryPoints = layer.getLatLngs(); // Retrieve the boundary points of the drawn polygon
        var event = new Event('change');

        document.getElementById("top").value = boundaryPoints[0][1].lat + "," + boundaryPoints[0][1].lng;
        document.getElementById("top").dispatchEvent(event);
        document.getElementById("bottom").value = boundaryPoints[0][3].lat + "," + boundaryPoints[0][3].lng;
        document.getElementById("bottom").dispatchEvent(event);

        console.log(boundaryPoints); // Display the boundary points in the console

    });

    window.overlay = L.layerGroup();
    window.map = map;
    window.overlay.addTo(map);

    return "";
}

export function zoom(lon, lat) {
    window.map.setView({ lon: lon, lat: lat }, 16);
    window.overlay.clearLayers();
}

export function addLine(lon1, lat1, lon2, lat2, curve, tolerance) {


    var pointA = new L.LatLng(lat1, lon1);
    var pointB = new L.LatLng(lat2, lon2);
    var pointList = [pointA, pointB];

    window.overlay.addLayer(L.polyline(pointList, { color: curve < tolerance ? 'green' : 'red' }))
}