/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

/** Microclimate location response model. */
export interface Location {
  /** Address [example: "Pražská tržnice"] */
  address?: string | null;
  /** LocDescription [example: "areál bez zeleně"] */
  loc_description?: string | null;
  /** LocOrientation [example: "východ-západ"] */
  loc_orientation?: string | null;
  /** LocSurface [example: "asfalt/beton"] */
  loc_surface?: string | null;
  /** LocationName [example: "Pražská Holešovická tržnice"] */
  location?: string | null;
  /**
   * LocationId
   * @format int32
   */
  location_id?: number;
  /** Points */
  points?: LocationPoint[] | null;
}

/** Microclimate location point response model. */
export interface LocationPoint {
  /**
   * PointId
   * @format int32
   */
  point_id?: number;
  /** PointName [example: "Pražská tržnice osvětlení"] */
  point_name?: string | null;
}

/** Microclimate measurement response model. */
export interface Measurement {
  /**
   * LocationId
   * @format double
   */
  location_id?: number | null;
  /** Measure [example: "air_temp200"] */
  measure?: string | null;
  /**
   * MeasuredAt [example: "2022-08-21T17:30:00.000Z"]
   * @format date-time
   */
  measured_at?: string | null;
  /**
   * PointId
   * @format double
   */
  point_id?: number | null;
  /** Unit [example: "°C"] */
  unit?: string | null;
  /**
   * Value
   * @format double
   */
  value?: number | null;
}

export type MicroclimateLocationsAndPointsListData = Point3[];

export type MicroclimateLocationsListData = Location[];

export type MicroclimatePointMeasurementsListData = Measurement[];

export interface MicroclimatePointMeasurementsListParams {
  /** @format date-time */
  from?: string;
  /** @format int32 */
  locationId?: number;
  measure?: string;
  /** @format int32 */
  pointId?: number;
  /** @format date-time */
  to?: string;
}

export type MicroclimatePointMeasurementsParallelListData = Measurement[];

export interface MicroclimatePointMeasurementsParallelListParams {
  /** @format date-time */
  from?: string;
  /** @format int32 */
  locationId?: number;
  measure?: string;
  /** @format int32 */
  pointId?: number;
  /** @format date-time */
  to?: string;
}

/** Microclimate point response model. */
export interface Point3 {
  /**
   * ElevationM
   * @format double
   */
  elevation_m?: number | null;
  /**
   * Lat
   * @format double
   */
  lat?: number | null;
  /**
   * Lng
   * @format double
   */
  lng?: number | null;
  /** LocDescription [example: "areál bez zeleně"] */
  loc_description?: string | null;
  /** LocOrientation [example: "východ-západ"] */
  loc_orientation?: string | null;
  /** LocSurface [example: "asfalt/beton"] */
  loc_surface?: string | null;
  /** LocationName [example: "Pražská Holešovická tržnice"] */
  location?: string | null;
  /**
   * LocationId
   * @format int32
   */
  location_id?: number;
  /** Measures */
  measures?: Point3Measure[] | null;
  /**
   * PointId
   * @format int32
   */
  point_id?: number;
  /** PointNamed [example: "Pražská tržnice osvětlení"] */
  point_named?: string | null;
  /** SensorPosition [example: "veřejné osvětlení"] */
  sensor_position?: string | null;
  /** SensorPositionDetail [example: "veřejné osvětlení ve středu Tržnice"] */
  sensor_position_detail?: string | null;
  /**
   * XJtsk
   * @format double
   */
  x_jtsk?: number | null;
  /**
   * YJtsk
   * @format double
   */
  y_jtsk?: number | null;
}

/** Microclimate point measure response model. */
export interface Point3Measure {
  /** Measure [example: "air_temp200"] */
  measure?: string | null;
  /** MeasureCz [example: "Teplota vzduchu, 200 cm"] */
  measure_cz?: string | null;
  /** Unit [example: "°C"] */
  unit?: string | null;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<
  FullRequestParams,
  "body" | "method" | "query" | "path"
>;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown>
  extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  JsonApi = "application/vnd.api+json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) =>
    fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter(
      (key) => "undefined" !== typeof query[key],
    );
    return keys
      .map((key) =>
        Array.isArray(query[key])
          ? this.addArrayQueryParam(query, key)
          : this.addQueryParam(query, key),
      )
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string")
        ? JSON.stringify(input)
        : input,
    [ContentType.JsonApi]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string")
        ? JSON.stringify(input)
        : input,
    [ContentType.Text]: (input: any) =>
      input !== null && typeof input !== "string"
        ? JSON.stringify(input)
        : input,
    [ContentType.FormData]: (input: any) => {
      if (input instanceof FormData) {
        return input;
      }

      return Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
              ? JSON.stringify(property)
              : `${property}`,
        );
        return formData;
      }, new FormData());
    },
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(
    params1: RequestParams,
    params2?: RequestParams,
  ): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (
    cancelToken: CancelToken,
  ): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(
      `${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`,
      {
        ...requestParams,
        headers: {
          ...(requestParams.headers || {}),
          ...(type && type !== ContentType.FormData
            ? { "Content-Type": type }
            : {}),
        },
        signal:
          (cancelToken
            ? this.createAbortSignal(cancelToken)
            : requestParams.signal) || null,
        body:
          typeof body === "undefined" || body === null
            ? null
            : payloadFormatter(body),
      },
    ).then(async (response) => {
      const r = response as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const responseToParse = responseFormat ? response.clone() : response;
      const data = !responseFormat
        ? r
        : await responseToParse[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title PragueMicroclimateProject
 * @version 1.0
 */
export class Api<SecurityDataType extends unknown> {
  http: HttpClient<SecurityDataType>;

  constructor(http: HttpClient<SecurityDataType>) {
    this.http = http;
  }

  api = {
    /**
     * No description
     *
     * @tags Microclimate
     * @name MicroclimateLocationsAndPointsList
     * @summary Get ALL microclimate locations and points.
     * @request GET:/api/Microclimate/locationsAndPoints
     */
    microclimateLocationsAndPointsList: (params: RequestParams = {}) =>
      this.http.request<MicroclimateLocationsAndPointsListData, any>({
        path: `/api/Microclimate/locationsAndPoints`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Microclimate
     * @name MicroclimateLocationsList
     * @summary Get ALL microclimate locations.
     * @request GET:/api/Microclimate/locations
     */
    microclimateLocationsList: (params: RequestParams = {}) =>
      this.http.request<MicroclimateLocationsListData, any>({
        path: `/api/Microclimate/locations`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Microclimate
     * @name MicroclimatePointMeasurementsList
     * @summary Get point measurements.
     * @request GET:/api/Microclimate/pointMeasurements
     */
    microclimatePointMeasurementsList: (
      query: MicroclimatePointMeasurementsListParams = {},
      params: RequestParams = {},
    ) =>
      this.http.request<MicroclimatePointMeasurementsListData, any>({
        path: `/api/Microclimate/pointMeasurements`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Microclimate
     * @name MicroclimatePointMeasurementsParallelList
     * @summary Get point measurements.
     * @request GET:/api/Microclimate/pointMeasurementsParallel
     */
    microclimatePointMeasurementsParallelList: (
      query: MicroclimatePointMeasurementsParallelListParams = {},
      params: RequestParams = {},
    ) =>
      this.http.request<MicroclimatePointMeasurementsParallelListData, any>({
        path: `/api/Microclimate/pointMeasurementsParallel`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),
  };
}
