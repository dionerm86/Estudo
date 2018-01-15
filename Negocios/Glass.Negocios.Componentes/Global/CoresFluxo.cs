using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de cores.
    /// </summary>
    public class CoresFluxo : ICoresFluxo
    {
        #region Cor Vidro

        /// <summary>
        /// Pesquisa as cores dos vidros do sistema.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        public IList<Entidades.CorVidro> PesquisarCoresVidro()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorVidro>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.CorVidro>();
        }

        /// <summary>
        /// Recupera as cores de vidro.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCoresVidro()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorVidro>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.CorVidro>()
                .ToList();
        }

        /// <summary>
        /// Recupera a cor do vidro.
        /// </summary>
        /// <param name="idCorVidro"></param>
        /// <returns></returns>
        public Entidades.CorVidro ObtemCorVidro(int idCorVidro)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorVidro>()
                .Where("IdCorVidro=?idCorVidro")
                .Add("?idCorVidro", idCorVidro)
                .ProcessLazyResult<Entidades.CorVidro>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do cor do vidro.
        /// </summary>
        /// <param name="corVidro"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCorVidro(Entidades.CorVidro corVidro)
        {
            corVidro.Require("corVidro").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corVidro.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a cor do vidro.
        /// </summary>
        /// <param name="corVidro">Cor do vidro.</param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCorVidro(Entidades.CorVidro corVidro)
        {
            corVidro.Require("corVidro").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corVidro.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Cor Ferragem

        /// <summary>
        /// Pesquisa as cores de ferragem.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.CorFerragem> PesquisarCoresFerragem()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorFerragem>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.CorFerragem>();
        }

        /// <summary>
        /// Recupera as cores de ferragem.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCoresFerragem()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorFerragem>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.CorFerragem>()
                .ToList();
        }

        /// <summary>
        /// Recupera a cor da ferragem.
        /// </summary>
        /// <param name="idCorFerragem">Identificador da cor.</param>
        /// <returns></returns>
        public Entidades.CorFerragem ObtemCorFerragem(int idCorFerragem)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorFerragem>()
                .Where("IdCorFerragem=?idCorFerragem")
                .Add("?idCorFerragem", idCorFerragem)
                .ProcessLazyResult<Entidades.CorFerragem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a cor da ferragem.
        /// </summary>
        /// <param name="corFerragem"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCorFerragem(Entidades.CorFerragem corFerragem)
        {
            corFerragem.Require("corFerragem").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corFerragem.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a cor da ferragem.
        /// </summary>
        /// <param name="corFerragem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCorFerragem(Entidades.CorFerragem corFerragem)
        {
            corFerragem.Require("corFerragem").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corFerragem.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Cor Aluminio

        /// <summary>
        /// Pesquisa as cores de aluminio.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.CorAluminio> PesquisarCoresAluminio()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorAluminio>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.CorAluminio>();
        }

        /// <summary>
        /// Recupera as cores de alumínio.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCoresAluminio()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorAluminio>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.CorAluminio>()
                .ToList();
        }

        /// <summary>
        /// Recupera as cores de alumínio.
        /// </summary>
        /// <param name="idCorAluminio">Identificador da cor.</param>
        /// <returns></returns>
        public Entidades.CorAluminio ObtemCorAluminio(int idCorAluminio)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CorAluminio>()
                .Where("IdCorAluminio=?idCorAluminio")
                .Add("?idCorAluminio", idCorAluminio)
                .ProcessLazyResult<Entidades.CorAluminio>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a cor da alumínio.
        /// </summary>
        /// <param name="corAluminio"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCorAluminio(Entidades.CorAluminio corAluminio)
        {
            corAluminio.Require("corAluminio").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corAluminio.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a cor da aluminio.
        /// </summary>
        /// <param name="corAluminio"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCorAluminio(Entidades.CorAluminio corAluminio)
        {
            corAluminio.Require("corAluminio").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = corAluminio.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion
    }
}
