using System;
using System.Collections.Generic;
using System.Text;
using BearPlan.Common.Enums;

namespace BearPlan.Infrastructure.Authentication
{
    public class RefreshTokenCacheModel
    {
        public long UserId { get; set; }

        public VersionEnum ApiVersion { get; set; }
        public long ExpiresAt { get; set; }
    }

}
