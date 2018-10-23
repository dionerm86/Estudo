// <copyright file="TraducaoOrdenacaoListaTiposPerda.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.TiposPerda
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de tipos de perda.
    /// </summary>
    internal class TraducaoOrdenacaoListaTiposPerda : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTiposPerda"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTiposPerda(string ordenacao)
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

                case "setor":
                    return "idSetor";

                case "exibirnopaineldeproducao":
                    return "ExibirPainelProducao";

                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
