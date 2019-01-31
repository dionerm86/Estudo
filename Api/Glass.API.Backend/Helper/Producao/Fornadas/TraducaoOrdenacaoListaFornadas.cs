// <copyright file="TraducaoOrdenacaoListaFornadas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Fornadas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de fornadas.
    /// </summary>
    internal class TraducaoOrdenacaoListaFornadas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFornadas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaFornadas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get
            {
                return "IdFornada DESC";
            }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "IdFornada DESC";

                case "funcionario":
                    return "DescrUsuCad";

                case "datacadastro":
                    return "DataCad";

                case "capacidade":
                    return campo;

                case "metroquadradolido":
                    return "M2Lido";

                case "aproveitamento":
                    return "Aproveitamento";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}