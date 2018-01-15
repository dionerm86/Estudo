using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de sugestão.
    /// </summary>
    public class SugestaoFluxo : ISugestaoFluxo
    {
        /// <summary>
        /// Pesquisa as sugestões do cliente.
        /// </summary>
        /// <param name="idSugestao">Identificador do sugestão</param>
        /// <param name="idCliente">Identificador do cliente associad a sugestão.</param>
        /// <param name="idFunc">Identificador do funcionário associado.</param>
        /// <param name="nomeFuncionario">Nome do funcionário.</param>
        /// <param name="nomeCliente">Nome do cliente.</param>
        /// <param name="dataInicio">Data de início.</param>
        /// <param name="dataFim">Data de fim.</param>
        /// <param name="tipo">Tipo.</param>
        /// <param name="descricao">Descrição.</param>
        /// <param name="situacoes">Situação.</param>
        /// <returns></returns>
        public IList<Entidades.SugestaoClientePesquisa> PesquisarSugestoes(
            int? idSugestao, int? idCliente, int? idFunc, string nomeFuncionario, string nomeCliente,
            DateTime? dataInicio, DateTime? dataFim, Data.Model.TipoSugestao? tipo,
            string descricao, Situacao[] situacoes, int? idRota, int? idPedido, uint? idOrcamento)
        {
            var descrNomeCliente = Configuracoes.Geral.ExibirRazaoSocialTelaSugestao ?
                "ISNULL(c.Nome, c.NomeFantasia)" : "ISNULL(c.NomeFantasia, c.Nome)";

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SugestaoCliente>("sc")
                .InnerJoin<Data.Model.Funcionario>("sc.Usucad = f.IdFunc", "f")
                .LeftJoin<Data.Model.Cliente>("sc.IdCliente = c.IdCli", "c")
                .LeftJoin<Data.Model.RotaCliente>("c.IdCli = rc.IdCliente", "rc")
                .LeftJoin<Data.Model.Rota>("rc.IdRota = r.IdRota", "r")
                .Select(string.Format(@"sc.IdSugestao, sc.IdCliente, sc.IdPedido, sc.DataCad, sc.TipoSugestao,
                          sc.Descricao, sc.Cancelada, f.IdFunc, f.Nome AS Funcionario, 
                          {0} AS Cliente, r.Descricao as DescricaoRota", descrNomeCliente));

            var whereClause = consulta.WhereClause;

            if (idSugestao.HasValue && idSugestao.Value > 0)
                whereClause
                    .And("sc.IdSugestao=?id")
                    .Add("?id", idSugestao)
                    .AddDescription(string.Format("Cód. Sugestão: {0}  ", idSugestao));

            else
            {
                if (idCliente.HasValue && idCliente.Value > 0)
                    whereClause
                        .And("sc.IdCliente=?idCliente")
                        .Add("?idCliente", idCliente)
                        .AddDescription(string.Format("Cód. Cliente: {0}", idCliente));

                if (idFunc.HasValue && idFunc.Value > 0)
                    whereClause
                        .And("sc.Usucad=?idFunc")
                        .Add("?idFunc", idFunc)
                        .AddDescription(() =>
                            string.Format("Funcionário: {0}  ",
                                SourceContext.Instance.CreateQuery()
                                .From<Data.Model.Funcionario>()
                                .Where("IdFunc=?idFunc")
                                .Add("?idFunc", idFunc)
                                .ProcessResultDescriptor<Entidades.Funcionario>()
                                .Select(f => f.Name)
                                .FirstOrDefault()));

                if (tipo.HasValue)
                    whereClause
                        .And("TipoSugestao=?tipo")
                        .Add("?tipo", tipo)
                        .AddDescription(string.Format("Tipo: {0}", tipo.Translate().Format()));

                if (!string.IsNullOrEmpty(nomeFuncionario))
                    whereClause
                        .And("f.Nome LIKE ?nomeFunc")
                        .Add("?nomeFunc", string.Format("%{0}%", nomeFuncionario))
                        .AddDescription("Funcionário: {0} ", nomeFuncionario);

                if (!string.IsNullOrEmpty(nomeCliente))
                    whereClause
                        .And("(c.Nome LIKE ?nomeCliente OR c.NomeFantasia LIKE ?nomeCliente)")
                        .Add("?nomeCliente", string.Format("%{0}%", nomeCliente))
                        .AddDescription(string.Format("Cliente: {0} ", nomeCliente));

                if (!string.IsNullOrEmpty(descricao))
                    whereClause
                        .And("sc.Descricao LIKE ?descricao")
                        .Add("?descricao", string.Format("%{0}%", descricao))
                        .AddDescription(string.Format("Descrição: {0} ", descricao));

                if (dataInicio.HasValue)
                    whereClause
                        .And("sc.DataCad>=?dataInicio")
                        .Add("?dataInicio", dataInicio.Value.Date)
                        .AddDescription(string.Format("Data inicial: {0} ", dataInicio.Value.ToString("dd-MM-yyyy")));

                if (dataFim.HasValue)
                    whereClause
                        .And("sc.DataCad<=?dataFim")
                        .Add("?dataFim", dataFim.Value.Date.AddDays(1).AddMinutes(-1))
                        .AddDescription(string.Format("Data final: {0} ", dataFim.Value.ToString("dd-MM-yyyy")));

                if (idPedido.GetValueOrDefault() > 0)
                    whereClause
                        .And("sc.IdPedido=?idPedido")
                        .Add("?idPedido", idPedido)
                        .AddDescription("Pedido: " + idPedido);

                if (idOrcamento.GetValueOrDefault() > 0)
                    whereClause
                        .And("sc.IdOrcamento=?idOrcamento")
                        .Add("?idOrcamento", idOrcamento)
                        .AddDescription("Orçamento: " + idOrcamento);

                if (situacoes != null && situacoes.Length > 0)
                {
                    whereClause
                        .And(string.Format("Cancelada IN ({0})",
                            string.Join(",", situacoes.Select(f => f == Situacao.Ativo ? "0" : "1").ToArray())));

                    if (situacoes.Length > 1)
                        whereClause.AddDescription(" Situação: Ativas e Canceladas");
                    
                    else
                    {
                        if (situacoes.Any(f => f == Situacao.Ativo))
                            whereClause.AddDescription(" Situação: Ativas");
                        if (situacoes.Any(f => f == Situacao.Inativo))
                            whereClause.AddDescription(" Situação: Canceladas");
                    }

                    if (idRota.HasValue && idRota.Value > 0)
                        whereClause
                        .And("r.IdRota=?idRora")
                        .Add("?idRora", idRota)
                        .AddDescription(string.Format("CÃ³d. Rota: {0}", idRota));
                }
            }

            return consulta.ToVirtualResult<Entidades.SugestaoClientePesquisa>();
        }

        /// <summary>
        /// Recupera os dados da sugestão.
        /// </summary>
        /// <param name="idSugestao"></param>
        /// <returns></returns>
        public Entidades.SugestaoCliente ObtemSugestao(int idSugestao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SugestaoCliente>()
                .Where("IdSugestao=?id")
                .Add("?id", idSugestao)
                .ProcessLazyResult<Entidades.SugestaoCliente>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Retorna uma nova instância de sugestão
        /// </summary>
        /// <returns></returns>
        public Entidades.SugestaoCliente CriarSugestaoCliente()
        {
            return SourceContext.Instance.Create<Entidades.SugestaoCliente>();
        }

        /// <summary>
        /// Salva os dados da sugestão.
        /// </summary>
        /// <param name="sugestao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarSugestao(Entidades.SugestaoCliente sugestao)
        {
            sugestao.Require("sugestao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = sugestao.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da sugestão.
        /// </summary>
        /// <param name="sugestao"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarSugestao(Entidades.SugestaoCliente sugestao)
        {
            sugestao.Require("sugestao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = sugestao.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
    }
}
