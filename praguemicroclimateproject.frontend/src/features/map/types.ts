import type { Measurement, Point } from "../microclimate/api";

export type MapPoint = Point & {
    locationId?: number | null;
    locationName?: string | null;
    locationSurface?: string | null;
};

export type MeasurementTypeOption = {
    code: string;
    label: string;
};

export type MeasurementQuery = {
    locationId?: number;
    pointId?: number;
    measure: string;
    from: string;
    to: string;
};

export type ChartPoint = {
    timestamp: string;
    timeLabel: string;
    value: number;
};

export type MeasurementResult = {
    items: Measurement[];
    unit?: string;
};
