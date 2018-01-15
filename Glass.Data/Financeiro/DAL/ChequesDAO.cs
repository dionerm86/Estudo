using System;
using System.Collections.Generic;
using System.Globalization;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ChequesDAO : BaseCadastroDAO<Cheques, ChequesDAO>
    {
        //private ChequesDAO() { }

        #region Busca cheques para tela de cadastros de cheque para: Entrada, Conta a Receber, A Vista, Cheque Devolvido, etc.

        private string Sql(uint idCheque, uint idPedido, uint idAcerto, uint idContaR, uint idChequeDevolvido,
            uint idLiberarPedido, uint idObra,
            int situacao, int origem, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = String.Empty;
            string campos = selecionar
                ? @"c.*, cli.Nome as NomeCliente, coalesce(forn.NomeFantasia, forn.RazaoSocial) as NomeFornecedor, 
                pag.IdPagto"
                : "Count(*)";

            if (selecionar)
                campos +=
                    @", Cast(group_concat(distinct Coalesce(cp.idCompra, '') separator ', ') as Char) as IdsCompra, 
                    Cast(group_concat(distinct Coalesce(cp.idNf, '')) as Char) as IdsNF";

            string sql = @"
                Select " + campos + @" From cheques c
                    Left join cliente cli on (c.idCliente=cli.id_cli) 
                    Left Join pagto_cheque pagc on (pagc.idCheque=c.idCheque) 
                    Left Join pagto pag on (pag.idPagto=pagc.idPagto)
                    Left Join fornecedor forn on (forn.idFornec=pag.idFornec) ";

            if (selecionar)
                sql += "Left Join contas_pagar cp On (cp.idPagto=pag.idPagto) ";

            sql += "Where 1 ?filtroAdicional?";

            if (idCheque > 0)
                filtroAdicional += " And c.idCheque=" + idCheque;

            if (idPedido > 0)
                filtroAdicional += " And (c.IdPedido=" + idPedido +
                                   " Or c.idSinal In (Select idSinal From pedido Where idPedido=" + idPedido + "))";

            if (idAcerto > 0)
                filtroAdicional += " And IdAcerto=" + idAcerto;

            if (idChequeDevolvido > 0)
                filtroAdicional += " And IdChequeDevolvido=" + idChequeDevolvido;

            if (idLiberarPedido > 0)
                filtroAdicional += " And IdLiberarPedido=" + idLiberarPedido;

            if (idObra > 0)
                filtroAdicional += " And idObra=" + idObra;

            if (situacao > 0)
                filtroAdicional += " And Situacao=" + situacao;

            if (origem > 0)
                filtroAdicional += " And Origem=" + origem;

            if (idContaR > 0)
                filtroAdicional += " And IdContaR=" + idContaR;

            if (selecionar)
                sql += " group By c.idCheque";

            return sql;
        }

        /// <summary>
        /// Busca todos os cheques do Pedido
        /// </summary>
        public IList<Cheques> GetByPedido(uint idPedido, uint idContaR, int situacao, int origem)
        {
            string filtroAdicional;
            string sql = Sql(0, idPedido, 0, idContaR, 0, 0, 0, 0, 0, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Cheques> GetList(uint idPedido, uint idAcerto, uint idContaR, uint idChequeDevolvido,
            uint idLiberarPedido, int situacao, int origem, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(0, idPedido, idAcerto, idContaR, idChequeDevolvido, idLiberarPedido, 0, situacao, origem,
                true, out filtroAdicional).
                Replace("?filtroAdicional?", String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional);
        }

        /// <summary>
        /// Conta a quantidade de cheques associados ao pedido passado
        /// </summary>
        public int GetCount(uint idPedido, uint idAcerto, uint idContaR, uint idChequeDevolvido, uint idLiberarPedido,
            int situacao, int origem)
        {
            string filtroAdicional;
            string sql = Sql(0, idPedido, idAcerto, idContaR, idChequeDevolvido, idLiberarPedido, 0, situacao, origem,
                true, out filtroAdicional).
                Replace("?filtroAdicional?", String.Empty);

            return GetCountWithInfoPaging(sql, false, filtroAdicional);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public Cheques GetElement(uint idCheque)
        {
            return GetElement(null, idCheque);
        }

        public Cheques GetElement(GDASession sessao, uint idCheque)
        {
            string filtroAdicional;
            string sql = Sql(idCheque, 0, 0, 0, 0, 0, 0, 0, 0, true, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadOneData(sessao, sql);
        }

        #endregion

        #region Busca cheques para consulta e relatório

        private string SqlFilter(uint idLoja, uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo, int numCheque, string situacao, bool? reapresentado, int advogado, string titular, string agencia, string conta,
            string dataIni, string dataFim, string dataCadIni, string dataCadFim, string cpfCnpj, uint idCli, string nomeCli,
            uint idFornec, string nomeFornec, float valorInicial, float valorFinal, string nomeUsuCad, string idsRotas, string ordenacao,
            bool selecionar, bool caixaDiario, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = String.Empty;

            string campos = selecionar
                ? @"c.*, cli.Nome as NomeCliente, '$$$' as Criterio, forn.idFornec as IdFornecedor, 
                pag.IdPagto, coalesce(forn.NomeFantasia, forn.RazaoSocial) as NomeFornecedor,
                coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja"
                : "Count(*)";

            if (selecionar)
                campos += @", Cast(group_concat(distinct cp.idCompra) as Char) as IdsCompra, 
                    Cast(group_concat(distinct cp.idNf) as Char) as IdsNF";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From cheques c 
                Left Join loja l on (c.idLoja=l.idLoja)
                Left join cliente cli on (c.idCliente=cli.id_cli) 
                Left Join pagto_cheque pagc on (pagc.idCheque=c.idCheque) 
                Left Join pagto pag on (pag.idPagto=pagc.idPagto) 
                Left Join fornecedor forn on (forn.idFornec=pag.idFornec) 
                LEFT JOIN rota_cliente rc ON (cli.id_cli = rc.IdCliente)";

            if (selecionar)
                sql += "Left Join contas_pagar cp On (cp.idPagto=pag.idPagto) ";

            sql += " Where 1 ?filtroAdicional?";

            if (idLoja > 0)
            {
                filtroAdicional += " And c.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idPedido > 0)
            {
                filtroAdicional += " And (c.idPedido=" + idPedido +
                                   " Or c.idSinal In (Select idSinal From pedido Where idPedido=" +
                                   idPedido + "))";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                filtroAdicional += " and c.idLiberarPedido=" + idLiberarPedido + "    ";
                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            if (idAcerto > 0)
            {
                filtroAdicional += " And c.IdAcerto=" + idAcerto + "    ";
                criterio += "Acerto: " + idAcerto;
            }

            if (numeroNfe > 0)
            {
                // Obtém os cheques a partir da Liberação associada ao cheque.
                filtroAdicional += @" And (((c.idLiberarPedido In (Select idLiberarPedido From pedidos_nota_fiscal Where idNf In
                    (Select idNf From nota_fiscal Where numeroNfe=" + numeroNfe + ")))";
                // Obtém os cheques a partir do Pedido associado ao cheque.
                filtroAdicional += @" Or (c.idPedido In (Select idPedido From pedidos_nota_fiscal Where idNf In
                    (Select idNf From nota_fiscal Where numeroNfe=" + numeroNfe + ")))";
                // Obtém os cheques a partir do Acerto associado ao cheque.
                filtroAdicional +=
                    @" Or (c.idAcerto In (Select idAcerto From contas_receber Where idLiberarPedido In
                    (Select idLiberarPedido From pedidos_nota_fiscal Where idNf In (Select idNf from nota_fiscal Where numeroNfe=" +
                    numeroNfe + @"))))
                    Or (c.idAcerto In (Select idAcerto From contas_receber Where idPedido In
                    (Select idPedido From pedidos_nota_fiscal Where idNf In (Select idNf from nota_fiscal Where numeroNfe=" +
                    numeroNfe + ")))))";
                // Obtém os cheques a partir da conta a pagar associada ao cheque.
                filtroAdicional += " Or cp.idNf In (Select idNf From nota_fiscal Where numeroNfe=" + numeroNfe + "))";

                criterio += "Nota Fiscal: " + numeroNfe + "    ";
            }

            if (tipo > 0)
            {
                filtroAdicional += " And c.Tipo=" + (tipo != 3 ? tipo : 2);
                criterio += "Tipo: ";
                criterio += tipo == 1 ? "Próprio    " : tipo == 2 || tipo == 3 ? "Terceiro    " : "Indefinido    ";
            }

            if (numCheque > 0)
            {
                filtroAdicional += " And c.Num=" + numCheque;
                criterio += "Num. Cheque: " + numCheque + "    ";
            }

            if (!String.IsNullOrEmpty(situacao) && situacao != "0")
            {
                // Busca os que estão na situação Devolvido e Em Aberto
                if (situacao == "10")
                    filtroAdicional += " And (c.Situacao=" + (int) Cheques.SituacaoCheque.Devolvido +
                                       " Or c.Situacao=" + (int) Cheques.SituacaoCheque.EmAberto + ")";
                else if (situacao == "11") // Usado na tela de marcar cheque devolvido
                    filtroAdicional += " And c.Situacao=" + (int) Cheques.SituacaoCheque.Devolvido;
                // A situação 12 é utilizada na tela de marcar cheque advogado, neste caso devem ser buscados os cheques nas situações
                // Devolvido, Protestado ou ambos.
                else if (situacao.Contains("12"))
                    filtroAdicional += " And c.Situacao in (" + (String.IsNullOrEmpty(situacao.Replace("12", ""))
                        ? (int) Cheques.SituacaoCheque.Devolvido + "," + (int) Cheques.SituacaoCheque.Protestado
                        : situacao.Replace("12,", "")) + ")";
                else
                    filtroAdicional += " And c.Situacao in (" + situacao + ")";

                criterio += "Situação: ";

                foreach (string s in situacao.Split(','))
                {
                    switch (s.StrParaInt())
                    {
                        case 1:
                            criterio += "Em Aberto, ";
                            break;
                        case 2:
                            criterio += "Compensado, ";
                            break;
                        case 3:
                            criterio += "Devolvido, ";
                            break;
                        case 4:
                            criterio += "Quitado, ";
                            break;
                        case 5:
                            criterio += "Cancelado, ";
                            break;
                        case 6:
                            criterio += "Trocado, ";
                            break;
                        case 7:
                            criterio += "Protestado, ";
                            break;
                        case 10:
                            criterio += "Devolvido/Em Aberto, ";
                            break;
                        case 11:
                            criterio += "Reapresentado, ";
                            break;
                        // O critério "Devolvido/Protestado" será adicionado somente se na tela nenhuma situação for filtrada,
                        // pois pode ser que somente uma destas situações seja filtrada.
                        case 12:
                            criterio += situacao.Split(',').Length > 1 ? "" : "Devolvido/Protestado, ";
                            break;
                    }
                }

                criterio = criterio.TrimEnd(',', ' ') + "    ";
            }

            if (("," + situacao + ",").Contains(",3,") || situacao == "10")
            {
                if (reapresentado == true)
                {
                    filtroAdicional += " And (c.reapresentado=true Or c.situacao in (" + (situacao != "10"
                        ? situacao
                        : (int) Cheques.SituacaoCheque.EmAberto + "," + (int) Cheques.SituacaoCheque.Devolvido) + "))";

                    criterio += "Cheques reapresentados    ";
                }
                else if (reapresentado == false)
                    filtroAdicional += " And (c.reapresentado=false Or c.reapresentado is null)";
            }
            else if (situacao == "11") // Situação usada na tela de seleção de cheques e marcar cheques devolvidos
            {
                if (reapresentado == true)
                {
                    filtroAdicional += " And c.reapresentado=true";
                    criterio += "Cheques reapresentados    ";
                }
                else if (reapresentado == false)
                    filtroAdicional += " And (c.reapresentado=false Or c.reapresentado is null)";
            }

            if (advogado > 0)
            {
                switch (advogado)
                {
                    case 1:
                        filtroAdicional += " And c.advogado=True";
                        criterio += "Cheques advogados    ";
                        break;
                    case 2:
                        filtroAdicional += " And Coalesce(c.advogado, False)=False";
                        criterio += "Cheques não advogados    ";
                        break;
                    default:
                        break;
                }
            }

            if (!String.IsNullOrEmpty(titular))
            {
                filtroAdicional += " And c.Titular Like ?titular";
                criterio += "Titular: " + titular + "    ";
            }

            if (!String.IsNullOrEmpty(agencia))
            {
                filtroAdicional += " And c.Agencia Like ?agencia";
                criterio += "Agência: " + agencia + "    ";
            }

            if (!String.IsNullOrEmpty(conta))
            {
                filtroAdicional += " And c.Conta Like ?conta";
                criterio += "Conta: " + conta + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " And c.DataVenc>=?dataIni";
                criterio += "Vencimento: A partir de " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " And c.DataVenc<=?dataFim";

                if (!String.IsNullOrEmpty(dataIni))
                    criterio += " até " + dataFim + "    ";
                else
                    criterio += "Vencimento: Até " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                filtroAdicional += " And c.DataCad>=?dataCadIni";
                criterio += "Cadastrado a partir de " + dataCadIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                filtroAdicional += " And c.DataCad<=?dataCadFim";

                if (!String.IsNullOrEmpty(dataCadIni))
                    criterio += " até " + dataCadFim + "    ";
                else
                    criterio += "Cadastrado até " + dataCadFim + "    ";
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                filtroAdicional += " AND REPLACE(REPLACE(REPLACE(c.CpfCnpj, '.', ''), '-', ''), '/', '') LIKE ?cpfCnpj ";
                criterio += "CPF/CNPJ: " + cpfCnpj + "    ";
            }

            if (idCli > 0)
            {
                filtroAdicional += " And c.idCliente=" + idCli;
                criterio += "Cliente: " + idCli + " - " + nomeCli + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And cli.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
                temFiltro = true;
            }

            if (idFornec > 0)
            {
                sql += " And forn.idFornec=" + idFornec;
                criterio += "Fornecedor: " + idFornec + " - " + nomeFornec + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                sql += " And forn.idFornec in (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "    ";
                temFiltro = true;
            }

            if (valorInicial > 0)
            {
                filtroAdicional += " And c.Valor>=" + valorInicial.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                criterio += "Valor Inicial: " + valorInicial.ToString("C") + "    ";
            }

            if (valorFinal > 0)
            {
                filtroAdicional += " And c.Valor<=" + valorFinal.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                criterio += "Valor Final: " + valorFinal.ToString("C") + "    ";
            }

            if (!string.IsNullOrEmpty(nomeUsuCad))
            {
                filtroAdicional += " AND c.usuCad IN(" + FuncionarioDAO.Instance.GetIds(nomeUsuCad) + ")";
                criterio += " Cadastrado por: " + nomeUsuCad + "   ";
            }

            if (caixaDiario)
            {
                filtroAdicional += " AND c.movcaixadiario";
                criterio += " Movimenta caixa diario: " + caixaDiario + "   ";
            }

            if (!string.IsNullOrEmpty(idsRotas))
            {
                filtroAdicional += string.Format(" AND rc.IdRota IN ({0})", idsRotas);
                criterio += "Rota: " + RotaDAO.Instance.ObtemDescrRotas(idsRotas) + " ";
            }

            if (selecionar)
                sql += " Group By c.IdCheque";

            if (!string.IsNullOrEmpty(ordenacao))
            {
                var sqlOrd = new List<string>();
                var criterioOrd = new List<string>();

                foreach (var o in ordenacao.Split(',').Select(f=>f.StrParaInt()))
                {
                    switch (o)
                    {
                        case 1:
                            sqlOrd.Add("c.Titular");
                            criterioOrd.Add("Titular");
                            break;
                        case 2:
                            sqlOrd.Add("c.DataVenc");
                            criterioOrd.Add("Data Venc.");
                            break;
                        case 3:
                            sqlOrd.Add("c.Banco");
                            criterioOrd.Add("Banco"); ;
                            break;
                        case 4:
                            sqlOrd.Add("c.idPedido");
                            criterioOrd.Add("Pedido");
                            break;
                        case 5:
                            sqlOrd.Add("c.valor");
                            criterioOrd.Add("Pedido");
                            break;
                    }
                }

                if (sqlOrd.Count > 0)
                {
                    sql += " ORDER BY " + string.Join(",", sqlOrd);
                    criterio += "Ordenar por: " + string.Join(", ", criterioOrd) + "     ";
                }
            } 

            if (!string.IsNullOrEmpty(filtroAdicional))
                temFiltro = true;

            return sql.Replace("$$$", criterio);
        }

        public IList<Cheques> GetForDeposito(int numero)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, 0, 0, 0, 0, 2, numero, ((int) Cheques.SituacaoCheque.EmAberto).ToString(), null, 0,
                null, null, null,
                null, null, null, null, null, 0, null, 0, null, 0, 0, null, null, null, true, false, out temFiltro,
                out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Cheques> GetForRpt(uint idLoja, uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo,
            int numCheque, string situacao, bool reapresentado, int advogado, string titular, string agencia,
            string conta, string dataIni,
            string dataFim, string dataCadIni, string dataCadFim, string cpfCnpj, uint idCli, string nomeCli,
            uint idFornec,
            string nomeFornec, float valorInicial, float valorFinal, string nomeUsuCad, string idsRotas,
            bool chequesCaixaDiario,
            string ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(idLoja, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao,
                reapresentado,
                advogado, titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj, idCli, nomeCli,
                idFornec, nomeFornec,
                valorInicial, valorFinal, nomeUsuCad, idsRotas, ordenacao, true, chequesCaixaDiario, out temFiltro,
                out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            return
                objPersistence.LoadData(sql,
                    GetFilterParam(titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj,
                        nomeCli, nomeFornec)).ToList();
        }

        public IList<Cheques> GetByFilter(uint idLoja, uint idPedido, uint idLiberarPedido, uint idAcerto,
            uint numeroNfe, int tipo,
            int numCheque, string situacao, bool reapresentado, int advogado, string titular, string agencia,
            string conta, string dataIni,
            string dataFim, string dataCadIni, string dataCadFim, string cpfCnpj, uint idCli, string nomeCli,
            uint idFornec,
            string nomeFornec, float valorInicial, float valorFinal, string nomeUsuCad, string idsRotas, string ordenacao,
            bool chequesCaixaDiario, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(idLoja, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao,
                reapresentado,
                advogado, titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj, idCli, nomeCli,
                idFornec,
                nomeFornec, valorInicial, valorFinal, nomeUsuCad, idsRotas, ordenacao, true, chequesCaixaDiario,
                out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj, nomeCli,
                    nomeFornec));
        }

        public int GetCountFilter(uint idLoja, uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo, int numCheque,
            string situacao, bool reapresentado, int advogado, string titular, string agencia, string conta,
            string dataIni,
            string dataFim, string dataCadIni, string dataCadFim, string cpfCnpj, uint idCli, string nomeCli,
            uint idFornec,
            string nomeFornec, float valorInicial, float valorFinal, string nomeUsuCad, string idsRotas, string ordenacao,
            bool chequesCaixaDiario)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(idLoja, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao,
                reapresentado,
                advogado, titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj, idCli, nomeCli,
                idFornec,
                nomeFornec, valorInicial, valorFinal, nomeUsuCad, idsRotas, ordenacao, true, chequesCaixaDiario,
                out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, dataCadIni, dataCadFim, cpfCnpj, nomeCli,
                    nomeFornec));
        }

        public IList<Cheques> GetListDevolvido(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo, int numCheque,
            int situacao, bool reapresentado, string titular, string agencia, string conta, string dataIni,
            string dataFim, uint idCli,
            string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal, int ordenacao,
            string sortExpression,
            int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?",
                    temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        public int GetCountDevolvido(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque,
            int situacao, bool reapresentado, string titular, string agencia, string conta, string dataIni,
            string dataFim, uint idCli,
            string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal, int ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?",
                    temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        public IList<Cheques> GetListReapresentado(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo,
            int numCheque, int situacao, bool reapresentado, string titular, string agencia, string conta,
            string dataIni, string dataFim,
            uint idCli, string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal,
            int ordenacao,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?",
                    temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        public int GetCountReapresentado(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque,
            int situacao, bool reapresentado, string titular, string agencia, string conta, string dataIni,
            string dataFim, uint idCli,
            string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal, int ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?",
                    temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        public IList<Cheques> GetForAdvogado(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe,
            int tipo, int numCheque,
            int advogado, string situacao, string titular, string agencia, string conta, string dataIni, string dataFim,
            float valorInicial, float valorFinal, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            // A situação 12 significa que estão sendo buscados cheques na situação Devolvido/Protestado.
            situacao = "12" + (String.IsNullOrEmpty(situacao) ? "" : "," + situacao);

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao, null,
                advogado, titular,
                agencia, conta, dataIni, dataFim, null, null, null, 0, null, 0, null, valorInicial, valorFinal, null,
                null, null, true, false,
                out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        public int GetCountAdvogado(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque,
            string situacao, int advogado, string titular, string agencia, string conta, string dataIni, string dataFim,
            float valorInicial, float valorFinal)
        {
            bool temFiltro;
            string filtroAdicional;

            // A situação 12 significa que estão sendo buscados cheques na situação Devolvido/Protestado.
            situacao = "12" + (String.IsNullOrEmpty(situacao) ? "" : "," + situacao);

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque, situacao, null,
                advogado, titular,
                agencia, conta, dataIni, dataFim, null, null, null, 0, null, 0, null, valorInicial, valorFinal, null,
                null, null, true, false,
                out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        public IList<Cheques> GetForSel(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque,
            int situacao, bool? reapresentado, string titular, string agencia, string conta, string dataIni,
            string dataFim, uint idCli,
            string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal, int ordenacao,
            bool chequesCaixaDiario,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            // Alteração requisitada pela Riber Vidros, não alterar
            sortExpression = ordenacao == 0 && String.IsNullOrEmpty(sortExpression) ? "dataVenc asc" : sortExpression;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, chequesCaixaDiario, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        public IList<Cheques> GetForSel(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque,
            int situacao, bool? reapresentado, string titular, string agencia, string conta, string dataIni,
            string dataFim, uint idCli,
            string nomeCli, uint idFornec, string nomeFornec, float valorInicial, float valorFinal, int ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            return
                objPersistence.LoadData(sql,
                    GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli,
                        nomeFornec)).ToList();
        }

        public int GetCountSel(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNfe, int tipo,
            int numCheque, int situacao,
            bool reapresentado, string titular, string agencia, string conta, string dataIni, string dataFim, uint idCli,
            string nomeCli,
            uint idFornec, string nomeFornec, float valorInicial, float valorFinal, bool chequesCaixaDiario,
            int ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, idLiberarPedido, idAcerto, numeroNfe, tipo, numCheque,
                situacao.ToString(), reapresentado,
                0, titular, agencia, conta, dataIni, dataFim, null, null, null, idCli, nomeCli, idFornec, nomeFornec,
                valorInicial,
                valorFinal, null, null, ordenacao.ToString(), true, chequesCaixaDiario, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, nomeCli, nomeFornec));
        }

        private GDAParameter[] GetFilterParam(string titular, string agencia, string conta, string dataIni,
            string dataFim,
            string dataCadIni, string dataCadFim, string cpfCnpj, string nomeCli, string nomeFornec)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(titular))
                lstParam.Add(new GDAParameter("?titular", "%" + titular + "%"));

            if (!String.IsNullOrEmpty(agencia))
                lstParam.Add(new GDAParameter("?agencia", "%" + agencia + "%"));

            if (!String.IsNullOrEmpty(conta))
                lstParam.Add(new GDAParameter("?conta", "%" + conta + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59")));

            if (!String.IsNullOrEmpty(cpfCnpj))
                lstParam.Add(new GDAParameter("?cpfCnpj",
                    "%" + cpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "") + "%"));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca Cheques Devolvidos

        /// <summary>
        /// Busca cheques devolvidos para consulta ao efetuar acerto
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IList<Cheques> GetDevolvidosPorCliente(uint idCliente)
        {
            if (idCliente == 0)
                return null;

            string filtroAdicional;
            bool temFiltro;

            string sql = SqlFilter(0, 0, 0, 0, 0, 0, 0, ((int) Cheques.SituacaoCheque.Devolvido + "," +
                                                         (int) Cheques.SituacaoCheque.Protestado).ToString(), false, 0,
                null, null, null, null, null, null, null, null, idCliente,
                null, 0, null, 0, 0, null, null, null, true, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca Cheques Devolvidos para Recebimento
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idAcerto"></param>
        /// <param name="situacao"></param>
        /// <param name="tipo"></param>
        /// <param name="numCheque"></param>
        /// <param name="titular"></param>
        /// <param name="agencia"></param>
        /// <param name="conta"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<Cheques> GetDevolvidos(uint idPedido, uint idAcerto, int situacao, int tipo, int numCheque,
            string titular,
            string agencia, string conta, string dataIni, string dataFim, string sortExpression, int startRow,
            int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, 0, idAcerto, 0, tipo, numCheque, situacao.ToString(), null, 0, titular,
                agencia, conta,
                dataIni, dataFim, null, null, null, 0, null, 0, null, 0, 0, null, null, null, true, false, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        public int GetCountDevolvidos(uint idPedido, uint idAcerto, int situacao, int tipo, int numCheque,
            string titular,
            string agencia, string conta, string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, 0, idAcerto, 0, tipo, numCheque, situacao.ToString(), null, 0, titular,
                agencia, conta,
                dataIni, dataFim, null, null, null, 0, null, 0, null, 0, 0, null, null, null, true, false, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        #endregion

        #region Busca Cheques de Terceiros para serem utilizados em um pagamento

        public IList<Cheques> GetTerceiros(uint idPedido, uint idAcerto, int numCheque, string titular, string agencia,
            string conta,
            string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, 0, idAcerto, 0, 2, numCheque,
                ((int) Cheques.SituacaoCheque.EmAberto).ToString(), null, 0,
                titular, agencia, conta, dataIni, dataFim, null, null, null, 0, null, 0, null, 0, 0, null, null, null, true,
                false,
                out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        public int GetCountTerceiros(uint idPedido, uint idAcerto, int numCheque, string titular, string agencia,
            string conta,
            string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlFilter(0, idPedido, 0, idAcerto, 0, 2, numCheque,
                ((int) Cheques.SituacaoCheque.EmAberto).ToString(), null, 0,
                titular, agencia, conta, dataIni, dataFim, null, null, null, 0, null, 0, null, 0, 0, null, null, null, true,
                false,
                out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetFilterParam(titular, agencia, conta, dataIni, dataFim, null, null, null, null, null));
        }

        #endregion

        #region Busca cheques cadastrados pelo financeiro pagto.

        private string SqlFinanc(int numCheque, int situacao, string titular, string agencia, string conta,
            string dataIni,
            string dataFim, bool chequesCaixaDiario, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = " and origem=" + (uint) Cheques.OrigemCheque.FinanceiroPagto;

            string campos = selecionar ? "c.*, '$$$' as Criterio" : "Count(*)";
            string criterio = String.Empty;

            string sql = "Select " + campos + " From cheques c Where 1 ?filtroAdicional?";

            if (numCheque > 0)
            {
                filtroAdicional += " And c.Num=" + numCheque;
                criterio = "Num. Cheque: " + numCheque + "    ";
            }

            if (situacao > 0)
            {
                filtroAdicional += " And Situacao=" + situacao;
                criterio = "Situação: " + Instance.GetSituacaoCheque(situacao) + "    ";
            }

            if (!String.IsNullOrEmpty(titular))
            {
                filtroAdicional += " And c.Titular Like ?titular";
                criterio += "Titular: " + titular + "    ";
            }

            if (!String.IsNullOrEmpty(agencia))
            {
                filtroAdicional += " And c.Agencia Like ?agencia";
                criterio += "Agência: " + agencia + "    ";
            }

            if (!String.IsNullOrEmpty(conta))
            {
                filtroAdicional += " And c.Conta Like ?conta";
                criterio += "Conta: " + conta + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " And c.DataVenc>=?dataIni";
                criterio += "Vencimento: A partir de " + dataIni;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " And c.DataVenc<=?dataFim";

                if (!String.IsNullOrEmpty(dataIni))
                    criterio += " até " + dataFim + "    ";
                else
                    criterio += "Vencimento: Até " + dataFim;
            }

            if (chequesCaixaDiario)
            {
                filtroAdicional += " AND c.movcaixadiario";
                criterio += " Movimenta caixa diario: sim    ";
            }


            return sql.Replace("$$$", criterio);
        }

        public IList<Cheques> GetListFinanc(int numCheque, int situacao, string titular, string agencia, string conta,
            string dataIni,
            string dataFim, bool chequesCaixaDiario, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlFinanc(numCheque, situacao, titular, agencia, conta, dataIni, dataFim, chequesCaixaDiario,
                true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return
                objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(sortExpression),
                    new InfoPaging(startRow, pageSize),
                    GetFinancParam(titular, agencia, conta, dataIni, dataFim)).ToList();
        }

        public int GetCountFinanc(int numCheque, int situacao, string titular, string agencia, string conta,
            string dataIni, string dataFim, bool chequesCaixaDiario)
        {
            string filtroAdicional;
            string sql = SqlFinanc(numCheque, situacao, titular, agencia, conta, dataIni, dataFim, chequesCaixaDiario,
                false, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sql, GetFinancParam(titular, agencia, conta, dataIni, dataFim));
        }

        private GDAParameter[] GetFinancParam(string titular, string agencia, string conta, string dataIni,
            string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(titular))
                lstParam.Add(new GDAParameter("?titular", "%" + titular + "%"));

            if (!String.IsNullOrEmpty(agencia))
                lstParam.Add(new GDAParameter("?agencia", "%" + agencia + "%"));

            if (!String.IsNullOrEmpty(conta))
                lstParam.Add(new GDAParameter("?conta", "%" + conta + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca cheques próprios utilizados em pagamentos

        private string SqlChequesPagto(uint idContaBanco, int situacao, uint idFornec, string nomeFornec, string dataIni,
            string dataFim, float valorInicial,
            float valorFinal, int numCheque, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and c.tipo=1";

            string campos = selecionar
                ? "c.*, forn.idFornec as IdFornecedor, pag.idPagto, " +
                  "coalesce(forn.NomeFantasia, forn.RazaoSocial) as NomeFornecedor"
                : "Count(*)";

            string sql = @"
                Select " + campos + @" From cheques c 
                    Left Join pagto_cheque pagc on (pagc.idCheque=c.idCheque) 
                    Left Join pagto pag on (pag.idPagto=pagc.idPagto) 
                    Left Join fornecedor forn on (forn.idFornec=pag.idFornec) 
                Where 1 ?filtroAdicional?";

            if (idContaBanco > 0)
            {
                string conta = ContaBancoDAO.Instance.ObtemValorCampo<string>("conta", "idContaBanco=" + idContaBanco);
                string agencia = ContaBancoDAO.Instance.ObtemValorCampo<string>("agencia",
                    "idContaBanco=" + idContaBanco);
                filtroAdicional += " And (c.idContaBanco=" + idContaBanco + " or (lower(c.Conta)='" + conta.ToLower() +
                                   "' And lower(c.Agencia)='" + agencia.ToLower() + "'))";
            }

            if (idFornec > 0)
            {
                sql += " And forn.idFornec=" + idFornec;
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                sql += " And forn.idFornec in (" + ids + ")";
                temFiltro = true;
            }

            if (situacao > 0)
                filtroAdicional += " And c.Situacao=" + situacao;

            if (!String.IsNullOrEmpty(dataIni))
                filtroAdicional += " And c.DataVenc>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                filtroAdicional += " And c.DataVenc<=?dataFim";

            if (valorInicial > 0)
                filtroAdicional += " And c.Valor>=" + valorInicial.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

            if (valorFinal > 0)
                filtroAdicional += " And c.Valor<=" + valorFinal.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

            if (numCheque > 0)
                filtroAdicional += " And c.Num=" + numCheque;

            return sql;
        }

        public IList<Cheques> GetChequesPagto(uint idContaBanco, int situacao, uint idFornec, string nomeFornec,
            string dataIni, string dataFim, float valorInicial,
            float valorFinal, int numCheque, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "c.dataVenc, c.Num";

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlChequesPagto(idContaBanco, situacao, idFornec, nomeFornec, dataIni, dataFim, valorInicial,
                valorFinal, numCheque, true, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetChequesPagtoParam(nomeFornec, dataIni, dataFim));
        }

        public int GetCountChequesPagto(uint idContaBanco, int situacao, uint idFornec, string nomeFornec,
            string dataIni, string dataFim, float valorInicial,
            float valorFinal, int numCheque)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlChequesPagto(idContaBanco, situacao, idFornec, nomeFornec, dataIni, dataFim, valorInicial,
                valorFinal, numCheque, true, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : String.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetChequesPagtoParam(nomeFornec, dataIni, dataFim));
        }

        private GDAParameter[] GetChequesPagtoParam(string nomeFornec, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca cheques de um pagamento específico

        /// <summary>
        /// Busca cheques de um pagamento específico
        /// </summary>
        public IList<Cheques> GetByPagto(uint idPagto)
        {
            return GetByPagto(null, idPagto);
        }

        /// <summary>
        /// Busca cheques de um pagamento específico
        /// </summary>
        public IList<Cheques> GetByPagto(GDASession session, uint idPagto)
        {
            var buscarReais = !PagtoDAO.Instance.Exists(session, idPagto) ||
                PagtoDAO.Instance.ObtemValorCampo<Pagto.SituacaoPagto>(session, "situacao", "idPagto=" + idPagto) !=
                Pagto.SituacaoPagto.Cancelado;

            var sql = "Select * From cheques Where idCheque In (";

            if (buscarReais)
                sql += "Select idCheque From pagto_cheque Where idPagto=" + idPagto + " and situacao<>" +
                    (int) Cheques.SituacaoCheque.Cancelado;
            else
            {
                string idsChequesPg = PagtoDAO.Instance.ObtemValorCampo<string>(session, "idsChequesPg", "idPagto=" + idPagto);
                sql += (!String.IsNullOrEmpty(idsChequesPg) ? idsChequesPg : "0");
            }

            return objPersistence.LoadData(session, sql + ")").ToList();
        }

        #endregion

        #region Busca cheques de uma liberação de pedidos

        /// <summary>
        /// Busca cheques de uma liberação de pedidos
        /// </summary>
        public IList<Cheques> GetByLiberacaoPedido(GDASession sessao, uint idLiberarPedido)
        {
            string sql = "Select * From cheques Where idLiberarPedido=" + idLiberarPedido;

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca cheques de um depósito

        /// <summary>
        /// Busca cheques de um depósito
        /// </summary>
        /// <param name="idDeposito"></param>
        /// <returns></returns>
        public IList<Cheques> GetByDeposito(uint idDeposito)
        {
            return GetByDeposito(null, idDeposito);
        }

        /// <summary>
        /// Busca cheques de um depósito
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idDeposito"></param>
        /// <returns></returns>
        public IList<Cheques> GetByDeposito(GDASession sessao, uint idDeposito)
        {
            string sql =
                "Select c.*, cli.Nome as NomeCliente From cheques c Left Join cliente cli On (c.idCliente=cli.id_Cli) " +
                "Where c.idDeposito=" + idDeposito;

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Retorna os cheques devolvidos relacionado à esta depósito
        /// </summary>
        /// <param name="idDepositoCanc"></param>
        /// <returns></returns>
        public IList<Cheques> GetByDepositoDev(uint idDepositoCanc)
        {
            string sql = "Select * From cheques Where idDepositoCanc=" + idDepositoCanc;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca cheques de um acerto

        /// <summary>
        /// Busca cheques de um acerto
        /// </summary>
        public IList<Cheques> GetByAcerto(uint idAcerto)
        {
            return GetByAcerto(null, idAcerto);
        }

        /// <summary>
        /// Busca cheques de um acerto
        /// </summary>
        public IList<Cheques> GetByAcerto(GDASession session, uint idAcerto)
        {
            bool buscarReais = !AcertoDAO.Instance.Exists(session, idAcerto) ||
                AcertoDAO.Instance.ObtemValorCampo<Acerto.SituacaoEnum>(session, "situacao",
                    "idAcerto=" + idAcerto) != Acerto.SituacaoEnum.Cancelado;

            string sql = "Select * From cheques Where 1";

            if (buscarReais)
                sql += " and idAcerto=" + idAcerto;
            else
            {
                string idsChequesR = AcertoDAO.Instance.ObtemValorCampo<string>(session, "idsChequesR", "idAcerto=" + idAcerto);
                sql += " and idCheque in (" + (!string.IsNullOrEmpty(idsChequesR) ? idsChequesR : "0") + ")";
            }

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca cheques de um sinal

        /// <summary>
        /// Busca cheques de um sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<Cheques> GetBySinal(uint idSinal)
        {
            return GetBySinal(null, idSinal);
        }

        /// <summary>
        /// Busca cheques de um sinal
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<Cheques> GetBySinal(GDASession sessao, uint idSinal)
        {
            bool buscarReais = !SinalDAO.Instance.Exists(sessao, idSinal) ||
                               SinalDAO.Instance.ObtemValorCampo<Sinal.SituacaoEnum>(sessao, "situacao",
                                   "idSinal=" + idSinal) != Sinal.SituacaoEnum.Cancelado;

            string sql = "Select * From cheques Where 1";

            if (buscarReais)
                sql += " and idSinal=" + idSinal;
            else
            {
                string idsChequesR = SinalDAO.Instance.ObtemValorCampo<string>(sessao, "idsChequesR",
                    "idSinal=" + idSinal);
                sql += " and idCheque in (" + (!String.IsNullOrEmpty(idsChequesR) ? idsChequesR : "0") + ")";
            }

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca cheques de um sinal de compra

        /// <summary>
        /// Busca cheques de um sinal
        /// </summary>
        public IList<Cheques> GetBySinalCompra(GDASession session, uint idSinalCompra)
        {
            bool buscarReais = !SinalCompraDAO.Instance.Exists(session, idSinalCompra) ||
                               !SinalCompraDAO.Instance.ObtemValorCampo<bool>(session, "cancelado",
                                   "idSinalCompra=" + idSinalCompra);

            string sql = "Select * From cheques Where 1";

            if (buscarReais)
                sql += " and idSinalCompra=" + idSinalCompra;
            else
            {
                string idsChequesR = SinalCompraDAO.Instance.ObtemValorCampo<string>(session, "idsCheques",
                    "idSinalCompra=" + idSinalCompra);
                sql += " and idCheque in (" + (!String.IsNullOrEmpty(idsChequesR) ? idsChequesR : "0") + ")";
            }

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca cheques de uma antecipação de fornecedor

        /// <summary>
        /// Busca cheques de uma antecipação de fornecedor
        /// </summary>
        public IList<Cheques> GetByAntecipFornec(uint idAntecipFornec)
        {
            return GetByAntecipFornec(null, idAntecipFornec);
        }

        /// <summary>
        /// Busca cheques de uma antecipação de fornecedor
        /// </summary>
        public IList<Cheques> GetByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            string sql = "Select * From cheques Where idAntecipFornec=" + idAntecipFornec;

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca cheques de uma conta recebida

        /// <summary>
        /// Busca cheques de um acerto
        /// </summary>
        public IList<Cheques> GetByContaR(uint idContaR)
        {
            string sql = "Select * From cheques Where idContaR=" + idContaR;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca cheques de um acerto de cheques

        /// <summary>
        /// Busca cheques de um acerto
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        /// <param name="acertado">Retornar os cheques que foram acertados?</param>
        /// <returns></returns>
        public IList<Cheques> GetByAcertoCheque(uint idAcertoCheque, bool acertado)
        {
            return GetByAcertoCheque(null, idAcertoCheque, acertado);
        }

        /// <summary>
        /// Busca cheques de um acerto
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAcertoCheque"></param>
        /// <param name="acertado">Retornar os cheques que foram acertados?</param>
        /// <returns></returns>
        public IList<Cheques> GetByAcertoCheque(GDASession session, uint idAcertoCheque, bool acertado)
        {
            string sql = acertado
                ? @"
                Select c.idCheque, c.idPedido, c.num, c.banco, c.agencia, c.conta, c.titular, c.valor, c.dataVenc, c.dataCad, c.usuCad,
                    c.idAcerto, c.origem, c.situacao, c.obs, c.tipo, c.idDeposito, c.idContaR, c.valorReceb, c.jurosReceb, c.dataReceb,
                    c.idLiberarPedido, c.idAcertoCheque, c.movCaixaFinanceiro, c.idCliente, c.movBanco, c.dataVencOriginal, c.reapresentado,
                    c.advogado, c.idTrocaDevolucao, c.idObra, c.jurosPagto, c.multaPagto, c.idDevolucaoPagto, c.idContaBanco, c.digitoNum,
                    c.idSinal, c.idDepositoCanc, c.cancelouDevolucao, c.idCreditoFornecedor, c.idSinalCompra, c.idAntecipFornec, c.cpfCnpj,
                    c.idLoja, iac.idAcertoCheque As IdAcertoChequeAcertado, pag.idPagto, iac.valorReceb as valorAcertoCheque
                From cheques c
                    Left Join pagto_cheque pagc On (pagc.idCheque=c.idCheque) 
                    Left Join pagto pag On (pag.idPagto=pagc.idPagto)
                    Inner Join item_acerto_cheque iac ON (c.idCheque=iac.idCheque And iac.idAcertoCheque=" +
                  idAcertoCheque + ")"
                : "Select c.* From cheques c Where c.idAcertoCheque=" + idAcertoCheque;

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Busca cheques de uma devolução de pagamento

        /// <summary>
        /// Busca os IDs dos cheques de uma devolução de pagamento.
        /// </summary>
        public string GetIdsByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            string sql = "select coalesce(cheques, '') from devolucao_pagto where idDevolucaoPagto=" + idDevolucaoPagto;
            return objPersistence.ExecuteScalar(session, sql).ToString();
        }

        /// <summary>
        /// Busca os cheques de uma devolução de pagamento.
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public IList<Cheques> GetByDevolucaoPagto(uint idDevolucaoPagto)
        {
            string ids = GetIdsByDevolucaoPagto(null, idDevolucaoPagto);
            return !String.IsNullOrEmpty(ids) ? GetByPks(ids) : new Cheques[0];
        }

        #endregion

        #region Busca cheques de uma obra

        /// <summary>
        /// Busca cheques de uma obra.
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public IList<Cheques> GetByObra(uint idObra)
        {
            string filtroAdicional;
            string sql = Sql(0, 0, 0, 0, 0, 0, idObra, 0, 0, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca cheques de um crédito de fornecedor

        /// <summary>
        /// Busca cheques de um crédito de fornecedor
        /// </summary>
        /// <param name="idCreditoFornecedor"></param>
        /// <returns></returns>
        public IList<Cheques> GetByCreditoFornecedor(uint idCreditoFornecedor)
        {
            return GetByCreditoFornecedor(null, idCreditoFornecedor);
        }

        /// <summary>
        /// Busca cheques de um crédito de fornecedor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCreditoFornecedor"></param>
        /// <returns></returns>
        public IList<Cheques> GetByCreditoFornecedor(GDASession session, uint idCreditoFornecedor)
        {
            string sql = "Select * From cheques Where idCreditoFornecedor=" + idCreditoFornecedor;

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Exclui cheques em aberto e cancela os demais

        /// <summary>
        /// Deleta todos os cheques abertos do pedido e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(uint idPedido)
        {
            DeleteByPedido(null, idPedido);
        }

        /// <summary>
        /// Deleta todos os cheques abertos do pedido e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(GDASession session, uint idPedido)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete from cheques Where IdPedido=" + idPedido + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(session,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idPedido=" + idPedido);
        }

        /// <summary>
        /// Exclui todos os cheques abertos do sinal do pedido e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="idSinal"></param>
        public void DeleteBySinalPedido(uint idSinal)
        {
            DeleteBySinalPedido(null, idSinal);
        }

        /// <summary>
        /// Exclui todos os cheques abertos do sinal do pedido e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSinal"></param>
        public void DeleteBySinalPedido(GDASession sessao, uint idSinal)
        {
            objPersistence.ExecuteCommand(sessao,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " Where idSinal=" + idSinal);
        }

        /// <summary>
        /// Exclui cheques utilizados para pagar acerto
        /// </summary>
        public void DeleteByAcerto(GDASession sessao, uint idAcerto)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete From cheques Where idAcerto=" + idAcerto + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(sessao,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idAcerto=" + idAcerto);
        }

        /// <summary>
        /// Exclui cheques utilizados para pagar a contas a receber passada
        /// </summary>
        public void DeleteByContaRec(GDASession sessao, uint idContaR)
        {
            // Altera os cheques para cancelado e desassocia a conta a receber
            objPersistence.ExecuteCommand(sessao,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado +
                ", idContaR=null where idContaR=" + idContaR);
        }

        /// <summary>
        /// Exclui todos os cheques abertos da liberação passada e cancela os cheques que não forem excluídos.
        /// </summary>
        public void DeleteByLiberarPedido(GDASession sessao, uint idLiberarPedido)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete From cheques Where idLiberarPedido=" + idLiberarPedido + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(sessao,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idLiberarPedido=" +
                idLiberarPedido);
        }

        /// <summary>
        /// Exclui todos os cheques abertos da obra passada e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="idObra"></param>
        public void DeleteByObra(uint idObra)
        {
            DeleteByObra(null, idObra);
        }

        /// <summary>
        /// Exclui todos os cheques abertos da obra passada e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        public void DeleteByObra(GDASession sessao, uint idObra)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete From cheques Where idObra=" + idObra + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(sessao,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idObra=" + idObra);
        }

        /// <summary>
        /// Exclui todos os cheques abertos do acerto de cheque passado e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="idAcertoCheque"></param>
        public void DeleteByAcertoCheque(uint idAcertoCheque)
        {
            DeleteByAcertoCheque(null, idAcertoCheque);
        }

        /// <summary>
        /// Exclui todos os cheques abertos do acerto de cheque passado e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAcertoCheque"></param>
        public void DeleteByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete From cheques Where idAcertoCheque=" + idAcertoCheque + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(session,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idAcertoCheque=" +
                idAcertoCheque);
        }

        /// <summary>
        /// Exclui todos os cheques abertos da troca/devolução passada e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        public void DeleteByTrocaDevolucao(uint idTrocaDevolucao)
        {
            DeleteByTrocaDevolucao(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Exclui todos os cheques abertos da troca/devolução passada e cancela os cheques que não forem excluídos.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        public void DeleteByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete From cheques Where idTrocaDevolucao=" + idTrocaDevolucao + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);
            objPersistence.ExecuteCommand(session,
                "update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado + " where idTrocaDevolucao=" +
                idTrocaDevolucao);
        }

        #endregion

        #region Devolução de pagamento

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cancela os cheques usados na devolução de pagamento.
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <param name="idsCheques"></param>
        public void CancelaForDevolucaoPagto(uint idDevolucaoPagto, string idsCheques)
        {
            CancelaForDevolucaoPagto(null, idDevolucaoPagto, idsCheques);
        }

        /// <summary>
        /// Cancela os cheques usados na devolução de pagamento.
        /// </summary>
        public void CancelaForDevolucaoPagto(GDASession sessao, uint idDevolucaoPagto, string idsCheques)
        {
            foreach (var idCheque in idsCheques.Split(','))
            {
                var cheque = GetElementByPrimaryKey(sessao, idCheque.StrParaInt());
                cheque.Situacao = (int)Cheques.SituacaoCheque.Cancelado;
                cheque.IdDevolucaoPagto = idDevolucaoPagto;

                UpdateBase(sessao, cheque, false);
            }
        }

        /// <summary>
        /// Reabre todos os cheques usados na devolução de pagamento.
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        public void ReabreByDevolucaoPagto(uint idDevolucaoPagto)
        {
            ReabreByDevolucaoPagto(null, idDevolucaoPagto);
        }

        /// <summary>
        /// Reabre todos os cheques usados na devolução de pagamento.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idDevolucaoPagto"></param>
        public void ReabreByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            // Reabre os cheques de terceiro devolvidos.
            var idsChequeTerceiros =
                ExecuteMultipleScalar<int>(session,
                    string.Format("SELECT IdCheque FROM CHEQUES WHERE Tipo=2 AND IdDevolucaoPagto={0}", idDevolucaoPagto));

            foreach (var idCheque in idsChequeTerceiros)
            {
                var cheque = GetElementByPrimaryKey(session, idCheque);
                cheque.Situacao = (int)Cheques.SituacaoCheque.EmAberto;
                cheque.IdDevolucaoPagto = null;

                UpdateBase(session, cheque, false);
            }

            // Cancela os cheques próprios pagos ao cliente
            var idsChequeProprio =
                ExecuteMultipleScalar<int>(session,
                    string.Format("SELECT IdCheque FROM CHEQUES WHERE Tipo=1 AND IdDevolucaoPagto={0}", idDevolucaoPagto));

            foreach (var idCheque in idsChequeProprio)
            {
                var cheque = GetElementByPrimaryKey(session, idCheque);
                cheque.Situacao = (int)Cheques.SituacaoCheque.Cancelado;
                cheque.IdDevolucaoPagto = null;

                UpdateBase(session, cheque, false);
            }
        }

        #endregion

        #region Retorna o total em cheques cadastrados para o acerto passado.

        /// <summary>
        /// Retorna o total em cheques cadastrados para o acerto passado.
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public string GetTotalInAcerto(uint idAcerto)
        {
            object retorno =
                objPersistence.ExecuteScalar("Select Sum(Valor) From cheques Where idAcerto=" + idAcerto).ToString();

            return retorno.ToString() == String.Empty ? "0" : retorno.ToString();
        }

        #endregion

        #region Retorna o total em cheques cadastrados no pedido passado

        /// <summary>
        /// Retorna o total em cheques cadastrados para o pedido passado.
        /// </summary>
        public string GetTotalInPedido(uint idPedido, uint idContaR, int situacao, int origem)
        {
            string sql = "Select Sum(Valor) From cheques Where idPedido=" + idPedido;

            if (situacao > 0)
                sql += " And Situacao=" + situacao;

            if (origem > 0)
                sql += " And Origem=" + origem;

            if (idContaR > 0)
                sql += " And IdContaR=" + idContaR;

            object retorno = objPersistence.ExecuteScalar(sql).ToString();

            return retorno.ToString() == String.Empty ? "0" : retorno.ToString();
        }

        #endregion

        #region Retorna o total em cheques cadastrados para o depósito passado.

        /// <summary>
        /// Retorna o total em cheques cadastrados para o depósito passado.
        /// </summary>
        public decimal GetTotalInDeposito(uint idDeposito)
        {
            return GetTotalInDeposito(null, idDeposito);
        }

        /// <summary>
        /// Retorna o total em cheques cadastrados para o depósito passado.
        /// </summary>
        public decimal GetTotalInDeposito(GDASession sessao, uint idDeposito)
        {
            return ExecuteScalar<decimal>(sessao, "Select Sum(Valor) From cheques Where idDeposito=" + idDeposito +
                                                  " and (Origem<>" + (int) Cheques.OrigemCheque.FinanceiroPagto +
                                                  " or (Origem=" +
                                                  (int) Cheques.OrigemCheque.FinanceiroPagto +
                                                  " and MovCaixaFinanceiro=true))");
        }

        #endregion

        #region Retorna o total em cheques cadastrados para o pagamento passado.

        /// <summary>
        /// Retorna o total em cheques cadastrados para o pagamento passado.
        /// </summary>
        public decimal GetTotalInPagto(uint idPagto)
        {
            return ExecuteScalar<decimal>(@"Select Sum(Valor) From cheques c 
                Left Join pagto_cheque pagc on (pagc.idCheque=c.idCheque) Where pagc.idPagto=" + idPagto +
                                          " and (c.Origem<>" + (int) Cheques.OrigemCheque.FinanceiroPagto +
                                          " or (c.Origem=" +
                                          (int) Cheques.OrigemCheque.FinanceiroPagto +
                                          " and c.MovCaixaFinanceiro=true))");
        }

        #endregion

        #region Retorna o total em cheques cadastrados para o recebimento (Quitamento) do cheque devolvido passado

        /// <summary>
        /// Retorna o total em cheques cadastrados para o recebimento do cheque devolvido passado.
        /// </summary>
        public string GetTotalInChequeDevolvido(uint idChequeDevolvido)
        {
            string sql = "Select Sum(Valor) From cheques Where idChequeDevolvido=" + idChequeDevolvido;

            object retorno = objPersistence.ExecuteScalar(sql).ToString();

            return retorno.ToString() == String.Empty ? "0" : retorno.ToString();
        }

        #endregion

        #region Retorna o total em cheques cadastrados para a liberação de pedidos passada

        /// <summary>
        /// Retorna o total em cheques cadastrados para a liberação de pedidos passada.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public string GetTotalInLiberarPedido(uint idLiberarPedido)
        {
            string sql = "Select Sum(Valor) From cheques Where idLiberarPedido=" + idLiberarPedido;

            object retorno = objPersistence.ExecuteScalar(sql).ToString();

            return retorno.ToString() == String.Empty ? "0" : retorno.ToString();
        }

        #endregion

        #region Retorna o total em cheques de terceiros em aberto e vencidos

        /// <summary>
        /// Retorna o total em cheques de terceiros em aberto e vencidos
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalTercVenc(uint idLoja)
        {
            // Não deve considerar os cheques devolvidos
            string sql =
                "Select Coalesce(Sum(Valor), 0) From cheques Where tipo=2 and datavenc<=now() and situacao in (" +
                (int) Cheques.SituacaoCheque.EmAberto + ")";

            if (idLoja > 0)
                sql += " AND idLoja=" + idLoja;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Retorna o total em cheques de terceiros em aberto

        /// <summary>
        /// Retorna o total em cheques de terceiros em aberto/devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalTerc(uint idLoja)
        {
            return GetTotalTerc(true, false, TipoReapresentados.IgnorarReapresentados, idLoja);
        }

        public enum TipoReapresentados
        {
            Todos,
            ApenasReapresentados,
            IgnorarReapresentados
        }

        /// <summary>
        /// Retorna o total em cheques de terceiros em aberto ou devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalTerc(bool emAberto, bool devolvidos, TipoReapresentados reapresentados, uint idLoja)
        {
            string situacao = String.Empty;
            if (emAberto || devolvidos)
            {
                if (emAberto)
                    situacao += ", " + (int) Cheques.SituacaoCheque.EmAberto;

                if (devolvidos)
                    situacao += ", " + (int) Cheques.SituacaoCheque.Devolvido;

                situacao = situacao.Substring(2);
            }
            else
                situacao = "0";

            string sql = "Select Coalesce(Sum(Valor), 0) From cheques Where tipo=2 and situacao in (" + situacao + ")";
            switch (reapresentados)
            {
                case TipoReapresentados.ApenasReapresentados:
                    sql += " and reapresentado=true";
                    break;
                case TipoReapresentados.IgnorarReapresentados:
                    sql += " and (reapresentado=false or reapresentado is null)";
                    break;
            }

            if (idLoja > 0)
                sql += " AND idloja=" + idLoja;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Retorna o total em cheques próprios em aberto

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o total em cheques próprios em aberto/devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalProp(uint idContaBanco)
        {
            return GetTotalProp(null, idContaBanco);
        }

        /// <summary>
        /// Retorna o total em cheques próprios em aberto/devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalProp(GDASession sessao, uint idContaBanco)
        {
            return GetTotalProp(sessao, idContaBanco, true, true);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o total em cheques próprios em aberto ou devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalProp(uint idContaBanco, bool emAberto, bool devolvidos)
        {
            return GetTotalProp(null, idContaBanco, emAberto, devolvidos);
        }

        /// <summary>
        /// Retorna o total em cheques próprios em aberto ou devolvidos.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalProp(GDASession sessao, uint idContaBanco, bool emAberto, bool devolvidos)
        {
            string situacao = String.Empty;
            if (emAberto || devolvidos)
            {
                if (emAberto)
                    situacao += ", " + (int) Cheques.SituacaoCheque.EmAberto;

                if (devolvidos)
                    situacao += ", " + (int) Cheques.SituacaoCheque.Devolvido;

                situacao = situacao.Substring(2);
            }
            else
                situacao = "0";

            string sql =
                "Select Coalesce(Sum(Valor), 0) From cheques c " +
                (idContaBanco > 0
                    ? @"left join conta_banco b on (Cast(c.banco as Char Character Set latin1)=Cast(b.nome as Char Character Set latin1) 
                    and Cast(c.conta as Char Character Set latin1)=Cast(b.conta as Char Character Set latin1)) "
                    : String.Empty) +
                "Where c.tipo=1 and c.situacao in (" + situacao + ")";

            if (idContaBanco > 0)
                sql += " and b.idContaBanco=" + idContaBanco;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        #endregion

        #region Verifica se foi cadastrado algum cheque para o sinal do pedido

        /// <summary>
        /// Verifica se foi cadastrado algum cheque referente ao sinal do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsChequeEntrada(uint idPedido)
        {
            string sql = "Select count(*) from cheques where origem=1 and idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se existe algum cheque com os dados informados

        /// <summary>
        /// Verifica se existe algum cheque com os dados informados
        /// </summary>
        public bool ExisteCheque(GDASession session, uint idCheque)
        {
            string sql = "Select Count(*) From cheques Where idCheque=" + idCheque;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se existe algum cheque com os dados informados
        /// </summary>
        public bool ExisteCheque(string banco, string agencia, string conta, int numero)
        {
            string sql =
                "Select Count(*) From cheques Where banco=?banco and agencia=?agencia and conta=?conta and num=" +
                numero +
                " and situacao Not In (" + (int) Cheques.SituacaoCheque.Cancelado + "," +
                (int) Cheques.SituacaoCheque.Trocado + ")";

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?banco", banco),
                new GDAParameter("?agencia", agencia),
                new GDAParameter("?conta", conta)) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o cheque com o número e dígito já existe para um cliente específico
        /// </summary>
        public bool ExisteChequeDigito(uint idCliente, uint idCheque, int numero, string digitoNum)
        {
            return ExisteChequeDigito(null, idCliente, idCheque, numero, digitoNum);
        }

        /// <summary>
        /// Verifica se o cheque com o número e dígito já existe para um cliente específico
        /// </summary>
        public bool ExisteChequeDigito(GDASession sessao, uint idCliente, uint idCheque, int numero, string digitoNum)
        {
            string sql = "Select Count(*) From cheques Where digitoNum=?digitoNum and num=" + numero + " and idcliente=" +
                         idCliente +
                         " and situacao Not In (" + (int) Cheques.SituacaoCheque.Cancelado + "," +
                         (int) Cheques.SituacaoCheque.Trocado + ")";

            // Usado nos métodos de atualização, para não verificar o próprio cheque sendo atualizado
            if (idCheque > 0)
                sql += " And idCheque<>" + idCheque;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?digitoNum", digitoNum)) > 0;
        }

        #endregion

        #region Busca cheques pelos ids passados

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna os cheques que possuir os ids passados, ordenados decrescentemente pelo valor.
        /// </summary>
        /// <param name="idsCheques">IDs dos cheques separados por ",". Ex.: 1,2,3.</param>
        /// <returns></returns>
        public Cheques[] GetByPks(string idsCheques)
        {
            return GetByPks(null, idsCheques);
        }

        /// <summary>
        /// Retorna os cheques que possuir os ids passados, ordenados decrescentemente pelo valor.
        /// </summary>
        public Cheques[] GetByPks(GDASession sessao, string idsCheques)
        {
            return
                objPersistence.LoadData(sessao,
                    "Select * From cheques where idCheque In (" + idsCheques.TrimEnd(' ').TrimEnd(',') +
                    ") Order By valor Desc").ToList().ToArray();
        }

        #endregion

        #region Excluir cheques pela PK

        /// <summary>
        /// Exclui cheques pelas suas PKs
        /// </summary>
        public void DeleteByPKs(List<Cheques> lstCheques)
        {
            if (lstCheques.Count == 0)
                return;

            string pks = lstCheques.Aggregate(String.Empty, (current, c) => current + (c.IdCheque + ","));

            // Não apaga os cheques
            //objPersistence.ExecuteCommand("Delete from cheques where idCheque In (" + pks.TrimEnd(',') + ")"  + " And situacao=" + (int)Cheques.SituacaoCheque.EmAberto);

            objPersistence.ExecuteCommand("update cheques set situacao=" + (int) Cheques.SituacaoCheque.Cancelado +
                                          " where idCheque In (" + pks.TrimEnd(',') + ")" + " And situacao=" +
                                          (int) Cheques.SituacaoCheque.EmAberto);
        }

        #endregion

        #region Altera a situação do cheque

        /// <summary>
        /// Atualiza a situação do cheque
        /// </summary>
        public void UpdateSituacao(GDASession sessao, uint idCheque, Cheques.SituacaoCheque situacao)
        {
            if (idCheque == 0)
                return;

            var chequeNovo = GetElementByPrimaryKey(sessao, idCheque);
            chequeNovo.Situacao = (int) situacao;

            objPersistence.ExecuteCommand(sessao,
                "Update cheques Set Situacao=" + (int) situacao + " Where idCheque=" + idCheque);

            LogAlteracaoDAO.Instance.LogCheque(sessao, chequeNovo, LogAlteracaoDAO.SequenciaObjeto.Novo);
        }

        /// <summary>
        /// Atualiza a situação dos cheques passados
        /// </summary>
        public void UpdateSituacao(GDASession sessao, string idsCheque, Cheques.SituacaoCheque situacao)
        {
            idsCheque = idsCheque.TrimEnd(' ').TrimEnd('|').TrimEnd(';').Replace('|', ',');

            if (idsCheque == String.Empty)
                return;

            foreach (var id in idsCheque.Split(','))
                UpdateSituacao(sessao, id.StrParaUint(), situacao);
        }

        #endregion

        #region Cancela cheques utilizados em acertos de cheque próprio devolvido

        /// <summary>
        /// Cancela cheques utilizados em pagto
        /// </summary>
        public void CancelaChequesProprioAcertoChequeProprioDevolvido(GDASession session, int idAcertoCheque, Cheques.SituacaoCheque situacao)
        {
            var chequesProprio = objPersistence.LoadData(session,
                string.Format("SELECT * FROM cheques WHERE Tipo=1 AND IdAcertoCheque={0}", idAcertoCheque)).ToList();

            // Usado para evitar bloqueio do índice no caixa geral.
            var contadorDataUnica = 0;

            // Estorna cheques próprios utilizados no pagamento
            foreach (var c in chequesProprio)
            {
                // Cancela cheque no caixa geral (Mesmo se tiver em aberto, gerar mov de saída no cx geral)
                // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral de cheque de terceiros
                var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, 0, idAcertoCheque, null, (uint)c.IdFornecedor,
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoChequeProprio), 1,
                    c.Valor, 0, null, null, 0, false, null);

                objPersistence.ExecuteCommand(session,
                    string.Format(
                        "UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                        contadorDataUnica++, idCaixaGeral));

                if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                {
                    // Pega movimentação referente ao cheque.
                    var obj =
                        objPersistence.ExecuteScalar(session,
                            string.Format("SELECT IdMovBanco FROM mov_banco WHERE IdCheque={0} ORDER BY IdMovBanco ASC LIMIT 1", c.IdCheque));
                    var idMovBanco = obj != null && !string.IsNullOrEmpty(obj.ToString()) ?
                        Conversoes.StrParaUint(obj.ToString()) : 0;

                    try
                    {
                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, c.IdContaBanco.GetValueOrDefault(),
                            MovBancoDAO.Instance.ObtemDataMov(session, idMovBanco));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Falha ao marcar cheque como Cancelado. {0}", ex.Message));
                    }

                    // Antes de apagar o registro da movimentação o salvo deve ser corrigido.
                    objPersistence.ExecuteCommand(session,
                        string.Format("UPDATE mov_banco SET ValorMov=0 WHERE IdCheque={0} AND TipoMov=2", c.IdCheque));

                    // Corrige saldo
                    MovBancoDAO.Instance.CorrigeSaldo(session, idMovBanco, idMovBanco);

                    // Exclui movimentações que este cheque gerou
                    objPersistence.ExecuteCommand(session,
                        string.Format("DELETE FROM mov_banco WHERE IdCheque={0} AND TipoMov=2", c.IdCheque));
                }
            }

            // Cancela os cheques
            objPersistence.ExecuteCommand(session,
                string.Format("UPDATE cheques SET Situacao={0} WHERE Tipo=1 AND IdAcertoCheque={1}",
                    (int)Cheques.SituacaoCheque.Cancelado, idAcertoCheque));
        }

        #endregion

        #region Cancela cheques utilizados em pagto

        /// <summary>
        /// Cancela cheques utilizados em pagto
        /// </summary>
        public KeyValuePair<uint[], uint[]> CancelaChequesPagto(GDASession session, uint idPagto, int tipo, Cheques.SituacaoCheque situacao)
        {
            List<uint> lstCaixa = new List<uint>(), lstMov = new List<uint>();

            // Exclui cheques próprios utilizados neste pagamento
            if (tipo == 1)
            {
                var lstChequeProprio = objPersistence.LoadData(session, "Select * From cheques Where tipo=1" +
                    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" +
                    idPagto + ")").ToList();

                // Usado para evitar bloqueio do índice no caixa geral.
                var contadorDataUnica = 0;

                // Estorna cheques próprios utilizados no pagamento
                foreach (Cheques c in lstChequeProprio)
                {
                    // Cancela cheque no caixa geral (Mesmo se tiver em aberto, gerar mov de saída no cx geral)
                    // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral de cheque de terceiros
                    var idCaixaGeral = CaixaGeralDAO.Instance.MovCxPagto(session, idPagto, null, (uint)c.IdFornecedor,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoChequeProprio), 1,
                        c.Valor - c.JurosPagto - c.MultaPagto, c.JurosPagto + c.MultaPagto, null, null, 0, false, null);

                    objPersistence.ExecuteCommand(session,
                        string.Format(
                            "UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                            contadorDataUnica++, idCaixaGeral));

                    if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                    {
                        // Pega movimentação referente ao cheque.
                        var obj =
                            objPersistence.ExecuteScalar(session, "Select idMovBanco From mov_banco Where idCheque=" + c.IdCheque +
                                " Order By idMovBanco Asc Limit 1");
                        var idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString())
                            ? Glass.Conversoes.StrParaUint(obj.ToString())
                            : 0;

                        try
                        {
                            // Verifica a conciliação bancária
                            ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, c.IdContaBanco.GetValueOrDefault(),
                                MovBancoDAO.Instance.ObtemDataMov(session, idMovBanco));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Falha ao marcar cheque como Cancelado. " + ex.Message);
                        }

                        // Antes de apagar o registro da movimentação o salvo deve ser corrigido.
                        objPersistence.ExecuteCommand(session, "Update mov_banco Set valorMov=0 Where idCheque=" + c.IdCheque +
                            " And tipoMov=2");

                        // Corrige saldo
                        MovBancoDAO.Instance.CorrigeSaldo(session, idMovBanco, idMovBanco);

                        // Exclui movimentações que este cheque gerou
                        objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idCheque=" + c.IdCheque +
                            " And tipoMov=2");
                    }
                }

                // Cancela os cheques
                objPersistence.ExecuteCommand(session, "Update cheques Set situacao=" + (int)Cheques.SituacaoCheque.Cancelado +
                    " Where tipo=" + tipo +
                    //" and situacao<>" + (int)Cheques.SituacaoCheque.EmAberto +
                    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" +
                    idPagto + ")");

                // Não apaga os cheques
                //objPersistence.ExecuteCommand("Delete From cheques Where tipo=" + tipo + " and situacao=" + (int)Cheques.SituacaoCheque.EmAberto +
                //    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" + idPagto + ")");
            }
            else
            {
                // Reabre cheques de terceiros utilizados neste pagamento, a menos que esteja cancelado ou protestado
                objPersistence.ExecuteCommand(session, "Update cheques Set Situacao=" + (int)situacao +
                    " Where tipo=" + tipo +
                    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" +
                    idPagto + ") " +
                    "And Situacao=" + (int)Cheques.SituacaoCheque.Compensado);

                // Desassocia os cheques de terceiro associados à este pagto
                objPersistence.ExecuteCommand(session, @"delete from pagto_cheque Where idPagto=" + idPagto +
                    " And idCheque in (select idCheque from cheques where tipo=" + tipo + ")");
            }

            return new KeyValuePair<uint[], uint[]>(lstCaixa.ToArray(), lstMov.ToArray());
        }

        #endregion

        #region Cancela cheques utilizados em antecipação de fornecedor
        
        /// <summary>
        /// Cancela cheques utilizados em antecipação de fornecedor
        /// </summary>
        public void CancelaChequesAntecipFornec(GDASession session, uint idAntecipFornec, int tipo, Cheques.SituacaoCheque situacao)
        {
            // Exclui cheques próprios utilizados nesta antecipação de fornecedor
            if (tipo == 1)
            {
                List<Cheques> lstChequeProprio = objPersistence.LoadData(session, "Select * From cheques Where tipo=1" +
                    " And idCheque In (Select idCheque from pagto_cheque Where idAntecipFornec=" +
                    idAntecipFornec + ")").ToList();

                // Estorna cheques próprios utilizados na antecipação de fornecedor
                foreach (Cheques c in lstChequeProprio)
                {
                    // Cancela cheque no caixa geral (Mesmo se tiver em aberto, gerar mov de saída no cx geral)
                    // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral de cheque de terceiros
                    CaixaGeralDAO.Instance.MovCxAntecipFornec(session, idAntecipFornec, (uint) c.IdFornecedor,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoAntecipFornecChequePropio),
                        1,
                        c.Valor, null, 0, false, null);

                    if (c.Situacao == (int) Cheques.SituacaoCheque.Compensado)
                    {
                        // Pega a primeira movimentação da conta bancária que esta antecipação foi feita apenas para alterar o saldo
                        object obj =
                            objPersistence.ExecuteScalar(session, "Select idMovBanco from mov_banco Where idContaBanco=" +
                                                         c.IdContaBanco + " order by idMovBanco asc limit 1");
                        uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString())
                            ? Glass.Conversoes.StrParaUint(obj.ToString())
                            : 0;

                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idMovBanco);

                        MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(session, idMovBanco);

                        // Corrige saldo
                        objPersistence.ExecuteCommand(session, "Update mov_banco Set valorMov=0 Where idCheque=" + c.IdCheque +
                                                      " And tipoMov=2");
                        if (movAnterior != null) MovBancoDAO.Instance.CorrigeSaldo(session, movAnterior.IdMovBanco, idMovBanco);

                        // Exclui movimentações que este cheque gerou
                        objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idCheque=" + c.IdCheque +
                                                      " And tipoMov=2");
                    }
                }

                // Cancela os cheques
                objPersistence.ExecuteCommand(session, "Update cheques Set situacao=" + (int) Cheques.SituacaoCheque.Cancelado +
                                              " Where tipo=" + tipo +
                                              //" and situacao<>" + (int)Cheques.SituacaoCheque.EmAberto +
                                              " And idCheque In (Select idCheque from pagto_cheque Where idAntecipFornec=" +
                                              idAntecipFornec + ")");

                // Não apaga os cheques
                //objPersistence.ExecuteCommand("Delete From cheques Where tipo=" + tipo + " and situacao=" + (int)Cheques.SituacaoCheque.EmAberto +
                //    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" + idPagto + ")");
            }
            else
            {
                // Reabre cheques de terceiros utilizados neste pagamento, a menos que esteja cancelado ou protestado
                objPersistence.ExecuteCommand(session, "Update cheques Set Situacao=" + (int) situacao +
                                              " Where tipo=" + tipo +
                                              " And idCheque In (Select idCheque from pagto_cheque Where idAntecipFornec=" +
                                              idAntecipFornec + ") " +
                                              "And Situacao=" + (int) Cheques.SituacaoCheque.Compensado);

                // Desassocia os cheques de terceiro associados à este pagto
                objPersistence.ExecuteCommand(session, @"delete from pagto_cheque Where idAntecipFornec=" + idAntecipFornec +
                                              " And idCheque in (select idCheque from cheques where tipo=" + tipo + ")");
            }
        }

        #endregion

        #region Cancela cheques utilizados em sinal da compra

        /// <summary>
        /// Cancela cheques utilizados em sinal da compra
        /// </summary>
        public void CancelaChequesSinalCompra(GDASession session, uint idSinalCompra, int tipo, Cheques.SituacaoCheque situacao)
        {
            // Exclui cheques próprios utilizados
            if (tipo == 1)
            {
                List<Cheques> lstChequeProprio = objPersistence.LoadData(session, "Select * From cheques Where tipo=1" +
                                                                         " And idCheque In (Select idCheque from pagto_cheque Where idSinalCompra=" +
                                                                         idSinalCompra + ")").ToList();

                // Estorna cheques próprios utilizados no pagamento
                foreach (Cheques c in lstChequeProprio)
                {
                    // Cancela cheque no caixa geral (Mesmo se tiver em aberto, gerar mov de saída no cx geral)
                    // a forma de pagto deve ser 0 (zero), para que não atrapalhe o cálculo feito no caixa geral de cheque de terceiros
                    CaixaGeralDAO.Instance.MovCxSinalCompra(session, idSinalCompra, null, (uint) c.IdFornecedor,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoChequeProprio), 1,
                        c.Valor, null, 0, false, null);

                    if (c.Situacao == (int) Cheques.SituacaoCheque.Compensado)
                    {
                        // Pega a primeira movimentação da conta bancária que esta antecipação foi feita apenas para alterar o saldo
                        object obj =
                            objPersistence.ExecuteScalar(session, "Select idMovBanco from mov_banco Where idContaBanco=" +
                                                         c.IdContaBanco + " order by idMovBanco asc limit 1");
                        uint idMovBanco = obj != null && !String.IsNullOrEmpty(obj.ToString())
                            ? Glass.Conversoes.StrParaUint(obj.ToString())
                            : 0;

                        // Verifica a conciliação bancária
                        ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(session, idMovBanco);

                        MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(session, idMovBanco);

                        // Corrige saldo
                        objPersistence.ExecuteCommand(session, "Update mov_banco Set valorMov=0 Where idCheque=" + c.IdCheque +
                                                      " And tipoMov=2");
                        if (movAnterior != null) MovBancoDAO.Instance.CorrigeSaldo(session, movAnterior.IdMovBanco, idMovBanco);

                        // Exclui movimentações que este cheque gerou
                        objPersistence.ExecuteCommand(session, "Delete From mov_banco Where idCheque=" + c.IdCheque +
                                                      " And tipoMov=2");
                    }
                }

                // Cancela os cheques
                objPersistence.ExecuteCommand(session, "Update cheques Set situacao=" + (int) Cheques.SituacaoCheque.Cancelado +
                                              " Where tipo=" + tipo +
                                              //" and situacao<>" + (int)Cheques.SituacaoCheque.EmAberto +
                                              " And idCheque In (Select idCheque from pagto_cheque Where idSinalCompra=" +
                                              idSinalCompra + ")");

                // Não apaga os cheques
                //objPersistence.ExecuteCommand("Delete From cheques Where tipo=" + tipo + " and situacao=" + (int)Cheques.SituacaoCheque.EmAberto +
                //    " And idCheque In (Select idCheque from pagto_cheque Where idPagto=" + idPagto + ")");
            }
            else
            {
                // Reabre cheques de terceiros utilizados neste pagamento, a menos que esteja cancelado ou protestado
                objPersistence.ExecuteCommand(session, "Update cheques Set Situacao=" + (int) situacao +
                                              " Where tipo=" + tipo +
                                              " And idCheque In (Select idCheque from pagto_cheque Where idSinalCompra=" +
                                              idSinalCompra + ") " +
                                              "And Situacao=" + (int) Cheques.SituacaoCheque.Compensado);

                // Desassocia os cheques de terceiro associados à este pagto
                objPersistence.ExecuteCommand(session, @"delete from pagto_cheque Where idSinalCompra=" + idSinalCompra +
                                              " And idCheque in (select idCheque from cheques where tipo=" + tipo + ")");
            }
        }

        #endregion

        #region Quitar Cheque Devolvido
        
        private static readonly object _quitarChequeDevolvidoLock = new object();
        
        public uint QuitarChequeDevolvido(uint[] idCheque, DateTime dataRecebido, uint[] formasPagto, decimal[] valoresReceb,
            uint[] tiposCartao, uint[] idContasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, decimal juros, string numAutConstrucard,
            bool recebParcial, uint[] numParcCartoes, string chequesPagto, bool gerarCredito, decimal creditoUtilizado, uint idCliente,
            decimal desconto, bool isChequeProprio, string obs, bool caixaDiario, string[] numAutCartao)
        {
            lock(_quitarChequeDevolvidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var podePagar = true;

                        foreach (var i in idCheque)
                        {
                            var situacaoCheque = ObterSituacao(transaction, (int)i);
                            
                            /* Chamado 37009. */
                            if (situacaoCheque == 2 || situacaoCheque == 4 || situacaoCheque == 5)
                                throw new Exception(string.Format("O cheque {0} não pode ser quitado porque está compensado, foi cancelado ou já foi quitado.",
                                    ObtemNumCheque(transaction, i)));

                            if (!podePagar)
                                break;

                            podePagar = false;

                            var idClienteCheque = GetElement(transaction, i).IdCliente;

                            if (idClienteCheque == idCliente || idClienteCheque == null)
                            {
                                podePagar = true;
                                continue;
                            }
                            else
                            {
                                var idsVinculados = ClienteVinculoDAO.Instance.GetIdsVinculados(transaction, idCliente);
                                var listaIdsVinculados = idsVinculados != "" ? idsVinculados.Split(',') : new string[0];

                                foreach (var idVinc in listaIdsVinculados)
                                {
                                    if (Conversoes.StrParaUintNullable(idVinc) == idClienteCheque)
                                    {
                                        podePagar = true;
                                        continue;
                                    }
                                }
                            }
                        }

                        if (!podePagar)
                            throw new Exception(
                                "Cliente Selecionado para pagamento não está vinculado com os clientes dos cheques");

                        // Busca os cheques
                        string idsCheques = "";
                        foreach (uint id in idCheque)
                            idsCheques += "," + id;

                        var cheques = GetByPks(transaction, idsCheques.Substring(1));

                        decimal totalPago = 0;
                        foreach (var valor in valoresReceb)
                            totalPago += valor;

                        // Se for pago com crédito, soma o mesmo ao totalPago
                        if (creditoUtilizado > 0)
                            totalPago += creditoUtilizado;

                        decimal valorAReceber = juros;
                        foreach (var c in cheques)
                            valorAReceber += c.ValorRestante;

                        var totalRestante = valorAReceber - juros;
                        // Desconsidera o desconto
                        valorAReceber -= desconto;

                        // Ignora os juros dos cartões ao calcular o valor pago/a pagar
                        totalPago -= UtilsFinanceiro.GetJurosCartoes(transaction, UserInfo.GetUserInfo.IdLoja, valoresReceb,
                            formasPagto, tiposCartao, numParcCartoes);

                        /* Chamado 18310. */
                        if (idCliente == 0 && !isChequeProprio)
                            throw new Exception("Selecione o cliente para continuar.");

                        // Mesmo se for recebimento parcial, não é permitido receber valor maior do que o valor do cheque
                        if (recebParcial && !gerarCredito)
                        {
                            if (Math.Round(totalPago, 2) > Math.Round(valorAReceber, 2))
                                throw new Exception("Valor informado excede o valor a ser quitado.");
                        }
                        // Se o total a ser pago for diferente do valor pago
                        else if (gerarCredito && Math.Round(totalPago, 2) < Math.Round(valorAReceber, 2))
                            throw new Exception("Valor a ser quitado não confere com valor informado. Valor a ser quitado: " +
                                Math.Round(valorAReceber, 2).ToString("C") + " Valor informado: " + Math.Round(totalPago, 2).ToString("C"));
                        else if (!gerarCredito && Math.Round(valorAReceber, 2) != Math.Round(totalPago, 2))
                            throw new Exception("Valor a ser quitado não confere com valor informado. Valor a ser quitado: " +
                                Math.Round(valorAReceber, 2).ToString("C") + " Valor informado: " + Math.Round(totalPago, 2).ToString("C"));

                        // Se o valor pago for menor ou igual que o valor do juros o recebimento não é validado.
                        if (juros >= totalPago)
                            throw new Exception("O valor total pago não pode ser igual ou menor do que o valor do juros.");

                        foreach (Cheques c in cheques)
                            if (c.Valor == c.ValorReceb && c.Situacao != 1 && c.Situacao != 3)
                                throw new Exception("O cheque Número " + c.Num + " Banco " + c.Banco + " Conta " + c.Conta + " já foi quitado ou trocado.");

                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        // Se não for financeiro, não pode quitar cheque
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para quitar cheques.");

                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        uint idAcertoCheque = 0;
                        var tipoReceb = !isChequeProprio ?
                            UtilsFinanceiro.TipoReceb.ChequeDevolvido : UtilsFinanceiro.TipoReceb.ChequeProprioDevolvido;

                        AcertoCheque acertoCheque = new AcertoCheque();
                        acertoCheque.IdCliente = idCliente;
                        acertoCheque.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, idCliente);
                        acertoCheque.Desconto = desconto;
                        acertoCheque.IdAcertoCheque = AcertoChequeDAO.Instance.Insert(transaction, acertoCheque);
                        acertoCheque.IdFunc = UserInfo.GetUserInfo.CodUser;
                        idAcertoCheque = acertoCheque.IdAcertoCheque;

                        retorno = UtilsFinanceiro.Receber(transaction, UserInfo.GetUserInfo.IdLoja, null, null, null, null, null, null,
                            null, null, idAcertoCheque, null, null, idCliente, 0, null, dataRecebido.ToString("dd/MM/yyyy"), valorAReceber,
                            totalPago, valoresReceb, formasPagto, idContasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao, null, null, juros,
                            recebParcial, gerarCredito, creditoUtilizado, numAutConstrucard, caixaDiario, numParcCartoes, chequesPagto,
                            false, !isChequeProprio ?
                                UtilsFinanceiro.TipoReceb.ChequeDevolvido : UtilsFinanceiro.TipoReceb.ChequeProprioDevolvido);

                        if (retorno.ex != null)
                            throw retorno.ex;

                        acertoCheque.DataAcerto = DateTime.Now;
                        acertoCheque.CreditoGeradoCriar = retorno.creditoGerado;
                        acertoCheque.CreditoUtilizadoCriar = creditoUtilizado;
                        acertoCheque.Obs = obs;
                        AcertoChequeDAO.Instance.Update(transaction, acertoCheque);

                        #region Cadastra os cheques na tabela do acerto

                        foreach (uint id in idCheque)
                        {
                            ItemAcertoCheque novo = new ItemAcertoCheque();
                            novo.IdAcertoCheque = idAcertoCheque;
                            novo.IdCheque = id;

                            ItemAcertoChequeDAO.Instance.Insert(transaction, novo);
                        }

                        #endregion

                        #region Cadastra os pagamentos na tabela

                        int numPagto = 0;

                        for (int i = 0; i < valoresReceb.Length; i++)
                        {
                            if (valoresReceb[i] > 0 && formasPagto[i] > 0)
                            {
                                if (formasPagto.Length > i && formasPagto[i] == (int)Data.Model.Pagto.FormaPagto.CartaoNaoIdentificado)
                                {
                                    var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);

                                    foreach (var cni in CNIs)
                                    {
                                        PagtoAcertoCheque novo = new PagtoAcertoCheque();
                                        novo.IdAcertoCheque = idAcertoCheque;
                                        novo.IdFormaPagto = formasPagto[i];
                                        novo.IdTipoCartao = (uint)cni.TipoCartao;
                                        novo.NumFormaPagto = ++numPagto;
                                        novo.ValorPagto = cni.Valor;
                                        novo.IdContaBanco = (uint)cni.IdContaBanco;
                                        novo.NumAutCartao = cni.NumAutCartao;

                                        PagtoAcertoChequeDAO.Instance.Insert(transaction, novo);
                                    }
                                }
                                else
                                {
                                    PagtoAcertoCheque novo = new PagtoAcertoCheque();
                                    novo.IdAcertoCheque = idAcertoCheque;
                                    novo.IdFormaPagto = formasPagto[i];
                                    novo.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;
                                    novo.NumFormaPagto = ++numPagto;
                                    novo.ValorPagto = valoresReceb[i];
                                    novo.IdContaBanco = idContasBanco[i];
                                    novo.NumAutCartao = numAutCartao[i];

                                    PagtoAcertoChequeDAO.Instance.Insert(transaction, novo);
                                }
                            }
                        }

                        if (creditoUtilizado > 0)
                        {
                            PagtoAcertoCheque novo = new PagtoAcertoCheque();
                            novo.IdAcertoCheque = idAcertoCheque;
                            novo.IdFormaPagto = (uint)Pagto.FormaPagto.Credito;
                            novo.NumFormaPagto = ++numPagto;
                            novo.ValorPagto = creditoUtilizado;

                            PagtoAcertoChequeDAO.Instance.Insert(transaction, novo);
                        }

                        #endregion

                        decimal valorAcumulado = totalPago;

                        if (juros > 0)
                        {
                            decimal jurosRateado = Math.Round(juros / cheques.Length, 2);

                            // Salva o valor de juros para cada cheque.
                            for (int i = 0; i < cheques.Length; i++)
                            {
                                valorAcumulado -= valorAcumulado > jurosRateado ? jurosRateado : valorAcumulado;
                                cheques[i].JurosReceb += jurosRateado;
                            }
                        }

                        //Rateia o desconto nos cheques
                        decimal descontoTotalRateado = 0;
                        foreach (var c in cheques)
                        {
                            c.DescontoReceb += Math.Round(c.ValorRestante / totalRestante * desconto, 2);
                            descontoTotalRateado += c.DescontoReceb;
                        }
                        if (desconto - descontoTotalRateado != 0)
                            cheques[0].DescontoReceb += desconto - descontoTotalRateado;

                        // Salva o valor recebido de cada cheque.
                        for (int i = 0; i < cheques.Length; i++)
                        {
                            decimal valorReceber = valorAcumulado > cheques[i].ValorRestante ?
                                cheques[i].ValorRestante : valorAcumulado;
                            cheques[i].ValorReceb += valorReceber;
                            valorAcumulado -= valorReceber;
                        }

                        // Usado para evitar bloqueio do índice no caixa geral.
                        var contadorDataUnica = 0;

                        // Atualiza o cheque e gera movimentação no caixa.
                        foreach (Cheques c in cheques)
                        {
                            decimal valorRecebido = ItemAcertoChequeDAO.Instance.ObtemValorCampo<decimal>(transaction,
                                "Sum(valorReceb)", "idCheque=" + c.IdCheque);
                            /* Chamado 18003. */
                            var valorReceb = Math.Max(c.ValorReceb, valorRecebido) - Math.Min(c.ValorReceb, valorRecebido);
                            // Atualiza a data do recebimento deste cheque
                            c.DataReceb = dataRecebido;

                            // Os cheques devolvidos ou protestados podem ser marcados como Advogado, neste caso ao quitar estes cheques o campo advogado deve ser atualizado para falso.
                            if (c.Situacao == (int)Cheques.SituacaoCheque.Devolvido ||
                                c.Situacao == (int)Cheques.SituacaoCheque.Protestado)
                                c.Advogado = false;

                            // Se o valor restante for igual a zero, ou se não for recebimento parcial, muda a situação do cheque para quitado
                            if ((c.ValorRestante == 0 || !recebParcial) &&
                                (c.Situacao == (int)Cheques.SituacaoCheque.Devolvido ||
                                c.Situacao == (int)Cheques.SituacaoCheque.Protestado))
                                c.Situacao = (int)Cheques.SituacaoCheque.Quitado;
                            else if (c.Situacao == (int)Cheques.SituacaoCheque.EmAberto)
                                c.Situacao = (int)Cheques.SituacaoCheque.Trocado;

                            // Gera movimentação de saída deste cheque no caixa pois ao marcar o mesmo como devolvido, o valor dele voltou para o caixa,
                            // e como ele está sendo quitado agora, deve sair do caixa.
                            if (c.Tipo == 2 &&
                                (c.Situacao == (int)Cheques.SituacaoCheque.Trocado ||
                                c.Situacao == (int)Cheques.SituacaoCheque.Devolvido ||
                                (c.Situacao == (int)Cheques.SituacaoCheque.Quitado &&
                                FinanceiroConfig.FinanceiroRec.MovimentarCaixaAoQuitarCheque)) &&
                                /* Chamado 51808.
                                 * "c.Origem != (int)Cheques.OrigemCheque.FinanceiroPagto": a origem FinanceiroPagto é salva
                                 * quando o cheque é cadastrado avulso, ou seja, verifica se não foi cadastrado avulso.
                                 * "c.MovCaixaFinanceiro": verifica se a opção "Gerar movimentação no caixa geral" foi marcada
                                 * quando o cheque foi cadastrado de forma avulsa. */
                                (c.Origem != (int)Cheques.OrigemCheque.FinanceiroPagto || c.MovCaixaFinanceiro))
                            {
                                // Chamado 13081. A movimentação de recebimento é gerada no caixa geral, pois, existem as considerações abaixo que
                                // definem se a movimentação deve ser feita no caixa geral ou no caixa diário. As mesmas considerações devem ser levadas em
                                // conta ao gerar a movimentação do cheque, dessa forma as movimentações de saída e a entrada será lançada no mesmo lugar.
                                bool isCaixaDiario = (Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && caixaDiario);
                                var recebApenasCxGeral =
                                    tipoReceb == UtilsFinanceiro.TipoReceb.ChequeProprioDevolvido ||
                                    tipoReceb == UtilsFinanceiro.TipoReceb.CreditoValeFuncionario ||
                                    tipoReceb == UtilsFinanceiro.TipoReceb.DebitoValeFuncionario ||
                                    tipoReceb == UtilsFinanceiro.TipoReceb.DevolucaoPagto;

                                // Se o funcionário for Financeiro
                                bool isCaixaGeral =
                                    Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) ||
                                    ((tipoReceb == UtilsFinanceiro.TipoReceb.ChequeDevolvido ||
                                    tipoReceb == UtilsFinanceiro.TipoReceb.ChequeProprioDevolvido ||
                                    tipoReceb == UtilsFinanceiro.TipoReceb.ChequeProprioReapresentado) &&
                                    Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento));

                                uint idCaixaGeral = 0;
                                uint idCaixaDiario = 0;

                                // Chamado 15313: Se estiver quitando um cheque, a saída dele deverá ser sempre no caixa geral
                                if (isCaixaDiario && !recebApenasCxGeral &&
                                    tipoReceb != UtilsFinanceiro.TipoReceb.ChequeDevolvido)
                                    idCaixaDiario = CaixaDiarioDAO.Instance.MovCxAcertoCheque(transaction, c.IdLoja,
                                        c.IdCheque, null, idAcertoCheque, c.IdCliente, (uint?)c.IdFornecedor,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado), 2,
                                        valorReceb, 0, null, true, "Cheque núm. " + c.Num);
                                else if (isCaixaGeral)
                                    // Gera a movimentação no caixa geral
                                    idCaixaGeral = CaixaGeralDAO.Instance.MovCxCheque(transaction, c.IdCheque, null,
                                        idAcertoCheque, c.IdCliente, (uint?)c.IdFornecedor,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado), 2,
                                        valorReceb, 0, null, true, "Cheque núm. " + c.Num, null);
                                /* Chamado 17841. */
                                // Chamado 15313: Se estiver quitando um cheque, a saída dele deverá ser sempre no caixa geral.
                                else if (isCaixaDiario && tipoReceb == UtilsFinanceiro.TipoReceb.ChequeDevolvido)
                                    idCaixaGeral = CaixaGeralDAO.Instance.MovCxCheque(transaction, c.IdCheque, null,
                                        idAcertoCheque, c.IdCliente, (uint?)c.IdFornecedor,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado), 2,
                                        valorReceb, 0, null, true, "Cheque núm. " + c.Num, null);
                                else
                                    throw new Exception("Você não tem permissão para receber contas.");

                                if (idCaixaGeral > 0)
                                {
                                    objPersistence.ExecuteCommand(transaction,
                                        string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaGeral={1}",
                                            contadorDataUnica++, idCaixaGeral));

                                    retorno.idCxGeral.Add(idCaixaGeral);
                                }
                                else if (idCaixaDiario > 0)
                                {
                                    objPersistence.ExecuteCommand(transaction,
                                        string.Format("UPDATE caixa_diario SET DataUnica=CONCAT(DATAUNICA, '_{0}') WHERE IdCaixaDiario={1}",
                                            contadorDataUnica++, idCaixaDiario));

                                    retorno.idCxDiario.Add(idCaixaDiario);
                                }
                            }

                            UpdateBase(transaction, c, false);
                            ItemAcertoChequeDAO.Instance.AtualizaValorRecebCheque(transaction, idAcertoCheque, c.IdCheque, valorReceb);
                        }

                        // Atualiza o acerto do cheque
                        AcertoChequeDAO.Instance.AtualizaAcertoCheque(transaction, idAcertoCheque, idCliente, totalPago - juros, juros);

                        transaction.Commit();
                        transaction.Close();

                        return idAcertoCheque;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("Falha ao inserir acerto de cheque.", ex);
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber valor de cheque devolvido.", ex));
                    }
                }
            }
        }

        #endregion

        #region Quitar Cheque Reapresentado

        private static readonly object _quitarChequeReapresentadoLock = new object();

        public uint QuitarChequeReapresentado(uint[] idCheque, uint idCliente, uint idContaBanco, DateTime dataRecebido,
            decimal juros, decimal desconto, bool isChequeProprio, string obs)
        {
            lock(_quitarChequeReapresentadoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Busca os cheques
                        string idsCheques = idCheque.Aggregate("", (current, id) => current + ("," + id));

                        var cheques = GetByPks(transaction, idsCheques.Substring(1));
                        decimal totalCheques = 0;

                        foreach (var c in cheques)
                        {
                            totalCheques += c.ValorRestante;
                            if (c.Valor == c.ValorReceb && c.Situacao != 1 && c.Situacao != 3)
                                throw new Exception("O cheque Número " + c.Num + " Banco " + c.Banco + " Conta " + c.Conta + " já foi quitado ou trocado.");
                        }

                        var totalRestante = totalCheques;
                        // Desconsidera o desconto
                        totalCheques -= desconto;

                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        if (idCliente == 0 && !isChequeProprio)
                            throw new Exception("Selecione o cliente para continuar.");

                        // Se não for financeiro, não pode quitar cheque
                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                            throw new Exception("Você não tem permissão para quitar cheques.");

                        uint idAcertoCheque = 0;

                        AcertoCheque acertoCheque = new AcertoCheque
                        {
                            IdCliente = idCliente,
                            ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, idCliente),
                            Desconto = desconto
                        };

                        idAcertoCheque = AcertoChequeDAO.Instance.Insert(transaction, acertoCheque);

                        #region Cadastra os cheques na tabela do acerto

                        foreach (uint id in idCheque)
                        {
                            ItemAcertoCheque novo = new ItemAcertoCheque
                            {
                                IdAcertoCheque = idAcertoCheque,
                                IdCheque = id
                            };

                            ItemAcertoChequeDAO.Instance.Insert(transaction, novo);
                        }

                        #endregion

                        #region Cadastra os pagamentos na tabela

                        PagtoAcertoCheque novoPagto = new PagtoAcertoCheque
                        {
                            IdAcertoCheque = idAcertoCheque,
                            IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio,
                            NumFormaPagto = 1,
                            ValorPagto = totalCheques
                        };

                        PagtoAcertoChequeDAO.Instance.Insert(transaction, novoPagto);

                        #endregion

                        //Rateia o desconto nos cheques
                        decimal descontoTotalRateado = 0;
                        foreach (var c in cheques)
                        {
                            c.DescontoReceb += Math.Round(c.ValorRestante / totalRestante * desconto, 2);
                            descontoTotalRateado += c.DescontoReceb;
                        }
                        if (desconto - descontoTotalRateado != 0)
                            cheques[0].DescontoReceb += desconto - descontoTotalRateado;

                        foreach (var c in cheques)
                        {
                            decimal valor = c.ValorRestante;

                            // Soma valor recebido ao valor que já possa ter resolvido
                            c.ValorReceb += valor;
                            c.JurosReceb = c.JurosReceb + Math.Round(juros / cheques.Length, 2);
                            c.DataReceb = dataRecebido;

                            // Se o valor a receber for igual ao valor recebido, ou se não for recebimento parcial,
                            // muda a situação do cheque para quitado
                            if ((c.ValorReceb == c.Valor) && c.Situacao == (int)Cheques.SituacaoCheque.Devolvido)
                                c.Situacao = (int)Cheques.SituacaoCheque.Quitado;
                            else if (c.Situacao == (int)Cheques.SituacaoCheque.EmAberto)
                                c.Situacao = (int)Cheques.SituacaoCheque.Trocado;

                            UpdateBase(transaction, c, false);
                            ItemAcertoChequeDAO.Instance.AtualizaValorRecebCheque(transaction, idAcertoCheque, c.IdCheque, valor);
                        }

                        // Atualiza o acerto do cheque
                        AcertoChequeDAO.Instance.AtualizaAcertoCheque(transaction, idAcertoCheque, idCliente, totalCheques, juros);
                        
                        transaction.Commit();
                        transaction.Close();

                        return idAcertoCheque;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao receber valor de cheque devolvido.", ex));
                    }
                }
            }
        }

        #endregion

        #region Quitar Cheque Pagto

        /// <summary>
        /// Quita um cheque próprio que foi utilizado em um pagamento
        /// </summary>
        public void QuitarChequePagto(Cheques cheque)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    string dataQuitChequeProprio = cheque.DataQuitChequeProprio;

                    cheque = GetElement(transaction, cheque.IdCheque);
                    if (cheque.IdContaBanco == null || cheque.IdContaBanco == 0)
                        throw new Exception("Esse cheque não possui referência da conta bancária.");

                    // Verifica se cheque já foi quitado
                    if (ChequePagtoQuitado(transaction, cheque.IdCheque))
                        throw new Exception("Este cheque já foi quitado.");

                    // Verifica se o cheque está em aberto
                    if (cheque.Situacao != (int)Cheques.SituacaoCheque.EmAberto)
                        throw new Exception("Apenas cheques em aberto podem ser quitados.");

                    DateTime dataQuitacao = !String.IsNullOrEmpty(dataQuitChequeProprio) ? DateTime.Parse(dataQuitChequeProprio) : DateTime.Now;

                    // Para cada cheque utilizado neste pagamento, debita o valor da conta bancária associada ao mesmo
                    if (ContaBancoDAO.Instance.MovContaCheque(transaction, cheque.IdContaBanco.Value, UtilsPlanoConta.GetPlanoContaPagto(2),
                        (int)UserInfo.GetUserInfo.IdLoja, null, cheque.IdPagto, cheque.IdCheque, cheque.IdCliente,
                        null, 2, cheque.Valor - cheque.JurosPagto - cheque.MultaPagto, dataQuitacao) < 1)
                        throw new Exception("Falha ao quitar cheque. Inserção retornou 0;");

                    // O juros e a multa devem ser debitados nesta função, uma vez que não foram debitados ao inserir 
                    // o pagamento pois os cheques estavam em aberto
                    // Gera movimentação de juros
                    if (cheque.JurosPagto > 0)
                        ContaBancoDAO.Instance.MovContaCheque(transaction, cheque.IdContaBanco.Value, FinanceiroConfig.PlanoContaJurosPagto,
                            (int)UserInfo.GetUserInfo.IdLoja, null, cheque.IdPagto, cheque.IdCheque, cheque.IdCliente, null, 2, cheque.JurosPagto, dataQuitacao);

                    // Gera movimentação de multa
                    if (cheque.MultaPagto > 0)
                        ContaBancoDAO.Instance.MovContaCheque(transaction, cheque.IdContaBanco.Value, FinanceiroConfig.PlanoContaMultaPagto,
                            (int)UserInfo.GetUserInfo.IdLoja, null, cheque.IdPagto, cheque.IdCheque, cheque.IdCliente, null, 2, cheque.MultaPagto, dataQuitacao);

                    cheque.Situacao = (int)Cheques.SituacaoCheque.Compensado;
                    cheque.DataReceb = !String.IsNullOrEmpty(dataQuitChequeProprio) ? DateTime.Parse(dataQuitChequeProprio) : DateTime.Now;
                    Update(transaction, cheque);
                    
                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Verifica se cheque já foi quitado

        /// <summary>
        /// Verifica se cheque já foi quitado
        /// </summary>
        /// <param name="idCheque"></param>
        public bool ChequePagtoQuitado(uint idCheque)
        {
            return ChequePagtoQuitado(null, idCheque);
        }

        /// <summary>
        /// Verifica se cheque já foi quitado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCheque"></param>
        public bool ChequePagtoQuitado(GDASession session, uint idCheque)
        {
            uint? idContaBanco = ObtemValorCampo<uint?>(session, "idContaBanco", "idCheque=" + idCheque);

            if (idContaBanco == null || idContaBanco == 0)
                throw new Exception("Cheque não possui referência à sua conta bancária.");

            return ContaBancoDAO.Instance.ChequePagtoQuitado(session, idCheque, idContaBanco.Value);
        }

        #endregion

        #region Insere cheque a partir de uma string

        public int GetTipo(string tipo)
        {
            switch (tipo.Trim().ToLower())
            {
                case "proprio":
                case "próprio":
                case "1":
                    return 1;

                default:
                    return 2;
            }
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cria um Cheque a partir de uma string.
        /// </summary>
        /// <param name="stringCheque"></param>
        /// <returns></returns>
        public Cheques GetFromString(string stringCheque)
        {
            return GetFromString(null, stringCheque);
        }

        /// <summary>
        /// Cria um Cheque a partir de uma string.
        /// </summary>
        public Cheques GetFromString(GDASession sessao, string stringCheque)
        {
            return GetFromString(sessao, stringCheque, false);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Cria um Cheque a partir de uma string.
        /// </summary>
        /// <param name="stringCheque">Dados do cheque que será criado</param>
        /// <param name="recuperarIdCheque">Parametro que define se o id do cheque deve ser retornado ou não</param>
        /// <returns></returns>
        public Cheques GetFromString(string stringCheque, bool recuperarIdCheque)
        {
            return GetFromString(null, stringCheque, recuperarIdCheque);
        }

        /// <summary>
        /// Cria um Cheque a partir de uma string.
        /// </summary>
        public Cheques GetFromString(GDASession sessao, string stringCheque, bool recuperarIdCheque)
        {
            // Divide o cheque para pegar suas propriedades
            string[] dadosCheque = stringCheque.Split('\t');

            // Insere cheque no BD
            Cheques cheque = new Cheques
            {
                IdCheque =
                    recuperarIdCheque && !String.IsNullOrEmpty(dadosCheque[18])
                        ? dadosCheque[18].StrParaUint()
                        : 0,
                IdContaBanco = dadosCheque[1].StrParaUintNullable(),
                Num = dadosCheque[2].StrParaInt(),
                DigitoNum = dadosCheque[3],
                Titular = dadosCheque[4],
                Valor = decimal.Parse(dadosCheque[5], System.Globalization.NumberStyles.AllowDecimalPoint),
                DataVenc = DateTime.Parse(dadosCheque[6]),
                Tipo = GetTipo(dadosCheque[0]),
                Origem = dadosCheque[8].StrParaInt(),
                Situacao = dadosCheque[7].StrParaInt(),
                IdAcertoCheque = dadosCheque[9].StrParaUintNullable(),
                IdContaR = dadosCheque[10].StrParaUintNullable(),
                IdPedido = dadosCheque[11].StrParaUintNullable(),
                IdAcerto = dadosCheque[12].StrParaUintNullable(),
                IdLiberarPedido = dadosCheque[13].StrParaUintNullable(),
                IdTrocaDevolucao = dadosCheque[14].StrParaUintNullable(),
                IdSinal = dadosCheque[20].StrParaUintNullable()
            };
            cheque.IdCliente = cheque.IdContaR != null ? ContasReceberDAO.Instance.ObtemValorCampo<uint>(sessao, "idCliente", "idContaR=" + cheque.IdContaR.Value) :
                cheque.IdPedido != null ? PedidoDAO.Instance.ObtemIdCliente(sessao, cheque.IdPedido.Value) :
                cheque.IdAcerto != null ? AcertoDAO.Instance.ObtemIdCliente(sessao, cheque.IdAcerto.Value) :
                cheque.IdLiberarPedido != null ? LiberarPedidoDAO.Instance.ObtemValorCampo<uint>(sessao, "idCliente", "idLiberarPedido=" + cheque.IdLiberarPedido.Value) :
                cheque.IdAcertoCheque != null ? AcertoChequeDAO.Instance.ObtemValorCampo<uint?>(sessao, "idCliente", "idAcertoCheque=" + cheque.IdAcertoCheque.Value) :
                cheque.IdTrocaDevolucao != null ? TrocaDevolucaoDAO.Instance.ObtemValorCampo<uint>(sessao, "idCliente", "idTrocaDevolucao=" + cheque.IdTrocaDevolucao.Value) :
                cheque.IdSinal != null ? (uint?)SinalDAO.Instance.ObtemIdCliente(sessao, cheque.IdSinal.Value) : null;
            cheque.Banco = dadosCheque[15];
            cheque.Agencia = dadosCheque[16];
            cheque.Conta = dadosCheque[17];
            cheque.Obs = dadosCheque[19];
            cheque.CpfCnpj = dadosCheque[21].Replace(".", "").Replace("-", "").Replace("/", "");
            cheque.IdLoja = dadosCheque[22].StrParaUint();

            return cheque;
        }

        /// <summary>
        /// Insere um cheque a partir de uma string (usado nas telas de pagamento).
        /// </summary>
        /// <param name="stringCheque">Uma string com os dados do cheque.</param>
        /// <returns>O objeto que foi cadastrado no banco de dados.</returns>
        public Cheques InsertFromString(string stringCheque)
        {
            Cheques cheque = GetFromString(stringCheque);
            cheque.IdCheque = InsertBase(cheque);

            if (cheque.IdCheque < 1)
                throw new Exception("retorno do insert do cheque=0");

            return cheque;
        }

        #endregion

        #region Cheque Financeiro Pagto.

        /// <summary>
        /// Retorna o cheque usado no financeiro.
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public Cheques GetForFinanceiro(uint idCheque)
        {
            string sql = @"
                select c.*, coalesce(g.idConta, m.idConta) as idConta, coalesce(g.descrPlanoConta, m.descrPlanoConta) as descrPlanoConta, 
                    coalesce(g.descrContaBanco, m.descrContaBanco) as descrContaBanco
                from cheques c
                    left join (
                        select cg.idCheque, cg.idConta, concat(gc.descricao, ' - ', pc.descricao) as descrPlanoConta, null as descrContaBanco
                        from caixa_geral cg
                            left join plano_contas pc on (cg.idConta=pc.idConta)
                            left join grupo_conta gc on (pc.idGrupo=gc.idGrupo)
                            left join cheques c1 on (cg.idCheque=c1.idCheque)
                        where cg.dataMov>=date_sub(c1.dataCad, interval 5 minute)
                            and cg.dataMov<=date_add(c1.dataCad, interval 5 minute)
                    ) as g on (g.idCheque=c.idCheque)
                    left join (
                        select m.idCheque, m.idConta, concat(gc.descricao, ' - ', pc.descricao) as descrPlanoConta,
                            concat(c.nome, ' Agência: ', c.agencia, ' Conta: ', c.Conta) as descrContaBanco
                        from mov_banco m
                            left join plano_contas pc on (m.idConta=pc.idConta)
                            left join grupo_conta gc on (pc.idGrupo=gc.idGrupo)
                            left join cheques c1 on (m.idCheque=c1.idCheque)
                            left join conta_banco c on (m.idContaBanco=c.idContaBanco)
                        where m.dataMov>=date_sub(c1.dataCad, interval 5 minute)
                            and m.dataMov<=date_add(c1.dataCad, interval 5 minute)
                    ) as m on (m.idCheque=c.idCheque)
                where c.idCheque=" + idCheque;

            List<Cheques> c = objPersistence.LoadData(sql);
            return c.Count > 0 ? c[0] : null;
        }

        /// <summary>
        /// Cancela o cheque no financeiro.
        /// </summary>
        /// <param name="cheque"></param>
        public void CancelarCheque(Cheques cheque)
        {
            cheque = GetForFinanceiro(cheque.IdCheque);
            if (cheque == null)
                return;

            if (cheque.MovCaixaFinanceiro && cheque.IdConta > 0)
                if (cheque.IdConta != null)
                    CaixaGeralDAO.Instance.MovCxCheque(null, cheque.IdCheque, null, null, cheque.IdCliente, (uint)cheque.IdFornecedor, cheque.IdConta.Value, 2,
                        cheque.Valor, 0, null, true, "Cancelamento de cheque", null);

            bool cancelar = true;

            try
            {
                if (cheque.IdAcerto != null)
                    throw new Exception("Cheque utilizado no acerto " + cheque.IdAcerto.Value + ".");
                else if (cheque.IdAcertoCheque != null)
                    throw new Exception("Cheque utilizado no acerto de cheque " + cheque.IdAcertoCheque.Value + ".");
                else if (cheque.IdContaR != null)
                    throw new Exception("Cheque utilizado na conta " + cheque.IdContaR.Value + ".");
                else if (cheque.IdDeposito != null)
                {
                    cancelar = false;
                    throw new Exception("Cheque utilizado no depósito " + cheque.IdDeposito.Value + ".");
                }
                else if (cheque.IdLiberarPedido != null && LiberarPedidoDAO.Instance.ObtemValorCampo<LiberarPedido.SituacaoLiberarPedido>("situacao",
                    "idLiberarPedido=" + cheque.IdLiberarPedido.Value) != LiberarPedido.SituacaoLiberarPedido.Cancelado)
                    throw new Exception("Cheque utilizado na liberação " + cheque.IdLiberarPedido.Value + ". Cancele a liberação para continuar.");
                else if (cheque.IdPedido != null && PedidoDAO.Instance.ObtemSituacao(cheque.IdPedido.Value) != Pedido.SituacaoPedido.Cancelado)
                    throw new Exception("Cheque utilizado no pedido " + cheque.IdPedido.Value + ". Cancele o pedido para continuar.");
                else
                {
                    // Não apaga o cheque, apenas marca como cancelado
                    //Delete(cheque);

                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                if (cancelar)
                    objPersistence.ExecuteCommand("update cheques set situacao=" + (int)Cheques.SituacaoCheque.Cancelado + " where idCheque=" + cheque.IdCheque);
                else
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar cheque.", ex));
            }
        }

        #endregion

        #region Alterar dados do cheque

        /// <summary>
        /// Altera dados do cheque.
        /// </summary>
        /// <param name="c"></param>
        public void AlterarDados(Cheques c)
        {
            Cheques c1 = GetElementByPrimaryKey(c.IdCheque);

            string sql = "update cheques set dataVencOriginal=if(dataVencOriginal is not null, dataVencOriginal, dataVenc), " +
                (c.EditarAgenciaConta ? "agencia=?agencia, conta=?conta, " : "") +
                "dataVenc=?dataVenc, obs=?obs" + (!String.IsNullOrEmpty(c.Titular) ? ", titular=?titular" : "") +
                " where idCheque=" + c.IdCheque;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?dataVenc", c.DataVenc), new GDAParameter("?agencia", c.Agencia),
                new GDAParameter("?conta", c.Conta), new GDAParameter("?obs", c.Obs), new GDAParameter("?titular", c.Titular));

            LogAlteracaoDAO.Instance.LogCheque(c1, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        /// <summary>
        /// Altera a conta bancária do cheque.
        /// </summary>
        /// <param name="idCheque"></param>
        /// <param name="idContaBanco"></param>
        public void AlteraContaBancoCheque(uint idCheque, uint idContaBanco)
        {
            Cheques c1 = GetElementByPrimaryKey(idCheque);

            string sql = "update cheques set idContaBanco=" + idContaBanco + " where idCheque=" + idCheque;
            objPersistence.ExecuteCommand(sql);

            LogAlteracaoDAO.Instance.LogCheque(c1, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Marca cheques como reapresentados

        /// <summary>
        /// Marca um cheque como Reapresentado.
        /// </summary>
        public void ReapresentarCheque(uint idCheque, DateTime dataReapresentar)
        {
            FilaOperacoes.ReapresentarCheque.AguardarVez();
            
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    Cheques c = GetElementByPrimaryKey(transaction, idCheque);
                    if (c.IdContaBanco == null || c.IdContaBanco == 0)
                        throw new Exception("Esse cheque não possui a referência da conta bancária.");

                    // Se o cheque já estiver reapresentado, apenas não faz nada.
                    if (c.Reapresentado)
                        return;

                    DepositoChequeDAO.Instance.CompensaCheques(transaction, idCheque.ToString(), 0, c.Valor, c.Valor, 0, c.IdContaBanco.Value, dataReapresentar, c.Tipo);

                    // Os cheques devolvidos ou protestados podem ser marcados como Advogado, neste caso ao marcar o cheque como reapresentado significa
                    // que o cheque não está mais nas mãos do advogado e sendo assim o campo deve ser atualizado para falso.
                    objPersistence.ExecuteCommand(transaction, "update cheques set reapresentado=true, advogado=false where idCheque=" + idCheque);

                    LogAlteracaoDAO.Instance.LogCheque(transaction, c, LogAlteracaoDAO.SequenciaObjeto.Atual);
                    
                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.ReapresentarCheque.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Devolve um cheque reapresentado.
        /// </summary>
        public void DevolverChequeReapresentado(uint idCheque, string obs, DateTime data)
        {
            Cheques c = GetElementByPrimaryKey(idCheque);

            DepositoChequeDAO.Instance.ChequeDevolvido(idCheque, obs, data);
            objPersistence.ExecuteCommand("update cheques set reapresentado=false where idCheque=" + idCheque);

            LogAlteracaoDAO.Instance.LogCheque(c, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        /// <summary>
        /// Protesta um cheque reapresentado.
        /// </summary>
        public void ProtestarChequeReapresentado(uint idCheque, string obs)
        {
            Cheques c = GetElementByPrimaryKey(idCheque);

            DepositoChequeDAO.Instance.ChequeProtestado(idCheque, obs);
            objPersistence.ExecuteCommand("update cheques set reapresentado=false where idCheque=" + idCheque);

            LogAlteracaoDAO.Instance.LogCheque(c, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Marcar/Desmarcar cheque Advogado

        /// <summary>
        /// Marca um cheque como enviado ao advogado.
        /// </summary>
        /// <param name="idCheque"></param>
        public void MarcarAdvogado(uint idCheque)
        {
            Cheques c = GetElementByPrimaryKey(idCheque);

            objPersistence.ExecuteCommand("Update cheques Set advogado=True Where idCheque=" + idCheque);
            LogAlteracaoDAO.Instance.LogCheque(c, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        /// <summary>
        /// Desmarca um cheque como enviado ao advogado.
        /// </summary>
        /// <param name="idCheque"></param>
        public void DesmarcarAdvogado(uint idCheque)
        {
            Cheques c = GetElementByPrimaryKey(idCheque);

            objPersistence.ExecuteCommand("Update cheques Set advogado=False Where idCheque=" + idCheque);
            LogAlteracaoDAO.Instance.LogCheque(c, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Retorna campos dos cheques

        /// <summary>
        /// Retorna número do cheque
        /// </summary>
        public int ObtemNumCheque(uint idCheque)
        {
            return ObtemNumCheque(null, idCheque);
        }

        /// <summary>
        /// Retorna número do cheque
        /// </summary>
        public int ObtemNumCheque(GDASession session, uint idCheque)
        {
            return ObtemValorCampo<int>(session, "num", "idCheque=" + idCheque);
        }

        /// <summary>
        /// Obtém o idCliente do cheque
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idCheque)
        {
            return ObtemValorCampo<uint>("idCliente", "idCheque=" + idCheque);
        }

        /// <summary>
        /// Obtem o valor do cheque.
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public decimal ObtemValor(uint idCheque)
        {
            return ObtemValor(null, idCheque);
        }

        /// <summary>
        /// Obtem o valor do cheque.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public decimal ObtemValor(GDASession session, uint idCheque)
        {
            return ObtemValorCampo<decimal>(session, "valor", "idCheque=" + idCheque);
        }
        
        /// <summary>
        /// Obtem a situação do cheque.
        /// </summary>
        public decimal ObterSituacao(GDASession session, int idCheque)
        {
            return ObtemValorCampo<decimal>(session, "situacao", string.Format("IdCheque={0}", idCheque));
        }

        /// <summary>
        /// Informa se este cliente possui cheques devolvidos
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public int ObtemQtdChequeDevolvido(uint idCliente)
        {
            if (idCliente == 0)
                return 0;

            string sql = "Select Count(*) From cheques Where idCliente=" + idCliente +
                " And (reapresentado is null or reapresentado=false) And situacao=" + (int)Cheques.SituacaoCheque.Devolvido;

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Informa se este cliente possui cheques protestados
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public int ObtemQtdChequeProtestado(uint idCliente)
        {
            if (idCliente == 0)
                return 0;

            string sql = "Select Count(*) From cheques Where idCliente=" + idCliente + " And situacao=" + (int)Cheques.SituacaoCheque.Protestado;

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Retorna as notas fiscais associadas ao pedido ou liberação do cheque, separadas por vírgula.
        /// </summary>
        public string ObtemIdsNfRecebimento(uint? idLiberarPedido, uint? idPedido, uint? idAcerto)
        {
            if (idPedido.GetValueOrDefault(0) == 0 && idLiberarPedido.GetValueOrDefault(0) == 0 && idAcerto.GetValueOrDefault(0) == 0)
                return String.Empty;

            // Busca os idLiberarPedido através do acerto
            string idsLiberarPedido = PedidoConfig.LiberarPedido && idAcerto > 0 && idLiberarPedido == null && idPedido == null ?
                ExecuteScalar<string>("Select cast(group_concat(idLiberarPedido) as char) From contas_receber Where idAcerto=" + idAcerto) :
                String.Empty;

            // Busca os idPedido através do acerto
            string idsPedido = !PedidoConfig.LiberarPedido && idAcerto > 0 && idLiberarPedido == null && idPedido == null ?
                ExecuteScalar<string>("Select cast(group_concat(idPedido) as char) From contas_receber Where idAcerto=" + idAcerto) :
                String.Empty;

            if (PedidoConfig.LiberarPedido && string.IsNullOrEmpty(idsLiberarPedido) && idLiberarPedido.GetValueOrDefault() == 0)
                return String.Empty;

            try
            {
                string sql = "Select cast(group_concat(idNf) as char) From (Select distinct idNf From pedidos_nota_fiscal Where ";

                if (idLiberarPedido > 0)
                    sql += "idLiberarPedido=" + idLiberarPedido + " Or idPedido In (Select idPedido From produtos_liberar_pedido Where idLiberarPedido In (" + idLiberarPedido + "))";
                else if (idPedido > 0)
                    sql += "idPedido=" + idPedido;
                else if (idAcerto > 0)
                {
                    if (PedidoConfig.LiberarPedido && !String.IsNullOrEmpty(idsLiberarPedido))
                        sql += "idLiberarPedido In (" + idsLiberarPedido +
                            ") Or idPedido In (Select idPedido From produtos_liberar_pedido Where idLiberarPedido In (" + idsLiberarPedido + "))";
                    else if (!PedidoConfig.LiberarPedido && !String.IsNullOrEmpty(idsPedido))
                        sql += " idPedido In (" + idsPedido + ")";
                }

                IList<string> dados = ExecuteMultipleScalar<string>(sql + " limit 3) as tbl");

                return String.Join(",", dados.ToArray());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtém a origem do cheque.
        /// </summary>
        public int ObterOrigem(GDASession session, uint idCheque)
        {
            return ObtemValorCampo<int>(session, "Origem", "IdCheque=" + idCheque);
        }

        /// <summary>
        /// Obtém a propriedade que define se o cheque deve movimentar ou não o caixa geral.
        /// </summary>
        public bool ObterMovimentarCaixaGeral(GDASession session, uint idCheque)
        {
            return ObtemValorCampo<bool?>(session, "MovCaixaFinanceiro", "IdCheque=" + idCheque).GetValueOrDefault();
        }

        #endregion

        #region Verifica se um cheque está reapresentado

        /// <summary>
        /// Verifica se um cheque está reapresentado.
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public bool IsReapresentado(uint idCheque)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from cheques where reapresentado=true and situacao=" +
                (int)Cheques.SituacaoCheque.Devolvido + " and idCheque=" + idCheque) > 0;
        }

        #endregion

        #region Busca para Log de pagamento

        internal Cheques GetForLogPagto(GDASession session, uint idCheque)
        {
            Cheques ch = new Cheques
            {
                IdCheque = idCheque,
                Num = ObtemNumCheque(session, idCheque),
                Banco = ObtemValorCampo<string>(session, "banco", "idCheque=" + idCheque),
                Agencia = ObtemValorCampo<string>(session, "agencia", "idCheque=" + idCheque),
                Conta = ObtemValorCampo<string>(session, "conta", "idCheque=" + idCheque),
                Valor = ObtemValorCampo<decimal>(session, "valor", "idCheque=" + idCheque),
                DataVenc = ObtemValorCampo<DateTime?>(session, "dataVenc", "idCheque=" + idCheque)
            };

            return ch;
        }

        #endregion

        #region Validação do limite do cheque

        /// <summary>
        /// Validação do limite do cheque 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cheque"></param>
        public void ValidaValorLimiteCheque(GDASession session, List<Cheques> lstCheque)
        {
            // Só valida se a configuração está ativa
            if (!FinanceiroConfig.LimitarChequesPorCpfOuCnpj)
                return;

            List<int> situacoesLimite = new List<int> {
                (int)Cheques.SituacaoCheque.EmAberto,
                (int)Cheques.SituacaoCheque.Devolvido,
                (int)Cheques.SituacaoCheque.Protestado,
                (int)Cheques.SituacaoCheque.Trocado
            };

            // Valida apenas cheques de terceiros, que possuam cpfCnpj e que esteja em uma das situações definidas acima
            lstCheque = lstCheque.Where(f => f.Tipo != 1 && !string.IsNullOrEmpty(f.CpfCnpj) && situacoesLimite.Contains(f.Situacao)).ToList();
            
            if (lstCheque.Count() == 0)
                return;

            // Valida o limite dos cheques por cpf/cnpj agrupados
            foreach (var c in lstCheque.GroupBy(f => f.CpfCnpjFormatado))
            {
                // Soma o total de cheques agrupado por cpf/cnpj
                var totalCheques = c.Sum(f => f.Valor);

                string cpfCnpj = c.Key.Replace(".", "").Replace("/", "").Replace("-", "");

                // Recupera o valor do limite geral do cheque
                bool validarLimite;
                var valorRestante = LimiteChequeCpfCnpjDAO.Instance.ObtemValorRestanteLimite(session, cpfCnpj, out validarLimite);

                // Valida o cheque contra o limite geral
                if (validarLimite && totalCheques > valorRestante)
                    throw new Exception(
                        string.Format("Limite excedido para o CPF/CNPJ {0} - Limite restante: {1:c}, Valor dos cheques: {2:c}",
                            cpfCnpj, valorRestante, totalCheques));
            }
            
            // Filtra apenas os cheques que tenham cliente associado
            lstCheque = lstCheque.Where(f => f.IdCliente > 0).ToList();

            // Verifica o limite do cliente, se o cheque tiver um cliente indicado, após ter agrupado por cliente e cpf/cnpj
            foreach (var c in lstCheque.GroupBy(f => new { f.CpfCnpjFormatado, f.IdCliente }))
            {
                // Soma o total de cheques agrupado por cliente e cpf/cnpj
                var totalCheques = c.Sum(f => f.Valor);
 
                bool validarLimite;
                var valorRestante = LimiteChequeCpfCnpjDAO.Instance.ObtemValorRestanteLimiteCliente(session, c.Key.CpfCnpjFormatado, c.Key.IdCliente.Value, out validarLimite);

                // Valida o cheque contra o limite do cliente
                if (validarLimite && totalCheques > valorRestante)
                    throw new Exception(
                        string.Format("Limite excedido para o CPF/CNPJ {0}, cliente {1} - Limite restante: {2:c}, Valor do cheque: {3:c}",
                            c.Key.CpfCnpjFormatado, ClienteDAO.Instance.GetNome(session, c.Key.IdCliente.Value), valorRestante, totalCheques));
            }
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public override uint Insert(Cheques objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession sessao, Cheques objInsert)
        {
            // Coloca a situação do cheque em aberto
            objInsert.Situacao = (int)Cheques.SituacaoCheque.EmAberto;
            objInsert.Tipo = 2; // Cheque de terceiro
            
            return InsertBase(sessao, objInsert, true);
        }

        public uint InsertFinanc(Cheques objInsert)
        {
            objInsert.Origem = (int)Cheques.OrigemCheque.FinanceiroPagto;

            if ((objInsert.IdCliente != null &&
                (FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador && objInsert.IdCliente > 0 &&
                ExisteChequeDigito(objInsert.IdCliente.Value, 0, objInsert.Num, objInsert.DigitoNum))) ||
                ExisteCheque(objInsert.Banco, objInsert.Agencia, objInsert.Conta, objInsert.Num))
                throw new Exception("Este cheque já foi cadastrado no sistema.");

            return InsertBase(objInsert);
        }

        public uint InsertBase(Cheques objInsert)
        {
            return InsertBase(null, objInsert, true);
        }

        public uint InsertBase(GDASession sessao, Cheques objInsert, bool validarLimite)
        {
            FilaOperacoes.Cheques.AguardarVez();

            try
            {
                if (objInsert.IdLoja == 0)
                    objInsert.IdLoja = UserInfo.GetUserInfo.IdLoja;

                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                objInsert.DataCad = DateTime.Now;

                if (!String.IsNullOrEmpty(objInsert.CpfCnpj))
                    objInsert.CpfCnpj = objInsert.CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                // Valida o limite do cheque
                if (validarLimite)
                    ValidaValorLimiteCheque(sessao, new List<Cheques>() { objInsert });

                return base.Insert(sessao, objInsert);
            }
            finally
            {
                FilaOperacoes.Cheques.ProximoFila();
            }
        }

        public uint InsertExecScript(Cheques objInsert)
        {
            return base.Insert(objInsert);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="objUpdate"></param>
        /// <returns></returns>
        public override int Update(Cheques objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession sessao, Cheques objUpdate)
        {
            if (objUpdate.IdCliente != null && (FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador && objUpdate.IdCliente > 0 &&
                ChequesDAO.Instance.ExisteChequeDigito(sessao, objUpdate.IdCliente.Value, objUpdate.IdCheque, objUpdate.Num, objUpdate.DigitoNum)))
                throw new Exception("Este cheque já foi cadastrado no sistema.");

            return UpdateBase(sessao, objUpdate, true);
        }

        public int UpdateBase(Cheques objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        public int UpdateBase(GDASession session, Cheques objUpdate)
        {
            return UpdateBase(session, objUpdate, true);
        }

        internal int UpdateBase(Cheques objUpdate, bool verificarLimite)
        {
            return UpdateBase(null, objUpdate, verificarLimite);
        }

        internal int UpdateBase(GDASession session, Cheques objUpdate, bool verificarLimite)
        {
            FilaOperacoes.Cheques.AguardarVez();

            try
            {
                if (!String.IsNullOrEmpty(objUpdate.CpfCnpj))
                    objUpdate.CpfCnpj = objUpdate.CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                // Valida o limite do cheque
                if (verificarLimite)
                    ValidaValorLimiteCheque(session, new List<Cheques>() { objUpdate });

                objUpdate.Obs = objUpdate.Obs.Replace("'", "").Replace('"', ' ');

                LogAlteracaoDAO.Instance.LogCheque(session, objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
                return base.Update(session, objUpdate);
            }
            catch { throw; }
            finally
            {
                FilaOperacoes.Cheques.ProximoFila();
            }
        }

        public override int Delete(Cheques objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdCheque);
        }

        public override int DeleteByPrimaryKey(uint key)
        {
            // Não apaga o cheque, apenas marca como cancelado
            //LogAlteracaoDAO.ApagaLogCheque(Key);
            //return base.DeleteByPrimaryKey(Key);

            Cheques c = new Cheques {IdCheque = key};
            CancelarCheque(c);

            return 1;
        }

        #endregion

        /// <summary>
        /// Retorna a situação do cheque
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string GetSituacaoCheque(int situacao)
        {
            switch (situacao)
            {
                case (int)Cheques.SituacaoCheque.EmAberto:
                    return "Em Aberto";
                case (int)Cheques.SituacaoCheque.Compensado:
                    return "Compensado";
                case (int)Cheques.SituacaoCheque.Devolvido:
                    return "Devolvido";
                case (int)Cheques.SituacaoCheque.Quitado:
                    return "Quitado";
                case (int)Cheques.SituacaoCheque.Cancelado:
                    return "Cancelado";
                case (int)Cheques.SituacaoCheque.Trocado:
                    return "Trocado";
                case (int)Cheques.SituacaoCheque.Protestado:
                    return "Protestado";
                default:
                    return "";
            }
        }
    }
}