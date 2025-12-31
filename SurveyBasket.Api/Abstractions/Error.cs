namespace SurveyBasket.Api.Abstractions
{
    public record Error(string Code,string description)
    {
        public static readonly Error None = new Error(string.Empty,string.Empty);
    }

}
