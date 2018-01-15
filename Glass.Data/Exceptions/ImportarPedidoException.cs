using System;

namespace Glass.Data.Exceptions
{
    internal class ImportarPedidoException : Exception
    {
        public ImportarPedidoException(uint? idPedido, Exception erro) :
            base("Erro ao importar pedido." + (idPedido > 0 ? " Código: " + idPedido : ""), erro)
        {
        }
    }
}