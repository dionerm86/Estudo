using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public abstract class ProprietarioVeiculo
    {
        private Glass.Data.Model.Cte.ProprietarioVeiculo _proprietarioVeiculoCte;

        #region contrutores

        public ProprietarioVeiculo(Glass.Data.Model.Cte.ProprietarioVeiculo proprietarioVeiculoCte)
        {
            _proprietarioVeiculoCte = proprietarioVeiculoCte ?? new Glass.Data.Model.Cte.ProprietarioVeiculo();
        }

        #endregion

        #region Propriedades

        public uint IdPropVeic 
        {
            get { return _proprietarioVeiculoCte.IdPropVeic; }
            set { _proprietarioVeiculoCte.IdPropVeic = value; }
        }

        public string Nome 
        {
            get { return _proprietarioVeiculoCte.Nome; }
            set { _proprietarioVeiculoCte.Nome = value; }
        }

        public string Cpf 
        {
            get { return _proprietarioVeiculoCte.Cpf; }
            set { _proprietarioVeiculoCte.Cpf = value; }
        }

        public string Cnpj 
        {
            get { return _proprietarioVeiculoCte.Cnpj; }
            set { _proprietarioVeiculoCte.Cnpj = value; }
        }

        public string RNTRC 
        {
            get { return _proprietarioVeiculoCte.RNTRC; }
            set { _proprietarioVeiculoCte.RNTRC = value; }
        }

        public string IE 
        {
            get { return _proprietarioVeiculoCte.IE; }
            set { _proprietarioVeiculoCte.IE = value; }
        }

        public string UF 
        {
            get { return _proprietarioVeiculoCte.UF; }
            set { _proprietarioVeiculoCte.UF = value; }
        }

        public int TipoProp 
        {
            get { return _proprietarioVeiculoCte.TipoProp; }
            set { _proprietarioVeiculoCte.TipoProp = value; }
        }

        #endregion
    }
}
