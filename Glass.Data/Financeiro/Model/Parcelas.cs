using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelasDAO))]
    [PersistenceClass("parcelas")]
    public class Parcelas : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPARCELA", PersistenceParameterType.IdentityKey)]
        public int IdParcela { get; set; }

        [PersistenceProperty("NUMPARCELAS")]
        public int NumParcelas { get; set; }

        [PersistenceProperty("DIAS")]
        public string Dias { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("PARCELAPADRAO")]
        public bool ParcelaPadrao { get; set; }

        [PersistenceProperty("ParcelaAVista")]
        public bool ParcelaAVista { get; set; }

        [PersistenceProperty("Desconto")]
        public decimal Desconto { get; set; }

        [PersistenceProperty("Situacao")]
        public Situacao Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NAOUSAR", DirectionParameter.InputOptional)]
        public bool NaoUsar { get; set; }

        #endregion

        #region Propriedades de Suporte

        public int[] NumeroDias
        {
            get
            {
                if (String.IsNullOrEmpty(Dias))
                    return new int[0];

                string[] dias = Dias.Split(',');
                int[] retorno = new int[dias.Length];

                int i = 0;
                foreach (string s in dias)
                    retorno[i++] = Glass.Conversoes.StrParaInt(s);

                return retorno;
            }
            set
            {
                if (value == null || value.Length == 0)
                    Dias = null;

                string dias = "";
                foreach (int i in value)
                    dias += "," + i;

                Dias = dias.Substring(1);
            }
        }

        public string DescrNumParcelas
        {
            get
            {
                if (NumParcelas == 0)
                    return "À vista";
                else if (NumParcelas == 1)
                    return "1 parcela";
                else
                    return NumParcelas + " parcelas";
            }
        }

        public string DescrCompleta
        {
            get { return DescrNumParcelas + (NumParcelas > 0 || String.Compare(Descricao, DescrNumParcelas, true) != 0 ? " - " + Descricao : ""); }
        }

        public int TipoPagto
        {
            get
            {
                if (NumParcelas == 0)
                    return 0;
                else
                    return 1;
            }
        }

        #endregion
    }
}