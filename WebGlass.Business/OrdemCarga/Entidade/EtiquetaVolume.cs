namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class EtiquetaVolume
    {
        #region Variaveis Locais

        internal Glass.Data.Model.Volume _volume;

        #endregion

        #region Construtores

        public EtiquetaVolume()
            : this(new Glass.Data.Model.Volume())
        {
        }

        internal EtiquetaVolume(Glass.Data.Model.Volume model)
        {
            _volume = model;
        }

        #endregion

        #region Propiedades

        public uint IdPedido
        {
            get { return _volume.IdPedido; }
        }

        public string CodCliente
        {
            get { return _volume.CodCliente; }
        }

        public uint IdVolume
        {
            get { return _volume.IdVolume; }
        }

        public string Etiqueta
        {
            get { return _volume.Etiqueta; }
        }

        public string IdNomeCliente
        {
            get { return _volume.IdNomeCliente; }
        }

        public string DataEntregaStr
        {
            get { return _volume.DataEntregaPedido.ToString(); }
        }

        public string Rota
        {
            get { return _volume.CodRota; }
        }

        public double QtdeItens
        {
            get { return _volume.QtdeItens; }
        }

        public string CodigoProdutos
        {
            get
            {
                return Glass.Data.DAL.VolumeProdutosPedidoDAO.Instance.ObterCodigoProdutosVolume(null, (int)_volume.IdVolume);
            }
        }

        public byte[] BarCodeImage
        {
            get { return Glass.Data.Helper.Utils.GetBarCode(Etiqueta); }
        }

        public bool PedidoImportado
        {
            get
            {
                return _volume.PedidoImportado;
            }
        }

        public uint IdPedidoExterno
        {
            get
            {
                return _volume.IdPedidoExterno;
            }
        }

        public uint IdClienteExterno
        {
            get
            {
                return _volume.IdClienteExterno;
            }
        }

        public string ClienteExterno
        {
            get
            {
                return _volume.ClienteExterno;
            }
        }

        public string RotaExterna
        {
            get
            {
                return _volume.RotaExterna;
            }
        }

        #endregion
    }
}
