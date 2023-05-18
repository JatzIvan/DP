export function load_map() {
    let map = L.map('map').setView({ lon: 17.099710, lat: 48.138736}, 16);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);

    var drawnItems = new L.FeatureGroup();

    window.drawnItems = drawnItems;

    map.addLayer(drawnItems);

    var drawControl = new L.Control.Draw({
        edit: {
            featureGroup: drawnItems,
            edit: false,
            remove: false
        },
        draw: {
            polygon: false,
            polyline: false,
            circle: false,
            rectangle: {
                shapeOptions: {
                    color: '#424bf5',
                    fillOpacity: 0,
                    opacity: 1
                },
                tooltip: {
                    text: 'Draw a rectangle'
                }
            },
            marker: false
        },
    });
    map.addControl(drawControl);
    
    window.currentShape = null;
    map.on(L.Draw.Event.CREATED, function (event) {

        if (window.currentShape) {
            window.drawnItems.removeLayer(window.currentShape);
        }

        var layer = event.layer;
        window.currentShape = layer;
        window.drawnItems.addLayer(layer);

        var boundaryPoints = layer.getLatLngs();
        var event = new Event('change');

        document.getElementById("top").value = boundaryPoints[0][1].lat + "," + boundaryPoints[0][1].lng;
        document.getElementById("top").dispatchEvent(event);
        document.getElementById("bottom").value = boundaryPoints[0][3].lat + "," + boundaryPoints[0][3].lng;
        document.getElementById("bottom").dispatchEvent(event);

        console.log(boundaryPoints);

    });

    window.overlay = L.layerGroup();
    window.map = map;
    window.overlay.addTo(map);

    return "";
}

export function drawRect(lon1, lat1, lon2, lat2) {
    var drawnRectangle = new L.Rectangle([[lat1, lon1], [lat2, lon2]], {
        color: '#424bf5',
        fillOpacity: 0,
        opacity: 1
    });
    window.currentShape = L.layerGroup();
    window.drawnItems.addLayer(window.currentShape);
    window.currentShape.addLayer(drawnRectangle)
}

export function zoom(lon, lat) {
    window.map.setView({ lon: lon, lat: lat }, 12.5);
    window.overlay.clearLayers();
}

export function clear_layers() {
    window.overlay.clearLayers();
}

export function addLine(lon1, lat1, lon2, lat2, curve, tolerance) {


    var pointA = new L.LatLng(lat1, lon1);
    var pointB = new L.LatLng(lat2, lon2);
    var pointList = [pointA, pointB];

    window.overlay.addLayer(L.polyline(pointList, { color: curve < tolerance ? 'green' : 'red' }))
}