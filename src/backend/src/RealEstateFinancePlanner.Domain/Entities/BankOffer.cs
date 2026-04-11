namespace RealEstateFinancePlanner.Domain.Entities;

public class BankOffer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BankName { get; set; } = string.Empty;
    public decimal BaseTinPercentage { get; set; }
    public List<Bonus> Bonuses { get; set; } = [];
    public List<int> MortgageTermsYears { get; set; } = [15, 20, 25, 30];
}
