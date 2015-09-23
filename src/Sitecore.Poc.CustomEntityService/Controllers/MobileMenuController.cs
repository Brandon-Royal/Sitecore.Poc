using Sitecore.Poc.CustomEntityService.Entities;
using Sitecore.Poc.CustomEntityService.Repositories;
using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Sitecore.Services;

namespace Sitecore.Poc.CustomEntityService.Controllers
{
    [ServicesController("menu/mobile")]
    public class MobileMenuController : EntityService<MenuItem>
    {
        public MobileMenuController(IRepository<MenuItem> repository) : base(repository)
        { }

        public MobileMenuController() : this(new MenuRespository<Mobile>())
        { }
    }
}
