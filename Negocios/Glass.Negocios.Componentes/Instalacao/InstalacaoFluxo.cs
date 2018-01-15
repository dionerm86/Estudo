using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Instalacao.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio da instalação.
    /// </summary>
    public class InstalacaoFluxo : IInstalacaoFluxo
    {
        #region Fixacao Vidro

        /// <summary>
        /// Pesquisa as fixações do vidro.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.FixacaoVidro> PesquisarFixacoesVidro()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FixacaoVidro>()
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.FixacaoVidro>();
        }

        /// <summary>
        /// Recupera as fixações dos vidros.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFixacoesVidro()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FixacaoVidro>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.FixacaoVidro>()
                .ToList();
        }

        /// <summary>
        /// Recupera a fixacao do vidro.
        /// </summary>
        /// <param name="idFixacaoVidro"></param>
        /// <returns></returns>
        public Entidades.FixacaoVidro ObtemFixacaoVidro(int idFixacaoVidro)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FixacaoVidro>()
                .Where("IdFixacaoVidro=?id")
                .Add("?id", idFixacaoVidro)
                .ProcessResult<Entidades.FixacaoVidro>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a fixação do vidro.
        /// </summary>
        /// <param name="fixacaoVidro"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFixacaoVidro(Entidades.FixacaoVidro fixacaoVidro)
        {
            fixacaoVidro.Require("fixacaoVidro").NotNull();

            if (fixacaoVidro.IdFixacaoVidro > 0)
                fixacaoVidro.DataModel.ExistsInStorage = true;
            
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = fixacaoVidro.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a fixação do vidro.
        /// </summary>
        /// <param name="idFixacaoVidro"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFixacaoVidro(int idFixacaoVidro)
        {
            var fixacaoVidro = ObtemFixacaoVidro(idFixacaoVidro);

            if (fixacaoVidro == null)
                return new Colosoft.Business.DeleteResult(false, "Fixação de vidro não encotrada.".GetFormatter());

            return ApagarFixacaoVidro(fixacaoVidro);
        }

        /// <summary>
        /// Apaga a fixação do vidro.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFixacaoVidro(Entidades.FixacaoVidro fixacaoVidro)
        {
            fixacaoVidro.Require("fixacaoVidro").NotNull();

            if (fixacaoVidro.IdFixacaoVidro > 0 && !fixacaoVidro.ExistsInStorage)
                fixacaoVidro = ObtemFixacaoVidro(fixacaoVidro.IdFixacaoVidro);

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = fixacaoVidro.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
        
        #endregion
    }
}
