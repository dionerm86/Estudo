using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DocumentoArrecadacaoDAO))]
    [PersistenceClass("documento_arrecadacao")]
    public class DocumentoArrecadacao : Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao
    {
        #region Propriedades

        [PersistenceProperty("IDDOCARREC", PersistenceParameterType.IdentityKey)]
        public uint IdDocArrec { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("CODTIPO")]
        public int CodTipo { get; set; }

        [PersistenceProperty("UF")]
        public string Uf { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [PersistenceProperty("CODAUTBANCO")]
        public string CodAutBanco { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATAVENC")]
        public DateTime DataVenc { get; set; }

        [PersistenceProperty("DATAPAGTO")]
        public DateTime DataPagto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrCodTipo
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoDocumentoArrec(CodTipo); }
        }

        #endregion

        #region IDocumentoArrecadacao Members

        string Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.NumeroDocumento
        {
            get { return Numero; }
        }

        Sync.Fiscal.Enumeracao.DocumentoArrecadacao.TipoDocumentoArrecadacao Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.TipoDocumentoArrecadacao
        {
            get { return (Sync.Fiscal.Enumeracao.DocumentoArrecadacao.TipoDocumentoArrecadacao)CodTipo; }
        }

        string Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.UF
        {
            get { return Uf; }
        }

        string Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.CodigoAutorizacaoBancaria
        {
            get { return CodAutBanco; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.DataVencimento
        {
            get { return DataVenc; }
        }

        DateTime Sync.Fiscal.EFD.Entidade.IDocumentoArrecadacao.DataPagamento
        {
            get { return DataPagto; }
        }

        #endregion
    }
}