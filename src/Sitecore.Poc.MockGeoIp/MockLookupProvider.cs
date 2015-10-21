using System;
using Sitecore.Analytics.Lookups;
using Sitecore.Analytics.Model;

namespace Sitecore.Poc.MockGeoIp
{
    public class MockLookupProvider : LookupProviderBase
    {
        public override WhoIsInformation GetInformationByIp(string ip)
        {
            return new WhoIsInformation()
            {
                AreaCode = "94965",
                BusinessName = "Sitecore",
                City = "Sausilito",
                Country = "United States",
                Dns = "sitecore.net",
                Isp = "Acme ISP",
                IsUnknown = false,
                Latitude = 37.8653244,
                Longitude = -122.4986624,
                Region = "CA",
                Url = "www.sitecore.net"
            };
        }
    }
}
