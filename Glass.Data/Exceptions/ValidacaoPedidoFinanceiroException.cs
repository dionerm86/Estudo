using System;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Exceptions
{
    public class ValidacaoPedidoFinanceiroException : Exception
    {
        public List<int> IdsPedido { get; private set; }

        public ObservacaoFinalizacaoFinanceiro.MotivoEnum Motivo { get; private set; }

        public ValidacaoPedidoFinanceiroException(List<int> idsPedido, string message, ObservacaoFinalizacaoFinanceiro.MotivoEnum motivo)
            : base(message)
        {
            IdsPedido = idsPedido;
            Motivo = motivo;
        }
    }
}
