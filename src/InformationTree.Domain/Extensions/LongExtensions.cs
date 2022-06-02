namespace InformationTree.Domain.Extensions
{
    public static class LongExtensions
    {
        public static int ToInt(this long value)
        {
            // Use modulo operator to ensure the value can be passed to an int
            if (value > int.MaxValue)
                value %= int.MaxValue;
            if (value < int.MinValue)
                value %= int.MinValue;
            
            var intValue = (int)value;
            
            return intValue;
        }
    }
}