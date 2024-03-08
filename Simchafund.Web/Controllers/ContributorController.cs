using Microsoft.AspNetCore.Mvc;
using Simchafund.Web.Models;
using SimchaFund.Data;

namespace Simchafund.Web.Controllers
{
    public class ContributorController : Controller
    {
        private readonly DbManager _manager = new(@"Data Source=.\sqlexpress; Initial Catalog=WebDevPractice; Integrated Security=true;");

        public IActionResult Index()
        {
            return View(new ContributorsViewModel
            {
                Contributors = _manager.GetContributors(),
                Total = _manager.GetTotalBalance()
            });
        }

        [HttpPost]
        public IActionResult Add(Contributor contributor)
        {
            if (ValidContrInfo(contributor) && contributor.Amount != default)
            {
                _manager.AddContributor(contributor);
                _manager.AddDeposit(new() { ContributorId = contributor.Id, Amount = contributor.Amount, Date = DateTime.Now });
            }
            return Redirect("/Contributor");
        }

        [HttpPost]
        public IActionResult Update(Contributor contributor)
        {
            if (ValidContrInfo(contributor))
            {
                _manager.UpdateContributor(contributor);
            }
            return Redirect("/contributor");
        }

        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            if (deposit.Amount != default && deposit.Date != default)
            {
                _manager.AddDeposit(deposit);
            }
            return Redirect("/contributor");
        }

        public IActionResult History(int id)
        {
            return View(new HistoryViewModel
            {
                Contributor = _manager.GetContributor(id),
                Actions = GetContributorActions(id)
            }); ;
        }

        private List<ContributorAction> GetContributorActions(int id)
        {
            var actions = new List<ContributorAction>();
            var contributions = _manager.GetContributions(id);
            var deposits = _manager.GetDeposits(id);

            actions.AddRange(contributions.Select(c => new ContributorAction
            {
                Action = $"Contribution for the {_manager.GetSimcha(c.SimchaId).Name} simcha",
                Amount = c.Amount,
                Date = c.Date
            }));
            actions.AddRange(deposits.Select(d => new ContributorAction
            {
                Action = "Deposit",
                Amount = -(d.Amount),
                Date = d.Date
            }));

            return actions.OrderBy(a => a.Date).ToList();
        }

        private static bool ValidContrInfo(Contributor contributor) =>
            !string.IsNullOrEmpty(contributor.FirstName) &&
            !string.IsNullOrEmpty(contributor.LastName) &&
            !string.IsNullOrEmpty(contributor.CellNumber);
    }
}
