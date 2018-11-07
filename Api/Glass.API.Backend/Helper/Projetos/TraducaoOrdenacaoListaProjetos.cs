// <copyright file="TraducaoOrdenacaoListaProjetos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de projetos.
    /// </summary>
    internal class TraducaoOrdenacaoListaProjetos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProjetos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaProjetos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdProjeto ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdProjeto";

                case "cliente":
                    return "NomeCliente";

                case "loja":
                    return "NomeLoja";

                case "funcionario":
                    return "NomeFunc";

                case "datacadastro":
                    return "DataCad";

                case "total":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
