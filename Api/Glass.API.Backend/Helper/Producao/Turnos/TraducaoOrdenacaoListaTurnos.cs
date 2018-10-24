// <copyright file="TraducaoOrdenacaoListaTurnos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Turnos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de turnos.
    /// </summary>
    internal class TraducaoOrdenacaoListaTurnos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTurnos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTurnos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Descricao ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "Descricao";

                case "sequencia":
                    return "NumSeq";

                case "inicio":
                case "termino":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
