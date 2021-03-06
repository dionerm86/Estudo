﻿// <copyright file="TraducaoOrdenacaoListaLiberacoes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Liberacoes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de liberações.
    /// </summary>
    internal class TraducaoOrdenacaoListaLiberacoes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaLiberacoes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaLiberacoes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdLiberarPedido DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdLiberarPedido";

                case "total":
                case "valoricms":
                case "dataliberacao":
                case "situacao":
                    return campo;

                case "nomecliente":
                    return "NomeClienteFantasia";

                case "descricaopagamento":
                    return "DescrFormaPagto";

                case "nomefuncionario":
                    return "NomeFunc";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
