// <copyright file="TraducaoOrdenacaoExportacaoPedidos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Exportacao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de grupos de medidas de projetos.
    /// </summary>
    internal class TraducaoOrdenacaoExportacaoPedidos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoExportacaoPedidos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoExportacaoPedidos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "dataExportacao DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "cod":
                    return "IdExportacao";

                case "fornecedor":
                    return "NomeFornec";

                case "funcionario":
                    return "NomeFunc";

                case "dataexportacao":
                    return "DataExportacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
