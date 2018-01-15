using System;
using Glass.Data.DAL.CTe;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class SeguroCte
    {
        private Glass.Data.Model.Cte.SeguroCte _seguroCte;

        public Glass.Data.Model.Cte.SeguroCte SeguroCteModel
        {
            get { return _seguroCte; }
            set { _seguroCte = value; }
        }

        #region Construtores

        public SeguroCte(Glass.Data.Model.Cte.SeguroCte seguroCte)
        {
            _seguroCte = seguroCte ?? new Glass.Data.Model.Cte.SeguroCte();
        }

        #endregion

        #region Propriedades

        public uint IdCte 
        {
            get { return _seguroCte.IdCte; }
            set { _seguroCte.IdCte = value; }
        }

        public uint IdSeguradora
        {
            get { return _seguroCte.IdSeguradora; }
            set { _seguroCte.IdSeguradora = value; }
        } 

        public int ResponsavelSeguro
        {
            get { return _seguroCte.ResponsavelSeguro; }
            set { _seguroCte.ResponsavelSeguro = value; }
        }

        public string NumeroApolice
        {
            get { return _seguroCte.NumeroApolice; }
            set { _seguroCte.NumeroApolice = value; }
        }

        public string NumeroAverbacao
        { 
            get { return _seguroCte.NumeroAverbacao; }
            set { _seguroCte.NumeroAverbacao = value; }
        }

        public decimal ValorCargaAverbacao
        {
            get { return _seguroCte.ValorCargaAverbacao; }
            set { _seguroCte.ValorCargaAverbacao = value; }
        }

        public Seguradora ObjSeguradora { get; set; }

        public string NomeSeguradora
        {
            get { return SeguradoraDAO.Instance.ObtemNomeSeguradora(IdSeguradora); }
        }

        public string DescricaoResponsavelSeguro
        {
            get
            {
                switch (ResponsavelSeguro)
                {
                    case 0: return "Remetente";
                    case 1: return "Expedidor";
                    case 2: return "Recebedor";
                    case 3: return "Destinatário";
                    case 4: return "Emitente do CT-e";
                    default: return "Tomador do Serviço";
                }
            }
        }

        #endregion
    }
}
