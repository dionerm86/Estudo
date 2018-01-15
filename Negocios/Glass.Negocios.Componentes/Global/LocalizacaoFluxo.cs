using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de localização.
    /// </summary>
    public class LocalizacaoFluxo : ILocalizacaoFluxo
    {
        #region Cidade

        /// <summary>
        /// Recupera os descritores da cidades do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCidades()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cidade>()
                .OrderBy("NomeCidade")
                .ProcessResultDescriptor<Entidades.Cidade>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da cidade.
        /// </summary>
        /// <param name="idCidade"></param>
        /// <returns></returns>
        public Entidades.Cidade ObtemCidade(int idCidade)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cidade>()
                .Where("IdCidade=?id")
                .Add("?id", idCidade)
                .ProcessLazyResult<Entidades.Cidade>()
                .FirstOrDefault();
        }

        #endregion

        #region Uf

        /// <summary>
        /// Recupera as unidades federativas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.IEntityDescriptor> ObtemUfs()
        {
            yield return new EntityDescriptor(1, "AC");
            yield return new EntityDescriptor(2, "AL");
            yield return new EntityDescriptor(3, "AM");
            yield return new EntityDescriptor(4, "AP");
            yield return new EntityDescriptor(5, "BA");
            yield return new EntityDescriptor(6, "CE");
            yield return new EntityDescriptor(7, "DF");
            yield return new EntityDescriptor(8, "ES");
            yield return new EntityDescriptor(9, "EX");
            yield return new EntityDescriptor(10, "GO");
            yield return new EntityDescriptor(11, "MA");
            yield return new EntityDescriptor(12, "MG");
            yield return new EntityDescriptor(13, "MS");
            yield return new EntityDescriptor(14, "MT");
            yield return new EntityDescriptor(15, "PA");
            yield return new EntityDescriptor(16, "PB");
            yield return new EntityDescriptor(17, "PE");
            yield return new EntityDescriptor(18, "PI");
            yield return new EntityDescriptor(19, "PR");
            yield return new EntityDescriptor(20, "RJ");
            yield return new EntityDescriptor(21, "RN");
            yield return new EntityDescriptor(22, "RO");
            yield return new EntityDescriptor(23, "RR");
            yield return new EntityDescriptor(24, "RS");
            yield return new EntityDescriptor(25, "SC");
            yield return new EntityDescriptor(26, "SE");
            yield return new EntityDescriptor(27, "SP");
            yield return new EntityDescriptor(28, "TO");
        }

        #endregion

        #region Pais

        /// <summary>
        /// Recupera os descritores dos paises.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemPaises()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Pais>()
                .OrderBy("NomePais")
                .ProcessResultDescriptor<Entidades.Pais>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do país.
        /// </summary>
        /// <param name="idPais"></param>
        /// <returns></returns>
        public Entidades.Pais ObtemPais(int idPais)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Pais>()
                .Where("IdPais=?id")
                .Add("?id", idPais)
                .ProcessLazyResult<Entidades.Pais>()
                .FirstOrDefault();
        }

        #endregion
    }
}
