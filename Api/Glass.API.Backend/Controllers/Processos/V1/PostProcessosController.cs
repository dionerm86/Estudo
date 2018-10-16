// <copyright file="PostProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Processos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Processos.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Processos.V1
{
    /// <summary>
    /// Controller de processos (etiqueta).
    /// </summary>
    public partial class ProcessosController : BaseController
    {
        /// <summary>
        /// Cadastra um processo de etiqueta.
        /// </summary>
        /// <param name="dadosParaCadastro">Os dados para o cadastro do processo.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Processo cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarProcesso([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarCadastroProcesso(sessao, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var processo = new ConverterCadastroAtualizacaoParaProcesso(dadosParaCadastro)
                        .ConverterParaProcesso();

                    var idProcesso = EtiquetaProcessoDAO.Instance.Insert(sessao, processo);
                    sessao.Commit();

                    return this.Criado($"Processo de etiqueta {idProcesso} inserido com sucesso!", idProcesso);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir processo.", ex);
                }
            }
        }
    }
}
