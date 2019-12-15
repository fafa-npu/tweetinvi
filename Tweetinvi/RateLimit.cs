﻿using System;
using System.Threading.Tasks;
using Tweetinvi.Core.Controllers;
using Tweetinvi.Core.RateLimit;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace Tweetinvi
{
    /// <summary>
    /// Handle the RateLimits restricting the Twitter API.
    /// </summary>
    public static class RateLimit
    {
        [ThreadStatic]
        private static IHelpController _helpController;
        public static IHelpController HelpController
        {
            get
            {
                if (_helpController == null)
                {
                    Initialize();
                }

                return _helpController;
            }
        }

        [ThreadStatic]
        private static IRateLimitCacheManager _rateLimitCacheManager;

        private static IRateLimitCacheManager RateLimitCacheManager
        {
            get
            {
                if (_rateLimitCacheManager == null)
                {
                    Initialize();
                }

                return _rateLimitCacheManager;
            }
        }

        [ThreadStatic]
        private static IRateLimitAwaiter _rateLimitAwaiter;

        private static IRateLimitAwaiter RateLimitAwaiter
        {
            get
            {
                if (_rateLimitAwaiter == null)
                {
                    Initialize();
                }

                return _rateLimitAwaiter;
            }
        }

        private static readonly IRateLimitCache _rateLimitCache;

        static RateLimit()
        {
            Initialize();

            _rateLimitCache = TweetinviContainer.Resolve<IRateLimitCache>();
        }

        static void Initialize()
        {
            _helpController = TweetinviContainer.Resolve<IHelpController>();
            _rateLimitCacheManager = TweetinviContainer.Resolve<IRateLimitCacheManager>();
            _rateLimitAwaiter = TweetinviContainer.Resolve<IRateLimitAwaiter>();
        }

        /// <summary>
        /// Notify that a query is awaiting for RateLimits to become available in order to continue
        /// </summary>
        public static event EventHandler<QueryAwaitingEventArgs> QueryAwaitingForRateLimit
        {
            add { RateLimitAwaiter.QueryAwaitingForRateLimit += value; }
            remove { RateLimitAwaiter.QueryAwaitingForRateLimit -= value; }
        }

        /// <summary>
        /// Configure how to Tweetinvi will handle RateLimits
        /// </summary>
        public static RateLimitTrackerMode RateLimitTrackerMode
        {
            get { return TweetinviConfig.CurrentThreadSettings.RateLimitTrackerMode; }
            set { TweetinviConfig.CurrentThreadSettings.RateLimitTrackerMode = value; }
        }

        /// <summary>
        /// Clear all the RateLimits information stored in the cache
        /// </summary>
        public static void ClearRateLimitCache()
        {
            _rateLimitCache.ClearAll();
        }

        /// <summary>
        /// Clear a specific set of credentials RateLimits information stored in the cache
        /// </summary>
        public static void ClearRateLimitCache(ITwitterCredentials credentials)
        {
            _rateLimitCache.Clear(credentials);
        }

        /// <summary>
        /// Wait for the rate limits to be available. This should be used before executing a query
        /// </summary>
        public static void AwaitForQueryRateLimit(string query, ITwitterCredentials credentials)
        {
            RateLimitAwaiter.WaitForCredentialsRateLimit(query, credentials);
        }

        /// <summary>
        /// Wait for the rate limits to be available. This should be used before executing a query
        /// </summary>
        public static void AwaitForQueryRateLimit(IEndpointRateLimit endpointRateLimit)
        {
            RateLimitAwaiter.WaitForCredentialsRateLimit(endpointRateLimit);
        }


    }
}