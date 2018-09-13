using System;
using System.Collections.Generic;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MovBancoDAO : BaseCadastroDAO<MovBanco, MovBancoDAO>
	{
        //private MovBancoDAO() { }

        #region Variáveis globais

        private static readonly object _correcaoSaldoMovBancoLock = new object();
        private static readonly object _inserirMovimentacaoLock = new object();
        private static readonly object _transferirContaBancariaParaCaixaGeralLock = new object();

        #endregion

        #region Busca as movimentações da conta passada no período passado

        /// <summary>
        /// Busca as movimentações realizadas na conta passada dentro do período informado
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        public MovBanco[] GetMovimentacoes(uint idContaBanco, string dtIni, string dtFim, float valorInicial, float valorFinal, int tipoMov, 
            bool lancManual)
        {
            string criterioRpt = String.Empty;

            string sql = @"
                Select m.*, '$$$' as criterio, f.Nome as DescrUsuCad, if(pl.idGrupo not in (" +
                    UtilsPlanoConta.GetGruposSistema + @"), Concat(g.Descricao, ' - ', pl.Descricao), pl.Descricao) as DescrPlanoConta,
                    l.NomeFantasia as NomeLoja, cli.Nome as NomeCliente, Coalesce(forn.RazaoSocial, forn.NomeFantasia) as NomeFornec,
                    Concat(cb.Nome, ' Agência: ', cb.Agencia, ' Conta: ', cb.Conta) as DescrContaBanco,
                    IF(pp.NumBoleto != '', CONCAT('Num.Boleto: ', pp.NumBoleto), '') AS NumBoleto
                From mov_banco m 
                    Inner Join conta_banco cb On (m.idContaBanco=cb.idContaBanco)
                    Left Join funcionario f On (m.UsuCad=f.IdFunc) 
                    Left Join loja l On (f.IdLoja=l.IdLoja) 
                    Left Join plano_contas pl On (m.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo) 
                    Left Join cliente cli on (m.IdCliente=cli.Id_Cli) 
                    Left Join fornecedor forn on (m.idFornec=forn.idFornec)
                    LEFT JOIN pagto_pagto pp ON (m.IdPagto=pp.IdPagto) And (pp.IdContaBanco = m.IdContaBanco)
                Where 1 ";

            if (idContaBanco > 0)
            {
                sql += " And m.IdContaBanco=" + idContaBanco;
                criterioRpt += "Conta: " + ContaBancoDAO.Instance.GetDescricao(idContaBanco) + "    ";
            }
            else
                criterioRpt += "Todas contas bancárias    ";

            if (!String.IsNullOrEmpty(dtIni))
            {
                sql += " And m.DataMov>=?dtIni";
                criterioRpt += "Movimentações do dia: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                sql += " And m.DataMov<=?dtFim";
                criterioRpt = (dtFim != dtIni ? criterioRpt.Trim() + " a " + dtFim: criterioRpt) + "    ";
            }

            if (valorInicial > 0)
            {
                sql += " And valorMov>=" + valorInicial.ToString().Replace(",", ".");
                criterioRpt += "Valor: a partir de " + valorInicial.ToString("C") + (valorFinal > 0 ? "" : "    ");
            }

            if (valorFinal > 0)
            {
                sql += " And valorMov<=" + valorFinal.ToString().Replace(",", ".");
                criterioRpt += valorInicial > 0 ? " até " + valorFinal.ToString("C") + "    " : "Valor: até " + valorFinal.ToString("C") + "    ";
            }

            if (tipoMov > 0)
            {
                sql += " And m.TipoMov=" + tipoMov;
                criterioRpt += "Apenas movimetações de " + (tipoMov == 1 ? "entrada" : "saída") + "    ";
            }

            if (lancManual)
            {
                sql += " And m.lancManual=" + lancManual;
                criterioRpt += "Apenas lançamentos manuais    ";
            }

            sql += " Group by IdMovBanco Order By DATE_FORMAT(m.dataMov, '%Y-%m-%d %H%i') Asc, IdMovBanco Asc";

            List<MovBanco> lstMov = objPersistence.LoadData(sql.Replace("$$$", criterioRpt), GetParams(dtIni, dtFim));

            for (int i = 0; i < lstMov.Count; i++)
            {
                if (lstMov[i].IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancaria) && lstMov[i].IdContaBancoDest > 0)
                    lstMov[i].DescrPlanoConta += ContaBancoDAO.Instance.GetElement(lstMov[i].IdContaBancoDest.Value).Descricao;

                // Inclui mov. SALDO ANTERIOR
                if (idContaBanco > 0 && (i == 0 ||
                    lstMov[i].DataMov.ToString("dd/MM/yyyy") != lstMov[i - 1].DataMov.ToString("dd/MM/yyyy")))
                {
                    MovBanco mov = new MovBanco();
                    mov.Obs = "SALDO ANTERIOR";
                    mov.Saldo = i > 0 ? lstMov[i - 1].Saldo : lstMov[0].Saldo - (lstMov[0].ValorMov * ((decimal)lstMov[0].TipoMov == 1 ? 1 : -1));
                    mov.Criterio = lstMov[0].Criterio;
                    lstMov.Insert(i, mov);
                    i++;
                }
            }

            // Inclui mov. SALDO ATUAL, CHEQUES PRÓPRIOS EM ABERTO, SALDO PREVISTO
            if (idContaBanco > 0 && lstMov.Count > 0)
            {
                MovBanco mov = new MovBanco();
                mov.Obs = "SALDO ATUAL";
                mov.Saldo = lstMov[lstMov.Count-1].Saldo;
                lstMov.Add(mov);

                mov = new MovBanco();
                mov.Obs = "CHEQUES PRÓPRIOS EM ABERTO";
                mov.Saldo = (decimal)ChequesDAO.Instance.GetTotalProp(idContaBanco);
                lstMov.Add(mov);

                mov = new MovBanco();
                mov.Obs = "SALDO PREVISTO";
                mov.Saldo = lstMov[lstMov.Count - 2].Saldo - lstMov[lstMov.Count - 1].Saldo;
                lstMov.Add(mov);  
            }

            if (idContaBanco > 0 && lstMov.Count > 0)
            {
                var ultConciliacao = ConciliacaoBancariaDAO.Instance.ObtemDataUltimaConciliacao(idContaBanco);

                if (ultConciliacao.HasValue)
                {
                    for (int i = lstMov.Count - 1; i >= 0; i--)
                    {
                        if (lstMov[i].DataMov.Date.ToString("dd/MM/yyyy") != "01/01/0001" && lstMov[i].DataMov.Date <= ultConciliacao.Value.Date)
                        {
                            MovBanco mov = new MovBanco();
                            mov.Obs = "CONCILIADA";
                            lstMov.Insert(i + 1, mov);
                            break;
                        }
                    }
                }
            }

            return lstMov.ToArray();
        }

        private GDAParameter[] GetParams(string dtIni, string dtFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna a movimentação do sinal do pedido passado

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca o sinal recebido na conta referente ao pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public MovBanco GetPedidoSinal(uint idPedido)
        {
            return GetPedidoSinal(null, idPedido);
        }

        /// <summary>
        /// Busca o sinal recebido na conta referente ao pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public MovBanco GetPedidoSinal(GDASession sessao, uint idPedido)
        {
            uint sinalDinheiro = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaDinheiro);
            uint sinalCheque = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaCheque);
            uint sinalCartao = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaCartao);
            uint sinalConstrucard = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaConstrucard);
            uint sinalDeposito = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaDeposito);
            uint sinalPermuta = UtilsPlanoConta.GetPlanoConta(Glass.Data.Helper.UtilsPlanoConta.PlanoContas.EntradaPermuta);

            string sql = "Select m.*, f.Nome as DescrUsuCad From mov_banco m " +
                "Left Join funcionario f On m.UsuCad=f.IdFunc " +
                "Where idPedido=" + idPedido + " And (IdConta=" + sinalDinheiro + " Or IdConta=" + sinalCheque +
                " Or IdConta=" + sinalCartao + " Or IdConta=" + sinalConstrucard + " Or IdConta=" + sinalDeposito +
                " Or IdConta=" + sinalPermuta + ")";

            List<MovBanco> lst = objPersistence.LoadData(sessao, sql).ToList();

            return lst.Count > 0 ? lst[0] : null;
        }

        #endregion

        #region Retorna o saldo
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo da conta passada
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public decimal GetSaldo(uint idContaBanco)
        {
            return GetSaldo(null, idContaBanco);
        }

        /// <summary>
        /// Recupera o saldo da conta passada
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public decimal GetSaldo(GDASession sessao, uint idContaBanco)
        {
            return GetSaldo(sessao, idContaBanco, null, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="descontarChequesPropriosAbertos"></param>
        /// <returns></returns>
        public decimal GetSaldo(uint idContaBanco, bool descontarChequesPropriosAbertos)
        {
            return GetSaldo(null, idContaBanco, descontarChequesPropriosAbertos);
        }

        public decimal GetSaldo(GDASession sessao, uint idContaBanco, bool descontarChequesPropriosAbertos)
        {
            return GetSaldo(sessao, idContaBanco, null, descontarChequesPropriosAbertos);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo da conta passada
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="dataSaldo">Determina de qual dia será buscado o saldo</param>
        /// <param name="descontarChequesPropriosAbertos">Determina se o valor dos cheques próprios será descontado do saldo</param>
        /// <returns></returns>
        public decimal GetSaldo(uint idContaBanco, string dataSaldo, bool descontarChequesPropriosAbertos)
        {
            return GetSaldo(null, idContaBanco, dataSaldo, descontarChequesPropriosAbertos);
        }

        /// <summary>
        /// Recupera o saldo da conta passada
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="dataSaldo">Determina de qual dia será buscado o saldo</param>
        /// <param name="descontarChequesPropriosAbertos">Determina se o valor dos cheques próprios será descontado do saldo</param>
        /// <returns></returns>
        public decimal GetSaldo(GDASession sessao, uint idContaBanco, string dataSaldo, bool descontarChequesPropriosAbertos)
        {
            string sql = "Select idMovBanco From mov_banco Where idContaBanco=" + idContaBanco;

            GDAParameter[] lstParam = null;

            if (!String.IsNullOrEmpty(dataSaldo))
            {
                sql += " And dataMov<=?dataSaldo";
                lstParam = new GDAParameter[] { new GDAParameter("?dataSaldo", DateTime.Parse(dataSaldo + " 23:59:59")) };
            }

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') {0}, IdMovBanco {0} limit 1";

            uint idMovBancoUlt = ExecuteScalar<uint>(sessao, String.Format(sql, "Desc"), lstParam);
            uint idMovBancoPrim = ExecuteScalar<uint>(sessao, String.Format(sql, "Asc"), lstParam);

            decimal saldo = ObtemValorCampo<decimal>(sessao, "saldo", "idMovBanco=" + (idMovBancoUlt > 0 ? idMovBancoUlt : idMovBancoPrim));

            if (descontarChequesPropriosAbertos && dataSaldo == null)
                saldo -= (decimal)ChequesDAO.Instance.GetTotalProp(sessao, idContaBanco);

            return saldo;
        }

        #endregion

        #region Retorna o saldo por forma de pagamento

        /// <summary>
        /// Retorna o saldo por forma de pagamento
        /// </summary>
        /// <param name="formaPagto"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public decimal GetSaldoByFormaPagto(Glass.Data.Model.Pagto.FormaPagto formaPagto, string dataIni, string dataFim)
        {
            string sql = "Select Coalesce((Select Sum(valorMov) From mov_banco Where tipomov=1 and idConta in(" +
                UtilsPlanoConta.GetLstEntradaByFormaPagto(formaPagto, 0, false).Replace("," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancariaParaCxGeralDinheiro).ToString(), "") + ")&dt&), 0)-" +
                "Coalesce((Select Sum(valorMov) From mov_banco Where tipomov=2 and (idConta in (" +
                UtilsPlanoConta.GetLstSaidaByFormaPagto(formaPagto, 0, 1) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancariaParaCxGeralDinheiro).ToString() + "))&dt&), 0) as total";

            sql = sql.Replace("&dt&", !String.IsNullOrEmpty(dataIni) ? " And dataMov>=?dtIni" : String.Empty);
            sql = sql.Replace("&dt&", !String.IsNullOrEmpty(dataFim) ? " And dataMov<=?dtFim" : String.Empty);

            return ExecuteScalar<decimal>(sql, GetParams(dataIni, dataFim));
        }

        #endregion

        #region Busca movimentações relacionadas a um acerto

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public MovBanco[] GetByAcerto(GDASession sessao, uint idAcerto)
        {
            var sql =
                string.Format(@"
                    Select m.*, Concat(cb.Nome, ' Agência: ', cb.Agencia, ' Conta: ', cb.Conta) as DescrContaBanco 
                    From mov_banco m
                        Inner Join conta_banco cb On (m.idContaBanco=cb.idContaBanco)
                        LEFT JOIN contas_receber cr On (m.IdContaR=cr.IdContaR)
                    Where (m.IdAcerto={0} OR (cr.IdAcerto={0} AND m.IdAcerto IS NULL))", idAcerto);

            sql += " ORDER BY DATE_FORMAT(m.DataCad, '%Y-%m-%d %H%i') DESC, IdMovBanco DESC";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um acerto de cheque

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public MovBanco[] GetByAcertoCheque(uint idAcertoCheque)
        {
            return GetByAcertoCheque(null, idAcertoCheque);
        }

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public MovBanco[] GetByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            string sql = "Select * From mov_banco where idAcertoCheque=" + idAcertoCheque;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um pedido à vista

        /// <summary>
        /// Retorna movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public MovBanco[] GetByPedidoAVista(uint idPedido)
        {
            return GetByPedidoAVista(null, idPedido);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public MovBanco[] GetByPedidoAVista(GDASession session, uint idPedido)
        {
            string sql = "Select * From mov_banco mb Where idConta In (" + UtilsPlanoConta.ContasAVista() + ") And idPedido=" + idPedido;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas à uma liberação

        /// <summary>
        /// Busca movimentações relacionadas à uma liberação
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="tipo">1-À Vista, 2-Entrada</param>
        /// <returns></returns>
        public MovBanco[] GetByLiberacao(GDASession sessao, uint idLiberarPedido, int tipo)
        {
            // Busca o plano conta de juros de cartão apenas no caso de estar buscando sinal da liberação, isso porque no método de cancelamento
            // de liberação à vista, são estornados tanto movimentações da liberação à vista quanto de sinal, por isso da forma como está será
            // estornado apenas uma vez
            string idConta =
                tipo == 1 ? UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.ContasTipoPrazoCartao() +"," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) :
                tipo == 2 ? UtilsPlanoConta.ContasSinalPedido() + "," + FinanceiroConfig.PlanoContaJurosCartao + "," + 
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) : "";

            string sql = @"
                Select * From mov_banco 
                Where idConta In (" + idConta + @") 
                    And idLiberarPedido=" + idLiberarPedido;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um sinal

        /// <summary>
        /// Retorna movimentações relacionadas a um sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public MovBanco[] GetBySinal(uint idSinal)
        {
            return GetBySinal(null, idSinal);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a um sinal
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public MovBanco[] GetBySinal(GDASession sessao, uint idSinal)
        {
            string sql = "Select * From mov_banco Where idConta In (" + UtilsPlanoConta.ContasSinalPedido() + "," +
                FinanceiroConfig.PlanoContaJurosCartao + ") And idSinal=" + idSinal;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList().ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um sinal

        /// <summary>
        /// Retorna movimentações relacionadas a um sinal
        /// </summary>
        public IList<MovBanco> GetBySinalCompra(GDASession session, uint idSinalCompra, uint? idConta)
        {
            string filtroConta = idConta > 0 ? "=" + idConta.Value : " In (" + UtilsPlanoConta.ContasPagto() + ")";
            string sql = "Select * From mov_banco mb Where idConta" + filtroConta + " And idSinalCompra=" + idSinalCompra;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a uma antecipação de pagmento de fornecedor

        /// <summary>
        /// Retorna movimentações relacionadas a uma antecipação de fornecedor
        /// </summary>
        public IList<MovBanco> GetByAntecipFornec(GDASession session, uint idAntecipFornec, uint? idConta)
        {
            string filtroConta = idConta > 0 ? "=" + idConta.Value : " In (" + UtilsPlanoConta.ContasAntecipFornec() + ")";
            string sql = "Select * From mov_banco mb Where idConta" + filtroConta + " And idAntecipFornec=" + idAntecipFornec;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas à uma conta recebida

        /// <summary>
        /// Busca movimentações relacionadas à uma conta recebida
        /// </summary>
        public MovBanco[] GetByContaRec(uint idContaR)
        {
            return GetByContaRec(null, idContaR);
        }

        /// <summary>
        /// Busca movimentações relacionadas à uma conta recebida
        /// </summary>
        public MovBanco[] GetByContaRec(GDASession session, uint idContaR)
        {
            return GetByContaRec(session, idContaR, null);
        }

        /// <summary>
        /// Busca movimentações relacionadas à uma conta recebida
        /// </summary>
        public MovBanco[] GetByContaRec(GDASession session, uint idContaR, int? idArquivoQuitacaoParcelaCartao)
        {
            string sql = "Select * From mov_banco where idContaR=" + idContaR;

            if (idArquivoQuitacaoParcelaCartao.GetValueOrDefault(0) > 0)
                 sql +=" OR IdArquivoQuitacaoParcelaCartao=" + idArquivoQuitacaoParcelaCartao;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList().ToArray();
        }

        /// <summary>
        /// Recupera movimentações bancárias referentes à um depósito não identificado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCartaoNaoIdentificado"></param>
        /// <returns></returns>
        public IList<MovBanco> ObterMovimentacoesPorCartaoNaoIdentificado(GDASession sessao, int idCartaoNaoIdentificado)
        {
            string sql = "Select * From mov_banco where idcartaonaoidentificado=" + idCartaoNaoIdentificado;

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Retorna as movimentações para cancelamento (são ordenadas pela ordem de lançamento)
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public List<MovBanco> GetByContaRecForCanc(GDASession sessao, uint idContaR)
        {
            string sql = "Select * From mov_banco where idContaR=" + idContaR + " Order By IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a uma obra

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByObra(uint idObra)
        {
            return GetByObra(null, idObra);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByObra(GDASession sessao, uint idObra)
        {
            var idsConta = UtilsPlanoConta.ContasAVista();

            if (FinanceiroConfig.PlanoContaJurosCartao > 0)
                idsConta += "," + FinanceiroConfig.PlanoContaJurosCartao;

            if (FinanceiroConfig.PlanoContaJurosReceb > 0)
                idsConta += "," + FinanceiroConfig.PlanoContaJurosReceb;

            if (FinanceiroConfig.PlanoContaMultaReceb > 0)
                idsConta += "," + FinanceiroConfig.PlanoContaMultaReceb;

            string sql = String.Format("Select * From mov_banco mb Where idConta In ({0}) And idObra={1}", idsConta, idObra);

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a uma troca/devolução

        /// <summary>
        /// Retorna movimentações relacionadas a uma troca/devolução.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            return GetByTrocaDevolucao(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma troca/devolução.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            string sql = "Select * From mov_banco mb Where idConta In (" + UtilsPlanoConta.ContasAVista() + ") And idTrocaDevolucao=" + idTrocaDevolucao;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a uma devolução de pagamento

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento.
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByDevolucaoPagto(uint idDevolucaoPagto)
        {
            return GetByDevolucaoPagto(null, idDevolucaoPagto);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            string sql = "Select * From mov_banco mb Where idConta In (" + UtilsPlanoConta.ContasDevolucaoPagto() + ") And idDevolucaoPagto=" + idDevolucaoPagto;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a um pagamento

        /// <summary>
        /// Retorna movimentações relacionadas a um pagamento.
        /// </summary>
        public IList<MovBanco> GetByPagto(uint idPagto, uint? idConta)
        {
            return GetByPagto(null, idPagto, idConta);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a um pagamento.
        /// </summary>
        public IList<MovBanco> GetByPagto(GDASession session, uint idPagto, uint? idConta)
        {
            var filtroConta = idConta > 0 ? "=" + idConta.Value : " In (" + UtilsPlanoConta.ContasPagto() + ")";
            var sql = "Select * From mov_banco mb Where idConta" + filtroConta + " And idPagto=" + idPagto;
            
            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a um cheque

        /// <summary>
        /// Retorna movimentações relacionadas ao cheque passado
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByCheque(uint idCheque)
        {
            return GetByCheque(null, idCheque);
        }

        /// <summary>
        /// Retorna movimentações relacionadas ao cheque passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByCheque(GDASession session, uint idCheque)
        {
            string sql = "Select * From mov_banco where idCheque=" + idCheque;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas à um deposito não identificado

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca movimentações relacionadas à um deposito nao identificado
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByDepositoNaoIdentificado(uint idDepositoNaoIdentificado)
        {
            return GetByDepositoNaoIdentificado(null, idDepositoNaoIdentificado);
        }        

        /// <summary>
        /// Busca movimentações relacionadas à um deposito nao identificado
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByDepositoNaoIdentificado(GDASession sessao, uint idDepositoNaoIdentificado)
        {
            string sql = "Select * From mov_banco where idDepositoNaoIdentificado=" + idDepositoNaoIdentificado + " Order By IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Busca movimentações relacionadas à um cartão nao identificado
        /// </summary>
        public IList<MovBanco> GetByCartaoNaoIdentificado(GDASession sessao, uint idCartaoNaoIdentificado)
        {
            string sql = "Select * From mov_banco where idCartaoNaoIdentificado=" + idCartaoNaoIdentificado + " Order By IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a um crédito de fornecedor

        /// <summary>
        /// Retorna movimentações relacionadas a um crédito de fornecedor
        /// </summary>
        /// <param name="idCreditoFornec"></param>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByCreditoFornec(uint idCreditoFornec, uint? idConta)
        {
            return GetByCreditoFornec(null, idCreditoFornec, idConta);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a um crédito de fornecedor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCreditoFornec"></param>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByCreditoFornec(GDASession session, uint idCreditoFornec, uint? idConta)
        {
            string filtroConta = idConta > 0 ? "=" + idConta.Value : " In (" + UtilsPlanoConta.ContasAntecipFornec() + ")";
            string sql = "Select * From mov_banco mb Where idConta" + filtroConta + " And idCreditoFornecedor=" + idCreditoFornec;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca movimentações relacionadas a uma antecipação de conta à receber

        /// <summary>
        /// Retorna movimentações relacionadas a uma antecipação de conta à receber
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<MovBanco> GetByAntecipacaoContaRec(GDASession sessao, uint idAntecip)
        {
            string sql = "Select * From mov_banco mb Where IdAntecipContaRec=" + idAntecip;

            sql += " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Exclui movimentações por PKs

        public void DeleteByPKs(string pks, string motivo)
        {
            DeleteByPKs(null, pks, motivo);
        }

        /// <summary>
        /// Exclui movimentações com as PKs passadas
        /// </summary>
        /// <param name="pks"></param>
        public void DeleteByPKs(GDASession session, string pks, string motivo)
        {
            if (String.IsNullOrEmpty(pks))
                return;

            var movBanco = objPersistence.LoadData(session, "select * from mov_banco where idMovBanco in (" + pks.TrimEnd(',') + ")").ToList();

            // Verifica a conciliação bancária
            foreach (MovBanco m in movBanco)
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, m.IdContaBanco, m.DataMov);

            foreach (MovBanco m in movBanco)
                LogCancelamentoDAO.Instance.LogMovimentacaoBancaria(session, m, motivo, false);

            string sql = "Delete From mov_banco Where idMovBanco In (" + pks.TrimEnd(',') + ")";
            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Corrige o saldo de todas as movimentações de uma conta bancária

        /// <summary>
        /// Corrige o saldo de todas as movimentações de uma conta bancária
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="idMovBancoConc">Conta a ser verificada a conciliação bancaria</param>
        public void CorrigeSaldo(uint idMovBanco, uint idMovBancoConc)
        {
            CorrigeSaldo(null, idMovBanco, idMovBancoConc);
        }

        /// <summary>
        /// Corrige o saldo de todas as movimentações de uma conta bancária
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="idMovBancoConc">Conta a ser verificada a conciliação bancaria</param>
        public void CorrigeSaldo(GDASession sessao, uint idMovBanco, uint idMovBancoConc)
        {
            lock (_correcaoSaldoMovBancoLock)
            {
                uint idContaBanco = ObtemIdContaBanco(sessao, idMovBanco);
                DateTime dataMov = ObtemDataMov(sessao, idMovBanco);
                DateTime dataMovConc = ObtemDataMov(sessao, idMovBancoConc);

                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(sessao, idContaBanco, dataMovConc);

                // O round ao calcular o valor do saldo foi colocado para evitar erros caso o saldo possua 5 na terceira casa decimal
                string sqlAjuste = @"
                    set @saldo := Round(Coalesce((Select saldo From mov_banco
                        Where idContaBanco=" + idContaBanco + @" and dataMov<=?dataMov 
                            And if(dataMov=?dataMov, idMovBanco<" + idMovBanco + @", true)
                        Order By dataMov desc, idMovBanco desc limit 1),0),2);

                    Update mov_banco set saldo=(@saldo := @saldo + 
                        if(tipoMov=1, valorMov, -valorMov))

                    Where idContaBanco=" + idContaBanco + @"
                        And dataMov>=?dataMov
                        And if(dataMov=?dataMov, idMovBanco>=" + idMovBanco + @", true)
                
                    Order by dataMov asc, idMovBanco asc";

                objPersistence.ExecuteCommand(sessao, sqlAjuste, new GDAParameter("?dataMov", dataMov));
            }
        }

        #endregion

        #region Troca movimentações de lugar (Up, Down)

        private static object _trocaMovimentacao = new object();

        /// <summary>
        /// Troca a posição de uma movimentação
        /// </summary>
        public void TrocaMovimentacao(uint idMovBanco, int sentido)
        {
            lock (_trocaMovimentacao)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        uint idContaBanco = ObtemIdContaBanco(transaction, idMovBanco);
                        DateTime dataMov = ObtemDataMov(transaction, idMovBanco);

                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(idContaBanco, dataMov);

                        var lstMov = ObterIdsMovBancoGrupo(transaction, idMovBanco, sentido == 1 ? 2 : 1);
                        var qtdeMovimentacoesAdjacentes = ObterQtdeMovimentacoesAdjacentes(transaction, idContaBanco, dataMov, lstMov[0], sentido);

                        for (int i = 0; i < lstMov.Count; i++)
                        {
                            var idMov = lstMov[i];

                            for (int j = 0; j < qtdeMovimentacoesAdjacentes; j++)
                                idMov = TrocarMovimentacao(transaction, idMov, sentido);
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("MovBancoDAO(TrocaMovimentacao) IDMOVBANCO=" + idMovBanco, ex);
                        throw ex;
                    }
                }
            }
        }

        private uint TrocarMovimentacao(GDATransaction transaction, uint idMovBanco, int sentido)
        {
            var idTrocado = idMovBanco;

            uint idContaBanco = ObtemIdContaBanco(transaction, idMovBanco);
            DateTime dataMov = ObtemDataMov(transaction, idMovBanco);

            List<MovBanco> lstAnterior = objPersistence.LoadData(transaction, @"Select mb.* From mov_banco mb 
                            Where mb.dataMov<=?dataMov and idContaBanco=" + idContaBanco +
                                            " and if(mb.dataMov=?dataMov, mb.IDMOVBANCO<" + idMovBanco + ", 1) " +
                                            "Order By DATE_FORMAT(mb.dataMov, '%Y-%m-%d %H%i') Desc, IdMovBanco Desc limit 1", new GDAParameter("?dataMov", dataMov)).ToList();

            List<MovBanco> lstPosterior = objPersistence.LoadData(transaction, @"Select mb.* From mov_banco mb 
                            Where mb.dataMov>=?dataMov and idContaBanco=" + idContaBanco +
                    " and if(mb.dataMov=?dataMov, mb.IDMOVBANCO>" + idMovBanco + ", 1) " +
                    "Order By DATE_FORMAT(mb.dataMov, '%Y-%m-%d %H%i') Asc, IdMovBanco Asc limit 1", new GDAParameter("?dataMov", dataMov)).ToList();

            if ((sentido == 2 && lstAnterior.Count == 0) || (sentido == 1 && lstPosterior.Count == 0))
                return idTrocado;

            //Se ao mover as movimentações tiverem em dias diferentes, atualiza a data do pagamento ou recebimento
            // de acordo com a data da movimentação
            if ((lstPosterior.Count > 0 && dataMov.ToString("dd/MM/yyyy") != lstPosterior[0].DataMov.ToString("dd/MM/yyyy")) ||
                (lstAnterior.Count > 0 && dataMov.ToString("dd/MM/yyyy") != lstAnterior[0].DataMov.ToString("dd/MM/yyyy")))
            {
                var dtMov = sentido == 1 ? lstPosterior[0].DataMov.AddMinutes(-1) : lstAnterior[0].DataMov.AddMinutes(1);
                AtualizaDataContaPaga(transaction, idMovBanco, dtMov);
                AtualizaDataContaRec(transaction, idMovBanco, dtMov);
            }

            // Avança movimentação
            if (sentido == 1)
            {
                // Se a movimentação posterior estiver em um dia diferente, coloca a movimentação corrente atrás dela
                if (dataMov.ToString("dd/MM/yyyy") != lstPosterior[0].DataMov.ToString("dd/MM/yyyy"))
                {
                    // Iguala a data das movimentações
                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + idMovBanco,
                        new GDAParameter("?dataMov", lstPosterior[0].DataMov.AddMinutes(-1)));
                }
                // Se a movimentação posterior estiver no mesmo dia da movimentação corrente porém com horários diferentes, troca a data
                else if (dataMov.ToString("dd/MM/yyyy HH:mm") != lstPosterior[0].DataMov.ToString("dd/MM/yyyy HH:mm"))
                {
                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + idMovBanco,
                        new GDAParameter("?dataMov", lstPosterior[0].DataMov));

                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + lstPosterior[0].IdMovBanco,
                        new GDAParameter("?dataMov", dataMov));

                    if (idMovBanco < lstPosterior[0].IdMovBanco)
                        idTrocado = TrocaIdMovimentacao(transaction, idMovBanco, lstPosterior[0].IdMovBanco);
                }
                // Se a movimentação estiver no mesmo dia e possuir a mesma data/hora, troca o idMovBanco das duas movimentações
                else if (idMovBanco < lstPosterior[0].IdMovBanco)
                    idTrocado = TrocaIdMovimentacao(transaction, idMovBanco, lstPosterior[0].IdMovBanco);

                // Sempre que houver um item anterior à movimentação que está sendo alterada, corrige o saldo a partir da mesma,
                // pois em alguns casos, corrigir o saldo a patir da movimentação alterada não corrigia o saldo da movimentação anterior
                if (lstAnterior.Count > 0)
                    CorrigeSaldo(transaction, lstAnterior[0].IdMovBanco, idMovBanco);
                else
                    CorrigeSaldo(transaction, idMovBanco, idMovBanco);
            }
            else
            {
                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, idContaBanco, lstAnterior[0].DataMov);

                // Se a movimentação anterior estiver em um dia diferente, coloca a movimentação corrente após ela
                if (dataMov.ToString("dd/MM/yyyy") != lstAnterior[0].DataMov.ToString("dd/MM/yyyy"))
                {
                    // Iguala a data das movimentações
                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + idMovBanco,
                        new GDAParameter("?dataMov", lstAnterior[0].DataMov.AddMinutes(1)));
                }
                // Se a movimentação anterior estiver no mesmo dia da movimentação corrente porém com horários diferentes, troca a data
                else if (dataMov.ToString("dd/MM/yyyy HH:mm") != lstAnterior[0].DataMov.ToString("dd/MM/yyyy HH:mm"))
                {
                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + idMovBanco,
                        new GDAParameter("?dataMov", lstAnterior[0].DataMov));

                    objPersistence.ExecuteCommand(transaction, "Update mov_banco Set dataMov=?dataMov Where idMovBanco=" + lstAnterior[0].IdMovBanco,
                        new GDAParameter("?dataMov", dataMov));

                    if (idMovBanco > lstAnterior[0].IdMovBanco)
                        idTrocado = TrocaIdMovimentacao(transaction, idMovBanco, lstAnterior[0].IdMovBanco);
                }
                // Se a movimentação estiver no mesmo dia e possuir a mesma data/hora, troca o idMovBanco das duas movimentações
                else if (idMovBanco > lstAnterior[0].IdMovBanco)
                    idTrocado = TrocaIdMovimentacao(transaction, idMovBanco, lstAnterior[0].IdMovBanco);

                // Sempre que houver um item anterior à movimentação que está sendo alterada, corrige o saldo a partir da mesma,
                // pois em alguns casos, corrigir o saldo a patir da movimentação alterada não corrigia o saldo da movimentação anterior
                MovBanco mov = ObtemMovAnterior(transaction, lstAnterior[0].IdMovBanco);

                if (mov != null)
                    CorrigeSaldo(transaction, mov.IdMovBanco, lstAnterior[0].IdMovBanco);
                else
                    CorrigeSaldo(transaction, lstAnterior[0].IdMovBanco, lstAnterior[0].IdMovBanco);
            }

            return idTrocado;
        }

        private void AtualizaDataContaPaga(uint idMovBanco, DateTime dataMov)
        {
            AtualizaDataContaPaga(null, idMovBanco, dataMov);
        }

        private void AtualizaDataContaPaga(GDASession sessao, uint idMovBanco, DateTime dataMov)
        {
            AtualizaDataContaPaga(sessao, idMovBanco, dataMov, true);
        }

        /// <summary>
        /// Atualiza a data da conta paga vinculada a movimentação
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="dataMov"></param>
        private void AtualizaDataContaPaga(GDASession sessao, uint idMovBanco, DateTime dataMov, bool bloquearReferenciasMultiplas)
        {
            var idPagto = ObtemValorCampo<uint?>(sessao, "IdPagto", "IdMovBanco = " + idMovBanco);

            if (idPagto.GetValueOrDefault(0) > 0)
            {
                /* Chamado 52861.
                 * Caso o pagamento possua mais de uma movimentação, mas seja uma de entrada e outra de saída e o pagamento esteja cancelado, permite a alteração da movimentação. */
                // Verifica se o pagamento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var quantidadesPorTipoMov = ExecuteMultipleScalar<int>(sessao, string.Format("SELECT COUNT(*) FROM mov_banco WHERE IdPagto={0} GROUP BY TipoMov", idPagto));
                if (quantidadesPorTipoMov.Count > 1)
                {
                    if (PagtoDAO.Instance.ObtemSituacao(sessao, idPagto.Value) == (int)Pagto.SituacaoPagto.Finalizado || quantidadesPorTipoMov.Any(f => f > 1))
                        throw new Exception("Não é possivel mover essa movimentação, pois o pagamento: " + idPagto + " gerou mais de uma movimentação.");
                }

                //Verifica se houve mais de uma forma de pagamento, se sim não pode mover.
                var pagtos = PagtoPagtoDAO.Instance.GetByPagto(sessao, idPagto.Value);
                if (pagtos.Count > 1)
                    throw new Exception("Não é possivel mover essa movimentação, pois o pagamento: " + idPagto + " possui mais de uma forma de pagamento.");
                
                /* Chamado 52861.
                 * Caso a movimentação de estorno esteja sendo alterada, não altera a data de pagamento da conta paga. */
                if (ObtemTipoMov(sessao, idMovBanco) == 2)
                    objPersistence.ExecuteCommand(sessao, "UPDATE contas_pagar SET dataPagto = ?dt WHERE paga = 1 AND IdPagto=" + idPagto, new GDAParameter("?dt", dataMov));
            }
        }

        private void AtualizaDataContaRec(uint idMovBanco, DateTime dataMov)
        {
            AtualizaDataContaRec(null, idMovBanco, dataMov);
        }

        /// <summary>
        /// Atualiza a data da conta recebida vinculada a movimentação
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="dataMov"></param>
        private void AtualizaDataContaRec(GDASession sessao, uint idMovBanco, DateTime dataMov)
        {
            AtualizaDataContaRec(sessao, idMovBanco, dataMov, true);
        }

        /// <summary>
        /// Atualiza a data da conta recebida vinculada a movimentação
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="dataMov"></param>
        private void AtualizaDataContaRec(GDASession sessao, uint idMovBanco, DateTime dataMov, bool bloquearReferenciasMultiplas)
        {
            var movBanco = GetElementByPrimaryKey(sessao, idMovBanco);

            // Mensagem a ser exibida caso não possa efetuar a operação
            var msgImpedimento = "Não é possível mover essa movimentação, pois o recebimento gerou mais de uma movimentação. É necessário refazer o recebimento/pagamento informando a data correta";

            #region Conta Receber

            if (movBanco.IdContaR.GetValueOrDefault(0) > 0)
            {
                var formasPagto = PagtoContasReceberDAO.Instance.ObtemPagtos(sessao, movBanco.IdContaR.Value);

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Chamado 60630
                ////Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                //var dataCadMovBanco = ExecuteMultipleScalar<DateTime>(sessao, "SELECT dataCad FROM mov_banco WHERE idContaR = " + movBanco.IdContaR + " ORDER BY dataCad");
                //if (dataCadMovBanco.Count > 1)
                //    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                //    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                //    for (int i = 1; i < dataCadMovBanco.Count; i++)
                //        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                //            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand(sessao, "UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND idContaR=" + movBanco.IdContaR,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

            #region Pedido

            else if (movBanco.IdPedido.GetValueOrDefault(0) > 0 && movBanco.IdContaR.GetValueOrDefault(0) == 0)
            {
                var contasR = ContasReceberDAO.Instance.GetByPedido(null, movBanco.IdPedido.Value, false,true);
                var formasPagto = new List<PagtoContasReceber>();

                foreach (var c in contasR)
                    formasPagto.AddRange(PagtoContasReceberDAO.Instance.ObtemPagtos(c.IdContaR));

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var dataCadMovBanco = ExecuteMultipleScalar<DateTime>("SELECT dataCad FROM mov_banco WHERE IdPedido = " + movBanco.IdPedido.Value + " ORDER BY dataCad");
                if (dataCadMovBanco.Count > 1)
                    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                    for (int i = 1; i < dataCadMovBanco.Count; i++)
                        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand("UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND IdPedido=" + movBanco.IdPedido.Value,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

            #region Acerto

            else if (movBanco.IdAcerto.GetValueOrDefault(0) > 0)
            {
                var formasPagto = PagtoAcertoDAO.Instance.GetByAcerto(movBanco.IdAcerto.Value);

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var dataCadMovBanco = ExecuteMultipleScalar<DateTime>("SELECT dataCad FROM mov_banco WHERE idAcerto = " + movBanco.IdAcerto + " ORDER BY dataCad");
                if (dataCadMovBanco.Count > 1)
                    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                    for (int i = 1; i < dataCadMovBanco.Count; i++)
                        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand("UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND IdAcerto=" + movBanco.IdAcerto,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

            #region Liberação

            else if (movBanco.IdLiberarPedido.GetValueOrDefault(0) > 0 && movBanco.IdContaR.GetValueOrDefault(0) == 0)
            {
                var contasR = ContasReceberDAO.Instance.GetByLiberacaoPedido(movBanco.IdLiberarPedido.Value, false);
                var formasPagto = new List<PagtoContasReceber>();

                foreach (var c in contasR)
                    formasPagto.AddRange(PagtoContasReceberDAO.Instance.ObtemPagtos(c.IdContaR));

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var dataCadMovBanco = ExecuteMultipleScalar<DateTime>("SELECT dataCad FROM mov_banco WHERE IdLiberarPedido = " + movBanco.IdLiberarPedido.Value + " ORDER BY dataCad");
                if (dataCadMovBanco.Count > 1)
                    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                    for (int i = 1; i < dataCadMovBanco.Count; i++)
                        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand("UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND IdLiberarPedido=" + movBanco.IdLiberarPedido.Value,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

            #region Obra

            else if (movBanco.IdObra.GetValueOrDefault(0) > 0)
            {
                var formasPagto = PagtoObraDAO.Instance.GetByObra(movBanco.IdObra.Value);

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var dataCadMovBanco = ExecuteMultipleScalar<DateTime>("SELECT dataCad FROM mov_banco WHERE IdObra = " + movBanco.IdObra.Value + " ORDER BY dataCad");
                if (dataCadMovBanco.Count > 1)
                    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                    for (int i = 1; i < dataCadMovBanco.Count; i++)
                        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand("UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND IdObra=" + movBanco.IdObra.Value,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

            #region Sinal

            else if (movBanco.IdSinal.GetValueOrDefault(0) > 0)
            {
                var formasPagto = PagtoSinalDAO.Instance.GetBySinal(movBanco.IdSinal.Value);

                if (formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() == "BOLETO" || f.DescrFormaPagto.Trim().ToUpper() == "DEPÓSITO").Count() > 0 &&
                    formasPagto.Where(f => f.DescrFormaPagto.Trim().ToUpper() != "BOLETO" && f.DescrFormaPagto.Trim().ToUpper() != "DEPÓSITO").Count() > 0)
                    throw new Exception("Essa movimentação não pode ser alterada, pois possui mais de uma forma de pagamento.");

                //Verifica se o recebimento gerou mais de uma movimentação no banco, se gerou não pode mover.
                var dataCadMovBanco = ExecuteMultipleScalar<DateTime>("SELECT dataCad FROM mov_banco WHERE IdSinal = " + movBanco.IdSinal.Value + " ORDER BY dataCad");
                if (dataCadMovBanco.Count > 1 && bloquearReferenciasMultiplas)
                    // Se tiver mais de uma movimentação no banco, verifica se alguma movimentação tem a mesma dataCad.
                    // Se a movimentação de menor dataCad for igual a outra ou for menor em 5 segundos, significa que tem mais de uma movimentação para a mesma operação (Recebimento ou Estorno).
                    for (int i = 1; i < dataCadMovBanco.Count; i++)
                        if ((dataCadMovBanco[0] == dataCadMovBanco[i] || dataCadMovBanco[0].AddSeconds(5) > dataCadMovBanco[1]))
                            throw new Exception(msgImpedimento);

                objPersistence.ExecuteCommand("UPDATE contas_receber SET dataRec = ?dt WHERE recebida = 1 AND IdSinal=" + movBanco.IdSinal.Value,
                    new GDAParameter("?dt", dataMov));
            }

            #endregion

        }

        private uint TrocaIdMovimentacao(GDASession sessao, uint idMovBanco, uint idMovBancoAdjacente)
        {
            var dataOriginal = ObtemValorCampo<DateTime?>(sessao, "dataOriginal", "idMovBanco=" + idMovBanco);
            var dataOriginalAdjacente = ObtemValorCampo<DateTime?>(sessao, "dataOriginal", "idMovBanco=" + idMovBancoAdjacente);

            objPersistence.ExecuteCommand(sessao, "Update mov_banco Set idMovBanco=0, dataOriginal=?data Where idMovBanco=" + idMovBanco, new GDAParameter("?data", dataOriginalAdjacente));
            objPersistence.ExecuteCommand(sessao, "Update mov_banco Set idMovBanco=" + idMovBanco + ", dataOriginal=?data Where idMovBanco=" + idMovBancoAdjacente, new GDAParameter("?data", dataOriginal));
            objPersistence.ExecuteCommand(sessao, "Update mov_banco Set idMovBanco=" + idMovBancoAdjacente + " Where idMovBanco=0;");

            return idMovBancoAdjacente;
        }

        #endregion

        #region Verifica a existência de movimentações

        /// <summary>
        /// Existe movimentação por um campo/id?
        /// </summary>
        /// <param name="nomeCampo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsByCampo(string nomeCampo, uint id)
        {
            try
            {
                return objPersistence.ExecuteSqlQueryCount("select count(*) from mov_banco where " + nomeCampo + "=" + id +
                    " and idConta not in (" + UtilsPlanoConta.ListaEstornosAPrazo() + "," + UtilsPlanoConta.ListaEstornosAVista() + "," +
                    UtilsPlanoConta.ListaEstornosSinalPedido() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")") > 0;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Existe movimentação por pedido?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsByPedido(uint idPedido)
        {
            return ExistsByCampo("idPedido", idPedido);
        }

        /// <summary>
        /// Existe movimentação por troca?
        /// </summary>
        /// <param name="idTrocaDev"></param>
        /// <returns></returns>
        public bool ExistsByTroca(uint idTrocaDev)
        {
            return ExistsByCampo("idTrocaDev", idTrocaDev);
        }

        /// <summary>
        /// Existe movimentação por liberação?
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool ExistsByLiberacao(uint idLiberarPedido)
        {
            return ExistsByCampo("idLiberarPedido", idLiberarPedido);
        }

        #endregion

        #region Atualiza Movimentação

        /// <summary>
        /// Atualiza observação de uma movimentação
        /// </summary>
        /// <param name="movBanco"></param>
        public void AtualizaObs(MovBanco movBanco)
        {
            objPersistence.ExecuteCommand("Update mov_banco Set obs=?obs Where idMovBanco=" + movBanco.IdMovBanco, 
                new GDAParameter("?obs", movBanco.Obs));
        }

        /// <summary>
        /// Atualiza Data e Observação uma movimentação
        /// </summary>
        /// <param name="movBanco"></param>
        public void AtualizaDataObs(MovBanco movBanco)
        {
            AtualizaDataObs(null, movBanco, true);
        }

        /// <summary>
        /// Atualiza Data e Observação uma movimentação
        /// </summary>
        /// <param name="movBanco"></param>
        public void AtualizaDataObs(GDASession session, MovBanco movBanco, bool bloquearReferenciasMultiplas)
        {
            var mov = GetElementByPrimaryKey(session, movBanco.IdMovBanco);

            // Verifica a conciliação bancária
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, mov.IdContaBanco, mov.DataMov);

            var novaDataMov = Conversoes.StrParaDate(movBanco.DataMovString.GetValueOrDefault().Date.ToString("dd/MM/yyyy") + " " +
                mov.DataMovString.GetValueOrDefault().Hour.ToString() + ":" + mov.DataMovString.GetValueOrDefault().Minute.ToString() + ":" +
                mov.DataMovString.GetValueOrDefault().Second.ToString());



            AtualizaDataObsIndividual(session, movBanco.IdMovBanco, novaDataMov.Value, mov.DataMov, movBanco.Obs, bloquearReferenciasMultiplas);

            var lstMov = ObterIdsMovBancoGrupo(session, movBanco.IdMovBanco);

            foreach (var mb in lstMov)
            {
                var obs = ObtemValorCampo<string>(session, "Obs", "IdMovBanco = " + mb);
                AtualizaDataObsIndividual(session, mb, novaDataMov.Value, mov.DataMov, obs, bloquearReferenciasMultiplas);
            }

        }

        private void AtualizaDataObsIndividual(GDASession session, uint idMovBanco, DateTime novaDataMov, DateTime dataMovOriginal, string obs, bool bloquearReferenciasMultiplas)
        {
            //Se ao mover as movimentações tiverem em dias diferentes, atualiza a data do pagamento ou recebimento
            // da conta de acordo com a data da movimentação
            if (novaDataMov.Date != dataMovOriginal.Date)
            {
                AtualizaDataContaPaga(session, idMovBanco, novaDataMov, bloquearReferenciasMultiplas);
                AtualizaDataContaRec(session, idMovBanco, novaDataMov, bloquearReferenciasMultiplas);
            }

            var movAnteriorAntesAtualizar = ObtemMovAnterior(idMovBanco);
            var idMovAnteriorAntesAtualizar = movAnteriorAntesAtualizar != null ? movAnteriorAntesAtualizar.IdMovBanco : 0;

            objPersistence.ExecuteCommand(session, "Update mov_banco Set obs=?obs, datamov=?dataMov Where idMovBanco=" + idMovBanco,
                new GDAParameter("?obs", obs), new GDAParameter("?dataMov", novaDataMov));

            var movAnteriorDepoisAtualizar = ObtemMovAnterior(idMovBanco);
            var idMovAnteriorDepoisAtualizar = movAnteriorDepoisAtualizar != null ? movAnteriorDepoisAtualizar.IdMovBanco : 0;

            //Alterando para data anterior
            if (novaDataMov < dataMovOriginal)
            {
                if (idMovAnteriorDepoisAtualizar > 0)
                    CorrigeSaldo(session, idMovBanco, idMovAnteriorDepoisAtualizar);
                else
                    CorrigeSaldo(session, idMovAnteriorDepoisAtualizar, idMovAnteriorDepoisAtualizar);
            }
            else
            {
                if (idMovAnteriorAntesAtualizar > 0)
                    CorrigeSaldo(session, idMovAnteriorAntesAtualizar, idMovBanco);
                else
                    CorrigeSaldo(session, idMovBanco, idMovBanco);
            }
        }

        #endregion

        #region Obtém valor de campos

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public uint ObtemIdContaBanco(uint idMovBanco)
        {
            return ObtemIdContaBanco(null, idMovBanco);
        }

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public uint ObtemIdContaBanco(GDASession sessao, uint idMovBanco)
        {
            return ObtemValorCampo<uint>(sessao, "idContaBanco", "idMovBanco=" + idMovBanco);
        }

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public DateTime ObtemDataMov(uint idMovBanco)
        {
            return ObtemDataMov(null, idMovBanco);
        }

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public DateTime ObtemDataMov(GDASession sessao, uint idMovBanco)
        {
            return ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovBanco=" + idMovBanco);
        }
 
        /// <summary>
        /// Obtém o tipo da movimentação.
        /// </summary>
        public int ObtemTipoMov(GDASession sessao, uint idMovBanco)
        {
            return ObtemValorCampo<int>(sessao, "TipoMov", string.Format("idMovBanco={0}", idMovBanco));
        }

        /// <summary>
        /// Busca o numero do grupo de movimentação da movimentação passada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public int ObterNumGrupo(GDASession sessao, uint idMovBanco)
        {
            return ObtemValorCampo<int>(sessao, "NumGrupo", string.Format("idMovBanco={0}", idMovBanco));
        }

        /// <summary>
        /// Busca o numero do grupo de movimentação da movimentação passada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="mov"></param>
        /// <returns></returns>
        public int ObterNumGrupo(GDASession sessao, MovBanco mov)
        {
            var sql = @"
                SELECT 
                    COALESCE((
		                    SELECT MAX(NUMGRUPO) 
		                    FROM mov_banco 
		                    WHERE ABS(UNIX_TIMESTAMP(DATACAD) - UNIX_TIMESTAMP(?dataCad)) <= 5 AND
                            (
			                    IDACERTO = ?idAcerto OR
                                IDACERTOCHEQUE = ?idAcertoCheque OR
                                IDANTECIPCONTAREC = ?idAntecipContaRec OR
			                    IDANTECIPFORNEC = ?idAntecipFornec OR
			                    IDARQUIVOREMESSA = ?idArquivoRemessa OR
			                    IDCARTAONAOIDENTIFICADO = ?idCartaoNaoIdentificado OR
			                    IDCHEQUE = ?idCheque OR
			                    IDCONTAPG = ?idContaPg OR
			                    IDCONTAR = ?idContaR OR
			                    IDCREDITOFORNECEDOR = ?idCreditoFornecedor OR
			                    IDDEPOSITO = ?idDeposito OR
			                    IDDEPOSITONAOIDENTIFICADO = ?idDepositoNaoIdentificado OR
			                    IDDEVOLUCAOPAGTO = ?idDevolucaoPagto OR
			                    IDLIBERARPEDIDO = ?idLiberarPedido OR
			                    IDOBRA = ?idObra OR
			                    IDPAGTO = ?idPagto OR
			                    IDPEDIDO = ?idPedido OR
			                    IDSINAL = ?idSinal OR
			                    IDSINALCOMPRA = ?idSinalCompra OR
			                    IDTROCADEVOLUCAO = ?idTrocaDevolucao
                            )
                        ), 
                        (SELECT MAX(NUMGRUPO) FROM mov_banco) + 1, 1)";

            var gdaParams = new List<GDAParameter>();

            gdaParams.Add(new GDAParameter("?dataCad", DateTime.Now));
            gdaParams.Add(new GDAParameter("?idAcerto", mov.IdAcerto));
            gdaParams.Add(new GDAParameter("?idAcertoCheque", mov.IdAcertoCheque));
            gdaParams.Add(new GDAParameter("?idAntecipContaRec", mov.IdAntecipContaRec));
            gdaParams.Add(new GDAParameter("?idAntecipFornec", mov.IdAntecipFornec));
            gdaParams.Add(new GDAParameter("?idArquivoRemessa", mov.IdArquivoRemessa));
            gdaParams.Add(new GDAParameter("?idCartaoNaoIdentificado", mov.IdCartaoNaoIdentificado));
            gdaParams.Add(new GDAParameter("?idCheque", mov.IdCheque));
            gdaParams.Add(new GDAParameter("?idContaPg", mov.IdContaPg));
            gdaParams.Add(new GDAParameter("?idContaR", mov.IdContaR));
            gdaParams.Add(new GDAParameter("?idCreditoFornecedor", mov.IdCreditoFornecedor));
            gdaParams.Add(new GDAParameter("?idDeposito", mov.IdDeposito));
            gdaParams.Add(new GDAParameter("?idDepositoNaoIdentificado", mov.IdDepositoNaoIdentificado));
            gdaParams.Add(new GDAParameter("?idDevolucaoPagto", mov.IdDevolucaoPagto));
            gdaParams.Add(new GDAParameter("?idLiberarPedido", mov.IdLiberarPedido));
            gdaParams.Add(new GDAParameter("?idObra", mov.IdObra));
            gdaParams.Add(new GDAParameter("?idPagto", mov.IdPagto));
            gdaParams.Add(new GDAParameter("?idPedido", mov.IdPedido));
            gdaParams.Add(new GDAParameter("?idSinal", mov.IdSinal));
            gdaParams.Add(new GDAParameter("?idSinalCompra", mov.IdSinalCompra));
            gdaParams.Add(new GDAParameter("?idTrocaDevolucao", mov.IdTrocaDevolucao));

            return ExecuteScalar<int>(sessao, sql, gdaParams.ToArray());
        }

        /// <summary>
        /// Obtem o identificador da movimentação antirior ou posteior a informada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="idMovBanco"></param>
        /// <param name="dataMov"></param>
        /// <param name="sentido"></param>
        /// <returns></returns>
        public uint ObterIdMovBanco(GDASession sessao, uint idContaBanco, uint idMovBanco, int numGrupo, DateTime dataMov, int sentido)
        {
            var sql = @"
                SELECT mb.IdMovBanco
                FROM mov_banco mb 
                WHERE mb.dataMov {0}= ?dataMov 
                    AND idContaBanco = ?idContaBanco
                    AND IF(mb.dataMov = ?dataMov, mb.IDMOVBANCO {0} ?idMovBanco, 1)
                    AND mb.NumGrupo <> ?numGrupo
                 Order By DATE_FORMAT(mb.dataMov, '%Y-%m-%d %H%i') {1}, IdMovBanco {1} LIMIT 1";

            return ExecuteScalar<uint>(sessao, string.Format(sql, sentido == 1 ? ">" : "<", sentido == 1 ? "ASC" : "DESC"),
                new GDAParameter("?idContaBanco", idContaBanco),
                new GDAParameter("?idMovBanco", idMovBanco),
                new GDAParameter("?numGrupo", numGrupo),
                new GDAParameter("?dataMov", dataMov));
        }

        /// <summary>
        /// Busca as movimentações de um grupo ordenadas
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMovBanco"></param>
        /// <param name="sentido"></param>
        /// <returns></returns>
        public List<uint> ObterIdsMovBancoGrupo(GDASession sessao, uint idMovBanco, int sentido)
        {
            var numGrupo = ObterNumGrupo(sessao, idMovBanco);

            var sql = @"
                SELECT mb.IdMovBanco 
                FROM mov_banco mb 
                WHERE mb.NumGrupo = ?numGrupo
                ORDER BY DATE_FORMAT(mb.dataMov, '%Y-%m-%d %H%i') {0}, IdMovBanco {0}";

            return ExecuteMultipleScalar<uint>(sessao, string.Format(sql, sentido == 1 ? "ASC" : "DESC"), new GDAParameter("?numGrupo", numGrupo));
        }

        public List<uint> ObterIdsMovBancoGrupo(GDASession sessao, uint idMovBanco)
        {
            var numGrupo = ObterNumGrupo(sessao, idMovBanco);

            var sql = @"
                SELECT mb.IdMovBanco 
                FROM mov_banco mb 
                WHERE mb.NumGrupo = ?numGrupo AND idMovBanco <>" + idMovBanco;

            return ExecuteMultipleScalar<uint>(sessao, sql, new GDAParameter("?numGrupo", numGrupo));
        }

        public int ObterQtdeMovimentacoesAdjacentes(GDASession sessao, uint idContaBanco, DateTime dataMov, uint idMovBanco, int sentido)
        {
            var numGrupo = ObterNumGrupo(sessao, idMovBanco);
            var idMovBancoAdjacente = ObterIdMovBanco(sessao, idContaBanco, idMovBanco, numGrupo, dataMov, sentido);

            var dataMovAdjacente = ObtemDataMov(sessao, idMovBancoAdjacente);

            if (dataMov.ToString("dd/MM/yyyy") != dataMovAdjacente.ToString("dd/MM/yyyy"))
                return 1;

            var numGrupoAdjacente = ObterNumGrupo(sessao, idMovBancoAdjacente);

            return objPersistence.ExecuteSqlQueryCount(sessao, "SELECT COUNT(*) FROM mov_banco WHERE NumGrupo = " + numGrupoAdjacente);
        }

        #endregion

        #region Obtém a movimentação anterior

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public MovBanco ObtemMovAnterior(uint idMovBanco)
        {
            return ObtemMovAnterior(null, idMovBanco);
        }

        /// <summary>
        /// Obtém a movimentação anterior à passada por parâmetro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idMovBanco"></param>
        /// <returns></returns>
        public MovBanco ObtemMovAnterior(GDASession sessao, uint idMovBanco)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from mov_banco where idMovBanco=" + idMovBanco) == 0)
                return null;

            var idContabanco = ObtemIdContaBanco(sessao, idMovBanco);
            var dataMov = ObtemDataMov(sessao, idMovBanco);

            uint? idMovBancoAnt = ExecuteScalar<uint?>(sessao, "Select idMovBanco From mov_banco" +
                " Where idContaBanco=" + idContabanco + " And dataMov<?dataMov And idMovBanco<>" + idMovBanco +
                " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') desc, IdMovBanco desc limit 1",
                new GDAParameter("?dataMov", dataMov));

            if (idMovBancoAnt.GetValueOrDefault(0) == 0)
                idMovBancoAnt = ExecuteScalar<uint?>(sessao, "Select idMovBanco From mov_banco" +
                " Where idContaBanco=" + idContabanco +
                " Order By DATE_FORMAT(dataMov, '%Y-%m-%d %H%i') ASC, IdMovBanco ASC limit 1",
                new GDAParameter("?dataMov", dataMov));

            return idMovBancoAnt > 0 ? GetElementByPrimaryKey(sessao, idMovBancoAnt.Value) : null;
        }

        #endregion

        #region Busca as movimentações para a conciliação bancária

        private string SqlConciliacao(uint idContaBanco, string dataFim, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.AppendFormat(selecionar ? "m.*, c.dataUltimaConciliacao" : "count(*)");

            sql.Append(@"
                from mov_banco m
                    left join (
                        select idContaBanco, max(dataConciliada) as dataUltimaConciliacao
                        from conciliacao_bancaria
                        where situacao=" + (int)ConciliacaoBancaria.SituacaoEnum.Ativa + @"
                        group by idContaBanco
                    ) c on (m.idContaBanco=c.idContaBanco)
                where date(m.dataMov)>date(coalesce(c.dataUltimaConciliacao, '0001-01-01'))");

            if (idContaBanco > 0)
                sql.AppendFormat(" and m.idContaBanco={0}", idContaBanco);

            if (!String.IsNullOrEmpty(dataFim))
                sql.Append(" and date(m.dataMov)<=date(?dtFim)");

            sql.Append(@"
                order by m.dataMov asc, m.idMovBanco asc");

            return sql.ToString();
        }

        public IList<MovBanco> ObtemMovimentacoesParaConciliacao(uint idContaBanco, string dataFim)
        {
            return objPersistence.LoadData(SqlConciliacao(idContaBanco, dataFim, true),
                GetParams(null, dataFim)).ToList();
        }

        public MovBanco[] ObtemMovimentacoesDaConciliacao(uint idConciliacaoBancaria)
        {
            using (var cb = ConciliacaoBancariaDAO.Instance)
            {
                var datas = cb.ObtemDatasConciliacao(idConciliacaoBancaria);
                uint idContaBanco = cb.ObtemContaBanco(idConciliacaoBancaria);

                return GetMovimentacoes(idContaBanco, datas.Key != null ?
                    datas.Key.Value.ToString("dd/MM/yyyy") : null,
                    datas.Value.ToString("dd/MM/yyyy"), 0, 0, 0, false);
            }
        }

        #endregion

        #region Apaga uma movimentação

        /// <summary>
        /// Apaga uma movimentação bancária, corrigindo o saldo.
        /// </summary>
        /// <param name="idMovBanco"></param>
        internal void ApagaMovimentacaoCorrigindoSaldo(GDASession sessao, uint idMovBanco)
        {
            // Verifica a conciliação bancária
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(sessao, idMovBanco);

            MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(sessao, idMovBanco);

            // Corrige saldo
            objPersistence.ExecuteCommand(sessao, "Update mov_banco Set valorMov=0 Where idMovBanco=" + idMovBanco);
            if (movAnterior != null)
                MovBancoDAO.Instance.CorrigeSaldo(sessao, movAnterior.IdMovBanco, idMovBanco);

            // Exclui movimentações geradas
            objPersistence.ExecuteCommand(sessao, "Delete From mov_banco Where idMovBanco=" + idMovBanco);
        }

        #endregion

        #region Transfere valor para Cx. Geral

        /// <summary>
        /// Efetua transferência de valor da conta bancária para o caixa geral.
        /// </summary>
        public void TransferirCxGeral(int idContaBanco, decimal valor, DateTime dataMov, int formaSaida, string obs)
        {
            lock (_transferirContaBancariaParaCaixaGeralLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancariaParaCxGeralDinheiro);

                        if (ExisteMovimentacaoRecente(transaction, (int)UserInfo.GetUserInfo.IdLoja, (int)idConta, valor, obs))
                            throw new Exception("Foi feita uma movimentação idêntica nos últimos segundos, verifique se a operação foi efetuada no caixa diário.");

                        // Debita valor da conta bancária.
                        ContaBancoDAO.Instance.MovContaSaida(transaction, (uint)idContaBanco, idConta, (int)UserInfo.GetUserInfo.IdLoja, formaSaida, valor, dataMov, obs);

                        // Credita valor no caixa geral.
                        CaixaGeralDAO.Instance.MovCxContaBanco(transaction, (uint)idContaBanco, idConta, formaSaida == 1 ? 2 : 1, valor, 0, obs, true, dataMov);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("TransferirCxGeral - Valor {0} - FormaSaida {1} - Obs {2} - Funcionario {3}",
                            valor, formaSaida, obs, UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0), ex);

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se existe alguma movimentação feita com o valor passado nos últimos segundos
        /// </summary>
        public bool ExisteMovimentacaoRecente(GDASession session, int idLoja, int idConta, decimal valor, string obs)
        {
            return ExecuteScalar<bool>(session, @"SELECT COUNT(*)>0 FROM mov_banco
                WHERE ValorMov=?valor AND DataCad>=?dataCad AND IdLoja=?idLoja AND IdConta=?idConta AND Obs=?obs",
                new GDAParameter("?valor", valor), new GDAParameter("?dataCad", DateTime.Now.AddSeconds(-30)),
                new GDAParameter("?idConta", idConta), new GDAParameter("?idLoja", idLoja), new GDAParameter("?obs", obs));
        }

        #endregion

        #region Métodos sobrescritos

        private string GetWhere(object item)
        {
            return item == null ? " is null" : "=" + item.ToString();
        }

        public override uint Insert(MovBanco objInsert)
        {
            return InserirMovBancoComTransacao(objInsert);
        }

        public override uint Insert(GDASession sessao, MovBanco objInsert)
        {
            if (sessao == null)
                return InserirMovBancoComTransacao(objInsert);

            return InserirMovBanco(sessao, objInsert);
        }

        private uint InserirMovBancoComTransacao(MovBanco objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    
                    var id = InserirMovBanco(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        private uint InserirMovBanco(GDASession sessao, MovBanco objInsert)
        {
            lock (_inserirMovimentacaoLock)
            {
                // Verifica a conciliação bancária
                ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(objInsert.IdContaBanco, objInsert.DataMov);

                objInsert.DataMov = DateTime.Parse(objInsert.DataMov.ToString("dd/MM/yyyy 23:00"));
                objInsert.NumGrupo = ObterNumGrupo(sessao, objInsert);

                uint idMovBanco = base.Insert(sessao, objInsert);

                MovBanco movAnterior = ObtemMovAnterior(idMovBanco);
                decimal saldoAnterior = movAnterior != null ? movAnterior.Saldo : 0;

                // Corrige o saldo desta movimentação que acaba de ser inserida
                objPersistence.ExecuteCommand(sessao, @"Update mov_banco Set Saldo=" + saldoAnterior.ToString().Replace(',', '.') +
                    (objInsert.TipoMov == 1 ? "+" : "-") + objInsert.ValorMov.ToString().Replace(',', '.') +
                    " Where idMovBanco=" + idMovBanco,
                    new GDAParameter("?dataMov", DateTime.Parse(objInsert.DataMov.ToString("dd/MM/yyyy 23:00"))));

                CorrigeSaldo(sessao, idMovBanco, idMovBanco);

                return idMovBanco;
            }
        }

        public override int Update(MovBanco objUpdate)
        {
            // Verifica a conciliação bancária
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(objUpdate.IdMovBanco);

            return base.Update(objUpdate);
        }

        public override int Delete(MovBanco objDelete)
        {
            return Cancelar(objDelete.IdMovBanco, null, true);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            return Cancelar(sessao, Key, null, false);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return Cancelar(Key, null, false);
        }

        /// <summary>
        /// Cancela uma movimentação bancária.
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        /// <returns></returns>
        public int Cancelar(uint id, string motivo, bool manual)
        {
            return Cancelar(null, id, motivo, manual);
        }

        /// <summary>
        /// Cancela uma movimentação bancária.
        /// </summary>
        /// <param name="idMovBanco"></param>
        /// <param name="motivo"></param>
        /// <param name="manual"></param>
        /// <returns></returns>
        public int Cancelar(GDASession sessao, uint id, string motivo, bool manual)
        {
            MovBanco objDelete = GetElementByPrimaryKey(sessao, id);
            
            // Verifica a conciliação bancária
            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(sessao, objDelete.IdContaBanco, objDelete.DataMov);

            LogCancelamentoDAO.Instance.LogMovimentacaoBancaria(sessao, objDelete, motivo, manual);

            // Zera esta movimentação para calcular o saldo corretamente
            CurrentPersistenceObject.ExecuteCommand(sessao, "Update mov_banco Set valorMov=0 Where idMovBanco=" + objDelete.IdMovBanco);

            // Se estiver excluindo transferência bancária, exclui a outra também.
            if (objDelete.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfContaBancaria) &&
                objDelete.IdContaBancoDest > 0)
            {
                uint? idMovBanco = ExecuteScalar<uint?>(sessao, "Select idMovBanco From mov_banco Where idMovBanco=" +
                    (objDelete.TipoMov == 1 ? objDelete.IdMovBanco - 1 : objDelete.IdMovBanco + 1) + " And valorMov=" + 
                    objDelete.ValorMov.ToString().Replace(",", ".") + " And idContaBanco=" + objDelete.IdContaBancoDest.Value + 
                    " And idConta=" + objDelete.IdConta + " limit 1");

                if (idMovBanco > 0)
                {
                    LogCancelamentoDAO.Instance.LogMovimentacaoBancaria(sessao, GetElementByPrimaryKey(idMovBanco.Value), "Cancelamento da Movimentação Bancária " + id, manual);

                    // Zera esta movimentação para calcular o saldo corretamente
                    objPersistence.ExecuteCommand(sessao, "Update mov_banco Set valorMov=0 Where idMovBanco=" + idMovBanco);

                    // Corrige o saldo da conta origem/destino antes de excluir
                    CorrigeSaldo(sessao, idMovBanco.Value, objDelete.IdMovBanco);

                    GDAOperations.Delete(sessao, new MovBanco { IdMovBanco = idMovBanco.Value });
                }
            }

            CorrigeSaldo(sessao, objDelete.IdMovBanco, objDelete.IdMovBanco);

            return GDAOperations.Delete(sessao, new MovBanco { IdMovBanco = objDelete.IdMovBanco });
        }

        #endregion
    }
}