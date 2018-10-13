// <copyright file="ExibicaoSvgProjetoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Imagens.Exibicao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System.IO;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao
{
    /// <summary>
    /// Classe para tratamento de imagens de peça de produção para o controle de exibição.
    /// </summary>
    internal class ExibicaoSvgProjetoStrategy : BaseExibicaoStrategy, IExibicao
    {
        private readonly GDASession sessao;
        private readonly ApiController apiController;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ExibicaoSvgProjetoStrategy"/>.
        /// </summary>
        /// <param name="sessao">A transação com o banco de dados.</param>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public ExibicaoSvgProjetoStrategy(GDASession sessao, ApiController apiController)
        {
            this.sessao = sessao;
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarItem(int id)
        {
            if (!ProdutosPedidoEspelhoDAO.Instance.Exists(this.sessao, id))
            {
                return this.apiController.NaoEncontrado($"Produto de pedido conferência {id} não encontrado.");
            }

            var caminhoImagem = this.ObterCaminhoArquivoImagemProjeto(id);

            if (!File.Exists(caminhoImagem))
            {
                return this.apiController.SemConteudo();
            }

            return null;
        }

        /// <inheritdoc/>
        public DadosImagemDto RecuperarDados(int id)
        {
            var caminhoImagem = this.ObterCaminhoArquivoImagemProjeto(id);
            var arquivoImagemProjeto = File.ReadAllText(caminhoImagem);

            return new DadosImagemDto
            {
                SvgImagem = arquivoImagemProjeto,
            };
        }

        private string ObterCaminhoArquivoImagemProjeto(int id)
        {
            return PCPConfig.CaminhoSalvarCadProject(true)
                + id
                + ".svg";
        }
    }
}
