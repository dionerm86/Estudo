using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class InfoCarregamento
    {
        #region Variaveis Locais

        private List<Glass.Data.Model.ItemCarregamento> _itensCarregamento;
        private List<Glass.Data.Model.ItemCarregamento> _volumesCarregamento;

        #endregion

        #region Construtores

        public InfoCarregamento()
            : this(null, null)
        {

        }

        internal InfoCarregamento(List<Glass.Data.Model.ItemCarregamento> itensCarregamento,
            List<Glass.Data.Model.ItemCarregamento> volumesCarregamento)
        {
            _itensCarregamento = itensCarregamento;
            _volumesCarregamento = volumesCarregamento;
        }

        #endregion

        #region Propiedades

        public List<Glass.Data.Model.ItemCarregamento> ItensCarregamento
        {
            get
            {
                if (_itensCarregamento == null)
                    _itensCarregamento = new List<Glass.Data.Model.ItemCarregamento>();

                return _itensCarregamento;
            }

            set
            {
                _itensCarregamento = value;
            }
        }

        public List<Glass.Data.Model.ItemCarregamento> VolumesCarregamento
        {
            get
            {
                if (_volumesCarregamento == null)
                    _volumesCarregamento = new List<Glass.Data.Model.ItemCarregamento>();

                return _volumesCarregamento;
            }

            set
            {
                _volumesCarregamento = value;
            }
        }

        public double PecasCarregadas
        {
            get
            {
                return ItensCarregamento.Where(ic => ic.Carregado).Count();
            }
        }

        public double VolumesCarregados
        {
            get
            {
                return VolumesCarregamento.Where(v => v.Carregado).Count();
            }
        }

        public double PesoCarregado
        {
            get
            {
                return Math.Round(ItensCarregamento.Where(ic => ic.Carregado).Sum(ic => ic.Peso) +
                    VolumesCarregamento.Where(v => v.Carregado).Sum(v => v.Peso), 2);
            }
        }

        public double PecasPendentes
        {
            get
            {
                return ItensCarregamento.Where(ic => !ic.Carregado).Count();
            }
        }

        public double VolumesPendentes
        {
            get
            {
                return VolumesCarregamento.Where(v => !v.Carregado).Count();
            }
        }

        public double PesoPendente
        {
            get
            {
                return Math.Round(ItensCarregamento.Where(ic => !ic.Carregado).Sum(ic => ic.Peso) +
                    VolumesCarregamento.Where(v => !v.Carregado).Sum(v => v.Peso), 2);
            }
        }

        public int TotalPecas
        {
            get
            {
                return ItensCarregamento.Count;
            }
        }

        public int TotalVolumes
        {
            get
            {
                return VolumesCarregamento.Count;
            }
        }

        public double PesoTotal
        {
            get
            {
                return Math.Round(ItensCarregamento.Sum(ic => ic.Peso) + VolumesCarregamento.Sum(v => v.Peso), 2);
            }
        }

        public bool EstornarVisivel
        {
            get
            {
                return ItensCarregamento.Where(ic => ic.Carregado).Count() > 0 ||
                VolumesCarregamento.Where(ic => ic.Carregado).Count() > 0;
            }
        }

        #endregion
    }
}
