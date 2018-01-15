using System;
using System.Xml;

namespace Glass
{
    public class ConsultaCep
    {
        #region Váriaveis Locais

        private string _uf;
        private string _cidade;
        private uint _idCidade;
        private string _bairro;
        private string _tipo_logradouro;
        private string _logradouro;
        private int _resultado;
        private string _resultado_txt;

        #endregion

        #region Propiedades

        public string UF
        {
            get { return _uf.ToUpper(); }
        }

        public string Cidade
        {
            get { return _cidade.ToUpper(); }
        }

        public uint IdCidade
        {
            get { return _idCidade; }
            set { _idCidade = value; }
        }

        public string Bairro
        {
            get { return _bairro.ToUpper(); }
        }

        public string TipoLogradouro
        {
            get { return _tipo_logradouro.ToUpper(); }
        }

        public string Logradouro
        {
            get { return _logradouro.ToUpper(); }
        }

        public int Resultado
        {
            get { return _resultado; }
        }

        public string ResultadoTexto
        {
            get { return _resultado_txt.ToUpper(); }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// Consulta um CEP via WebService e salva o resultado nessa classe.
        /// </summary>
        /// <param name="cep">O CEP que será consultado.</param>
        public ConsultaCep(string cep)
        {
            _uf = "";
            _cidade = "";
            _bairro = "";
            _tipo_logradouro = "";
            _logradouro = "";
            _resultado = 0;
            _resultado_txt = "CEP não encontrado";

            if (string.IsNullOrEmpty(System.Configuration.ConfigurationSettings.AppSettings["consultaCep"]))
            {
                _resultado_txt = "Configuração de consulta de cep inexistente";
                return;
            }

            // Url para requisição do cep
            string webserviceUrl =
                "http://webservice.uni5.net/web_cep.php" +
                    "?auth=" + System.Configuration.ConfigurationSettings.AppSettings["consultaCep"] +
                    "&formato=xml&cep=" + cep.Replace("-", "").Replace(".", "").Trim();

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(webserviceUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (doc == null || doc.ToString() == String.Empty)
            {
                _resultado_txt = "Não foi possivel consultar o cep; serviço indisponivel";
                return;
            }

            switch (doc["webservicecep"]["resultado"].InnerText)
            {
                case "1":
                    _uf = doc["webservicecep"]["uf"].InnerText;
                    _cidade = doc["webservicecep"]["cidade"].InnerText;
                    _bairro = doc["webservicecep"]["bairro"].InnerText;
                    _tipo_logradouro = doc["webservicecep"]["tipo_logradouro"].InnerText;
                    _logradouro = doc["webservicecep"]["logradouro"].InnerText;
                    //_idCidade = CidadeDAO.Instance.GetCidadeByNomeUf(_cidade, _uf);
                    _resultado_txt = "CEP completo";
                    _resultado = 1;
                    break;
                case "2":
                    _uf = doc["webservicecep"]["uf"].InnerText;
                    _cidade = doc["webservicecep"]["cidade"].InnerText;
                    //_idCidade = CidadeDAO.Instance.GetCidadeByNomeUf(_cidade, _uf);
                    _resultado_txt = "CEP único";
                    _resultado = 2;
                    break;
                default:
                    _resultado_txt = doc["webservicecep"]["resultado_txt"].InnerText.Replace("sucesso - ", "");
                    break;
            }
        }

        #endregion
    }
}
