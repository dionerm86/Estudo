using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Projeto.Negocios.Entidades;

namespace Glass.Projeto.Negocios.Componentes
{
    public class FlagArqMesaFluxo : IFlagArqMesaFluxo, Entidades.IValidadorFlagArqMesa
    {
        /// <summary>
        /// Busca os flags do sistema
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.FlagArqMesa> PesquisarFlag()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.FlagArqMesa>()
                .Select("IdFlagArqMesa, Descricao, Padrao, TipoArquivo")
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.FlagArqMesa>();
        }

        /// <summary>
        /// Recupera os dados do flag
        /// </summary>
        /// <param name="idFlag"></param>
        /// <returns></returns>
        public Entidades.FlagArqMesa ObtemFlag(int IdFlagArqMesa)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.FlagArqMesa>()
                .Where("IdFlagArqMesa=?idFlag")
                .Add("?idFlag", IdFlagArqMesa)
                .ProcessLazyResult<Entidades.FlagArqMesa>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera um lista de flags
        /// </summary>
        /// <param name="idsFlags"></param>
        /// <returns></returns>
        public IList<Entidades.FlagArqMesa> ObtemFlags(int[] idsFlags)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.FlagArqMesa>()
                .Where(string.Format("IdFlagArqMesa IN {0}", string.Join(",",idsFlags.Select(f=>f.ToString()).ToArray())))
                .ProcessLazyResult<Entidades.FlagArqMesa>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos flags do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFlagsArqMesa()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.FlagArqMesa>()
                .OrderBy("Descricao")
                .Select("IdFlagArqMesa, Descricao")
                .Where("Padrao = 0")
                .ProcessResultDescriptor<Entidades.FlagArqMesa>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos flags do arquivo calcEngine.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObtemFlagsArqMesaArqCalcengine(int? idArquivoMesaCorte)
        {
            if (idArquivoMesaCorte > 0)
            {
                var idArquivoCalcEngine = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ArquivoMesaCorte>()
                    .Where("IdArquivoMesaCorte=?mesaCorte")
                    .Add("?mesaCorte", idArquivoMesaCorte)
                    .Select("IdArquivoCalcEngine")
                    .Execute()
                    .Select(f => f.GetInt32(0)).FirstOrDefault();

                if (idArquivoCalcEngine > 0)
                {
                    return SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.FlagArqMesa>("fam")
                        .OrderBy("Descricao")
                        .Select("IdFlagArqMesa, Descricao")
                        .RightJoin<Data.Model.FlagArqMesaArqCalcEngine>("fam.IdFlagArqMesa=famac.IdFlagArqMesa", "famac")
                        .Where("Padrao = 0 AND famac.IdArquivoCalcEngine=?idArquivoCalcEngine")
                        .Add("?idArquivoCalcEngine", idArquivoCalcEngine)
                        .ProcessResultDescriptor<Entidades.FlagArqMesa>()
                        .ToList();
                }
            }

            return new List<IEntityDescriptor>().ToList();
        }

        /// <summary>
        /// Salva os dados de um flag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFlagArqMesa(Entidades.FlagArqMesa flag)
        {
            flag.Require("flag").NotNull();

            if (string.IsNullOrEmpty(flag.Descricao))
                return new Colosoft.Business.SaveResult(false, "Nenhum descrição foi informada.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = flag.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do flag.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFlagArqMesa(Entidades.FlagArqMesa flag)
        {
            flag.Require("flag").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = flag.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region IValidadorFlagArqMesa Members

        public IMessageFormattable[] ValidaExclusao(FlagArqMesa flagArqMesa)
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
                // Verifica se o flag possui peças relacionadas à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FlagArqMesaPecaProjMod>()
                    .Where("IdFlagArqMesa=?id")
                    .Add("?id", flagArqMesa.IdFlagArqMesa)
                    .Count(),
                    tratarResultado("Esta flag não pode ser excluída por possuir peças de projeto relacionadas à mesma."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}
