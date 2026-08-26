import type { ReactNode } from "react";
import type { MeasurementTypeOption } from "../types";
import { getSurfaceLabel } from "../utils/microclimateMap";

type MapFiltersProps = {
    measurementOptions: MeasurementTypeOption[];
    selectedMeasurements: string[];
    selectedSurfaces: string[];
    surfaceOptions: string[];
    totalPointCount: number;
    visiblePointCount: number;
    onMeasurementToggle: (measurement: string) => void;
    onSurfaceToggle: (surface: string) => void;
    onReset: () => void;
};

function MapFilters({
    measurementOptions,
    selectedMeasurements,
    selectedSurfaces,
    surfaceOptions,
    totalPointCount,
    visiblePointCount,
    onMeasurementToggle,
    onSurfaceToggle,
    onReset,
}: MapFiltersProps) {
    const hasFilters = selectedMeasurements.length > 0 || selectedSurfaces.length > 0;

    return (
        <aside className="map-control-panel map-filter-panel" aria-label="Map filters">
            <div className="panel-header">
                <div>
                    <h1>Microclimate map</h1>
                    <p>{visiblePointCount} of {totalPointCount} points visible</p>
                </div>
                <button className="text-button" type="button" onClick={onReset} disabled={!hasFilters}>
                    Reset
                </button>
            </div>

            <FilterGroup title="Measurements">
                {measurementOptions.length === 0 ? (
                    <p className="empty-panel-text">No measurements available.</p>
                ) : (
                    measurementOptions.map((measurement) => (
                        <label className="check-row" key={measurement.code}>
                            <input
                                checked={selectedMeasurements.includes(measurement.code)}
                                type="checkbox"
                                onChange={() => onMeasurementToggle(measurement.code)}
                            />
                            <span>{measurement.label}</span>
                        </label>
                    ))
                )}
            </FilterGroup>

            <FilterGroup title="Surfaces">
                {surfaceOptions.length === 0 ? (
                    <p className="empty-panel-text">No surfaces available.</p>
                ) : (
                    surfaceOptions.map((surface) => (
                        <label className="check-row" key={surface}>
                            <input
                                checked={selectedSurfaces.includes(surface)}
                                type="checkbox"
                                onChange={() => onSurfaceToggle(surface)}
                            />
                            <span>{getSurfaceLabel(surface)}</span>
                        </label>
                    ))
                )}
            </FilterGroup>
        </aside>
    );
}

type FilterGroupProps = {
    children: ReactNode;
    title: string;
};

function FilterGroup({ children, title }: FilterGroupProps) {
    return (
        <section className="filter-group">
            <h2>{title}</h2>
            <div className="filter-options">{children}</div>
        </section>
    );
}

export default MapFilters;
