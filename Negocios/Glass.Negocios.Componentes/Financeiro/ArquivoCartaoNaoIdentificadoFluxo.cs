using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Financeiro.Negocios.Entidades;
using Colosoft;
using System.IO;
using Glass.Data.DAL;
using Glass.Data.Helper;
using GDA;
using Colosoft.Business;

namespace Glass.Financeiro.Negocios.Componentes
{
    public class ArquivoCartaoNaoIdentificadoFluxo : IArquivoCartaoNaoIdentificadoFluxo, IProvedorArquivoCartaoNaoIdentificado
    {
        /// <summary>
        /// Recupera informações dos arquivos de CNI
        /// </summary>
        public IList<ArquivoCartaoNaoIdentificadoPesquisa> PesquisarArquivosCartaoNaoIdentificado(Data.Model.SituacaoArquivoCartaoNaoIdentificado? situacao,
            DateTime? dataImportIni, DateTime? dataImportFim, string funcCad)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ArquivoCartaoNaoIdentificado>("acni")
                .LeftJoin<Data.Model.Funcionario>("acni.Usucad=f.IdFunc", "f")
                .Select(@"acni.IdArquivoCartaoNaoIdentificado, acni.DataCad, 
                            acni.Situacao, f.Nome AS NomeFuncCad");

            if (situacao.GetValueOrDefault() > 0)
                consulta.WhereClause
                    .And("acni.Situacao=?situacao")
                    .Add("?situacao", situacao)
                    .AddDescription("Situação: " + situacao.Translate());

            if (dataImportIni != null)
                consulta.WhereClause
                    .And("acni.DataCad>=?dataImportIni")
                    .Add("?dataImportIni", dataImportIni)
                    .AddDescription("Data da importação inicial: " + dataImportIni);

            if (dataImportFim != null)
                consulta.WhereClause
                    .And("acni.DataCad<=?dataImportFim")
                    .Add("?dataImportFim", dataImportFim.Value.AddDays(1).AddSeconds(-1))
                    .AddDescription("Data da importação final: " + dataImportFim.Value.AddDays(1).AddSeconds(-1));

            if (!funcCad.IsNullOrEmpty())
                consulta.WhereClause
                    .And("f.Nome LIKE ?funcCad")
                    .Add("?funcCad", string.Format("%{0}%", funcCad))
                    .AddDescription("Funcionário cadastro:" + funcCad);

            return consulta.ToVirtualResultLazy<ArquivoCartaoNaoIdentificadoPesquisa>();
        }

        /// <summary>
        /// Recupera o Arquivo de Cartão não identificado com base no Id
        /// </summary>
        public ArquivoCartaoNaoIdentificado ObterArquivoCartaoNaoIdentificado(int idArquivoCartaoNaoIdentificado)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ArquivoCartaoNaoIdentificado>()
                .Where("IdArquivoCartaoNaoIdentificado=?idArquivoCartaoNaoIdentificado")
                .Add("?idArquivoCartaoNaoIdentificado", idArquivoCartaoNaoIdentificado)
                .ProcessLazyResult<ArquivoCartaoNaoIdentificado>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        public Colosoft.Business.SaveResult SalvarArquivoCartaoNaoIdentificado(ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado)
        {
            arquivoCartaoNaoIdentificado.Require("arquivoCartaoNaoIdentificado").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult retorno;

                if (!(retorno = arquivoCartaoNaoIdentificado.Save(session)))
                    return retorno;

                retorno = session.Execute(false).ToSaveResult();

                return retorno;
            }
        }

