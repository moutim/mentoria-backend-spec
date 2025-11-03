\c postgres;
CREATE DATABASE movimentos;

\c movimentos;

CREATE TABLE categorias (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

CREATE TABLE movimentos (
    id SERIAL PRIMARY KEY,
    usuario_id VARCHAR(11) NOT NULL,
    remetente VARCHAR(11) NOT NULL,
    destinatario VARCHAR(11) NOT NUll,
    tipo VARCHAR(50) NOT NULL,
    categoria_id INT NULL REFERENCES categorias(id),
    descricao VARCHAR(255),
    valor NUMERIC(10, 2) NOT NULL,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Populando categorias
INSERT INTO categorias (nome) VALUES
('Transferência'),
('Pagamento'),
('Recebimento');

-- Populando movimentos
INSERT INTO movimentos (usuario_id, remetente, destinatario, tipo, categoria_id, descricao, valor)
VALUES
('12345678901', '12345678901', '10987654321', 'Transferência', 1, 'Envio de dinheiro', 150.00),
('10987654321', '10987654321', '12345678901', 'Recebimento', 3, 'Recebimento de dinheiro', 200.00),
('12345678901', '12345678901', '10987654321', 'Pagamento', 2, 'Pagamento de conta', 75.50);