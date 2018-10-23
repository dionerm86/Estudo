// <copyright file="ConverterCadastroAtualizacaoParaSetor.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Producao.V1.Setores.CadastroAtualizacao;
using Glass.PCP.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Producao.Setores
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de setores.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaSetor
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Setor> setor;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaSetor"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O setor atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaSetor(
            CadastroAtualizacaoDto cadastro,
            Setor atual = null)
        {
            this.cadastro = cadastro;
            this.setor = new Lazy<Setor>(() =>
            {
                var destino = atual ?? new Setor();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de setor preenchida.</returns>
        public Setor ConverterParaSetor()
        {
            return this.setor.Value;
        }

        private void ConverterDtoParaModelo(Setor destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.Sigla = this.cadastro.ObterValorNormalizado(c => c.Codigo, destino.Sigla);
            destino.NumeroSequencia = this.cadastro.ObterValorNormalizado(c => c.Sequencia, destino.NumeroSequencia);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.Altura = this.cadastro.ObterValorNormalizado(c => c.AlturaMaxima, destino.Altura);
            destino.Largura = this.cadastro.ObterValorNormalizado(c => c.LarguraMaxima, destino.Largura);
            destino.CapacidadeDiaria = this.cadastro.ObterValorNormalizado(c => c.CapacidadeDiaria, destino.CapacidadeDiaria);
            destino.IgnorarCapacidadeDiaria = this.cadastro.ObterValorNormalizado(c => c.IgnorarCapacidadeDiaria, destino.IgnorarCapacidadeDiaria);
            destino.Cor = this.cadastro.ObterValorNormalizado(c => c.CorSetor, destino.Cor);
            destino.CorTela = this.cadastro.ObterValorNormalizado(c => c.CorTela, destino.CorTela);
            destino.ExibirSetores = this.cadastro.ObterValorNormalizado(c => c.ExibirSetoresLeituraPeca, destino.ExibirSetores);
            destino.ExibirRelatorio = this.cadastro.ObterValorNormalizado(c => c.ExibirNaListaERelatorio, destino.ExibirRelatorio);
            destino.ExibirPainelComercial = this.cadastro.ObterValorNormalizado(c => c.ExibirPainelComercial, destino.ExibirPainelComercial);
            destino.ExibirPainelProducao = this.cadastro.ObterValorNormalizado(c => c.ExibirPainelProducao, destino.ExibirPainelProducao);
            destino.ExibirImagemCompleta = this.cadastro.ObterValorNormalizado(c => c.ExibirImagemCompleta, destino.ExibirImagemCompleta);
            destino.ConsultarAntes = this.cadastro.ObterValorNormalizado(c => c.ConsultarAntesDaLeitura, destino.ConsultarAntes);
            destino.Tipo = this.cadastro.ObterValorNormalizado(c => c.Tipo, destino.Tipo);
            destino.Corte = this.cadastro.ObterValorNormalizado(c => c.Corte, destino.Corte);
            destino.Laminado = this.cadastro.ObterValorNormalizado(c => c.Laminado, destino.Laminado);
            destino.Forno = this.cadastro.ObterValorNormalizado(c => c.Forno, destino.Forno);
            destino.EntradaEstoque = this.cadastro.ObterValorNormalizado(c => c.EntradaEstoque, destino.EntradaEstoque);
            destino.GerenciarFornada = this.cadastro.ObterValorNormalizado(c => c.GerenciaFornada, destino.GerenciarFornada);
            destino.DesafioPerda = (double)this.cadastro.ObterValorNormalizado(c => c.DesafioPerda, (decimal)destino.DesafioPerda);
            destino.MetaPerda = (double)this.cadastro.ObterValorNormalizado(c => c.MetaPerda, (decimal)destino.MetaPerda);
            destino.ImpedirAvanco = this.cadastro.ObterValorNormalizado(c => c.ImpedirAvanco, destino.ImpedirAvanco);
            destino.InformarRota = this.cadastro.ObterValorNormalizado(c => c.InformarRota, destino.InformarRota);
            destino.InformarCavalete = this.cadastro.ObterValorNormalizado(c => c.InformarCavalete, destino.InformarCavalete);
            destino.PermitirLeituraForaRoteiro = this.cadastro.ObterValorNormalizado(c => c.PermitirLeituraForaRoteiro, destino.PermitirLeituraForaRoteiro);
            destino.TempoLogin = this.cadastro.ObterValorNormalizado(c => c.TempoLogin, destino.TempoLogin);
            destino.TempoAlertaInatividade = this.cadastro.ObterValorNormalizado(c => c.TempoAlertaInatividade, destino.TempoAlertaInatividade);
        }
    }
}
