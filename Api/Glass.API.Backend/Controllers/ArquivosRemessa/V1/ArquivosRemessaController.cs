// <copyright file="ArquivosRemessaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ArquivosRemessa.V1
{
    /// <summary>
    /// Controller de arquivos de remessa.
    /// </summary>
    [RoutePrefix("api/v1/arquivosRemessa")]
    public partial class ArquivosRemessaController : BaseController
    {
        private IHttpActionResult ValidarIdArquivoRemessa(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do arquivo de remessa deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdArquivoRemessa(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdArquivoRemessa(id);

            if (validacao == null && !ArquivoRemessaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Arquivo de remessa não encontrado.");
            }

            return validacao;
        }
    }
}