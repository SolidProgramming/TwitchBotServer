using System;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Shares.Enum;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Q42.HueApi.Models.Bridge;

namespace PhilipsHueController
{
    public class PhilipsHueControllerClient
    {
        public string AppName { private get; init; }
        public string DeviceName { private get; init; }

        public async Task RegisterClient(string bridgeIp, PhilipsHueBridgeLocatorType bridgeLocatorType, CancellationTokenSource cts = default)
        {
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            IBridgeLocator locator = GetBridgeLocator(bridgeLocatorType);
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(cts.Token);

            ILocalHueClient client = new LocalHueClient(bridgeIp);

            string appKey = await client.RegisterAsync("mypersonalappname", "mydevicename");

            //bridges = await HueBridgeDiscovery.CompleteDiscoveryAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            //bridges = await HueBridgeDiscovery.FastDiscoveryWithNetworkScanFallbackAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
        }

        private IBridgeLocator GetBridgeLocator(PhilipsHueBridgeLocatorType bridgeLocatorType)
        {
            return bridgeLocatorType switch
            {
                PhilipsHueBridgeLocatorType.HttpBridgeLocator => new HttpBridgeLocator(),
                PhilipsHueBridgeLocatorType.LocalNetworkScanBridgeLocator => new LocalNetworkScanBridgeLocator(),
                _ => null,
            };
        }
    }
}
