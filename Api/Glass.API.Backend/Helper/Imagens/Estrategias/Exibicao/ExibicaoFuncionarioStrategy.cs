// <copyright file="ExibicaoFuncionarioStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Imagens.Exibicao;
using Glass.Data.DAL;
using Glass.Global.UI.Web.Process;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao
{
    /// <summary>
    /// Classe para tratamento de imagens de funcionário para o controle de exibição.
    /// </summary>
    internal class ExibicaoFuncionarioStrategy : BaseExibicaoStrategy, IExibicao
    {
        private readonly GDASession sessao;
        private readonly ApiController apiController;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ExibicaoFuncionarioStrategy"/>.
        /// </summary>
        /// <param name="sessao">A transação com o banco de dados.</param>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public ExibicaoFuncionarioStrategy(GDASession sessao, ApiController apiController)
        {
            this.sessao = sessao;
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarItem(int id)
        {
            if (!FuncionarioDAO.Instance.Exists(this.sessao, id))
            {
                return this.apiController.NaoEncontrado($"Funcionário {id} não encontrado.");
            }

            if (!Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.PossuiImagem(id))
            {
                return this.apiController.SemConteudo();
            }

            return null;
        }

        /// <inheritdoc/>
        public DadosImagemDto RecuperarDados(int id)
        {
            var url = Global.UI.Web.Process.Funcionarios.FuncionarioRepositorioImagens.Instance.ObtemUrl(id);
            return this.ObterDadosImagem(url);
        }
    }
}
