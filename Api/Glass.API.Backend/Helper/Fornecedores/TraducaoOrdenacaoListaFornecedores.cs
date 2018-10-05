// <copyright file="TraducaoOrdenacaoListaFornecedores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Fornecedores
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de fornecedores.
    /// </summary>
    internal class TraducaoOrdenacaoListaFornecedores : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFornecedores"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaFornecedores(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdFornec ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdFornec";

                case "nomefantasia":
                    return "NomeFantasia";

                case "cpfcnpj":
                    return "CpfCnpj";

                case "rginscricaoestadual":
                    return "RgInscEst";

                case "dataultimacompra":
                    return "DtUltCompra";

                case "telefonecontato":
                    return "TelCont";

                case "situacao":
                    return "Situacao";

                case "vendedor":
                    return "Vendedor";

                case "celularvendedor":
                    return "TelCelVend";

                case "credito":
                    return "Credito";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
