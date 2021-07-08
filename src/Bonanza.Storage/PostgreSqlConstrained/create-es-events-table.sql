/*
CREATE DATABASE "bonanza-dev-db"
    WITH 
    OWNER = root
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;
*/
-------------------------------------

DROP TABLE IF EXISTS pk_event_sequencenum;
DROP TABLE IF EXISTS uk_event_streamid_version;
DROP TABLE IF EXISTS es_events_constrained;
-- dont format these lines it's space-sensitive
CREATE TABLE IF NOT EXISTS es_events_constrained (id bigserial, tenantid INT NOT NULL, name VARCHAR (50) NOT NULL, version INT NOT NULL, data BYTEA NOT NULL, CONSTRAINT pk_event_sequencenum PRIMARY KEY (TenantId, id), CONSTRAINT uk_event_streamid_version UNIQUE (tenantid, name, version)) PARTITION BY RANGE(tenantid);

CREATE TABLE es_events_constrained_000_005 PARTITION OF es_events_constrained FOR VALUES FROM (MINVALUE) TO (5);
CREATE TABLE es_events_constrained_005_010 PARTITION OF es_events_constrained FOR VALUES FROM (5) TO (10);
CREATE TABLE es_events_constrained_010_015 PARTITION OF es_events_constrained FOR VALUES FROM (10) TO (15);
CREATE TABLE es_events_constrained_015_020 PARTITION OF es_events_constrained FOR VALUES FROM (15) TO (20);
CREATE TABLE es_events_constrained_020_025 PARTITION OF es_events_constrained FOR VALUES FROM (20) TO (25);
CREATE TABLE es_events_constrained_025_030 PARTITION OF es_events_constrained FOR VALUES FROM (25) TO (30);
CREATE TABLE es_events_constrained_030_035 PARTITION OF es_events_constrained FOR VALUES FROM (30) TO (35);
CREATE TABLE es_events_constrained_035_040 PARTITION OF es_events_constrained FOR VALUES FROM (35) TO (40);
CREATE TABLE es_events_constrained_040_045 PARTITION OF es_events_constrained FOR VALUES FROM (40) TO (45);
CREATE TABLE es_events_constrained_045_050 PARTITION OF es_events_constrained FOR VALUES FROM (45) TO (50);
CREATE TABLE es_events_constrained_050_055 PARTITION OF es_events_constrained FOR VALUES FROM (50) TO (55);
CREATE TABLE es_events_constrained_055_060 PARTITION OF es_events_constrained FOR VALUES FROM (55) TO (60);
CREATE TABLE es_events_constrained_060_065 PARTITION OF es_events_constrained FOR VALUES FROM (60) TO (65);
CREATE TABLE es_events_constrained_065_070 PARTITION OF es_events_constrained FOR VALUES FROM (65) TO (70);
CREATE TABLE es_events_constrained_070_075 PARTITION OF es_events_constrained FOR VALUES FROM (70) TO (75);
CREATE TABLE es_events_constrained_075_080 PARTITION OF es_events_constrained FOR VALUES FROM (75) TO (80);

/*

Similarly we can add a new partition to handle new data. We can create an empty partition 
in the partitioned table just as the original partitions were created above:

CREATE TABLE measurement_y2008m02 PARTITION OF measurement
    FOR VALUES FROM ('2008-02-01') TO ('2008-03-01')
    TABLESPACE fasttablespace;

*/

-------------------------------------

CREATE INDEX IF NOT EXISTS "name_idx_000_005" ON es_events_constrained_000_005 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_005_010" ON es_events_constrained_005_010 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_010_015" ON es_events_constrained_010_015 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_015_020" ON es_events_constrained_015_020 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_020_025" ON es_events_constrained_020_025 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_025_030" ON es_events_constrained_025_030 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_030_035" ON es_events_constrained_030_035 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_035_040" ON es_events_constrained_035_040 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_040_045" ON es_events_constrained_040_045 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_045_050" ON es_events_constrained_045_050 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_050_055" ON es_events_constrained_050_055 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_055_060" ON es_events_constrained_055_060 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_060_065" ON es_events_constrained_060_065 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_065_070" ON es_events_constrained_065_070 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_070_075" ON es_events_constrained_070_075 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;
CREATE INDEX IF NOT EXISTS "name_idx_075_080" ON es_events_constrained_075_080 USING btree(name COLLATE pg_catalog."default" ASC NULLS LAST)TABLESPACE pg_default;