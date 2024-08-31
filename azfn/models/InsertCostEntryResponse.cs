using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azfn
{
    [Serializable]
    public class InsertCostEntryResponse
    {
        public InsertCostEntryResponse() => Exceptions = new List<string>();
        public int RecordCount { get; set; }
        public List<string> Exceptions { get; set; }
    }
}
