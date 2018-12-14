// <copyright file="PostGruposMedidaProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.Medidas.Grupos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto.GruposMedidaProjeto
{
    /// <summary>
    /// Controller de grupos de medida de projeto.
    /// </summary>
    public partial class GruposMedidaProjetoController : BaseController
    {
        /// <summary>
        /// Cadastra um grupo de medida de projeto.
        /// </summary>
        /// <param name="dados">Objeto contendo dados para inserção de um grupo de medida de projeto.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Grupo de medida projeto cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarGrupoMedidaProjeto([FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var grupoMedidaProjeto = new ConverterCadastroAtualizacaoParaGrupoMedidaProjeto(dados)
                        .ConverterParaGrupoMedidaProjeto();

                    var id = GrupoMedidaProjetoDAO.Instance.Insert(sessao, grupoMedidaProjeto);

                    sessao.Commit();

                    return this.Criado("Grupo de medida de projeto cadastrado com sucesso!", id);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar grupo de medida de projeto.", ex);
                }
            }
        }
    }
}