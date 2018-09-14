// <copyright file="ConversorParticipanteModeloParaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.NotasFiscais.TiposParticipantes;
using Glass.Data.Model;
using System;
using System.Linq;
using static Glass.Data.EFD.DataSourcesEFD;

namespace Glass.API.Backend.Helper.NotasFiscais
{
    /// <summary>
    /// Classe com os métodos de conversão para o participante a partir de um modelo.
    /// </summary>
    internal class ConversorParticipanteModeloParaDto
    {
        private IdNomeDto participante;
        private IdNomeDto tipoParticipante;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorParticipanteModeloParaDto"/>.
        /// </summary>
        /// <param name="estoqueProduto">Os dados de estoque que serão convertidos.</param>
        public ConversorParticipanteModeloParaDto(ProdutoLoja estoqueProduto)
        {
            this.Converter(estoqueProduto);
        }

        /// <summary>
        /// Obtém os dados do participante.
        /// </summary>
        /// <returns>O DTO convertido.</returns>
        public IdNomeDto ObterParticipante()
        {
            return this.participante;
        }

        /// <summary>
        /// Obtém os dados do tipo de participante.
        /// </summary>
        /// <returns>O DTO convertido.</returns>
        public TipoParticipanteDto ObterTipoParticipante()
        {
            return this.tipoParticipante != null
                ? new TipoParticipanteDto(this.tipoParticipante)
                : null;
        }

        private void Converter(ProdutoLoja estoqueProduto)
        {
            var tiposParticipantes = new ConversorEnum<TipoPartEnum>()
                .ObterTraducao();

            Func<TipoPartEnum, IdNomeDto> obterTipoParticipante =
                tipo => tiposParticipantes.First(t => t.Id == (int)tipo);

            this.ConverterCliente(estoqueProduto, obterTipoParticipante(TipoPartEnum.Cliente));
            this.ConverterFornecedor(estoqueProduto, obterTipoParticipante(TipoPartEnum.Fornecedor));
            this.ConverterLoja(estoqueProduto, obterTipoParticipante(TipoPartEnum.Loja));
            this.ConverterTransportador(estoqueProduto, obterTipoParticipante(TipoPartEnum.Transportador));
            this.ConverterAdministradoraCartao(estoqueProduto, obterTipoParticipante(TipoPartEnum.AdministradoraCartao));
        }

        private void ConverterCliente(ProdutoLoja estoqueProduto, IdNomeDto tipoParticipanteCliente)
        {
            if (!estoqueProduto.IdCliente.HasValue)
            {
                return;
            }

            this.participante = new IdNomeDto
            {
                Id = estoqueProduto.IdCliente,
                Nome = estoqueProduto.NomeCliente,
            };

            this.tipoParticipante = tipoParticipanteCliente;
        }

        private void ConverterFornecedor(ProdutoLoja estoqueProduto, IdNomeDto tipoParticipanteFornecedor)
        {
            if (!estoqueProduto.IdFornec.HasValue)
            {
                return;
            }

            this.participante = new IdNomeDto
            {
                Id = estoqueProduto.IdFornec,
                Nome = estoqueProduto.NomeFornec,
            };

            this.tipoParticipante = tipoParticipanteFornecedor;
        }

        private void ConverterLoja(ProdutoLoja estoqueProduto, IdNomeDto tipoParticipanteLoja)
        {
            if (!estoqueProduto.IdLojaTerc.HasValue)
            {
                return;
            }

            this.participante = new IdNomeDto
            {
                Id = estoqueProduto.IdLojaTerc,
                Nome = estoqueProduto.NomeLojaTerc,
            };

            this.tipoParticipante = tipoParticipanteLoja;
        }

        private void ConverterTransportador(ProdutoLoja estoqueProduto, IdNomeDto tipoParticipanteTransportador)
        {
            if (!estoqueProduto.IdTransportador.HasValue)
            {
                return;
            }

            this.participante = new IdNomeDto
            {
                Id = estoqueProduto.IdTransportador,
                Nome = estoqueProduto.NomeTransportador,
            };

            this.tipoParticipante = tipoParticipanteTransportador;
        }

        private void ConverterAdministradoraCartao(
            ProdutoLoja estoqueProduto,
            IdNomeDto tipoParticipanteAdministradoraCartao)
        {
            if (!estoqueProduto.IdAdminCartao.HasValue)
            {
                return;
            }

            this.participante = new IdNomeDto
            {
                Id = estoqueProduto.IdAdminCartao,
                Nome = estoqueProduto.NomeAdminCartao,
            };

            this.tipoParticipante = tipoParticipanteAdministradoraCartao;
        }
    }
}
