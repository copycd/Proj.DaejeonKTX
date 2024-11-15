
namespace Proj.DaejeonConverter
{
    internal class All4Def
    {
        public class TransformTableField
        {
            public const int FileName = 0;
            public const int ObjectCode = 1;
            public const int X = 2;
            public const int Y = 3;
            // 중간에 포맷이 변경되서, 사라짐.
            // 다시 생김.
            public const int Z = 4;
            public const int AngleX = 5;
            public const int AngleY = 6;
            public const int AngleZ = 7;
            // 241115. 사라짐.
            //public const int Depth = 7;     // 맨홀, 벨브냐에 따라서 필드이름은 틀리나 index는 같음.
        }
    }
}
