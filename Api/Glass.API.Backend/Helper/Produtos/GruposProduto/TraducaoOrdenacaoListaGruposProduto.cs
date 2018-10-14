// <copyright file="TraducaoOrdenacaoListaGruposProduto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.GruposProduto
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de grupos de produto.
    /// </summary>
    internal class TraducaoOrdenacaoListaGruposProduto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaGruposProduto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaGruposProduto(string ordenacao)
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
                case "id":
                    return "IdGrupoProd";

                case "nome":
                    return "Descricao";

                case "tipocalculopedido":
                    return "TipoCalculo";

                case "tipocalculonotafiscal":
                    return "TipoCalculoNf";

                case "bloquearestoque":
                    return "BloquearEstoque";

                case "alterarestoque":
                    return "NaoAlterarEstoque";

                case "alterarestoquefiscal":
                    return "NaoAlterarEstoqueFiscal";

                case "exibirmensagemestoque":
                    return "ExibirMensagemEstoque";

                case "geravolume":
                    return "GeraVolume";

                case "tipo":
                    return "TipoGrupo";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
