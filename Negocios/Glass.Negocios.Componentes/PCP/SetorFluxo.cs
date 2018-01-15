using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos setores do sistema.
    /// </summary>
    public class SetorFluxo : ISetorFluxo,
        Entidades.IProvedorSetor
    {
        #region Setor

        /// <summary>
        /// Pequisa os setores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Setor> PesquisarSetores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .OrderBy("NumeroSequencia")
                .ToVirtualResultLazy<Entidades.Setor>();
        }

        /// <summary>
        /// Recupera os descritores dos setores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.SetorDescritor> ObtemSetores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .OrderBy("NumeroSequencia")
                .Select("IdSetor, Descricao, Tipo")
                .ProcessResultDescriptor<Entidades.Setor>()
                .Select(f => (Entidades.SetorDescritor)f)
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public Entidades.Setor ObtemSetor(int idSetor)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.Data.Model.Setor>()
               .Where("IdSetor=?id")
               .Add("?id", idSetor)
               .ProcessLazyResult<Entidades.Setor>()
               .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarSetor(Entidades.Setor setor)
        {
            setor.Require("setor").NotNull();

            if (!setor.ExistsInStorage)
            {
                var numeroSequencia = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Setor>()
                    .Select("MAX(NumeroSequencia)")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .FirstOrDefault() + 1;

                // Gera um novo número de sequência para este novo setor
                setor.NumeroSequencia = numeroSequencia;
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = setor.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarSetor(Entidades.Setor setor)
        {
            setor.Require("setor").NotNull();

            // Verifica se este alguma peça foi inserida neste setor
            if (SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.LeituraProducao>()
                .Where("IdSetor=?id")
                .Add("?id", setor.IdSetor)
                .ExistsResult())
                return new Colosoft.Business.DeleteResult(false,
                    "Este Setor não pode ser excluído por haver peças relacionadas ao mesmo.".GetFormatter());


            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = setor.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Método usado para alterar a posição do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="acima"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicao(int idSetor, bool acima)
        {
            var setor = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Where("IdSetor=?id").Add("?id", idSetor)
                .ProcessLazyResult<Entidades.Setor>()
                .FirstOrDefault();

            var maiorSequencia = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Select("MAX(NumeroSequencia)")
                .Execute()
                .Select(f => f.GetInt32(0)).FirstOrDefault();


            // Só troca de posição se houver algum setor abaixo/acima deste para ser trocado, 
            // lembrando que a posição Impr. Etiqueta não pode ser trocada
            if (setor.NumeroSequencia == 1 || (acima && setor.NumeroSequencia == 2) ||
                (!acima && maiorSequencia == setor.NumeroSequencia))
                return new Colosoft.Business.SaveResult(true, null);


            var destino = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Where("NumeroSequencia" + (acima ? "<" : ">") + "?num")
                .Add("?num", setor.NumeroSequencia)
                .OrderBy("NumeroSequencia " + (acima ? "Desc" : "Asc"))
                .Take(1)
                .ProcessLazyResult<Entidades.Setor>()
                .FirstOrDefault();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var numSeqNovo = destino.NumeroSequencia;

                // Altera a posição do setor adjacente à este
                destino.NumeroSequencia = setor.NumeroSequencia;

                // Altera a posição deste setor
                setor.NumeroSequencia = numSeqNovo;

                var resultado = setor.Save(session);
                if (!resultado)
                    return resultado;

                resultado = destino.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();

                // Recarrega listagem de setores
                //Utils.GetSetores = SetorDAO.Instance.GetOrdered();
            }
        }

        #endregion

        #region IProvedorSetor

        /// <summary>
        /// Valida a atualização dos dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IProvedorSetor.ValidaAtualizacao(Entidades.Setor setor)
        {
            if (setor.ExistsInStorage)
            {
                if(setor.Situacao == Situacao.Inativo)
                {
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.FuncionarioSetor>("fs")
                        .Where("fs.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("fs.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à funcionário(s) de produção. " +
                                "Para inativá-lo, desassocie o setor do funcionário.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 51830. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.ProdutoPedidoProducao>("ppp")
                        .Where("ppp.IdSetor=?id AND ppp.Situacao=?situacaoProdutoProducao")
                            .Add("?id", setor.IdSetor)
                            .Add("?situacaoProdutoProducao", (int)Glass.Data.Model.ProdutoPedidoProducao.SituacaoEnum.Producao)
                        .GroupBy("ppp.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à etiqueta(s) de produção. " +
                                "Para inativá-lo, retire a(s) etiqueta(s) deste setor.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 56684. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.RoteiroProducaoSetor>("rps")
                        .Where("rps.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("rps.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à roteiro(s) de produção. " +
                                "Para inativá-lo, retire este setor do(s) roteiro(s) de produção.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 56684. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.RoteiroProducaoEtiqueta>("rpe")
                        .Where("rpe.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("rpe.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, existem etiquetas configuradas com este setor em seu roteiro de produção.", setor.Descricao)).GetFormatter()
                        };
                }
            }

            return new IMessageFormattable[0];
        }

        #endregion
    }
}
