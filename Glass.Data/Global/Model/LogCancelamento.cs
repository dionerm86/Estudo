using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LogCancelamentoDAO))]
    [PersistenceClass("log_cancelamento")]
    public class LogCancelamento
    {
        #region Enumeradores

        public enum TabelaCancelamento : byte
        {
            ContasReceber               = 1,
            ContasPagar,
            MovimentacaoBancaria,
            ImpressaoEtiqueta,
            PecaProjetoModelo,          // 5
            MaterialProjetoModelo,
            CaixaGeral,
            Acerto,
            TrocaDevolucao,
            Pedido,                     // 10
            LiberarPedido,
            Sinal,
            AcertoCheque,
            Obra,
            DevolucaoPagamento,         // 15
            CreditoFornecedor,
            MovEstoque,
            MovEstoqueFiscal,
            FinalizacaoInstalacao,
            ProdutoImpressao,           // 20
            RetalhoProducao,
            SinalCompra,
            AntecipFornec,
            EncontroContas,
            DepositoNaoIdentificado,    // 25
            CotacaoCompras,
            ConciliacaoBancaria,
            RegraNaturezaOperacao,
            MovEstoqueCliente,
            InventarioEstoque,          // 30
            ControleCreditosEfd,
            PerdaChapaVidro,
            Instalacao,
            DepositoCheque,
            PedidoOC,                  //35
            OrdemCarga,
            TextoPedido,
            CartaoNaoIdentificado,
            ArquivoCartaoNaoIdentificado,
            Medicao,                   //40
            ImpostoServico,            //41
            Pagto                      //42
        }

        public static string GetDescrTabela(int tabela)
        {
            return GetDescrTabela((TabelaCancelamento)tabela);
        }

        public static string GetDescrTabela(TabelaCancelamento tabela)
        {
            switch (tabela)
            {
                case TabelaCancelamento.ContasPagar: return "Contas a Pagar";
                case TabelaCancelamento.ContasReceber: return "Contas a Receber";
                case TabelaCancelamento.MovimentacaoBancaria: return "Movimentação Bancária";
                case TabelaCancelamento.ImpressaoEtiqueta: return "Impressão de Etiquetas";
                case TabelaCancelamento.PecaProjetoModelo: return "Peça do Modelo de Projeto";
                case TabelaCancelamento.MaterialProjetoModelo: return "Material do Modelo de Projeto";
                case TabelaCancelamento.CaixaGeral: return "Caixa Geral";
                case TabelaCancelamento.Acerto: return "Acerto";
                case TabelaCancelamento.TrocaDevolucao: return "Troca/Devolução";
                case TabelaCancelamento.Pedido: return "Pedido";
                case TabelaCancelamento.LiberarPedido: return "Liberação de Pedido";
                case TabelaCancelamento.Sinal: return "Sinal/Pagto. Antecipado";
                case TabelaCancelamento.AcertoCheque: return "Acerto de Cheque";
                case TabelaCancelamento.Obra: return "Pagamento Antecipado de Obra";
                case TabelaCancelamento.DevolucaoPagamento: return "Devolução de Pagamento";
                case TabelaCancelamento.CreditoFornecedor: return "Crédito Fornecedor";
                case TabelaCancelamento.MovEstoque: return "Extrato de Estoque";
                case TabelaCancelamento.MovEstoqueFiscal: return "Extrato de Estoque Fiscal";
                case TabelaCancelamento.FinalizacaoInstalacao: return "Finalizaçao de Instalação";
                case TabelaCancelamento.RetalhoProducao: return "Retalho de Produção";
                case TabelaCancelamento.ProdutoImpressao: return "Produto Impresso";
                case TabelaCancelamento.SinalCompra: return "Sinal da Compra";
                case TabelaCancelamento.AntecipFornec: return "Antecipação de Pagamento de Fornecedor";
                case TabelaCancelamento.EncontroContas: return "Encontro Contas a Pagar/Receber";
                case TabelaCancelamento.DepositoNaoIdentificado: return "Deposito Não Identificado";
                case TabelaCancelamento.CotacaoCompras: return "Cotação de Compras";
                case TabelaCancelamento.ConciliacaoBancaria: return "Conciliação Bancária";
                case TabelaCancelamento.RegraNaturezaOperacao: return "Regra de Natureza de Operação";
                case TabelaCancelamento.MovEstoqueCliente: return "Estrato de Estoque de Cliente";
                case TabelaCancelamento.InventarioEstoque: return "Inventário de Estoque";
                case TabelaCancelamento.ControleCreditosEfd: return "Controle de Créditos - EFD";
                case TabelaCancelamento.Instalacao: return "Instalação";
                case TabelaCancelamento.PerdaChapaVidro: return "Perda de Chapa de Vidro";
                case TabelaCancelamento.DepositoCheque: return "Depósito Cheque";
                case TabelaCancelamento.PedidoOC: return "Ordem de Carga";
                case TabelaCancelamento.OrdemCarga: return "Carregamento";
                case TabelaCancelamento.TextoPedido: return "Texto Pedido";
                case TabelaCancelamento.CartaoNaoIdentificado: return "Cartão não Identificado";
                case TabelaCancelamento.ArquivoCartaoNaoIdentificado: return "Arquivo cartão não identificado";
                case TabelaCancelamento.Medicao: return "Medição";
                case TabelaCancelamento.ImpostoServico: return "Imposto/Serviço";
                case TabelaCancelamento.Pagto: return "Pagamento";
                default: return "";
            }
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="idRegistroCanc"></param>
        /// <returns></returns>
        public static string GetReferencia(int tabela, uint idRegistroCanc)
        {
            return GetReferencia((TabelaCancelamento)tabela, idRegistroCanc);
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(TabelaCancelamento tabela, uint idRegistroCanc)
        {
            return GetReferencia(null, tabela, idRegistroCanc);
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(GDASession session, TabelaCancelamento tabela, uint idRegistroCanc)
        {
            try
            {
                string referencia;

                switch (tabela)
                {
                    case TabelaCancelamento.ContasReceber:
                        int numParc = ContasReceberDAO.Instance.ObtemValorCampo<int>(session, "numParc", "idContaR=" + idRegistroCanc);
                        int numParcMax = ContasReceberDAO.Instance.ObtemValorCampo<int>(session, "numParcMax", "idContaR=" + idRegistroCanc);
                        referencia = ContasReceberDAO.Instance.GetReferencia(session, idRegistroCanc) + " Parc: " + numParc + "/" + numParcMax;
                        break;
                    case TabelaCancelamento.ContasPagar:
                        referencia = idRegistroCanc + " Ref.: " + ContasPagarDAO.Instance.GetReferencia(session, idRegistroCanc);
                        break;
                    case TabelaCancelamento.PecaProjetoModelo: referencia = "Item " + PecaProjetoModeloDAO.Instance.ObtemValorCampo<string>(session, "item", "idPecaProjMod=" + idRegistroCanc); break;
                    case TabelaCancelamento.MaterialProjetoModelo: referencia = MaterialProjetoModeloDAO.Instance.GetElement(session, idRegistroCanc).DescrProdProj; break;
                    case TabelaCancelamento.RetalhoProducao: referencia = RetalhoProducaoDAO.Instance.GetElementByPrimaryKey(session, idRegistroCanc).NumeroEtiqueta; break;
                    case TabelaCancelamento.ProdutoImpressao: referencia = ProdutoImpressaoDAO.Instance.GetElementByPrimaryKey(session, idRegistroCanc).NumEtiqueta; break;
                    case TabelaCancelamento.RegraNaturezaOperacao: referencia = RegraNaturezaOperacaoDAO.Instance.ObtemDescricao(session, idRegistroCanc); break;
                    case TabelaCancelamento.ControleCreditosEfd:
                        ControleCreditoEfd item = ControleCreditoEfdDAO.Instance.GetElementByPrimaryKey(session, idRegistroCanc);
                        referencia = item.PeriodoGeracao + " - Imposto: " + item.DescrTipoImposto + (item.CodCred != null ? " - Cód. Cred.: " + item.DescrCodCred : "");
                        break;
                    default: referencia = idRegistroCanc.ToString(); break;
                }

                return referencia != null && referencia.Length > 100 ? referencia.Substring(0, 100) : referencia;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDLOGCANCELAMENTO", PersistenceParameterType.IdentityKey)]
        public int IdLogCancelamento { get; set; }

        [PersistenceProperty("TABELA")]
        public int Tabela { get; set; }

        [PersistenceProperty("IDREGISTROCANC")]
        public int IdRegistroCanc { get; set; }

        [PersistenceProperty("NUMEVENTO")]
        public uint NumEvento { get; set; }

        [PersistenceProperty("IDFUNCCANC")]
        public uint IdFuncCanc { get; set; }

        [PersistenceProperty("DATACANC")]
        public DateTime DataCanc { get; set; }

        [PersistenceProperty("MOTIVO")]
        public string Motivo { get; set; }

        [PersistenceProperty("CANCELAMENTOMANUAL")]
        public bool CancelamentoManual { get; set; }

        [PersistenceProperty("CAMPO")]
        public string Campo { get; set; }

        [PersistenceProperty("VALOR")]
        public string Valor { get; set; }

        [PersistenceProperty("REFERENCIA")]
        public string Referencia { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNCCANC", DirectionParameter.InputOptional)]
        public string NomeFuncCanc { get; set; }

        #endregion
    }
}
