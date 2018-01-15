using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosArquivoFCIDAO))]
    [PersistenceClass("produtos_arquivo_fci")]
    public class ProdutosArquivoFCI : Sync.Fiscal.EFD.Entidade.IProdutoArquivoFCI
    {
        #region Propiedades

        [PersistenceProperty("IDPRODARQUIVOFCI",PersistenceParameterType.IdentityKey)]
        public uint IdProdArquivoFCI { get; set; }

        [PersistenceProperty("IDARQUIVOFCI")]
        public uint IdArquivoFCI { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("PARCELAIMPORTADA")]
        public decimal ParcelaImportada { get; set; }

        [PersistenceProperty("SAIDAINTERESTADUAL")]
        public decimal SaidaInterestadual { get; set; }

        [PersistenceProperty("CONTEUDOIMPORTACAO")]
        public decimal ConteudoImportacao { get; set; }

        [PersistenceProperty("NUMCONTROLEFCI")]
        public byte[] NumControleFci { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("Ncm", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        [PersistenceProperty("GTINProduto", DirectionParameter.InputOptional)]
        public int GTINProduto { get; set; }

        [PersistenceProperty("Unidade", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NumControleFciStr
        {
            get { return NumControleFci != null && NumControleFci.Length > 0 ? new Guid(NumControleFci).ToString().ToUpper() : null; }
            set { NumControleFci = new Guid(value).ToByteArray(); }
        }

        public string CodInternoDescrProduto
        {
            get { return CodInterno + " - " + DescrProduto; }
        }

        #endregion

        #region IProdutoArquivoFCI Members

        int Sync.Fiscal.EFD.Entidade.IProdutoArquivoFCI.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        #endregion
    }
}
