using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;
using Colosoft;

namespace Glass.Data.DAL
{
    public sealed class ClienteDAO : BaseCadastroDAO<Cliente, ClienteDAO>
    {
        //private ClienteDAO() { }

        #region Busca de clientes

        private string Sql(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao, bool isRota, string dataSemCompraIni, string dataSemCompraFim, bool selecionar,
            out bool temFiltro, out string filtroAdicional, out string criterio)
        {
            List<GDAParameter> temp = new List<GDAParameter>();

            return Sql(codCliente, nome, codRota, 0, idFunc, endereco, bairro, 0, telefone, cpfCnpj, situacao, isRota, null, null,
                dataSemCompraIni, dataSemCompraFim, null, null, 0, null, 0, selecionar, out temp, out temFiltro, out filtroAdicional, out criterio);
        }

        private string Sql(string codCliente, string nome, string codRota, uint idLoja, uint idFunc, string endereco, string bairro, uint idCidade,
            string telefone, string cpfCnpj, int situacao, bool isRota, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, uint idTipoCliente, string tipoFiscal, uint idTabelaDesconto,
            bool selecionar, out List<GDAParameter> param, out bool temFiltro, out string filtroAdicional, out string criterio)
        {
            temFiltro = false;
            filtroAdicional = "";
            criterio = String.Empty;

            // Campos que serão retornados pelo sql, se usuário tiver permissão, busca total comprado de cada cliente
            string campos = selecionar ? @"c.*, fg.descricao As FormaPagamento, tc.descricao As TipoCliente, lj.nomeFantasia As Loja,
                r.descricao As Rota, r.codInterno as CodigoRota, p.descricao As Parcela, da.descricao As DescontoAcrescimo,
                If(c.idCidade Is Null, c.cidade, cid.nomeCidade) As NomeCidade, da.Descricao AS TabelaDescontoAcrescimo, 
                If(c.idCidadeCobranca Is Null, c.cidadeCobranca, cobrcid.nomeCidade) As NomeCidadeCobranca,
                cidEntrega.nomeCidade As NomeCidadeEntrega, cid.nomeUf As Uf, cobrcid.nomeUf As UfCobranca,
                cidEntrega.nomeUf As UfEntrega, cid.codIbgeUf as CodIbgeUf, f.nome As DescrUsuCad,
                ff.nome As DescrUsuAlt, com.nome As NomeComissionado, fVend.nome As NomeFunc" : "Count(Distinct c.id_Cli)";

            param = new List<GDAParameter>();
            string sqlDataInativado = String.IsNullOrEmpty(dataInativadoIni) && String.IsNullOrEmpty(dataInativadoFim) ? "" :
                "Left Join (" + LogAlteracaoDAO.Instance.SqlDataAlt((int)LogAlteracao.TabelaAlteracao.Cliente, null, "Situação", "",
                new object[] { "Inativo", "Cancelado", "Bloqueado" }, out param, false) + ") l On (l.idRegistroAlt=c.id_Cli)";

            string sql = @"
                Select " + campos + @" From cliente c 
                    Left Join cidade cid On (cid.idCidade=c.idCidade)
                    Left Join cidade cidEntrega On (cidEntrega.idCidade=c.idCidadeEntrega)
                    Left Join funcionario f On (c.UsuCad=f.idFunc) 
                    Left Join funcionario ff On (c.UsuAlt=ff.idFunc) 
                    Left Join funcionario fVend On (c.idFunc=fVend.idFunc)
                    Left Join comissionado com On (c.idComissionado=com.idComissionado)
                    Left Join cidade cobrcid On (cobrcid.idCidade=c.idCidadeCobranca) 
                    Left Join formapagto fg On (c.IdFormaPagto=fg.IdFormaPagto)
                    Left Join tipo_cliente tc On(c.IdTipoCliente=tc.IdTipoCliente)
                    Left Join loja lj On(c.Id_Loja=lj.IdLoja)
                    Left Join rota_cliente rc On(c.Id_Cli=rc.IdCliente)
                    Left Join rota r On(rc.IdRota=r.IdRota)
                    Left Join parcelas p On(p.IdParcela=c.TipoPagto)
                    Left Join tabela_desconto_acrescimo_cliente da On (c.IdTabelaDesconto=da.IdTabelaDesconto)
                    " + sqlDataInativado + @"
                Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(nome))
            {
                filtroAdicional += " And (c.Nome Like ?nome Or c.nomeFantasia Like ?nome)";
                criterio += "Nome: " + nome + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                filtroAdicional += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.id_loja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFunc > 0)
            {
                filtroAdicional += " And c.idFunc=" + idFunc;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idCidade > 0)
            {
                filtroAdicional += " And c.idCidade=" + idCidade;
                criterio += "Cidade: " + CidadeDAO.Instance.GetNome(idCidade) + "    ";
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                filtroAdicional += " And c.Endereco Like ?endereco ";
                criterio += "Endereço: " + endereco + "    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                filtroAdicional += " And c.Bairro Like ?bairro ";
                criterio += "Bairro: " + bairro + "    ";
            }

            if (!String.IsNullOrEmpty(codCliente))
            {
                filtroAdicional += " And id_cli=?codcli ";
                criterio += "Cliente: " + GetNome(Glass.Conversoes.StrParaUint(codCliente)) + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                filtroAdicional += " And (Tel_Res Like ?telRes Or Tel_Cont Like ?telCont Or Tel_Cel Like ?telCel) ";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                filtroAdicional += " And Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";
                criterio += "CPF/CNPJ: " + cpfCnpj + "    ";
            }

            if (situacao > 0)
            {
                filtroAdicional += " And c.situacao=" + situacao;
                Cliente temp = new Cliente();
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idTipoCliente > 0)
            {
                filtroAdicional += " And c.idtipocliente=" + idTipoCliente;
                criterio += "Tipo: " + TipoClienteDAO.Instance.GetNome(idTipoCliente) + "    ";
            }

            if (!String.IsNullOrEmpty(tipoFiscal))
            {
                filtroAdicional += " And c.tipoFiscal In (" + tipoFiscal + ")";

                if (tipoFiscal.Contains(((int)TipoFiscalCliente.ConsumidorFinal).ToString()))
                    criterio += "Tipo fiscal: Consumidor final";
                if (tipoFiscal.Contains(((int)TipoFiscalCliente.Revenda).ToString()))
                    criterio += (criterio.Contains("Tipo fiscal: ") ? ", Revenda" : "Tipo fiscal: Revenda");

                criterio += criterio.Contains("Tipo fiscal: ") ? "    " : "";
            }

