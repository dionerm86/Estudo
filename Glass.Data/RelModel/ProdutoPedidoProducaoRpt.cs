using System;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutoPedidoProducaoRptDAL))]
    public class ProdutoPedidoProducaoRpt
    {
        #region Construtores

        public ProdutoPedidoProducaoRpt()
        {
        }

        public ProdutoPedidoProducaoRpt(ProdutoPedidoProducao prodPedProducao)
        {
            IdProdPedProducao = prodPedProducao.IdProdPedProducao;
            SituacaoProducao = prodPedProducao.SituacaoProducao;
            IdPedido = prodPedProducao.IdPedido;
            DescrProduto = prodPedProducao.DescrProduto;
            DescrBeneficiamentos = prodPedProducao.DescrBeneficiamentos;
            LarguraAltura = prodPedProducao.LarguraAltura;
            DescrTipoPerdaLista = prodPedProducao.DescrTipoPerdaLista;
            NumEtiquetaExibir = prodPedProducao.NumEtiquetaExibir;
            PecaCancelada = prodPedProducao.PecaCancelada;
            IdImpressao = prodPedProducao.IdImpressao;
            Criterio = prodPedProducao.Criterio;
            CorLinha = prodPedProducao.CorLinha;
            NumEtiquetaChapa = prodPedProducao.NumEtiquetaChapa;
            IdCliente = prodPedProducao.IdCliente;
            NomeCliente = prodPedProducao.NomeCliente;
            IdPedidoExibir = prodPedProducao.IdPedidoExibir;
            SiglaTipoPedido = prodPedProducao.SiglaTipoPedido;
            CodCliente = prodPedProducao.CodCliente;
            IdNomeCliente = prodPedProducao.IdNomeCliente;
            CodAplicacao = prodPedProducao.CodAplicacao;
            CodProcesso = prodPedProducao.CodProcesso;
            DataEntregaExibicao = prodPedProducao.DataEntregaExibicao;
            DataLiberacaoPedido = prodPedProducao.DataLiberacaoPedido;
            PlanoCorte = prodPedProducao.PlanoCorte;
            TotM2 = prodPedProducao.TotM2;
            LoteChapa = prodPedProducao.LoteChapa;
            TotM = prodPedProducao.TotM;
            NumeroNFeChapa = prodPedProducao.NumeroNFeChapa;
        }

        #endregion

        #region Propriedades

        public uint IdProdPedProducao { get; set; }

        public int SituacaoProducao { get; set; }

        public uint IdPedido { get; set; }

        public string DescrProduto { get; set; }

        public string DescrBeneficiamentos { get; set; }

        public string LarguraAltura { get; set; }

        public string DescrTipoPerdaLista { get; set; }

        public string NumEtiquetaExibir { get; set; }

        public bool PecaCancelada { get; set; }

        public uint? IdImpressao { get; set; }

        public string Criterio { get; set; }

        public string CorLinha { get; set; }

        public string NumEtiquetaChapa { get; set; }

        public uint? IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public string IdPedidoExibir { get; set; }

        public string SiglaTipoPedido { get; set; }

        public string CodCliente { get; set; }

        public string IdNomeCliente { get; set; }

        public string CodAplicacao { get; set; }
        
        public string CodProcesso { get; set; }

        public string DataEntregaExibicao { get; set; }

        public DateTime? DataLiberacaoPedido { get; set; }

        public string PlanoCorte { get; set; }

        public double TotM2 { get; set; }

        public string LoteChapa { get; set; }

        public double TotM { get; set; }

        public string NumeroNFeChapa { get; set; }

        #endregion
    }
}