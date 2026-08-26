import { type FormEvent, useMemo, useState } from "react";
import MeasurementChart from "./MeasurementChart";
import type { ChartPoint, MapPoint, MeasurementQuery, MeasurementResult } from "../types";
import {
    formatMeasurementList,
    getDefaultDateRange,
    getMeasurementLabel,
    getSensorPositionLabel,
    getSurfaceLabel,
    getSupportedPointMeasurements,
    mapMeasurementsToChartPoints,
    toApiDateTime,
} from "../utils/microclimateMap";

type PointDetailsPanelProps = {
    errorMessage?: string;
    isLoading: boolean;
    point: MapPoint | null;
    result: MeasurementResult | null;
    onApply: (query: MeasurementQuery) => void;
    onClose: () => void;
};

function PointDetailsPanel({ errorMessage, isLoading, point, result, onApply, onClose }: PointDetailsPanelProps) {
    const supportedMeasurements = useMemo(() => getSupportedPointMeasurements(point), [point]);
    const [defaultRange] = useState(getDefaultDateRange);
    const [selectedMeasure, setSelectedMeasure] = useState(() => supportedMeasurements[0]?.code ?? "");
    const [from, setFrom] = useState(defaultRange.from);
    const [to, setTo] = useState(defaultRange.to);
    const [validationError, setValidationError] = useState("");

    const chartData = useMemo<ChartPoint[]>(
        () => (result ? mapMeasurementsToChartPoints(result.items) : []),
        [result],
    );

    if (!point) {
        return null;
    }

    const canSubmit = Boolean(selectedMeasure && from && to && !isLoading);
    const selectedMeasurementLabel = selectedMeasure ? getMeasurementLabel(selectedMeasure) : "Measurement";

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (!point) {
            return;
        }

        if (!selectedMeasure) {
            setValidationError("Choose a supported measurement type.");
            return;
        }

        if (new Date(from) > new Date(to)) {
            setValidationError("The start time must be before the end time.");
            return;
        }

        setValidationError("");
        onApply({
            from: toApiDateTime(from),
            locationId: point.locationId ?? undefined,
            measure: selectedMeasure,
            pointId: point.id ?? undefined,
            to: toApiDateTime(to),
        });
    }

    return (
        <aside className="map-control-panel point-details-panel" aria-label="Selected point details">
            <div className="panel-header">
                <div>
                    <h2>{point.name || `Point ${point.id ?? ""}`.trim()}</h2>
                    <p>{point.locationName || "Unknown location"}</p>
                </div>
                <button className="icon-button" type="button" onClick={onClose} aria-label="Close point details">
                    x
                </button>
            </div>

            <dl className="point-meta-grid">
                <div>
                    <dt>Surface</dt>
                    <dd>{point.locationSurface ? getSurfaceLabel(point.locationSurface) : "Unknown"}</dd>
                </div>
                <div>
                    <dt>Sensor position</dt>
                    <dd>{point.sensorPosition ? getSensorPositionLabel(point.sensorPosition) : "Unknown"}</dd>
                </div>
                <div className="point-meta-wide">
                    <dt>Measurements</dt>
                    <dd>{formatMeasurementList(point.measurementTypes)}</dd>
                </div>
            </dl>

            <form className="query-form" onSubmit={handleSubmit}>
                <label>
                    <span>Measurement</span>
                    <select
                        value={selectedMeasure}
                        onChange={(event) => setSelectedMeasure(event.target.value)}
                        disabled={supportedMeasurements.length === 0}
                    >
                        {supportedMeasurements.length === 0 ? (
                            <option value="">No supported measurements</option>
                        ) : (
                            supportedMeasurements.map((measurement) => (
                                <option key={measurement.code} value={measurement.code}>
                                    {measurement.label}
                                </option>
                            ))
                        )}
                    </select>
                </label>

                <div className="date-input-row">
                    <label>
                        <span>From</span>
                        <input
                            step={3600}
                            type="datetime-local"
                            value={from}
                            onChange={(event) => setFrom(event.target.value)}
                        />
                    </label>
                    <label>
                        <span>To</span>
                        <input
                            step={3600}
                            type="datetime-local"
                            value={to}
                            onChange={(event) => setTo(event.target.value)}
                        />
                    </label>
                </div>

                <button className="primary-button" disabled={!canSubmit} type="submit">
                    {isLoading ? "Loading..." : "Show chart"}
                </button>
            </form>

            {(validationError || errorMessage) && (
                <div className="inline-error" role="alert">
                    {validationError || errorMessage}
                </div>
            )}

            {result && (
                <MeasurementChart data={chartData} measurementLabel={selectedMeasurementLabel} unit={result.unit} />
            )}
        </aside>
    );
}

export default PointDetailsPanel;
