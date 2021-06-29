# Readme

## benchmark speed

| Func			| Db on			| Ram	| CPUs	| Optimized	| Benchmark		| Speed on start	| Events written	| Speed et the end	| 
| ----			| -----			| ----	| ----	| ---------	| ---------		| --------------	| ----------------	| -----------------	|
| Postgres		| macBook		| 16Gb	| 6		| Yes		| mac			| 3500				|					|					|
| Postgres		| macBook		| 16Gb	| 6		| Yes		| lenovo		| 1200				|					|					|
| Postgres		| lenovo		| 16Gb	| 6		| Yes		| lenovo		| 2500				|					|					|
| Postgres		| azure-dev01	| 08Gb	| 2		| Yes		| azure-dev02	| 4444				|	800,000,000		|					|
| Postgres		| azure-dev01	| 08Gb	| 2		| No		| azure-dev02	| 2000				|					|					|
| Postgres		| macBook		| 16Gb	| 6		| No		| mac			| 1500				|					|					|
| TimeScale		| lenovo		| 9Gb	| 8		| No		| lenovo		| 2000				|					|					|
| TimeScale		| azure-dev01	| 08Gb	| 2		| No		| azure-dev02	| 3000				|					|					|


This formats starts writes 2500, 

after 100M it's 2000, 
after 200M it's 1500, 
after 300M it's 1300'

order-20210619-tenant-00079-stream-0014144

CREATE TABLE IF NOT EXISTS public.es_events
(
    id integer NOT NULL DEFAULT nextval('es_events_id_seq'::regclass),
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    version integer NOT NULL,
    data bytea NOT NULL
)

CREATE INDEX "name-idx"
    ON public.es_events USING btree
    (name COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;