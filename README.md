# preciosgamer.xyz

English | [Español](/README-ES.md)

Gaming stores prices in a single place

## Running the solution

> [!IMPORTANT]
> You must have docker installed in your machine. If you do not want to install docker you can still run the solution but you will have to update the appsettings.Development.json with your infrastructure details.

There is a `docker compose` that will set up all the infrastructure you need to run the solution without configuring anything.

This will run:

- PostgreSQL
- RabbitMQ
- Nginx

```
docker compose up
```

Now you can run any project you need manually using your IDE or terminal.
