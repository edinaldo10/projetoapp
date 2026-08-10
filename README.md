# 🚀 Cloud Application (.NET 8 MVC & API)

Aplicação web e API REST moderna de gerenciamento de pedidos (Orders & Items), desenvolvida com foco em **boas práticas de arquitetura, resiliência e prontidão para nuvem (Cloud-Native)**.

## 🛠️ Tecnologias e Padrões Utilizados
* **.NET 8** (ASP.NET Core MVC & Minimal APIs)
* **Entity Framework Core 8** (Suporte multi-banco: PostgreSQL para Produção e SQLite/In-Memory para Testes)
* **Bootstrap 5** (Interface Web responsiva e moderna)
* **Scalar** (Documentação interativa de API moderna)
* **Health Checks** (`/health/live` e `/health/ready` para orquestradores como Kubernetes/ECS)
* **Docker & Docker Compose** (Containerização com Multi-Stage Builds e usuários não-root)

---

## ⚙️ Como Executar o Projeto Localmente (Via Docker)

A maneira mais rápida e recomendada de testar a aplicação completa (com o banco PostgreSQL embutido) é utilizando o Docker Compose:

1. Clone o repositório e acesse a pasta raiz.
2. Execute o comando:
   ```bash
   docker compose up --build