#Mock GeoIP Provider
Sample code for a mock GeoIP provider

*MockLookupProvider.cs*
Mock lookup provider implementation

*Sitecore.Poc.MockGeoIp.config*
Config file to replace default provider with Mock GeoIp provider

*GeoIp.aspx*
Test page to check GeoIp data on current interaction

NOTE: Be sure to clear cache and remove GeoIp records for 127.0.0.1 for mock provider to take effect
