# Local Elastic Stack

This directory contains the local Docker Desktop setup for Elasticsearch, Logstash, and Kibana.

## Services

- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601
- Logstash HTTP input: http://localhost:8080
- Logstash monitoring API: http://localhost:9600

## Credentials

The default local credentials are stored in `.env` and are for development only.

- Elasticsearch user: `elastic`
- Elasticsearch password: `pragueelastic`
- Kibana system user: `kibana_system`
- Kibana system password: `praguekibana`

Use the `elastic` user and `ELASTIC_PASSWORD` value from `.env` to sign in to Kibana.

## Usage

From this directory:

```powershell
docker compose up -d
```

To stop the stack while keeping data volumes:

```powershell
docker compose down
```

To remove containers and data volumes:

```powershell
docker compose down -v
```

The application posts Serilog batches to Logstash when `Logstash:RequestUri` is configured. The local development value is `http://localhost:8080`.
