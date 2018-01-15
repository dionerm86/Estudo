using System;
using Glass.Data.Model;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for CtrlFoto
/// </summary>
namespace Glass.UI.Web
{
    public class CtrlFoto : BaseUserControl
    {
        private IFoto _foto;
    
        public IFoto Foto
        {
            get { return _foto; }
            set 
            { 
                _foto = value;
                _path = _foto.FilePath;
                _virtualPath = _foto.VirtualFilePath;
            }
        }
    
        private Unit _larguraTabela;
    
        public Unit LarguraTabela
        {
            get { return _larguraTabela; }
            set { _larguraTabela = value; }
        }
    
        private string _path;
    
        public string Path
        {
            get { return Arquivos.IsImagem(GetExtensao()) ? _path : GetPathByExtensao(); }
            set { _path = value; }
        }
    
        private string _virtualPath;
    
        public string VirtualPath
        {
            get { return _virtualPath; }
            set { _virtualPath = value; }
        }

        public bool EditVisible { get; set; }
    
        public CtrlFoto()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    
        protected string GetExtensao()
        {
            return !String.IsNullOrEmpty(_path) && _path.LastIndexOf('.') > -1 ? _path.Substring(_path.LastIndexOf('.')).ToLower() : "";
        }

        private string GetPathByExtensao()
        {
            if (Arquivos.IsImagem(GetExtensao()))
                return String.Empty;
    
            string path = "";
    
            switch (GetExtensao())
            {
                case ".pdf": 
                    path = "../Images/PDF_grande.jpg";
                    break;
    
                case ".xls":
                case ".xlsx":
                    path = "../Images/Excel_grande.jpg";
                    break;
    
                case ".doc":
                case ".docx":
                    path = "../Images/Word_grande.jpg";
                    break;
    
                default: return String.Empty;
            }
    
            return Server.MapPath(path);
        }
    }
}
