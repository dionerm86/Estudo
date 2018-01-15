using System.Drawing;
using Glass.Data.DAL;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class ItemCarregamento
    {
        #region Variaveis Locais

        private Glass.Data.Model.ItemCarregamento _model;

        #endregion

        #region Construtores

        public ItemCarregamento()
            : this(new Glass.Data.Model.ItemCarregamento())
        {

        }

        internal ItemCarregamento(Glass.Data.Model.ItemCarregamento model)
        {
            _model = model;
        }

        #endregion

        #region Propiedades

        public uint IdItemCarregamento
        {
            get { return _model.IdItemCarregamento; }
        }

        public uint IdOc
        {
            get { return _model.IdOc; }
        }

        public uint IdPedido
        {
            get { return _model.IdPedido; }
        }

        public uint IdProd
        {
            get { return _model.IdProd.GetValueOrDefault(); }
        }

        public uint IdCliente
        {
            get { return _model.IdCliente; }
        }

        public string NomeCLiente
        {
            get { return _model.NomeCliente; }
        }

        public string IdNomeCliente 
        {
            get { return _model.IdNomeCliente; }
        }

        public string CodDescProduto
        {
            get { return _model.CodProduto + " - " + _model.DescProduto; }
        }

        public double Altura
        {
            get { return _model.Altura; }
        }

        public double Largura
        {
            get { return _model.Largura; }
        }

        public double M2
        {
            get { return _model.M2; }
        }

        public double Peso
        {
            get { return _model.Peso; }
        }

        public string Etiqueta
        {
            get { return _model.Etiqueta; }
        }

        public string PedidoEtiqueta
        {
            get { return _model.PedidoEtiqueta; }
        }

        public Color CorLinha
        {
            get
            {
                if (_model.Carregado)
                    return Color.Green;
                else
                    return Color.Red;
            }
        }

        public bool Carregado
        {
            get { return _model.Carregado; }
        }

        public string DataLeitura
        {
            get
            {
                if (!Carregado)
                    return "";

                return _model.DataLeitura.ToString();
            }
        }

        public string NomeFuncLeitura
        {
            get
            {
                if (!Carregado)
                    return "";

                return FuncionarioDAO.Instance.GetNome(_model.IdFuncLeitura);
            }
        }

        public bool LogEstornoVisible
        {
            get
            {
                return _model.LogEstornoVisible;
            }
        }

        public string ImagemPecaUrl
        {
            get { return _model.ImagemPecaUrl; }
        }

        public string SetoresPendentes
        {
            get { return _model.SetoresPendentes; }
        }

        #endregion
    }
}
