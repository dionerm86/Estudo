using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Colosoft.Business;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio de clientes.
    /// </summary>
    public class ClienteFluxo :
        Negocios.IClienteFluxo, Entidades.IValidadorCliente,
        Negocios.Entidades.IDescricaoDescontoAcrescimoCliente,
        Entidades.IValidadorTipoCliente,
        Entidades.IValidadorGrupoCliente,
        Entidades.IValidadorTabelaDescontoAcrescimoCliente,
        Entidades.IProvedorDescontoAcrescimoCliente
    {
        #region Cliente

        /// <summary>
        /// Define os filtros para uma consulta.
        /// </summary>
        private void DefineFiltrosConsulta(ref Colosoft.Query.Queryable consulta, int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, DateTime? dataNascimentoIni,
            DateTime? dataNascimentoFim, int? idTabelaDescontoAcrescimo, bool apenasSemRota, int limite, string uf, string tipoPessoa, bool comCompra)
        {
            if (idCliente.HasValue && idCliente.Value > 0)
                consulta.WhereClause
                    .And("c.IdCli=?id")
                    .Add("?id", idCliente.Value);

            if (!String.IsNullOrEmpty(nomeOuApelido))
                consulta.WhereClause
                    .And("(c.Nome LIKE ?nome OR c.NomeFantasia LIKE ?nome)")
                    .Add("?nome", String.Format("%{0}%", nomeOuApelido))
                    .AddDescription(String.Format("Nome: {0}", nomeOuApelido));

            if (!String.IsNullOrEmpty(cpfCnpj))
                consulta.WhereClause
                    .And("REPLACE(REPLACE(REPLACE(c.CpfCnpj, '.', ''), '-', ''), '/', '') LIKE ?cpfCnpj")
                    .Add("?cpfCnpj", Glass.Formatacoes.LimpaCpfCnpj(cpfCnpj))
                    .AddDescription(String.Format("CPF/CNPJ: {0}", cpfCnpj));

            if (idLoja.HasValue && idLoja.Value > 0)
                consulta.WhereClause
                    .And("c.IdLoja=?loja")
                    .Add("?loja", idLoja.Value)
                    .AddDescription(() => String.Format("Loja: {0}",
                        SourceContext.Instance.GetDescriptor<Entidades.Loja>(idLoja.Value).Name));

            if (!String.IsNullOrEmpty(telefone))
                consulta.WhereClause
                    .And("(c.TelRes LIKE ?tel OR c.TelCont LIKE ?tel OR c.TelCel LIKE ?tel)")
                    .Add("?tel", string.Format("%{0}%", telefone))
                    .AddDescription(String.Format("Telefone: {0}", telefone));

            if (!String.IsNullOrEmpty(logradouro))
                consulta.WhereClause
                    .And("c.Endereco LIKE ?endereco")
                    .Add("?endereco", String.Format("%{0}%", logradouro))
                    .AddDescription(String.Format("Endereço: {0}", logradouro));

            if (!String.IsNullOrEmpty(bairro))
                consulta.WhereClause
                    .And("c.Bairro LIKE ?bairro")
                    .Add("?bairro", String.Format("%{0}%", bairro))
                    .AddDescription(String.Format("Bairro: {0}", bairro));

            if (idCidade.HasValue && idCidade.Value > 0)
                consulta.WhereClause
                    .And("c.IdCidade=?cidade")
                    .Add("?cidade", idCidade.Value)
                    .AddDescription(() => String.Format("Cidade: {0}",
                        SourceContext.Instance.GetDescriptor<Entidades.Cidade>(idCidade.Value).Name));

            if(idsTipoCliente != null && idsTipoCliente.Count() > 0 && idsTipoCliente[0] > 0)
            {
                consulta.WhereClause
                    .And(string.Format("c.IdTipoCliente IN ({0})", string.Join(",", idsTipoCliente)))
                    .AddDescription("Tipos de Cliente: {0}", string.Join(", ",
                        idsTipoCliente.Select(f => SourceContext.Instance.GetDescriptor<Entidades.TipoCliente>(f).Name )));
            }

            if (!tipoPessoa.IsNullOrEmpty() && tipoPessoa != "0")
                consulta.WhereClause
                    .And("c.TipoPessoa=?tipoPessoa")
                    .Add("?tipoPessoa", tipoPessoa == "1" ? "F" : "J")
                    .AddDescription(() => String.Format("Tipo pessoa: {0}",
                        tipoPessoa == "1" ? "Física" : "Jurídica"));

            if (situacao != null && situacao.Count() > 0 && (situacao[0] > 0 || situacao.Count() > 1))
            {
                var sit = situacao.Select(f => ((Data.Model.SituacaoCliente)f).Translate().Format()).ToArray();

                consulta.WhereClause
                    .And(string.Format("c.Situacao IN ({0})", string.Join(",", situacao)))
                    .AddDescription(string.Format("Situação: {0}", string.Join(", ", sit)));
            }

            if (apenasSemRota)
                consulta.WhereClause
                    .And("c.IdCli NOT IN (?sqlRota)")
                    .Add("?sqlRota", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Rota>("r")
                        .LeftJoin<Data.Model.RotaCliente>("r.IdRota=rc.IdRota", "rc")
                        .Where("rc.IdCliente=c.IdCli")
                        .Select("rc.IdCliente"))
                    .AddDescription("Apenas clientes sem rota associada    ");
            else if (!String.IsNullOrEmpty(codigoRota))
                consulta.WhereClause
                    .And("EXISTS (?sqlRota)")
                    .Add("?sqlRota", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Rota>("r")
                        .LeftJoin<Data.Model.RotaCliente>("r.IdRota=rc.IdRota", "rc")
                        .Where("r.CodInterno=?codRota AND rc.IdCliente=c.IdCli")
                            .Add("?codRota", codigoRota)
                        .Select("rc.IdCliente"))
                    .AddDescription(String.Format("Rota: {0}", codigoRota));

            if (idVendedor.HasValue && idVendedor.Value > 0)
                consulta.WhereClause
                    .And("c.IdFunc=?vendedor")
                    .Add("?vendedor", idVendedor.Value)
                    .AddDescription(() => String.Format("Vendedor: {0}",
                        SourceContext.Instance.GetDescriptor<Entidades.Funcionario>(idVendedor.Value).Name));

            if (!string.IsNullOrEmpty(uf))
                consulta.WhereClause
                    .And("c.IdCidade IN (?sqlCidades)")
                    .Add("?sqlCidades",
                            SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.Cidade>()
                            .Where("NomeUf = ?uf")
                            .Select("IdCidade")
                            .Add("?uf", uf)
                        );

            if (tiposFiscais != null && tiposFiscais.Length > 0)
            {
                var parametros = new Dictionary<string, Data.Model.TipoFiscalCliente>();

                for (int i = 0; i < tiposFiscais.Length; i++)
                {
                    var chave = "?tipoFiscal_" + i;
                    parametros.Add(chave, tiposFiscais[i]);
                    consulta.WhereClause.Add(chave, parametros[chave]);
                }

                consulta.WhereClause
                    .And(String.Format("c.TipoFiscal IN ({0})",
                        String.Join(", ", parametros.Select(x => x.Key).ToArray())));

                consulta.WhereClause
                    .AddDescription(String.Format("Tipo fiscal: {0}", String.Join(", ",
                        parametros.Select(x => x.Value.Translate().Format()).ToArray())));
            }

            if (formasPagto != null && formasPagto.Length > 0)
            {
                var consultaClienteFormaPagto =
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Cliente>("cfp")
                        .Select("cfp.IdCli");

                for (var i = 0; i < formasPagto.Length; i++)
                {
                    consultaClienteFormaPagto
                        .WhereClause
                        .Or(string.Format("cfp.IdCli NOT IN (?formaPagto{0})", i))
                        .Add(string.Format("?formaPagto{0}", i), SourceContext.Instance.CreateQuery()
                            .From<Data.Model.FormaPagtoCliente>("fpc1")
                            .Select("fpc1.IdCliente")
                            .Where(string.Format("fpc1.IdFormaPagto IN (?formaPagto{0})", i))
                            .Add(string.Format("?formaPagto{0}", i), formasPagto[i]));

                }

                var clienteFormaPagto = string.Join(",", consultaClienteFormaPagto.Execute().Select(f => f.GetString(0)).ToList());

                if (!string.IsNullOrEmpty(clienteFormaPagto))
                    consulta.WhereClause.And(string.Format("IdCli IN ({0})", clienteFormaPagto));
                /* Chamado 44721.
                    * O problema ocorreu porque nenhum cliente possui a forma de pagamento filtrada. */
                else
                    consulta.WhereClause.And("1=0");

                consulta.WhereClause
                    .AddDescription(string.Format("Forma(s) Pagto.: {0}",
                        string.Join(", ", SourceContext.Instance.CreateQuery()
                            .From<Data.Model.FormaPagto>()
                            .Select("Descricao")
                            .Where(string.Format("IdFormaPagto IN ({0})", string.Join(", ", formasPagto)))
                            .Execute().Select(f => f.GetString(0)).ToList())));
            }

            if (dataCadastroIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("c.DataCad>=?dataCadIni")
                    .Add("?dataCadIni", dataCadastroIni.Value)
                    .AddDescription(String.Format("Data início cad.: {0:d}", dataCadastroIni.Value));

            if (dataCadastroFim > DateTime.MinValue)
                consulta.WhereClause
                    .And("c.DataCad<=?dataCadFim")
                    .Add("?dataCadFim", dataCadastroFim.Value.AddDays(1))
                    .AddDescription(String.Format("Data fim cad.: {0:d}", dataCadastroFim.Value));

            // Período sem compra
            if (dataSemCompraIni > DateTime.MinValue || dataSemCompraFim > DateTime.MinValue)
            {
                // Empresas que trabalham com liberação de pedidos.
                if (PedidoConfig.LiberarPedido)
                {
                    // Busca os ID's dos clientes que possuem liberação dentro do período filtrado.
                    var consultaPedidoLiberado =
                        SourceContext.Instance.CreateQuery()
                        .From<Data.Model.LiberarPedido>("lp")
                        .SelectDistinct("lp.IdCliente");

                    if (dataSemCompraIni > DateTime.MinValue)
                        consultaPedidoLiberado
                            .WhereClause.And("lp.DataLiberacao>=?dataSemCompraIni")
                            .Add("?dataSemCompraIni", dataSemCompraIni);

                    if (dataSemCompraFim > DateTime.MinValue)
                        consultaPedidoLiberado
                            .WhereClause.And("lp.DataLiberacao<=?dataSemCompraFim")
                            .Add("?dataSemCompraFim", dataSemCompraFim.Value.AddHours(23).AddMinutes(59).AddSeconds(59));

                    var retornoClientePedidoLiberado = consultaPedidoLiberado.Execute().Select(f => f.GetInt32(0)).ToList();

                    // Na consulta principal, desconsidera os clientes que possuem liberação no período filtrado.
                    consulta.WhereClause
                        .And(string.Format("IdCli NOT IN ({0})", retornoClientePedidoLiberado.Count() > 0 ? string.Join(",", retornoClientePedidoLiberado) : "0"));
                }
                // Empresas que não trabalham com liberação de pedidos.
                else
                {
                    // Busca os ID's dos clientes que possuem pedidos confirmados dentro do período filtrado.
                    var consultaPedidoConfirmado =
                        SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Pedido>("p")
                        .SelectDistinct("p.IdCli");

                    if (dataSemCompraIni > DateTime.MinValue)
                        consultaPedidoConfirmado
                            .WhereClause.And("p.DataConf>=?dataSemCompraIni")
                            .Add("?dataSemCompraIni", dataSemCompraIni);

                    if (dataSemCompraFim > DateTime.MinValue)
                        consultaPedidoConfirmado
                            .WhereClause.And("p.DataConf<=?dataSemCompraFim")
                            .Add("?dataSemCompraFim", dataSemCompraFim.Value.AddHours(23).AddMinutes(59).AddSeconds(59));

                    var retornoClientePedidoConfirmado = consultaPedidoConfirmado.Execute().Select(f => f.GetInt32(0)).ToList();

                    // Na consulta principal, desconsidera os clientes que possuem pedidos confirmados no período filtrado.
                    consulta.WhereClause
                        .And(string.Format("IdCli NOT IN ({0})", retornoClientePedidoConfirmado.Count() > 0 ? string.Join(",", retornoClientePedidoConfirmado) : "0"));
                }

                if (dataSemCompraIni > DateTime.MinValue)
                    consulta.WhereClause
                        .AddDescription(string.Format("Data início sem compra: {0:d}", dataSemCompraIni.Value));

                if (dataSemCompraFim > DateTime.MinValue)
                    consulta.WhereClause
                        .AddDescription(string.Format("Data fim sem compra: {0:d}", dataSemCompraFim.Value.AddHours(23).AddMinutes(59).AddSeconds(59)));
            }

            // Adiciona o JOIN para buscar a data de inativação
            if (dataInativadoIni > DateTime.MinValue || dataInativadoFim > DateTime.MinValue)
            {
                consulta
                    .LeftJoin(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.LogAlteracao>()
                        .Where("Tabela=?tabela AND Campo=?campo AND ValorAtual in (?v1, ?v2, ?v3)")
                            .Add("?tabela", Data.Model.LogAlteracao.TabelaAlteracao.Cliente)
                            .Add("?campo", "Situação")
                            .Add("?v1", "Inativo")
                            .Add("?v2", "Cancelado")
                            .Add("?v3", "Bloqueado")
                        .Select("IdRegistroAlt, DataAlt"), "c.IdCli=l.IdRegistroAlt", "l");
            }

            if (dataInativadoIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("l.DataAlt>=?dataInativadoIni")
                    .Add("?dataInativadoIni", dataInativadoIni.Value)
                    .AddDescription(String.Format("Data início inativado: {0:d}", dataInativadoIni.Value));

            if (dataInativadoFim > DateTime.MinValue)
                consulta.WhereClause
                    .And("l.DataAlt<=?dataInativadoFim")
                    .Add("?dataInativadoFim", dataInativadoFim.Value)
                    .AddDescription(String.Format("Data fim inativado: {0:d}", dataInativadoFim.Value));

            if (dataNascimentoIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("c.DataNasc>=?dataNiverIni")
                    .Add("?dataNiverIni", dataNascimentoIni.Value)
                    .AddDescription(String.Format("Data início Aniversario: {0:d}", dataNascimentoIni.Value));

            if (dataNascimentoFim > DateTime.MinValue)
                consulta.WhereClause
                    .And("c.DataNasc<=?dataNiverFim")
                    .Add("?dataNiverFim", dataNascimentoFim.Value)
                    .AddDescription(String.Format("Data fim Aniversario: {0:d}", dataNascimentoFim.Value));

            if (idTabelaDescontoAcrescimo > 0)
                consulta.WhereClause
                    .And("c.IdTabelaDesconto=?tabelaDesconto")
                    .Add("?tabelaDesconto", idTabelaDescontoAcrescimo.Value)
                    .AddDescription(String.Format("Tabela Desconto/Acréscimo Cliente: {0}",
                        SourceContext.Instance.GetDescriptor<Entidades.TabelaDescontoAcrescimoCliente>(idTabelaDescontoAcrescimo.Value).Name));

            if (comCompra)
                consulta.WhereClause
                    .And("c.TotalComprado>0")
                    .AddDescription("Clientes com compras");

            if (limite > 0)
            {
                if (limite == 1)
                    consulta.WhereClause.And("c.Limite < c.UsoLimite").AddDescription("Acima do limite");
                else if (limite == 2)
                    consulta.WhereClause.And("c.Limite >= c.UsoLimite").AddDescription("Dentro do limite");
                else
                    consulta.WhereClause.And("IsNull(c.Limite, 0) = 0").AddDescription("Sem limite cadastrado");
            }
        }

        /// <summary>
        /// Cria uma nova instancia do cliente.
        /// </summary>
        /// <returns></returns>
        public Entidades.Cliente CriarCliente()
        {
            return SourceContext.Instance.Create<Entidades.Cliente>();
        }

        /// <summary>
        /// Pesquisa os clientes do sistema.
        /// </summary>
        public IList<Entidades.ClientePesquisa> PesquisarClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, DateTime? dataNascimentoIni,
            DateTime? dataNascimentoFim, int? idTabelaDescontoAcrescimo, bool apenasSemRota, int limite, string uf, string tipoPessoa, bool comCompra)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>("c")
                .LeftJoin<Data.Model.Funcionario>("c.IdFunc=fVend.IdFunc", "fVend")
                .LeftJoin<Data.Model.Funcionario>("c.Usucad=fCad.IdFunc", "fCad")
                .LeftJoin<Data.Model.Funcionario>("c.UsuAlt=fAlt.IdFunc", "fAlt")
                .LeftJoin<Data.Model.Funcionario>("c.IdFuncAtendente=fAtendente.IdFunc", "fAtendente")
                .LeftJoin<Data.Model.Cidade>("c.IdCidade=cid.IdCidade", "cid")
                .LeftJoin<Data.Model.FormaPagto>("c.IdFormaPagto=fp.IdFormaPagto", "fp")
                .LeftJoin<Data.Model.Parcelas>("c.TipoPagto=p.IdParcela", "p")
                .OrderBy("c.IdCli")
                .Select(
                    @"c.IdCli, c.Nome, c.NomeFantasia, c.CpfCnpj, c.Endereco, c.Numero, c.Compl, c.Bairro, cid.NomeCidade as Cidade,
                    cid.NomeUf as Uf, c.TelCont, c.TelRes, c.TelCel, c.Situacao, c.Email, c.DtUltCompra, c.Historico,
                    c.TotalComprado, c.Revenda, fp.Descricao As FormaPagamento, p.Descricao As Parcela,
                    c.DataCad, c.DataAlt, c.IdTabelaDesconto, fCad.Nome as DescrUsuCad, fAlt.Nome as DescrUsuAlt, c.IdFunc, fVend.Nome as NomeFunc,
                    fAtendente.IdFunc AS IdFuncAtendente, fAtendente.Nome AS NomeAtendente, c.Limite, c.UsoLimite, c.BairroEntrega, c.NumeroEntrega, c.ComplEntrega, c.EnderecoEntrega");

            DefineFiltrosConsulta(ref consulta, idCliente, nomeOuApelido, cpfCnpj, idLoja, telefone, logradouro, bairro, idCidade, idsTipoCliente,
                situacao, codigoRota, idVendedor, tiposFiscais, formasPagto, dataCadastroIni, dataCadastroFim, dataSemCompraIni, dataSemCompraFim,
                dataInativadoIni, dataInativadoFim, dataNascimentoIni, dataNascimentoFim, idTabelaDescontoAcrescimo, apenasSemRota, limite, uf, tipoPessoa, comCompra);

            return consulta.ToVirtualResultLazy<Entidades.ClientePesquisa>();
        }

        /// <summary>
        /// Completa os dados das fichas dos clientes.
        /// </summary>
        /// <param name="produtos"></param>
        private void CompletaDadosFichaClientes(IEnumerable<Entidades.FichaCliente> clientes)
        {
            var count = clientes.Count();
            var index = 0;

            var formasPagto = new List<Data.Model.FormaPagto>();
            var formasPagtoCliente = new List<Data.Model.FormaPagtoCliente>();
            var parcelas = new List<Data.Model.Parcelas>();
            var parcelasCliente = new List<Data.Model.ParcelasNaoUsar>();

            var consultas = SourceContext.Instance.CreateMultiQuery();

            consultas.Add<Data.Model.FormaPagto>(
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FormaPagto>()
                    .Where("isnull(ApenasSistema, 0)=0"),
                        (sender, q, result) =>
                        {
                            formasPagto.AddRange(result);
                        });

            consultas.Add<Data.Model.Parcelas>(
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Parcelas>(),
                        (sender, q, result) =>
                        {
                            parcelas.AddRange(result);
                        });

            while (count > 0)
            {
                var take = count < 100 ? count : 100;

                // Recupera o filtro dos identificadores do cliente
                var identificadores = string.Format("IdCliente IN ({0})",
                    string.Join(",", clientes
                        .Skip(index).Take(take)
                        .Select(f => f.IdCli.ToString())
                        .ToArray()));

                // Adiciona a consulta das formas de pagto do cliente
                consultas.Add<Data.Model.FormaPagtoCliente>(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.FormaPagtoCliente>()
                        .Where(identificadores),
                            (sender, q, result) =>
                            {
                                formasPagtoCliente.AddRange(result);
                            });

                // Adiciona a consulta das parcelas do cliente
                consultas.Add<Data.Model.ParcelasNaoUsar>(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ParcelasNaoUsar>()
                        .Where(identificadores),
                            (sender, q, result) =>
                            {
                                parcelasCliente.AddRange(result);
                            });

                index += take;
                count -= take;
            }

            consultas.Execute();

            // Processa os produtos
            foreach (var cli in clientes)
            {
                // Recupera a descrição das formas de pagamento do cliente
                var idsFormasPagtoCliente = formasPagtoCliente
                    .Where(x => x.IdCliente == cli.IdCli)
                    .Select(x => x.IdFormaPagto).ToList();

                cli.FormasPagamento = String.Join(", ", formasPagto
                    .Where(x => x.IdFormaPagto.HasValue && !idsFormasPagtoCliente.Contains((int)x.IdFormaPagto.Value))
                    .Select(x => x.Descricao)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray());

                // Recupera a descrição das parcelas do cliente
                var idsParcelasCliente = parcelasCliente
                    .Where(x => x.IdCliente == cli.IdCli)
                    .Select(x => x.IdParcela).ToList();

                cli.Parcelas = String.Join(", ", parcelas
                    .Where(x => !idsParcelasCliente.Contains((int)x.IdParcela))
                    .Select(x => x.Descricao)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray());
            }
        }

        /// <summary>
        /// Recupera os dados para o relatório de fichas de clientes.
        /// </summary>
        public IList<Entidades.FichaCliente> PesquisarFichasClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo, bool apenasSemRota,
            string uf)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>("c")
                .LeftJoin<Data.Model.Cidade>("c.IdCidade=cid.IdCidade", "cid")
                .LeftJoin<Data.Model.Cidade>("c.IdCidadeCobranca=cidCobr.IdCidade", "cidCobr")
                .LeftJoin<Data.Model.Parcelas>("c.TipoPagto=p.IdParcela", "p")
                .LeftJoin<Data.Model.TabelaDescontoAcrescimoCliente>("c.IdTabelaDesconto=td.IdTabelaDesconto", "td")
                .LeftJoin<Data.Model.Loja>("c.IdLoja=l.IdLoja", "l")
                .LeftJoin<Data.Model.Funcionario>("c.IdFunc=fVend.IdFunc", "fVend")
                .LeftJoin<Data.Model.Comissionado>("c.IdComissionado=com.IdComissionado", "com")
                .LeftJoin<Data.Model.RotaCliente>("c.IdCli=rc.IdCliente", "rc")
                .LeftJoin<Data.Model.Rota>("rc.IdRota=r.IdRota", "r")
                .LeftJoin<Data.Model.TipoCliente>("c.IdTipoCliente=tc.IdTipoCliente", "tc")
                .LeftJoin<Data.Model.FormaPagtoCliente>("c.IdCli=fpc.IdCliente", "fpc")
                .OrderBy("c.IdCli")
                .GroupBy("c.IdCli")
                .Select(
                    @"c.IdCli, c.Nome, c.NomeFantasia, c.Email, c.TipoPessoa, c.ProdutorRural, c.DataNasc, c.CpfCnpj, c.Suframa,
                      c.RgEscinst as RgInscEst, c.Fax, c.TelRes, c.TelCel, c.Contato, c.TelCont, c.Endereco, c.Numero, c.Compl,
                      c.Bairro, cid.NomeCidade as Cidade, cid.NomeUf as Uf, c.Cep, c.EnderecoCobranca, c.NumeroCobranca, c.ComplCobranca,
                      c.BairroCobranca, cidCobr.NomeCidade as CidadeCobranca, cidCobr.NomeUf as UfCobranca, c.CepCobranca, c.Limite,
                      c.ValorMediaIni, c.ValorMediaFim, c.Credito, c.PercSinalMinimo, c.Revenda, p.Descricao as Parcela,
                      c.PagamentoAntesProducao, td.Descricao as TabelaDescontoAcrescimo, c.CobrarIcmsSt, c.CobrarIpi,
                      ISNULL(l.NomeFantasia, l.RazaoSocial) as Loja, fVend.Nome as NomeFunc, com.Nome as NomeComissionado, c.PercComissaoFunc,
                      r.CodInterno as Rota, c.Login, c.Situacao, tc.Descricao as TipoCliente, c.BloquearPedidoContaVencida, c.IgnorarBloqueioPedPronto,
                      c.Obs, c.Contato1, c.CelContato1, c.EmailContato1, c.RamalContato1, c.Contato2, c.CelContato2, c.EmailContato2, c.RamalContato2,
                      c.Historico, c.IdLoja, l.CalcularIcmsPedido, l.CalcularIpiPedido");

            DefineFiltrosConsulta(ref consulta, idCliente, nomeOuApelido, cpfCnpj, idLoja, telefone, logradouro, bairro, idCidade, idsTipoCliente,
                situacao, codigoRota, idVendedor, tiposFiscais, formasPagto, dataCadastroIni, dataCadastroFim, dataSemCompraIni, dataSemCompraFim,
                dataInativadoIni, dataInativadoFim, null, null, idTabelaDescontoAcrescimo, apenasSemRota, 0, uf, null, false);

            var resultado = consulta.Execute<Entidades.FichaCliente>().ToList();

            CompletaDadosFichaClientes(resultado);
            return resultado;
        }

        /// <summary>
        /// Altera o vendedor dos clientes que preenchem aos requisitos.
        /// </summary>
        public Colosoft.Business.SaveResult AlterarVendedorClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, int idVendedorNovo, string uf)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>("c");

            DefineFiltrosConsulta(ref consulta, idCliente, nomeOuApelido, cpfCnpj, idLoja, telefone, logradouro, bairro, idCidade, idsTipoCliente,
                situacao, codigoRota, idVendedor, tiposFiscais, formasPagto, dataCadastroIni, dataCadastroFim, dataSemCompraIni, dataSemCompraFim,
                dataInativadoIni, dataInativadoFim, null, null, idTabelaDescontoAcrescimo, apenasSemRota, 0, uf, null, false);

            var itens = consulta.ProcessLazyResult<Entidades.Cliente>();

            using (var session = SourceContext.Instance.CreateSession())
            {
                foreach (var item in itens)
                {
                    item.IdFunc = idVendedorNovo > 0 ? (int?)idVendedorNovo : null;
                    var retorno = item.Save(session);

                    if (!retorno)
                        return retorno;
                }

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Altera a Rota dos clientes que preenchem aos requisitos.
        /// </summary>
        public Colosoft.Business.SaveResult AlterarRotaClientes(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, int[] situacao, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, int idRotaNova, string uf)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>("c");

            DefineFiltrosConsulta(ref consulta, idCliente, nomeOuApelido, cpfCnpj, idLoja, telefone, logradouro, bairro, idCidade, idsTipoCliente,
                situacao, codigoRota, idVendedor, tiposFiscais, formasPagto, dataCadastroIni, dataCadastroFim, dataSemCompraIni, dataSemCompraFim,
                dataInativadoIni, dataInativadoFim, null, null, idTabelaDescontoAcrescimo, apenasSemRota, 0, uf, null, false);

            var itens = consulta.ProcessLazyResult<Entidades.Cliente>();

            using (var session = SourceContext.Instance.CreateSession())
            {
                foreach (var item in itens)
                {
                    item.IdRota = idRotaNova > 0 ? (int?)idRotaNova : null;
                    var resultado = item.Save(session);

                    if (!resultado)
                        return resultado;
                }

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Ativa todos os clientes que preenchem aos requisitos e que estejam inativos.
        /// </summary>
        public Colosoft.Business.SaveResult AtivarClientesInativos(int? idCliente, string nomeOuApelido, string cpfCnpj, int? idLoja, string telefone,
            string logradouro, string bairro, int? idCidade, int[] idsTipoCliente, string codigoRota, int? idVendedor,
            Data.Model.TipoFiscalCliente[] tiposFiscais, int[] formasPagto, DateTime? dataCadastroIni, DateTime? dataCadastroFim,
            DateTime? dataSemCompraIni, DateTime? dataSemCompraFim, DateTime? dataInativadoIni, DateTime? dataInativadoFim, int? idTabelaDescontoAcrescimo,
            bool apenasSemRota, string uf)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>("c");

            DefineFiltrosConsulta(ref consulta, idCliente, nomeOuApelido, cpfCnpj, idLoja, telefone, logradouro, bairro, idCidade, idsTipoCliente,
                new int[] { (int)Data.Model.SituacaoCliente.Inativo }, codigoRota, idVendedor, tiposFiscais, formasPagto, dataCadastroIni, dataCadastroFim, dataSemCompraIni,
                dataSemCompraFim, dataInativadoIni, dataInativadoFim, null, null, idTabelaDescontoAcrescimo, apenasSemRota, 0, uf, null, false);

            var itens = consulta.ProcessResult<Entidades.Cliente>();

            using (var sessao = SourceContext.Instance.CreateSession())
            {
                foreach (var item in itens)
                {
                    item.Situacao = Data.Model.SituacaoCliente.Ativo;
                    var resultado = item.Save(sessao);

                    if (!resultado)
                        return resultado;
                }

                return sessao.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Recupera os descritores dos clientes.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObtemClientes()
        {
            return SourceContext.Instance.CreateQuery()
               .From<Data.Model.Cliente>()
               .OrderBy("IdCli")
               .ProcessResultDescriptor<Entidades.Cliente>()
               .ToList();
        }

        /// <summary>
        /// Recupera o cliente pelo código informado.
        /// </summary>
        public Entidades.Cliente ObtemCliente(int idCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>()
                .Where("IdCli=?id").Add("?id", idCliente)
                .ProcessLazyResult<Entidades.Cliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Verifica existência do cliente pelo nome
        /// </summary>
        public bool VerificarClientePeloNome(string nome)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cliente>()
                .Where("Nome = ?nome OR NomeFantasia = ?nome")
                .Add("?nome", nome)
                .ExistsResult();
        }

        /// <summary>
        /// Recupera o descritor de um cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public Colosoft.IEntityDescriptor ObtemDescritorCliente(int idCliente)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Data.Model.Cliente>()
               .Where("IdCli=?id").Add("?id", idCliente)
               .ProcessResultDescriptor<Entidades.Cliente>()
               .FirstOrDefault();
        }

        public Glass.Negocios.Global.SalvarClienteResultado SalvarClienteRetornando(Entidades.Cliente cliente)
        {
            var resultado = SalvarCliente(cliente);

            if (resultado)
                return new Glass.Negocios.Global.SalvarClienteResultado(cliente.IdCli);

            return new Glass.Negocios.Global.SalvarClienteResultado(false, resultado.Message);
        }

        /// <summary>
        /// Salva os dados do cliente.
        /// </summary>
        public Colosoft.Business.SaveResult SalvarCliente(Entidades.Cliente cliente)
        {
            cliente.Require("cliente").NotNull();

            // Não permite que a palavra ISENTO seja salva incorretamente
            if (!string.IsNullOrEmpty(cliente.RgEscinst) &&
                (cliente.RgEscinst.ToUpper() == "ISENTA" || cliente.RgEscinst.ToUpper() == "INSENTA" || cliente.RgEscinst.ToUpper() == "INSENTO"))
                cliente.RgEscinst = "ISENTO";

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = cliente.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do cliente.
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarCliente(Entidades.Cliente cliente)
        {
            cliente.Require("cliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = cliente.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Tipo de Cliente

        /// <summary>
        /// Recupera os tipos de clientes do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.TipoCliente> PesquisaTiposCliente()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoCliente>()
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.TipoCliente>();
        }

        /// <summary>
        /// Recupera os tipos de clientes cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<IEntityDescriptor> ObtemDescritoresTipoCliente()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoCliente>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.TipoCliente>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos tipos de cliente pelos identificadores informados.
        /// </summary>
        /// <param name="idsTipoCliente"></param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemTiposCliente(IEnumerable<int> idsTipoCliente)
        {
            idsTipoCliente.Require("idsTipoCliente").NotNull();

            var ids = string.Join(",", idsTipoCliente.Distinct().Select(f => f.ToString()).ToArray());

            if (string.IsNullOrEmpty(ids))
                return new List<Colosoft.IEntityDescriptor>();

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoCliente>()
                .Where(string.Format("IdTipoCliente IN ({0})", ids))
                .ProcessResultDescriptor<Entidades.TipoCliente>()
                .ToList();
        }

        /// <summary>
        /// Recupera o tipo de cliente pelo identificador informado.
        /// </summary>
        /// <param name="idTipoCliente"></param>
        /// <returns></returns>
        public Entidades.TipoCliente ObtemTipoCliente(int idTipoCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoCliente>()
                .Where("IdTipoCliente=?id")
                .Add("?id", idTipoCliente)
                .ProcessLazyResult<Entidades.TipoCliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarTipoCliente(Entidades.TipoCliente tipoCliente)
        {
            tipoCliente.Require("tipoCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoCliente.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga o tipo de cliente.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarTipoCliente(Entidades.TipoCliente tipoCliente)
        {
            tipoCliente.Require("tipoCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoCliente.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Grupo Cliente

        /// <summary>
        /// Recupera os tipos de clientes do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.GrupoCliente> PesquisarGruposCliente()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoCliente>()
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.GrupoCliente>();
        }

        /// <summary>
        /// Recupera os tipos de clientes cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<IEntityDescriptor> ObterDescritoresGrupoCliente()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoCliente>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.GrupoCliente>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos tipos de cliente pelos identificadores informados.
        /// </summary>
        /// <param name="idsTipoCliente"></param>
        /// <returns></returns>
        public IList<IEntityDescriptor> ObterGruposCliente(IEnumerable<int> idsGrupoCliente)
        {
            idsGrupoCliente.Require("idGrupoCliente").NotNull();

            var ids = string.Join(",", idsGrupoCliente.Distinct().Select(f => f.ToString()).ToArray());

            if (string.IsNullOrEmpty(ids))
                return new List<Colosoft.IEntityDescriptor>();

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoCliente>()
                .Where(string.Format("IdGrupoCliente IN ({0})", ids))
                .ProcessResultDescriptor<Entidades.TipoCliente>()
                .ToList();
        }

        /// <summary>
        /// Recupera o tipo de cliente pelo identificador informado.
        /// </summary>
        /// <param name="idTipoCliente"></param>
        /// <returns></returns>
        public Entidades.GrupoCliente ObterGrupoCliente(int idGrupoCliente)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoCliente>()
                .Where("IdGrupoCliente=?id")
                .Add("?id", idGrupoCliente)
                .ProcessLazyResult<Entidades.GrupoCliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarGrupoCliente(Entidades.GrupoCliente grupoCliente)
        {
            grupoCliente.Require("grupoCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = grupoCliente.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga o tipo de cliente.
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarGrupoCliente(Entidades.GrupoCliente grupoCliente)
        {
            grupoCliente.Require("grupoCliente").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = grupoCliente.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Tabela de desconto/acréscimo

        /// <summary>
        /// Cria uma nova instancia da tabela de desconto/acréscimo.
        /// </summary>
        /// <returns></returns>
        public Entidades.TabelaDescontoAcrescimoCliente CriarTabelaDescontoAcrescimoCliente()
        {
            return SourceContext.Instance.Create<Entidades.TabelaDescontoAcrescimoCliente>();
        }

        /// <summary>
        /// Recupera a tabela de desconto/acréscimo pelo código informado.
        /// </summary>
        /// <returns></returns>
        public Entidades.TabelaDescontoAcrescimoCliente ObtemTabelaDescontoAcrescimoCliente(int IdTabelaDesconto)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TabelaDescontoAcrescimoCliente>()
                .Where("IdTabelaDesconto=?id")
                    .Add("?id", IdTabelaDesconto)
                .ProcessLazyResult<Entidades.TabelaDescontoAcrescimoCliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera as tabelas de desconto/acréscimo cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemDescritoresTabelaDescontoAcrescimo()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TabelaDescontoAcrescimoCliente>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.TabelaDescontoAcrescimoCliente>()
                .ToList();
        }

        /// <summary>
        /// Pesquisa as tabelas os descontos/acréscimos do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.TabelaDescontoAcrescimoCliente> PesquisarTabelasDescontosAcrescimos()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TabelaDescontoAcrescimoCliente>()
                .OrderBy("IdTabelaDesconto")
                .ToVirtualResultLazy<Entidades.TabelaDescontoAcrescimoCliente>();
        }

        /// <summary>
        /// Salva os dados da tabela de desconto/acréscimo.
        /// </summary>
        public Colosoft.Business.SaveResult SalvarTabelaDescontoAcrescimo(Entidades.TabelaDescontoAcrescimoCliente tabelaDesconto)
        {
            tabelaDesconto.Require("tabelaDesconto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tabelaDesconto.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da tabela de desconto/acréscimo.
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarTabelaDescontoAcrescimo(Entidades.TabelaDescontoAcrescimoCliente tabelaDesconto)
        {
            tabelaDesconto.Require("tabelaDesconto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tabelaDesconto.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Desconto/acréscimo

        /// <summary>
        /// Pesquisa os descontos/acréscimos do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.DescontoAcrescimoClientePesquisa> PesquisarDescontosAcrescimos(int? idCliente,
            int? idTabelaDesconto, int? idGrupoProd, int? idSubgrupoProd, string codProduto, string produto, Situacao? situacao)
        {
            // Exige a tabela de desconto caso o cliente não seja informado
            if (!idCliente.HasValue || idCliente.Value <= 0)
                idTabelaDesconto.Require("idTabelaDesconto").NotNull();

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.DescontoAcrescimoCliente>("dc");

            var idDados = idCliente.HasValue && idCliente.Value > 0 ?
                "?idCliente as IdCliente" :
                "?idTabelaDesconto as IdTabelaDesconto";

            var joinDados = idCliente.HasValue && idCliente.Value > 0 ?
                "dados.IdCliente=dc.IdCliente" :
                "dados.IdTabelaDesconto=dc.IdTabelaDesconto";

            var idProd = "dc.IdProduto";
            var descrProd = String.Empty;

            consulta.OrderBy("dados.Grupo, dados.Subgrupo");

            if ((idGrupoProd ?? 0) == 0 && (idSubgrupoProd ?? 0) == 0)
            {
                var prod = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.GrupoProd>("g")
                    .InnerJoin<Data.Model.SubgrupoProd>("g.IdGrupoProd=s.IdGrupoProd", "s")
                    .Select(String.Format(
                        @"{0}, g.IdGrupoProd, s.IdSubgrupoProd, g.Descricao as Grupo,
                          s.Descricao as Subgrupo", idDados))
                    .OrderBy("g.Descricao, s.Descricao");

                consulta.RightJoin(prod, String.Format(@"{0}
		            AND dados.IdGrupoProd=ISNULL(dc.IdGrupoProd, dados.IdGrupoProd)
		            AND dados.IdSubgrupoProd=ISNULL(dc.IdSubgrupoProd, dados.IdSubgrupoProd)
		            AND dc.IdProduto IS NULL", joinDados), "dados");
            }
            else
            {
                idProd = "dados.IdProd";
                descrProd = ", dados.Produto";

                consulta.OrderBy("dados.Produto");

                var prod = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>("p")
                    .InnerJoin<Data.Model.GrupoProd>("p.IdGrupoProd=g.IdGrupoProd", "g")
                    .LeftJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd=s.IdSubgrupoProd", "s")
                    .Select(String.Format(
                        @"{0}, p.IdProd, p.IdGrupoProd, p.IdSubgrupoProd, g.Descricao as Grupo,
                          s.Descricao as Subgrupo, p.Descricao as Produto", idDados))
                    .OrderBy("Descricao");

                if (!String.IsNullOrEmpty(produto) || (situacao.HasValue && (int)situacao.Value > 0))
                {
                    if (!String.IsNullOrEmpty(codProduto))
                        prod.WhereClause
                            .And("CodInterno like ?codProd")
                            .Add("?codProd", String.Format("%{0}%", codProduto));
                    else if (!String.IsNullOrEmpty(produto))
                        prod.WhereClause
                            .And("Descricao like ?prod")
                            .Add("?prod", String.Format("%{0}%", produto));

                    if (situacao.HasValue && (int)situacao.Value > 0)
                        prod.WhereClause
                            .And("Situacao=?situacao")
                            .Add("?situacao", situacao.Value);
                }

                consulta
                    .RightJoin(prod, String.Format(@"{0}
		                AND dados.IdGrupoProd=ISNULL(dc.IdGrupoProd, dados.IdGrupoProd)
		                AND dados.IdSubgrupoProd=ISNULL(dc.IdSubgrupoProd, dados.IdSubgrupoProd)
		                AND dados.IdProd=dc.IdProduto", joinDados), "dados");
            }

            consulta.Select(String.Format(
                @"dc.IdDesconto, {2}.IdCliente, {3}.IdTabelaDesconto, dados.IdGrupoProd,
                  dados.IdSubgrupoProd, {0} as IdProduto, dc.Desconto, dc.Acrescimo, dc.DescontoAVista,
                  dc.AplicarBeneficiamentos, dados.Grupo, dados.Subgrupo {1}",

                idProd,
                descrProd,
                idCliente.HasValue && idCliente.Value > 0 ? "dados" : "dc",
                idTabelaDesconto.HasValue && idTabelaDesconto.Value > 0 ? "dados" : "dc"));

            if (idCliente.HasValue && idCliente.Value > 0)
            {
                var idsSubgrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdCli = ?id").Add("?id", idCliente.Value)
                    .Select("IdsSubgrupoProd")
                    .Execute()
                    .Select(f => f.GetString(0))
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(idsSubgrupo))
                    consulta.WhereClause
                        .And(string.Format("dados.IdSubgrupoProd IN ({0})", idsSubgrupo));

                consulta.Add("?idCliente", idCliente.Value);

            }

            if (idTabelaDesconto.HasValue && idTabelaDesconto.Value > 0)
                consulta.Add("?idTabelaDesconto", idTabelaDesconto.Value);

            if (idGrupoProd.HasValue && idGrupoProd.Value > 0)
                consulta.WhereClause
                    .And("dados.IdGrupoProd=?idGrupoProd")
                    .Add("?idGrupoProd", idGrupoProd.Value);

            if (idSubgrupoProd.HasValue && idSubgrupoProd.Value > 0)
                consulta.WhereClause
                    .And("dados.IdSubgrupoProd=?idSubgrupoProd")
                    .Add("?idSubgrupoProd", idSubgrupoProd.Value);

            return consulta.ToVirtualResultLazy<Entidades.DescontoAcrescimoClientePesquisa>();
        }

        /// <summary>
        /// Recupera os dados do desconto/acréscimo.
        /// </summary>
        /// <param name="IdDesconto"></param>
        /// <returns></returns>
        public Entidades.DescontoAcrescimoCliente ObtemDescontoAcrescimo(int idDesconto)
        {
            if (idDesconto == 0)
                return new Entidades.DescontoAcrescimoCliente();

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.DescontoAcrescimoCliente>()
                .Where("IdDesconto=?id")
                .Add("?id", idDesconto)
                .ProcessLazyResult<Entidades.DescontoAcrescimoCliente>()
                .FirstOrDefault();
        }

        private static readonly object _salvarDescontoAcrescimoLock = new object();

        /// <summary>
        /// Salva os dados do desconto/acréscimo.
        /// </summary>
        /// <param name="descontoAcrescimo"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarDescontoAcrescimo(Entidades.DescontoAcrescimoCliente descontoAcrescimo)
        {
            lock(_salvarDescontoAcrescimoLock)
            {
                descontoAcrescimo.Require("descontoAcrescimo").NotNull();

                // Insere no banco, se a entidade não tiver ID
                // (indica pra entidade que ela não está no banco de dados)
                if (descontoAcrescimo.IdDesconto == 0)
                {
                    descontoAcrescimo.IdDesconto = Colosoft.Business.EntityTypeManager.Instance.GenerateInstanceUid(typeof(Entidades.DescontoAcrescimoCliente));
                    descontoAcrescimo.DataModel.ExistsInStorage = false;
                }

                if (descontoAcrescimo.Desconto < 0 || descontoAcrescimo.Desconto > 100 || descontoAcrescimo.Acrescimo < 0 || descontoAcrescimo.Acrescimo > 100)
                    return new SaveResult(false, "São permitidos valores apenas entre 0 e 100".GetFormatter());

                // Não faz nada se não estiver no banco e os valores estiverem zerados
                if (!descontoAcrescimo.DataModel.ExistsInStorage && descontoAcrescimo.Desconto == 0 && descontoAcrescimo.Acrescimo == 0 && descontoAcrescimo.DescontoAVista == 0)
                    return new SaveResult(true, null);

                using (var session = SourceContext.Instance.CreateSession())
                {
                    var resultado = descontoAcrescimo.Save(session);

                    if (!resultado)
                        return resultado;

                    return session.Execute(false).ToSaveResult();
                }
            }
        }

        #endregion

        #region IDescricaoDescontoAcrescimoCliente Members

        /**
         * Implementação para manter os logs funcionando enquanto não é criado o log em banco.
         */

        string Entidades.IDescricaoDescontoAcrescimoCliente.ObtemDescricao(int idDescontoAcrescimoCliente)
        {
            return Data.DAL.DescontoAcrescimoClienteDAO.Instance.GetElement((uint)idDescontoAcrescimoCliente).DescricaoCompleta;
        }

        string Entidades.IDescricaoDescontoAcrescimoCliente.ObtemDescricao(int? idCliente, int? idTabelaDescontoAcrescimo, int idGrupoProd, int? idSubgrupoProd, int? idProduto)
        {
            return String.Format("{0}: {1}  Grupo: {2}" + (idProduto > 0 ? "  Produto: {3}" : String.Empty),
                idCliente > 0 ? "Cliente" : "Tabela",
                idCliente > 0 ? Data.DAL.ClienteDAO.Instance.GetNome((uint)idCliente.Value) :
                    Data.DAL.TabelaDescontoAcrescimoClienteDAO.Instance.GetDescricao((uint)idTabelaDescontoAcrescimo.Value),
                Data.DAL.GrupoProdDAO.Instance.GetDescricao(idGrupoProd) + (idSubgrupoProd > 0 ?
                    String.Format(" / {0}", Data.DAL.SubgrupoProdDAO.Instance.GetDescricao(idSubgrupoProd.Value)) : String.Empty),
                idProduto > 0 ? Data.DAL.ProdutoDAO.Instance.GetDescrProduto(idProduto.Value) : String.Empty);
        }

        #endregion

        #region IValidadorCliente Members

        /// <summary>
        /// Valida a atualização do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorCliente.ValidaAtualizacao(Entidades.Cliente cliente)
        {
            if (!cliente.ExistsInStorage)
            {
                // Verifica se foi cadastrado um cliente com o mesmo cpf/cnpj, para evitar duplicidade
                if (SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("CpfCnpj=?cpfCnpj")
                    .Add("?cpfCnpj", cliente.CpfCnpj)
                    .ExistsResult())

                    if ((cliente.CpfCnpj == "999.999.999-99" || cliente.CpfCnpj == "99.999.999/9999-99") && ClienteConfig.TelaCadastro.PermitirCpfCnpjTudo9AoInserir)
                    {
                        return new IMessageFormattable[0];
                    }
                    else
                    {
                        return new[] { "Já existe um cliente cadastrado com o CPF/CNPJ informado.".GetFormatter() };
                    }
            }

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a existencia do cliente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorCliente.ValidaExistencia(Entidades.Cliente cliente)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Pedido>()
                    .Where("IdCli=?id")
                    .Add("?id", cliente.IdCli)
                    .Count(),
                    tratarResultado("Há pedidos associados ao mesmo."))

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Orcamento>()
                    .Where("IdCliente=?id")
                    .Add("?id", cliente.IdCli)
                    .Count(),
                    tratarResultado("Há orçamentos associados ao mesmo."))

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Projeto>()
                    .Where("IdCliente=?id")
                    .Add("?id", cliente.IdCli)
                    .Count(),
                    tratarResultado("Há projetos associados ao mesmo."))

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>()
                    .Where("IdCliente=?id")
                    .Add("?id", cliente.IdCli)
                    .Count(),
                    tratarResultado("Há notas fiscais associados ao mesmo."))

                .Execute();

            if (cliente.Credito > 0)
            {
                mensagens.Add($"O cliente possui {cliente.Credito.ToString("C")} de crédito ");
            }

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorTipoCliente Members

        /// <summary>
        /// Valida a existencia do dados do tipo de cliente.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.TipoCliente tipoCliente)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se o tipo de cliente possui clientes relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdTipoCliente=?id")
                    .Add("?id", tipoCliente.IdTipoCliente)
                    .Count(),
                    tratarResultado("Este tipo de cliente não pode ser excluído por possuir clientes relacionados ao mesmo."))

                // Verifica se o tipo de cliente possui dados de ICMS relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.IcmsProdutoUf>()
                    .Where("IdTipoCliente=?id")
                    .Add("?id", tipoCliente.IdTipoCliente)
                    .Count(),
                    tratarResultado("Este tipo de cliente não pode ser excluído por possuir dados de ICMS de produto relacionados ao mesmo."))

                // Verifica se o tipo de cliente possui regras de natureza de operação relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.RegraNaturezaOperacao>()
                    .Where("IdTipoCliente=?id")
                    .Add("?id", tipoCliente.IdTipoCliente)
                    .Count(),
                    tratarResultado("Este tipo de cliente não pode ser excluído por possuir regras de natureza de operação relacionadas ao mesmo."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorGrupoCliente Members

        /// <summary>
        /// Valida a existencia do dados do tipo de cliente.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.GrupoCliente grupoCliente)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se o tipo de cliente possui clientes relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdGrupoCliente=?id")
                    .Add("?id", grupoCliente.IdGrupoCliente)
                    .Count(),
                    tratarResultado("Este grupo de cliente não pode ser excluído por possuir clientes relacionados ao mesmo."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        /// <summary>
        /// Valida a existencia do dados do tipo de cliente.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaInsercao(Entidades.GrupoCliente grupoCliente)
        {
            var mensagens = new List<string>();
            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });
            if (!grupoCliente.Descricao.IsNullOrEmpty())
                SourceContext.Instance.CreateMultiQuery()
                    // Verifica se o tipo de cliente possui clientes relacionados à seu id
                    .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.GrupoCliente>()
                        .Where("Descricao Like?descricao")
                        .Add("?descricao", $"%{grupoCliente.Descricao}%")
                        .Count(),
                        tratarResultado("Já existe um grupo de cliente cadastrado com essa descrição"))
                    .Execute();
            if (grupoCliente.Descricao.IsNullOrEmpty())
                mensagens.Add("A descrição não pode ser vazia !");
            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorTabelaDescontoAcrescimoCliente Members

        /// <summary>
        /// Valida a existencia do dados do desconto/acréscimo.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.TabelaDescontoAcrescimoCliente tabelaDesconto)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se o desconto/acréscimo possui clientes relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdTabelaDesconto=?id")
                    .Add("?id", tabelaDesconto.IdTabelaDesconto)
                    .Count(),
                    tratarResultado("Esta tabela de desconto/acréscimo não pode ser excluída por possuir clientes relacionados à mesma."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }
        #endregion

        #region Membros de IProvedorDescontoAcrescimoCliente

        /// <summary>
        /// Verifica se Existe Desconto na tabela de desconto acrescimo cliente.
        /// </summary>
        bool Entidades.IProvedorDescontoAcrescimoCliente.ExisteDescontoGrupoSubgrupoProdutoPorClienteOuTabela(int? idCliente, int? idTabelaDesconto, int? idGrupoProd, int? idSubgrupoProd, int? idProduto)
        {
            var filtroClienteTabela = idCliente > 0 ? string.Format("IdCliente={0}", idCliente.Value) : idTabelaDesconto > 0 ? string.Format("IdTabelaDesconto={0}", idTabelaDesconto.Value) : string.Empty;

            if (string.IsNullOrWhiteSpace(filtroClienteTabela))
                return false;

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.DescontoAcrescimoCliente>()
                .Where(string.Format("{0}{1}{2}{3}",
                    filtroClienteTabela,
                    idGrupoProd > 0 ? string.Format(" AND IdGrupoProd={0}", idGrupoProd) : string.Empty,
                    idSubgrupoProd > 0 ? string.Format(" AND IdSubgrupoProd={0}", idSubgrupoProd) : string.Empty,
                    idProduto > 0 ? string.Format(" AND IdProduto={0}", idProduto) : " AND IdProduto IS NULL"))
                .Count()
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault() > 0;
        }

        #endregion
    }
}
