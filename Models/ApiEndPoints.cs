using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcquireX.Models
{
    internal static class ApiEndPoints
    {
        public static string TokenUrl = "https://auth.dkhardware.com/realms/ctesting/protocol/openid-connect/token";
        public static string RSHughesUrl = "https://dkh-c-testing-api.staging.dkhdev.com/products/json";
        public static string BannerUrl = "https://dkh-c-testing-api.staging.dkhdev.com/products/xml";
    }
}
