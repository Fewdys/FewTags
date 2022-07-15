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
            public string Text { get; set; }
            public bool Active { get; set; }
        }
    }
}
