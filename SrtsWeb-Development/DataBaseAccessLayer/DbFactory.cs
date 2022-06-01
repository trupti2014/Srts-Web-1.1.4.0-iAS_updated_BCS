using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtsWeb.DataBaseAccessLayer
{
    public static class DbFactory
    {
        public static IDatabaseAccessLayer GetDbObject(DataBaseType DbType)
        {
            switch (DbType)
            {
                case DataBaseType.SQL:
                    return new SqlDataBaseAccessLayer();
                case DataBaseType.Excel:
                    return new ExcelDataBaseAccessLayer();
                default:
                    return new SqlDataBaseAccessLayer();
            }
        }

        public static IDatabaseAccessLayer GetDbObject(DataBaseType DbType, String ConfigConnectionStringName)
        {
            IDatabaseAccessLayer dbA = null;
            switch (DbType)
            {
                case DataBaseType.SQL:
                    dbA = new SqlDataBaseAccessLayer(ConfigConnectionStringName);
                    break;
                case DataBaseType.Excel:
                    dbA = new ExcelDataBaseAccessLayer(ConfigConnectionStringName);
                    break;
            }
            return dbA;
        }
    }
}
