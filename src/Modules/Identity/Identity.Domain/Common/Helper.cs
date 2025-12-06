using RT.Comb;

namespace Identity.Domain.Common
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
