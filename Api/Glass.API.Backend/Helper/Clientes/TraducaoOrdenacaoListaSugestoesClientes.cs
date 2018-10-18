// <copyright file="TraducaoOrdenacaoListaSugestoesClientes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Clientes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de clientes.
    /// </summary>
    internal class TraducaoOrdenacaoListaSugestoesClientes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaSugestoesClientes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaSugestoesClientes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdSugestao ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdSugestao";

                case "cliente":
                    return "Cliente";

                case "idpedido":
                    return "IdPedido";

                case "rota":
                    return "DescricaoRota";

                case "datacadastro":
                    return "DataCad";

                case "nomefuncionario":
                    return "Funcionario";

                case "tipo":
                    return "DescricaoTipoSugestao";

                case "descricao":
                    return "Descricao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
