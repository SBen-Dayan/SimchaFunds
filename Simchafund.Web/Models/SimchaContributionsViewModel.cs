using SimchaFund.Data;

namespace Simchafund.Web.Models
{
    public class SimchaContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public List<Contributor> Contributors { get; set; } = new();

    }
}
