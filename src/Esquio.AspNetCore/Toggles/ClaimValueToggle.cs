﻿using Esquio.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Esquio.AspNetCore.Toggles
{
    [DesignType(Description = "Toggle that is active depending on the current claims of authenticated users.")]
    [DesignTypeParameter(ParameterName = ClaimType, ParameterType = "System.String", ParameterDescription = "The claim type used to check value.")]
    [DesignTypeParameter(ParameterName = ClaimValues, ParameterType = "System.String", ParameterDescription = "The claim value to check, multiple items separated by ';'.")]
    public class ClaimValueToggle
        : IToggle
    {
        const string ClaimType = nameof(ClaimType);
        const string ClaimValues = nameof(ClaimValues);
        private static char[] SPLIT_SEPARATOR = new char[] { ';' };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRuntimeFeatureStore _featureStore;
        public ClaimValueToggle(IHttpContextAccessor httpContextAccessor, IRuntimeFeatureStore store)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _featureStore = store ?? throw new ArgumentNullException(nameof(store));
        }
        public async Task<bool> IsActiveAsync(string featureName, string productName = null)
        {
            var feature = await _featureStore.FindFeatureAsync(featureName, productName);
            var toggle = feature.GetToggle(this.GetType().FullName);
            var data = toggle.GetData();

            string claimType = data.ClaimType?.ToString();
            string allowedValues = data.ClaimValues?.ToString();

            if (claimType != null
                &&
                ClaimValues != null)
            {
                var user = _httpContextAccessor.HttpContext.User;
                if (user != null && user.Identity.IsAuthenticated)
                {
                    var value = user.FindFirst(claimType)?
                        .Value;

                    if (value != null)
                    {
                        var tokenizer = new StringTokenizer(allowedValues, SPLIT_SEPARATOR);

                        return tokenizer.Contains(
                            value, StringSegmentComparer.OrdinalIgnoreCase);
                    }
                }
            }
            return false;
        }
    }
}
