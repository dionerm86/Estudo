// <copyright file="TraducaoOrdenacaoListaTabelasDescontoAcrescimoCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.TabelasDescontoAcrescimoCliente
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de tabelas de desconto/acréscimo.
    /// </summary>
    internal class TraducaoOrdenacaoListaTabelasDescontoAcrescimoCliente : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTabelasDescontoAcrescimoCliente"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTabelasDescontoAcrescimoCliente(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdTabelaDesconto ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdTabelaDesconto";

                case "nome":
                    return "Descricao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
