// <copyright file="TraducaoOrdenacaoListaOrcamentos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Orcamentos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de orçamentos.
    /// </summary>
    internal class TraducaoOrdenacaoListaOrcamentos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaOrcamentos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaOrcamentos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdOrcamento DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdOrcamento";

                case "cliente":
                    return "NomeCliente";

                case "loja":
                    return "NomeLoja";

                case "idvendedor":
                    return "NomeFuncionario";

                case "datacadastro":
                    return "DataCad";

                case "telefoneorcamento":
                    return "TelCliente";

                case "total":
                    return "Total";

                case "situacao":
                    return "Situacao";

                case "idprojeto":
                    return "IdProjeto";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
