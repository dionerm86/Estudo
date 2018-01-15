using System;
using Glass.Data.DAL;

namespace WebGlass.Business.NaturezaOperacao.Entidade
{
    public class NaturezaOperacao
    {
        internal Glass.Data.Model.NaturezaOperacao _naturezaOperacao;
        private bool _podeExcluir;
        private string _codigoInternoCfop;

        #region Construtores

        public NaturezaOperacao()
            : this(new Glass.Data.Model.NaturezaOperacao())
        {
            _podeExcluir = true;
        }

        internal NaturezaOperacao(Glass.Data.Model.NaturezaOperacao model)
        {
            _naturezaOperacao = model;
            _podeExcluir = !String.IsNullOrEmpty(model.CodInterno);
        }

        #endregion

        public uint Codigo
        {
            get { return (uint)_naturezaOperacao.IdNaturezaOperacao; }
            set { _naturezaOperacao.IdNaturezaOperacao = (int)value; }
        }

        public uint CodigoCfop
        {
            get { return (uint)_naturezaOperacao.IdCfop; }
            set
            {
                _naturezaOperacao.IdCfop = (int)value;
                _codigoInternoCfop = null;
            }
        }

        public string DescricaoCfop
        {
            get { return _naturezaOperacao.DescricaoCfop; }
            set { _naturezaOperacao.DescricaoCfop = value; }
        }

        public string CodigoInternoCfop
        {
            get
            {
                if (_codigoInternoCfop == null)
                    _codigoInternoCfop = CfopDAO.Instance.ObtemCodInterno(CodigoCfop);

                return _codigoInternoCfop;
            }
        }

        public string CodigoInterno
        {
            get { return _naturezaOperacao.CodInterno; }
            set { _naturezaOperacao.CodInterno = PodeExcluir ? value : null; }
        }

        public string Mensagem
        {
            get { return _naturezaOperacao.Mensagem; }
            set { _naturezaOperacao.Mensagem = value; }
        }

        public bool CalcularIcms
        {
            get { return _naturezaOperacao.CalcIcms; }
            set { _naturezaOperacao.CalcIcms = value; }
        }

        public bool CalcularIcmsSt
        {
            get { return _naturezaOperacao.CalcIcmsSt; }
            set { _naturezaOperacao.CalcIcmsSt = value; }
        }

        public bool CalcularIpi
        {
            get { return _naturezaOperacao.CalcIpi; }
            set { _naturezaOperacao.CalcIpi = value; }
        }

        public bool CalcularPis
        {
            get { return _naturezaOperacao.CalcPis; }
            set { _naturezaOperacao.CalcPis = value; }
        }

        public bool CalcularCofins
        {
            get { return _naturezaOperacao.CalcCofins; }
            set { _naturezaOperacao.CalcCofins = value; }
        }

        public bool IpiIntegraBaseCalculoIcms
        {
            get { return _naturezaOperacao.IpiIntegraBcIcms; }
            set { _naturezaOperacao.IpiIntegraBcIcms = value; }
        }

        public bool AlterarEstoqueFiscal
        {
            get { return _naturezaOperacao.AlterarEstoqueFiscal; }
            set { _naturezaOperacao.AlterarEstoqueFiscal = value; }
        }

        public string CstIcms
        {
            get { return _naturezaOperacao.CstIcms; }
            set { _naturezaOperacao.CstIcms = value; }
        }

        public float PercReducaoBcIcms
        {
            get { return _naturezaOperacao.PercReducaoBcIcms; }
            set { _naturezaOperacao.PercReducaoBcIcms = value; }
        }

        public int? CstIpi
        {
            get { return (int?)_naturezaOperacao.CstIpi; }
            set { _naturezaOperacao.CstIpi = (Glass.Data.Model.ProdutoCstIpi?)value; }
        }

        public int? CstPisCofins
        {
            get { return _naturezaOperacao.CstPisCofins; }
            set { _naturezaOperacao.CstPisCofins = value; }
        }

        public string Csosn
        {
            get { return _naturezaOperacao.Csosn; }
            set { _naturezaOperacao.Csosn = value; }
        }

        public string CodigoCompleto
        {
            get { return _naturezaOperacao.CodCompleto; }
        }

        public string CodigoControleUsar
        {
            get { return _naturezaOperacao.CodigoControleUsar; }
        }

        public bool PodeExcluir
        {
            get { return _podeExcluir; }
        }
    }
}