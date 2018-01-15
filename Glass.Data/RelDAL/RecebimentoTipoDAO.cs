using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.RelDAL
{
    public class RecebimentoTipoDAO : Glass.Data.DAL.BaseDAO<RecebimentoTipo, RecebimentoTipoDAO>
    {
        //private RecebimentoTipoDAO() { }

        public IList<RecebimentoTipo> GetRecebimentoTipo(string dataIni, string dataFim, uint idLoja, uint usucad)
        { 
            string sqlBoleto = SqlReceb("Boleto", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Boleto), idLoja, usucad);
            string sqlCartao = SqlReceb("Cartão", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Cartao), idLoja, usucad);
            string sqlCheque = SqlReceb("Cheque", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio), idLoja, usucad);
            string sqlConstrucard = SqlReceb("Construcard", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard), idLoja, usucad);
            string sqlDeposito = SqlReceb("Depósito", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Deposito), idLoja, usucad);
            string sqlDinheiro = SqlReceb("Dinheiro", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro), idLoja, usucad);
            string sqlCredito = SqlReceb("Crédito", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Credito), idLoja, usucad);
            string sqlPermuta = SqlReceb("Permuta", UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Permuta), idLoja, usucad);
            string sqlMastercardCredito = SqlReceb("Mastercard Crédito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.MasterCredito), idLoja, usucad);
            string sqlMastercardDebito = SqlReceb("Mastercard Débito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.MasterDebito), idLoja, usucad);
            string sqlVisaCredito = SqlReceb("Visa Crédito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.VisaCredito), idLoja, usucad);
            string sqlVisaDebito = SqlReceb("Visa Débito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.VisaDebito), idLoja, usucad);
            string sqlOutrosCredito = SqlReceb("Outros Crédito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.OutrosCredito), idLoja, usucad);
            string sqlOutrosDebito = SqlReceb("Outros Débito", UtilsPlanoConta.ContasTipoCartao(Utils.TipoCartao.OutrosDebito), idLoja, usucad);

            string sqlReceb = @"
                select descricao, cast(valor as decimal(12,2)) as valor, 'Período: " + dataIni + " a " + dataFim + @"' as criterio
                from (
                    " + sqlBoleto + @"
                    union " + sqlCartao + @"
                    union " + sqlCheque + @"
                    union " + sqlConstrucard + @"
                    union " + sqlDeposito + @"
                    union " + sqlDinheiro + @"
                    union " + sqlCredito + @"
                    union " + sqlPermuta + @"
                    /* union " + sqlMastercardCredito + @"
                    union " + sqlMastercardDebito + @"
                    union " + sqlVisaCredito + @"
                    union " + sqlVisaDebito + @"
                    union " + sqlOutrosCredito + @"
                    union " + sqlOutrosDebito + @" */
                ) as recebimento_tipo";

            var lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return objPersistence.LoadData(sqlReceb, lstParam.ToArray()).ToList();
        }

        public RecebimentoTipo[] GetList(string dataIni, string dataFim)
        {
            List<RecebimentoTipo> retorno = new List<RecebimentoTipo>(GetRecebimentoTipo(dataIni, dataFim, 0, 0));

            decimal t = 0;
            foreach (RecebimentoTipo r in retorno)
                t += r.Valor;

            RecebimentoTipo total = new RecebimentoTipo();
            total.IsTotal = true;
            total.Descricao = "TOTAL";
            total.Valor = t;
            retorno.Add(total);

            return retorno.ToArray();
        }

        public RecebimentoTipo[] GetRecebimentosGrafico(string dataIni, string dataFim, uint idLoja, uint usucad)
        {
            List<RecebimentoTipo> retorno = new List<RecebimentoTipo>(GetRecebimentoTipo(dataIni, dataFim, idLoja, usucad));

            int remove = retorno.RemoveAll(delegate(RecebimentoTipo p) { return p.Valor == 0; });

            return retorno.ToArray();
        }

        #region Métodos privados

        private string SqlReceb(string descricao, string idsContas, uint idLoja, uint usucad)
        {
            bool lojaSelecionada = idLoja > 0 ? true : false;
            bool funcSelecionado = usucad > 0 ? true : false;

            string sqlReceb = @"Select '" + descricao + @"' as descricao, Coalesce(Sum(valCxGeral + valCxDiario + valContaBanco - estCxGeral - 
                estCxDiario - estContaBanco), 0) as valor From 

                (Select Sum(cg.valormov + cg.juros) as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_geral cg Where cg.tipoMov=1 And cg.DataMov>=?dataIni And 
                cg.DataMov<=?dataFim And cg.IdConta In ($contas) And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + @") ";

                if (lojaSelecionada)
                    sqlReceb += "and cg.idLoja=" + idLoja + " ";

                if (funcSelecionado)
                    sqlReceb += "and cg.usucad=" + usucad + " ";
                
                sqlReceb += @"
                union all Select 0 as valCxGeral, Sum(cd.valor + cd.juros) as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_diario cd Where cd.tipoMov=1 And cd.DataCad>=?dataIni And 
                cd.DataCad<=?dataFim And cd.IdConta In ($contas) ";

                if (lojaSelecionada)
                    sqlReceb += "and cd.idLoja=" + idLoja + " ";

                if (funcSelecionado)
                    sqlReceb += "and cd.usucad=" + usucad + " ";
                
                sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, Sum(mb.valorMov) as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From mov_banco mb Where mb.tipoMov=1 And mb.DataMov>=?dataIni And 
                mb.DataCad<=?dataFim And mb.IdConta In ($contas) ";

                if (funcSelecionado)
                    sqlReceb += "and mb.usucad=" + usucad + " ";

                sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, Sum(cg.valorMov + cg.juros) as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_geral cg Where cg.tipoMov=2 And cg.DataMov>=?dataIni And 
                cg.DataMov<=?dataFim And cg.IdConta In ($contas) And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + @") ";

                if (lojaSelecionada)
                    sqlReceb += "and cg.idLoja=" + idLoja + " ";

                if (funcSelecionado)
                    sqlReceb += "and cg.usucad=" + usucad + " ";

                sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, Sum(cd.valor + cd.juros) as estCxDiario, 0 as estContaBanco 
                From caixa_diario cd Where cd.tipoMov=2 And cd.DataCad>=?dataIni And 
                cd.DataCad<=?dataFim And cd.IdConta In ($contas) ";

                if (lojaSelecionada)
                    sqlReceb += "and cd.idLoja=" + idLoja + " ";

                if (funcSelecionado)
                    sqlReceb += "and cd.usucad=" + usucad + " ";

                sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, Sum(mb.valorMov) as estContaBanco 
                From mov_banco mb Where mb.tipoMov=2 And mb.DataCad>=?dataIni And 
                mb.DataCad<=?dataFim And mb.IdConta In ($contas) ";

                if (funcSelecionado)
                    sqlReceb += "and mb.usucad=" + usucad + " ";

                sqlReceb += @"
                ) as tbl";

            return sqlReceb.Replace("$contas", idsContas);
        }

        #endregion
    }
}
