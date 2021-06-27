# Prepare postgres for dev

## Connenct to VM via ssh

ssh -i "E:\documents\Passwords\azure-key\mac-key" azurecat@20.102.99.1

## Run docker container

In terminal go to folder where docker compose is stored, then:
`docker-compose up`

## Open DB UI

For accessing DB `pgadmin4` is used. Open your favorite web browser by vising the URL http://localhost:5050/. Use the admin@admin.com for the email address and root as the password to log in.

## Add server

In `pgadmin4` select menu item: Servers > Create > Server.
Fill fields 
- name: `bonanza-test-server`
- hostname(connection tab): 
- - get docker container id: `docker ps`.
- - get docker container ip by id `docker inspect fcc97e066cc8 | grep IPAddress`
- user: root
- password: root123456# [`root` by default]

## Add DB

Add db with name `bonanza-test-db` via UI

## ENter container's bash

`docker ps` to get id of the container
docker exec -it 7cf8e751ecf6 /bin/bash

## Get System info

### Get OS version

In docker, press button `cli` on docker container.
In CLI: `uname -a`
Current version is:
`Linux 103e63f5d491 5.10.25-linuxkit #1 SMP Tue Mar 23 09:27:39 UTC 2021 x86_64 GNU/Linux`
`Linux 7cf8e751ecf6 4.19.84-microsoft-standard #1 SMP Wed Nov 13 11:44:37 UTC 2019 x86_64 GNU/Linux`

### Get CPU info

lscpu

### Get PGsql config path

`psql -d bonanza-test-db -c 'SHOW config_file'`
currently: `/var/lib/postgresql/data/postgresql.conf`

view file: `less /var/lib/postgresql/data/postgresql.conf`

## Restart | Start |Stop Postgres server

/etc/init.d/postgresql {start|stop|restart|reload|force-reload|status}

## how to check SQL

https://dbfiddle.uk/?rdbms=postgres_13&fiddle=42dd48d82a48cd6afa941a63835d2b02

## ?

Don't know why, but:
shared_buffers = 4GB  = 222 i/sec
shared_buffers = 40MB = 260 i/sec