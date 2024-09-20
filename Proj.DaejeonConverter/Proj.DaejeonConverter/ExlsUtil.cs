using ExcelDataReader;
using System.Data;
using System.Text;

namespace Proj.DaejeonConverter
{
    internal class ExlsUtil
    {
        public static DataTable? ReadExcelFile(string xlsFilePath)
        {
            if (File.Exists(xlsFilePath) == false)
                return null;

            using (var stream = File.Open(xlsFilePath, FileMode.Open, FileAccess.Read))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding("UTF-8");
                using (var reader = ExcelReaderFactory.CreateReader(stream,
                  new ExcelReaderConfiguration() { FallbackEncoding = encoding }))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                    });

                    if (result.Tables.Count > 0)
                    {
                        return result.Tables[0];
                    }
                }
            }
            return null;
        }
    }
}
