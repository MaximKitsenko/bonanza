CREATE DATABASE "bonanza-dev-db"
    WITH 
    OWNER = root
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    CONNECTION LIMIT = -1;
-------------------------------------
CREATE EXTENSION IF NOT EXISTS timescaledb;
-------------------------------------
DROP TABLE IF EXISTS es_events;
-------------------------------------
-- The 'time' column used in the create_hypertable 
-- function supports timestamp, date, or integer types, 
-- so you can use a parameter that is not explicitly 
-- time-based, as long as it can increment. For example, 
-- a monotonically increasing id would work.


-- dont format these lines it's space-sensitive

CREATE TABLE es_events (id SERIAL NOT NULL, name TEXT NOT NULL, version INT NOT NULL, data BYTEA NOT NULL);
-------------------------------------
SELECT create_hypertable('es_events', 'id',chunk_time_interval => 100000);
-------------------------------------