using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosNFeDAO))]
    public class ProdutosNFe
    {
        #region Propriedades

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string NcmSh { get; set; }

        public string Cst { get; set; }

        public string Csosn { get; set; }

        public string Cfop { get; set; }

        public string Unidade { get; set; }

        public string Qtd { get; set; }

        public string VlrUnit { get; set; }

        public string VlrTotal { get; set; }

        public string BcIcms { get; set; }

        public string VlrIcms { get; set; }

        public string BcIcmsSt { get; set; }

        public string VlrIcmsSt { get; set; }

        public string VlrIpi { get; set; }

        public string AliqIcms { get; set; }

        public string AliqIpi { get; set; }

        public bool InfAdic { get; set; }

        #endregion
    }
}