            if (isRota)
            {
                filtroAdicional += " And id_Cli not in (Select idCliente From rota_cliente Where 1)";
                criterio += "Apenas clientes que não possuam rota    ";
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                filtroAdicional += " And c.dataCad >= ?dataCadIni";
                criterio += "Data início cad.: " + dataCadIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                filtroAdicional += " And c.dataCad <= ?dataCadFim";
                criterio += "Data fim cad.: " + dataCadFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraIni))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad>=?dataSemCompraIni)";
                criterio += "Data início sem compra: " + dataSemCompraIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraFim))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad<=?dataSemCompraFim)";
                criterio += "Data fim sem compra: " + dataSemCompraFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataInativadoIni))
            {
                sql += " and l.dataAlt>=?dataInativadoIni";
                criterio += "Data início inativado: " + dataInativadoIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataInativadoFim))
            {
                sql += " and l.dataAlt<=?dataInativadoFim";
                criterio += "Data fim inativado: " + dataInativadoFim + "    ";
                temFiltro = true;
            }

            if (idTabelaDesconto > 0)
            {
                string descr = TabelaDescontoAcrescimoClienteDAO.Instance.GetDescricao(idTabelaDesconto);
                sql += " and c.idtabeladesconto = ?idtabeladesconto";
                criterio += "Tabela Desconto/Acréscimo Cliente: " + descr + "    ";
            }

            sql += " group by c.id_Cli";

            return sql;
        }

        public Cliente GetElement(uint idCliente)
        {
            return GetElement(null, idCliente);
        }

        public Cliente GetElement(GDASession sessao, uint idCliente)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = Sql(idCliente.ToString(), null, null, 0, null, null, null, null, 0, false, null,
                null, true, out temFiltro, out filtroAdicional, out criterio).Replace("?filtroAdicional?", filtroAdicional);

            Cliente cliente = objPersistence.LoadOneData(sessao, sql, GetParamFilter(idCliente.ToString(), null, null, null, null, null, null, null, null, null, null, null, null, 0));

            uint idRota = ClienteDAO.Instance.ObtemIdRota(sessao, idCliente);
            if (idRota > 0 && cliente != null) cliente.IdRota = (int)idRota;

            return cliente;
        }

        public IList<Cliente> GetList(string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = Sql(null, null, null, 0, null, null, null, null, 0, false, null, null, true,
                out temFiltro, out filtroAdicional, out criterio).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            IList<Cliente> lstCliente = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional);

            if (lstCliente != null && lstCliente.Count > 0)
                lstCliente[0].Criterio = criterio;

            return lstCliente;
        }

        public int GetCount()
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = Sql(null, null, null, 0, null, null, null, null, 0, false, null, null, true,
                out temFiltro, out filtroAdicional, out criterio).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional);
        }

        #endregion

        #region Busca os clientes em ordem alfabética

        public IList<Cliente> GetOrdered()
        {
            return objPersistence.LoadData("Select * From cliente c Order By " + ClienteDAO.Instance.GetNomeCliente("c")).ToList();
        }

        #endregion

        #region Busca cliente para relatório

        /// <summary>
        /// QUALQUER MUDANÇA NESSE MÉTODO MUDAR TAMBÉM NO MÉTODO DO FLUXO QUE GERA O RELATÓRIO.
        /// </summary>
        private string SqlRpt(string codRota, uint idLoja, uint idFunc, string endereco, string bairro, uint idCidade, string telefone,
            string cpfCnpj, string dataIni, string dataFim, int tipoPessoa, int revenda, int compra, uint idCli, string nomeCli,
            int situacao, uint idTipoCliente, string tipoFiscal, bool ApenasSemRota, bool ApenasSemPrecoTabela, string dataNiverIni,
            string dataNiverFim, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, uint idTabelaDesconto, int limite,
            out List<GDAParameter> param, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";

            string custoPedido = $@"(Select Round(Sum(p.custoPedido), 2) From pedido p Where p.situacao={ (int)Pedido.SituacaoPedido.Confirmado }
                 And p.tipovenda<>{ (int)Pedido.TipoVendaPedido.Garantia } And p.tipovenda<>{ (int)Pedido.TipoVendaPedido.Reposição }
                 And p.IdCli=c.Id_Cli)";

            string campos = selecionar ? "c.*, if(c.idCidade is null, c.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, cast(" +
                custoPedido + " as decimal(12,2)) as TotalCusto, '^^^' as Criterio, f.nome as nomeFunc, r.Descricao As Rota, " +
                "lj.NomeFantasia As Loja" : "Count(*)";

            string criterio = " ";

            param = new List<GDAParameter>();
            string sqlDataInativado = String.IsNullOrEmpty(dataInativadoIni) && String.IsNullOrEmpty(dataInativadoFim) ? "" :
                "Left Join (" + LogAlteracaoDAO.Instance.SqlDataAlt((int)LogAlteracao.TabelaAlteracao.Cliente, null, "Situação", "",
                new object[] { "Inativo", "Cancelado", "Bloqueado" }, out param, false) + ") l On (l.idRegistroAlt=c.id_Cli)";

            string sql = $@"
                Select { campos } From cliente c 
                    Left Join funcionario f On (c.idFunc=f.idFunc)
                    Left Join cidade cid On (cid.idCidade=c.idCidade)
                    Left Join rota_cliente rc On(c.Id_Cli=rc.IdCliente)
                    Left Join rota r On(rc.IdRota=r.IdRota)
                    Left Join loja lj On(c.Id_Loja=lj.IdLoja)
                    { sqlDataInativado }
                Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(dataNiverIni))
            {
                filtroAdicional += @" And MONTH(Data_nasc) * 100 + DAY(Data_Nasc) >= MONTH(?dataNiverIni) * 100 + DAY(?dataNiverIni)";
                criterio = $"Data Aniversário Início: { dataNiverIni }    ";
            }

            if (!String.IsNullOrEmpty(dataNiverFim))
            {
                filtroAdicional += !String.IsNullOrEmpty(dataNiverIni) && DateTime.Parse(dataNiverIni + " 00:00") <= DateTime.Parse(dataNiverFim + " 00:00") ?
                "And MONTH(Data_nasc) * 100 + DAY(Data_Nasc) <= MONTH(?dataNiverFim) * 100 + DAY(?dataNiverFim)" :
                "And (MONTH(Data_nasc) + 12) * 100 + DAY(Data_Nasc) <= (MONTH(?dataNiverFim) + 12) * 100 + DAY(?dataNiverFim))";
                criterio = "Data Aniversário Fim: " + dataNiverFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                filtroAdicional += " And c.dataCad >= ?dataCadIni";
                criterio += $"Data início cad.: { dataCadIni }    ";
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                filtroAdicional += " And c.dataCad <= ?dataCadFim";
                criterio += $"Data fim cad.: { dataCadFim }    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraIni))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad>=?dataSemCompraIni)";
                criterio += $"Data início sem compra: { dataSemCompraIni }    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraFim))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad<=?dataSemCompraFim)";
                criterio += $"Data fim sem compra: { dataSemCompraFim }    ";
            }

            if (!String.IsNullOrEmpty(dataInativadoIni))
            {
                sql += " and l.dataAlt>=?dataInativadoIni";
                criterio += $"Data início inativado: { dataInativadoIni }    ";
            }

            if (!String.IsNullOrEmpty(dataInativadoFim))
            {
                sql += " and l.dataAlt<=?dataInativadoFim";
                criterio += $"Data fim inativado: { dataInativadoFim }    ";
            }

            if (!String.IsNullOrEmpty(sqlDataInativado) && selecionar)
                sql += " group by c.id_Cli";

            if (ApenasSemRota)
            {
                filtroAdicional += " And c.id_Cli Not In (Select idCliente From rota_cliente)";
                criterio += "Apenas Clientes sem Rota    ";
            }
            else if (!String.IsNullOrEmpty(codRota))
            {
                filtroAdicional += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += $"Rota: { codRota }    ";
            }

            if (ApenasSemPrecoTabela)
            {
                filtroAdicional += " And c.idTabelaDesconto IS NULL";
                criterio += "Apenas Clientes sem Tabela de Desconto/Acréscimo    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += $" And c.id_loja={ idLoja }";
                criterio += $"Loja: { LojaDAO.Instance.GetNome(idLoja) }    ";
            }

            if (idFunc > 0)
            {
                filtroAdicional += $" And c.idFunc={ idFunc }";
                criterio += $"Vendedor: { FuncionarioDAO.Instance.GetNome(idFunc) }    ";
            }

            if (idCidade > 0)
            {
                filtroAdicional += $" And c.idCidade={ idCidade }";
                criterio += $"Cidade: { CidadeDAO.Instance.GetNome(idCidade) }    ";
            }

            if (idTipoCliente > 0)
            {
                filtroAdicional += $" And c.idTipoCliente={ idTipoCliente }";
                criterio += $"Tipo: { TipoClienteDAO.Instance.GetNome(idTipoCliente) }    ";
            }

            if (!String.IsNullOrEmpty(tipoFiscal))
            {
                filtroAdicional += $" And c.tipoFiscal In ({ tipoFiscal })";

                if (tipoFiscal.Contains(((int)TipoFiscalCliente.ConsumidorFinal).ToString()))
                    criterio += "Tipo fiscal: Consumidor final";
                if (tipoFiscal.Contains(((int)TipoFiscalCliente.Revenda).ToString()))
                    criterio += (criterio.Contains("Tipo fiscal: ") ? ", Revenda" : "Tipo fiscal: Revenda");

                criterio += criterio.Contains("Tipo fiscal: ") ? "    " : "";
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                filtroAdicional += " And c.Endereco Like ?endereco ";
                criterio += $"Endereço: { endereco }    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                filtroAdicional += " And c.Bairro Like ?bairro ";
                criterio += $"Bairro: { bairro }    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                filtroAdicional += " And (Tel_Res Like ?telRes Or Tel_Cont Like ?telCont Or Tel_Cel Like ?telCel) ";
                criterio += $"Telefone: { telefone }    ";
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                filtroAdicional += " And Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";
                criterio += $"CPF/CNPJ: { cpfCnpj }    ";
            }

            if (idTabelaDesconto > 0)
            {
                string descr = TabelaDescontoAcrescimoClienteDAO.Instance.GetDescricao(idTabelaDesconto);
                sql += " and c.idtabeladesconto = ?idtabeladesconto";
                criterio += $"Tabela Desconto/Acréscimo Cliente: { descr }    ";
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " And c.Dt_Ult_Compra>=?dataIni";
                criterio = $"Data Início: { dataIni }    ";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " And c.Dt_Ult_Compra<=?dataFim";
                criterio += $"Data Fim: {dataFim}    ";
            }

            if (tipoPessoa > 0)
            {
                filtroAdicional += $" And c.Tipo_Pessoa='{ (tipoPessoa == 1 ? "F" : "J") }'";
                criterio += $"Tipo Pessoa: { (tipoPessoa == 1 ? "Física" : "Jurídica") }    ";
            }

            if (idCli > 0)
                filtroAdicional += $" And c.Id_Cli={ idCli }";
            else
            {
                if (!String.IsNullOrEmpty(nomeCli))
                {
                    string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                    filtroAdicional += $" And c.id_Cli in ({ ids })";
                }

                if (compra == 1)
                    filtroAdicional += " And totalComprado > 0";

                if (revenda == 1)
                {
                    filtroAdicional += " And c.Revenda=1";
                    criterio += "Clientes Revenda    ";
                }

                if (situacao > 0)
                {
                    filtroAdicional += $" And c.situacao={ situacao }";
                    Cliente temp = new Cliente();
                    temp.Situacao = situacao;
                    criterio += $"Situação: { temp.DescrSituacao }    ";
                }
            }

            if (limite > 0)
            {
                if (limite == 1)
                {
                    filtroAdicional += " and c.limite < c.usoLimite";
                    criterio += "Acima do limite  ";
                }
                else if (limite == 2)
                {
                    filtroAdicional += " and c.limite >= c.usoLimite";
                    criterio += "Dentro do limite  ";
                }
                else
                {
                    filtroAdicional += " and COALESCE(c.limite, 0) = 0";
                    criterio += "Sem limite cadastrado  ";
                }
            }

            return sql.Replace("^^^", criterio);
        }

        /// <summary>
        /// QUALQUER MUDANÇA NESSE MÉTODO MUDAR TAMBÉM NO MÉTODO DO FLUXO QUE GERA O RELATÓRIO.
        /// </summary>
        public IList<Cliente> GetForListaRpt(string codRota, uint idFunc, string dataIni, string dataFim, int tipoPessoa, bool revenda, bool compra, uint idCli,
            string nomeCli, int situacao, bool apenasSemRota, bool apenasSemPrecoTabela, string dataNiverIni, string dataNiverFim, string dataCadIni,
            string dataCadFim, string dataSemCompraIni, string dataSemCompraFim, string dataInativadoIni, string dataInativadoFim, int limite,
            string sortExpression, int startRow, int pageSize)
        {
            var orderBy = String.IsNullOrEmpty(sortExpression) ? "TotalComprado desc" : sortExpression;
            var temp = new List<GDAParameter>();
            string filtroAdicional;
            var sql = SqlRpt(codRota, 0, idFunc, null, null, 0, null, null, dataIni, dataFim, tipoPessoa, revenda ? 1 : 0, compra ? 1 : 0,
                idCli, nomeCli, situacao, 0, null, apenasSemRota, apenasSemPrecoTabela, dataNiverIni, dataNiverFim, dataCadIni, dataCadFim, dataSemCompraIni, dataSemCompraFim,
                dataInativadoIni, dataInativadoFim, 0, limite, out temp, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, orderBy, startRow, pageSize, !String.IsNullOrEmpty(filtroAdicional), filtroAdicional,
                GetRptParams(codRota, null, null, null, null, dataIni, dataFim, nomeCli, dataNiverIni, dataNiverFim, null, null,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, 0, temp));
        }

        void a_LoadResultPage(object sender, GDA.Sql.LoadResultPageArgs<Cliente> e)
        {
            throw new NotImplementedException();
        }

        public int GetRptCount(string codRota, uint idFunc, string dataIni, string dataFim, int tipoPessoa, bool revenda, bool compra,
            uint idCli, string nomeCli, int situacao, bool apenasSemRota, bool apenasSemPrecoTabela, string dataNiverIni,
            string dataNiverFim, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, int limite)
        {
            string filtroAdicional;
            var temp = new List<GDAParameter>();
            string sql = SqlRpt(codRota, 0, idFunc, null, null, 0, null, null, dataIni, dataFim, tipoPessoa, revenda ? 1 : 0, compra ? 1 : 0,
                idCli, nomeCli, situacao, 0, null, apenasSemRota, apenasSemPrecoTabela, dataNiverIni, dataNiverFim, dataCadIni, dataCadFim,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, 0, limite, out temp, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, !String.IsNullOrEmpty(filtroAdicional), filtroAdicional, GetRptParams(codRota, null, null, null, null, dataIni, dataFim, nomeCli, dataNiverIni,
                dataNiverFim, dataCadIni, dataCadFim, dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, 0, temp));
        }

        private GDAParameter[] GetRptParams(string codRota, string endereco, string bairro, string telefone, string cpfCnpj,
            string dataIni, string dataFim, string nomeCli, string dataNiverIni, string dataNiverFim, string dataCadIni,
            string dataCadFim, string dataSemCompraIni, string dataSemCompraFim, string dataInativadoIni, string dataInativadoFim,
            uint idTabelaDesconto, List<GDAParameter> param)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codRota))
                lstParam.Add(new GDAParameter("?codRota", codRota));

            if (!String.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!String.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!String.IsNullOrEmpty(telefone))
            {
                lstParam.Add(new GDAParameter("?telCont", "%" + telefone + "%"));
                lstParam.Add(new GDAParameter("?telRes", "%" + telefone + "%"));
                lstParam.Add(new GDAParameter("?telCel", "%" + telefone + "%"));
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
                lstParam.Add(new GDAParameter("?cpfCnpj", "%" + cpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "") + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dataNiverIni))
                lstParam.Add(new GDAParameter("?dataNiverIni", DateTime.Parse(dataNiverIni)));

            if (!String.IsNullOrEmpty(dataNiverFim))
                lstParam.Add(new GDAParameter("?dataNiverFim", DateTime.Parse(dataNiverFim)));

            if (!String.IsNullOrEmpty(dataCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataSemCompraIni))
                lstParam.Add(new GDAParameter("?dataSemCompraIni", DateTime.Parse(dataSemCompraIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataSemCompraFim))
                lstParam.Add(new GDAParameter("?dataSemCompraFim", DateTime.Parse(dataSemCompraFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataInativadoIni))
                lstParam.Add(new GDAParameter("?dataInativadoIni", DateTime.Parse(dataInativadoIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataInativadoFim))
                lstParam.Add(new GDAParameter("?dataInativadoFim", DateTime.Parse(dataInativadoFim + " 23:59")));

            if (idTabelaDesconto > 0)
                lstParam.Add(new GDAParameter("?idTabelaDesconto", idTabelaDesconto));

            if (param != null)
                for (int i = 0; i < param.Count; i++)
                    lstParam.Add(new GDAParameter(param.ToArray()[i].ParameterName, param.ToArray()[i].Value));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca clientes vinculados à um cliente

        /// <summary>
        /// Busca clientes vinculados ao cliente passado
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public IList<Cliente> GetVinculados(uint idCli)
        {
            string sql = "Select * From cliente Where id_Cli In (Select idClienteVinculo From cliente_vinculo Where idCliente=" + idCli + ")";
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca clientes para uma rota

        private string SqlRotaCliente(uint idRota, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = " and id_Cli In (Select idCliente From rota_cliente Where idRota=" + idRota + ")";

            string campos = selecionar ? "c.*, if(c.idCidade is null, c.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, rc.IdRota" :
                "Count(*)";

            string sql = @"
                Select " + campos + @" From cliente c 
                    Left Join cidade cid On (cid.idCidade=c.idCidade) 
                    Left Join rota_cliente rc On (c.id_Cli=rc.idCliente)
                Where 1 ?filtroAdicional?";

            return sql;
        }

        public IList<Cliente> GetRotaClienteList(uint idRota, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlRotaCliente(idRota, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, "rc.NumSeq Asc", startRow, pageSize, false, filtroAdicional);
        }

        public int GetRotaClienteCount(uint idRota)
        {
            string filtroAdicional;
            string sql = SqlRotaCliente(idRota, true, out filtroAdicional).Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional);
        }

        public IList<Cliente> GetForRotaRpt(uint idRota)
        {
            string filtroAdicional;
            string sql = SqlRotaCliente(idRota, true, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca clientes para EFD

        /// <summary>
        /// Busca clientes para montar arquivo EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Cliente GetForEFD(uint idCliente)
        {
            return objPersistence.LoadOneData(@"select c.*, cid.codIbgeUf, cid.codIbgeCidade, cid.nomeUF as UF
                from cliente c left join cidade cid on (cid.idCidade=c.idCidade)
                where c.id_Cli=" + idCliente);
        }

        #endregion

        #region Retorna o nome do cliente

        /// <summary>
        /// Retorna o nome do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNome(uint idCliente)
        {
            return GetNome(null, idCliente);
        }

        /// <summary>
        /// Retorna o nome do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNome(GDASession sessao, uint idCliente)
        {
            string sql = "Select " + ClienteDAO.Instance.GetNomeCliente("c") + " From cliente c Where id_cli=" + idCliente;

            object nome = objPersistence.ExecuteScalar(sessao, sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        public bool Existe(string cpfCnpj)
        {
            string sql = "Select count(*) From cliente Where cpf_Cnpj=?cnpj";
            return Convert.ToInt32(objPersistence.ExecuteScalar(sql, new GDAParameter("?cnpj", cpfCnpj))) > 0;
        }

        /// <summary>
        /// Retorna o nome fantasia do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNomeFantasia(GDASession session, uint idCliente)
        {
            string sql = "Select nomefantasia From cliente Where id_cli=" + idCliente;

            object nome = objPersistence.ExecuteScalar(session, sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o nome fantasia do cliente
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetRazaoSocial(GDASession session, uint idCliente)
        {
            var sql = "SELECT Nome FROM cliente WHERE Id_Cli=" + idCliente;

            object nome = objPersistence.ExecuteScalar(session, sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o nome do cliente a partir do acerto
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNomeByAcerto(uint idAcerto)
        {
            string sql = "Select nome From cliente Where id_cli in (select id_cli from acerto where idacerto=" + idAcerto + ")";

            object nome = objPersistence.ExecuteScalar(sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o nome do cliente a partir do pedido
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNomeByPedido(uint idPedido)
        {
            string sql = "Select nome From cliente Where id_cli in (select idcli from pedido where idpedido=" + idPedido + ")";

            object nome = objPersistence.ExecuteScalar(sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o nome do cliente a partir de uma liberação de pedido
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetNomeByLiberarPedido(uint idLiberarPedido)
        {
            string sql = "Select nome From cliente Where id_cli in (select idcliente from liberarpedido where idLiberarPedido=" + idLiberarPedido + ")";

            object nome = objPersistence.ExecuteScalar(sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        #endregion

        #region Busca por pedido

        /// <summary>
        /// Retorna o cliente usado no pedido passado
        /// </summary>
        public Cliente GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        /// <summary>
        /// Retorna o cliente usado no pedido passado
        /// </summary>
        public Cliente GetByPedido(GDASession session, uint idPedido)
        {
            string sql = "Select * From cliente Where id_Cli In (Select idCli From pedido Where idPedido=" + idPedido + ")";

            return objPersistence.LoadOneData(session, sql);
        }

        #endregion

        #region Retorna o percentual de redução que o cliente possui na NFe

        /// <summary>
        /// Retorna o percentual de redução que o cliente possui na NFe
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public float GetPercReducaoNFe(uint idCliente)
        {
            string sql = "Select Coalesce(percReducaoNfe, 0) From cliente Where id_cli=" + idCliente;

            object value = objPersistence.ExecuteScalar(sql);

            return value != null ? float.Parse(value.ToString().Replace('.', ',')) : 0;
        }

        /// <summary>
        /// Retorna o percentual de redução que o cliente possui na NFe para produtos de revenda
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public float GetPercReducaoNFeRevenda(uint idCliente)
        {
            string sql = "Select Coalesce(percReducaoNfeRevenda, 0) From cliente Where id_cli=" + idCliente;

            object value = objPersistence.ExecuteScalar(sql);

            return value != null ? float.Parse(value.ToString().Replace('.', ',')) : 0;
        }

        #endregion

        #region Verifica se cliente já existe

        /// <summary>
        /// Verifica se já existe um cliente cadastrado com o CPF/CNPJ cadastrado
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public bool CheckIfExists(string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return false;

            string sql = "Select Count(*) From cliente Where " +
                "Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '')='" + cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "") + "'";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteSqlQueryCount(sql).ToString()) > 0;
        }

        #endregion

        #region Busca Clientes utilizando filtros

        private string SqlListaCliente(string codCliente, string nome, string codRota, uint idLoja, uint idFunc, string endereco, string bairro, uint idCidade,
            string telefone, string cpfCnpj, int situacao, bool isRota, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, uint idTipoCliente, string tipoFiscal, uint idTabelaDesconto,
            bool selecionar, out List<GDAParameter> param, out bool temFiltro, out string filtroAdicional, out string criterio)
        {
            temFiltro = false;
            filtroAdicional = "";
            criterio = String.Empty;

            // Campos que serão retornados pelo sql, se usuário tiver permissão, busca total comprado de cada cliente
            string campos = selecionar ? @"c.*, tc.descricao As TipoCliente, lj.nomeFantasia As Loja,
                cid.nomeCidade, cid.nomeUf As Uf, cid.codIbgeUf as CodIbgeUf, f.nome As DescrUsuCad,
                ff.nome As DescrUsuAlt, com.nome As NomeComissionado, fVend.nome As NomeFunc" : "Count(Distinct c.id_Cli)";

            param = new List<GDAParameter>();
            string sqlDataInativado = String.IsNullOrEmpty(dataInativadoIni) && String.IsNullOrEmpty(dataInativadoFim) ? "" :
                "Left Join (" + LogAlteracaoDAO.Instance.SqlDataAlt((int)LogAlteracao.TabelaAlteracao.Cliente, null, "Situação", "",
                new object[] { "Inativo", "Cancelado", "Bloqueado" }, out param, false) + ") l On (l.idRegistroAlt=c.id_Cli)";

            string sql = @"
                Select " + campos + @" From cliente c 
                    Left Join cidade cid On (cid.idCidade=c.idCidade)
                    Left Join funcionario f On (c.UsuCad=f.idFunc) 
                    Left Join funcionario ff On (c.UsuAlt=ff.idFunc) 
                    Left Join funcionario fVend On (c.idFunc=fVend.idFunc)
                    Left Join comissionado com On (c.idComissionado=com.idComissionado)
                    Left Join tipo_cliente tc On(c.IdTipoCliente=tc.IdTipoCliente)
                    Left Join loja lj On(c.Id_Loja=lj.IdLoja)
                    " + sqlDataInativado + @"
                Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(nome))
            {
                filtroAdicional += " And (c.Nome Like ?nome Or c.nomeFantasia Like ?nome)";
                criterio += "Nome: " + nome + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                filtroAdicional += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                criterio += "Rota: " + codRota + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.id_loja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFunc > 0)
            {
                filtroAdicional += " And c.idFunc=" + idFunc;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idCidade > 0)
            {
                filtroAdicional += " And c.idCidade=" + idCidade;
                criterio += "Cidade: " + CidadeDAO.Instance.GetNome(idCidade) + "    ";
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                filtroAdicional += " And c.Endereco Like ?endereco ";
                criterio += "Endereço: " + endereco + "    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                filtroAdicional += " And c.Bairro Like ?bairro ";
                criterio += "Bairro: " + bairro + "    ";
            }

            if (!String.IsNullOrEmpty(codCliente))
            {
                filtroAdicional += " And id_cli=?codcli ";
                criterio += "Cliente: " + GetNome(Glass.Conversoes.StrParaUint(codCliente)) + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                filtroAdicional += " And (Tel_Res Like ?telRes Or Tel_Cont Like ?telCont Or Tel_Cel Like ?telCel) ";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                filtroAdicional += " And Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";
                criterio += "CPF/CNPJ: " + cpfCnpj + "    ";
            }

            if (situacao > 0)
            {
                filtroAdicional += " And c.situacao=" + situacao;
                Cliente temp = new Cliente();
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idTipoCliente > 0)
            {
                filtroAdicional += " And c.idtipocliente=" + idTipoCliente;
                criterio += "Tipo: " + TipoClienteDAO.Instance.GetNome(idTipoCliente) + "    ";
            }

            if (!String.IsNullOrEmpty(tipoFiscal))
            {
                filtroAdicional += " And c.tipoFiscal In (" + tipoFiscal + ")";

                if (tipoFiscal.Contains(((int)TipoFiscalCliente.ConsumidorFinal).ToString()))
                    criterio += "Tipo fiscal: Consumidor final";
                if (tipoFiscal.Contains(((int)TipoFiscalCliente.Revenda).ToString()))
                    criterio += (criterio.Contains("Tipo fiscal: ") ? ", Revenda" : "Tipo fiscal: Revenda");

                criterio += criterio.Contains("Tipo fiscal: ") ? "    " : "";
            }

            if (isRota)
            {
                filtroAdicional += " And id_Cli not in (Select idCliente From rota_cliente Where 1)";
                criterio += "Apenas clientes que não possuam rota    ";
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                filtroAdicional += " And c.dataCad >= ?dataCadIni";
                criterio += "Data início cad.: " + dataSemCompraIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                filtroAdicional += " And c.dataCad <= ?dataCadFim";
                criterio += "Data fim cad.: " + dataCadFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraIni))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad>=?dataSemCompraIni)";
                criterio += "Data início sem compra: " + dataSemCompraIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataSemCompraFim))
            {
                filtroAdicional += " and id_Cli not in (select idCli from pedido where dataCad<=?dataSemCompraFim)";
                criterio += "Data fim sem compra: " + dataSemCompraFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataInativadoIni))
            {
                sql += " and l.dataAlt>=?dataInativadoIni";
                criterio += "Data início inativado: " + dataInativadoIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataInativadoFim))
            {
                sql += " and l.dataAlt<=?dataInativadoFim";
                criterio += "Data fim inativado: " + dataInativadoFim + "    ";
                temFiltro = true;
            }

            if (idTabelaDesconto > 0)
            {
                string descr = TabelaDescontoAcrescimoClienteDAO.Instance.GetDescricao(idTabelaDesconto);
                sql += " and c.idtabeladesconto = ?idtabeladesconto";
                criterio += "Tabela Desconto/Acréscimo Cliente: " + descr + "    ";
            }

            if (selecionar)
                sql += " group by c.id_Cli";

            return sql;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="codCliente"></param>
        /// <param name="nome"></param>
        /// <param name="codRota"></param>
        /// <param name="idFunc"></param>
        /// <param name="endereco"></param>
        /// <param name="bairro"></param>
        /// <param name="telefone"></param>
        /// <param name="cpfCnpj"></param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string GetIds(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao)
        {
            return GetIds(null, codCliente, nome, codRota, idFunc, endereco, bairro, telefone, cpfCnpj, situacao);
        }

        public string GetIds(GDASession sessao, string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao)
        {
            string sql = "Select c.* From cliente c Where 1 ";

            if (!String.IsNullOrEmpty(nome))
                sql += " And (c.Nome Like ?nome Or c.nomeFantasia Like ?nome)";

            if (!String.IsNullOrEmpty(codRota))
                sql += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";

            if (idFunc > 0)
                sql += " And c.idFunc=" + idFunc;

            if (!String.IsNullOrEmpty(endereco))
                sql += " And c.Endereco Like ?endereco ";

            if (!String.IsNullOrEmpty(bairro))
                sql += " And c.Bairro Like ?bairro ";

            if (!String.IsNullOrEmpty(codCliente))
                sql += " And id_cli=?codcli ";

            if (!String.IsNullOrEmpty(telefone))
                sql += " And (Tel_Res Like ?telRes Or Tel_Cont Like ?telCont Or Tel_Cel Like ?telCel) ";

            if (!String.IsNullOrEmpty(cpfCnpj))
                sql += " And Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";

            if (situacao > 0)
                sql += " And c.situacao=" + situacao;

            var ids = objPersistence.LoadResult(sessao, sql,
                GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0)).Select(f => f.GetUInt32(0))
                       .ToList();;

            if (ids.Count == 0)
                return "0";

            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate(uint x)
                {
                    return x.ToString();
                }
            )));
        }

        public IList<Cliente> GetListaCliente(string codCliente, string nome, string codRota, uint idLoja, uint idFunc, string endereco, string bairro,
           string telefone, string cpfCnpj, int situacao, bool apenasSemRota, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim, string dataInativadoIni,
           string dataInativadoFim, uint idTipoCliente, string tipoFiscal, uint idCidade, uint idTabelaDesconto, string sortExpression, int startRow, int pageSize)
        {
            List<GDAParameter> p;
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = SqlListaCliente(codCliente, nome, codRota, idLoja, idFunc, endereco, bairro, idCidade, telefone, cpfCnpj, situacao, apenasSemRota, dataCadIni, dataCadFim,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, idTipoCliente, tipoFiscal, idTabelaDesconto, true, out p, out temFiltro,
                out filtroAdicional, out criterio).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            p.AddRange(GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, dataCadIni, dataCadFim, dataSemCompraIni,
                dataSemCompraFim, dataInativadoIni, dataInativadoFim, idTabelaDesconto));

            IList<Cliente> lstCliente = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, p.ToArray()).ToList();

            if (lstCliente != null && lstCliente.Count > 0)
                lstCliente[0].Criterio = criterio;

            return lstCliente;
        }

        public int GetCountListaCliente(string codCliente, string nome, string codRota, uint idLoja, uint idFunc, string endereco, string bairro, uint idCidade,
            string telefone, string cpfCnpj, int situacao, bool apenasSemRota, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, uint idTipoCliente, string tipoFiscal, uint idTabelaDesconto)
        {
            List<GDAParameter> p;
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = SqlListaCliente(codCliente, nome, codRota, idLoja, idFunc, endereco, bairro, idCidade, telefone, cpfCnpj, situacao, apenasSemRota, dataCadIni, dataCadFim,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, idTipoCliente, tipoFiscal, idTabelaDesconto, false, out p, out temFiltro,
                out filtroAdicional, out criterio).Replace("?filtroAdicional?", filtroAdicional);

            p.AddRange(GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, dataCadIni, dataCadFim,
                dataSemCompraIni, dataSemCompraFim, dataInativadoIni, dataInativadoFim, idTabelaDesconto));

            return objPersistence.ExecuteSqlQueryCount(sql, p.ToArray());
        }

        public IList<Cliente> GetForSel(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao, bool isRota, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            List<GDAParameter> p;
            string sql = SqlListaCliente(codCliente, nome, codRota, 0, idFunc, endereco, bairro, 0, telefone, cpfCnpj, situacao, isRota,
                null, null, null, null, null, null, 0, null, 0, true, out p, out temFiltro, out filtroAdicional, out criterio)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            p.AddRange(GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));

            IList<Cliente> lstCliente = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));

            if (lstCliente != null && lstCliente.Count > 0)
                lstCliente[0].Criterio = criterio;

            return lstCliente;
        }

        public IList<Cliente> GetForSel(string nome, string sortExpression, int startRow, int pageSize)
        {
            return this.GetForSel(String.Empty, nome, string.Empty, 0, string.Empty,
                string.Empty, string.Empty, string.Empty, 0, false, sortExpression, startRow, pageSize).ToList();
        }

        public int GetCountSel(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao, bool isRota)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            List<GDAParameter> p;
            string sql = SqlListaCliente(codCliente, nome, codRota, 0, idFunc, endereco, bairro, 0, telefone, cpfCnpj, situacao, isRota,
                null, null, null, null, null, null, 0, null, 0, false, out p, out temFiltro, out filtroAdicional, out criterio)
                .Replace("?filtroAdicional?", filtroAdicional);

            p.AddRange(GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));

            return objPersistence.ExecuteSqlQueryCount(sql, p.ToArray());
        }

        public int GetCountSel(string nome)
        {
            return this.GetCountSel(string.Empty, nome, string.Empty, 0, string.Empty, string.Empty, string.Empty, string.Empty, 0, false);
        }

        public IList<Cliente> GetForVinculo(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = Sql(codCliente, nome, codRota, idFunc, endereco, bairro, telefone, cpfCnpj, situacao, false,
                null, null, true, out temFiltro, out filtroAdicional, out criterio).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            IList<Cliente> lstCliente = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamFilter(codCliente, nome, codRota, endereco, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));

            if (lstCliente != null && lstCliente.Count > 0)
                lstCliente[0].Criterio = criterio;

            return lstCliente;
        }

        public int GetCountVinculo(string codCliente, string nome, string codRota, uint idFunc, string endereco, string bairro,
            string telefone, string cpfCnpj, int situacao)
        {
            bool temFiltro;
            string filtroAdicional, criterio;

            string sql = Sql(codCliente, nome, codRota, idFunc, endereco, bairro, telefone, cpfCnpj, situacao, false,
                null, null, true, out temFiltro, out filtroAdicional, out criterio).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamFilter(codCliente, nome, codRota, endereco,
                bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));
        }

        private GDAParameter[] GetParamFilter(string codCliente, string nome, string codRota, string endereco, string bairro,
            string telefone, string cpfCnpj, string dataCadIni, string dataCadFim, string dataSemCompraIni, string dataSemCompraFim,
            string dataInativadoIni, string dataInativadoFim, uint idTabelaDesconto)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codCliente))
                lstParam.Add(new GDAParameter("?codcli", codCliente));

            if (!String.IsNullOrEmpty(codRota))
                lstParam.Add(new GDAParameter("?codRota", codRota));

            if (!String.IsNullOrEmpty(nome))
                lstParam.Add(new GDAParameter("?nome", "%" + nome + "%"));

            if (!String.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!String.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!String.IsNullOrEmpty(telefone))
            {
                lstParam.Add(new GDAParameter("?telCont", "%" + telefone + "%"));
                lstParam.Add(new GDAParameter("?telRes", "%" + telefone + "%"));
                lstParam.Add(new GDAParameter("?telCel", "%" + telefone + "%"));
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
                lstParam.Add(new GDAParameter("?cpfCnpj", "%" + cpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "") + "%"));

            if (!String.IsNullOrEmpty(dataCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataSemCompraIni))
                lstParam.Add(new GDAParameter("?dataSemCompraIni", DateTime.Parse(dataSemCompraIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataSemCompraFim))
                lstParam.Add(new GDAParameter("?dataSemCompraFim", DateTime.Parse(dataSemCompraFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataInativadoIni))
                lstParam.Add(new GDAParameter("?dataInativadoIni", DateTime.Parse(dataInativadoIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataInativadoFim))
                lstParam.Add(new GDAParameter("?dataInativadoFim", DateTime.Parse(dataInativadoFim + " 23:59")));

            if (idTabelaDesconto > 0)
                lstParam.Add(new GDAParameter("?idTabelaDesconto", idTabelaDesconto));

            return lstParam.ToArray();
        }

        #endregion

        #region Busca Clientes que possuam crédito

        private string SqlListCredito(string codCliente, string nome, string bairro, string telefone, string cpfCnpj,
            bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = " and credito>0";
            string campos = selecionar ? @"c.*, if(c.idCidade is null, c.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, 
                '$$$' as criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From cliente c 
                    Left Join cidade cid On (cid.idCidade=c.idCidade) 
                Where 1 ?filtroAdicional?";

            if (!String.IsNullOrEmpty(codCliente) && codCliente != "0")
            {
                filtroAdicional += " And id_cli=?codcli ";
                criterio += "Cliente: " + GetNome(Glass.Conversoes.StrParaUint(codCliente)) + "    ";
            }
            else if (!String.IsNullOrEmpty(nome))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nome, null, 0, null, null, null, null, 0);
                filtroAdicional += " And c.id_Cli in (" + ids + ") ";
                criterio += "Cliente: " + nome + "    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                filtroAdicional += " And Bairro Like ?bairro ";
                criterio += "Bairro: " + bairro + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                filtroAdicional += " And (Tel_Res Like ?telRes Or Tel_Cont Like ?telCont Or Tel_Cel Like ?telCel) ";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                filtroAdicional += " And Replace(Replace(Replace(Cpf_Cnpj, '.', ''), '-', ''), '/', '') Like ?cpfCnpj ";
                criterio += "CPF/CNPJ: " + cpfCnpj + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        public Cliente[] GetListCreditoRpt(string codCliente, string nome, string bairro, string telefone, string cpfCnpj)
        {
            string filtroAdicional;
            string sql = SqlListCredito(codCliente, nome, bairro, telefone, cpfCnpj, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, GetParamFilter(codCliente, nome, null, null, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0)).ToArray();
        }

        public IList<Cliente> GetListCredito(string codCliente, string nome, string bairro, string telefone, string cpfCnpj, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlListCredito(codCliente, nome, bairro, telefone, cpfCnpj, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional,
                GetParamFilter(codCliente, nome, null, null, bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));
        }

        public int GetCountListCredito(string codCliente, string nome, string bairro, string telefone, string cpfCnpj)
        {
            string filtroAdicional;
            string sql = SqlListCredito(codCliente, nome, bairro, telefone, cpfCnpj, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParamFilter(codCliente, nome, null, null,
                bairro, telefone, cpfCnpj, null, null, null, null, null, null, 0));
        }

        #endregion

        #region Busca Rápida de Cliente

        #endregion

        #region Retorna Crédito

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o crédito do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal GetCredito(uint idCliente)
        {
            return GetCredito(null, idCliente);
        }

        /// <summary>
        /// Retorna o crédito do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal GetCredito(GDASession sessao, uint idCliente)
        {
            if (idCliente == 0)
                return 0;

            return ObtemValorCampo<decimal>(sessao, "credito", "id_Cli=" + idCliente);
        }

        #endregion

        #region Debita Crédito

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="valor"></param>
        public void DebitaCredito(uint idCliente, decimal valor)
        {
            DebitaCredito(null, idCliente, valor);
        }

        public void DebitaCredito(GDASession sessao, uint idCliente, decimal valor)
        {
            string sql = "Update cliente Set credito=coalesce(credito, 0)-" + valor.ToString().Replace(',', '.') + " Where id_Cli=" + idCliente;

            if (objPersistence.ExecuteCommand(sessao, sql, null) < 1)
                throw new Exception("Falha ao debitar crédito do cliente. Atualização afetou 0 registros.");
        }

        #endregion

        #region Credita Crédito

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="valor"></param>
        public void CreditaCredito(uint idCliente, decimal valor)
        {
            CreditaCredito(null, idCliente, valor);
        }

        public void CreditaCredito(GDASession sessao, uint idCliente, decimal valor)
        {
            string sql = "Update cliente Set credito=coalesce(credito, 0)+" + Math.Round(valor, 2).ToString().Replace(',', '.') + " Where id_Cli=" + idCliente;

            if (objPersistence.ExecuteCommand(sessao, sql, null) < 1)
                throw new Exception("Falha ao creditar crédito do cliente. Atualização afetou 0 registros.");
        }

        #endregion

        #region Verifica se o cliente é revendedor

        /// <summary>
        /// Verifica se o cliente é revendedor.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsRevenda(uint? idCliente)
        {
            return IsRevenda(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente é revendedor.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsRevenda(GDASession sessao, uint? idCliente)
        {
            if (idCliente == null || idCliente == 0)
                return false;

            string sql = "Select Count(*) From cliente Where id_cli=" + idCliente + " And revenda=true";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Retorna prazo máximo de pagamento do cliente

        /// <summary>
        /// Retorna prazo máximo de pagamento do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="exibirRelatorio"></param>
        /// <returns></returns>
        public int GetPrazoMaximoPagto(uint idCliente, bool exibirRelatorio)
        {
            return GetPrazoMaximoPagto(null, idCliente, exibirRelatorio);
        }

        /// <summary>
        /// Retorna prazo máximo de pagamento do cliente
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <param name="exibirRelatorio"></param>
        /// <returns></returns>
        public int GetPrazoMaximoPagto(GDASession sessao, uint idCliente, bool exibirRelatorio)
        {
            string sql = "Select Coalesce(p.numParcelas, 0) From cliente c left join parcelas p on (c.tipoPagto=p.idParcela) Where c.id_Cli=" + idCliente;
            int retorno = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString());

            if (!exibirRelatorio)
                retorno = retorno > 0 ? 1 : 0;

            return retorno;
        }

        /// <summary>
        /// Retorna prazo máximo de pagamento do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public int GetPrazoMaximoPagto(uint idCliente)
        {
            return GetPrazoMaximoPagto(null, idCliente, false);
        }

        /// <summary>
        /// Retorna prazo máximo de pagamento do cliente
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public int GetPrazoMaximoPagto(GDASession sessao, uint idCliente)
        {
            return GetPrazoMaximoPagto(sessao, idCliente, false);
        }

        #endregion

        #region Verifica se o cliente paga ICMS

        public bool IsCobrarIcmsSt(uint idCliente)
        {
            return IsCobrarIcmsSt(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente paga ICMS.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsCobrarIcmsSt(GDASession sessao, uint idCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from cliente where cobrarIcmsSt=true and id_cli=" + idCliente) > 0;
        }

        #endregion

        #region Verifica se o cliente paga IPI

        /// <summary>
        /// Verifica se o cliente paga IPI.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsCobrarIpi(GDASession sessao, uint idCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from cliente where cobrarIpi=true and id_cli=" + idCliente) > 0;
        }

        #endregion

        #region Verifica se o cliente gera orçamento ao finalizar PCP

        /// <summary>
        /// Verifica se o cliente paga IPI.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool GeraOrcamentoPCP(uint idCliente)
        {
            return GeraOrcamentoPCP(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente paga IPI.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool GeraOrcamentoPCP(GDASession sessao, uint idCliente)
        {
            return ObtemValorCampo<bool>(sessao, "Coalesce(gerarOrcamentoPcp, false)", "id_cli=" + idCliente);
        }

        #endregion

        #region Verifica se o cliente é consumidor final

        /// <summary>
        /// Verifica se o cliente é consumidor final
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsConsumidorFinal(uint idCliente)
        {
            return IsConsumidorFinal(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente é consumidor final
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsConsumidorFinal(GDASession session, uint idCliente)
        {
            return ClienteDAO.Instance.GetNome(session, idCliente).ToLower().Trim().Contains("consumidor final");
        }

        #endregion

        #region Autentica usuário

        /// <summary>
        /// Autentica usuário pelo login e senha
        /// </summary>
        /// <param name="login"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public LoginUsuario Autenticacao(string login, string senha, string connString)
        {
            var situacoesCliente = string.Empty;
            if (FinanceiroConfig.ClienteInativoBloqueadoEmitirPedidoComFinalizacaoPeloFinanceiro ||
                (FinanceiroConfig.ClienteInativoBloqueadoEmitirPedidoComConfirmacaoPeloFinanceiro && ProjetoConfig.TelaCadastroParceiros.ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido))
                situacoesCliente = string.Format("{0},{1},{2}", (int)SituacaoCliente.Ativo, (int)SituacaoCliente.Inativo, (int)SituacaoCliente.Bloqueado);
            else
                situacoesCliente = ((int)SituacaoCliente.Ativo).ToString();

            string sql = string.Format("Select Id_Cli From cliente Where Login=?login And Senha=?senha And situacao in ({0})", situacoesCliente);

            object idCliente;

            GDAParameter[] param = new GDAParameter[2];
            param.SetValue(new GDAParameter("?login", login), 0);
            param.SetValue(new GDAParameter("?senha", senha), 1);

            try
            {
                idCliente = objPersistence.ExecuteScalar(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao autenticar. Erro: " + ex.Message);
            }

            if (idCliente == null)
                throw new Exception("Login ou senha inválidos.");

            return GetLogin(Glass.Conversoes.StrParaUint(idCliente.ToString()));
        }

        #endregion

        #region Retorna classe login do usuário

        /// <summary>
        /// Recupera o login do usuário com base no login informado.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public LoginUsuario GetLoginUsuario(string login)
        {
            var cliente = objPersistence.LoadResult("SELECT Id_Cli, Nome, ID_LOJA FROM cliente WHERE Login=?login", new GDAParameter("?login", login))
                .Select(f => new
                {
                    IdCli = f.GetInt32("Id_Cli"),
                    Nome = f.GetString("Nome"),
                    IdLoja = f.GetUInt32("ID_LOJA")
                })
                .FirstOrDefault();

            if (cliente != null)
                return new LoginUsuario
                {
                    IdCliente = (uint)cliente.IdCli,
                    Nome = cliente.Nome,
                    IdLoja = cliente.IdLoja
                };

            return null;
        }

        /// <summary>
        /// Recupera a senha do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetSenha(uint idCliente)
        {
            return objPersistence.LoadResult("SELECT Senha FROM cliente WHERE Id_Cli=?idCliente", new GDAParameter("?idCliente", idCliente))
                .Select(f => f.GetString(0))
                .FirstOrDefault();
        }

        /// <summary>
        /// Retorna informações do usuário logado
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public LoginUsuario GetLogin(uint idCliente)
        {
            return GetLogin(null, idCliente);
        }

        /// <summary>
        /// Retorna informações do usuário logado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public LoginUsuario GetLogin(GDASession sessao, uint idCliente)
        {
            var cliente = new Cliente();

            try
            {
                cliente = objPersistence.LoadOneData(sessao, "Select * From cliente Where id_cli=" + idCliente, "");
            }
            catch
            {
                cliente = null;
            }

            LoginUsuario login = new LoginUsuario();

            if (cliente != null)
            {
                login.IdCliente = (uint)cliente.IdCli;
                login.Nome = cliente.Nome;

                return login;
            }

            return null;
        }

        #endregion

        #region Recupera o percentual mínimo de sinal de pedido

        /// <summary>
        /// Recupera o percentual mínimo de sinal de pedido.
        /// </summary>
        public float? GetPercMinSinalPedido(uint idCliente)
        {
            return GetPercMinSinalPedido(null, idCliente);
        }

        /// <summary>
        /// Recupera o percentual mínimo de sinal de pedido.
        /// </summary>
        public float? GetPercMinSinalPedido(GDASession session, uint idCliente)
        {
            object retorno = objPersistence.ExecuteScalar(session, "select percSinalMin from cliente where id_Cli=" + idCliente);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (float?)float.Parse(retorno.ToString()) : null;
        }

        #endregion

        #region Obtém dados do cliente

        /// <summary>
        /// Obtém id da cidade do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public uint ObtemIdCidade(GDASession session, uint idCliente)
        {
            string sql = "Select idCidade From cliente Where id_cli=" + idCliente;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaUint(obj.ToString());
        }
 
        /// <summary>
        /// Obtém o id da conta bancária do cliente.
        /// </summary>
        public int ObtemIdContaBanco(GDASession session, int idCliente)
        {
            var sql = string.Format("Select IdContaBanco FROM cliente WHERE id_cli={0}", idCliente);

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == string.Empty ? 0 : obj.ToString().StrParaInt();
        }

        /// <summary>
        /// Obtem o id do vendedor
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public uint? ObtemIdFunc(uint idCliente)
        {
            return ObtemIdFunc(null, idCliente);
        }

        /// <summary>
        /// Obtem o id do vendedor
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public uint? ObtemIdFunc(GDASession sessao, uint idCliente)
        {
            return ObtemValorCampo<uint?>(sessao, "idFunc", "id_Cli=" + idCliente);
        }

        public uint? ObtemIdTransportador(uint idCliente)
        {
            return ObtemIdTransportador(null, idCliente);
        }

        public uint? ObtemIdTransportador(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<uint?>(session, "IdTransportador", $"Id_Cli={ idCliente }");
        }

        public bool ObtemHabilitarEditorCad(uint idCliente)
        {
            return ObtemValorCampo<bool>("HabilitarEditorCad", "id_Cli=" + idCliente);
        }

        public uint? ObtemIdTipoCliente(uint idCliente)
        {
            return ObtemValorCampo<uint?>("idTipoCliente", "id_Cli=" + idCliente);
        }

        public string ObtemCpfCnpj(uint idCliente)
        {
            return ObtemCpfCnpj(null, idCliente);
        }

        public string ObtemCpfCnpj(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<string>(session, "cpf_cnpj", "id_Cli=" + idCliente);
        }

        public string ObtemUrlSistema(uint idCliente)
        {
            return ObtemValorCampo<string>("urlSistema", "id_Cli=" + idCliente);
        }

        public string ObtemTelCont(uint idCliente)
        {
            return ObtemValorCampo<string>("tel_cont", "id_Cli=" + idCliente);
        }

        public string ObtemTelRes(uint idCliente)
        {
            return ObtemValorCampo<string>("tel_res", "id_Cli=" + idCliente);
        }

        public string ObtemTelCel(uint idCliente)
        {
            return ObtemValorCampo<string>("tel_cel", "id_Cli=" + idCliente);
        }

        public string ObtemTelefone(uint idCliente)
        {
            string telefone = ObtemTelCont(idCliente);
            if (!String.IsNullOrEmpty(telefone))
                return telefone;

            telefone = ObtemTelCel(idCliente);
            if (!String.IsNullOrEmpty(telefone))
                return telefone;

            telefone = ObtemTelRes(idCliente);
            if (!String.IsNullOrEmpty(telefone))
                return telefone;

            return String.Empty;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a loja associada ao cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(uint idCliente)
        {
            return ObtemIdLoja(null, idCliente);
        }

        /// <summary>
        /// Obtém a loja associada ao cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(GDASession sessao, uint idCliente)
        {
            var sql = "Select id_loja From cliente Where id_cli=" + idCliente;

            var obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaUint(obj.ToString());
        }

        public uint ObtemIdRota(uint idCliente)
        {
            return ObtemIdRota(null, idCliente);
        }

        /// <summary>
        /// Obtém uma rota associada ao cliente
        /// </summary>
        public uint ObtemIdRota(GDASession sessao, uint idCliente)
        {
            return RotaClienteDAO.Instance.ObtemValorCampo<uint>(sessao, "idRota", "idCliente=" + idCliente);
        }

        /// <summary>
        /// Verifica se este cliente ignora o bloqueio de emissão de pedidos caso haja pedido pronto
        /// </summary>
        public bool IgnorarBloqueioPedidoPronto(uint idCliente)
        {
            return IgnorarBloqueioPedidoPronto(null, idCliente);
        }

        /// <summary>
        /// Verifica se este cliente ignora o bloqueio de emissão de pedidos caso haja pedido pronto
        /// </summary>
        public bool IgnorarBloqueioPedidoPronto(GDASession session, uint idCliente)
        {
            var sql = "Select Count(*) From cliente Where ignorarBloqueioPedPronto=true And id_cli=" + idCliente;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Recupera o e-mail do cliente.
        /// </summary>
        public string GetEmail(GDASession sessao, uint idCliente)
        {
            string sql = "select email from cliente where id_Cli=" + idCliente;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        /// <summary>
        /// Retorna o número do celular do cliente que será usado para envio de SMS
        /// </summary>
        public string ObtemCelEnvioSMS(uint idCliente)
        {
            return ObtemCelEnvioSMS(null, idCliente);
        }

        /// <summary>
        /// Retorna o número do celular do cliente que será usado para envio de SMS
        /// </summary>
        public string ObtemCelEnvioSMS(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<string>(session, "tel_cel", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Retorna a situação de um cliente.
        /// </summary>
        public int GetSituacao(uint idCliente)
        {
            return GetSituacao(null, idCliente);
        }

        /// <summary>
        /// Retorna a situação de um cliente.
        /// </summary>
        public int GetSituacao(GDASession sessao, uint idCliente)
        {
            string sql = "select coalesce(situacao," + (int)SituacaoCliente.Inativo + ") from cliente where id_Cli=" + idCliente;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? Glass.Conversoes.StrParaInt(retorno.ToString()) : 2;
        }

        /// <summary>
        /// Verifica se o cliente não recebe e-mail de pedido finalizado no PCP.
        /// </summary>
        public bool NaoReceberEmailPedPcp(uint idCliente)
        {
            return NaoReceberEmailPedPcp(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe e-mail de pedido finalizado no PCP.
        /// </summary>
        public bool NaoReceberEmailPedPcp(GDASession sessao, uint idCliente)
        {
            return ObtemValorCampo<bool>(sessao, "naoRecebeEmailPedPcp", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe e-mail de pedido pronto.
        /// </summary>
        public bool NaoReceberEmailPedPronto(uint idCliente)
        {
            return NaoReceberEmailPedPronto(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe e-mail de pedido pronto.
        /// </summary>
        public bool NaoReceberEmailPedPronto(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<bool>(session, "naoRecebeEmailPedPronto", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe e-mail de liberação.
        /// </summary>
        public bool NaoReceberEmailLiberacao(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<bool>(session, "naoEnviarEmailLiberacao", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe SMS.
        /// </summary>
        public bool NaoRecebeSMS(uint idCliente)
        {
            return NaoRecebeSMS(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente não recebe SMS.
        /// </summary>
        public bool NaoRecebeSMS(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<bool>(session, "naoRecebeSms", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Retorna a tabela que o cliente usa para desconto/acréscimo.
        /// </summary>
        public uint? ObtemTabelaDescontoAcrescimo(GDASession sessao, uint idCliente)
        {
            return ObtemValorCampo<uint?>(sessao, "idTabelaDesconto", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Retorna o percentual de comissão do vendedor associado ao cliente.
        /// </summary>
        public float ObtemPercComissaoFunc(uint idCliente)
        {
            return ObtemValorCampo<float>("percComissaoFunc", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Retorna o percentual de comissão do pedido.
        /// </summary>
        public float ObtemPercentualComissao(uint idCliente)
        {
            return ObtemPercentualComissao(null, idCliente);
        }

        /// <summary>
        /// Retorna o percentual de comissão do pedido.
        /// </summary>
        public float ObtemPercentualComissao(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<float>(session, "percentualComissao", "id_cli=" + idCliente);
        }

        /// <summary>
        /// Obtém o campo obs do cliente
        /// </summary>
        public string ObtemObs(uint idCliente)
        {
            return ObtemValorCampo<string>("obs", "id_Cli=" + idCliente);
        }

        /// <summary>
        /// Obtém o campo obs de liberação do cliente
        /// </summary>
        public string ObtemObsLiberacao(uint idCliente)
        {
            return ObtemValorCampo<string>("obsLiberacao", "id_Cli=" + idCliente);
        }

        public string ObterObsPedido(uint idCliente)
        {
            try
            {
                string obs = string.Empty;

                var percSinalMinimo = GetPercMinSinalPedido(idCliente);
                var limite = ObtemValorCampo<decimal>("limite", "id_Cli=" + idCliente);
                var obsCli = ObtemValorCampo<string>("obs", "id_Cli=" + idCliente);
                var codRota = RotaDAO.Instance.ObtemCodRota(idCliente);
                var isPagamentoAntesProducao = IsPagamentoAntesProducao(idCliente);

                if (isPagamentoAntesProducao && PedidoConfig.LiberarPedido)
                    obs += " <br />ESTE PEDIDO DEVE SER PAGO ANTES DA PRODUÇÃO.";

                if (percSinalMinimo.GetValueOrDefault(0) > 0)
                    obs += " <br />ESTE PEDIDO DEVE SER PAGO COM UM SINAL MÍNIMO DE " + percSinalMinimo + "%.";

                if (limite > 0)
                {
                    var limiteDisp = limite - ContasReceberDAO.Instance.GetDebitos(idCliente, null);
                    obs += " <br />LIMITE DISPONÍVEL PARA COMPRA: " + (limiteDisp > 0 ? limiteDisp : 0).ToString("C");
                }

                obsCli = ((obsCli != null ? obsCli : "").Replace("\n", "<br />").Replace(";", ",") + obs).Trim();
                obsCli += !string.IsNullOrEmpty(codRota) ? " <br />ROTA: " + codRota : "";

                return (obsCli.StartsWith("<br />") ? obsCli.Substring("<br />".Length) : obsCli);
            }
            catch (Exception ex)
            {
                return MensagemAlerta.FormatErrorMsg("Falha ao recuperar observação de cliente.", ex);
            }
        }

        public uint? ObtemTipoPagto(uint idCliente)
        {
            return ObtemTipoPagto(null, idCliente);
        }

        public uint? ObtemTipoPagto(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<uint?>(session, "tipoPagto", "id_Cli=" + idCliente);
        }

        public int? ObtemTipoFiscal(GDASession sessao, uint idCliente)
        {
            return ObtemValorCampo<int?>(sessao, "tipoFiscal", "id_Cli=" + idCliente);
        }

        public uint? ObtemIdFormaPagto(uint idCliente)
        {
            return ObtemIdFormaPagto(null, idCliente);
        }

        public uint? ObtemIdFormaPagto(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<uint?>(session, "idFormaPagto", "id_Cli=" + idCliente);
        }

        public string ObtemUf(uint idCliente)
        {
            return GetElement(idCliente).Uf;
        }

        public decimal ObtemLimite(uint idCliente)
        {
            return ObtemValorCampo<decimal>("limite", "id_Cli=" + idCliente);
        }

        public string ObtemEnderecoCompleto(uint idCliente)
        {
            string where = "id_Cli=" + idCliente;
            uint idCidade = ObtemValorCampo<uint>("idCidade", where);

            Cliente temp = new Cliente()
            {
                Endereco = ObtemValorCampo<string>("endereco", where),
                Numero = ObtemValorCampo<string>("numero", where),
                Compl = ObtemValorCampo<string>("compl", where),
                Bairro = ObtemValorCampo<string>("bairro", where),
                Cidade = CidadeDAO.Instance.GetNome(idCidade),
                Uf = CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + idCidade)
            };

            return temp.EnderecoCompleto;
        }

        /// <summary>
        /// Retorna o endereço completo de entrega do cliente, usado para salvar o endereço nas informações complementares
        /// da nota fisca, solicitado pela Modelo e Termari, chamado 8892.
        /// </summary>
        public string ObtemEnderecoEntregaCompleto(uint idCliente)
        {
            var where = "id_cli=" + idCliente;
            var idCidadeEntrega = ObtemValorCampo<uint>("idCidadeEntrega", where);

            var temp = new Cliente()
            {
                EnderecoEntrega = ObtemValorCampo<string>("enderecoEntrega", where),
                NumeroEntrega = ObtemValorCampo<string>("numeroEntrega", where),
                ComplEntrega = ObtemValorCampo<string>("complEntrega", where),
                BairroEntrega = ObtemValorCampo<string>("bairroEntrega", where),
                CidadeEntrega = CidadeDAO.Instance.GetNome(idCidadeEntrega),
                UfEntrega = CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + idCidadeEntrega)
            };

            // Caso o cliente não possua endereço de entrega deverá ser retornado um texto vazio.
            if (String.IsNullOrEmpty(temp.EnderecoEntrega) && String.IsNullOrEmpty(temp.NumeroEntrega) && String.IsNullOrEmpty(temp.ComplEntrega) &&
                String.IsNullOrEmpty(temp.BairroEntrega) && String.IsNullOrEmpty(temp.CidadeEntrega) && String.IsNullOrEmpty(temp.UfEntrega))
                return String.Empty;

            return temp.EnderecoCompletoEntrega;
        }

        public string ObtemCidadeUf(uint idCliente)
        {
            string where = "id_Cli=" + idCliente;
            uint idCidade = ObtemValorCampo<uint>("idCidade", where);
            var cidade = CidadeDAO.Instance.GetNome(idCidade);
            var uf = CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + idCidade);
            return cidade + "/" + uf;
        }

        public string ObtemObsNfe(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<string>("ObsNfe", $"Id_Cli={ idCliente }");
        }

        /// <summary>
        /// Retorna o campo que será usado no SQL para retornar o nome do cliente.
        /// </summary>
        /// <param name="aliasCliente"></param>
        /// <returns></returns>
        public string GetNomeCliente(string aliasCliente)
        {
            string retorno = Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido ==
                DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                "coalesce({0}.nomeFantasia, {0}.nome)" : "coalesce({0}.nome, {0}.nomeFantasia)";

            return String.Format(retorno, aliasCliente);
        }

        public string ObtemIdsClientesExternos(string nome)
        {
            var sql = @"SELECT DISTINCT IdClienteExterno FROM pedido WHERE ClienteExterno LIKE ?cliExterno";

            var dados = ExecuteMultipleScalar<uint>(sql, new GDAParameter("?cliExterno", "%" + nome + "%"));

            return string.Join(",", dados.Select(f => f.ToString()).ToArray());
        }

        public IList<KeyValuePair<uint, string>> ObtemClientesExternos()
        {
            var sql = @"
                SELECT CONCAT(COALESCE(IdClienteExterno, ''), ',', COALESCE(ClienteExterno ,''))
                FROM pedido
                WHERE COALESCE(Importado, 0) = 1
                	AND (IdClienteExterno IS NOT NULL OR ClienteExterno IS NOT NULL)
                GROUP BY IdClienteExterno, ClienteExterno";

            var dados = ExecuteMultipleScalar<string>(sql);

            return dados.Select(f => new KeyValuePair<uint, string>(f.Split(',')[0].StrParaUint(), f.Split(',')[1])).ToList();
        }

        public string ObtemIdsSubgrupo(uint idCliente)
        {
            return ObtemValorCampo<string>("IdsSubgrupoProd", "id_Cli=" + idCliente);
        }

        public List<uint> ObtemIdsSubgrupoArr(uint idCliente)
        {
            var ids = ObtemValorCampo<string>("IdsSubgrupoProd", "id_Cli=" + idCliente);

            if (string.IsNullOrWhiteSpace(ids))
                return new List<uint>();

            return ids.Split(',').Select(f => f.StrParaUint()).ToList();
        }

        /// <summary>
        /// Informa se o cliente importa pedido
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool ClienteImportacao(uint idCliente)
        {
            return ObtemValorCampo<bool>("Importacao", "Id_Cli = " + idCliente);
        }
 
        /// <summary>
        /// Obtém o CRT do cliente.
        /// </summary>
        public CrtCliente? ObterCrt(GDASession session, int idCliente)
        {
            var crt = ObtemValorCampo<int?>(session, "Crt", string.Format("Id_Cli = {0}", idCliente));

            return crt.HasValue && crt > 0 ? (CrtCliente)crt.Value : (CrtCliente?)null;
        }

        /// <summary>
        /// Retorna o complemento do endereço do cliente.
        /// </summary>
        public string ObterComplementoEndereco(GDASession session, int idCliente)
        {
            return ObtemValorCampo<string>(session, "COALESCE(ComplEntrega, Compl, ComplCobranca)", string.Format("Id_Cli={0}", idCliente));
        }

        /// <summary>
        /// Retorna o indicador da IE do destinatário do cliente.
        /// </summary>
        public IndicadorIEDestinatario ObterIndicadorIEDestinatario(GDASession session, int idCliente)
        {
            return ObtemValorCampo<IndicadorIEDestinatario>(session, "IndicadorIEDestinatario", string.Format("Id_Cli={0}", idCliente));
        }

        /// <summary>
        /// Retorna o saldo devedor e o saldo de credito do cliente
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCliente"></param>
        /// <param name="saldoDevedor"></param>
        /// <param name="saldoCredito"></param>
        public void ObterSaldoDevedor(GDASession session, uint idCliente, out decimal saldoDevedor, out decimal saldoCredito)
        {
            saldoCredito = GetCredito(session, idCliente);

            var contasEmAberto = ContasReceberDAO.Instance.ObterValorParaSaldoDevedor(session, idCliente);
            var chequesEmAberto = ChequesDAO.Instance.ObterValorParaSaldoDevedor(session, idCliente);

            saldoDevedor = contasEmAberto + chequesEmAberto;
        }

        public decimal? ObterPorcentagemDescontoEcommerce(GDASession sessao, int idCliente)
        {
            return ObtemValorCampo<decimal?>(sessao, "DescontoEcommerce", "id_Cli=" + idCliente);
        }

        #endregion

        #region Atualiza data da última compra

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualiza data da última compra
        /// </summary>
        /// <param name="idCliente"></param>
        public void AtualizaUltimaCompra(uint idCliente)
        {
            AtualizaUltimaCompra(null, idCliente);
        }

        /// <summary>        
        /// Atualiza data da última compra
        /// </summary>
        /// <param name="idCliente"></param>
        public void AtualizaUltimaCompra(GDASession sessao, uint idCliente)
        {
            objPersistence.ExecuteCommand(sessao, "Update cliente Set dt_Ult_Compra=now() Where id_Cli=" + idCliente);
        }

        #endregion

        #region Atualiza total comprado

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualiza o total comprado pelo cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void AtualizaTotalComprado(uint idCliente)
        {
            AtualizaTotalComprado(null, idCliente);
        }

        /// <summary>        
        /// Atualiza o total comprado pelo cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void AtualizaTotalComprado(GDASession sessao, uint idCliente)
        {
            string totalComprado = @" 
                Update cliente set totalComprado=(
                    Select Round(Sum(p.total), 2) From pedido p Where p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado + @"
                    And p.tipovenda<>" + (int)Pedido.TipoVendaPedido.Garantia + @" 
                    And p.tipovenda<>" + (int)Pedido.TipoVendaPedido.Reposição + @"
                    And p.IdCli=" + idCliente + @"
                ) 
                Where id_cli=" + idCliente;

            objPersistence.ExecuteCommand(sessao, totalComprado);
        }

        #endregion

        #region Atualiza data da última consulta no sintegra

        /// <summary>
        /// Atualiza data da última consulta no sintegra
        /// </summary>
        /// <param name="cpfCnpj"></param>
        public void AtualizaUltimaConsultaSintegra(string cpfCnpj)
        {
            try
            {
                cpfCnpj = Formatacoes.LimpaCpfCnpj(cpfCnpj);

                string sql = "Update cliente Set DTULTCONSINTEGRA=now(), USUARIOULTCONSINTEGRA=" +
               UserInfo.GetUserInfo.CodUser + " where replace(replace(replace(replace(cpf_Cnpj, '.', ''), ' ', ''), '-', ''), '/', '')=?cpfCnpj";
                objPersistence.ExecuteCommand(sql, new GDAParameter("?cpfCnpj", cpfCnpj));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Verifica se o cliente tem que pagar o pedido antes dele ser produzido

        /// <summary>
        /// Verifica se o cliente tem que pagar o pedido antes dele ser produzido.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsPagamentoAntesProducao(uint idCliente)
        {
            return IsPagamentoAntesProducao(null, idCliente);
        }

        /// <summary>
        /// Verifica se o cliente tem que pagar o pedido antes dele ser produzido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public bool IsPagamentoAntesProducao(GDASession sessao, uint idCliente)
        {
            if (!PedidoConfig.LiberarPedido)
                return true;

            string sql = "select count(*) from cliente where pagamentoAntesProducao=true and id_Cli=" + idCliente;
            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Situação do cliente

        /// <summary>
        /// Altera a situação de um cliente, ativando ou inativando o cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public void AlteraSituacao(uint idCliente)
        {
            if (GetNome(idCliente).ToLower() == "consumidor final" && GetSituacao(idCliente) == (int)SituacaoCliente.Ativo)
                throw new Exception("Consumidor Final não pode ser inativado/cancelado.");

            Cliente cli = GetElementByPrimaryKey(idCliente);
            cli.Situacao = cli.Situacao == 1 ? 2 : 1;
            cli.IdRota = (int)ClienteDAO.Instance.ObtemIdRota(idCliente);
            cli.IdRota = cli.IdRota > 0 ? cli.IdRota : cli.IdRota = null;
            Update(cli);
        }

        #endregion

        #region Retorna o cliente usado nos pedidos de produção

        /// <summary>
        /// Retorna o cliente usado nos pedidos de produção.
        /// </summary>
        /// <returns></returns>
        public uint GetClienteProducao()
        {
            // Busca o cliente mais usado dos pedidos para produção
            string sql = "select idCli from pedido where tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + " group by idCli order by count(*) desc limit 1";
            uint? retorno = ExecuteScalar<uint?>(sql);
            if (retorno > 0)
                return retorno.Value;

            // Tenta buscar um cliente com o mesmo CNPJ
            sql = @"select id_Cli from cliente where replace(replace(replace(cpf_Cnpj, '.', ''), '-', ''), '/', '') in (
                select replace(replace(replace(cnpj, '.', ''), '-', ''), '/', '') from loja) limit 1";

            retorno = ExecuteScalar<uint?>(sql);
            if (retorno > 0)
                return retorno.Value;

            // Tenta buscar um cliente com o mesmo nome
            sql = @"select id_Cli from cliente c where exists (select * from loja l where l.nomeFantasia like 
                concat('%', c.nome, '%') or l.razaoSocial like concat('%', c.nome, '%')) limit 1";

            retorno = ExecuteScalar<uint?>(sql);
            return retorno.GetValueOrDefault(0);
        }

        #endregion

        #region Retorna os clientes que tem o mesmo cnpj da loja

        /// <summary>
        /// Retorna os clientes que tem o mesmo cnpj da loja
        /// </summary>
        /// <returns></returns>
        public List<Cliente> GetClienteLoja()
        {
            var cnpj = LojaDAO.Instance.ObtemCnpj();

            string cnpjs = "";

            foreach (var c in cnpj)
                cnpjs += "'" + c + "',";

            string sql = "select * from cliente where cpf_Cnpj IN(" + cnpjs.Trim(',') + ")";
            return objPersistence.LoadData(sql);
        }

        #endregion

        #region Altera o vendedor de um ou mais clientes

        /// <summary>
        /// Altera o vendedor de um ou mais clientes.
        /// </summary>
        /// <param name="idsClientes"></param>
        /// <param name="idFunc"></param>
        public void AlteraVendedor(string idsClientes, uint? idFunc)
        {
            Cliente[] cli = objPersistence.LoadData("select * from cliente where id_Cli in (" +
                idsClientes.TrimEnd(',') + ")").ToArray();

            foreach (Cliente c in cli)
            {
                c.IdRota = (int)ObtemIdRota((uint)c.IdCli);
                c.IdFunc = (int?)idFunc;
                Update(c);
            }
        }

        #endregion

        #region Verifica se os cheques devem ser inseridos

        /// <summary>
        /// Verifica se os cheques devem ser inseridos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="cheques"></param>
        public void ValidaInserirCheque(GDASession sessao, uint idCliente, IEnumerable<Cheques> cheques, 
            string idsPedidos, string idsContasR, string idsChequesR, bool validarLimite)
        {
            if (idCliente == 0 || objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from cliente where id_Cli=" + idCliente) == 0)
                throw new Exception("Cliente não encontrado. Para inserir um cheque é necessário informar um cliente.");

            // Verifica se o cliente bloqueia os cheques
            decimal limite = ObtemValorCampo<decimal>(sessao, "limite", "id_Cli=" + idCliente);

            /* Chamado 15815.
             * Esta condição foi alterada para que a verificação de limite de cheque próprio e cheque de terceiro
             * seja separada da verificação de limite por CPF/CNPJ. */
            if (limite > 0 && FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite)
            {
                // Recupera o total de cheques que serão cadastrados
                decimal totalChequeTerceiro = 0, totalChequeProprio = 0;
                // Separa os cheques pelos códigos.
                string idsChequeTerceiro = string.Empty, idsChequeProprio = string.Empty;
                // Recupera o CPF/CNPJ do cliente para verificar se o cheque é de terceiros ou próprio.
                var cpfCnpjCliente = ClienteDAO.Instance.ObtemCpfCnpj(idCliente).Replace(".", "").Replace("/", "").Replace("-", "");

                foreach (Cheques c in cheques)
                {
                    // Separa os cheques por terceiro e próprio, de acordo com o CPF/CNPJ.
                    var cpfCnpjCheque = !String.IsNullOrEmpty(c.CpfCnpj) ? c.CpfCnpj.Replace(".", "").Replace("/", "").Replace("-", "") : "";
                    // Caso o CPF/CNPJ seja diferente do CPF/CNPJ do cliente então o cheque é considerado como cheque de terceiros.
                    if (String.IsNullOrEmpty(cpfCnpjCheque) || cpfCnpjCheque != cpfCnpjCliente)
                        totalChequeTerceiro += c.Valor;
                    // Caso o CPF/CNPJ seja igual ao CPF/CNPJ do cliente então o cheque é considerado como cheque próprio.
                    else if (cpfCnpjCheque == cpfCnpjCliente)
                        totalChequeProprio += c.Valor;
                }

                var diasConsiderarChequeCompensado = FinanceiroConfig.FinanceiroRec.DiasConsiderarChequeCompensado;
                var situacoesCheque = (int)Cheques.SituacaoCheque.EmAberto + ", " + (int)Cheques.SituacaoCheque.Devolvido + ", " +
                                (int)Cheques.SituacaoCheque.Trocado + ", " + (int)Cheques.SituacaoCheque.Protestado;

                /* Caso haja alteração neste método altere também o método ContasReceberDAO.Instance.SqlDebitos */
                var sqlCheques = @"
                    SELECT SUM(c.Valor - COALESCE(c.ValorReceb, 0)) AS ValorVec
                    FROM cheques c
                        INNER JOIN cliente cli ON (c.IdCliente=cli.Id_Cli)
                    WHERE c.IdCliente=?IdCliente
                        AND Replace(Replace(Replace(c.CpfCnpj, '.', ''), '/', ''), '-', ''){0}Replace(Replace(Replace(cli.Cpf_Cnpj, '.', ''), '/', ''), '-', '')
                        AND (c.Valor - COALESCE(c.ValorReceb, 0)) > 0
                        AND (c.Situacao IN (" + situacoesCheque + @")
                        OR (c.Situacao=" + (int)Cheques.SituacaoCheque.Compensado + " AND DataVenc > DATE_ADD(NOW(), INTERVAL " +
                            diasConsiderarChequeCompensado + " DAY)))";

                // Recupera o limite de cheque de terceiros e próprio utilizados.
                var debitoChequeProprio = Glass.Conversoes.StrParaDecimal(ChequesDAO.Instance.GetValoresCampo(String.Format(sqlCheques, "="), "ValorVec",
                    new GDAParameter("?IdCliente", idCliente)));
                var debitoChequeTerceiro = Glass.Conversoes.StrParaDecimal(ChequesDAO.Instance.GetValoresCampo(String.Format(sqlCheques, "<>"), "ValorVec",
                    new GDAParameter("?IdCliente", idCliente)));

                // Recupera o limite disponível
                var limiteGasto = ContasReceberDAO.Instance.GetDebitos(sessao, idCliente, idsPedidos, idsContasR, idsChequesR);
                var limiteDisp = limite - limiteGasto;

                // Verifica se o valor dos cheques é maior que o limite disponível
                if (validarLimite && (totalChequeTerceiro + totalChequeProprio) > limiteDisp)
                    throw new Exception("O valor do(s) cheque(s) cadastrados (" + (totalChequeTerceiro + totalChequeProprio).ToString("C") + ") " +
                        "é maior que o limite disponível " + limiteDisp.ToString("C") + ".");

                // Valida o percentual de 50% se o cliente assim estiver indicado
                else if (ObtemValorCampo<bool>(sessao, "bloquearRecebChequeLimite", "id_Cli=" + idCliente) ||
                    ObtemValorCampo<bool>(sessao, "bloquearRecebChequeProprioLimite", "id_Cli=" + idCliente))
                {
                    #region Valida cheque próprio

                    // Verifica se o valor dos cheques próprio é maior que a metade do limite total
                    if (totalChequeProprio > (limite / 2))
                        throw new Exception("O valor dos cheques próprio " + totalChequeProprio.ToString("C") + " é maior que 50% do limite total " +
                            limite.ToString("C") + ".");

                    // Verifica se o valor dos cheques próprio, somado aos cheques próprio em aberto, é maior que a metade do limite total
                    else if (validarLimite && (debitoChequeProprio + totalChequeProprio) > (limite / 2))
                        throw new Exception("O valor dos cheques próprio " + totalChequeProprio.ToString("C") + ", somado aos cheques próprio em aberto " +
                            debitoChequeProprio.ToString("C") + ", é maior que 50% do limite total " + limite.ToString("C") + ".");

                    #endregion

                    #region Valida cheque terceiro

                    // Verifica se o valor dos cheques de terceiros é maior que a metade do limite total
                    if (totalChequeTerceiro > (limite / 2))
                        throw new Exception("O valor dos cheques de terceiros " + totalChequeTerceiro.ToString("C") + " é maior que 50% do limite total " +
                            limite.ToString("C") + ".");

                    // Verifica se o valor dos cheques de terceiros, somado aos cheques de terceiros em aberto, é maior que a metade do limite total
                    else if (validarLimite && (debitoChequeTerceiro + totalChequeTerceiro) > (limite / 2))
                        throw new Exception("O valor dos cheques de terceiros " + totalChequeTerceiro.ToString("C") +
                            ", somado aos cheques de terceiros em aberto " +
                            debitoChequeTerceiro.ToString("C") + ", é maior que 50% do limite total " + limite.ToString("C") + ".");

                    #endregion
                }
            }
        }

        #endregion

        #region Busca um cliente por CPF/CNPJ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public Cliente GetByCpfCnpj(string cpfCnpj)
        {
            return GetByCpfCnpj(null, cpfCnpj);
        }

        /// <summary>
        /// Busca um cliente por CPF/CNPJ.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public Cliente GetByCpfCnpj(GDASession session, string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return null;

            cpfCnpj = cpfCnpj.Replace("/", "").Replace("-", "").Replace(" ", "").Replace(".", "");

            string sql = "select * from cliente where replace(replace(replace(replace(cpf_Cnpj, '.', ''), ' ', ''), '-', ''), '/', '')=?cpfCnpj";
            List<Cliente> itens = objPersistence.LoadData(session, sql, new GDAParameter("?cpfCnpj", cpfCnpj));
            return itens.Count > 0 ? itens[0] : null;
        }

        public uint? ObterIdPorCpfCnpj(GDASession sessao, string cpfCnpj)
        {
            if (string.IsNullOrEmpty(cpfCnpj))
                return null;

            cpfCnpj = cpfCnpj.Replace("/", "").Replace("-", "").Replace(" ", "").Replace(".", "");

            string sql = "select ID_CLI from cliente where replace(replace(replace(replace(cpf_Cnpj, '.', ''), ' ', ''), '-', ''), '/', '')=?cpfCnpj";
            return ExecuteScalar<uint?>(sessao, sql, new GDAParameter("?cpfCnpj", cpfCnpj));
        }

        #endregion

        #region Busca clientes com vendas

        /// <summary>
        /// Busca clientes com vendas.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Cliente[] GetClientesVendas(uint idLoja, string dataIni, string dataFim)
        {
            string sql = @"select * from cliente c where id_Cli in (
                select * from (
                    select p.idCli from pedido p
                        left join produtos_liberar_pedido plp on (p.idPedido=plp.idPedido)
                        left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                    where {0}>=?dataIni and {0}<=?dataFim and p.situacao in (" + (int)Pedido.SituacaoPedido.Confirmado + "," +
                    (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")";

            if (PedidoConfig.LiberarPedido)
                sql = String.Format(sql, "lp.dataLiberacao") + " and lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;
            else
                sql = String.Format(sql, "p.dataConf");

            if (idLoja > 0)
                sql += " and p.idLoja=" + idLoja;

            sql += " group by p.idCli order by sum(p.total) desc limit 0,15) as temp)";
            return objPersistence.LoadData(sql, new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")),
                new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59"))).ToArray();
        }

        #endregion

        #region Busca clientes para alterar a rota ou revenda

        public Cliente[] ObterClientesRotaRevenda(string rota, string revenda)
        {
            string sql = "select * from cliente c where 1 ";

            if (rota == null) rota = "";
            if (revenda == null) revenda = "";

            string[] ro = rota.Split(',');
            string[] re = revenda.Split(',');

            for (int i = 0; i < ro.Length; i++)
            {
                if (ro[i] == "0")
                    ro[i] = ro[i].Replace("0", "c.ID_CLI not in (select r.IDCLIENTE from rota_cliente r)");
                if (ro[i] == "1")
                    ro[i] = ro[i].Replace("1", "c.ID_CLI in (select r.IDCLIENTE from rota_cliente r)");
            }

            for (int i = 0; i < re.Length; i++)
            {
                if (re[i] == "0")
                    re[i] = re[i].Replace("0", "c.REVENDA = 0");
                if (re[i] == "1")
                    re[i] = re[i].Replace("1", "c.revenda=1");
            }

            sql += (string.Join(" or ", ro) != "" ? "and " + string.Join(" or ", ro) : "") + (string.Join(" or ", re) != "" ? " and " + string.Join(" or ", re) : "") + " order by Nome";

            return objPersistence.LoadData(sql).ToArray();
        }

        public void IgnorarBloqueioPedidoPronto(string ids, bool ignorar)
        {
            var aux = ids.Split(',');

            string sql = "update cliente set IgnorarBloqueioPedPronto=?i where id_cli in (" + ids + ")";
            objPersistence.ExecuteCommand(sql, new GDAParameter("?i", ignorar));
        }

        public void BloquearPedidoContaVencida(string ids, bool bloquear)
        {
            var aux = ids.Split(',');

            string sql = "update cliente set BloquearPedidoContaVencida=?i where id_cli in (" + ids + ")";
            objPersistence.ExecuteCommand(sql, new GDAParameter("?i", bloquear));
        }

        /// <summary>
        /// Busca vários clientes pelos seus ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Cliente[] GetByString(string ids)
        {
            string sql = "select * from cliente where id_Cli in (" + ids.Trim(',') + ")";
            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Busca de Clientes x Tabelas de Desconto

        /// <summary>
        /// Recupera os clientes de que possuem uma tabela de desconto
        /// </summary>
        /// <returns></returns>
        public Cliente[] GetListForRptTabelaDesconto()
        {
            string sql = @"Select c.*, da.descricao as DescontoAcrescimo From cliente c 
                Inner Join tabela_desconto_acrescimo_cliente da On (c.IdTabelaDesconto=da.IdTabelaDesconto)";
            return objPersistence.LoadData(sql).ToArray(); ;
        }

        #endregion

        #region Troca senha

        /// <summary>
        /// Altera a senha do cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="senha"></param>
        public void AlteraSenha(uint idCliente, string senha)
        {
            objPersistence.ExecuteCommand("Update cliente Set senha=?senha Where id_cli=" + idCliente, 
                new GDAParameter("?senha", senha));
        }

        #endregion

        #region Limite de cheques por CPF/CNPJ

        /// <summary>
        /// Retorna o limite de cheques por CPF/CNPJ de um cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtemLimiteCheques(uint idCliente)
        {
            return ObtemLimiteCheques((GDASession)null, idCliente);
        }

        /// <summary>
        /// Retorna o limite de cheques por CPF/CNPJ de um cliente.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtemLimiteCheques(GDASession session, uint idCliente)
        {
            return ObtemValorCampo<decimal>(session, "limiteCheques", "id_Cli=" + idCliente);
        }

        #endregion

        #region Métodos Sobrescritos

        [Obsolete("Migrado para a entidade")]
        public override uint Insert(Cliente objInsert)
        {
            FilaOperacoes.CadastrarCliente.AguardarVez();

            try
            {
                // Verifica se foi cadastrado um cliente com o mesmocpf/cnpj dos últimos 10 segundos, para evitar duplicidade
                if (ExecuteScalar<bool>("Select Count(*)>0 from cliente Where cpf_cnpj=?cpfCnpj And dataCad>=DATE_ADD(now(), INTERVAL -10 second)",
                    new GDAParameter("?cpfCnpj", objInsert.CpfCnpj)))
                    return 0;

                if (objInsert.DataLimiteCad.HasValue && objInsert.DataLimiteCad.Value < DateTime.Now.Date)
                    throw new Exception("A data limite do cadastro não pode ser menor que a data atual");

                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                objInsert.DataCad = DateTime.Now;

                // Não permite que o nome do cliente possua ' ou "
                objInsert.Nome = objInsert.Nome != null ? objInsert.Nome.Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\"", "") : null;
                objInsert.NomeFantasia = objInsert.NomeFantasia != null ? objInsert.NomeFantasia.Replace("'", "").Replace("\t", "").Replace("\n", "").Replace("\"", "") : null;

                // Não permite que seja inserido "INSENTO"
                if (objInsert.RgEscinst != null && (objInsert.RgEscinst.ToLower().Contains("insento") || objInsert.RgEscinst.ToLower().Contains("insenta")))
                    objInsert.RgEscinst.Replace("INSENTO", "ISENTO").Replace("INSENTA", "ISENTO");

                // Cópia o endereço do cliente para o endereço de entrega caso o usuário
                // não informe o endereço de entrega
                if (String.IsNullOrEmpty(objInsert.EnderecoEntrega))
                {
                    objInsert.EnderecoEntrega = objInsert.Endereco;

                    if (objInsert.IdCidade.HasValue)
                        objInsert.IdCidadeEntrega = objInsert.IdCidade.Value;

                    objInsert.BairroEntrega = objInsert.Bairro;
                    objInsert.NumeroEntrega = objInsert.Numero;
                    objInsert.ComplEntrega = objInsert.Compl;
                    objInsert.CepEntrega = objInsert.Cep;
                }

                objInsert.IdCli = (int)base.Insert(objInsert);

                // Atualiza a rota do cliente
                if (objInsert.IdRota > 0)
                    RotaClienteDAO.Instance.AssociaCliente(null, (uint)objInsert.IdRota.Value, (uint)objInsert.IdCli);

                return (uint)objInsert.IdCli;
            }
            finally
            {
                FilaOperacoes.CadastrarCliente.ProximoFila();
            }
        }

        public override int Update(GDASession session, Cliente objUpdate)
        {
            if (objUpdate.Nome.ToLower() == "consumidor final" && objUpdate.Situacao != (int)SituacaoCliente.Ativo)
                throw new Exception("Consumidor Final não pode ser inativado/cancelado.");

            if (objUpdate.DataLimiteCad.HasValue && objUpdate.DataLimiteCad.Value < DateTime.Now.Date)
                throw new Exception("A data limite do cadastro não pode ser menor que a data atual");

            // Se o funcionário não for administrador ou financeiro/financeiro geral, não pode alterar o limite do cliente
            LoginUsuario login = UserInfo.GetUserInfo;
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                !Config.PossuiPermissao(Config.FuncaoMenuCadastro.AlterarLimiteCliente))
                objUpdate.Limite = ObtemValorCampo<decimal>(session, "limite", "id_Cli=" + objUpdate.IdCli);

            // Inclui as informações de alteração
            objUpdate.DataAlt = DateTime.Now;
            objUpdate.UsuAlt = (int)UserInfo.GetUserInfo.CodUser;

            // Não permite que o nome do cliente possua ' ou "
            objUpdate.Nome = objUpdate.Nome != null ? objUpdate.Nome.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "") : null;
            objUpdate.NomeFantasia = objUpdate.NomeFantasia != null ? objUpdate.NomeFantasia.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "") : null;

            // Recupera crédito e data da última compra
            objUpdate.Credito = ObtemValorCampo<decimal>(session, "credito", "id_Cli=" + objUpdate.IdCli);
            objUpdate.DtUltCompra = ObtemValorCampo<DateTime?>(session, "dt_Ult_Compra", "id_Cli=" + objUpdate.IdCli);

            //Recupera a data e usuario da última consulta no sintegra
            objUpdate.DtUltConSintegra = ObtemValorCampo<DateTime?>(session, "DTULTCONSINTEGRA", "id_Cli=" + objUpdate.IdCli);
            objUpdate.UsuUltConSintegra = ObtemValorCampo<int?>(session, "USUARIOULTCONSINTEGRA", "id_Cli=" + objUpdate.IdCli);

            // Não permite que seja inserido "INSENTO"
            if (objUpdate.RgEscinst != null && (objUpdate.RgEscinst.ToLower().Contains("insento") || objUpdate.RgEscinst.ToLower().Contains("insenta")))
                objUpdate.RgEscinst = objUpdate.RgEscinst.ToUpper().Replace("INSENTO", "ISENTO").Replace("INSENTA", "ISENTO");

            // Atualiza a rota do cliente
            uint idRotaAtual = ObtemIdRota(session, (uint)objUpdate.IdCli);
            if (objUpdate.IdRota > 0)
            {
                if (idRotaAtual == 0)
                    RotaClienteDAO.Instance.AssociaCliente(session, (uint)objUpdate.IdRota.Value, (uint)objUpdate.IdCli);
                else if (idRotaAtual != objUpdate.IdRota)
                {
                    RotaClienteDAO.Instance.DesassociaCliente(session, idRotaAtual, (uint)objUpdate.IdCli);
                    RotaClienteDAO.Instance.AssociaCliente(session, (uint)objUpdate.IdRota.Value, (uint)objUpdate.IdCli);
                }
            }
            else if (idRotaAtual > 0)
                RotaClienteDAO.Instance.DesassociaCliente(session, idRotaAtual, (uint)objUpdate.IdCli);

            // Cópia o endereço do cliente para o endereço de entrega caso o usuário
            // não informe o endereço de entrega
            if (String.IsNullOrEmpty(objUpdate.EnderecoEntrega))
            {
                objUpdate.EnderecoEntrega = objUpdate.Endereco;

                if (objUpdate.IdCidade.HasValue)
                    objUpdate.IdCidadeEntrega = objUpdate.IdCidade.Value;

                objUpdate.BairroEntrega = objUpdate.Bairro;
                objUpdate.NumeroEntrega = objUpdate.Numero;
                objUpdate.ComplEntrega = objUpdate.Compl;
                objUpdate.CepEntrega = objUpdate.Cep;
            }

            LogAlteracaoDAO.Instance.LogCliente(session, objUpdate);
            return base.Update(session, objUpdate);
        }

        [Obsolete("Migrado para a entidade")]
        public override int Update(Cliente objUpdate)
        {
            return Update(null, objUpdate);
        }

        [Obsolete("Migrado para a entidade")]
        public override int Delete(Cliente objDelete)
        {
            string sql = "select count(*) from pedido where idCli=" + objDelete.IdCli;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há pedidos associados ao mesmo.");

            sql = "select count(*) from orcamento where idCliente=" + objDelete.IdCli;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há orçamentos associados ao mesmo.");

            sql = "select count(*) from projeto where idCliente=" + objDelete.IdCli;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há projetos associados ao mesmo.");

            sql = "select count(*) from nota_fiscal where idCliente=" + objDelete.IdCli;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há notas fiscais associadas ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogCliente((uint)objDelete.IdCli);
            
            sql = "delete from parcelas_nao_usar where idcliente=" + objDelete.IdCli;
            objPersistence.ExecuteCommand(sql);

            return base.Delete(objDelete);
        }

        [Obsolete("Migrado para a entidade")]
        public override int DeleteByPrimaryKey(uint key)
        {
            string sql = "select count(*) from pedido where idCli=" + key;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há pedidos associados ao mesmo.");

            sql = "select count(*) from orcamento where idCliente=" + key;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há orçamentos associados ao mesmo.");

            sql = "select count(*) from projeto where idCliente=" + key;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há projetos associados ao mesmo.");

            sql = "select count(*) from nota_fiscal where idCliente=" + key;
            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("Há notas fiscais associadas ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogCliente(key);
            return base.DeleteByPrimaryKey(key);
        }

        #endregion

        #region AtualizarUsoLimiteCliente

        public void AtualizarUsoLimiteCliente()
        {
            var idsClientes = ExecuteMultipleScalar<uint>("SELECT id_cli FROM cliente");
            bool buscarCheques = FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite;

            foreach (var id in idsClientes)
            {
                var totalCheques = buscarCheques ? ContasReceberDAO.Instance.GetDebitosByTipo(id, ContasReceberDAO.TipoDebito.ChequesTotal) : 0;
                var totalContasRec = ContasReceberDAO.Instance.GetDebitosByTipo(id, ContasReceberDAO.TipoDebito.ContasAReceberTotal);
                var totalPedidos = ContasReceberDAO.Instance.GetDebitosByTipo(id, ContasReceberDAO.TipoDebito.PedidosEmAberto);

                var usoLimite = totalCheques + totalContasRec + totalPedidos;

                objPersistence.ExecuteCommand("UPDATE cliente SET usoLimite = ?uso WHERE id_cli = " + id, new GDAParameter("?uso", usoLimite));
            }
        }

        #endregion

        #region Validações

        /// <summary>
        /// Valida se o cliente pode usar o produto informado.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool ValidaSubgrupo(uint idCliente, string codInterno)
        {
            var ids = ObtemIdsSubgrupoArr(idCliente);

            if (ids.Count == 0)
                return true;

            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var idSubgrupo = ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd);

            return idSubgrupo.GetValueOrDefault(0) > 0 && ids.Contains((uint)idSubgrupo.GetValueOrDefault(0));
        }

        #endregion

        #region Bloqueio de Clientes

        /// <summary>
        /// Atualiza a situação de um cliente.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idsCliente"></param>
        /// <param name="motivo"></param>
        public void AtualizaSituacaoComTransacao(SituacaoCliente situacao, string idsCliente, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    //var idFunc = UserInfo.GetUserInfo.CodUser;

                    //if (idFunc == 0)
                    //    throw new Exception("Falha ao alterar situação do cliente, funcionário da alteração nulo.");

                    if (string.IsNullOrWhiteSpace(idsCliente))
                        throw new Exception("Falha ao alterar situação do cliente, nenhum cliente foi informado.");

                    if (string.IsNullOrWhiteSpace(motivo))
                        throw new Exception("Falha ao alterar situação do cliente, nenhum motivo foi informado.");

                    var situacaoStr = string.Format("{0} - {1}", situacao.Translate().Format(), motivo);

                    LogAlteracaoDAO.Instance.LogSituacaoCliente(transaction, situacaoStr, idsCliente);

                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE cliente SET Situacao = {0}, Obs=IF(INSTR(COALESCE(Obs, ''), '{1}') > 0, Obs, CONCAT(COALESCE(Obs, ''), ' {1}')) WHERE Id_Cli IN ({2})",
                        (int)situacao, motivo, idsCliente));

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("AtualizaSituacaoComTransacao", ex);
                }
            }
        }

        /// <summary>
        /// Bloqueia os clientes que tem pedidos prontos há mais de X dias.
        /// </summary>
        public void BloquearProntosNaoLiberados()
        {
            var ids = PedidoDAO.Instance.GetClientesAtrasados();

            if (ids.Length > 0)
            {
                var idsClientes = string.Join(",", ids.Select(f => f.ToString()));
                var motivoBloqueio = "Possui um ou mais pedidos prontos não liberados há mais dias do que o permitido.";

                AtualizaSituacaoComTransacao(SituacaoCliente.Bloqueado, idsClientes, motivoBloqueio);
            }
        }

        /// <summary>
        /// Inativa os clientes de acordo com sua última compra.
        /// </summary>
        public void InativarPelaUltimaCompra()
        {
            if (FinanceiroConfig.PeriodoInativarClienteUltimaCompra == 0)
                return;

            // Para verificar se o cliente não compra há mais de x dias, além de fazer esta verificação, considera também
            // os clientes que nunca fizeram compras e que foram cadastrados há mais de x dias
            string sql = @"select c.id_Cli from cliente c 
                    {0}
                where lower(c.nome)<>'consumidor final' and c.situacao=" + (int)SituacaoCliente.Ativo + @"
                    and ((c.dt_Ult_Compra is null and c.dataCad<=date_sub(now(), interval " + FinanceiroConfig.PeriodoInativarClienteUltimaCompra + @" day)) 
                    or c.dt_Ult_Compra<=date_sub(now(), interval " + FinanceiroConfig.PeriodoInativarClienteUltimaCompra + " day))";

            string join = String.Empty;

            int numeroDiasConsiderar = FinanceiroConfig.NumeroDiasIgnorarClientesRecemAtivosInativarAutomaticamente;

            if (numeroDiasConsiderar > 0)
            {
                join = @"left join (
		                select * from (
			                select idRegistroAlt, dataAlt
			                from log_alteracao
			                where tabela=" + (int)LogAlteracao.TabelaAlteracao.Cliente + @" and campo='Situação' and valorAtual='Ativo'
			                order by numEvento desc
		                ) as temp
		                group by idRegistroAlt
	                ) l on (l.idRegistroAlt=c.id_Cli)";

                sql += " and (l.idRegistroAlt is null or date_add(l.dataAlt, interval " + numeroDiasConsiderar + " day) < now())";
            }

            string idsClientes = GetValoresCampo(String.Format(sql, join), "id_Cli");

            // Inativa os clientes
            if (!string.IsNullOrWhiteSpace(idsClientes))
            {
                var motivoBloqueio = string.Format("Última compra há mais de {0} dias.", FinanceiroConfig.PeriodoInativarClienteUltimaCompra);
                AtualizaSituacaoComTransacao(SituacaoCliente.Inativo, idsClientes, motivoBloqueio);
            }
        }

        /// <summary>
        /// Inativa os clientes que não foram pesquisados no sintegra há mais de X dias
        /// </summary>
        public void InativaPelaUltConSintegra()
        {
            if (FinanceiroConfig.PeriodoInativarClienteUltimaConsultaSintegra < 1)
                return;

            var sql = @"SELECT ID_CLI 
                        FROM cliente 
                        WHERE LOWER(NOME) <> 'consumidor final' 
                            AND LOWER(TIPO_PESSOA) = 'j' 
                            AND DTULTCONSINTEGRA <= date_sub(now(), interval " + FinanceiroConfig.PeriodoInativarClienteUltimaConsultaSintegra + " day)";

            var ids = ExecuteMultipleScalar<int>(sql);

            var idsClientes = string.Join(",", ids.Select(f => f.ToString()));

            if (!string.IsNullOrWhiteSpace(idsClientes))
            {
                var motivoBloqueio = string.Format("Última pesquisa ao cadastro do sintegra há mais de {0} dias. ", FinanceiroConfig.PeriodoInativarClienteUltimaConsultaSintegra);
                AtualizaSituacaoComTransacao(SituacaoCliente.Inativo, idsClientes, motivoBloqueio);
            }
        }

        /// <summary>
        /// Inativa os clientes que estão com a data limite do cadastro vencidas
        /// </summary>
        public void InativaPelaDataLimiteCad()
        {
            var sql = @"
                SELECT Id_cli 
                FROM cliente
                WHERE LOWER(nome) <> 'consumidor final'
                    AND dataLimiteCad IS NOT NULL
                    AND date(dataLimiteCad) < curdate()
                    AND situacao=" + (int)SituacaoCliente.Ativo;

            var ids = ExecuteMultipleScalar<int>(sql);

            var idsCliente = string.Join(",", ids.Select(f => f.ToString()));

            if (!string.IsNullOrWhiteSpace(idsCliente))
            {
                var motivoBloqueio = "Data limite do cadastro vencida.";
                AtualizaSituacaoComTransacao(SituacaoCliente.Inativo, idsCliente, motivoBloqueio);
            }
        }

        #endregion
    }
}