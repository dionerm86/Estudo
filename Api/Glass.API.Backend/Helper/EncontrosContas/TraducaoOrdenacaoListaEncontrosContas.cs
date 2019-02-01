// <copyright file="TraducaoOrdenacaoListaEncontrosContas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.EncontrosContas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de encontros de contas.
    /// </summary>
    internal class TraducaoOrdenacaoListaEncontrosContas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaEncontrosContas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaEncontrosContas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "idEncontroContas asc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdEncontroContas";

                case "cliente":
                    return "NomeCliente";

                case "fornecedor":
                    return "NomeFornecedor";

                case "datacadastro":
                    return "DataCad";

                case "observacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}