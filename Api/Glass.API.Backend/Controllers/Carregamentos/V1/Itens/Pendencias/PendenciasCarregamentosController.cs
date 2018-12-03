// <copyright file="PendenciasCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.Itens.Pendencias
{
    /// <summary>
    /// Controller de carregamentos pendentes.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos/itens/pendencias")]
    public partial class PendenciasCarregamentosController : BaseController
    {
        private IHttpActionResult ValidarIdCarregamentoPendente(int idCarregamento)
        {
            if (idCarregamento <= 0)
            {
                return this.ErroValidacao("Identificador do carregamento pendente deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCarregamentoPendente(GDASession sessao, int idCarregamento)
        {
            var validacao = this.ValidarIdCarregamentoPendente(idCarregamento);

            if (validacao == null && !CarregamentoDAO.Instance.Exists(sessao, idCarregamento))
            {
                return this.NaoEncontrado("Carregamento não encontrado.");
            }

            return validacao;
        }
    }
}
