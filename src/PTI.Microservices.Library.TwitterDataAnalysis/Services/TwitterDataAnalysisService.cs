using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetKeyPhrases;
using PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetSentiment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Services.Specialized
{
    /// <summary>
    /// Service in charge of exposing functionality to analyze twitter data
    /// </summary>
    public sealed class TwitterDataAnalysisService
    {
        private ILogger<TwitterDataAnalysisService> Logger { get; }
        private TwitterService TwitterService { get; }
        private AzureTextAnalyticsService AzureTextAnalyticsService { get; }

        /// <summary>
        /// Creates a new instance of <see cref="TwitterDataAnalysisService"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="twitterService"></param>
        /// <param name="azureTextAnalyticsService"></param>
        public TwitterDataAnalysisService(ILogger<TwitterDataAnalysisService> logger, 
            TwitterService twitterService, AzureTextAnalyticsService azureTextAnalyticsService)
        {
            this.Logger = logger;
            this.TwitterService = twitterService;
            this.AzureTextAnalyticsService = azureTextAnalyticsService;
        }

        /// <summary>
        /// Returns topics found in the last 50 tweets for the user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwExceptionOnError"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(List<string>, List<LinqToTwitter.Status>)> GetTopicsForUserAsync(string username, bool throwExceptionOnError=false,
            CancellationToken cancellationToken=default)
        {
            List<string> lstResults = new List<string>();
            var origintalTweets = await this.TwitterService.GetTweetsByUsernameAsync(username, maxTweets: 50, includeRetweets: false, cancellationToken: cancellationToken);
            if (origintalTweets != null && origintalTweets.Count > 0)
            {
                int totalPages = (int) Math.Ceiling( (double)origintalTweets.Count / 10);
                for (int iPage=0; iPage< totalPages; iPage++)
                {
                    try
                    {
                        var itemsToAnalyze = origintalTweets.Skip(iPage * 10).Take(10);
                        GetKeyPhrasesRequest getKeyPhrasesRequest = new GetKeyPhrasesRequest()
                        {
                            documents = itemsToAnalyze.Select(p => new GetKeyPhrasesRequestDocument()
                            {
                                id = p.StatusID.ToString(),
                                language = p.Lang,
                                text = p.Text
                            }).ToArray()
                        };
                        var pageResult = await this.AzureTextAnalyticsService.GetKeyPhrasesAsync(getKeyPhrasesRequest, cancellationToken);
                        lstResults.AddRange(pageResult.documents.SelectMany(p => p.keyPhrases));
                    }
                    catch (Exception ex)
                    {
                        this.Logger?.LogError(ex, ex.Message);
                        if (throwExceptionOnError)
                            throw;
                    }
                }
                return (lstResults.Distinct().ToList(), origintalTweets);
            }
            else
            {
                throw new Exception($"Unable to find original tweets for {username}");
            }
        }

        /// <summary>
        /// Returns sentiment found in the last 50 tweets for the user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="throwExceptionOnError"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(List<GetSentimentResponse>, List<LinqToTwitter.Status>)> GetSentimentForUserTweetsAsync(string username, bool throwExceptionOnError = false,
            CancellationToken cancellationToken = default)
        {
            List<GetSentimentResponse> lstResults = new List<GetSentimentResponse>();
            var origintalTweets = await this.TwitterService.GetTweetsByUsernameAsync(username, maxTweets: 50, includeRetweets: false, cancellationToken: cancellationToken);
            if (origintalTweets != null && origintalTweets.Count > 0)
            {
                int totalPages = (int)Math.Ceiling((double)origintalTweets.Count / 10);
                for (int iPage = 0; iPage < totalPages; iPage++)
                {
                    try
                    {
                        var itemsToAnalyze = origintalTweets.Skip(iPage * 10).Take(10);
                        GetSentimentRequest getSentimentRequest = new GetSentimentRequest()
                        {
                            documents = itemsToAnalyze.Select(p => new GetSentimentRequestDocument()
                            {
                                id = p.StatusID.ToString(),
                                language = p.Lang,
                                text = p.Text
                            }).ToArray()
                        };
                        var pageResult = await this.AzureTextAnalyticsService.GetSentimentAsync(getSentimentRequest, cancellationToken);
                        lstResults.Add(pageResult);
                    }
                    catch (Exception ex)
                    {
                        this.Logger?.LogError(ex, ex.Message);
                        if (throwExceptionOnError)
                            throw;
                    }
                }
                return (lstResults.Distinct().ToList(), origintalTweets);
            }
            else
            {
                throw new Exception($"Unable to find original tweets for {username}");
            }
        }
    }
}
