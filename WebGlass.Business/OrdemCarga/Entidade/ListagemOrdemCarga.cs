using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using System.Drawing;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class ListagemOrdemCarga
    {
        #region Variaveis Locais

        uint _idCliente;
        string _idsPedidos;
        uint _idRota;
        uint _idLoja;
        string _dtEntPedIni;
        string _dtEntPedFin;
        bool _pedidosObs;
        string _codRotasExternas;
        uint _idCliExterno;
        string _nomeCliExterno;
        bool _fastDelivery;
        string _obsLiberacao;

        private List<Glass.Data.Model.Pedido> _pedidos;
        private Glass.Data.Model.OrdemCarga.TipoOCEnum _tipoOC;

        #endregion

        #region Construtores

        public ListagemOrdemCarga()
            : this(new KeyValuePair<uint, KeyValuePair<uint, string>>(), 0, null, null, 0, false, null, 0, null, false, "")
        {
        }

        internal ListagemOrdemCarga(KeyValuePair<uint, KeyValuePair<uint, string>> pedidos, uint idLoja,
            string dtEntPedIni, string dtEntPedFin, Glass.Data.Model.OrdemCarga.TipoOCEnum tipoOC, bool pedidosObs, 
            string codRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao)
        {
            _idCliente = pedidos.Key;
            _idsPedidos = pedidos.Value.Value;
            _idRota = pedidos.Value.Key;
            _idLoja = idLoja;
            _dtEntPedIni = dtEntPedIni;
            _dtEntPedFin = dtEntPedFin;
            _tipoOC = tipoOC;
            _pedidosObs = pedidosObs;
            _codRotasExternas = codRotasExternas;
            _idCliExterno = idCliExterno;
            _nomeCliExterno = nomeCliExterno;
            _fastDelivery = fastDelivery;
            _obsLiberacao = obsLiberacao;

            CarregarDados();
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Carrega os dados de pedidos da listagem
        /// </summary>
        public void CarregarDados()
        {
            if (!string.IsNullOrEmpty(_idsPedidos))
                _pedidos = PedidoDAO.Instance.GetPedidosForOC(_idsPedidos, 0, true);
        }

        #endregion

        #region Propiedades

        public string IdsPedidos
        {
            get { return _idsPedidos; }
        }

        public List<Glass.Data.Model.Pedido> Pedidos
        {
            get
            {
                return _pedidos;
            }
        }

        public uint IdCliente
        {
            get { return _idCliente; }
        }

        public uint IdRota
        {
            get { return _idRota; }
        }

        public uint IdLoja
        {
            get { return _idLoja; }
        }

        public string DtEntPedIni
        {
            get { return _dtEntPedIni; }
        }

        public string DtEntPedFin
        {
            get { return _dtEntPedFin; }
        }

        public Glass.Data.Model.OrdemCarga.TipoOCEnum TipoOC
        {
            get { return _tipoOC; }
        }

        public string NomeCliente
        {
            get
            {
                if (Pedidos.Count == 0)
                    return "";

                return Pedidos[0].NomeCli;
            }
        }

        public string IdNomeCliente
        {
            get
            {
                return IdCliente + " - " + NomeCliente;
            }
        }

        public double Peso
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.PesoOC), 2);
            }
        }

        public double PesoPendenteProducao
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.PesoPendenteProducao), 2);
            }
        }

        public double TotalM2
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.TotMOC), 2);
            }
        }

        public double TotalM2PendenteProducao
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.TotMPendenteProducao), 2);
            }
        }

        public double QtdePecasVidro
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.QtdePecasVidro), 2);
            }
        }

        public double QtdePecaPendenteProducao
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.QtdePecaPendenteProducao), 2);
            }
        }

        public double QtdeVolumes
        {
            get
            {
                return Math.Round(Pedidos.Sum(p => p.QtdeVolume), 2);
            }
        }

        int? _numPedidosParaGerar = null;

        public int NumPedidosParaGerar
        {
            get
            {
                if (_numPedidosParaGerar == null)
                    _numPedidosParaGerar = PedidoDAO.Instance.GetCountPedidosForOC(TipoOC, IdCliente, IdRota, IdLoja,
                        DtEntPedIni, DtEntPedFin, false, _pedidosObs, CodRotasExternas, IdCliExterno, NomeCliExterno, _fastDelivery, _obsLiberacao);
                
                return _numPedidosParaGerar.GetValueOrDefault(0);
            }
        }

        public bool GerarOCVisible
        {
            get
            {
                return NumPedidosParaGerar > 0;
            }
        }

        public Color CorLinha
        {
            get
            {
                if (NumPedidosParaGerar == Pedidos.Count)
                    return Color.Red;
                else if (NumPedidosParaGerar > 0)
                    return Color.Blue;
                else
                    return Color.Black;
            }
        }

        public bool GerouTodosVolumes
        {
            get
            {
                return Pedidos.Where(p => !p.GerouTodosVolumes).Count() == 0;
            }
        }

        public string VolumesPendentes
        {
            get { return GerouTodosVolumes ? "" : "Sim"; }
        }

        private string _cidadeCliente;

        public string CidadeCliente
        {
            get 
            { 
                if(string.IsNullOrEmpty(_cidadeCliente))
                    _cidadeCliente = ClienteDAO.Instance.ObtemCidadeUf(IdCliente);

                return _cidadeCliente;
            }
        }

        private string _rotaCliente;

        public string RotaCliente
        {
            get
            {
                if (string.IsNullOrEmpty(_rotaCliente))
                {
                    var rota = RotaDAO.Instance.GetByCliente(null, IdCliente);
                    if (rota != null)
                        _rotaCliente = rota.CodInterno + " - " + rota.Descricao;
                }

                return _rotaCliente;
            }
        }

        public string CodRotasExternas
        {
            get { return _codRotasExternas; }
        }

        public uint IdCliExterno
        {
            get { return _idCliExterno; }
        }

        public string NomeCliExterno
        {
            get { return _nomeCliExterno; }
        }

        #endregion
    }
}
