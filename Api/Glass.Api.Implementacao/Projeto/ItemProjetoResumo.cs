using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    public class ItemProjetoResumo : Glass.Api.Projeto.IITemProjetoResumo
    {
        public int IdItemProjeto { get; }

        public int IdProjetoModelo { get; }

        public string Valor { get; }

        public string ImageUrl { get; }

        public string Ambiente { get; }

        public string ValorCobradoM2 { get; }

        public string DescricaoProduto { get; }

        public string CodigoModelo { get; }

        public int IdCorVidro { get; }

        public int EspessuraVidro { get; }

        public ItemProjetoResumo(Glass.Data.Model.ItemProjeto itemProjeto)
        {
            IdItemProjeto = (int)itemProjeto.IdItemProjeto;
            IdProjetoModelo = (int)itemProjeto.IdProjetoModelo;
            Valor = itemProjeto.Total.ToString("c", new CultureInfo("pt-BR"));
            ImageUrl = itemProjeto.ImagemUrlMini.Replace("~/", ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().EnderecoServicoImagem);
            CodigoModelo = itemProjeto.CodigoModelo;
            IdCorVidro = (int)itemProjeto.IdCorVidro;
            EspessuraVidro = itemProjeto.EspessuraVidro;

            if (!ImageUrl.StartsWith(ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().EnderecoServicoImagem))
                ImageUrl = string.Format("{0}{1}", ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().EnderecoServicoImagem, ImageUrl);

            Ambiente = itemProjeto.Ambiente;

            var materiais = Glass.Data.DAL.MaterialItemProjetoDAO.Instance.GetList(itemProjeto.IdItemProjeto, string.Empty, 0, 1);
            if (materiais.FirstOrDefault() != null)
            {
                ValorCobradoM2 = materiais.FirstOrDefault().Valor.ToString("c", new CultureInfo("pt-BR"));
                DescricaoProduto = materiais.FirstOrDefault().DescrProduto;
            }
        }
    }
}
