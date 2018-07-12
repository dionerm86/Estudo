// <copyright file="TraducaoOrdenacaoListaClientes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Clientes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de clientes.
    /// </summary>
    internal class TraducaoOrdenacaoListaClientes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaClientes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaClientes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCli DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdCli";

                case "codigopedidocliente":
                    return "CodCliente";

                case "cliente":
                    return "NomeCliente";

                case "loja":
                    return "NomeLoja";

                case "vendedor":
                    return "NomeFunc";

                case "tipovenda":
                    return "DescrTipoVenda";

                case "datafinalizacao":
                    return "DataFin";

                case "dataconfirmacao":
                    return "DataConf";

                case "idprojeto":
                case "idorcamento":
                case "total":
                case "datapedido":
                case "dataentrega":
                case "datapronto":
                case "dataliberacao":
                case "situacao":
                case "situacaoproducao":
                case "tipopedido":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
