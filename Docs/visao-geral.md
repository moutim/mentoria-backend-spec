
Bem-vindos ao desafio prático da nossa mentoria! Este projeto foi cuidadosamente estruturado para consolidar e aplicar os conhecimentos adquiridos durante nossos encontros, abrangendo desde conceitos fundamentais até práticas avançadas de engenharia de software.

  

## 🎯 Visão Geral do Desafio

  

### Contexto do Domínio

Você irá implementar um sistema backend para **movimentações bancárias**, trabalhando com dois contextos principais:

  

- **Contexto de Saldos**: Responsável pelo gerenciamento e consulta de saldos das contas

- **Contexto de Movimentos**: Responsável pelo registro e processamento de transações bancárias

  

### Filosofia do Aprendizado

Este desafio foi pensado para acomodar diferentes níveis de senioridade - desde estagiários até desenvolvedores seniores. A estrutura permite que cada pessoa inicie no seu nível de conforto e evolua gradualmente, aplicando conceitos progressivamente mais complexos.

  

## 📋 Objetivos de Aprendizado por Senioridade

  

### 🌱 Para Estagiários e Júniors

**Foco**: Fundamentos e conceitos básicos

- Compreender o que são APIs REST e como funcionam

- Entender conceitos de containerização com Docker

- Implementar operações básicas de CRUD

- Compreender injeção de dependência e inversão de controle

- Escrever seus primeiros testes unitários

  

### 🌿 Para Desenvolvedores Plenos

**Foco**: Padrões arquiteturais e práticas intermediárias

- Aplicar padrões como Repository e separação de responsabilidades

- Implementar comunicação assíncrona entre serviços

- Gerenciar transações de banco de dados adequadamente

- Criar testes de integração robustos

- Aplicar princípios SOLID na prática

  

### 🌳 Para Desenvolvedores Seniores

**Foco**: Arquitetura distribuída e liderança técnica

- Arquitetar soluções distribuídas e escaláveis

- Implementar arquitetura orientada a eventos

- Otimizar performance e considerar aspectos de escalabilidade

- Mentorear código de outros desenvolvedores

- Definir estratégias de deployment e observabilidade

  

## 🏗️ Pilares Arquiteturais

  

### Infraestrutura como Código (IaC)

Utilizaremos **Terraform** e **LocalStack** para simular recursos AWS localmente, proporcionando uma experiência próxima ao ambiente de produção sem custos ou complexidades de cloud.

  

### Containerização e Orquestração

O ambiente será totalmente containerizado usando **Docker** e **Docker Compose**, facilitando a padronização do ambiente de desenvolvimento e simulando cenários de produção.

  

### Arquitetura Distribuída

O projeto segue uma arquitetura de microserviços com separação clara de responsabilidades:

- **APIs**: Camadas de exposição de dados

- **BFFs (Backend for Frontend)**: Camadas de agregação e adaptação

- **Workers**: Processamento assíncrono e background jobs

  

### Comunicação Assíncrona

Implementaremos padrões de comunicação baseados em eventos usando **Amazon SQS** (via LocalStack), promovendo baixo acoplamento entre os serviços.

  

## 🎨 Conceitos Técnicos Abordados

  

### Desenvolvimento Orientado a Objetos

- Encapsulamento, herança, polimorfismo e abstração

- Composição vs herança

- Design patterns clássicos (Strategy, Factory, Observer, etc.)

  

### Princípios de Engenharia de Software

- **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion

- **DRY**: Don't Repeat Yourself

- **KISS**: Keep It Simple, Stupid

- **YAGNI**: You Aren't Gonna Need It

  

### Clean Architecture e Clean Code

- Separação de camadas e responsabilidades

- Dependency Rule e inversão de dependências

- Nomenclatura expressiva e código autoexplicativo

- Refatoração contínua

  

### Arquitetura Baseada em Eventos

- Comunicação assíncrona via eventos

- Separação de responsabilidades (Commands e Queries)

- Processamento em background

- Integração entre serviços via filas

  

### Programação Assíncrona

- async/await patterns

- Task e ValueTask

- ConfigureAwait(false)

- Cancellation Tokens

- Parallel processing

  

### Estratégias de Teste

- **Testes Unitários**: Testando unidades isoladas

- **Testes de Integração**: Testando interações entre componentes

- **Testes de Contrato**: Validando APIs

- **Test-Driven Development (TDD)**: Ciclo Red-Green-Refactor

  

## 🚀 Jornada de Implementação

  

### Fase 1: Preparação do Ambiente

Configuração do ambiente local com Docker, banco de dados PostgreSQL, LocalStack para serviços AWS e ferramentas de desenvolvimento.

  

### Fase 2: Fundamentos

Implementação de operações básicas, entendimento da estrutura do projeto e criação dos primeiros endpoints.

  

### Fase 3: Camada de Dados

Integração com banco de dados, implementação de repositórios e gerenciamento de transações.

  

### Fase 4: Lógica de Negócio

Implementação das regras de negócio, validações e processamento de movimentações bancárias.

  

### Fase 5: Comunicação Assíncrona

Configuração de filas, implementação de publishers/consumers e processamento em background.

  

### Fase 6: Observabilidade e Qualidade

Implementação de logs estruturados, métricas, health checks e cobertura completa de testes.

  

## 📚 Documentação Detalhada

  

Para instruções específicas de implementação, exemplos de código, configurações detalhadas e guias passo a passo, consulte:

  

📖 **[Documentação Técnica Completa](./docs/README.md)**

  

A documentação técnica contém:

- Guias detalhados de implementação

- Exemplos práticos de código

- Configurações de ambiente

- Troubleshooting e resolução de problemas

- Referências e recursos adicionais

  

## Suporte e Comunidade

  

- **Dúvidas Conceituais**: Discussões durante as sessões de mentoria

- **Problemas Técnicos**: Consulte a documentação técnica detalhada

- **Code Review**: Sessões individuais para revisão e feedback

- **Discussões Arquiteturais**: Reuniões de alinhamento técnico

  

## 🚦 Começando

  

```bash

# 1. Clone e navegue para o projeto

cd mentoria-backend

  

# 2. Suba a infraestrutura local

docker-compose up --build -d

  

# 3. Verifique se os serviços estão funcionando

docker ps

  

# 4. Consulte a documentação técnica para próximos passos

# Ver: ./docs/README.md

```

  

---

  

**Lembre-se**: Este é um ambiente seguro para experimentar, errar e aprender. O objetivo principal é a evolução técnica e profissional de cada participante, respeitando o ritmo e nível de conhecimento individual.

  

**Bom desenvolvimento e aprendizado! 🚀**