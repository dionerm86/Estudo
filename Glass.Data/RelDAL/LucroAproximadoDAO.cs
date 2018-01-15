using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class LucroAproximadoDAO : Glass.Data.DAL.BaseDAO<LucroAproximado, LucroAproximadoDAO>
    {
        //private LucroAproximadoDAO() { }

        public IList<LucroAproximado> GetLucroAproximado(string dataIni, string dataFim)
        {
            // Sql para buscar gasto com décimo terceiro
            string sqlDecTerc = "Select 'Décimo Terceiro' as descricao, Sum(coalesce(salario, 0) / 12) as Valor From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gasto com férias
            string sqlFerias = "Select 'Férias' as descricao, Sum((coalesce(salario, 0) / 3) / 12) as Valor From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gasto com IPVA
            string sqlIpva = "Select 'IPVA' as descricao, Sum(Coalesce(ValorIpva, 0) / 12) as Valor From veiculo";

            // Sql para buscar gastos com Salários
            string sqlSalarios = "Select 'Salários' as descricao, Sum(coalesce(salario, 0) + coalesce(gratificacao, 0) + coalesce(auxalimentacao, 0)) as Valor " +
                "From funcionario Where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar cheques devolvidos
            string sqlChequesDevolvidos = "Select 'Cheques Devolvidos' as descricao, Sum(valor) as Valor From cheques Where situacao=" + 
                (int)Glass.Data.Model.Cheques.SituacaoCheque.Devolvido + " And idCheque In (" +
                "Select IdCheque From mov_banco where idConta=" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido) + ")";

            // Sql para buscar pagamentos efetuados com cheques próprios
            string sqlPagoChequeProprio = "Select 'Pagtos. Cheques Próprios' as descricao, Sum(valorPagto) as Valor From pagto_pagto pp inner join pagto p on " +
                "(pp.idPagto=p.idPagto) Where p.dataPagto>=?dataIni And p.dataPagto<=?dataFim And idFormaPagto=" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio;

            // Sql para buscar pagamentos efetuados com cheques de terceiros
            string sqlPagoChequeTerc = "Select 'Pagtos. Cheques Terceiros' as descricao, Sum(valorPagto) as Valor From pagto_pagto pp inner join pagto p on " +
                "(pp.idPagto=p.idPagto) Where p.dataPagto>=?dataIni And p.dataPagto<=?dataFim And idFormaPagto=" + (int)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro + "";
            
            // Sql para buscar total gasto com pedidos de reposição
            string sqlPedidoRepos = "Select 'Pedidos Reposição' as descricao, Sum(total) as Valor From pedido Where situacao=" + 
                (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado + " And tipoVenda=" + (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição + 
                " And dataConf>=?dataIni And dataConf<=?dataFim";

            // Sql para executar todos os sqls acima com apenas uma requisição ao banco
            string sqlFinal = @"
                select grupo, descricao, cast(valor as decimal(12,2)) as valor, 'Período: " + dataIni + " a " + dataFim + @"' as criterio
                from (
                    Select 1 as grupo, descricao, valor
                    from (
                        " + SqlTotalCxGeral(1) + @"
                        union " + SqlTotalCxDiario(1, dataIni, dataFim) + @"
                    ) as entradas
                    
                    union select 2 as grupo, descricao, valor
                    from (
                        " + SqlTotalCxGeral(2) + @"
                        union " + SqlTotalCxDiario(2, dataIni, dataFim) + @"
                        union " + sqlPagoChequeProprio + @"
                        union " + sqlPagoChequeTerc + @"
                        union " + sqlPedidoRepos + @"
                        union " + sqlSalarios + @"
                        union " + sqlDecTerc + @"
                        union " + sqlFerias + @"
                        union " + sqlIpva + @"
                        union " + sqlChequesDevolvidos + @"
                    ) as saidas
                ) as lucro_aproximado";

            var lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
            
            return objPersistence.LoadData(sqlFinal, lstParam.ToArray()).ToList();
        }

        public LucroAproximado[] GetList(string dataIni, string dataFim)
        {
            var itens = GetLucroAproximado(dataIni, dataFim);
            var retorno = new List<LucroAproximado>();

            var subtotais = new Dictionary<long, decimal>();

            int inicio = 0;
            for (int i = 0; i < itens.Count; i++)
            {
                retorno.Add(itens[i]);

                if (i < itens.Count - 1 ? itens[i].Grupo != itens[i + 1].Grupo : true)
                {
                    decimal s = 0;
                    for (int j = inicio; j <= i; j++)
                        s += itens[j].Valor;

                    inicio = i + 1;

                    LucroAproximado subtotal = new LucroAproximado();
                    subtotal.IsTotal = true;
                    subtotal.Descricao = itens[i].DescrGrupo.ToUpper();
                    subtotal.Valor = s;
                    retorno.Add(subtotal);

                    subtotais.Add(itens[i].Grupo, s);
                }
            }

            LucroAproximado total = new LucroAproximado();
            total.IsTotal = true;
            total.Descricao = "LUCRO APROXIMADO";
            total.Valor = subtotais[1] - subtotais[2];
            retorno.Add(total);
            
            return retorno.ToArray();
        }

        #region Métodos privados

        private string SqlTotalCxGeral(int tipoMov)
        {
            string contasQueNaoEntram = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral).ToString() + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.DepositoCheque) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCartao) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevCheque) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDeposito) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecChequeDevDinheiro) + "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfBancoCheques) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDepositoCheque);

            return "Select 'Caixa Geral' as descricao, Sum(valorMov) as Valor From caixa_geral Where idConta Not In (" + contasQueNaoEntram + ") And " +
                "tipoMov=" + tipoMov + " And dataMov>=?dataIni And dataMov<=?dataFim " +
                "And (IdPedido Not In (Select IdPedido From pedido Where situacao=" + (int)Glass.Data.Model.Pedido.SituacaoPedido.Cancelado + ") Or idPedido is null)";
        }

        private string SqlTotalCxDiario(int tipoMov, string dataIni, string dataFim)
        {
            return "select concat('Caixa Diário ', l.nomeFantasia) as descricao, valor from (" +
                CaixaDiarioDAO.Instance.SqlValorMovimentacoes("l.idLoja", tipoMov, true) + ") as temp " +
                "left join loja l on (temp.idLoja=l.idLoja)";
        }

        #endregion
    }
}
