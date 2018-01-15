using System;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class InfoCargaCte
    {
        private Glass.Data.Model.Cte.InfoCargaCte _infoCargaCte;

        #region construtores

        public InfoCargaCte(Glass.Data.Model.Cte.InfoCargaCte infoCargaCte)
        {
            _infoCargaCte = infoCargaCte ?? new Glass.Data.Model.Cte.InfoCargaCte();
        }

        #endregion

        #region Propriedades

        public uint IdInfoCarga
        {
            get { return _infoCargaCte.IdInfoCarga; }
            set { _infoCargaCte.IdInfoCarga = value; }
        }

        public uint IdCte 
        {
            get { return _infoCargaCte.IdCte; }
            set { _infoCargaCte.IdCte = value; }
        }

        public int TipoUnidade 
        {
            get { return _infoCargaCte.TipoUnidade; }
            set { _infoCargaCte.TipoUnidade = value; }
        }

        public string TipoMedida 
        {
            get { return _infoCargaCte.TipoMedida; }
            set { _infoCargaCte.TipoMedida = value; }
        }

        public float Quantidade 
        {
            get { return _infoCargaCte.Quantidade; }
            set { _infoCargaCte.Quantidade = value; }
        }

        public string DescricaoTipoUnidade
        {
            get
            {
                switch (TipoUnidade)
                {
                    case -1: return "Selecione";
                    case 0: return "M3";
                    case 1: return "KG";
                    case 2: return "TON";
                    case 3: return "UNIDADE";
                    case 4: return "LITROS";
                    default: return "MMBTU";
                }
            }
        }

        #endregion
    }
}
