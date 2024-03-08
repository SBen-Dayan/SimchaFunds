using SimchaFund.Data;

namespace Simchafund.Web.Models
{
    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<ContributorAction> Actions { get; set; } = new();
    }
}
