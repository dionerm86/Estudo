// <copyright file="ChapasMateriaPrimaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima
{
    /// <summary>
    /// Controller de chapas de matéria prima.
    /// </summary>
    [RoutePrefix("api/v1/produtos/materiaPrima")]
    public partial class ChapasMateriaPrimaController : BaseController
    {
        private IHttpActionResult ValidarIdCorVidro(int idCorVidro)
        {
            if (idCorVidro <= 0)
            {
                return this.ErroValidacao("Identificador da cor do vidro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCorVidro(GDASession sessao, int idCorVidro)
        {
            var validacao = this.ValidarIdCorVidro(idCorVidro);

            if (validacao == null && !CorVidroDAO.Instance.Exists(sessao, idCorVidro))
            {
                return this.NaoEncontrado("Cor do vidro não encontrada.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarEspessura(decimal espessura)
        {
            if (espessura <= 0)
            {
                return this.ErroValidacao("Espessura do vidro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaEspessuraECorVidro(GDASession sessao, int idCorVidro, decimal espessura)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdCorVidro(sessao, idCorVidro)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarEspessura(espessura)));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }
    }
}
