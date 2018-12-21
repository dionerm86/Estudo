// <copyright file="TraducaoOrdenacaoListaTrocasDevolucoes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de Troca/Devolucao.
    /// </summary>
    internal class TraducaoOrdenacaoListaTrocasDevolucoes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTrocasDevolucoes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTrocasDevolucoes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "idTrocaDevolucao DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "idTrocaDevolucao";

                case "pedido":
                    return "idPedido";

                case "funcionario":
                    return "nomeFunc";

                case "cliente":
                    return "idCliente";

                case "data":
                    return "dataTroca";

                case "origem":
                    return "descrOrigemTrocaDevolucao";

                case "observacao":
                    return "obs";

                case "usuariocadastro":
                    return "nomeUsuCad";

                case "tipo":
                case "dataerro":
                case "loja":
                case "creditogerado":
                case "valorexcedente":
                case "situacao":
                case "setor":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
