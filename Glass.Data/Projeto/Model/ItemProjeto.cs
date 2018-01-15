using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ItemProjetoDAO))]
	[PersistenceClass("item_projeto")]
	public class ItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDITEMPROJETO", PersistenceParameterType.IdentityKey)]
        public uint IdItemProjeto { get; set; }

        [PersistenceProperty("IDPROJETO")]
        public uint? IdProjeto { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint? IdOrcamento { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDPEDIDOESPELHO")]
        public uint? IdPedidoEspelho { get; set; }

        [PersistenceProperty("IDPROJETOMODELO")]
        public uint IdProjetoModelo { get; set; }

        [PersistenceProperty("IDCORVIDRO")]
        public uint IdCorVidro { get; set; }

        [PersistenceProperty("IDCORALUMINIO")]
        public uint IdCorAluminio { get; set; }

        [PersistenceProperty("IDCORFERRAGEM")]
        public uint IdCorFerragem { get; set; }

        [PersistenceProperty("ESPESSURAVIDRO")]
        public int EspessuraVidro { get; set; }

        [PersistenceProperty("MEDIDAEXATA")]
        public bool MedidaExata { get; set; }

        [PersistenceProperty("APENASVIDROS")]
        public bool ApenasVidros { get; set; }

        [PersistenceProperty("CONFERIDO", DirectionParameter.Input)]
        public bool Conferido { get; set; }

        [PersistenceProperty("AMBIENTE")]
        public string Ambiente { get; set; }

        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [PersistenceProperty("M2VAO")]
        public Single M2Vao { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("CUSTOTOTAL")]
        public decimal CustoTotal { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODIGOMODELO", DirectionParameter.InputOptional)]
        public string CodigoModelo { get; set; }

        [PersistenceProperty("DESCRMODELO", DirectionParameter.InputOptional)]
        public string DescrModelo { get; set; }

        private string _textoOrcamento;

        [PersistenceProperty("TEXTOORCAMENTO", DirectionParameter.InputOptional)]
        public string TextoOrcamento
        {
            get { return _textoOrcamento != null ? _textoOrcamento : String.Empty; }
            set { _textoOrcamento = value; }
        }

        [PersistenceProperty("DESCRCORVIDRO", DirectionParameter.InputOptional)]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("DESCRCORALUMINIO", DirectionParameter.InputOptional)]
        public string DescrCorAluminio { get; set; }

        [PersistenceProperty("DESCRCORFERRAGEM", DirectionParameter.InputOptional)]
        public string DescrCorFerragem { get; set; }

        [XmlIgnore]
        public int LarguraVao
        {
            get { return GDAOperations.GetDAO<MedidaItemProjeto, MedidaItemProjetoDAO>().GetByItemProjeto(IdItemProjeto, 2, false); }
        }

        [XmlIgnore]
        public int AlturaVao
        {
            get { return GDAOperations.GetDAO<MedidaItemProjeto, MedidaItemProjetoDAO>().GetByItemProjeto(IdItemProjeto, 3, false); }
        }

        [XmlIgnore]
        [PersistenceProperty("EDITDELETEVISIBLE", DirectionParameter.InputOptional)]
        public bool EditDeleteVisible { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDAMBIENTEPEDIDOESPELHO", DirectionParameter.InputOptional)]
        public uint? IdAmbientePedidoEspelho { get; set; }
        
        [XmlIgnore]
        [PersistenceProperty("DATAENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaPedido { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DATAFABRICAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataFabricaPedido { get; set; }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Controla se os materiais do itemProjeto serão mostrados na impressão
        /// </summary>
        [XmlIgnore]
        public bool MostrarMateriais { get; set; }

        [XmlIgnore]
        public byte[] ImagemProjeto { get; set; }

        [XmlIgnore]
        public byte[] ImagemProjetoModelo { get; set; }

        [XmlIgnore]
        public string ImagemUrl
        {
            get 
            {
                Page atual = HttpContext.Current.CurrentHandler as Page;
                string[] url = UtilsProjeto.GetFiguraAssociadaUrl(IdItemProjeto, IdProjetoModelo).Split('?');

                return (atual != null ? atual.ResolveClientUrl(url[0]) : url[0]) + "?" + url[1];
            }
        }

        [XmlIgnore]
        public bool EditVisible
        {
            get
            {
                if (IdPedidoEspelho == null || IdPedidoEspelho == 0)
                    return true;

                return ItemProjetoDAO.Instance.PodeSerEditado(IdItemProjeto);
            }
        }

        [XmlIgnore]
        public string ImagemUrlMini
        {
            get
            {
                string nomeFigura = ProjetoModeloDAO.Instance.GetNomeFiguraByItemProjeto(IdItemProjeto);

                Page atual = HttpContext.Current.CurrentHandler as Page;
                string url = "~/Handlers/LoadImage.ashx";
                return (atual != null ? atual.ResolveClientUrl(url) : url) + "?path=" + Utils.GetModelosProjetoPath + nomeFigura + "&perc=0.8"; 
            }
        }

        private uint? _idCliente;

        [XmlIgnore]
        public uint? IdCliente
        {
            get 
            {
                if (_idCliente == null)
                {
                    bool temp;
                    ItemProjetoDAO.Instance.GetTipoEntregaCliente(IdItemProjeto, out _tipoEntrega, out _idCliente, out temp);
                    _reposicao = temp;
                }

                return _idCliente;
            }
        }

        private string _nomeCliente;

        [XmlIgnore]
        public string NomeCliente
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeCliente) && _idCliente > 0)
                    _nomeCliente = GDAOperations.GetDAO<Cliente, ClienteDAO>().GetNome(_idCliente.Value);

                return _nomeCliente;
            }
        }

        private int? _tipoEntrega;

        [XmlIgnore]
        public int? TipoEntrega
        {
            get 
            {
                if (_tipoEntrega == null)
                {
                    bool temp;
                    ItemProjetoDAO.Instance.GetTipoEntregaCliente(IdItemProjeto, out _tipoEntrega, out _idCliente, out temp);
                    _reposicao = temp;
                }
                
                return _tipoEntrega; 
            }
        }

        private bool? _reposicao;

        [XmlIgnore]
        public bool Reposicao
        {
            get
            {
                if (_reposicao == null)
                {
                    bool temp;
                    ItemProjetoDAO.Instance.GetTipoEntregaCliente(IdItemProjeto, out _tipoEntrega, out _idCliente, out temp);
                    _reposicao = temp;
                }

                return _reposicao.GetValueOrDefault();
            }
        }

        private string _descrTipoEntrega;

        [XmlIgnore]
        public string DescrTipoEntrega
        {
            get
            {
                if (String.IsNullOrEmpty(_descrTipoEntrega) && _tipoEntrega > 0)
                {
                    foreach (GenericModel g in DataSources.Instance.GetTipoEntregaForPojeto())
                        if (g.Id == _tipoEntrega)
                        {
                            _descrTipoEntrega = g.Descr;
                            break;
                        }
                }

                return _descrTipoEntrega;
            }
        }

        private uint? _idFunc;

        [XmlIgnore]
        public uint? IdFunc
        {
            get
            {
                if (_idFunc == null)
                {
                    if (IdProjeto > 0)
                        _idFunc =  ProjetoDAO.Instance.ObtemValorCampo<uint>("idFunc", "idProjeto=" + IdProjeto.Value);
                    else if (IdPedido > 0)
                        _idFunc = PedidoDAO.Instance.ObtemIdFunc(IdPedido.Value);
                    else if (IdPedidoEspelho > 0)
                        _idFunc = PedidoEspelhoDAO.Instance.ObtemValorCampo<uint>("idFuncConf", "idPedido=" + IdPedidoEspelho.Value);
                    else if (IdOrcamento > 0)
                        _idFunc = OrcamentoDAO.Instance.ObtemValorCampo<uint>("idFunc", "idOrcamento=" + IdOrcamento.Value);
                }

                return _idFunc;
            }
        }

        private string _nomeFunc;

        [XmlIgnore]
        public string NomeFunc
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeFunc) && _idFunc > 0)
                    _nomeFunc = FuncionarioDAO.Instance.GetNome(_idFunc.Value);

                return _nomeFunc;
            }
        }

        [XmlIgnore]
        public string DatasPedido
        {
            get
            {
                string retorno = String.Empty;

                if (DataFabricaPedido == null && DataEntregaPedido != null)
                {
                    DataFabricaPedido = DataEntregaPedido.Value;
                    int diasUteis = PCPConfig.Etiqueta.DiasDataFabrica;

                    for (int i = 0; i < diasUteis; i++)
                    {
                        DataFabricaPedido = DataFabricaPedido.Value.AddDays(-1);

                        while (!DataFabricaPedido.Value.DiaUtil())
                            DataFabricaPedido = DataFabricaPedido.Value.AddDays(-1);
                    }
                }

                if (DataFabricaPedido != null)
                    retorno += "Data de fábrica: " + DataFabricaPedido.Value.ToString("dd/MM/yyyy") + "    ";

                if (DataEntregaPedido != null)
                    retorno += "Data de entrega: " + DataEntregaPedido.Value.ToString("dd/MM/yyyy") + "    ";

                return retorno.Trim();
            }
        }

        #endregion
    }
}