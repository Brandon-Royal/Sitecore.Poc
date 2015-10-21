using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Analytics;

namespace Sitecore.Poc.MockGeoIp
{
    public partial class GeoIp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var tracker = Tracker.Current;
            if (tracker == null)
            {
                GeoIpLiteral.Text = "No Tracker Found";
                return;
            }
            var interaction = Tracker.Current.Interaction;
            if (interaction != null)
            {
                var geoData = interaction.GeoData;
                if (geoData != null)
                {
                    GeoIpLiteral.Text = string.Format(
                        "<p>Business Name: {0}</p><p>City: {1}</p><p>Region: {2}</p><p>Postal Code: {3}</p><p>Country: {4}</p><p>DNS: {5}</p><p>ISP: {6}</p><p>Coordinates: {7},{8}</p><p>Url: {9}</p>",
                        geoData.BusinessName, geoData.City, geoData.Region, geoData.PostalCode, geoData.Country, geoData.Dns, geoData.Isp, geoData.Latitude, geoData.Longitude, geoData.Url);
                }
                else
                {
                    GeoIpLiteral.Text = "No GeoData Found";
                }
            }
            else
            {
                GeoIpLiteral.Text = "No Interaction Data Found";
            }
        }
    }
}