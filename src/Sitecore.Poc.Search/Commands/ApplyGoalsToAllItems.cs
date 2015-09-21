using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Analytics.Configuration;
using Sitecore.Buckets.Commands;
using Sitecore.Buckets.Util;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XamlSharp.Continuations;

namespace Sitecore.Poc.Search.Commands
{
    public class ApplyGoalsToAllItems : Command, ISupportsContinuation, IItemBucketsCommand
    {
        public override void Execute(CommandContext context)
        {
            if (!AnalyticsSettings.Enabled)
            {
                SheerResponse.Alert(Translate.Text("You need to enable Analytics to use this functionality"));
            }
            else
            {
                Assert.ArgumentNotNull((object)context, "context");
                if (context.Items.Length != 1)
                    return;
                if (!SecurityHelper.CanRunApplication("Content Editor/Ribbons/Chunks/Analytics - Attributes/Attributes"))
                {
                    SheerResponse.Alert("You don't have sufficient rights for this operation");
                }
                else
                {
                    NameValueCollection parameters = new NameValueCollection();
                    parameters["items"] = this.SerializeItems(context.Items);
                    parameters["fieldid"] = context.Parameters["fieldid"];
                    parameters["user"] = Context.User.Name;
                    parameters["searchString"] = context.Parameters.GetValues("url")[0].Replace("\"", string.Empty);
                    ClientPipelineArgs args = new ClientPipelineArgs(parameters);
                    if (ContinuationManager.Current != null)
                        ContinuationManager.Current.Start((ISupportsContinuation)this, "Run", args);
                    else
                        Context.ClientPage.Start((object)this, "Run", args);
                }
            }
        }

        protected virtual string GetUrl()
        {
            return "/sitecore/shell/~/xaml/Sitecore.Shell.Applications.Analytics.TrackingField.Goals.aspx";
        }

        [UsedImplicitly]
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Item obj = this.DeserializeItems(args.Parameters["items"])[0];
            if (!SheerResponse.CheckModified())
                return;
            string str = args.Parameters["user"];
            string index = args.Parameters["fieldid"];
            if (string.IsNullOrEmpty(index))
                index = Sitecore.Buckets.Util.Constants.PersonalisationField;
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;
                List<SearchStringModel> searchQuery = UIFilterHelpers.ExtractSearchQuery(args.Parameters["searchString"]);
                ProgressBox.Execute(Translate.Text("Applying Goals Change"), Translate.Text("Applying Goals"), "~/icon/Applications/32x32/signpost.png", new ProgressBoxMethod(this.StartProcess), (object)obj, (object)searchQuery, (object)args.Result, (object)index, (object)str);
                SheerResponse.Alert(Translate.Text("Finished Applying Goals"));
                if (AjaxScriptManager.Current != null)
                    AjaxScriptManager.Current.Dispatch("analytics:trackingchanged");
                else
                    Context.ClientPage.SendMessage((object)this, "analytics:trackingchanged");
            }
            else if (obj.Appearance.ReadOnly)
                SheerResponse.Alert(Translate.Text("You cannot edit the '{0}' item because it is protected.", (object)obj.DisplayName));
            else if (!obj.Access.CanWrite())
            {
                SheerResponse.Alert(Translate.Text("You cannot edit the '{0}' because you do not have write access to it.", (object)obj.DisplayName));
            }
            else
            {
                UrlString urlString = new UrlString(this.GetUrl());
                UrlHandle urlHandle = new UrlHandle();
                urlHandle["tracking"] = obj[index];
                urlHandle.Add(urlString);
                this.ShowDialog(urlString.ToString());
                args.WaitForPostBack();
            }
        }

        protected virtual void ShowDialog(string url)
        {
            Assert.ArgumentNotNull((object)url, "url");
            SheerResponse.ShowModalDialog(url, true);
        }

        private void StartProcess(params object[] parameters)
        {
            string str = (string)parameters[2];
            string index = (string)parameters[3];
            SitecoreIndexableItem sitecoreIndexableItem = (SitecoreIndexableItem)((Item)parameters[0]);
            if (sitecoreIndexableItem == null)
            {
                Log.Error("Apply Personalization - Unable to cast current item - " + parameters[0].GetType().FullName, (object)this);
            }
            else
            {
                string accountName = (string)parameters[4];
                List<SearchStringModel> list = (List<SearchStringModel>)parameters[1];
                using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex((IIndexable)sitecoreIndexableItem).CreateSearchContext(SearchSecurityOptions.EnableSecurityCheck))
                {
                    IQueryable<SitecoreUISearchResultItem> query = LinqHelper.CreateQuery<SitecoreUISearchResultItem>(searchContext, (IEnumerable<SearchStringModel>)list, (Item)sitecoreIndexableItem, (IEnumerable<IExecutionContext>)null);
                    Assert.IsNotNull((object)sitecoreIndexableItem, "item");
                    Account account = Account.FromName(accountName, AccountType.User);
                    foreach (SitecoreUISearchResultItem searchResultItem in (IEnumerable<SitecoreUISearchResultItem>)query)
                    {
                        Item obj = searchResultItem.GetItem();
                        using (new SecurityEnabler())
                        {
                            if (obj != null)
                            {
                                if (obj.Security.CanWrite(account))
                                {
                                    using (new UserSwitcher((User)account))
                                    {
                                        obj.Editing.BeginEdit();
                                        obj[index] = str;
                                        obj.Editing.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
