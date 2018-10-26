// <copyright file="TraducaoOrdenacaoListaClassificacoesRoteiro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Roteiros.ClassificacoesRoteiro
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de classificações de roteiro.
    /// </summary>
    internal class TraducaoOrdenacaoListaClassificacoesRoteiro : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaClassificacoesRoteiro"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaClassificacoesRoteiro(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdClassificacaoRoteiroProducao ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdClassificacaoRoteiroProducao";

                case "nome":
                    return "Descricao";

                case "capacidadediaria":
                case "metadiaria":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
