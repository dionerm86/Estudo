// <copyright file="FuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de funcionários.
    /// </summary>
    [RoutePrefix("api/v1/funcionarios")]
    public partial class FuncionariosController : BaseController
    {
        private IHttpActionResult ValidarIdFuncionario(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do funcionário deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaFuncionario(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFuncionario(id);

            if (validacao == null && !FuncionarioDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Funcionário não encontrado.");
            }

            return null;
        }
    }
}
