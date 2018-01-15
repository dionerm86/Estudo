
namespace Glass.Api.Host.Areas.App.Models.Mensagem
{
    public class EnvioMensagem
    {
        public string Assunto { get; set; }

        public int[] Destinatarios { get; set; }

        public string Mensagem { get; set; }
    }
}
