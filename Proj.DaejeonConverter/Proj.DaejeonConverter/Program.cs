// See https://aka.ms/new-console-template for more information
using Proj.DaejeonConverter;

Console.WriteLine("Hello, World!");
Console.WriteLine("Version is 2420.24");

// 지형높이값을 계산할 필요가 있을때, 지형데이터경로를 설정함.
var demFileList = new[] { "" };

var outputBaseRoot = "E:\\대전역사작업\\맨홀들";
var dataBaseRoot = "D:\\Data\\daejeon\\all4land\\맨홀밸브전체";

List<string[]> dataNameTableList = new List<string[]>();
dataNameTableList.Add(new[] { "가스밸브", "UFL_GVAL_PS.obj", "UFL_GVAL_PS.xlsx" });
dataNameTableList.Add(new[] { "상수맨홀", "WTL_MANH_PS.obj", "WTL_MANH_PS.xlsx" });
dataNameTableList.Add(new[] { "상수밸브", "WTL_VALV_PS.obj", "WTL_VALV_PS.xlsx" });
dataNameTableList.Add(new[] { "전력맨홀", "UFL_BMAN_PS.obj", "UFL_BMAN_PS.xlsx" });
dataNameTableList.Add(new[] { "통신맨홀", "UFL_KMAN_PS.obj", "UFL_KMAN_PS.xlsx" });
dataNameTableList.Add(new[] { "하수맨홀", "SWL_MANH_PS.obj", "SWL_MANH_PS.xlsx" });

foreach (var dataNameTable in dataNameTableList)
{
    var title = dataNameTable[0];
    var modelFileName = dataNameTable[1];
    var xlsxFileName = dataNameTable[2];

    var dataRoot = Path.Combine(dataBaseRoot, title);

    var modelRoot = Path.Combine(dataRoot, "대표모델");
    var xlsFilePath = Path.Combine(dataRoot, "이동량", xlsxFileName );

    string outputDir = Path.Combine(outputBaseRoot, title);
    InstanceExlsToBatchJson exlsToJson = new InstanceExlsToBatchJson();
    // 지형높이값을 계산할 필요가 있을때.
    if(demFileList != null && demFileList.Length > 0 )
        exlsToJson.setDemFiles(demFileList);

    exlsToJson.convert(modelRoot, xlsFilePath, outputDir);
}

Console.WriteLine("************   Finish!   ****************");