# Prepare postgres for dev

## Connenct to VM via ssh

ssh -i "E:\documents\Passwords\sshkeys-dev01\bonanza-dev01_key.pem" azurecat@20.102.99.1

## Run docker container

In terminal go to folder where docker compose is stored, then:
`docker-compose up`

## Copy file from stoped container

docker cp 3f817b963d70:/var/lib/postgresql/data/postgresql.conf postgresql.conf

## Disable restart container
docker update --restart=no container

## get contaier logs

docker logs --tail 50 container

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

## Backup db

create dump:

`pg_dump -p 5432 -h 172.20.0.3 -U root -W -d bonanza-test-db > dbexport.pgsql`

or (for compressed file)

`pg_dump -p 5432 -h 172.20.0.3 -U root -W -Fc -Z 6 -d bonanza-test-db > dbexport.dump`

copy from container to host machine:

`sudo docker cp 3f817b963d70:dbexport.dump  dbexport-copy.dump`

copy from remote azure Vm to local path:

`scp -i "E:\documents\Passwords\sshkeys-dev01\bonanza-dev01_key.pem" azurecat@20.102.99.1:~/dbexport-copy.dump dbexport-copy.dump`

## Restart | Start |Stop Postgres server

/etc/init.d/postgresql {start|stop|restart|reload|force-reload|status}

## how to check SQL

https://dbfiddle.uk/?rdbms=postgres_13&fiddle=42dd48d82a48cd6afa941a63835d2b02

## Tune DB

https://dba.stackexchange.com/questions/212563/what-performance-advantages-does-append-only-postgres-allow

## ?

Don't know why, but:
shared_buffers = 4GB  = 222 i/sec
shared_buffers = 40MB = 260 i/sec