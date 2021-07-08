# FAQ

## how to connect vm via ssh

`ssh -i ~/max-projects/projects/bonanza-dev01_key.pem azurecat@20.102.99.1`

## install docker:

`sudo apt-get update`
`sudo apt-get install docker.io`
`sudo apt  install docker-compose`

## run docker image

`sudo docker-compose up -d`

## restart docker

`systemctl restart docker`

## install chromium
`sudo apt-get install chromium-browser`

## create docker valume for persistent data

### YML

Add this line to docker compose, it will create valume, on first start:
```
volumes:
      - myapp:/home/node/app
```
       or create valume manually

### Manually

`docker volume create postgres-data`
`docker volume ls`
`docker volume inspect my-vol`


docker run -d --name postgres-server -p 5432:5432 -v postgres-data:/var/lib/postgresql/data -e "POSTGRES_PASSWORD=kamisama123" postgres


## Copy file via ssh

create file in remote linux machine via ssh:
-connect to ssh
-`mkdir home/projects`
-`mkdir home/projects/bonanza`
-`sudo chmod  777 -R  home/projects/`

from local pc terminal:
`scp ~/max-projects/bonanza/src/Bonanza.Storage/PostgreSql/docker-compose.yml  azurecat@20.102.99.1:/home/projects/bonanza`



`scp <file to upload> <username>@<hostname>:<destination path>`



