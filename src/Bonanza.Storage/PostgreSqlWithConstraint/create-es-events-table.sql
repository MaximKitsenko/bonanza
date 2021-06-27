CREATE DATABASE "bonanza-dev-db"
    WITH 
    OWNER = root
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;

-------------------------------------

DROP TABLE IF EXISTS es_events;
-- dont format these lines it's space-sensitive
CREATE TABLE IF NOT EXISTS es_events (id SERIAL, tenantid INT NOT NULL,name VARCHAR (50) NOT NULL,version INT NOT NULL,data BYTEA NOT NULL) PARTITION BY RANGE(tenantid);

CREATE TABLE es_events_000_005 PARTITION OF es_events FOR VALUES FROM (MINVALUE) TO (5);
CREATE TABLE es_events_005_010 PARTITION OF es_events FOR VALUES FROM (5) TO (10);
CREATE TABLE es_events_010_015 PARTITION OF es_events FOR VALUES FROM (10) TO (15);
CREATE TABLE es_events_015_020 PARTITION OF es_events FOR VALUES FROM (15) TO (20);
CREATE TABLE es_events_020_025 PARTITION OF es_events FOR VALUES FROM (20) TO (25);
CREATE TABLE es_events_025_030 PARTITION OF es_events FOR VALUES FROM (25) TO (30);
CREATE TABLE es_events_030_035 PARTITION OF es_events FOR VALUES FROM (30) TO (35);
CREATE TABLE es_events_035_040 PARTITION OF es_events FOR VALUES FROM (35) TO (40);
CREATE TABLE es_events_040_045 PARTITION OF es_events FOR VALUES FROM (40) TO (45);
CREATE TABLE es_events_045_050 PARTITION OF es_events FOR VALUES FROM (45) TO (50);
CREATE TABLE es_events_050_055 PARTITION OF es_events FOR VALUES FROM (50) TO (55);
CREATE TABLE es_events_055_060 PARTITION OF es_events FOR VALUES FROM (55) TO (60);
CREATE TABLE es_events_060_065 PARTITION OF es_events FOR VALUES FROM (60) TO (65);
CREATE TABLE es_events_065_070 PARTITION OF es_events FOR VALUES FROM (65) TO (70);
CREATE TABLE es_events_070_075 PARTITION OF es_events FOR VALUES FROM (70) TO (75);
CREATE TABLE es_events_075_080 PARTITION OF es_events FOR VALUES FROM (75) TO (80);

/*

Similarly we can add a new partition to handle new data. We can create an empty partition 
in the partitioned table just as the original partitions were created above:

CREATE TABLE measurement_y2008m02 PARTITION OF measurement
    FOR VALUES FROM ('2008-02-01') TO ('2008-03-01')
    TABLESPACE fasttablespace;

*/

-------------------------------------
CREATE INDEX IF NOT EXISTS "name_idx_000_005" ON es_events_000_005 
CREATE INDEX IF NOT EXISTS "name_idx_005_010" ON es_events_005_010 
CREATE INDEX IF NOT EXISTS "name_idx_010_015" ON es_events_010_015 
CREATE INDEX IF NOT EXISTS "name_idx_015_020" ON es_events_015_020 
CREATE INDEX IF NOT EXISTS "name_idx_020_025" ON es_events_020_025 
CREATE INDEX IF NOT EXISTS "name_idx_025_030" ON es_events_025_030 
CREATE INDEX IF NOT EXISTS "name_idx_030_035" ON es_events_030_035 
CREATE INDEX IF NOT EXISTS "name_idx_035_040" ON es_events_035_040 
CREATE INDEX IF NOT EXISTS "name_idx_040_045" ON es_events_040_045 
CREATE INDEX IF NOT EXISTS "name_idx_045_050" ON es_events_045_050 
CREATE INDEX IF NOT EXISTS "name_idx_050_055" ON es_events_050_055 
CREATE INDEX IF NOT EXISTS "name_idx_055_060" ON es_events_055_060 
CREATE INDEX IF NOT EXISTS "name_idx_060_065" ON es_events_060_065 
CREATE INDEX IF NOT EXISTS "name_idx_065_070" ON es_events_065_070 
CREATE INDEX IF NOT EXISTS "name_idx_070_075" ON es_events_070_075 
CREATE INDEX IF NOT EXISTS "name_idx_075_080" ON es_events_075_080 