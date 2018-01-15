using System;
using System.Collections.Generic;
using Colosoft.Business;
using Glass.PCP.Negocios.Entidades;
using Colosoft;
using System.Linq;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos cavaletes do sistema.
    /// </summary>
    public class CavaleteFluxo : ICavaleteFluxo
    {
        /// <summary>
        /// Apaga os dados do cavalete
        /// </summary>
        /// <param name="cavalete"></param>
        /// <returns></returns>
        public DeleteResult ApagarCavalete(Cavalete cavalete)
        {
            cavalete.Require("cavalete").NotNull();

            // Verifica se alguma peça foi vinculada neste cavalete
            if (SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.LeituraProducao>()
                .Where("IdCavalete=?id")
                .Add("?id", cavalete.IdCavalete)
                .ExistsResult())
                return new DeleteResult(false,
                    "Este Cavalete não pode ser excluído por haver peças relacionadas ao mesmo.".GetFormatter());


            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = cavalete.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Recupera os dados do cavalete.
        /// </summary>
        /// <param name="idCavalete"></param>
        /// <returns></returns>
        public Cavalete ObterCavalete(int idCavalete)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.Data.Model.Cavalete>()
               .Where("IdCavalete=?id")
               .Add("?id", idCavalete)
               .ProcessLazyResult<Entidades.Cavalete>()
               .FirstOrDefault();
        }

        /// <summary>
        /// Pequisa os cavaletes do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Cavalete> PesquisarCavaletes()
        {
            return SourceContext.Instance.CreateQuery()
                 .From<Glass.Data.Model.Cavalete>()
                 .OrderBy("CodInterno")
                 .ToVirtualResultLazy<Entidades.Cavalete>();
        }

        /// <summary>
        /// Salva os dados do cavalete
        /// </summary>
        /// <param name="cavalete"></param>
        /// <returns></returns>
        public SaveResult SalvarCavalete(Cavalete cavalete)
        {
            cavalete.Require("cavalete").NotNull();

            // Verifica se algum cavelate com o mesmo cod interno
            var consulta = SourceContext.Instance.CreateQuery()
                 .From<Glass.Data.Model.Cavalete>()
                 .Where("CodInterno = ?cod")
                .Add("?cod", cavalete.CodInterno);

            if(cavalete.IdCavalete > 0)
                consulta.WhereClause.And("IdCavalete <> ?id")
                    .Add("?id", cavalete.IdCavalete);

            if (consulta.ExistsResult())
                return new SaveResult(false, ("Já existe um cavalete cadastrado com o cód.: " + cavalete.CodInterno).GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = cavalete.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }
    }
}
