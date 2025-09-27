# Boas Práticas de Desenvolvimento C# .NET 8 Web API (Sem Autenticação)

Este guia lista práticas recomendadas para desenvolvimento de **Web APIs em .NET 8**, sem camada de autenticação, visando **qualidade, performance, escalabilidade e segurança básica**.

---

## 📁 Estrutura e Organização
- Utilize **Minimal APIs** quando possível para endpoints simples.
- Separe responsabilidades em **camadas**:
  - `Domain` → entidades e regras de negócio.
  - `Application` → serviços, DTOs e lógica de aplicação.
  - `Infrastructure` → persistência, acesso a dados, integração externa.
  - `Presentation` → controllers ou endpoints (Minimal APIs).
- Organize namespaces de acordo com o domínio da aplicação.
- Prefira **arquitetura limpa** (Clean Architecture) para projetos de médio/grande porte.

---

## 🧑‍💻 Padrões de Código
- Habilite **nullable reference types** (`<Nullable>enable</Nullable>` no csproj).
- Utilize `record` ou `record struct` para DTOs imutáveis.
- Prefira **`required` properties** para inicialização obrigatória.
- Evite `public set` em entidades → use métodos ou construtores.
- Utilize **`var`** para variáveis locais, mas seja explícito em retornos de métodos.
- Utilize **interpolação de strings** em vez de concatenação.
- Prefira `async/await` em chamadas de I/O.

---

## ⚡ Performance
- Configure **Pooling de conexões** no EF Core.
- Utilize **`AsNoTracking()`** em consultas somente leitura no EF Core.
- Utilize **caching** (MemoryCache ou DistributedCache) para dados frequentemente acessados.
- Prefira **Pagination** em consultas que retornam coleções.
- Use **Span/Memory** para manipulação de dados em cenários de alto desempenho.
- Configure **Response Compression** no pipeline.

---

## 🔒 Segurança (mesmo sem autenticação)
- Sempre **valide dados de entrada** (FluentValidation ou DataAnnotations).
- Nunca confie em dados fornecidos pelo cliente → sanitize inputs.
- Configure **CORS** para permitir apenas origens necessárias.
- Utilize **HTTPS obrigatório**.
- Ative **Rate Limiting** (ASP.NET Core RateLimiter Middleware).
- Configure **Content Security Policy (CSP)** no proxy ou servidor.

---

## 📊 Logging e Observabilidade
- Utilize **`ILogger<T>`** para logging estruturado.
- Configure **Serilog** ou **OpenTelemetry** para observabilidade.
- Registre logs de **entrada, saída e erros**.
- Use **Correlation IDs** para rastrear requisições.
- Exponha métricas com **Prometheus / OpenTelemetry Metrics**.

---

## 🧪 Testes
- Utilize **xUnit** ou **NUnit** para testes unitários.
- Faça testes de integração com **WebApplicationFactory**.
- Prefira **Mocks com Moq** ou **NSubstitute** para dependências.
- Cubra **services, repositórios e endpoints principais**.

---

## 📦 Versionamento e Deploy
- Use **Versionamento de API** (Microsoft.AspNetCore.Mvc.Versioning).
- Utilize **Docker** para padronizar ambiente.
- Configure **CI/CD pipelines** (GitHub Actions, Azure DevOps, GitLab CI).
- Sempre valide com **dotnet format** e **analyzers** antes do build.
- Configure **health checks** (`/health`) para monitoramento.

---

## 🏗️ Boas Práticas de API
- Utilize **status codes corretos**:
  - `200 OK` → sucesso.
  - `201 Created` → recurso criado.
  - `204 No Content` → sem resposta.
  - `400 Bad Request` → erro do cliente.
  - `404 Not Found` → recurso inexistente.
  - `500 Internal Server Error` → erro inesperado.
- Retorne **ProblemDetails** para erros (`Results.Problem()` em Minimal APIs).
- Utilize **DTOs específicos para requests/responses**, não exponha entidades do domínio diretamente.
- Padronize rotas: `/api/{recurso}/{id}`.
- Documente endpoints com **Swagger (Swashbuckle)**.

---

## ✅ Checklist Rápido
- [ ] Projeto estruturado em camadas ou Minimal APIs organizadas.
- [ ] Nullable e analisadores habilitados.
- [ ] Validação de entrada aplicada.
- [ ] Logging e métricas configurados.
- [ ] Versionamento da API implementado.
- [ ] Testes unitários e de integração presentes.
- [ ] CORS restrito e HTTPS forçado.
- [ ] Rate Limiting configurado.
- [ ] Health checks implementados.
- [ ] Swagger documentando a API.

---

📖 **Referências Oficiais:**
- [ASP.NET Core Docs](https://learn.microsoft.com/aspnet/core)
- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)

