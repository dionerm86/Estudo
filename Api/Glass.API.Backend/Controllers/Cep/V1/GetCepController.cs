// <copyright file="GetCepController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cep.V1.Endereco;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System.Configuration;
using System.Net.Http;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cep.V1
{
    /// <summary>
    /// Controller de CEPs.
    /// </summary>
    public partial class CepController : BaseController
    {
        private static readonly HttpClient Cliente = new HttpClient();

        /// <summary>
        /// Recupera o endereço de um CEP.
        /// </summary>
        /// <param name="cep">O CEP que será buscado.</param>
        /// <returns>Um objeto JSON com os dados do endereço relacionado ao CEP.</returns>
        [HttpGet]
        [Route("{cep}/endereco")]
        [SwaggerResponse(200, "Endereço encontrado.", Type = typeof(EnderecoDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Endereço não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterEndereco(string cep)
        {
            if (string.IsNullOrEmpty(cep))
            {
                return this.ErroValidacao("O CEP é obrigatório.");
            }

            const string url = "http://webservice.uni5.net/web_cep.php?auth={0}&formato=json&cep={1}";
            string auth = ConfigurationManager.AppSettings["consultaCep"];

            var resposta = Cliente.GetAsync(string.Format(url, auth, cep.Replace("-", string.Empty))).Result;

            if (!resposta.IsSuccessStatusCode)
            {
                return this.ErroInternoServidor("Erro ao buscar o CEP. Erro: " + resposta.Content.ToString());
            }

            var resultado = resposta.Content.ReadAsStringAsync().Result;
            var dadosCep = JsonConvert.DeserializeObject<ResultadoServicoDto>(resultado);

            if (dadosCep.Resultado == 0)
            {
                string erro = string.Empty;

                if (!string.IsNullOrWhiteSpace(dadosCep.TextoResultado))
                {
                    erro = dadosCep.TextoResultado.StartsWith("sucesso - ")
                        ? dadosCep.TextoResultado.Substring(10)
                        : dadosCep.TextoResultado;

                    erro = erro[0].ToString().ToUpper() + erro.Substring(1);
                    erro = "Erro: " + erro;
                }

                return erro == "Erro: Cep não encontrado"
                    ? this.NaoEncontrado("Cep não encontrado") as IHttpActionResult
                    : this.ErroValidacao("Erro ao buscar o CEP." + erro);
            }

            var idCidade = CidadeDAO.Instance.GetCidadeByNomeUf(dadosCep.Cidade, dadosCep.Uf);

            return this.Item(new EnderecoDto
            {
                Cep = cep,
                Logradouro = $"{dadosCep.TipoLogradouro} {dadosCep.Logradouro}",
                Bairro = dadosCep.Bairro,
                Cidade = new CidadeDto
                {
                    Id = (int)idCidade,
                    Nome = dadosCep.Cidade,
                    Uf = dadosCep.Uf,
                },
            });
        }
    }
}
