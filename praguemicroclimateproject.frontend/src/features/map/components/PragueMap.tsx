import { MapContainer, TileLayer } from "react-leaflet";
import type { LatLngExpression } from "leaflet";

const PRAGUE_CENTER: LatLngExpression = [50.0755, 14.4378];

function PragueMap() {
    return (
        <div style={{ height: "500px", width: "100%" }}>
            <MapContainer center={PRAGUE_CENTER} zoom={13} style={{ height: "100%", width: "100%" }}>
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
            </MapContainer>
        </div>
    );
}

export default PragueMap;