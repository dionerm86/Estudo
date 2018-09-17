// <copyright file="ConversorParticipanteDtoParaModelo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using System;
using static Glass.Data.EFD.DataSourcesEFD;

namespace Glass.API.Backend.Helper.NotasFiscais
{
    /// <summary>
    /// Classe com os métodos de conversão para o participante a partir do estoque.
    /// </summary>
    internal class ConversorParticipanteDtoParaModelo
    {
        private readonly int? idParticipante;
        private readonly TipoPartEnum? tipoParticipante;

        private readonly Lazy<int?> idCliente;
        private readonly Lazy<int?> idFornecedor;
        private readonly Lazy<int?> idLoja;
        private readonly Lazy<int?> idTransportador;
        private readonly Lazy<int?> idAdministradoraCartao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorParticipanteDtoParaModelo"/>.
        /// </summary>
        /// <param name="participanteInformado">Indica se o participante foi informado.</param>
        /// <param name="idParticipante">O identificador do participante informado.</param>
        /// <param name="tipoParticipanteInformado">Indica se o tipo do participante foi informado.</param>
        /// <param name="tipoParticipante">O tipo de participante informado.</param>
        /// <param name="estoqueProdutos">O objeto com os dados do estoque que será alterado.</param>
        public ConversorParticipanteDtoParaModelo(
            bool participanteInformado,
            int? idParticipante,
            bool tipoParticipanteInformado,
            TipoPartEnum? tipoParticipante,
            ProdutoLoja estoqueProdutos)
        {
            this.idParticipante = participanteInformado
                ? idParticipante
                : this.ObterIdParticipanteModelo(estoqueProdutos);

            this.tipoParticipante = tipoParticipanteInformado
                ? tipoParticipante
                : this.ObterTipoParticipanteModelo(estoqueProdutos);

            this.idCliente = new Lazy<int?>(() => this.ConverterParticipante(TipoPartEnum.Cliente));
            this.idFornecedor = new Lazy<int?>(() => this.ConverterParticipante(TipoPartEnum.Fornecedor));
            this.idLoja = new Lazy<int?>(() => this.ConverterParticipante(TipoPartEnum.Loja));
            this.idTransportador = new Lazy<int?>(() => this.ConverterParticipante(TipoPartEnum.Transportador));
            this.idAdministradoraCartao = new Lazy<int?>(() => this.ConverterParticipante(TipoPartEnum.AdministradoraCartao));
        }

        /// <summary>
        /// Obtém o identificador do cliente.
        /// </summary>
        public int? IdCliente
        {
            get { return this.idCliente.Value; }
        }

        /// <summary>
        /// Obtém o identificador do cliente.
        /// </summary>
        public int? IdFornecedor
        {
            get { return this.idFornecedor.Value; }
        }

        /// <summary>
        /// Obtém o identificador do cliente.
        /// </summary>
        public int? IdLoja
        {
            get { return this.idLoja.Value; }
        }

        /// <summary>
        /// Obtém o identificador do cliente.
        /// </summary>
        public int? IdTransportador
        {
            get { return this.idTransportador.Value; }
        }

        /// <summary>
        /// Obtém o identificador do cliente.
        /// </summary>
        public int? IdAdministradoraCartao
        {
            get { return this.idAdministradoraCartao.Value; }
        }

        private int? ObterIdParticipanteModelo(ProdutoLoja estoqueProdutos)
        {
            return estoqueProdutos.IdCliente
                ?? estoqueProdutos.IdFornec
                ?? estoqueProdutos.IdLojaTerc
                ?? estoqueProdutos.IdTransportador
                ?? estoqueProdutos.IdAdminCartao;
        }

        private TipoPartEnum? ObterTipoParticipanteModelo(ProdutoLoja estoqueProdutos)
        {
            if (estoqueProdutos.IdCliente.HasValue)
            {
                return TipoPartEnum.Cliente;
            }

            if (estoqueProdutos.IdFornec.HasValue)
            {
                return TipoPartEnum.Fornecedor;
            }

            if (estoqueProdutos.IdLojaTerc.HasValue)
            {
                return TipoPartEnum.Loja;
            }

            if (estoqueProdutos.IdTransportador.HasValue)
            {
                return TipoPartEnum.Transportador;
            }

            if (estoqueProdutos.IdAdminCartao.HasValue)
            {
                return TipoPartEnum.AdministradoraCartao;
            }

            return null;
        }

        private int? ConverterParticipante(TipoPartEnum tipoComparar)
        {
            return this.tipoParticipante == tipoComparar
                ? this.idParticipante
                : null;
        }
    }
}
