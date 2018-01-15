using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.DAL;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class Carregamento
    {
        #region Variaveis Locais

        private uint _idCarregamento;
        private uint _idOc;
        private uint _idPedido;
        private uint _idCliente;
        private string _nomeCliente;
        private int _ordem;
        private string _filtro;

        private List<uint> _idsOcs;
        private List<uint> _idsPedidos;

        private List<Entidade.ItemCarregamento> _itensCarregamento;
        private List<Entidade.VolumeCarregamento> _volumesCarregamento;

        private List<Entidade.ItemCarregamento> _itensCarregamentoFiltro;
        private List<Entidade.VolumeCarregamento> _volumesCarregamentoFiltro;

        #endregion

        #region Construtores

        public Carregamento()
            : this(0, 0, 0, 0, null, 0, null)
        {

        }

        internal Carregamento(uint idCarregamento, uint idOc, uint idPedido, uint idCliente, string nomeCliente, int ordem, string filtro)
        {
            _idCarregamento = idCarregamento;
            _idOc = idOc;
            _idPedido = idPedido;
            _idCliente = idCliente;
            _nomeCliente = nomeCliente;
            _ordem = ordem;
            _filtro = filtro;

            CarregaDados();
        }

        #endregion

        #region Métodos Publicos

        public void CarregaDados()
        {
            if (IdCarregamento == 0)
                return;

            var volumes = ItemCarregamentoDAO.Instance.GetListItemVolume(IdCarregamento);
            var itensVenda = ItemCarregamentoDAO.Instance.GetListItemPeca(IdCarregamento, false);
            var itensRevenda = ItemCarregamentoDAO.Instance.GetListItemPeca(IdCarregamento, true);

            foreach (var volume in volumes)
                VolumesCarregamento.Add(new VolumeCarregamento(volume));

            foreach (var itemVenda in itensVenda)
                ItensCarregamento.Add(new ItemCarregamento(itemVenda));

            foreach (var itemRevenda in itensRevenda)
                ItensCarregamento.Add(new ItemCarregamento(itemRevenda));


            ItensCarregamentoFiltro = ItensCarregamento;
            VolumesCarregamentoFiltro = VolumesCarregamento;

            //Filtra por cliente
            if (_idCliente > 0)
            {
                ItensCarregamentoFiltro = ItensCarregamentoFiltro.Where(ic => ic.IdCliente == _idCliente).ToList();
                VolumesCarregamentoFiltro = VolumesCarregamentoFiltro.Where(v => v.IdCliente == _idCliente).ToList();
            }
            else if(!string.IsNullOrEmpty(_nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, _nomeCliente, null, 0, null, null, null, null, 0);

                ItensCarregamentoFiltro = ItensCarregamentoFiltro.Where(ic => ids.Contains(ic.IdCliente.ToString())).ToList();
                VolumesCarregamentoFiltro = VolumesCarregamentoFiltro.Where(v => ids.Contains(v.IdCliente.ToString())).ToList();
            }

            //Filtra por pedido
            if (_idPedido > 0)
            {
                ItensCarregamentoFiltro = ItensCarregamentoFiltro.Where(ic => ic.IdPedido == _idPedido).ToList();
                VolumesCarregamentoFiltro = VolumesCarregamentoFiltro.Where(v => v.IdPedido == _idPedido).ToList();
            }

            //Filtra por OC
            if (_idOc > 0)
            {
                ItensCarregamentoFiltro = ItensCarregamentoFiltro.Where(ic => ic.IdOc == _idOc).ToList();
                VolumesCarregamentoFiltro = VolumesCarregamentoFiltro.Where(v => v.IdOc == _idOc).ToList();
            }

            //Ordena as peças
            if (_ordem == 1)
                ItensCarregamentoFiltro = ItensCarregamentoFiltro.OrderByDescending(ic => ic.M2).ToList();
            else
                ItensCarregamentoFiltro = ItensCarregamentoFiltro.OrderByDescending(ic => ic.IdPedido).ToList();

            //Filtra as peças carredas
            if (!string.IsNullOrEmpty(_filtro) && !_filtro.Contains("1,2"))
            {
                var carregado = ("," + (_filtro) + ",").Contains(",1,");

                ItensCarregamentoFiltro = ItensCarregamentoFiltro.Where(ic => !string.IsNullOrEmpty(ic.Etiqueta) == carregado).ToList();
                VolumesCarregamentoFiltro = VolumesCarregamentoFiltro.Where(v => !string.IsNullOrEmpty(v.Etiqueta) == carregado).ToList();
            }
        }

        #endregion

        #region Propiedades

        public uint IdCarregamento
        {
            get { return _idCarregamento; }
            set { _idCarregamento = value; }
        }

        public List<uint> IdsOcs
        {
            get { return _idsOcs; }
            set { _idsOcs = value; }
        }

        public List<uint> IdsPedidos
        {
            get { return _idsPedidos; }
            set { _idsPedidos = value; }
        }

        public double PecasCarregadas
        {
            get
            {
                return ItensCarregamento.Where(ic => !string.IsNullOrEmpty(ic.Etiqueta)).Count();
            }
        }

        public double VolumesCarregados
        {
            get
            {
                return VolumesCarregamento.Where(v => !string.IsNullOrEmpty(v.Etiqueta)).Count();
            }
        }

        public double PesoCarregado
        {
            get
            {
                return Math.Round(ItensCarregamento.Where(ic => !string.IsNullOrEmpty(ic.Etiqueta)).Sum(ic => ic.Peso) +
                    VolumesCarregamento.Where(v => !string.IsNullOrEmpty(v.Etiqueta)).Sum(v => v.Peso), 2);
            }
        }

        public double PecasPendentes
        {
            get
            {
                return ItensCarregamento.Where(ic => string.IsNullOrEmpty(ic.Etiqueta)).Count();
            }
        }

        public double VolumesPendentes
        {
            get
            {
                return VolumesCarregamento.Where(v => string.IsNullOrEmpty(v.Etiqueta)).Count();
            }
        }

        public double PesoPendente
        {
            get
            {
                return Math.Round(ItensCarregamento.Where(ic => string.IsNullOrEmpty(ic.Etiqueta)).Sum(ic => ic.Peso) +
                    VolumesCarregamento.Where(v => string.IsNullOrEmpty(v.Etiqueta)).Sum(v => v.Peso), 2);
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

        public List<Entidade.ItemCarregamento> ItensCarregamento
        {
            get 
            {
                if (_itensCarregamento == null)
                    _itensCarregamento = new List<ItemCarregamento>();

                return _itensCarregamento;
            }

            set 
            { 
                _itensCarregamento = value;
            }
        }

        public List<Entidade.VolumeCarregamento> VolumesCarregamento
        {
            get
            {
                if (_volumesCarregamento == null)
                    _volumesCarregamento = new List<Entidade.VolumeCarregamento>();

                return _volumesCarregamento;
            }

            set
            {
                _volumesCarregamento = value;
            }
        }

        public List<Entidade.ItemCarregamento> ItensCarregamentoFiltro
        {
            get
            {
                if (_itensCarregamentoFiltro == null)
                    _itensCarregamentoFiltro = new List<ItemCarregamento>();

                return _itensCarregamentoFiltro;
            }

            set
            {
                _itensCarregamentoFiltro = value;
            }
        }

        public List<Entidade.VolumeCarregamento> VolumesCarregamentoFiltro
        {
            get
            {
                if (_volumesCarregamentoFiltro == null)
                    _volumesCarregamentoFiltro = new List<Entidade.VolumeCarregamento>();

                return _volumesCarregamentoFiltro;
            }

            set
            {
                _volumesCarregamentoFiltro = value;
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
