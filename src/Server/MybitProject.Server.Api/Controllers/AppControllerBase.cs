﻿namespace MybitProject.Server.Api.Controllers;

public partial class AppControllerBase : ControllerBase
{
    [AutoInject] protected ServerApiSettings AppSettings = default!;

    [AutoInject] protected ServerApiSettings Settings = default!;

    [AutoInject] protected AppDbContext DbContext = default!;

    [AutoInject] protected IStringLocalizer<AppStrings> Localizer = default!;
}
