# 9soat-g12-mc-king

Tech Challenge FIAP - 9SOAT - Grupo 12

  

Participantes:

Mayara Manzi - RM359734

mayaramanzi@hotmail.com

  

Renan Eustaquio Claudiano Martins - RM359737

renan.ecm@outlook.com

  

Link da documentação:

[https://miro.com/app/board/uXjVLT4i-kY=/](https://miro.com/app/board/uXjVLT4i-kY=/)

  

## Objetivo

  

Permitir a expansão de uma lanchonete de bairro. A automatização do sistema visa garantir a organização dos pedidos evitando atrasos e confusão.

  

## Tecnologias Utilizadas

  

Visual studio 2022

  

.NET - 8.0

  

Postgres - 16.4


Docker desktop


Kubernetes


MiniKube

# Guia de Setup do Projeto

  

Este guia descreve o processo de configuração do ambiente para rodar o projeto localmente utilizando Minikube e Docker.

  

## 1. Clonar o Repositório

  

Primeiro, clone o repositório do projeto para sua máquina local e, em seguida, abra um terminal na raiz do projeto.

  

```bash
git  clone  https://github.com/mayaramanziFiap/Tech-Challenge-Fiap-Mc-King-Fase2.git
```

  

## 2. Iniciar o Minikube

  

Com o terminal aberto na raiz do projeto, inicie um cluster Minikube para criar um ambiente Kubernetes local.

  

```bash
minikube  start
```

  

## 3. Configurar o Ambiente Docker com Minikube

  

Configure o Docker para utilizar o ambiente Minikube. Use um dos seguintes comandos:

  

```bash
minikube  docker-env
```

  

Ou, para aplicar diretamente as variáveis de ambiente:

  

```bash
eval $(minikube  docker-env)
```

  

## 4. Construir a Imagem Docker (Opcional)

  

Caso deseje construir a imagem Docker da aplicação, utilize o Dockerfile especificado no diretório `API`:

  

```bash
docker  buildx  build  -t  mckingapi-api:dev  -f  API/Dockerfile  .
```

  

## 5. Navegar para o Diretório e Iniciar o Minikube

  

Navegue até o diretório Kubernetes com o seguinte comando:

  

```bash
cd  Kubernetes
```

  

## 6. Criar o Pod do Banco de Dados

  

Aplique os arquivos de configuração no diretório `database` para criar o pod do banco de dados:

  

```bash
kubectl  apply  -f  Postgres  --validate=false
```

  

## 7. Verificar os Pods

  

Para verificar se os pods foram criados corretamente, execute:

  

```bash
kubectl  get  pod
```

  

## 8. Criar os Pods da Aplicação

  

Aplique os arquivos de configuração no diretório `api` para criar os pods da aplicação:

  

```bash
kubectl  apply  -f  Api
```

  

## 9. Listar os Serviços

  

Para listar todos os serviços que estão rodando no cluster Kubernetes, utilize:

  

```bash
kubectl  get  services
```

  

## 10. Expor o Serviço Externamente

  

Para expor o serviço `mckingapi-service` externamente, execute:

  

```bash
minikube  service  mckingapi-service
```

  

## 11. Criar o Pod do Ngrok

  

Aplique os arquivos de configuração no diretório `Ngrok` para criar os pods do Ngrok:

  

```bash
kubectl  apply  -f  Ngrok
```

## 12. Versão Free do Ngrok

  

A versão free do ngrok precisa de um acesso inicial para habilitar o acessso, copie o link abaixo e clique Acessar:

  

```bash
https://similarly-pleasant-ocelot.ngrok-free.app/
```
  
