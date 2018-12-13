// <copyright file="TraducaoOrdenacaoListaTiposFuncionario.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Funcionarios.Tipos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de tipos de funcionário.
    /// </summary>
    internal class TraducaoOrdenacaoListaTiposFuncionario : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTiposFuncionario"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTiposFuncionario(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DESCRICAO ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo)
            {
                case "descricao":
                    return "DESCRICAO";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}