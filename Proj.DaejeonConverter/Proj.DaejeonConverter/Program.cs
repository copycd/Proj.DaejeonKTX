// See https://aka.ms/new-console-template for more information
using Proj.DaejeonConverter;

Console.WriteLine("Hello, World!");
Console.WriteLine("Version is 2411.24");

// 지형높이값을 계산할 필요가 있을때, 지형데이터경로를 설정함.
var demFileList = new[] { "" };

var srcDataRoot = "E:\\대전역사작업\\AbstractSubterraneanPipe";
var outputRoot = "E:\\대전역사작업\\AbstractSubterraneanPipe_Output";

var modelFolderName = "ster_lod4Geometry";
var xlsFolderName = "ster_lod4ImplicitRepresentation";

List<DataItemInfo> dataNameTableList = new List<DataItemInfo>();
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Communication\\ster_Manhole\\ster_CommunicationManhole", xlsFileName = "UFL_KMAN_PS.xlsx" });
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Electricity\\ster_Manhole\\ster_ElectricityManhole", xlsFileName = "UFL_BMAN_PS.xlsx" });
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Gas\\ster_Valve\\ster_GasValve", xlsFileName = "UFL_GVAL_PS.xlsx" });
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Sewage\\ster_Manhole\\ster_SewageManhole", xlsFileName = "SWL_MANH_PS.xlsx" });
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Water\\ster_Manhole\\ster_WaterManhole\\", xlsFileName = "WTL_MANH_PS.xlsx" });
dataNameTableList.Add(new DataItemInfo { subPath = "ster_Water\\ster_Valve\\ster_WaterValve\\", xlsFileName = "WTL_VALV_PS.xlsx" });


foreach (var dataNameTable in dataNameTableList)
{
    var modelFileRoot = Path.Combine(srcDataRoot, dataNameTable.subPath, modelFolderName );
    var xlsFilePath = Path.Combine(srcDataRoot, dataNameTable.subPath, xlsFolderName, dataNameTable.xlsFileName );

    string outputDir = Path.Combine(outputRoot, dataNameTable.subPath );
    InstanceExlsToBatchJson exlsToJson = new InstanceExlsToBatchJson();
    // 지형높이값을 계산할 필요가 있을때.
    if(demFileList != null && demFileList.Length > 0 )
        exlsToJson.setDemFiles(demFileList);

    exlsToJson.convert(modelFileRoot, xlsFilePath, outputDir);
}

Console.WriteLine("************   Finish!   ****************");