        /// <summary>
        /// Cancela o arquivo passado
        /// </summary>
        public Colosoft.Business.SaveResult CancelarArquivoCartaoNaoIdentificado(ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado, string motivo)
        {
            if (!arquivoCartaoNaoIdentificado.PodeCancelar)
                return new Colosoft.Business.SaveResult(false, "O arquivo não pode ser cancelado.".GetFormatter());

            var idsCartaoNaoIdentificado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>()
                .Select("IdCartaoNaoIdentificado")
                .Where("IdArquivoCartaoNaoIdentificado=?idArquivoCartaoNaoIdentificado")
                .Add("?idArquivoCartaoNaoIdentificado", arquivoCartaoNaoIdentificado.IdArquivoCartaoNaoIdentificado)
                .Execute().Select(f => (uint)f.GetInt32(0))
                .ToList();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    CartaoNaoIdentificadoDAO.Instance.Cancelar(transaction, idsCartaoNaoIdentificado.ToArray(), motivo);
                    LogCancelamentoDAO.Instance.LogArquivoCartaoNaoIdentificado(transaction, arquivoCartaoNaoIdentificado.DataModel, motivo, true);
                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    return new Colosoft.Business.SaveResult(false, ex.Message.GetFormatter());
                }
            }

            arquivoCartaoNaoIdentificado.Situacao = Data.Model.SituacaoArquivoCartaoNaoIdentificado.Cancelado;

            return SalvarArquivoCartaoNaoIdentificado(arquivoCartaoNaoIdentificado);
        }

        /// <summary>
        /// Recupera se o arquivo pode ser cancelado baseado nos cartões associados a ele
        /// </summary>
        bool IProvedorArquivoCartaoNaoIdentificado.PodeCancelarArquivo(int idArquivoCartaoNaoIdentificado)
        {
            return !SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>()
                .Where("IdArquivoCartaoNaoIdentificado=?idArquivoCartaoNaoIdentificado AND Situacao > 1")
                .Add("?idArquivoCartaoNaoIdentificado", idArquivoCartaoNaoIdentificado)
                .ExistsResult();
        }

        /// <summary>
        /// Salva o Arquivo
        /// </summary>
        public Colosoft.Business.SaveResult SalvarArquivo(ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult retorno;

                if (!(retorno = arquivoCartaoNaoIdentificado.Save(session)))
                    return retorno;

                retorno = session.Execute(false).ToSaveResult();

                return retorno;
            }
        }

        /// <summary>
        /// Apaga o arquivo
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarArquivo(ArquivoCartaoNaoIdentificado arquivoCartaoNaoIdentificado)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.DeleteResult retorno;

                if (!(retorno = arquivoCartaoNaoIdentificado.Delete(session)))
                    return retorno;

                retorno = session.Execute(false).ToDeleteResult();

                return retorno;
            }
        }

        /// <summary>
        /// Insere um novo arquivo não identificado
        /// </summary>
        private Colosoft.Business.SaveResult InserirNovoArquivo(Stream stream, string extensao)
        {
            var novoArquivo = SourceContext.Instance.Create<ArquivoCartaoNaoIdentificado>();
            novoArquivo.Situacao = Data.Model.SituacaoArquivoCartaoNaoIdentificado.Ativo;

            var resultado = SalvarArquivo(novoArquivo);

            if (!resultado)
                return new ImportarArquivoCartaoNaoIdentificadoResultado(resultado.Message);

            var caminho = string.Format(@"{0}\{1}{2}", Utils.GetArquivosCNIPath, novoArquivo.IdArquivoCartaoNaoIdentificado, extensao);

            try
            {
                using (var fileStream = File.Create(caminho))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                ApagarArquivo(novoArquivo);
                return new ImportarArquivoCartaoNaoIdentificadoResultado(ex.Message.ToString().GetFormatter());
            }

            return new Colosoft.Business.SaveResult(true, novoArquivo.IdArquivoCartaoNaoIdentificado.ToString().GetFormatter());
        }

        /// <summary>
        /// Recupera o tipo cartão
        /// </summary>
        private int ObterTipoCartao(string nomeCartao, string bandeira)
        {
            var tiposCartaoCredito = TipoCartaoCreditoDAO.Instance.ObtemListaPorTipo(0);

            if (tiposCartaoCredito == null)
                return 0;

            var tipoCartao = tiposCartaoCredito.FirstOrDefault(f => bandeira.Contains(f.Bandeira.Translate().ToString()) && nomeCartao.Contains(f.Tipo.Translate().ToString()));

            if (tipoCartao == null)
                return 0;

            return (int)tipoCartao.IdTipoCartao;
        }

        /// <summary>
        /// Importa o arquivo contendo os CNIs
        /// </summary>
        public ImportarArquivoCartaoNaoIdentificadoResultado Importar(Stream stream, string extensao)
        {
            var streamAberto = new MemoryStream();
            stream.CopyTo(streamAberto);

            var imp = new LayoutCNI.Importador(stream, extensao);

            string msgErro;

            if (!imp.Valido(out msgErro))
                return new ImportarArquivoCartaoNaoIdentificadoResultado(msgErro.GetFormatter());

            var resultadoImportacaoArquivo = InserirNovoArquivo(streamAberto, extensao);
            if (!resultadoImportacaoArquivo)
                return new ImportarArquivoCartaoNaoIdentificadoResultado(resultadoImportacaoArquivo.Message);

            var cartoes = imp.Importar(resultadoImportacaoArquivo.Message.ToString().StrParaInt());

            return new ImportarArquivoCartaoNaoIdentificadoResultado(cartoes);
        }
    }
}
