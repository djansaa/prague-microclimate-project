import { useEffect, useMemo, useState } from "react";
import type { LatLngExpression } from "leaflet";
import { CircleMarker, MapContainer, TileLayer, ZoomControl } from "react-leaflet";
import { Api, HttpClient, type Location } from "../../microclimate/api";
import MapFilters from "./MapFilters";
import PointDetailsPanel from "./PointDetailsPanel";
import type { MapPoint, MeasurementQuery, MeasurementResult } from "../types";
import {
    flattenLocations,
    getMeasurementOptions,
    getMeasurementUnit,
    getSurfaceOptions,
} from "../utils/microclimateMap";

const PRAGUE_CENTER: LatLngExpression = [50.0755, 14.4378];
const microclimateApi = new Api(new HttpClient());

function PragueMap() {
    const [locations, setLocations] = useState<Location[]>([]);
    const [isLoadingLocations, setIsLoadingLocations] = useState(true);
    const [hasLocationError, setHasLocationError] = useState(false);
    const [selectedMeasurements, setSelectedMeasurements] = useState<string[]>([]);
    const [selectedSurfaces, setSelectedSurfaces] = useState<string[]>([]);
    const [selectedPoint, setSelectedPoint] = useState<MapPoint | null>(null);
    const [measurementResult, setMeasurementResult] = useState<MeasurementResult | null>(null);
    const [isLoadingMeasurements, setIsLoadingMeasurements] = useState(false);
    const [measurementError, setMeasurementError] = useState("");

    useEffect(() => {
        let isMounted = true;

        async function loadLocationsAndPoints() {
            try {
                setIsLoadingLocations(true);
                setHasLocationError(false);

                const response = await microclimateApi.api.microclimateLocationsAndPointsList();

                if (isMounted) {
                    setLocations(response.data);
                }
            } catch {
                if (isMounted) {
                    setHasLocationError(true);
                }
            } finally {
                if (isMounted) {
                    setIsLoadingLocations(false);
                }
            }
        }

        void loadLocationsAndPoints();

        return () => {
            isMounted = false;
        };
    }, []);

    const points = useMemo(() => flattenLocations(locations), [locations]);
    const measurementOptions = useMemo(() => getMeasurementOptions(points), [points]);
    const surfaceOptions = useMemo(() => getSurfaceOptions(locations), [locations]);

    const visiblePoints = useMemo(
        () =>
            points.filter((point) => {
                const matchesMeasurements =
                    selectedMeasurements.length === 0 ||
                    selectedMeasurements.some((measurement) => point.measurementTypes?.includes(measurement));
                const matchesSurface =
                    selectedSurfaces.length === 0 ||
                    (point.locationSurface != null && selectedSurfaces.includes(point.locationSurface));

                return matchesMeasurements && matchesSurface;
            }),
        [points, selectedMeasurements, selectedSurfaces],
    );

    async function handleApplyMeasurementQuery(query: MeasurementQuery) {
        try {
            setIsLoadingMeasurements(true);
            setMeasurementError("");
            setMeasurementResult(null);

            const response = await microclimateApi.api.microclimatePointMeasurementsList({
                from: query.from,
                locationId: query.locationId,
                measure: query.measure,
                pointId: query.pointId,
                to: query.to,
            });

            setMeasurementResult({
                items: response.data,
                unit: getMeasurementUnit(response.data),
            });
        } catch (error) {
            setMeasurementError(getErrorMessage(error));
        } finally {
            setIsLoadingMeasurements(false);
        }
    }

    function handlePointSelect(point: MapPoint) {
        setSelectedPoint(point);
        setMeasurementResult(null);
        setMeasurementError("");
    }

    function handleMeasurementToggle(measurement: string) {
        setSelectedMeasurements((current) => toggleValue(current, measurement));
    }

    function handleSurfaceToggle(surface: string) {
        setSelectedSurfaces((current) => toggleValue(current, surface));
    }

    function handleResetFilters() {
        setSelectedMeasurements([]);
        setSelectedSurfaces([]);
    }

    return (
        <div className="prague-map-shell">
            <MapContainer center={PRAGUE_CENTER} zoom={13} zoomControl={false} className="prague-map">
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                {visiblePoints.map((point) => (
                    <CircleMarker
                        center={[point.latitude!, point.longitude!]}
                        eventHandlers={{
                            click: () => handlePointSelect(point),
                        }}
                        key={getPointKey(point)}
                        pathOptions={{
                            color: selectedPoint === point ? "#1d4ed8" : "#0f766e",
                            fillColor: selectedPoint === point ? "#3b82f6" : "#14b8a6",
                            fillOpacity: 0.86,
                            weight: selectedPoint === point ? 3 : 2,
                        }}
                        radius={selectedPoint === point ? 9 : 7}
                    />
                ))}
                <ZoomControl position="topright" />
            </MapContainer>

            <MapFilters
                measurementOptions={measurementOptions}
                selectedMeasurements={selectedMeasurements}
                selectedSurfaces={selectedSurfaces}
                surfaceOptions={surfaceOptions}
                totalPointCount={points.length}
                visiblePointCount={visiblePoints.length}
                onMeasurementToggle={handleMeasurementToggle}
                onReset={handleResetFilters}
                onSurfaceToggle={handleSurfaceToggle}
            />

            <PointDetailsPanel
                errorMessage={measurementError}
                isLoading={isLoadingMeasurements}
                key={selectedPoint ? getPointKey(selectedPoint) : "no-selected-point"}
                point={selectedPoint}
                result={measurementResult}
                onApply={handleApplyMeasurementQuery}
                onClose={() => setSelectedPoint(null)}
            />

            {(isLoadingLocations || hasLocationError) && (
                <div className={`map-status ${hasLocationError ? "map-status-error" : ""}`}>
                    {hasLocationError ? "Unable to load map points." : "Loading map points..."}
                </div>
            )}
        </div>
    );
}

function toggleValue(values: string[], value: string) {
    return values.includes(value) ? values.filter((item) => item !== value) : [...values, value];
}

function getPointKey(point: MapPoint) {
    return `${point.locationId ?? point.locationName ?? "location"}-${point.id ?? `${point.latitude}-${point.longitude}`}`;
}

function getErrorMessage(error: unknown) {
    if (error instanceof Error) {
        return error.message;
    }

    return "Unable to load measurements. Try a shorter range or another measurement.";
}

export default PragueMap;
