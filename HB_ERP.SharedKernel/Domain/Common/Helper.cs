using RT.Comb;

namespace HB_ERP.SharedKernel.Domain.Common
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
