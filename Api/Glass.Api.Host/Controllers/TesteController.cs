using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Controllers
{
    public class TesteController : ApiController
    {
        public class TesteResultClass
        {
            public string Mensagem
            {
                get
                {
                    return "API funcionando!";
                }
            }
        }

        [HttpGet]
        public IHttpActionResult Teste()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Seguranca.IAutenticacaoFluxo>()
                    .TesteConexao();

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Usuário ou senha inválidos."))
                {
                    var result = Newtonsoft.Json.JsonConvert.SerializeObject(new TesteResultClass());
                    return Ok(result);
                }

                return BadRequest(ex.Message);
            }
        }
    }
}
