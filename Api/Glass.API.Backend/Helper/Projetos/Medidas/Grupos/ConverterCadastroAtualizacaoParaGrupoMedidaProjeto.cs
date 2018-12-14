// <copyright file="ConverterCadastroAtualizacaoParaGrupoMedidaProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Projetos.Medidas.Grupos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de grupos de medida de projeto.
    /// </summary>
    public class ConverterCadastroAtualizacaoParaGrupoMedidaProjeto
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<GrupoMedidaProjeto> grupoMedidaProjeto;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaGrupoMedidaProjeto"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O grupo de medida de projeto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaGrupoMedidaProjeto(
            CadastroAtualizacaoDto cadastro,
            GrupoMedidaProjeto atual = null)
        {
            this.cadastro = cadastro;
            this.grupoMedidaProjeto = new Lazy<GrupoMedidaProjeto>(() =>
            {
                var destino = atual ?? new GrupoMedidaProjeto();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model do grupo de medida de projeto preenchida.</returns>
        public GrupoMedidaProjeto ConverterParaGrupoMedidaProjeto()
        {
            return this.grupoMedidaProjeto.Value;
        }

        private void ConverterDtoParaModelo(GrupoMedidaProjeto destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
        }
    }
}