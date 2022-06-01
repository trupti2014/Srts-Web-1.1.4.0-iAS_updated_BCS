using System;

namespace SrtsWeb.BusinessLayer.Abstract
{
    public interface IExcelExporter : IDisposable
    {
        void CreateFile(global::System.Collections.Generic.List<string> columnNames);

        void CreateWorkBookObject();

        void CreateWorkSheetObject();

        void Dispose();
    }
}