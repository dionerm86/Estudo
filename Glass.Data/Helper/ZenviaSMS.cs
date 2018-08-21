using Glass.Data.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Glass.Data.Helper
{
    public static class ZenviaSMS
    {
        public static ZenviaResponsePayload EnviaSMS(string codMensagem, string remetente, string destinatario, string mensagem)
        {
            var conta = System.Configuration.ConfigurationManager.AppSettings["ContaSMS"];
            var senha = System.Configuration.ConfigurationManager.AppSettings["SenhaSMS"];

            var auth = string.Format("Basic {0}", string.Format("{0}:{1}", conta, senha).Base64Encode());

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            var payload = new ZenviaRequestPayload();
            payload.Request.From = remetente;
            payload.Request.To = string.Format("55{0}", destinatario);
            payload.Request.Msg = mensagem;
            payload.Request.Id = codMensagem;
            payload.Request.CallbackOption = "NONE";

            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, auth);
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");

                var json = JsonConvert.SerializeObject(payload);

                try
                {
                    byte[] bytes = Encoding.GetEncoding("iso-8859-9").GetBytes(json);
                    json = Encoding.UTF8.GetString(bytes);

                    var response = wc.UploadString("https://api-rest.zenvia.com/services/send-sms", json);

                    return JsonConvert.DeserializeObject<ZenviaResponsePayload>(response);
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException("EnviarSMS", ex);
                    throw;
                }               
            }
        }

        public class ZenviaRequestPayload
        {
            public ZenviaRequestPayload()
            {
                Request = new ZenviaSmsRequest();
            }

            [JsonProperty("sendSmsRequest")]
            public ZenviaSmsRequest Request { get; set; }
        }

        public class ZenviaResponsePayload
        {
            public ZenviaResponsePayload()
            {
                Response = new ZenviaSmsResponse();
            }

            [JsonProperty("sendSmsResponse")]
            public ZenviaSmsResponse Response { get; set; }
        }

        public class ZenviaSmsRequest
        {
            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("msg")]
            public string Msg { get; set; }

            [JsonProperty("callbackOption")]
            public string CallbackOption { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public class ZenviaSmsResponse
        {
            [JsonProperty("statusCode")]
            public int StatusCode { get; set; }

            [JsonProperty("statusDescription")]
            public string StatusDescription { get; set; }

            [JsonProperty("detailCode")]
            public string DetailCode { get; set; }

            [JsonProperty("detailDescription")]
            public string DetailDescription { get; set; }
        }
    }
}
