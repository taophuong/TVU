using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TVUStartup.Web.Data;
using TVUStartup.Web.Models.Entities;
using TVUStartup.Web.Models.ViewModels;

namespace TVUStartup.Web.Controllers;

/// <summary>Liên hệ chung với Ban quản trị website (không gắn với dự án cụ thể)</summary>
public class ContactController : Controller
{
    private readonly AppDbContext _db;

    public ContactController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("lien-he")]
    public IActionResult Index() => View(new ContactFormModel
    {
        FullName = User.Identity?.Name ?? "",
        Email = User.FindFirstValue(ClaimTypes.Email) ?? ""
    });

    [HttpPost("lien-he")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormModel model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.ContactRequests.Add(new ContactRequest
        {
            ProjectId = null, // liên hệ chung
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone,
            Organization = model.Organization,
            RequestType = model.RequestType,
            Message = model.Message.Trim(),
            Status = ContactStatus.Moi,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Cảm ơn bạn! Ban quản trị sẽ phản hồi qua email trong thời gian sớm nhất.";
        return RedirectToAction(nameof(Index));
    }
}
