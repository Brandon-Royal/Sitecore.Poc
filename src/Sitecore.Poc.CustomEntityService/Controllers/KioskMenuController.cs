using System.Web.Http;
using Sitecore.Poc.CustomEntityService.Entities;
using Sitecore.Poc.CustomEntityService.Repositories;
using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Sitecore.Services;

namespace Sitecore.Poc.CustomEntityService.Controllers
{
    [ServicesController("menu/kiosk")]
    public class KioskMenuController : EntityService<MenuItem>
    {
        public KioskMenuController(IRepository<MenuItem> repository) : base(repository)
        {}

        public KioskMenuController() : this(new MenuRespository<Kiosk>())
        {}
    }
}
