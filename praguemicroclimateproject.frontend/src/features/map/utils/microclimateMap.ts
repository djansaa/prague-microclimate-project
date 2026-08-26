import type { Location, Measurement } from "../../microclimate/api";
import type { ChartPoint, MapPoint, MeasurementTypeOption } from "../types";

export const SUPPORTED_MEASUREMENTS: MeasurementTypeOption[] = [
    { code: "AIR_TEMPERATURE", label: "Air temperature" },
    { code: "AIR_HUMIDITY", label: "Air humidity" },
    { code: "WIND_SPEED", label: "Wind speed" },
];

const MEASUREMENT_LABELS = new Map(SUPPORTED_MEASUREMENTS.map((measurement) => [measurement.code, measurement.label]));
const SURFACE_LABELS = new Map([
    ["ASPHALT", "Asphalt"],
    ["ASPHALT_CONCRETE", "Asphalt / concrete"],
    ["ASPHALT_PAVING_STONES", "Asphalt / paving stones"],
    ["ASPHALT_COMPACTED_GRAVEL_GREENERY", "Asphalt / compacted gravel / greenery"],
    ["ASPHALT_GREENERY", "Asphalt / greenery"],
    ["PAVING_STONES", "Paving stones"],
    ["ARABLE_LAND_GREENERY", "Arable land / greenery"],
    ["GREENERY", "Greenery"],
    ["GREENERY_COMPACTED_GRAVEL", "Greenery / compacted gravel"],
]);
const SENSOR_POSITION_LABELS = new Map([
    ["LIGHTING", "Lighting"],
    ["WALL", "Wall"],
    ["PUBLIC_LIGHTING", "Public lighting"],
    ["POLE", "Pole"],
    ["HIGH_VOLTAGE_POWER_LINE_POLE", "High-voltage power line pole"],
]);
const CHART_HOUR_WIDTH = 56;
const MIN_CHART_WIDTH = 720;

export function flattenLocations(locations: Location[]): MapPoint[] {
    return locations.flatMap((location) =>
        (location.points ?? [])
            .filter((point) => point.latitude != null && point.longitude != null)
            .map((point) => ({
                ...point,
                locationId: location.id,
                locationName: location.name,
                locationSurface: location.surface,
            })),
    );
}

export function getMeasurementOptions(points: MapPoint[]): MeasurementTypeOption[] {
    const measurementCodes = new Set<string>();

    points.forEach((point) => {
        point.measurementTypes?.forEach((measurementType) => measurementCodes.add(measurementType));
    });

    return [...measurementCodes]
        .sort((first, second) => getMeasurementLabel(first).localeCompare(getMeasurementLabel(second)))
        .map((code) => ({ code, label: getMeasurementLabel(code) }));
}

export function getSurfaceOptions(locations: Location[]): string[] {
    const surfaces = new Set<string>();

    locations.forEach((location) => {
        if (location.surface?.trim()) {
            surfaces.add(location.surface.trim());
        }
    });

    return [...surfaces].sort((first, second) => getSurfaceLabel(first).localeCompare(getSurfaceLabel(second)));
}

export function getSupportedPointMeasurements(point?: MapPoint | null): MeasurementTypeOption[] {
    if (!point?.measurementTypes) {
        return [];
    }

    return SUPPORTED_MEASUREMENTS.filter((measurement) => point.measurementTypes?.includes(measurement.code));
}

export function getMeasurementLabel(code: string) {
    return MEASUREMENT_LABELS.get(code) ?? formatCodeLabel(code);
}

export function getSurfaceLabel(code: string) {
    return SURFACE_LABELS.get(code) ?? formatCodeLabel(code);
}

export function getSensorPositionLabel(code: string) {
    return SENSOR_POSITION_LABELS.get(code) ?? formatCodeLabel(code);
}

export function formatMeasurementList(measurementTypes?: string[] | null) {
    if (!measurementTypes || measurementTypes.length === 0) {
        return "No measurements listed";
    }

    return measurementTypes.map(getMeasurementLabel).join(", ");
}

export function toDateTimeLocalValue(date: Date) {
    const offsetDate = new Date(date.getTime() - date.getTimezoneOffset() * 60_000);
    return offsetDate.toISOString().slice(0, 16);
}

export function toApiDateTime(dateTimeLocal: string) {
    return new Date(dateTimeLocal).toISOString();
}

export function getDefaultDateRange() {
    const to = roundDownToHour(new Date());
    const from = new Date(to);
    from.setHours(from.getHours() - 24);

    return {
        from: toDateTimeLocalValue(from),
        to: toDateTimeLocalValue(to),
    };
}

export function mapMeasurementsToChartPoints(measurements: Measurement[]): ChartPoint[] {
    return measurements
        .filter((measurement) => measurement.timestamp && measurement.value != null)
        .map((measurement) => ({
            timestamp: measurement.timestamp!,
            timeLabel: formatChartTime(measurement.timestamp!),
            value: measurement.value!,
        }))
        .sort((first, second) => new Date(first.timestamp).getTime() - new Date(second.timestamp).getTime());
}

export function getChartWidth(pointCount: number) {
    return Math.max(MIN_CHART_WIDTH, pointCount * CHART_HOUR_WIDTH);
}

export function getMeasurementUnit(measurements: Measurement[]) {
    return measurements.find((measurement) => measurement.unit)?.unit ?? undefined;
}

function roundDownToHour(date: Date) {
    const roundedDate = new Date(date);
    roundedDate.setMinutes(0, 0, 0);
    return roundedDate;
}

function formatChartTime(timestamp: string) {
    return new Intl.DateTimeFormat("en", {
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
    }).format(new Date(timestamp));
}

function formatCodeLabel(code: string) {
    return code
        .toLowerCase()
        .split("_")
        .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
        .join(" ");
}
