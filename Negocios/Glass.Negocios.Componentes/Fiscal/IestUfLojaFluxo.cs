using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes
{
    public class IestUfLojaFluxo : IIestUfLojaFluxo
    {
        /// <summary>
        /// Recupera a lista de IEST cadastrado por loja
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<Entidades.IestUfLoja> PesquisarIest(uint idLoja)
        {
            var consultaIestUfLoja = SourceContext.Instance.CreateQuery()
                .From<Data.Model.IestUfLoja>("iul");

            if (idLoja > 0)
                consultaIestUfLoja.WhereClause
                    .And("iul.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja);

            var resultado = consultaIestUfLoja.ToVirtualResultLazy<Entidades.IestUfLoja>();

            return resultado;
        }

        /// <summary>
        /// Recupera o IEST pelo Id.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public Entidades.IestUfLoja ObtemIestUfLoja(uint idIestUfLoja)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.IestUfLoja>()
                .Where("IdIestUfLoja=?idIestUfLoja")
                .Add("?idIestUfLoja", idIestUfLoja)
                .ProcessResult<Entidades.IestUfLoja>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera o IEST pela loja e nomeUF
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="nomeUf"></param>
        /// <returns></returns>
        public Entidades.IestUfLoja ObtemIestUfLoja(uint idLoja, string nomeUf)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.IestUfLoja>()
                .Where("IdLoja=?idLoja && NomeUf=?nomeUf")
                .Add("?idLoja", idLoja)
                .Add("?nomeUf", nomeUf)
                .ProcessResult<Entidades.IestUfLoja>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do IEST
        /// </summary>
        /// <param name="iestUfLoja"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarIestUfLoja(Entidades.IestUfLoja iestUfLoja)
        {
            iestUfLoja.Require("iestUfLoja").NotNull();

            // Se não for informado o IEST, não permite alterar ou inserir
            if (!iestUfLoja.ExistsInStorage && (iestUfLoja.InscEstSt == null || iestUfLoja.InscEstSt == ""))
                return new Colosoft.Business.SaveResult(false, "Não foi informado o IEST para o estado.".GetFormatter());

            // Se já existir um IEST para a mesma loja e estado, não permite alterar ou inserir
            if (!iestUfLoja.ExistsInStorage && SourceContext.Instance.CreateQuery()
                .From<Data.Model.IestUfLoja>()
                .Where("IdLoja=?idLoja && NomeUf=?nomeUf")
                .Add("?idLoja", iestUfLoja.IdLoja)
                .Add("?nomeUf", iestUfLoja.NomeUf)
                .ExistsResult())
                return new Colosoft.Business.SaveResult(false, "Já existe um IEST cadastrado para esse estado.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = iestUfLoja.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do IEST
        /// </summary>
        /// <param name="iestUfLoja"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarIestUfLoja(Entidades.IestUfLoja iestUfLoja)
        {
            iestUfLoja.Require("iestUfLoja").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = iestUfLoja.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
    }
}
