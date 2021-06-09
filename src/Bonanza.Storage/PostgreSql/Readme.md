# Prepare postgres for dev

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
- password: root123456#

## Add DB

Add db with name `bonanza-test-db` via UI

## Get Os version

In docker, press button `cli` on docker container.
In CLI: `uname -a`
Current version is:
`Linux 103e63f5d491 5.10.25-linuxkit #1 SMP Tue Mar 23 09:27:39 UTC 2021 x86_64 GNU/Linux`

## how to check SQL:
https://dbfiddle.uk/?rdbms=postgres_13&fiddle=42dd48d82a48cd6afa941a63835d2b02

