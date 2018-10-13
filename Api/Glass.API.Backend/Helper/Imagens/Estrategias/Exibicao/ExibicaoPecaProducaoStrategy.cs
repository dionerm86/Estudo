// <copyright file="ExibicaoPecaProducaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Imagens.Exibicao;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao
{
    /// <summary>
    /// Classe para tratamento de imagens de peça de produção para o controle de exibição.
    /// </summary>
    internal class ExibicaoPecaProducaoStrategy : BaseExibicaoStrategy, IExibicao
    {
        private readonly GDASession sessao;
        private readonly ApiController apiController;
        private ProdutoPedidoProducao peca;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ExibicaoPecaProducaoStrategy"/>.
        /// </summary>
        /// <param name="sessao">A transação com o banco de dados.</param>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public ExibicaoPecaProducaoStrategy(GDASession sessao, ApiController apiController)
        {
            this.sessao = sessao;
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarItem(int id)
        {
            this.peca = ProdutoPedidoProducaoDAO.Instance.GetElementByPrimaryKey(this.sessao, id);

            if (this.peca == null)
            {
                return this.apiController.NaoEncontrado($"Peça de produção {id} não encontrada.");
            }

            if (this.peca.ImagemPecaUrl == null)
            {
                return this.apiController.SemConteudo();
            }

            return null;
        }

        /// <inheritdoc/>
        public DadosImagemDto RecuperarDados(int id)
        {
            return this.ObterDadosImagem(this.peca.ImagemPecaUrl);
        }
    }
}
