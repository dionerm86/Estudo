using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Rentabilidade
{
    public partial class VisualizacaoItemRentabilidade : System.Web.UI.Page
    {
        #region Propriedades

        /// <summary>
        /// Tipo do item que será visualizado.
        /// </summary>
        public string Tipo => Request["tipo"];

        /// <summary>
        /// Identificador do item.
        /// </summary>
        public int IdItem
        {
            get
            {
                int id = 0;
                if (int.TryParse(Request["id"], out id))
                    return id;

                return 0;
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera o item da rentabilidade.
        /// </summary>
        /// <returns></returns>
        private Glass.Rentabilidade.IItemRentabilidade ObterItem()
        {
            var serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;

            switch ((Tipo ?? "").ToLower())
            {
                case "pedido": 
                    return serviceLocator
                        .GetInstance<Glass.Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.Pedido>>()
                        .ObterItem(IdItem);

                case "orcamento":
                    return serviceLocator
                        .GetInstance<Glass.Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.Orcamento>>()
                        .ObterItem(IdItem);

                case "pedidoespelho":
                    return serviceLocator
                        .GetInstance<Glass.Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.PedidoEspelho>>()
                        .ObterItem(IdItem);

                case "notafiscal":
                    return serviceLocator
                       .GetInstance<Glass.Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.NotaFiscal>>()
                       .ObterItem(IdItem);
            }

            return null;
        }

        #endregion

        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Recupera os dados para visualização.
        /// </summary>
        /// <returns></returns>
        protected string ObterDadosVisualizacao()
        {
            var item = ObterItem();

            if (item != null)
            {
                var provedorDescritores = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Rentabilidade.IProvedorDescritorRegistroRentabilidade>();

                var dataSource = new Glass.Rentabilidade.Relatorios.ItemRentabilidade
                    .ItemRentabilidadeDataSource(provedorDescritores, item, Glass.Globalizacao.Cultura.CulturaSistema);

                var dados = dataSource.ToJson();

                return Newtonsoft.Json.JsonConvert.SerializeObject(dados);
            }

            return null;
        }

        #endregion
    }
}