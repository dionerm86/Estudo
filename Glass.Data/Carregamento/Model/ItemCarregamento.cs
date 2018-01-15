using System;
using GDA;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Data.Helper;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ItemCarregamentoDAO))]
    [PersistenceClass("item_carregamento")]
    public class ItemCarregamento : ModelBaseCadastro
    {
        #region Enumerações

        public enum TipoItemCarregamento : long
        {
            Volume = 1,
            Venda,
            Revenda
        }

        #endregion

        #region Propiedades

        [PersistenceProperty("IDITEMCARREGAMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdItemCarregamento { get; set; }

        [PersistenceProperty("IDCARREGAMENTO")]
        public uint IdCarregamento { get; set; }

        [PersistenceProperty("IdOrdemCarga")]
        public int IdOrdemCarga { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IdProdPed")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("IDVOLUME")]
        public uint? IdVolume { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint? IdProdPedProducao { get; set; }

        [PersistenceProperty("IdProdImpressaoChapa")]
        public uint? IdProdImpressaoChapa { get; set; }

        [PersistenceProperty("CARREGADO")]
        public bool Carregado { get; set; }

        [PersistenceProperty("ENTREGUE")]
        public bool Entregue { get; set; }

        [PersistenceProperty("IDFUNCLEITURA")]
        public uint IdFuncLeitura { get; set; }

        [PersistenceProperty("DATALEITURA")]
        public DateTime DataLeitura { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("TipoItem", DirectionParameter.InputOptional)]
        public long? TipoItem { get; set; }

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("numEtiqueta", DirectionParameter.InputOptional)]
        public string Etiqueta { get; set; }

        [PersistenceProperty("PedidoEtiqueta", DirectionParameter.InputOptional)]
        public string PedidoEtiqueta { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.InputOptional)]
        public double Peso { get; set; }

        [PersistenceProperty("Qtde", DirectionParameter.InputOptional)]
        public double Qtde { get; set; }

        [PersistenceProperty("IdProdPedEsp", DirectionParameter.InputOptional)]
        public uint IdProdPedEsp { get; set; }

        [PersistenceProperty("IDOC", DirectionParameter.InputOptional)]
        public uint IdOc { get; set; }

        [PersistenceProperty("PEDCLI", DirectionParameter.InputOptional)]
        public string PedCli { get; set; }

        [PersistenceProperty("Rota", DirectionParameter.InputOptional)]
        public string Rota { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("IdRota", DirectionParameter.InputOptional)]
        public UInt32 IdRota { get; set; }

        [PersistenceProperty("IdPedidoExterno", DirectionParameter.InputOptional)]
        public uint IdPedidoExterno { get; set; }

        [PersistenceProperty("IdClienteExterno", DirectionParameter.InputOptional)]
        public uint IdClienteExterno { get; set; }

        [PersistenceProperty("ClienteExterno", DirectionParameter.InputOptional)]
        public string ClienteExterno { get; set; }

        [PersistenceProperty("PedidoImportado", DirectionParameter.InputOptional)]
        public bool PedidoImportado { get; set; }

        [PersistenceProperty("ClienteImportacao", DirectionParameter.InputOptional)]
        public bool ClienteImportacao { get; set; }

        #region Peça

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodProduto { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescProduto { get; set; }

        [PersistenceProperty("Altura", DirectionParameter.InputOptional)]
        public double Altura { get; set; }

        [PersistenceProperty("Largura", DirectionParameter.InputOptional)]
        public double Largura { get; set; }

        [PersistenceProperty("M2", DirectionParameter.InputOptional)]
        public double M2 { get; set; }        

        #endregion

        #region Volume

        [PersistenceProperty("DataFechamento", DirectionParameter.InputOptional)]
        public DateTime? DataFechamento { get; set; }

        #endregion

        #endregion

        #region Propiedades de Suporte

        public Color CorLinha
        {
            get
            {
                if (Carregado)
                    return Color.Green;
                else
                    return Color.Red;
            }
        }

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string CodInternoDescProd 
        { 
            get 
            { 
                return CodProduto + " - " + DescProduto;
            }
        }

        public bool LogEstornoVisible
        {
            get
            {
                if (IdItemCarregamento == 0)
                    return false;

                return EstornoItemCarregamentoDAO.Instance.TemEstorno(IdItemCarregamento);
            }
        }

        private string _imagemPecaUrl = null;

        public string ImagemPecaUrl
        {
            get
            {
                if (IdProdPedEsp == 0)
                    return "";

                if (_imagemPecaUrl == null)
                {
                    ProdutosPedidoEspelho ppe = ProdutosPedidoEspelhoDAO.Instance.GetForImagemPeca(IdProdPedEsp);
                    uint? idPecaItemProj = PecaItemProjetoDAO.Instance.ObtemIdPecaItemProjByIdProdPed(IdProdPedEsp);

                    ppe.Item = idPecaItemProj > 0 ? UtilsProjeto.GetItemPecaFromEtiqueta(PecaItemProjetoDAO.Instance.ObtemItem(idPecaItemProj.Value), PedidoEtiqueta) : 0;
                    _imagemPecaUrl = ppe.ImagemUrl;
                }

                return _imagemPecaUrl;
            }
        }

        public string SetoresPendentes
        {
            get { return SetorDAO.Instance.ObtemDescricaoSetoresRestantes(PedidoEtiqueta, IdProdPedEsp); }
        }

        public string NomeFuncLeitura
        {
            get
            {
                if (!Carregado)
                    return "";

                return FuncionarioDAO.Instance.GetNome(IdFuncLeitura);
            }
        }

        public string EtiquetaVolume
        {
            get
            {
                if (Carregado)
                    return "V" + IdVolume.GetValueOrDefault().ToString("D9");
                else
                    return string.Empty;
            }
        }

        public string PedidoEtiquetaVolume
        {
            get { return IdPedido + " (" + "V" + IdVolume.GetValueOrDefault().ToString("D9") + ")"; }
        }

        public string DataLeituraStr
        {
            get
            {
                if (!Carregado)
                    return "";

                return DataLeitura.ToString();
            }
        }

        public int? IdPedidoRevenda
        {
            get
            {
                return PedidoDAO.Instance.ObterIdPedidoRevenda(null, (int)IdPedido);
            }
        }

        #endregion
    }
}
