// <copyright file="TraducaoOrdenacaoListaPerdasChapasVidro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.ChapasVidro.Perdas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de perdas de chapas de vidro.
    /// </summary>
    internal class TraducaoOrdenacaoListaPerdasChapasVidro : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPerdasChapasVidro"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPerdasChapasVidro(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPerdaChapaVidro DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "IdPerdaChapaVidro";

                case "produto":
                    return "DescrProd";

                case "tipoperda":
                    return "TipoPerda";

                case "subtipoperda":
                    return "SubtipoPerda";

                case "etiqueta":
                    return "NumEtiqueta";

                case "data":
                    return "DataPerda";

                case "funcionario":
                    return "FuncPerda";

                case "observacao":
                    return "Obs";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}