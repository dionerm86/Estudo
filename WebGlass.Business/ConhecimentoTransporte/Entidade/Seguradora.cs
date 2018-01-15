using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class Seguradora
    {
        private Glass.Data.Model.Cte.Seguradora _seguradora;

        #region contrutores

        public Seguradora(Glass.Data.Model.Cte.Seguradora seguradora)
        {
            _seguradora = seguradora ?? new Glass.Data.Model.Cte.Seguradora();
        }

        #endregion

        #region propriedades

        public int IdSeguradora 
        {
            get { return _seguradora.IdSeguradora; }
            set { _seguradora.IdSeguradora = value; }
        }

        public string NomeSeguradora 
        {
            get { return _seguradora.NomeSeguradora; }
            set { _seguradora.NomeSeguradora = value; }
        }

        public string CNPJ
        {
            get { return _seguradora.CNPJ; }
            set { _seguradora.CNPJ = value; }
        }

        #endregion
    }
}
