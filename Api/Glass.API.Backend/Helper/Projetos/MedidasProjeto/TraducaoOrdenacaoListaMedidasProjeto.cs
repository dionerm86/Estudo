// <copyright file="TraducaoOrdenacaoListaMedidasProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.MedidasProjeto
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de medidas de projeto.
    /// </summary>
    internal class TraducaoOrdenacaoListaMedidasProjeto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaMedidasProjeto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaMedidasProjeto(string ordenacao)
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

                case "exibirapenasemcalculosdemedidaexata":
                    return "ExibirMedidaExata";

                case "exibirapenasemcalculosdeferragensealuminios":
                    return "ExibirApenasFerragensAluminios";

                case "grupomedidaprojeto":
                    return "IdGrupoMedProj";

                case "valorpadrao":
                    return "ValorPadrao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
