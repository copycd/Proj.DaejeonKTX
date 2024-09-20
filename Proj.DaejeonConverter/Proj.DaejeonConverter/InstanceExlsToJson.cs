using Assimp;
using Assimp.CCd;
using CCd.AssimpNet.CCd;
using CCd.Core.Three;
using CCd.GIS.ModelBatch;
using CCd.IOs;
using CCd.Maths;
using CCd.Nets.Rpc;
using CCd.WS.Core.D3Tiles.Batch;
using CCd.WS.Core.Mesh;
using ExcelDataReader;
using Polly;
using System;
using System.Data;
using System.IO.Compression;
using System.Text;
using static CCd.Maths.CCdAngle;


namespace Proj.DaejeonConverter
{
    internal class InstanceExlsToJson
    {
        public bool convert( string srcModelRoot, string xlsFilePath, string outputDir)
        {
            if (File.Exists(xlsFilePath) == false)
                return false;

            var dataTable = ExlsUtil.ReadExcelFile(xlsFilePath);
            if (dataTable != null)
            {
                List<ModelBatchItem> collectBatchItems = new List<ModelBatchItem>();
                Console.WriteLine($"전체 갯수: {dataTable.Rows.Count}");
                foreach (DataRow row in dataTable.Rows)
                {
                    var batchItem = new ModelBatchItem();
                    Console.WriteLine($"{row[All4Def.TransformTableField.FileName]}, {row[2]}, {row[3]} ");
                    batchItem.Id = row[All4Def.TransformTableField.ObjectCode].ToString();
                    // ID를 모델파일 이름으로 할건데 파일이름으로 할수 없는것들이 있으면 교체.
                    batchItem.Id = batchItem.Id.Replace(":", "_");

                    batchItem.FName = row[All4Def.TransformTableField.FileName].ToString();

                    double posX = (double)row[All4Def.TransformTableField.X];
                    double posY = (double)row[All4Def.TransformTableField.Y];
                    double posZ = (double)row[All4Def.TransformTableField.Z];
                    batchItem.Alt = posZ;

                    double rotationAngle = (double)row[All4Def.TransformTableField.AngleZ];

                    var srcModelFilePath = Path.Combine(srcModelRoot, batchItem.FName);
                    var outModelFilePath = Path.Combine(outputDir, $"{batchItem.Id}.obj");

                    using (AssimpContext importer = new AssimpContext())
                    {
                        Scene scene = importer.ImportFile(srcModelFilePath);

                        var rotation = Matrix4x4.FromRotationZ(CCdAngle.deg2rad(rotationAngle) );
                        SceneUtil.rotateScene(scene, rotation);
                        SceneUtil.moveVertices(scene, new Assimp.Vector3D(posX, posY, posZ));

                        PostProcessSteps processSteps = PostProcessSteps.GenerateNormals | PostProcessSteps.Triangulate;
                        var e = ModelConverter.getModelExportFormatDescription(ModelType.obj);
                        DirectoryUtils.confirm(Path.GetDirectoryName(outModelFilePath));

                        SceneMeshUtils.saveScene(importer, scene, outModelFilePath, ModelTypeUtil.getTypeFromExtention(srcModelFilePath), false);
                        // 원본과 다른 폴더이면 texture도 출력해줘야함.
                        ExportUtil.exportTextures(scene, srcModelFilePath, outModelFilePath);
                    }

                    collectBatchItems.Add(batchItem);
                }

                if (collectBatchItems.Count > 0)
                {
                    var outFilePath = Path.Combine(outputDir, "index.json");

                    ModelBatchHeader header = new ModelBatchHeader();
                    header.modelBatchGlobeTransOption = ModelBatchGlobeTransOption.defaultCenterPosition;

                    ModelBatchWriter.write(header, collectBatchItems, outFilePath);
                    return true;
                }
            }
            return false;
        }
    }
}
