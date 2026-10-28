using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVUStartup.Web.Helpers;
using TVUStartup.Web.Models.Entities;

namespace TVUStartup.Web.Areas.Admin.Controllers;

/// <summary>Lớp cha của mọi controller quản trị: thuộc Area "Admin" và chỉ vai trò Admin được truy cập</summary>
[Area("Admin")]
[Authorize(Roles = Role.Admin)]
[AutoValidateAntiforgeryToken] // mọi request POST trong khu quản trị đều phải có token chống CSRF
public abstract class AdminBaseController : Controller
{
    protected int CurrentId => User.GetAccountId()!.Value;

    protected void Success(string message) => TempData["Success"] = message;
    protected void Error(string message) => TempData["Error"] = message;
}
