namespace PreciosGamer.ExchangeRates.API.Persistence.Entities;

public class ExchangeRateEntity
{   
    // this could be an enum but for simplicity i will leave it as it is
    public string BaseCurrencyCode { get; set; } = string.Empty;
    // why a string and not a number/enum? In case in the future we have to add Dolar Blue a.k.a black market exchange rate
    public string TargetCurrencyCode { get; set; } = string.Empty;
    public decimal ConversionRate { get; set; }
    public DateOnly Date { get; set; }
}
