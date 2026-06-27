import { MapContainer, TileLayer, ZoomControl } from "react-leaflet";
import type {LatLngExpression } from "leaflet";

const PRAGUE_CENTER: LatLngExpression = [50.0755, 14.4378];

function PragueMap() {
    return (
        <div className="prague-map-shell">
            <MapContainer center={PRAGUE_CENTER} zoom={13} zoomControl={false} style={{ height: '100%', width: '100%' }}>
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <ZoomControl position="topright" />
            </MapContainer>
        </div>
    );
}

export default PragueMap;
