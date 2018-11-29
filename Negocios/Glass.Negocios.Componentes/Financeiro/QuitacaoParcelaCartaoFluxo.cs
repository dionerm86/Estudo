using Colosoft;
using Glass.Data.Helper;
using Glass.Financeiro.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Componentes
{
    public class QuitacaoParcelaCartaoFluxo : IQuitacaoParcelaCartaoFluxo
    {
        /// <summary>
        /// Retorna a lista de Quitação Parcela Cartão referente ao id do arquivo passado
        /// </summary>
        /// <param name="idArquivoQuitacaoParcelaCartao"></param>
        /// <returns></returns>
        public IList<QuitacaoParcelaCartaoPesquisa> PesquisarQuitacaoParcelaCartao(int idArquivoQuitacaoParcelaCartao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.QuitacaoParcelaCartao>("qpc")
                .LeftJoin<Data.Model.Funcionario>("qpc.Usucad=f.IdFunc", "f")
                .Where("IdArquivoQuitacaoParcelaCartao=?idArquivoQuitacaoParcelaCartao")
                .Add("?idArquivoQuitacaoParcelaCartao", idArquivoQuitacaoParcelaCartao)
                .Select(@"qpc.IdQuitacaoParcelaCartao, qpc.IdArquivoQuitacaoParcelaCartao, qpc.NumAutCartao, qpc.UltimosDigitosCartao, qpc.Valor, qpc.Tipo, qpc.Bandeira, qpc.NumParcela, qpc.NumParcelaMax,
                    qpc.Tarifa, qpc.Quitada, qpc.DataVenc as DataVencimento, qpc.DataCad as DataCadastro, f.Nome AS NomeFuncionarioCadastro");

            return consulta.ToVirtualResultLazy<QuitacaoParcelaCartaoPesquisa>();
        }

        /// <summary>
        /// Carrega o arquivo para a grid
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public List<QuitacaoParcelaCartaoPesquisa> CarregarArquivo(Stream stream)
        {
            var quitacaoParcelasCartoes = new List<QuitacaoParcelaCartaoPesquisa>();

            using (var reader = new StreamReader(stream))
            {
                var arquivo = reader.ReadToEnd();

                var linhas = System.Text.RegularExpressions.Regex.Split(arquivo, "\r\n");

                for (int i = 0; i < linhas.Length; i++)
                {
                    var reg1 = linhas[i];

                    if (string.IsNullOrWhiteSpace(reg1) || reg1[0] != '1' || (reg1.Substring(18, 2).Trim() != "" && reg1.Substring(18, 2) != "01"))
                        continue;

                    var regs2 = new List<string>();
                    var indexReg2 = i + 1;
                    while (linhas[indexReg2][0] == '2')
                    {
                        regs2.Add(linhas[indexReg2]);
                        indexReg2++;
                    }

                    var quitacaoParcelaCartaoPesquisa = new QuitacaoParcelaCartaoPesquisa();

                    for (int j = 0; j < regs2.Count; j++)
                    {
                        var reg2 = regs2[j];

                        // Posição 093 a 098 - Número sequencial, também conhecido como DOC 
                        var numAutCartao = reg2.Substring(92, 6);
                        // Posição 031 a 034 - Númerodocartãotruncado
                        var ultimosDigitosCartao = reg2.Substring(30, 4);

                        //var numeroEstabelecimento = reg2.Substring(1, 10);

                        // Posição 087 a 099 - Valor líquido
                        var valorLiquido = reg1.Substring(86, 13);
                        // Posição 086 - Sinal do valor líquido
                        var tipo = reg1.Substring(85, 1);
                        // Posição 185 a 187 - Código da Bandeira vide tabela VI
                        var bandeira = reg1.Substring(184, 3);
                        // Posição 060 a 061 - No caso de venda parcelada, será formatado com o número da parcela que está sendo liberada. No caso de venda à vista, será formatado com zeros.
                        var parcela = reg2.Substring(59, 2);
                        // Posição 062 a 063 - Número total de parcelas da venda. No caso de venda à vista, será formatado com zeros.
                        var totalParcelas = reg2.Substring(61, 2);
                        // Posição 214 a 218 - Tarifa cobrada por transação.
                        var tarifaAdministrativa = reg1.Substring(213, 5);
                        // Posição 032 a 037 - AAMMDD Data prevista de pagamento. Na recuperação, pode ser atualizada após oprocessamentoda transaçãoou ajuste.
                        DateTime dataVencimento;
                        DateTime.TryParseExact(reg1.Substring(31, 6), "yyMMdd", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dataVencimento);

                        quitacaoParcelaCartaoPesquisa.IdArquivoQuitacaoParcelaCartao = 0;
                        quitacaoParcelaCartaoPesquisa.NumAutCartao = numAutCartao;
                        quitacaoParcelaCartaoPesquisa.UltimosDigitosCartao = ultimosDigitosCartao;
                        quitacaoParcelaCartaoPesquisa.Valor = Conversoes.StrParaDecimal(valorLiquido) / 100;
                        quitacaoParcelaCartaoPesquisa.Tipo = tipo == "+" ? Data.Model.TipoCartaoEnum.Credito : Data.Model.TipoCartaoEnum.Debito;
                        //quitacaoParcelaCartaoPesquisa.Bandeira = 1;
                        quitacaoParcelaCartaoPesquisa.NumParcela = Conversoes.StrParaInt(parcela);
                        quitacaoParcelaCartaoPesquisa.NumParcelaMax = Conversoes.StrParaInt(totalParcelas);
                        quitacaoParcelaCartaoPesquisa.Tarifa = Conversoes.StrParaDecimal(tarifaAdministrativa) / 100;
                        // Busca por parcelas à receber
                        var cartoes = PesquisarCartoesNaoIdentificadosQuitarParcelas(null, ultimosDigitosCartao, numAutCartao, parcela).ToList();
                        quitacaoParcelaCartaoPesquisa.Quitada = cartoes.Count() > 0;

                        quitacaoParcelaCartaoPesquisa.DataVencimento = dataVencimento;
                        quitacaoParcelaCartaoPesquisa.DataCadastro = DateTime.Now;
                        quitacaoParcelaCartaoPesquisa.NomeFuncionarioCadastro = Data.Helper.UserInfo.GetUserInfo.Nome;
                    }

                    quitacaoParcelasCartoes.Add(quitacaoParcelaCartaoPesquisa);
                }
            }
            return quitacaoParcelasCartoes;
        }

        /// <summary>
        /// Recupera os valores da entidade
        /// </summary>
        /// <param name="idQuitacaoParcelaCartao"></param>
        /// <returns></returns>
        public QuitacaoParcelaCartao ObterQuitacaoParcelaCartao(int idQuitacaoParcelaCartao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.QuitacaoParcelaCartao>()
                .Where("IdQuitacaoParcelaCartao=?id")
                .Add("?id", idQuitacaoParcelaCartao)
                .ProcessLazyResult<Entidades.QuitacaoParcelaCartao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Cria um novo quitação para salvar no banco
        /// </summary>
        /// <param name="quitacaoParcelaCartao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult InserirNovoQuitacaoParcelaCartao(QuitacaoParcelaCartao quitacaoParcelaCartao)
        {
            quitacaoParcelaCartao.Require("quitacaoParcelaCartao").NotNull();

            var original = new QuitacaoParcelaCartao();
            original.IdArquivoQuitacaoParcelaCartao = quitacaoParcelaCartao.IdArquivoQuitacaoParcelaCartao;
            original.NumAutCartao = quitacaoParcelaCartao.NumAutCartao;
            original.UltimosDigitosCartao = quitacaoParcelaCartao.UltimosDigitosCartao;
            original.Valor = quitacaoParcelaCartao.Valor;
            original.Tipo = quitacaoParcelaCartao.Tipo;
            original.Bandeira = quitacaoParcelaCartao.Bandeira;
            original.NumParcela = quitacaoParcelaCartao.NumParcela;
            original.NumParcelaMax = quitacaoParcelaCartao.NumParcelaMax;
            original.Tarifa = quitacaoParcelaCartao.Tarifa;
            original.Quitada = quitacaoParcelaCartao.Quitada;
            original.DataVencimento = quitacaoParcelaCartao.DataVencimento;
            original.DataCadastro = quitacaoParcelaCartao.DataCadastro;
            //original.IdUsuarioCadastro

            return SalvarQuitacaoParcelaCartao(original);
        }

        /// <summary>
        /// Salva os dados da entidade
        /// </summary>
        /// <param name="quitacaoParcelaCartao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarQuitacaoParcelaCartao(QuitacaoParcelaCartao quitacaoParcelaCartao)
        {
            quitacaoParcelaCartao.Require("quitacaoParcelaCartao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult retorno;

                if (!(retorno = quitacaoParcelaCartao.Save(session)))
                    return retorno;

                retorno = session.Execute(false).ToSaveResult();

                return retorno;
            }
        }

        /// <summary>
        /// Retorna as parcelas de cartão a receber, com e sem CNI
        /// </summary>
        /// <param name="numeroEstabelecimento"></param>
        /// <param name="ultimosDigitosCartao"></param>
        /// <param name="numAutCartao"></param>
        /// <returns></returns>
        private IList<CartaoNaoIdentificadoQuitarParcelasPesquisa> PesquisarCartoesNaoIdentificadosQuitarParcelas(string numeroEstabelecimento, string ultimosDigitosCartao, string numAutCartao,
            string numeroParcela)
        {
            #region Consulta CNI

            var consultaCNI = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>("cni")
                .LeftJoin<Data.Model.ContasReceber>("cni.IdCartaoNaoIdentificado = cr.IdCartaoNaoIdentificado", "cr")
                .Where("cr.IsParcelaCartao = 1 AND IsNull(cr.Recebida, 0) = 0")
                .Select(@"cni.IdContaBanco, cr.IdContaR");

            if (!numAutCartao.IsNullOrEmpty())
                consultaCNI.WhereClause
                  .And("cni.NumAutCartao=?nAutorizacao")
                  .Add("?nAutorizacao", numAutCartao);

            if (!numeroEstabelecimento.IsNullOrEmpty())
                consultaCNI.WhereClause
                    .And("cni.NumeroEstabelecimento=?numEstabelecimento")
                    .Add("?numEstabelecimento", numeroEstabelecimento);

            if (!ultimosDigitosCartao.IsNullOrEmpty())
                consultaCNI.WhereClause
                    .And("cni.UltimosDigitosCartao=?ultimosDigitosCartao")
                    .Add("?ultimosDigitosCartao", ultimosDigitosCartao);

            // Foi adicionado a verificação de parcela "00" porque no arquivo ela é utilizada para parcela única, e no sistema não é utilizada.
                consultaCNI.WhereClause
                    .And("cr.NumParc=?numeroParcela")
                    .Add("?numeroParcela", Math.Max(Glass.Conversoes.StrParaInt(numeroParcela), 1));

            #endregion

            #region Consulta PagtoContaReceber

            // Consulta contas a receber sem CNI
            var consultaContasReceber = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>("cr")
                .LeftJoin<Data.Model.PagtoContasReceber>("pcr.IdContaR=cr.IdContaRRef", "pcr")
                .Where("IsNull(cr.IdAcerto, 0) = 0 AND IsNull(cr.IdCartaoNaoIdentificado, 0) = 0 AND cr.IsParcelaCartao = 1 AND IsNull(cr.Recebida, 0) = 0")
                .Select(@"pcr.IdContaBanco, cr.IdContaR");

            consultaContasReceber.WhereClause
                .And("pcr.NumAutCartao=?nAutorizacao")
                .Add("?nAutorizacao", numAutCartao);

            // Foi adicionado a verificação de parcela "00" porque no arquivo ela é utilizada para parcela única, e no sistema não é utilizada.
            if (!numeroParcela.IsNullOrEmpty() && numeroParcela != "00")
                consultaContasReceber.WhereClause
                    .And("cr.NumParc=?numeroParcela")
                    .Add("?numeroParcela", numeroParcela);

            #endregion

            #region Consulta PagtoAcerto

            // Consulta contas a receber de acerto
            var consultaAcerto = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>("cr")
                .LeftJoin<Data.Model.PagtoAcerto>("pa.IdAcerto=cr.IdAcerto", "pa")
                .Where("IsNull(cr.IdCartaoNaoIdentificado, 0) = 0 AND cr.IsParcelaCartao = 1 AND IsNull(cr.Recebida, 0) = 0")
                .Select(@"pa.IdContaBanco, cr.IdContaR");

            consultaAcerto.WhereClause
                .And("pa.NumAutCartao=?nAutorizacao")
                .Add("?nAutorizacao", numAutCartao);

            // Foi adicionado a verificação de parcela "00" porque no arquivo ela é utilizada para parcela única, e no sistema não é utilizada.
            if (!numeroParcela.IsNullOrEmpty() && numeroParcela != "00")
                consultaAcerto.WhereClause
                    .And("cr.NumParc=?numeroParcela")
                    .Add("?numeroParcela", numeroParcela);

            #endregion

            return consultaCNI.UnionAll(consultaContasReceber).UnionAll(consultaAcerto).ToVirtualResultLazy<CartaoNaoIdentificadoQuitarParcelasPesquisa>();
        }

        /// <summary>
        /// Importa o arquivo para quitar as parcelas do cartão
        /// </summary>
        public Colosoft.Business.SaveResult QuitarParcelas(List<QuitacaoParcelaCartao> quitacaoParcelaCartao)
        {
            var falhas = new List<string>();

            if (quitacaoParcelaCartao.Count() == 0)
                return new Colosoft.Business.SaveResult(false, "Não ha parcelas no arquivo para ser quitada.".GetFormatter());

            // List<KeyValuePair<IdContaBanco, IdContaR>
            List<KeyValuePair<uint, uint>> contasQuitar = new List<KeyValuePair<uint, uint>>();

            foreach (var q in quitacaoParcelaCartao)
            {
                var cartoes = PesquisarCartoesNaoIdentificadosQuitarParcelas(null, q.UltimosDigitosCartao, q.NumAutCartao, q.NumParcela.ToString()).ToList();

                foreach (var c in cartoes)
                {
                    if (!contasQuitar.Any(p => p.Key == (uint)c.IdContaBanco && p.Value == (uint)c.IdContaR))
                    {
                        contasQuitar.Add(new KeyValuePair<uint, uint>((uint)c.IdContaBanco, (uint)c.IdContaR));
                    }
                }

                // Salva a quitação
                InserirNovoQuitacaoParcelaCartao(q);
            }

            try
            {
                Glass.Data.DAL.ContasReceberDAO.Instance.QuitarVariasParcCartao(quitacaoParcelaCartao.First().IdArquivoQuitacaoParcelaCartao, contasQuitar,
                    quitacaoParcelaCartao.First().DataVencimento.ToShortDateString(), false);
            }
            catch (Exception ex)
            {
                falhas.Add(ex.Message);
                // Marca o arquivo de quitação como cancelado.
                var arquivoQuitacao = ObterArquivoQuitacaoParcelaCartao(quitacaoParcelaCartao.First().IdArquivoQuitacaoParcelaCartao);
                arquivoQuitacao.Situacao = Data.Model.SituacaoArquivoQuitacaoParcelaCartao.Cancelado;
                SalvarArquivo(arquivoQuitacao);
            }

            return new Colosoft.Business.SaveResult(falhas.Count == 0, string.Join("\r\n", falhas).GetFormatter());
        }

        /// <summary>
        /// Cancela o arquivo passado
        /// </summary>
        public Colosoft.Business.SaveResult CancelarArquivoQuitacaoParcelaCartao(int idArquivoQuitacaoParcelaCartao, bool estornarMovimentacaoBancaria, DateTime? dataEstornoBanco, string motivo)
        {
            var arquivoQuitacaoParcelaCartao = ObterArquivoQuitacaoParcelaCartao(idArquivoQuitacaoParcelaCartao);

            try
            {
                var idsContaR = Data.DAL.ContasReceberDAO.Instance.ObterIdsContaRPeloIdArquivoQuitacaoParcelaCartao(null, arquivoQuitacaoParcelaCartao.IdArquivoQuitacaoParcelaCartao);
                if (idsContaR != null)
                    foreach (var idContaR in idsContaR)
                        Data.DAL.ContasReceberDAO.Instance.CancelarRecebimentoParcCartao(idContaR, idArquivoQuitacaoParcelaCartao, estornarMovimentacaoBancaria, dataEstornoBanco, motivo);
            }
            catch (Exception ex)
            {
                return new Colosoft.Business.SaveResult(false, ex.Message.GetFormatter());
            }

            arquivoQuitacaoParcelaCartao.Situacao = Data.Model.SituacaoArquivoQuitacaoParcelaCartao.Cancelado;
            return SalvarArquivo(arquivoQuitacaoParcelaCartao);
        }

        #region Arquivo

        /// <summary>
        /// Retorna lista de arquivos quitação parcela cartão
        /// </summary>
        public IList<ArquivoQuitacaoParcelaCartaoPesquisa> PesquisarArquivoQuitacaoParcelaCartao()
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ArquivoQuitacaoParcelaCartao>("aqpc")
                .LeftJoin<Data.Model.Funcionario>("aqpc.Usucad=f.IdFunc", "f")
                .Select(@"aqpc.IdArquivoQuitacaoParcelaCartao, aqpc.DataCad, aqpc.Situacao, f.Nome AS NomeFuncionarioCadastro");

            return consulta.ToVirtualResult<ArquivoQuitacaoParcelaCartaoPesquisa>();
        }

        /// <summary>
        /// Insere um novo arquivo
        /// </summary>
        public Colosoft.Business.SaveResult InserirNovoArquivo(Stream stream, string extensao)
        {
            if (stream.Length == 0 || string.IsNullOrEmpty(extensao))
                return new Colosoft.Business.SaveResult(false, "Arquivo ou Extensão não disponível.".GetFormatter());

            var novoArquivo = SourceContext.Instance.Create<ArquivoQuitacaoParcelaCartao>();
            novoArquivo.Situacao = Data.Model.SituacaoArquivoQuitacaoParcelaCartao.Ativo;

            var resultado = SalvarArquivo(novoArquivo);

            if (!resultado)
                return new Colosoft.Business.SaveResult(false, resultado.Message);

            var caminho = string.Format(@"{0}\{1}{2}", Utils.GetArquivoQuitacaoParcelaCartaoPath, novoArquivo.IdArquivoQuitacaoParcelaCartao, extensao);

            try
            {
                // Salva o arquivo na pasta
                using (var fileStream = File.Create(caminho))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                ApagarArquivo(novoArquivo);
                return new Colosoft.Business.SaveResult(false, ex.Message.ToString().GetFormatter());
            }

            return new Colosoft.Business.SaveResult(true, novoArquivo.IdArquivoQuitacaoParcelaCartao.ToString().GetFormatter());
        }

        /// <summary>
        /// Salva o Arquivo
        /// </summary>
        private Colosoft.Business.SaveResult SalvarArquivo(ArquivoQuitacaoParcelaCartao arquivoQuitacaoParcelaCartao)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult retorno;

                if (!(retorno = arquivoQuitacaoParcelaCartao.Save(session)))
                    return retorno;

                retorno = session.Execute(false).ToSaveResult();

                return retorno;
            }
        }

        /// <summary>
        /// Apaga o arquivo
        /// </summary>
        private Colosoft.Business.DeleteResult ApagarArquivo(ArquivoQuitacaoParcelaCartao arquivoQuitacaoParcelaCartao)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.DeleteResult retorno;

                if (!(retorno = arquivoQuitacaoParcelaCartao.Delete(session)))
                    return retorno;

                retorno = session.Execute(false).ToDeleteResult();

                return retorno;
            }
        }

        /// <summary>
        /// Recupera os valores da entidade
        /// </summary>
        /// <param name="idArquivoQuitacaoParcelaCartao"></param>
        /// <returns></returns>
        public ArquivoQuitacaoParcelaCartao ObterArquivoQuitacaoParcelaCartao(int idArquivoQuitacaoParcelaCartao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ArquivoQuitacaoParcelaCartao>()
                .Where("IdArquivoQuitacaoParcelaCartao=?id")
                .Add("?id", idArquivoQuitacaoParcelaCartao)
                .ProcessLazyResult<Entidades.ArquivoQuitacaoParcelaCartao>()
                .FirstOrDefault();
        }

        #endregion
    }
}
