using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Fiscal.Negocios.Entidades;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio do CFOP.
    /// </summary>
    public class CfopFluxo : ICfopFluxo, IValidadorNaturezaOperacao
    {
        #region Local Variables

        private Glass.Negocios.IControleAlteracao _controleAlteracao;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="controleAlteracao"></param>
        public CfopFluxo(Glass.Negocios.IControleAlteracao controleAlteracao)
        {
            _controleAlteracao = controleAlteracao;
        }

        #endregion

        #region TipoCfop

        /// <summary>
        /// Recupera a relação dos tipos de CFOP.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemTiposCfop()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoCfop>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.TipoCfop>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do tipo de CFOP.
        /// </summary>
        /// <param name="idTipoCfop">Identificador do tipo.</param>
        /// <returns></returns>
        public Entidades.TipoCfop ObtemTipoCfop(int idTipoCfop)
        {
            return SourceContext.Instance.CreateQuery()
                .Where("IdTipoCfop=?id")
                .Add("?id", idTipoCfop)
                .ProcessResult<Entidades.TipoCfop>()
                .FirstOrDefault();
        }

        #endregion

        #region Cfop

        /// <summary>
        /// Pesquisa os Cfops.
        /// </summary>
        /// <param name="codInterno">Código interno usado como filtro.</param>
        /// <param name="descricao">Descrição usada como filtro.</param>
        /// <returns></returns>
        public IList<Entidades.CfopPesquisa> PesquisarCfops(string codInterno, string descricao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>("c")
                .LeftJoin<Data.Model.TipoCfop>("c.IdTipoCfop = t.IdTipoCfop", "t")
                .OrderBy("CodInterno")
                .Select("c.IdCfop, c.CodInterno, c.IdTipoCfop, c.Descricao, c.AlterarEstoqueTerceiros, c.AlterarEstoqueCliente, c.Obs, c.TipoMercadoria, t.Descricao AS Tipo");

            if (!string.IsNullOrEmpty(codInterno))
                consulta.WhereClause
                        .And("c.CodInterno LIKE ?codInterno")
                        .Add("?codInterno", string.Format("%{0}%", codInterno.Replace('%', ' ')));

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                        .And("c.Descricao LIKE ?descricao")
                        .Add("?descricao", string.Format("%{0}%", descricao.Replace('%', ' ')));

            return consulta.ToVirtualResult<Entidades.CfopPesquisa>();
                
        }

        /// <summary>
        /// Recupera os descritores dos CFOPs.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCfops()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>()
                .OrderBy("CodInterno")
                .ProcessResultDescriptor<Entidades.Cfop>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public Entidades.Cfop ObtemCfop(int idCfop)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>()
                .Where("IdCfop=?id")
                .Add("?id", idCfop)
                .ProcessLazyResult<Entidades.Cfop>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera o CFOP pelol código interno informado.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public Entidades.Cfop ObtemCfopPorCodInterno(string codInterno)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>()
                .Where("CodInterno=?codInterno")
                .Add("?codInterno", codInterno)
                .ProcessLazyResult<Entidades.Cfop>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCfop(Entidades.CfopPesquisa cfop)
        {
            cfop.Require("cfop").NotNull();
            var original = ObtemCfop(cfop.IdCfop);

            original.CodInterno = cfop.CodInterno;
            original.Descricao = cfop.Descricao;
            original.IdTipoCfop = cfop.IdTipoCfop;
            original.TipoMercadoria = cfop.TipoMercadoria;
            original.AlterarEstoqueTerceiros = cfop.AlterarEstoqueTerceiros;
            original.AlterarEstoqueCliente = cfop.AlterarEstoqueCliente;
            original.Obs = cfop.Obs;

            return SalvarCfop(original);
        }

        /// <summary>
        /// Salva os dados do CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCfop(Entidades.Cfop cfop)
        {
            cfop.Require("cfop").NotNull();

            if (!cfop.ExistsInStorage && 
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cfop>()
                    .Where("CodInterno=?codInterno")
                    .Add("?codInterno", cfop.CodInterno)
                    .ExistsResult())
                return new Colosoft.Business.SaveResult(false, "Este CFOP já foi cadastrado.".GetFormatter());

            // Se o código do CFOP estiver sendo alterado, não permite realizar esta alteração 
            // se este CFOP já estiver sendo usado por alguma nota fiscal
            if (cfop.ExistsInStorage &&
                cfop.ChangedProperties.Any(f => f == "CodInterno") &&
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>("nf")
                    .InnerJoin<Data.Model.NaturezaOperacao>("nf.IdNaturezaOperacao = n.IdNaturezaOperacao", "n")
                    .Where("n.IdCfop=?id")
                    .Add("?id", cfop.IdCfop)
                    .ExistsResult())
                return new Colosoft.Business.SaveResult(false,
                    "O código deste CFOP não pode ser alterado por haver notas fiscais relacionadas ao mesmo.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                if (!cfop.ExistsInStorage && cfop.NaturezasOperacao.Count == 0)
                    // Insere um natureza a operação padrão
                    cfop.NaturezasOperacao.Add(new Entidades.NaturezaOperacao());

                var resultado = cfop.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }                    
        }

        /// <summary>
        /// Apaga os dados do CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCfop(int idCfop)
        {
            var cfop = ObtemCfop(idCfop);

            if (cfop == null)
                return new Colosoft.Business.DeleteResult(false, "CFOP não encontrado".GetFormatter());

            return ApagarCfop(cfop);
        }

        /// <summary>
        /// Apaga os dados do CFOP associado com registro a pesquisa.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCfop(Entidades.CfopPesquisa cfop)
        {
            cfop.Require("cfop").NotNull();

            var original = ObtemCfop(cfop.IdCfop);

            if (original == null)
                return new Colosoft.Business.DeleteResult(false, "CFOP não encontrado".GetFormatter());

            return ApagarCfop(original);
        }

        /// <summary>
        /// Apaga os dados do CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCfop(Entidades.Cfop cfop)
        {
            cfop.Require("cfop").NotNull();

            // Verifica se esta CFOP está sendo utilizada em alguma nota
            if (SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>("nf")
                    .InnerJoin<Data.Model.NaturezaOperacao>("nf.IdNaturezaOperacao = n.IdNaturezaOperacao", "n")
                    .Where("n.IdCfop=?id").Add("?id", cfop.IdCfop)
                    .ExistsResult())
                return new Colosoft.Business.DeleteResult(false,
                    "Este CFOP não pode ser excluído por haver notas fiscais relacionadas ao mesmo.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = cfop.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Verifica se o CFOP é de entrada.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public bool VerificarCfopEntrada(int idCfop)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>()
                .Where("IdCfop=?id")
                .Add("?id", idCfop)
                .Select("CodInterno")
                .Execute()
                .Select(f => f.GetString(0))
                .Where(f => !string.IsNullOrEmpty(f) && char.IsDigit(f[0]) && int.Parse(f[0].ToString()) < 4)
                .Any();

            /*string codInterno = ObtemValorCampo<string>("codInterno", "idCfop=" + idCfop);
            return Glass.Conversoes.StrParaInt(codInterno[0].ToString()) < 4;*/
        }

        /// <summary>
        /// Verifica se CFOP é do tipo que está configurado como devolução.
        /// </summary>
        /// <param name="idTipoCfop"></param>
        /// <returns></returns>
        public bool VerificarCfopDevolucao(int idCfop)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cfop>("c")
                .InnerJoin<Data.Model.TipoCfop>("c.IdTipoCfop = t.IdTipoCfop", "t")
                .Where("c.IdCfop = ?id AND t.Devolucao=1")
                .Add("?id", idCfop)
                .ExistsResult();
        }

        #endregion

        #region Natureza Operação

        /// <summary>
        /// Valida o código interno da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        IMessageFormattable IValidadorNaturezaOperacao.ValidaCodigoInterno(Entidades.NaturezaOperacao naturezaOperacao)
        {
            var resultado = new List<IMessageFormattable>();

            if (string.IsNullOrEmpty(naturezaOperacao.CodInterno))
            {
                var item = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.NaturezaOperacao>("n")
                            .LeftJoin<Data.Model.Cfop>("n.IdCfop = c.IdCfop", "c")
                            .Select("c.CodInterno")
                            .Where("n.IdCfop=?idCfop AND (n.CodInterno IS NULL OR n.CodInterno = '') AND n.IdNaturezaOperacao<>?idNat")
                            .Add("?idCfop", naturezaOperacao.IdCfop)
                            .Add("?idNat", naturezaOperacao.IdNaturezaOperacao)
                            .Execute()
                            .Select(f => f.GetString(0))
                            .FirstOrDefault();

                if (item != null)
                    return string.Format("Já foi cadastrada a natureza de operação padrão para o CFOP {0}.", item).GetFormatter();
            }
            else
            {
                var item = SourceContext.Instance.CreateQuery()
                        .From<Data.Model.NaturezaOperacao>("n")
                        .LeftJoin<Data.Model.Cfop>("n.IdCfop = c.IdCfop", "c")
                        .Select("c.CodInterno")
                        .Where("n.IdCfop=?idCfop AND CodInterno=?codInterno AND IdNaturezaOperacao<>?idNat")
                        .Add("?idCfop", naturezaOperacao.IdCfop)
                        .Add("?codInterno", (!string.IsNullOrEmpty(naturezaOperacao.CodInterno) ? naturezaOperacao.CodInterno.Trim() : naturezaOperacao.CodInterno))
                        .Add("?idNat", naturezaOperacao.IdNaturezaOperacao)
                        .Execute()
                        .Select(f => f.GetString(0))
                        .FirstOrDefault();

                if (item != null)
                    return string.Format("O código '{0}' já está cadastrado para o CFOP {1}.", naturezaOperacao.CodInterno, item).GetFormatter();
            }

            return null;
        }

        /// <summary>
        /// Valida a existencia da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        IMessageFormattable[] IValidadorNaturezaOperacao.ValidaExistencia(Entidades.NaturezaOperacao naturezaOperacao)
        {
            var resultado = new List<IMessageFormattable>();
            var consultas = SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.NotaFiscal>()
                        .Where("IdNaturezaOperacao=?id")
                        .Add("?id", naturezaOperacao.IdNaturezaOperacao)
                        .Count(), 
                            (sender, query, result) =>
                            {
                                if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                                    resultado.Add("Ela é utilizada em pelo menos uma nota fiscal.".GetFormatter());
                            })
                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutosNf>()
                        .Where("IdNaturezaOperacao=?id")
                        .Add("?id", naturezaOperacao.IdNaturezaOperacao)
                        .Count(),
                            (sender, query, result) =>
                            {
                                if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                                    resultado.Add("Ela é utilizada em pelo menos um produto de nota fiscal.".GetFormatter());
                            })
                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Cte.ConhecimentoTransporte>()
                        .Where("IdNaturezaOperacao=?id")
                        .Add("?id", naturezaOperacao.IdNaturezaOperacao)
                        .Count(),
                            (sender, query, result) =>
                            {
                                if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                                    resultado.Add("Ela é utilizada em pelo menos um conhecimento de transporte.".GetFormatter());
                            });

            consultas.Execute();

            return resultado.ToArray();           
        }

        /// <summary>
        /// Pesquisa as naturezas de operação associadas com o CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public IList<Entidades.NaturezaOperacao> PesquisarNaturezasOperacao(int idCfop)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.NaturezaOperacao>()
                .Where("IdCfop=?idCfop")
                .Add("?idCfop", idCfop)
                .OrderBy("CodInterno")
                .ToVirtualResultLazy<Entidades.NaturezaOperacao>();
        }

        /// <summary>
        /// Pesquisa as naturezas de operação associadas com o CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="codNaturezaOperacao">Códig da natureza de operação.</param>
        /// <param name="codigoCfop">Código do CFOP.</param>
        /// <param name="descricaoCfop">Descrição do CFOP.</param>
        /// <returns></returns>
        public IList<Entidades.NaturezaOperacaoPesquisa> PesquisarNaturezasOperacao
            (int idCfop, string codNaturezaOperacao, string codigoCfop, string descricaoCfop)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.NaturezaOperacao>("n")
                .InnerJoin<Data.Model.Cfop>("n.IdCfop = c.IdCfop", "c")
                .Select("n.IdNaturezaOperacao, n.IdCfop, n.CodInterno, n.CalcIcms, " +
                        "n.CalcIcmsSt, n.CalcIpi, n.CalcPis, n.CalcCofins, " +
                        "n.IpiIntegraBcIcms, n.AlterarEstoqueFiscal, " +
                        "n.CstIcms, n.PercReducaoBcIcms, n.CstIpi, n.CstPisCofins, " +
                        "n.Csosn, c.CodInterno AS CodCfop, c.Descricao AS DescricaoCfop, c.CodEnqIpi, n.CalcEnergiaEletrica")
                .OrderBy("CodCfop ASC, CodInterno ASC");

            if (idCfop > 0)
                consulta.WhereClause
                    .And("n.IdCfop=?idCfop").Add("?idCfop", idCfop);

            if (!string.IsNullOrEmpty(codNaturezaOperacao))
                consulta.WhereClause
                    .And("n.CodInterno=?codInterno").Add("?codInterno", codNaturezaOperacao);

            if (!string.IsNullOrEmpty(descricaoCfop))
                consulta.WhereClause
                    .And("c.Descricao LIKE ?descricao")
                    .Add("?descricao", string.Format("%{0}%", descricaoCfop));

            if (!string.IsNullOrEmpty(codigoCfop))
                consulta.WhereClause
                    .And("c.CodInterno=?codCfop").Add("?codCfop", codigoCfop);

            return consulta.ToVirtualResult<Entidades.NaturezaOperacaoPesquisa>();
        }

        /// <summary>
        /// Recupera os dados da natureza de operação.
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public Entidades.NaturezaOperacao ObtemNaturezaOperacao(int idNaturezaOperacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.NaturezaOperacao>()
                .Where("IdNaturezaOperacao=?id")
                .Add("?id", idNaturezaOperacao)
                .ProcessLazyResult<Entidades.NaturezaOperacao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarNaturezaOperacao(Entidades.NaturezaOperacao naturezaOperacao)
        {
            naturezaOperacao.Require("naturezaOperacao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = naturezaOperacao.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }    

        /// <summary>
        /// Apaga os dados da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao">Instancia que será apagada.</param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarNaturezaOperacao(Entidades.NaturezaOperacao naturezaOperacao)
        {
            naturezaOperacao.Require("naturezaOperacao").NotNull();

            if (string.IsNullOrEmpty(naturezaOperacao.CodInterno))
                return new Colosoft.Business.DeleteResult(false,
                    "Não é possível excluir a natureza de operação padrão.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = naturezaOperacao.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Regra Natureza Operação

        /// <summary>
        /// Pesquisa as regras de natureza de operação do sistema.
        /// </summary>
        /// <param name="idLoja">Identificador da loja que será usado no filtro.</param>
        /// <param name="idTipoCliente">Identificador do tipo de cliente que será usado no filtro.</param>
        /// <param name="idGrupoProd">Identificador do grupo de produção que será usado no filtro.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produção que será usado no filtro.</param>
        /// <param name="idCorVidro">Identificador da cor do vidro.</param>
        /// <param name="idCorFerragem">Idenfificador da cor da ferragem.</param>
        /// <param name="idCorAluminio">Identificador da cor do alumínio.</param>
        /// <param name="espessura">Espessura.</param>
        /// <param name="idNaturezaOperacao">Identificador da natureza de operação.</param>
        /// <returns></returns>
        public IList<Entidades.RegraNaturezaOperacaoPesquisa> PesquisarRegrasNaturezaOperacao
            (int idLoja, int idTipoCliente, int idGrupoProd, int idSubgrupoProd,
             int idCorVidro, int idCorFerragem, int idCorAluminio, float espessura,
             int idNaturezaOperacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.RegraNaturezaOperacao>("rno")
                .LeftJoin<Data.Model.Loja>("rno.IdLoja = l.IdLoja", "l")
                .LeftJoin<Data.Model.TipoCliente>("rno.IdTipoCliente = tc.IdTipoCliente", "tc")
                .LeftJoin<Data.Model.GrupoProd>("rno.IdGrupoProd = gp.IdGrupoProd", "gp")
                .LeftJoin<Data.Model.SubgrupoProd>("rno.IdSubgrupoProd = sgp.IdSubgrupoProd", "sgp")
                .LeftJoin<Data.Model.CorVidro>("rno.IdCorVidro = cv.IdCorVidro", "cv")
                .LeftJoin<Data.Model.CorFerragem>("rno.IdCorFerragem = cf.IdCorFerragem", "cf")
                .LeftJoin<Data.Model.CorAluminio>("rno.IdCorAluminio = ca.IdCorAluminio", "ca")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoProdIntra = nopi.IdNaturezaOperacao", "nopi")
                .LeftJoin<Data.Model.Cfop>("(nopi.CodInterno IS NULL OR nopi.CodInterno = '') AND nopi.IdCfop = cfpi.IdCfop", "cfpi")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoRevIntra = nori.IdNaturezaOperacao", "nori")
                .LeftJoin<Data.Model.Cfop>("(nori.CodInterno IS NULL OR nori.CodInterno = '') AND nori.IdCfop = cfri.IdCfop", "cfri")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoProdInter = nopir.IdNaturezaOperacao", "nopir")
                .LeftJoin<Data.Model.Cfop>("(nopir.CodInterno IS NULL OR nopir.CodInterno = '') AND nopir.IdCfop = cfpir.IdCfop", "cfpir")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoRevInter = norir.IdNaturezaOperacao", "norir")
                .LeftJoin<Data.Model.Cfop>("(norir.CodInterno IS NULL OR norir.CodInterno = '') AND norir.IdCfop = cfrir.IdCfop", "cfrir")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoProdStIntra = nopsi.IdNaturezaOperacao", "nopsi")
                .LeftJoin<Data.Model.Cfop>("(nopsi.CodInterno IS NULL OR nopsi.CodInterno = '') AND nopsi.IdCfop = cfpsi.IdCfop", "cfpsi")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoRevStIntra = norsi.IdNaturezaOperacao", "norsi")
                .LeftJoin<Data.Model.Cfop>("(norsi.CodInterno IS NULL OR norsi.CodInterno = '') AND norsi.IdCfop = cfrsi.IdCfop", "cfrsi")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoProdStInter = nopsir.IdNaturezaOperacao", "nopsir")
                .LeftJoin<Data.Model.Cfop>("(nopsir.CodInterno IS NULL OR nopsir.CodInterno = '') AND nopsir.IdCfop = cfpsir.IdCfop", "cfpsir")
                .LeftJoin<Data.Model.NaturezaOperacao>("rno.IdNaturezaOperacaoRevStInter = norsir.IdNaturezaOperacao", "norsir")
                .LeftJoin<Data.Model.Cfop>("(norsir.CodInterno IS NULL OR norsir.CodInterno = '') AND norsir.IdCfop = cfrsir.IdCfop", "cfrsir")
                .OrderBy("IdLoja, IdTipoCliente, IdGrupoProd, IdSubgrupoProd")
                .Select(
                    @"rno.IdRegraNaturezaOperacao, rno.IdLoja, rno.IdTipoCliente, rno.Espessura,
                      l.NomeFantasia AS NomeFantasiaLoja, l.RazaoSocial AS RazaoSocialLoja, tc.Descricao AS DescricaoTipoCliente,
                      gp.Descricao AS DescricaoGrupoProduto, sgp.Descricao AS DescricaoSubgrupoProduto,
                      cv.Descricao AS DescricaoCorVidro, cf.Descricao AS DescricaoCorFerragem,
                      ca.Descricao AS DescricaoCorAluminio,
                      ISNULL(nopi.CodInterno, cfpi.CodInterno) AS DescricaoNaturezaOperacaoProducaoIntra,
                      ISNULL(nori.CodInterno, cfri.CodInterno) AS DescricaoNaturezaOperacaoRevendaIntra,
                      ISNULL(nopir.CodInterno, cfpir.CodInterno) AS DescricaoNaturezaOperacaoProducaoInter,
                      ISNULL(norir.CodInterno, cfrir.CodInterno) AS DescricaoNaturezaOperacaoRevendaInter,
                      ISNULL(nopsi.CodInterno, cfpsi.CodInterno) AS DescricaoNaturezaOperacaoProducaoStIntra,
                      ISNULL(norsi.CodInterno, cfrsi.CodInterno) AS DescricaoNaturezaOperacaoRevendaStIntra,
                      ISNULL(nopsir.CodInterno, cfpsir.CodInterno) AS DescricaoNaturezaOperacaoProducaoStInter,
                      ISNULL(norsir.CodInterno, cfrsir.CodInterno) AS DescricaoNaturezaOperacaoRevendaStInter");

            var clausula = consulta.WhereClause;

            if (idLoja > 0)
                clausula.And("rno.IdLoja=?idLoja").Add("?idLoja", idLoja);

            if (idTipoCliente > 0)
                clausula.And("rno.IdTipoCliente=?idTipoCliente").Add("?idTipoCliente", idTipoCliente);

            if (idGrupoProd > 0)
                clausula.And("rno.IdGrupoProd=?idGrupoProd").Add("?idGrupoProd", idGrupoProd);

            if (idSubgrupoProd > 0)
                clausula.And("rno.IdSubgrupoProd=?idSubgrupoProd").Add("?idSubgrupoProd", idSubgrupoProd);

            if (idCorVidro > 0)
                clausula.And("rno.IdCorVidro=?idCorVidro").Add("?idCorVidro", idCorVidro);

            if (idCorFerragem > 0)
                clausula.And("rno.IdCorFerragem=?idCorFerragem").Add("?idCorFerragem", idCorFerragem);

            if (idCorAluminio > 0)
                clausula.And("rno.IdCorAluminio=?idCorAluminio").Add("?idCorAluminio", idCorAluminio);

            if (espessura > 0)
                clausula.And("rno.Espessura=?espessura").Add("?espessura", espessura);

            if (idNaturezaOperacao > 0)
                clausula.And(@"(rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR 
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR 
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao OR 
                                rno.IdNaturezaOperacaoProdIntra=?idNaturezaOperacao)")
                        .Add("?idNaturezaOperacao", idNaturezaOperacao);

            var retorno = consulta.ToVirtualResult<Entidades.RegraNaturezaOperacaoPesquisa>();

            return retorno;
        }

        /// <summary>
        /// Recupera os dados da regra.
        /// </summary>
        /// <param name="idRegraNaturezaOperacao"></param>
        /// <returns></returns>
        public Entidades.RegraNaturezaOperacao ObtemRegraNaturezaOperacao(int idRegraNaturezaOperacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.RegraNaturezaOperacao>()
                .Where("IdRegraNaturezaOperacao=?id")
                .Add("?id", idRegraNaturezaOperacao)
                .ProcessLazyResult<Entidades.RegraNaturezaOperacao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a regra a natureza de operação.
        /// </summary>
        /// <param name="regra"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarRegraNaturezaOperacao(Entidades.RegraNaturezaOperacao regra)
        {
            regra.Require("regra").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = regra.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da regra da natureza de operação.
        /// </summary>
        /// <param name="regra"></param>
        /// <param name="motivo">Motivo do cancelamento da regra.</param>
        /// <param name="manual">Identifica se a exclusão foi manual.</param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarRegraNaturezaOperacao(Entidades.RegraNaturezaOperacao regra, string motivo, bool manual)
        {
            regra.Require("regra").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                _controleAlteracao.RegistraExclusao(session, regra);
                _controleAlteracao.IgnoreLogExclusao(regra);

                var resultado = regra.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion
    }
}
