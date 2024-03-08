using Microsoft.AspNetCore.Mvc;
using Simchafund.Web.Models;
using SimchaFund.Data;

namespace Simchafund.Web.Controllers
{
    public class SimchaController : Controller
    {
        private readonly DbManager _manager = new(@"Data Source=.\sqlexpress; Initial Catalog=WebDevPractice; Integrated Security=true;");

        public IActionResult Index()
        {
            ViewBag.Message = TempData["message"] != null ? (string)TempData["message"] : null;
            return View(new SimchaViewModel
            {
                Simchas = _manager.GetSimchas(),
                TotalContributorCount = _manager.GetTotalContributorCount()
            }); 
        }

        [HttpPost]
        public IActionResult Add(Simcha simcha)
        {
            if (!string.IsNullOrEmpty(simcha.Name) && simcha.Date != default)
            {
                _manager.AddSimcha(simcha);
                TempData["message"] = $"New simcha: {simcha.Name} added successfully!";
            }
            return Redirect("/");
        }

        public IActionResult Contributions(int simchaId)
        {
            var vm = new SimchaContributionsViewModel
            {
                Simcha = _manager.GetSimcha(simchaId),
                Contributors = _manager.GetContributors()
            };
            vm.Contributors.ForEach(c =>
            {
                c.Contribute = _manager.ContributedForSimcha(c.Id, simchaId);
                if (c.Contribute)
                {
                    c.Amount = _manager.GetAmountContributedForSimcha(c.Id, simchaId);
                }
            });
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateContributions(int simchaId, List<Contributor> contributors)
        {
            contributors.RemoveAll(c => !c.Contribute);
            _manager.UpdateContributionsForSimcha(simchaId, contributors);
            TempData["message"] = "Contributions successfully updated!";
            return Redirect("/");
        }
    }
}
