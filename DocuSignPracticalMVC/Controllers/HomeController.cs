using DocuSignPracticalMVC.Models;
using DocuSignPracticalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace DocuSignPracticalMVC.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("sign"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    public IActionResult Sign(string signEmail, string signName)
    {
        if(signEmail == null || signName == null)
        {
            return RedirectToAction("Index", "Home");
        }
        SendSignRequest signRequest = new SendSignRequest();
        signRequest.DigitalSignature(signEmail, signName);
        return RedirectToAction("Index", "Home");
    }
}
