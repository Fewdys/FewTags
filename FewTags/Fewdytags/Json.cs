using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FewTags
{
    internal class Json
    {

        [Serializable]
        public class Tags
        {
            public List<Tag> records { get; set; }
        }

        public class Tag
        {
            public int id { get; set; }
            public string UserID { get; set; }
            public string PlateText { get; set; }
            public string PlateText2 { get; set; }
            public string PlateText3 { get; set; }
            public string PlateBigText { get; set; }
            public bool Malicious { get; set; }
            public bool Active { get; set; }
            public bool TextActive { get; set; }
            public bool Text2Active { get; set; }
            public bool Text3Active { get; set; }
            public bool BigTextActive { get; set; }
            public string Size { get; set; }
        }
    }
}
