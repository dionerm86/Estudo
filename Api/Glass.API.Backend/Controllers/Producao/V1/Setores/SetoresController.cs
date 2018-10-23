// <copyright file="SetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores de produção.
    /// </summary>
    [RoutePrefix("api/v1/producao/setores")]
    public partial class SetoresController : BaseController
    {
        private IHttpActionResult ValidarIdSetor(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do setor deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdSetor(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdSetor(id);

            if (validacao == null && !SetorDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Setor não encontrado.");
            }

            return null;
        }
    }
}
