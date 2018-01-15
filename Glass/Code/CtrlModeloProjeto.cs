using System.Collections.Generic;
using Glass.Data.Model;

/// <summary>
/// Summary description for CtrlModeloProjeto
/// </summary>
namespace Glass.UI.Web
{
    public class CtrlModeloProjeto : BaseUserControl
    {
        public CtrlModeloProjeto()
        {
    
        }
    
        private ProjetoModelo _modelo;
    
        public ProjetoModelo Modelo
        {
            get { return _modelo; }
            set 
            {
                _modelo = value;
                _path = Data.Helper.Utils.GetModelosProjetoPath + _modelo.NomeFigura;
                _virtualPath = Data.Helper.Utils.GetModelosProjetoVirtualPath + _modelo.NomeFigura;
            }
        }
    
        private string _path;
    
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    
        private string _virtualPath;
    
        public string VirtualPath
        {
            get { return _virtualPath; }
            set { _virtualPath = value; }
        }

        private List<int> _espessuras;
        
        /// <summary>
        /// Espessuras dos vidros.
        /// </summary>
        public List<int> Espessuras
        {
            get { return _espessuras; }
            set { _espessuras = value; }
        }
    
        private CorVidro[] _vidros;
    
        public CorVidro[] Vidros
        {
            get { return _vidros; }
            set { _vidros = value; }
        }
    
        private CorAluminio[] _aluminios;
    
        public CorAluminio[] Aluminios
        {
            get { return _aluminios; }
            set { _aluminios = value; }
        }
    
        private CorFerragem[] _ferragens;
    
        public CorFerragem[] Ferragens
        {
            get { return _ferragens; }
            set { _ferragens = value; }
        }

        private bool _exibirCorAluminioFerragem = true;

        public bool ExibirCorAluminioFerragem
        {
            get { return _exibirCorAluminioFerragem; }
            set { _exibirCorAluminioFerragem = value; }
        }
    }
}
