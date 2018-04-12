using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MovFuncDAO : BaseDAO<MovFunc, MovFuncDAO>
    {
        //private MovFuncDAO() { }

        #region Movimenta a "conta"

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Faz uma movimentação de pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idConta"></param>
        /// <param name="valorMov"></param>
        public void MovimentarPedido(uint idFunc, uint idPedido, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            Movimentar(null, idFunc, idPedido, null, idConta, tipoMov, valorMov, obs);
        }

        /// <summary>
        /// Faz uma movimentação de pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idConta"></param>
        /// <param name="valorMov"></param>
        public void MovimentarPedido(GDASession sessao, uint idFunc, uint idPedido, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            Movimentar(sessao, idFunc, idPedido, null, idConta, tipoMov, valorMov, obs);
        }

        /// <summary>
        /// Faz uma movimetação de liberação de pedido.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idConta"></param>
        /// <param name="valorMov"></param>
        public void MovimentarLiberacao(uint idFunc, uint idLiberarPedido, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            Movimentar(idFunc, null, idLiberarPedido, idConta, tipoMov, valorMov, obs);
        }

        /// <summary>
        /// Faz uma movimentação no controle.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idConta"></param>
        /// <param name="tipoMov"></param>
        /// <param name="valorMov"></param>
        public void Movimentar(uint idFunc, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            Movimentar(idFunc, null, null, idConta, tipoMov, valorMov, obs);
        }

        /// <summary>
        /// Movimenta a "conta".
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idConta"></param>
        /// <param name="tipoMov">1-Entrada 2-Saída</param>
        /// <param name="valorMov"></param>
        private void Movimentar(uint idFunc, uint? idPedido, uint? idLiberarPedido, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            Movimentar(null, idFunc, idPedido, idLiberarPedido, idConta, tipoMov, valorMov, obs);
        }

        /// <summary>
        /// Movimenta a "conta".
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idConta"></param>
        /// <param name="tipoMov">1-Entrada 2-Saída</param>
        /// <param name="valorMov"></param>
        private void Movimentar(GDASession sessao, uint idFunc, uint? idPedido, uint? idLiberarPedido, uint idConta, int tipoMov, decimal valorMov, string obs)
        {
            MovFunc novo = new MovFunc();
            novo.IdFunc = idFunc;
            novo.IdConta = idConta;
            novo.IdPedido = idPedido;
            novo.IdLiberarPedido = idLiberarPedido;
            novo.TipoMov = tipoMov;
            novo.DataMov = DateTime.Now;
            novo.ValorMov = valorMov;
            novo.Saldo = GetSaldo(idFunc) + (tipoMov == 1 ? valorMov : -valorMov);
            novo.Obs = obs;
            
            Insert(sessao, novo);
        }

        #endregion

        #region Busca padrão

        private string Sql(uint idFunc, uint idPedido, uint idLiberarPedido, int tipoMov, string dataIni, string dataFim, int tipo, bool selecionar)
        {
            string campos = selecionar ? "m.*, f.nome as nomeFunc, p.descricao as descrPlanoConta" : "count(*)";

            string sql = @"
                select " + campos + @"
                from mov_func m
                    left join funcionario f on (m.idFunc=f.idFunc)
                    left join plano_contas p on (m.idConta=p.idConta)
                where 1";

            if (idFunc > 0)
                sql += " and m.idFunc=" + idFunc;

            if (idPedido > 0)
                sql += " and m.idPedido=" + idPedido;

            if (idLiberarPedido > 0)
                sql += " and m.idLiberarPedido=" + idLiberarPedido;

            if (tipoMov > 0)
                sql += " and m.tipoMov=" + tipoMov;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and m.dataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and m.dataMov<=?dataFim";

            if (tipo > 0)
                sql += " AND m.tipoMov = " + tipo;

            return sql + " order by idMovFunc asc";
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.ToArray();
        }

        public MovFunc[] GetMovimentacoes(uint idFunc, uint idPedido, uint idLiberarPedido, string dataIni, string dataFim, int tipo,
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = null;

            MovFunc[] movFuncLista;

            if (idFunc > 0)
            {
                List<MovFunc> lstMov = objPersistence.LoadData(Sql(idFunc, idPedido, idLiberarPedido, 0, dataIni, dataFim, tipo, true), GetParams(dataIni, dataFim));

                for (int i = 0; i < lstMov.Count; i++)
                {
                    if (i == 0 || (lstMov[i].DataMov.ToString("dd/MM/yyyy") != lstMov[i - 1].DataMov.ToString("dd/MM/yyyy")))
                    {
                        MovFunc mov = new MovFunc();
                        mov.Obs = "SALDO ANTERIOR";
                        mov.Saldo = i > 0 ? lstMov[i - 1].Saldo : lstMov[0].Saldo - lstMov[0].ValorMov * (decimal)(lstMov[0].TipoMov == 1 ? 1 : -1);
                        mov.ExibirColunas = false;
                        lstMov.Insert(i, mov);
                        i++;
                    }
                }

                if (lstMov.Count > 0)
                {
                    MovFunc mov = new MovFunc();
                    mov.Obs = "SALDO ATUAL";
                    mov.Saldo = lstMov[lstMov.Count - 1].Saldo;
                    mov.ExibirColunas = false;
                    lstMov.Add(mov);
                }

                movFuncLista = lstMov.ToArray();

            }
            else
            {
                movFuncLista = ((List<MovFunc>)LoadDataWithSortExpression(Sql(idFunc, idPedido, idLiberarPedido, 0, dataIni, dataFim, tipo, true), sortExpression,
                    startRow, pageSize, GetParams(dataIni, dataFim))).ToArray();
            }

            return movFuncLista;
        }

        public int GetCount(uint idFunc, uint idPedido, uint idLiberarPedido, string dataIni, string dataFim, int tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idFunc, idPedido, idLiberarPedido, 0, dataIni, dataFim, tipo, false), GetParams(dataIni, dataFim));
        }

        public IList<MovFunc> GetForRpt(uint idFunc, uint idPedido, uint idLiberarPedido, string dataIni, string dataFim, int tipo)
        {
            return objPersistence.LoadData(Sql(idFunc, idPedido, idLiberarPedido, 0, dataIni, dataFim, tipo, true), GetParams(dataIni, dataFim)).ToList();
        }

        #endregion

        #region Retorna a movimentação pelo idPedido

        /// <summary>
        /// Retorna a movimentação pelo id do pedido e tipo da movimentação
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<MovFunc> GetByPedido(GDASession sessao, uint idPedido)
        {
            var itens =  objPersistence.LoadData(sessao, Sql(0, idPedido, 0, 0, null, null, 0, true)).ToList();

            return itens.OrderByDescending(f => f.DataMov).ThenBy(f => f.IdMovFunc).ToList();
        }

        #endregion

        #region Retorna o saldo

        /// <summary>
        /// Recupera o saldo de um funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public decimal GetSaldo(uint idFunc)
        {
            string sql = "select saldo from mov_func where idFunc=" + idFunc + " order by idMovFunc desc";
            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Retorna a soma dos saldos dos funcionários para o relatório.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public decimal GetSomaSaldos(uint idFunc, uint idPedido, uint idLiberarPedido, string dataIni, string dataFim, int tipo)
        {
            string sql = "select sum(saldo) from (select saldo from (" + Sql(idFunc, idPedido, idLiberarPedido, 0, dataIni, dataFim, tipo, true) +
                ") as temp1 group by idFunc) as temp";

            return ExecuteScalar<decimal>(sql, GetParams(dataIni, dataFim));
        }

        #endregion

        #region Quita o débito de um funcionário

        /// <summary>
        /// Quita o débito de um funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public string Quitar(uint idFunc, decimal[] valores, uint[] formasPagto, uint[] tiposCartao, uint[] parcelasCartao,
            uint[] contasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, uint[] tiposBoleto, decimal[] taxaAntecip, string numAutConstrucard, bool recebimentoParcial,
            bool gerarCredito, string chequesPagto, string obs)
        {
            UtilsFinanceiro.DadosRecebimento retorno = null;

            try
            {
                decimal totalASerPago = Math.Abs(GetSaldo(idFunc));

                decimal totalPago = 0;
                foreach (decimal v in valores)
                    totalPago += v;

                if (!recebimentoParcial && !gerarCredito && totalPago != totalASerPago)
                    throw new Exception("Total pago (" + totalPago.ToString("C") + ") não confere com o total a ser pago (" + totalASerPago.ToString("C") + ").");
                else if (!recebimentoParcial && gerarCredito && totalPago < totalASerPago)
                    throw new Exception("Total pago (" + totalPago.ToString("C") + ") não pode ser menor que o total a ser pago (" + totalASerPago.ToString("C") + ").");
                else if (recebimentoParcial && totalPago > totalASerPago)
                    throw new Exception("Total pago (" + totalPago.ToString("C") + ") não pode ser maior que o total a ser pago (" + totalASerPago.ToString("C") + ").");

                retorno = UtilsFinanceiro.Receber(null, UserInfo.GetUserInfo.IdLoja, null, null, null, null, null, null, null, null, null, null, null, 0, 0, null, null, totalASerPago, totalPago,
                    valores, formasPagto, contasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao, tiposBoleto, taxaAntecip, 0, recebimentoParcial, false, 0,
                    numAutConstrucard, false, parcelasCartao, chequesPagto, false, UtilsFinanceiro.TipoReceb.CreditoValeFuncionario);

                if (retorno.ex != null)
                    throw retorno.ex;

                // Faz as movimentações de saída do controle de vales
                for (int i = 0; i < valores.Length; i++)
                    if (valores[i] > 0 && formasPagto[i] > 0)
                    {
                        uint idConta = UtilsPlanoConta.GetPlanoContaVendaFunc(formasPagto[i], tiposCartao[i]);
                        Movimentar(idFunc, null, null, idConta, 1, valores[i], obs);
                    }

                string textoRetorno = "Ok;Débito quitado";
                if (totalPago < totalASerPago)
                    textoRetorno += " parcialmente. Ainda restam " + (totalASerPago - totalPago).ToString("C") + " a serem quitados.";
                else
                    textoRetorno += ".";

                return textoRetorno;
            }
            catch (Exception ex)
            {
                if (retorno != null)
                {
                    foreach (Cheques c in retorno.lstChequesInseridos)
                        ChequesDAO.Instance.Delete(c);

                    foreach (uint idCxDiario in retorno.idCxDiario)
                        CaixaDiarioDAO.Instance.DeleteByPrimaryKey(idCxDiario);

                    foreach (uint idCxGeral in retorno.idCxGeral)
                        CaixaGeralDAO.Instance.DeleteByPrimaryKey(idCxGeral);

                    foreach (uint idMovBanco in retorno.idMovBanco)
                        MovBancoDAO.Instance.DeleteByPrimaryKey(idMovBanco);

                    foreach (uint idParcCartao in retorno.idParcCartao)
                        ContasReceberDAO.Instance.DeleteByPrimaryKey(idParcCartao);
                }

                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao quitar débito.", ex);
            }
        }

        #endregion

        #region Obtém dados da Movimentação

        /// <summary>
        /// Retorna o id do funcionário.
        /// </summary>
        /// <param name="idMovFunc">Id da movimentação.</param>
        /// <returns></returns>
        public uint ObtemIdFunc(uint idMovFunc)
        {
            return ObtemValorCampo<uint>("idFunc", "idMovFunc=" + idMovFunc);
        }

        /// <summary>
        /// Retorna a data da movimentação.
        /// </summary>
        /// <param name="idMovFunc">Id da movimentação.</param>
        /// <returns></returns>
        public DateTime ObtemDataMov(uint idMovFunc)
        {
            return ObtemValorCampo<DateTime>("DataMov", "idMovFunc=" + idMovFunc);
        }

        #endregion

        #region Deleta a movimentação e corrige o saldo

        /// <summary>
        /// Deleta a movimentação e corrige o saldo.
        /// </summary>
        /// <param name="idMovFunc">Id da movimentação.</param>
        /// <returns></returns>
        public void DeletaMov(uint idMovFunc)
        {
            uint idFunc = ObtemIdFunc(idMovFunc);
            DateTime dataMov = ObtemDataMov(idMovFunc);

            DeleteByPrimaryKey(idMovFunc);

            // O round ao calcular o valor do saldo foi colocado para evitar erros caso o saldo possua 5 na terceira casa decimal.
            string sqlAjuste = @"
                Set @saldo := Round(Coalesce((Select saldo From mov_func
                    Where idFunc=" + idFunc + @"
                        And dataMov<=?dataMov 
                        And idMovFunc<" + idMovFunc + @"
                    Order By dataMov Desc, idMovFunc Desc Limit 1),0),2);

                Update mov_func Set saldo=(@saldo := @saldo + 
                    If(tipoMov=1, valorMov, -valorMov))
                Where idFunc=" + idFunc + @"
                    And dataMov>=?dataMov
                    And idMovFunc>" + idMovFunc + " Order By dataMov Asc, idMovFunc Asc";

            objPersistence.ExecuteCommand(sqlAjuste, new GDAParameter("?dataMov", dataMov));
        }

        #endregion
    }
}