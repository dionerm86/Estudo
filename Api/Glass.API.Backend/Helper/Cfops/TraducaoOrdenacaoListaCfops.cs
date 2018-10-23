// <copyright file="TraducaoOrdenacaoListaCfops.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Cfops
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de CFOP's.
    /// </summary>
    internal class TraducaoOrdenacaoListaCfops : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCfops"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCfops(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CodInterno ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "CodInterno";

                case "nome":
                    return "Descricao";

                case "tipo":
                    return "Tipo";

                case "tipoMercadoria":
                    return "TipoMercadoria";

                case "alterarEstoqueTerceiros":
                    return "AlterarEstoqueTerceiros";

                case "alterarEstoqueCliente":
                    return "AlterarEstoqueCliente";

                case "obs":
                    return "Obs";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
