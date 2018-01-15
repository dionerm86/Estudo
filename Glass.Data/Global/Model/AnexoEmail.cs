using GDA;
using Glass.Data.Helper;
using System.Web;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AnexoEmailDAO))]
    [PersistenceClass("anexo_email")]
    public class AnexoEmail
    {
        #region Variaveis Locais

        private byte[] _dados;

        #endregion

        #region Construtores

        public AnexoEmail()
        {
        }

        public AnexoEmail(byte[] dados, string nomeArquivo)
        {
            _dados = dados;
            NomeArquivo = nomeArquivo;
        }

        public AnexoEmail(string caminho, string nomeArquivo)
        {
            Caminho = caminho;
            NomeArquivo = nomeArquivo;
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDANEXOEMAIL", PersistenceParameterType.IdentityKey)]
        public uint IdAnexoEmail { get; set; }

        [PersistenceProperty("IDEMAIL")]
        public uint IdEmail { get; set; }

        [PersistenceProperty("CAMINHOARQUIVO")]
        public string Caminho { get; set; }

        [PersistenceProperty("NOMEARQUIVO")]
        public string NomeArquivo { get; set; }

        #endregion

        #region Propriedades de Suporte

        private bool _getDados = false;

        internal bool GetDados
        {
            get { return _getDados; }
            set { _getDados = value; }
        }

        internal HttpContext Context { get; set; }

        internal byte[] Dados
        {
            get
            {
                if (!_getDados)
                    return new byte[0];

                if (_dados == null)
                {
                    if (Caminho.StartsWith("BoletoNFe", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        var boleto = System.IO.File.ReadAllBytes(Armazenamento.ArmazenamentoIsolado.DiretorioBoletos + string.Format("\\anexo{0}.pdf", IdAnexoEmail));

                        if (boleto != null)
                            _dados = boleto;
                    }

                    else if (Caminho.ToLower().Contains("idoc="))
                    {
                        uint idOC = Glass.Conversoes.StrParaUint(Caminho.Substring(5));
                        _dados = Handlers.OrdemCarga.GetBytesRelatorio(Context, idOC);
                    }
                    else if (Caminho.ToLower().IndexOf("danfe") == -1)
                    {
                        _dados = Utils.GetImageFromRequest(Context, Caminho);
                    }

                    else
                    {
                        uint idNf = Glass.Conversoes.StrParaUint(Caminho.Substring(Caminho.IndexOf("idNf=") + 5));
                        _dados = Handlers.Danfe.GetBytesRelatorio(Context, idNf, false);
                    }
                }

                return _dados;
            }
        }

        #endregion
    }
}