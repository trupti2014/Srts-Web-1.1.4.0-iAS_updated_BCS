using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Office.Interop.Excel;
using SrtsWeb.DataLayer.Repositories;
using SrtsWeb.BusinessLayer.Abstract;

namespace SrtsWeb.BusinessLayer.Concrete
{
    public class ExcelExporter : IExcelExporter
    {
        //Application xlApp;
        //Workbook wkBook;
        //Worksheet wkSheet;

        public ExcelExporter()
        {
            //this.xlApp = new Microsoft.Office.Interop.Excel.Application();
        }

        public void CreateWorkBookObject()
        {
            //this.wkBook = this.xlApp.Workbooks.Add();
        }
        public void CreateWorkSheetObject()
        {
            //this.wkSheet = this.wkBook.Worksheets.get_Item(1) as Worksheet;
        }

        public void CreateFile(List<String> columnNames)
        {
            //CreateWorkBookObject();
            //CreateWorkSheetObject();
            //for (var i = 0; i < columnNames.Count; i++)
            //{
            //    this.wkSheet.Cells[1, i + 1] = columnNames[i];
            //    ((Range)this.wkSheet.Cells[1, i + 1]).Font.Bold = true;
            //}

            //this.xlApp.Visible = true;
        }

        public void Dispose()
        {
            try
            {
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(this.wkSheet);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(this.wkBook);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(this.xlApp);
                //this.wkSheet = null;
                //this.wkBook = null;
                //this.xlApp = null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                //this.wkSheet = null;
                //this.wkBook = null;
                //this.xlApp = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
