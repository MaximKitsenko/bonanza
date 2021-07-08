# Prepare postgres for dev

## Connenct to VM via ssh

ssh -i "E:\documents\Passwords\sshkeys-dev01\bonanza-dev01_key.pem" azurecat@20.102.99.1

## Run docker container

In terminal go to folder where docker compose is stored, then:
`docker-compose up`

## Add client package

dotnet add package EventStore.Client.Grpc.Streams --version 20.10

## Get container IP

docker inspect bffee5e589e1

## Add server

## Add DB

## Enter container's bash

`docker ps` to get id of the container
docker exec -it 7cf8e751ecf6 /bin/bash

## Get System info

### Get OS version

### Get CPU info

## Restart | Start |Stop Postgres server

/etc/init.d/postgresql {start|stop|restart|reload|force-reload|status}