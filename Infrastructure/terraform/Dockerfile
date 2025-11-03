FROM hashicorp/terraform:latest

USER root

# Instala Python e virtualenv
RUN apk add --no-cache python3 py3-pip py3-virtualenv

# Cria ambiente virtual
RUN python3 -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

# Instala o pacote dentro do ambiente virtual
RUN pip install terraform-local

WORKDIR /infra