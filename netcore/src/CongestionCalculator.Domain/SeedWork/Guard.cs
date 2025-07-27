namespace CongestionCalculator.Domain.SeedWork;

public static class Guard
{
    public static class For
    {
        public static void NotNullOrEmpty(string parameterName, string value)
        {
            if (value == null)
                throw new DomainException(
                    $"{parameterName} cannot be null or empty",
                    new ArgumentNullException(parameterName)
                );
        }
    }
}
