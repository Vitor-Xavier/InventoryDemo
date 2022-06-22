# Inventário

Projeto de demonstração para fixação de conceitos, baseado em controle de inventário.

## Características

- .NET 6
- Autenticação em JWT
- Documentação de API em Swagger
- Entity Framework Core 6
- Bulk Extensions para operações de dados em massa
- Compressão de resposta por Gzip e Brotli
- Mensageria com RabbitMQ e MassTransit
- Operações agendadas em segundo plano
- Cache com Redis
- Padrão de erros com _Problem Details_

## Tecnologias

**Server:** .NET 6, SQL Server, Redis

## Rodar localmente

Clonar o projeto

```bash
git clone https://github.com/Vitor-Xavier/InventoryDemo.git
```

Instalar Redis

```bash
sudo apt-get install redis-server
```

Iniciar Redis

```bash
redis-server
```


## Licença

[MIT](https://github.com/Vitor-Xavier/InventoryDemo/blob/main/LICENSE)

