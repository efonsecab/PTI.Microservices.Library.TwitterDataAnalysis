# PTI.Microservices.Library.TwitterDataAnalysis

This is part of PTI.Microservices.Library set of packages

The purpose of this package is to facilitate Analyzing Twitter Data, while maintaining a consistent usage pattern among the different services in the group

**Examples:**

## Get Topics For User
    TwitterService twitterService = new TwitterService(null, 
        this.TwitterConfiguration, new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    AzureTextAnalyticsService azureTextAnalyticsService =
        new AzureTextAnalyticsService(null, this.AzureTextAnalyticsConfiguration,
        new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    TwitterDataAnalysisService twitterDataAnalysisService = new TwitterDataAnalysisService(
        null, twitterService, azureTextAnalyticsService);
    var (twitterUserTopics, twitterStatuses) = await twitterDataAnalysisService.GetTopicsForUserAsync(TWITTER_USERNAME);

## Get Sentiment For User Tweets
    TwitterService twitterService = new TwitterService(null,
        this.TwitterConfiguration, new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    AzureTextAnalyticsService azureTextAnalyticsService =
        new AzureTextAnalyticsService(null, this.AzureTextAnalyticsConfiguration,
        new Microservices.Library.Interceptors.CustomHttpClient(
            new Microservices.Library.Interceptors.CustomHttpClientHandler(null)));
    TwitterDataAnalysisService twitterDataAnalysisService = new TwitterDataAnalysisService(
        null, twitterService, azureTextAnalyticsService);
    var (sentimentResponses, twitterStatuses) = await twitterDataAnalysisService.GetSentimentForUserTweetsAsync(TWITTER_USERNAME);