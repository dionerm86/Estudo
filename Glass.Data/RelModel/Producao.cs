using System;
using System.Linq;
using System.Xml.Serialization;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoDAO))]
    [PersistenceClass("producao")]
    public class Producao
    {
        #region Propriedades

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPEDIDOANTERIOR")]
        public uint? IdPedidoAnterior { get; set; }

        [PersistenceProperty("IDPEDIDOEXPEDICAO")]
        public uint? IdPedidoExpedicao { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("ISPEDIDOMAODEOBRA")]
        public bool IsPedidoMaoDeObra { get; set; }

        [PersistenceProperty("ISPEDIDOPRODUCAO")]
        public bool IsPedidoProducao { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("IDSETOR")]
        public uint? IdSetor { get; set; }

        [PersistenceProperty("NUMSEQSETOR")]
        public long NumSeqSetor { get; set; }

        [PersistenceProperty("DATASETOR")]
        public string DataSetor { get; set; }

        [PersistenceProperty("NUMETIQUETA")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("NOMECLIENTE")]
        public string NomeCliente { get; set; }

        [PersistenceProperty("CODINTERNOPRODUTO")]
        public string CodInternoProduto { get; set; }

        [PersistenceProperty("DESCRPRODUTO")]
        public string DescrProduto { get; set; }

        [PersistenceProperty("NOMESETOR")]
        public string NomeSetor { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        [PersistenceProperty("CORSETOR")]
        public int CorSetor { get; set; }

        [PersistenceProperty("SITUACAOPRODUCAO")]
        public int SituacaoProducao { get; set; }

        [PersistenceProperty("TIPOPERDAPRODUCAO")]
        public int? TipoPerdaProducao { get; set; }

        [PersistenceProperty("OBSPERDAPRODUCAO")]
        public string ObsPerdaProducao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string TipoPedido
        {
            get { return IsPedidoMaoDeObra ? "MO" : IsPedidoProducao ? "P" : "V"; }
        }

        public string Cliente
        {
            get { return BibliotecaTexto.GetTwoFirstNames(NomeCliente); }
        }

        public string Produto
        {
            get { return DescrProduto + " " + DescrBeneficiamentos + " " + Largura + "x" + Altura; }
        }

        public string ProdutoPerda
        {
            get { return DescrProduto + " " + DescrBeneficiamentos; }
        }

        public string DescrBeneficiamentos
        {
            get
            {
                if (IsPedidoMaoDeObra)
                {
                    uint idAmbientePedido = AmbientePedidoEspelhoDAO.Instance.GetByIdProdPed(IdProdPed).IdAmbientePedido;
                    return "(" + AmbientePedidoEspelhoDAO.Instance.GetDescrMaoObra(idAmbientePedido).Trim() + ")";
                }
                else
                    return "";
                //return "(" + ProdutoPedidoEspelhoBenefDAO.Instance.GetDescrBenef(_idProdPed).Trim() + ")";
            }
        }

        public string NomeCorSetor
        {
            get 
            { 
                if(SituacaoProducao == (int)ProdutoPedidoProducao.SituacaoEnum.Producao)
                {
                    var cor = (Glass.Data.Model.CorSetor)this.CorSetor;
                    var field = typeof(Glass.Data.Model.CorSetor).GetField(cor.ToString());
                    if (field == null) return "Black";
                    return ((SoapEnumAttribute)field.GetCustomAttributes(typeof(SoapEnumAttribute), false).First()).Name;
                }
                else
                    return "Gray"; 
            }
        }

        public string DescrTipoPerda
        {
            get
            {
                return SituacaoProducao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda && TipoPerdaProducao != null ?
                    TipoPerdaDAO.Instance.GetNome((uint)TipoPerdaProducao.Value) + (!String.IsNullOrEmpty(ObsPerdaProducao) ? " - " + ObsPerdaProducao : "") : null;
            }
        }

        public string DescrTipoPerdaLista
        {
            get
            {
                string tipoPerda = DescrTipoPerda;
                return !String.IsNullOrEmpty(tipoPerda) ? "Perda: " + tipoPerda : null;
            }
        }

        public string Pedido
        {
            get 
            {
                return IdPedido.ToString() + (IdPedidoAnterior > 0 ? " (" + IdPedidoAnterior.Value.ToString() + "R)" : "") +
                    (IdPedidoExpedicao > 0 ? " (Exp. " + IdPedidoExpedicao.Value.ToString() + ")" : ""); 
            }
        }

        #endregion
    }
}