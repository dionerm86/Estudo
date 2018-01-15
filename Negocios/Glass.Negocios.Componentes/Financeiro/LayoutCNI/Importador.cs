using Glass.Financeiro.Negocios.Entidades;
using Colosoft;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI
{
    public interface IArquivoCNI
    {
        //bool ValidaImportacao(out string msgErro);

        List<CartaoNaoIdentificado> Importar(int idArqCni);

        bool LayoutValido(NPOI.SS.UserModel.ISheet sheet);
    }

    public class Importador
    {
        #region Variaveis Locais

        private NPOI.SS.UserModel.ISheet _sheet;
        private IArquivoCNI _arquivoCni;

        #endregion

        #region Construtores

        public Importador(Stream stream, string extensao)
        {
            var streamAberto = new MemoryStream();
            stream.CopyTo(streamAberto);

            stream.Position = 0;

            var hssfwb = new HSSFWorkbook();
            var xssfwb = new XSSFWorkbook();
            var excelAntigo = extensao == ".xls";

            if (excelAntigo)
                hssfwb = new HSSFWorkbook(stream);
            else
                xssfwb = new XSSFWorkbook(stream);

            _sheet = excelAntigo ? hssfwb.GetSheet(hssfwb.GetSheetName(0)) : xssfwb.GetSheet(xssfwb.GetSheetName(0));
        }

        #endregion

        #region Métodos Publicos

        public List<CartaoNaoIdentificado> Importar(int idArqCni)
        {
            var cni = _arquivoCni.Importar(idArqCni);

            return cni;
        }

        public bool Valido(out string msgErro)
        {

            if (new LayoutRede(_sheet).LayoutValido(_sheet))
                _arquivoCni = new LayoutRede(_sheet);
            else if (new LayoutCielo(_sheet).LayoutValido(_sheet))
                _arquivoCni = new LayoutCielo(_sheet);

            if (_arquivoCni == null)
            {
                msgErro = "Nenhum layout cadastrado corresponde a planilha informada.";
                return false;
            }

            msgErro = "";
            return true;
        }

        #endregion
    }
}
