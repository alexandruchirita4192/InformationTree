namespace InformationTree.Domain.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal ValidatePercentage(this decimal decimalValue)
        {
            if (decimalValue < 0)
                decimalValue = 0;
            else if (decimalValue > 100)
                decimalValue = 100;
            return decimalValue;
        }
    }
}