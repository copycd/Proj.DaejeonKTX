// See https://aka.ms/new-console-template for more information
using Proj.DaejeonConverter;

Console.WriteLine("Hello, World!");

var dataRoot = "D:\\Data\\daejeon\\all4land\\맨홀밸브전체\\하수맨홀";
var modelRoot = Path.Combine(dataRoot, "대표모델");
var xlsFilePath = Path.Combine(dataRoot, "이동량", "SWL_MANH_PS.xlsx");

string outputDir = "E:\\맨홀";
InstanceExlsToJson exlsToJson = new InstanceExlsToJson();
exlsToJson.convert(modelRoot, xlsFilePath, outputDir);


Console.WriteLine("Finish!");