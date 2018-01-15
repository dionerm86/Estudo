using System.Collections.Generic;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(VendasDAO))]
    [PersistenceClass("vendas")]
    public class Vendas
    {
        #region Propriedades

        [PersistenceProperty("IdCliente")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NomeCliente")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("IdFuncionario")]
        public uint IdFuncionario { get; set; }

        [PersistenceProperty("NomeFuncionario")]
        public string NomeFuncionario { get; set; }

        [PersistenceProperty("IdComissionado")]
        public uint IdComissionado { get; set; }

        [PersistenceProperty("NomeComissionado")]
        public string NomeComissionado { get; set; }

        [PersistenceProperty("MesesVenda")]
        public string MesesVenda { get; set; }

        [PersistenceProperty("ValoresVenda")]
        public string ValoresVenda { get; set; }

        [PersistenceProperty("Total")]
        public decimal Total { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("TotalItens")]
        public int TotalItens { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string IdNomeComissionado
        {
            get { return IdComissionado + " - " + NomeComissionado; }
        }

        public string[] MesVenda
        {
            get { return MesesVenda.Split(','); }
        }

        public decimal[] ValorVenda
        {
            get
            {
                List<decimal> retorno = new List<decimal>();
                foreach (string s in ValoresVenda.Split(','))
                    retorno.Add(decimal.Parse(s.Replace('.', ',')));

                return retorno.ToArray();
            }
        }

        public string MediaCompraCliente
        {
            get
            {
                if (IdCliente == 0)
                    return "";

                decimal valorMediaIni = Glass.Data.DAL.ClienteDAO.Instance.ObtemValorCampo<decimal>("valorMediaIni", "id_Cli=" + IdCliente);
                decimal valorMediaFim = Glass.Data.DAL.ClienteDAO.Instance.ObtemValorCampo<decimal>("valorMediaFim", "id_Cli=" + IdCliente);

                return valorMediaIni > 0 || valorMediaFim > 0 ? valorMediaIni.ToString("N") + " a " + valorMediaFim.ToString("N") : "";
            }
        }

        public string TresPrimeirosNomesVendedor
        {
            get
            {
                return BibliotecaTexto.GetThreeFirstWords(NomeFuncionario);
            }
        }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRICAOTABELADESCONTOACRESCIMO", DirectionParameter.InputOptional)]
        public string DescricaoTabelaDescontoAcrescimo { get; set; }

        #endregion
    }
}