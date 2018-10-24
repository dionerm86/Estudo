// <copyright file="ClassificacoesRoteiroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros.ClassificacoesRoteiro
{
    /// <summary>
    /// Controller de classificações de roteiro.
    /// </summary>
    [RoutePrefix("api/v1/producao/roteiros/classificacoes")]
    public partial class ClassificacoesRoteiroController : BaseController
    {
        private IHttpActionResult ValidarIdClassificacaoRoteiro(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da classificação de roteiro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdClassificacaoRoteiro(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdClassificacaoRoteiro(id);

            var classificacao = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>()
                .ObtemClassificacao(id);

            if (validacao == null && classificacao != null)
            {
                return this.NaoEncontrado("Classificação de roteiro não encontrada.");
            }

            return null;
        }
    }
}
