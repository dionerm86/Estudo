using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class SugestaoClienteDAO : BaseCadastroDAO<SugestaoCliente, SugestaoClienteDAO>
    {
        //private SugestaoClienteDAO() { }

        #region Busca de Sugestões

        private string Sql(uint idSugestao, uint idCliente, uint IdFunc, string nomeFunc, string nomeCli, string dataIni, string dataFim, int tipo, string descr, string situacao, bool selecionar)
        {
            var descrNomeCliente = Configuracoes.Geral.ExibirRazaoSocialTelaSugestao ?
                "COALESCE(c.Nome, c.nomeFantasia)" : "COALESCE(c.nomeFantasia, c.Nome)";

            string campos = selecionar ? "sc.*, " + descrNomeCliente + " as NomeCliente, f.Nome as NomeFuncionario, '$$$' as Criterio" : "Count(sc.idSugestao)";
            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From sugestao_cliente sc
                           Left Join funcionario f On (sc.usuCad=f.IdFunc)
                           Left Join cliente c On (sc.idCliente=c.id_Cli) 
                           Where 1 ";

            if (idSugestao > 0)
            {
                sql += " And sc.idSugestao=" + idSugestao;
                criterio += "Cód. Sugestão: " + idSugestao + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And sc.idCliente=" + idCliente;
                criterio += "Cód. Cliente: " + idCliente + "    ";
            }

            if (IdFunc > 0)
            {
                sql += " And sc.usuCad=" + IdFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(IdFunc) + "    ";
            }

            if (tipo > 0)
            {
                sql += " And sc.tipo=" + tipo;
            }

            if (!String.IsNullOrEmpty(nomeFunc))
            {
                sql += " And f.Nome like ?nomeFunc";
            }

            if (!String.IsNullOrEmpty(nomeCli))
            {
                sql += " And c.Nome like ?nomeCli";
                criterio += "Cliente: " + nomeCli + "    ";
            }

            if (!String.IsNullOrEmpty(descr))
                sql += " And sc.descricao like ?descr";

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And sc.dataCad>=?dataIni";
                criterio += "Data inicial: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And sc.dataCad<=?dataFim";
                criterio += "Data final: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(situacao))
            {
                sql += " And Cancelada in (" + situacao + ")";

                if (situacao.Split(',').Length > 1)
                {
                    criterio += " Situação: Ativas e Canceladas";
                }
                else
                {
                    if (situacao.Contains("0"))
                        criterio += " Situação: Ativas";
                    if (situacao.Contains("1"))
                        criterio += " Situação: Canceladas";
                }
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<SugestaoCliente> GetList(uint idSugestao, uint idCliente, uint idFunc, string nomeFunc, string nomeCli, string dataIni, string dataFim, int tipo, string descr, string situacao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idSugestao, idCliente, idFunc, nomeFunc, nomeCli, dataIni, dataFim, tipo, descr, situacao, true), sortExpression, startRow, pageSize, GetParam(nomeFunc, nomeCli, descr, dataIni, dataFim));
        }

        public IList<SugestaoCliente> GetListForRpt(uint idSugestao, uint idCliente, uint idFunc, string nomeFunc, string nomeCli, string dataIni, string dataFim, int tipo, string descr, string situacao)
        {
            return objPersistence.LoadData(Sql(idSugestao, idCliente, idFunc, nomeFunc, nomeCli, dataIni, dataFim, tipo, descr, situacao, true), GetParam(nomeFunc, nomeCli, descr, dataIni, dataFim)).ToList();
        }

        public int GetCount(uint idSugestao, uint idCliente, uint idFunc, string nomeFunc, string nomeCli, string dataIni, string dataFim, int tipo, string descr, string situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idSugestao, idCliente, idFunc, nomeFunc, nomeCli, dataIni, dataFim, tipo, descr, situacao, false), GetParam(nomeFunc, nomeCli, descr, dataIni, dataFim));
        }

        private GDAParameter[] GetParam(string nomeFunc, string nomeCli, string descr, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFunc))
                lstParam.Add(new GDAParameter("?nomeFunc", "%" + nomeFunc + "%"));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(descr))
                lstParam.Add(new GDAParameter("?descr", "%" + descr + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        public int Cancelar(int IsSugestao)
        {
            return objPersistence.ExecuteCommand("update sugestao_cliente set cancelada=1 where idsugestao=?id", new GDAParameter("?id", IsSugestao));
        }
    }
}