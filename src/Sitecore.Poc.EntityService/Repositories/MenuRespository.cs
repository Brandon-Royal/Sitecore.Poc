using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Poc.CustomEntityService.Entities;
using Sitecore.Resources.Media;
using Sitecore.Services.Core;

namespace Sitecore.Poc.CustomEntityService.Repositories
{
    public class MenuRespository<T> : IRepository<MenuItem> where T: Channel
    {
        public IQueryable<MenuItem> GetAll()
        {
            var root = Db.GetItem(MenuRootItemId);
            if (root == null) return Enumerable.Empty<MenuItem>().AsQueryable();
            var items = root.Children;
            return items.Select(item => new MenuItem
            {
                Name = item["Name"],
                Description = item["Description"],
                Id = item.ID.ToString(),
                ProductCode = item["ProductCode"],
                ImageUrl = GetMediaUrl(item)
            })
            .AsQueryable();
        }

        public MenuItem FindById(string id)
        {
            var item = Db.GetItem(MainUtil.GetID(id));
            return new MenuItem
            {
                Name = item["Name"],
                Description = item["Description"],
                Id = item.ID.ToString(),
                ProductCode = item["ProductCode"],
                ImageUrl = GetMediaUrl(item)
            };
        }

        public void Add(MenuItem entity)
        {
            throw new NotImplementedException();
        }

        public bool Exists(MenuItem entity)
        {
            throw new NotImplementedException();
        }

        public void Update(MenuItem entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(MenuItem entity)
        {
            throw new NotImplementedException();
        }

        public string GetMediaUrl(Item item)
        {
            ImageField imageField = item.Fields["Image"];
            if (imageField == null || imageField.MediaItem == null) return null;
            var image = new MediaItem(imageField.MediaItem);
            var url = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image, GetMediaUrlOptions()));
            return HashingUtils.ProtectAssetUrl(url);
        }

        private MediaUrlOptions GetMediaUrlOptions()
        {
            if (typeof (T) == typeof (Kiosk))
            {
                return new MediaUrlOptions { AllowStretch = true, Width = 800 };
            }
            if (typeof (T) == typeof (Mobile))
            {
                return new MediaUrlOptions { AllowStretch = true, Width = 260 };
            }
            return new MediaUrlOptions();
        }

        private Database Db => Sitecore.Context.Database;

        private ID MenuRootItemId => MainUtil.GetID("{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}");
    }
}
