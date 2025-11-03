\c postgres;
CREATE DATABASE saldos;

\c saldos;
CREATE TABLE saldos (
    id SERIAL PRIMARY KEY,
    usuario_id VARCHAR(11) NOT NULL,
    saldo NUMERIC(10, 2) NOT NULL,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE saldo_categorias (
    id SERIAL PRIMARY KEY,
    usuario_id VARCHAR(11) NOT NULL,
    categoria_id INT NOT NULL,
    saldo NUMERIC(10, 2) NOT NULL,
    criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (usuario_id, categoria_id)
);


-- Populando saldos
INSERT INTO saldos (usuario_id, saldo)
VALUES
('12345678901', 500.00),
('10987654321', 750.00);

-- Populando saldo_categorias
INSERT INTO saldo_categorias (usuario_id, categoria_id, saldo)
VALUES
('12345678901', 1, 300.00),
('12345678901', 2, 200.00),
('10987654321', 1, 400.00),
('10987654321', 2, 350.00);