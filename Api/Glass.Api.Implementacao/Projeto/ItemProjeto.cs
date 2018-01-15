using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa um item projeto.
    /// </summary>
    public class ItemProjeto : Glass.Api.Projeto.IItemProjeto
    {
        public int IdProjeto { get; set; }

        public int IdItemProjeto { get; set; }

        public int IdProjetoModelo { get; set; }

        public int EspessuraVidro { get; set; }

        public int IdCorVidro { get; set; }

        public bool MedidaExata { get; set; }

        public string Total { get; set; }

        public string Ambiente { get; set; }

        public string ImagemUrl { get; set; }

        public ItemProjeto()
        {

        }

        public ItemProjeto(Glass.Data.Model.ItemProjeto itemProjeto)
        {
            IdProjeto = (int)itemProjeto.IdProjeto;
            IdItemProjeto = (int)itemProjeto.IdItemProjeto;
            IdProjetoModelo = (int)itemProjeto.IdProjetoModelo;
            EspessuraVidro = itemProjeto.EspessuraVidro;
            IdCorVidro = (int)itemProjeto.IdCorVidro;
            MedidaExata = itemProjeto.MedidaExata;
            Ambiente = itemProjeto.Ambiente;
            ImagemUrl = itemProjeto.ImagemUrl.Replace("../../", ServiceLocator.Current.GetInstance<Glass.Api.IConfiguracao>().EnderecoServicoImagem);
            Total = itemProjeto.Total.ToString("c", new CultureInfo("pt-BR"));
        }
    }

}
