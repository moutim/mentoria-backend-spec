# ============================================================================
# SQS Queues - Filas de Mensagens para Processamento Assíncrono
# ============================================================================
# 
# Este arquivo configura as filas SQS necessárias para o sistema de 
# processamento assíncrono de eventos:
#
# 1. notificacoes-queue: Processa envio de emails/SMS
# 2. notificacoes-dlq: Armazena mensagens que falharam 3x (Dead Letter Queue)
#
# ============================================================================

# ----------------------------------------------------------------------------
# Dead Letter Queue (DLQ) - Fila para Mensagens com Erro
# ----------------------------------------------------------------------------
# Esta fila recebe mensagens que não puderam ser processadas após 3 tentativas.
# Útil para debugging e análise de erros.

resource "aws_sqs_queue" "notificacoes_dlq" {
  name                      = "notificacoes-dlq"
  message_retention_seconds = 1209600  # 14 dias (máximo permitido)
  
  tags = {
    Name        = "notificacoes-dlq"
    Environment = "development"
    Project     = "mentoria-backend"
    Purpose     = "Dead Letter Queue para mensagens com erro"
  }
}

# ----------------------------------------------------------------------------
# Fila Principal de Notificações
# ----------------------------------------------------------------------------
# Processa eventos de movimentações para enviar notificações (email/SMS)
# aos clientes.

resource "aws_sqs_queue" "notificacoes_queue" {
  name                      = "notificacoes-queue"
  delay_seconds             = 0                 # Sem delay: processa imediatamente
  max_message_size          = 262144            # 256 KB (máximo permitido)
  message_retention_seconds = 345600            # 4 dias
  receive_wait_time_seconds = 20                # Long polling (aguarda até 20s por mensagens)
  visibility_timeout_seconds = 30               # Tempo que mensagem fica "invisível" após ser lida
  
  # Configuração de Dead Letter Queue (DLQ)
  # Mensagens que falharem 3x vão para notificacoes-dlq
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.notificacoes_dlq.arn
    maxReceiveCount     = 3  # Após 3 tentativas falhas, vai para DLQ
  })

  tags = {
    Name        = "notificacoes-queue"
    Environment = "development"
    Project     = "mentoria-backend"
    Purpose     = "Processa eventos para envio de notificações"
  }
}

# ----------------------------------------------------------------------------
# Policy para permitir publicação de mensagens
# ----------------------------------------------------------------------------
# Permite que qualquer serviço no LocalStack publique mensagens na fila

resource "aws_sqs_queue_policy" "notificacoes_queue_policy" {
  queue_url = aws_sqs_queue.notificacoes_queue.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = "*"
        Action = [
          "sqs:SendMessage",
          "sqs:ReceiveMessage",
          "sqs:DeleteMessage",
          "sqs:GetQueueAttributes"
        ]
        Resource = aws_sqs_queue.notificacoes_queue.arn
      }
    ]
  })
}

# ----------------------------------------------------------------------------
# Outputs - Informações úteis após criar recursos
# ----------------------------------------------------------------------------

output "sqs_queues" {
  description = "Informações das filas SQS criadas"
  value = {
    notificacoes_queue = {
      name = aws_sqs_queue.notificacoes_queue.name
      url  = aws_sqs_queue.notificacoes_queue.url
      arn  = aws_sqs_queue.notificacoes_queue.arn
    }
    notificacoes_dlq = {
      name = aws_sqs_queue.notificacoes_dlq.name
      url  = aws_sqs_queue.notificacoes_dlq.url
      arn  = aws_sqs_queue.notificacoes_dlq.arn
    }
  }
}

# Outputs individuais para fácil acesso
output "notificacoes_queue_url" {
  description = "URL da fila de notificações (use no código C#)"
  value       = aws_sqs_queue.notificacoes_queue.url
}

output "notificacoes_dlq_url" {
  description = "URL da Dead Letter Queue"
  value       = aws_sqs_queue.notificacoes_dlq.url
}
