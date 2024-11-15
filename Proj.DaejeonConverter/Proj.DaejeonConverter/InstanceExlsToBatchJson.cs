using Assimp;
using Assimp.CCd;
using CCd.AssimpNet.CCd;
using CCd.GIS.ModelBatch;
using CCd.IOs;
using CCd.Maths;
using CCd.WS.Core.Mesh;
using CCd.WS.Core.Utils;
using System.Data;



namespace Proj.DaejeonConverter
{
    internal class InstanceExlsToBatchJson
    {
        CCdElevateExtractor _elevateExtractor;

        public void setDemFiles(string[] demFilePathArray )
        {
            if (demFilePathArray == null || demFilePathArray.Length < 1)
                return;

            foreach (string filePath in demFilePathArray)
            {
                if (File.Exists(filePath) == false)
                    return;
            }

            if (_elevateExtractor == null)
            {
                _elevateExtractor = new CCdElevateExtractor();
                _elevateExtractor.loadRasterDem(demFilePathArray);
            }
        }


        public bool convert( string srcModelRoot, string xlsFilePath, string outputDir)
        {
            if (File.Exists(xlsFilePath) == false)
            {
                Console.WriteLine($"{xlsFilePath}  이 없음.");
                return false;
            }

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

                    var originalModelFileName = row[All4Def.TransformTableField.FileName].ToString();
                    // 새로이 저장될 모델 파일명.
                    batchItem.FName = $"{batchItem.Id}.obj";

                    double posX = (double)row[All4Def.TransformTableField.X];
                    double posY = (double)row[All4Def.TransformTableField.Y];
                    double posZ = 0;
                    // 241024.중간에 포맷에서 사라짐.
                    // 241115.다시생김.
                    posZ = (double)row[All4Def.TransformTableField.Z];

                    if (_elevateExtractor != null && _elevateExtractor.getElevation(posX, posY, out float terrainHeight))
                        posZ = terrainHeight;
                    batchItem.Alt = posZ;

                    double rotationAngle = (double)row[All4Def.TransformTableField.AngleZ];

                    // 얼만큼 내려갈지.
                    double downDepth = 0;
                    // 241115.사라짐.
                    //downDepth = (double)row[All4Def.TransformTableField.Depth];
                    batchItem.Alt -= downDepth;

                    var srcModelFilePath = Path.Combine(srcModelRoot, originalModelFileName );
                    var outModelFilePath = Path.Combine(outputDir, batchItem.FName );

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
