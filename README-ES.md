# preciosgamer.xyz

[English](/README.md) | Español

Precios de tiendas gaming del pais en un solo lugar

## Correr la solution

> [!IMPORTANT]
> Necesitas tener docker instalado. Si no quieres usar docker aun puedes correr la solucion pero tendras que modificar los appsettings.Development.json de cada proyecto con los datos de tu infraestructura

Hay un `docker compose` que se encargara de levantar la infraestructura necesaria para correr la solucion sin tener que configurar nada.

Se empezaran a ejecutar estas aplicaciones:

- PostgreSQL
- RabbitMQ
- Nginx

```
docker compose up
```

Ahora puedes correr cada proyecto por separado utilizando tu IDE.
