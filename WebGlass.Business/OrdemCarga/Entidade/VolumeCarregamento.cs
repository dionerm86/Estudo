using System;
using System.Drawing;
using Glass.Data.DAL;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class VolumeCarregamento
    {
        #region Variaveis Locais

        private Glass.Data.Model.ItemCarregamento _model;

        #endregion

        #region Construtores

        public VolumeCarregamento()
            :this(new Glass.Data.Model.ItemCarregamento())
        {

        }

        internal VolumeCarregamento(Glass.Data.Model.ItemCarregamento model)
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

        public uint IdVolume
        {
            get { return _model.IdVolume.GetValueOrDefault(); }
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

        public DateTime? DataFechamento 
        { 
            get { return _model.DataFechamento.GetValueOrDefault(); } 
        }

        public double Peso 
        {
            get { return _model.Peso; }
        }

        public string Etiqueta 
        {
            get
            {
                if (_model.Carregado)
                    return "V" + IdVolume.ToString("D9");
                else
                    return string.Empty;
            }
        }

        public string PedidoEtiqueta
        {
            get { return _model.IdPedido + " (" + "V" + IdVolume.ToString("D9") + ")"; }
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


        #endregion
    }
}
