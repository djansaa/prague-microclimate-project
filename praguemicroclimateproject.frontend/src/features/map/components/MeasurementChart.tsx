import { CartesianGrid, Line, LineChart, Tooltip, XAxis, YAxis } from "recharts";
import type { ChartPoint } from "../types";
import { getChartWidth } from "../utils/microclimateMap";

type MeasurementChartProps = {
    data: ChartPoint[];
    measurementLabel: string;
    unit?: string;
};

function MeasurementChart({ data, measurementLabel, unit }: MeasurementChartProps) {
    if (data.length === 0) {
        return (
            <div className="chart-empty-state">
                <strong>No measurements found</strong>
                <span>Try another measurement type or time range.</span>
            </div>
        );
    }

    const chartWidth = getChartWidth(data.length);
    const yAxisLabel = unit ? `${measurementLabel} (${unit})` : measurementLabel;

    return (
        <div className="chart-section">
            <div className="chart-title-row">
                <h3>{measurementLabel}</h3>
                {unit && <span>{unit}</span>}
            </div>
            <div className="chart-scroll" aria-label="Measurement chart">
                <LineChart
                    data={data}
                    height={300}
                    margin={{ bottom: 28, left: 12, right: 24, top: 16 }}
                    width={chartWidth}
                >
                    <CartesianGrid stroke="#d7dee8" strokeDasharray="4 4" />
                    <XAxis
                        angle={-35}
                        dataKey="timeLabel"
                        height={70}
                        interval={0}
                        stroke="#526173"
                        textAnchor="end"
                        tick={{ fontSize: 12 }}
                    />
                    <YAxis
                        label={{ angle: -90, position: "insideLeft", value: yAxisLabel }}
                        stroke="#526173"
                        tick={{ fontSize: 12 }}
                        width={78}
                    />
                    <Tooltip formatter={(value) => formatTooltipValue(value, unit)} labelClassName="chart-tooltip-label" />
                    <Line
                        dataKey="value"
                        dot={{ r: 2 }}
                        isAnimationActive={false}
                        name={measurementLabel}
                        stroke="#0f766e"
                        strokeWidth={2}
                        type="monotone"
                    />
                </LineChart>
            </div>
        </div>
    );
}

function formatTooltipValue(value: unknown, unit?: string) {
    const formattedValue = typeof value === "number" ? value.toFixed(2) : String(value);
    return unit ? `${formattedValue} ${unit}` : formattedValue;
}

export default MeasurementChart;
