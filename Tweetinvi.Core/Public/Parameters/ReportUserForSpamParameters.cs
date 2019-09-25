﻿using Tweetinvi.Models;

namespace Tweetinvi.Parameters
{
    public interface IReportUserForSpamParameters : ICustomRequestParameters
    {
        /// <summary>
        /// The user you want to block
        /// </summary>
        IUserIdentifier UserIdentifier { get; set; }
        
        /// <summary>
        /// Whether you want to block the user in addition to report him
        /// </summary>
        bool? PerformBlock { get; set; }
    }

    public class ReportUserForSpamParameters : CustomRequestParameters, IReportUserForSpamParameters
    {
        public ReportUserForSpamParameters(IUserIdentifier userIdentifier)
        {
            UserIdentifier = userIdentifier;
        }

        public ReportUserForSpamParameters(string username) : this(new UserIdentifier(username))
        {
        }

        public ReportUserForSpamParameters(long? userId) : this(new UserIdentifier(userId))
        {
        }

        public ReportUserForSpamParameters(IReportUserForSpamParameters source) : base(source)
        {
            UserIdentifier = source?.UserIdentifier;
            PerformBlock = source?.PerformBlock;
        }

        public IUserIdentifier UserIdentifier { get; set; }
        public bool? PerformBlock { get; set; }
    }
}