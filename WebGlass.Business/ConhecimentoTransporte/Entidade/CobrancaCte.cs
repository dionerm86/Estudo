using System;
using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class CobrancaCte
    {
        private Glass.Data.Model.Cte.CobrancaCte _cobrancaCte;
        private string _descricaoPlanoContas;

        #region contrutores

        public CobrancaCte()
        {
            _cobrancaCte = new Glass.Data.Model.Cte.CobrancaCte();
        }

        internal CobrancaCte(Glass.Data.Model.Cte.CobrancaCte cobrancaCte)
        {
            _cobrancaCte = cobrancaCte ?? new Glass.Data.Model.Cte.CobrancaCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _cobrancaCte.IdCte; }
            set { _cobrancaCte.IdCte = value; }
        }

        public string NumeroFatura 
        {
            get { return _cobrancaCte.NumeroFatura; }
            set { _cobrancaCte.NumeroFatura = value; }
        }

        public decimal ValorOrigFatura 
        {
            get { return _cobrancaCte.ValorOrigFatura; }
            set { _cobrancaCte.ValorOrigFatura = value; }
        }

        public decimal DescontoFatura 
        {
            get { return _cobrancaCte.DescontoFatura; }
            set { _cobrancaCte.DescontoFatura = value; }
        }

        public decimal ValorLiquidoFatura 
        {
            get { return _cobrancaCte.ValorLiquidoFatura; }
            set { _cobrancaCte.ValorLiquidoFatura = value; }
        }

        public bool GerarContasPagar
        {
            get { return _cobrancaCte.GerarContasPagar; }
            set { _cobrancaCte.GerarContasPagar = value; }
        }

        public uint? IdConta
        {
            get { return _cobrancaCte.IdConta; }
            set
            {
                _cobrancaCte.IdConta = value;
                _descricaoPlanoContas = null;
            }
        }

        public string DescricaoPlanoContas
        {
            get
            {
                if (_descricaoPlanoContas == null && IdConta > 0)
                    _descricaoPlanoContas = Glass.Data.DAL.PlanoContasDAO.Instance.GetDescricao(IdConta.Value, true);

                return _descricaoPlanoContas;
            }
        }

        public List<CobrancaDuplCte> ObjCobrancaDuplCte { get; set; }

        #endregion
    }
}
