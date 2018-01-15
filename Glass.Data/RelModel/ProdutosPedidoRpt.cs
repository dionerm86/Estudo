using Glass.Data.Model;
using Glass.Data.DAL;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosPedidoRptDAL))]
    public class ProdutosPedidoRpt
    {
        #region Construtores

        public ProdutosPedidoRpt()
        {
        }

        public ProdutosPedidoRpt(ProdutosPedido prodPed, bool incluirQtdeAmbiente)
        {
            IdProdPed = prodPed.IdProdPed;
            IdPedido = prodPed.IdPedido;
            TituloAltLarg1 = prodPed.TituloAltLarg1;
            TituloAltLarg2 = prodPed.TituloAltLarg2;
            IdAmbientePedido = prodPed.IdAmbientePedido;
            Ambiente = prodPed.Ambiente;
            DescrAmbiente = prodPed.DescrAmbiente;
            ObsProjeto = prodPed.ObsProjeto;
            CodInterno = prodPed.CodInterno;
            Qtde = prodPed.Qtde;
            QtdeDisponivelLiberacao = prodPed.QtdeDisponivelLiberacao;
            TotalCalc = prodPed.TotalCalc;
            AltLarg1 = prodPed.AltLarg1;
            AltLarg2 = prodPed.AltLarg2;
            DescrProduto = prodPed.DescrProduto;
            DescricaoProdutoComBenef = prodPed.DescricaoProdutoComBenef;
            CodAplicacao = prodPed.CodAplicacao;
            CodProcesso = prodPed.CodProcesso;
            TotM2Calc = prodPed.TotM2Calc;
            TotM = prodPed.TotM;
            ValorVendido = prodPed.ValorVendido;
            Total = prodPed.Total;
            ValorBenef = prodPed.ValorBenef;
            PedidoMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(IdPedido);
            DescrBeneficiamentos = prodPed.DescrBeneficiamentos;
            IdProdPedProducaoConsulta = prodPed.IdProdPedProducaoConsulta;
            TipoDescontoAmbiente = prodPed.TipoDescontoAmbiente;
            DescontoAmbiente = prodPed.DescontoAmbiente;
            Espessura = prodPed.Espessura;
            ValorDescontoQtde = prodPed.ValorDescontoQtde;
            PedCli = prodPed.PedCli;
            IsProdLamComposicao = prodPed.IsProdLamComposicao;
            IdProdPedParent = prodPed.IdProdPedParent;
            IsProdLamComposicaoComFilho = prodPed.IsProdLamComposicaoComFilho;

            var corVidro = ProdutoDAO.Instance.ObtemIdCorVidro((int)prodPed.IdProd);
            
            if (corVidro > 0)
                CorVidro = CorVidroDAO.Instance.GetNome((uint)corVidro.GetValueOrDefault());

            if (incluirQtdeAmbiente)
                QtdeAmbiente = prodPed.QtdeAmbiente;
        }

        #endregion

        #region Propriedades

        public uint IdProdPed { get; set; }

        public uint IdPedido { get; set; }

        public string TituloAltLarg1 { get; set; }

        public string TituloAltLarg2 { get; set; }

        public uint? IdAmbientePedido { get; set; }

        public string Ambiente { get; set; }

        public string DescrAmbiente { get; set; }

        public string ObsProjeto { get; set; }

        public string CodInterno { get; set; }

        public float Qtde { get; set; }

        public float QtdeDisponivelLiberacao { get; set; }

        public decimal TotalCalc { get; set; }

        public string AltLarg1 { get; set; }

        public string AltLarg2 { get; set; }

        public string DescrProduto { get; set; }

        public string DescricaoProdutoComBenef { get; set; }

        public string CodAplicacao { get; set; }

        public string CodProcesso { get; set; }

        public float TotM2Calc { get; set; }

        public float TotM { get; set; }

        public decimal ValorVendido { get; set; }

        public decimal Total { get; set; }

        public int QtdeAmbiente { get; set; }

        public decimal ValorBenef { get; set; }

        public bool PedidoMaoDeObra { get; set; }

        public string DescrBeneficiamentos { get; set; }

        public uint? IdProdPedProducaoConsulta { get; set; }

        public int TipoDescontoAmbiente { get; set; }

        public decimal DescontoAmbiente { get; set; }

        public float Espessura { get; set; }

        public string CorVidro { get; set; }

        public decimal ValorDescontoQtde { get; set; }

        public string PedCli { get; set; }

        public bool IsProdLamComposicao { get; set; }

        public uint? IdProdPedParent { get; set; }

        public bool IsProdLamComposicaoComFilho { get; set; }

        #endregion
    }
}