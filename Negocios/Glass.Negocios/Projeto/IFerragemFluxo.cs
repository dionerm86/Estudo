using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios
{
    public interface IFerragemFluxo
    {
        #region Ferragem

        /// <summary>
        /// Cria uma nova Instancia de Ferragem
        /// </summary>
        /// <returns></returns>
        Entidades.Ferragem CriarFerragem();

        /// <summary>
        /// Pesquisa as Ferragens
        /// </summary>
        IList<Entidades.FerragemPesquisa> PesquisarFerragem(string nomeFerragem, int idFabricanteFerragem, string codigo);

        /// <summary>
        /// Recupera os dados de Ferragem.
        /// </summary>
        Entidades.Ferragem ObterFerragem(int idFerragem);

        /// <summary>
        /// Recupera os dados de Ferragem.
        /// </summary>
        Entidades.Ferragem ObterFerragem(string nomeFerragem);

        /// <summary>
        /// Altera a situação da Ferragem
        /// </summary>
        /// <param name="ferragem"></param>
        Colosoft.Business.SaveResult AtivarInativarFerragem(Entidades.Ferragem ferragem);

        /// <summary>
        /// Salva os dados da ferragem.
        /// </summary>
        Colosoft.Business.SaveResult SalvarFerragem(Entidades.Ferragem ferragem);

        /// <summary>
        /// Apaga os dados da Ferragem.
        /// </summary>
        Colosoft.Business.DeleteResult ApagarFerragem(Entidades.Ferragem ferragem);

        #endregion

        #region Fabricante Ferragem

        /// <summary>
        /// Pesquisa os Fabricantes de Ferragem
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="sitio"></param>
        /// <returns></returns>
        IList<Entidades.FabricanteFerragemPesquisa> PesquisarFabricanteFerragem(string nome, string sitio);

        /// <summary>
        /// Recupera os dados do Fabricante de Ferragem.
        /// </summary>
        /// <param name="idFabricanteFerragem"></param>
        Entidades.FabricanteFerragem ObterFabricanteFerragem(int idFabricanteFerragem);

        /// <summary>
        /// Recupera os descritores dos fabricantes de ferragem.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObterFabricantesFerragem();

        /// <summary>
        /// Salva os dados do Fabricante de Ferragem
        /// </summary>
        Colosoft.Business.SaveResult SalvarFabricanteFerragem(Entidades.FabricanteFerragem fabricanteFerragem);

        /// <summary>
        /// Apaga os dados da medida do projeto.
        /// </summary>
        /// <param name="fabricanteFerragem"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFabricanteFerragem(Entidades.FabricanteFerragem fabricanteFerragem);

        #endregion
    }
}
