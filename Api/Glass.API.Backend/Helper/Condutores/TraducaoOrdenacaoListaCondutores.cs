// <copyright file="TraducaoOrdenacaoListaCondutores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Condutores
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de condutores.
    /// </summary>
    internal class TraducaoOrdenacaoListaCondutores : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCondutores"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCondutores(string ordenacao)
            : base(ordenacao)
        {
        }

        protected override string OrdenacaoPadrao
        {
            get { return "Nome ASC"; }
        }

        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "cod":
                    return "IdCondutor";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
