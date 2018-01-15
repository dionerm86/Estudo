namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class ListagemPendenciaCarregamento
    {
        #region Contrutores

        public ListagemPendenciaCarregamento()
            : this(0, 0, null, 0, 0 ,null)
        {
        }

        internal ListagemPendenciaCarregamento(uint idCarregamento, uint idCliente, string nomeCliente, decimal pesoTotal, uint idClienteExterno, string clienteExterno)
        {
            IdCliente = idCliente;
            NomeCliente = nomeCliente;
            IdCarregamento = idCarregamento;
            PesoTotal = pesoTotal;
            IdClienteExterno = idClienteExterno;
            ClienteExterno = clienteExterno;
        }

        #endregion

        #region Propiedades

        public uint IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public string IdNomeCliente 
        { 
            get 
            { 
                return IdCliente + " - " + NomeCliente;
            }
        }

        public uint IdCarregamento { get; set; }

        public decimal PesoTotal { get; set; }

        public uint IdClienteExterno { get; set; }

        public string ClienteExterno { get; set; }

        #endregion
    }
}
