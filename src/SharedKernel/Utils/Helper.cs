using RT.Comb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Helper
    {
        public static Guid GetNewCombSequentialID()
        {
            Guid result;
            var comb = new SqlCombProvider(new UnixDateTimeStrategy(), new UtcNoRepeatTimestampProvider().GetTimestamp);
            result = comb.Create();
            return result;
        }
    }
}
