using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class NfeCte
    {
        private Glass.Data.Model.Cte.NotaFiscalCte _nfeCte;

        #region construtores

        public NfeCte()
        {
            _nfeCte = new Glass.Data.Model.Cte.NotaFiscalCte();
        }

        public NfeCte(Glass.Data.Model.Cte.NotaFiscalCte nfeCte)
        {
            _nfeCte = nfeCte ?? new Glass.Data.Model.Cte.NotaFiscalCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _nfeCte.IdCte; }
            set { _nfeCte.IdCte = value; }
        }

        public uint IdNf 
        {
            get { return _nfeCte.IdNf; }
            set { _nfeCte.IdNf = value; }
        }

        public Glass.Data.Model.NotaFiscal ObjNotaFiscal { get; set; }

        public uint NumeroNFe
        {
            get { return ObjNotaFiscal.NumeroNFe; }
        }

        public string Modelo
        {
            get { return ObjNotaFiscal.Modelo; }
        }

        public string CodCfop
        {
            get { return ObjNotaFiscal.CodCfop; }
        }

        public string TipoDocumentoString
        {
            get { return ObjNotaFiscal.TipoDocumentoString; }
        }

        public string NomeEmitente
        {
            get { return ObjNotaFiscal.NomeEmitente; }
        }

        public DateTime DataEmissao
        {
            get { return ObjNotaFiscal.DataEmissao; }
        }

        #endregion
    }
}
