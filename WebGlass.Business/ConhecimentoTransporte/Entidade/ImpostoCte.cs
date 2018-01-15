using System;
using Glass.Data.EFD;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class ImpostoCte
    {
        private Glass.Data.Model.Cte.ImpostoCte _impostoCte;

        #region construtores

        public ImpostoCte()
            : this(new Glass.Data.Model.Cte.ImpostoCte())
        {
        }

        internal ImpostoCte(Glass.Data.Model.Cte.ImpostoCte impostoCte)
        {
            _impostoCte = impostoCte;
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _impostoCte.IdCte; }
            set { _impostoCte.IdCte = value; }
        }

        public int TipoImposto
        {
            get { return _impostoCte.TipoImposto; }
            set { _impostoCte.TipoImposto = value; }
        }

        public string Cst 
        {
            get { return _impostoCte.Cst; }
            set { _impostoCte.Cst = value; }
        }

        public decimal BaseCalc 
        {
            get { return _impostoCte.BaseCalc; }
            set { _impostoCte.BaseCalc = value; }
        }

        public float PercRedBaseCalc 
        {
            get { return _impostoCte.PercRedBaseCalc; }
            set { _impostoCte.PercRedBaseCalc = value; }
        }

        public float Aliquota 
        {
            get { return _impostoCte.Aliquota; }
            set { _impostoCte.Aliquota = value; }
        }

        public decimal Valor 
        {
            get { return _impostoCte.Valor; }
            set { _impostoCte.Valor = value; }
        }

        public decimal BaseCalcStRetido 
        {
            get { return _impostoCte.BaseCalcStRetido; }
            set { _impostoCte.BaseCalcStRetido = value; }
        }

        public float AliquotaStRetido 
        {
            get { return _impostoCte.AliquotaStRetido; }
            set { _impostoCte.AliquotaStRetido = value; }
        }

        public decimal ValorStRetido 
        {
            get { return _impostoCte.ValorStRetido; }
            set { _impostoCte.ValorStRetido = value; }
        }

        public decimal ValorCred 
        {
            get { return _impostoCte.ValorCred; }
            set { _impostoCte.ValorCred = value; }
        }

        #endregion

        public string DescricaoTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }
    }
}
