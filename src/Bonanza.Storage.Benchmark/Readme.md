# Readme

## benchmark speed

| Func			| Db on			| Ram	| CPUs	| Optimized	| Benchmark		| Speed	|
| ----			| -----			| ----	| ----	| ---------	| ---------		| -----	|
| Postgres		| macBook		| 16Gb	| 6		| Yes		| mac			| 3500	|
| Postgres		| macBook		| 16Gb	| 6		| Yes		| lenovo		| 1200	|
| Postgres		| lenovo		| 16Gb	| 6		| Yes		| lenovo		| 2500	|
| Postgres		| azure-dev01	| 08Gb	| 2		| Yes		| azure-dev02	| 4444	|
| Postgres		| azure-dev01	| 08Gb	| 2		| No		| azure-dev02	| 2000	|
| Postgres		| macBook		| 16Gb	| 6		| No		| mac			| 1500	|
| TimeScale		| lenovo		| 9Gb	| 8		| No		| lenovo		| 2000	|
| TimeScale		| azure-dev01	| 08Gb	| 2		| No		| azure-dev02	| 3000	|
