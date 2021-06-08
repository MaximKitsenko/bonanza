DROP TABLE es_events;

CREATE TABLE IF NOT EXISTS es_events (
  id SERIAL,
  name VARCHAR (50) NOT NULL,
  version INT NOT NULL,
  data BYTEA NOT NULL
);