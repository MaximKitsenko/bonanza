CREATE DATABASE "bonanza-dev-db"
    WITH 
    OWNER = root
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;

-------------------------------------

DROP TABLE IF EXISTS es_events;
-- dont format these lines it's space-sensitive
CREATE TABLE IF NOT EXISTS es_events (id SERIAL,name VARCHAR (50) NOT NULL,version INT NOT NULL,data BYTEA NOT NULL);

-------------------------------------
-- Query returned successfully in 37 min 44 secs.
-- on 300M table
CREATE INDEX IF NOT EXISTS "id-idx" ON public.es_events 
USING btree(id)TABLESPACE pg_default;
--