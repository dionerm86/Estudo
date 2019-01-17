// <copyright file="TraducaoOrdenacaoListaArquivosOtimizacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ArquivosOtimizacao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de arquivos de otimização.
    /// </summary>
    internal class TraducaoOrdenacaoListaArquivosOtimizacao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaArquivosOtimizacao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaArquivosOtimizacao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "dataCad desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
