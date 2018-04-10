using System;
using System.Linq;
using System.Web;

namespace Glass.Financeiro.UI.Web.Process.Handlers
{
    /// <summary>
    /// Handler responsavel por realizar o processamento do retorno do recebimento da CAPPTA
    /// </summary>
    public class RetornoRecebimentoCappta : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                //Carrega a classe com os dados de retorno da transação
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Glass.Data.Model.CapptaRetornoRecebimento>(context.Request.Form["respostaRecebimento"]);

                //Finaliza a transação
                Glass.Data.DAL.TransacaoCapptaTefDAO.Instance.FinalizarTransacao(response);

                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    sucesso = response.Sucesso,
                    mensagemErro = response.MensagemErro,
                    mensagemRetorno = response.MensagemRetorno,
                    idReferencia = response.IdReferencia,
                    tipoRecebimento = (int)response.TipoRecebimento,
                    codigosAdministrativos = response.Recebimentos?.Select(f => f.CodigoAdministrativo)
                }));
            }
            catch (Exception ex)
            {
                Glass.Data.DAL.ErroDAO.Instance.InserirFromException("RetornoRecebimentoCappta", ex);

                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    sucesso = false,
                    mensagemErro = ex.Message
                }));
            }
        }

        public bool IsReusable => false;
    }
}
