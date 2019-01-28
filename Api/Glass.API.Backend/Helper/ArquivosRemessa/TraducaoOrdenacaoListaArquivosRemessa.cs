// <copyright file="TraducaoOrdenacaoListaArquivosRemessa.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ArquivosRemessa
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de arquivos de remessa.
    /// </summary>
    internal class TraducaoOrdenacaoListaArquivosRemessa : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaArquivosRemessa"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaArquivosRemessa(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdArquivoRemessa desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdArquivoRemessa";

                case "funcionario":
                    return "DescrUsuCad";

                case "datacadastro":
                    return "DataCad";

                case "tipo":
                    return "Tipo";

                case "numeroremessa":
                    return "NumRemessa";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}