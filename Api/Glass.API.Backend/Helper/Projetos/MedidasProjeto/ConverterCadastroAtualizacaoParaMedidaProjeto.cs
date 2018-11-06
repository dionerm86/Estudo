// <copyright file="ConverterCadastroAtualizacaoParaMedidaProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Projetos.MedidasProjeto
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de medidas de projeto.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaMedidaProjeto
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<MedidaProjeto> medidaProjeto;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaMedidaProjeto"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A medida de projeto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaMedidaProjeto(
            CadastroAtualizacaoDto cadastro,
            MedidaProjeto atual = null)
        {
            this.cadastro = cadastro;
            this.medidaProjeto = new Lazy<MedidaProjeto>(() =>
            {
                var destino = atual ?? new MedidaProjeto();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de medida de projeto preenchida.</returns>
        public MedidaProjeto ConverterParaMedidaProjeto()
        {
            return this.medidaProjeto.Value;
        }

        private void ConverterDtoParaModelo(MedidaProjeto destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.ValorPadrao = this.cadastro.ObterValorNormalizado(c => c.ValorPadrao, destino.ValorPadrao);
            destino.ExibirMedidaExata = this.cadastro.ObterValorNormalizado(c => c.ExibirApenasEmCalculosDeMedidaExata, destino.ExibirMedidaExata);
            destino.ExibirApenasFerragensAluminios = this.cadastro.ObterValorNormalizado(c => c.ExibirApenasEmCalculosDeFerragensEAluminios, destino.ExibirApenasFerragensAluminios);
            destino.IdGrupoMedProj = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdGrupoMedidaProjeto, (int?)destino.IdGrupoMedProj);
        }
    }
}
