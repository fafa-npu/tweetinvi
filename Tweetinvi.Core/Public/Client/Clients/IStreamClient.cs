using Tweetinvi.Core.Streaming;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming;

namespace Tweetinvi.Client
{
    public interface IStreamClient
    {
        /// <inheritdoc cref="CreateSampleStream(ICustomRequestParameters)"/>
        ISampleStream CreateSampleStream();

        /// <summary>
        /// Create a stream notifying that a random tweets has been created.
        /// https://dev.twitter.com/streaming/reference/get/statuses/sample
        /// </summary>
        ISampleStream CreateSampleStream(ICustomRequestParameters parameters);

        /// <inheritdoc cref="CreateFilteredStream(ICustomRequestParameters)"/>
        IFilteredStream CreateFilteredStream();

        /// <summary>
        /// Create a stream notifying the client when a tweet matching the specified criteria is created.
        /// https://dev.twitter.com/streaming/reference/post/statuses/filter
        /// </summary>
        IFilteredStream CreateFilteredStream(ICustomRequestParameters parameters);

        /// <inheritdoc cref="CreateTweetStream(ICustomRequestParameters)"/>
        ITweetStream CreateTweetStream();

        /// <summary>
        /// Create a stream that receive tweets
        /// </summary>
        ITweetStream CreateTweetStream(ICustomRequestParameters parameters);

        /// <inheritdoc cref="CreateTrackedStream(ICustomRequestParameters)"/>
        ITrackedStream CreateTrackedStream();

        /// <summary>
        /// Create a stream that receive tweets. In addition this stream allow you to filter the results received.
        /// </summary>
        ITrackedStream CreateTrackedStream(ICustomRequestParameters parameters);
    }
}