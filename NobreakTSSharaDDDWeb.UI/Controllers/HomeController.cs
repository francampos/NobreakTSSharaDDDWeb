using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NobreakTSSharaDDDWeb.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = User.Identity.GetUserId();
            ViewBag.Message = "Nascida em 1990, da iniciativa empreendedora de três jovens engenheiros, a TS SHARA, fabricante de nobreaks, estabilizadores de tensão, filtros de linha, autotransformadores e protetores de rede, no segmento de baixa e média potência é hoje uma das maiores e mais produtivas empresas no mercado brasileiro de equipamentos de proteção e energia, além de exportadora para mais de 15 países.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contato";
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}