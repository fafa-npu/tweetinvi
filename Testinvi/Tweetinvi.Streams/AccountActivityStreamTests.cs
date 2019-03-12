﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testinvi.Helpers;
using Testinvi.json.net;
using Tweetinvi;
using Tweetinvi.Core.Factories;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Core.Public.Streaming;
using Tweetinvi.Core.Wrappers;
using Tweetinvi.Events;
using Tweetinvi.Models.Webhooks;
using Tweetinvi.Streams;

namespace Testinvi.Tweetinvi.Streams
{
    [TestClass]
    public class AccountActivityStreamTests
    {
        private FakeClassBuilder<AccountActivityStream> _fakeBuilder;
        private const int ACCOUNT_ACTIVITY_USER_ID = 42;

        [TestInitialize]
        public void TestInitialize()
        {
            _fakeBuilder = new FakeClassBuilder<AccountActivityStream>();
        }

        public IAccountActivityStream CreateAccountActivityStream()
        {
            var activityStream = _fakeBuilder.GenerateClass(
                new ConstructorNamedParameter("jsonObjectConverter", TweetinviContainer.Resolve<IJsonObjectConverter>()),
                new ConstructorNamedParameter("jObjectWrapper", TweetinviContainer.Resolve<IJObjectStaticWrapper>()),
                new ConstructorNamedParameter("tweetFactory", TweetinviContainer.Resolve<ITweetFactory>()),
                new ConstructorNamedParameter("userFactory", TweetinviContainer.Resolve<IUserFactory>()),
                new ConstructorNamedParameter("messageFactory", TweetinviContainer.Resolve<IMessageFactory>()));

            activityStream.UserId = ACCOUNT_ACTIVITY_USER_ID;

            return activityStream;
        }

        [TestMethod]
        public void TweetEventRaised()
        {
            var activityStream = CreateAccountActivityStream();
            
            var json = @"{
	            ""for_user_id"": ""100"",
	            ""tweet_create_events"": [
	              " + JsonTests.TWEET_TEST_JSON + @"
	            ]
            }";

            var eventsReceived = new List<TweetCreatedEventArgs>();
            activityStream.TweetCreated += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Tweet.CreatedBy.Id, ACCOUNT_ACTIVITY_USER_ID);
            Assert.AreEqual(eventsReceived[0].CreatedBy, TweetCreatedBy.AccountUser);
        }

        [TestMethod]
        public void TweetEventRaisedInReplyToATweetOfCurrentAccountUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""tweet_create_events"": [
	              " + @"
                    {
                      ""created_at"": ""Thu Apr 06 15:24:15 +0000 2017"",
                      ""id_str"": ""850006245121695744"",
                      ""text"": ""1\/ Today we\u2019re sharing our vision for the future of the Twitter API platform!\nhttps:\/\/t.co\/XweGngmxlP"",
                      ""in_reply_to_status_id"": 382973,
                      ""in_reply_to_user_id"": 42,
                      ""user"": {
                        ""id"": 100,
                        ""name"": ""Twitter Dev"",
                        ""screen_name"": ""TwitterDev"",
                        ""location"": ""Internet"",
                        ""url"": ""https:\/\/dev.twitter.com\/"",
                        ""description"": ""Your official source for Twitter Platform news, updates & events. Need technical help? Visit https:\/\/twittercommunity.com\/ \u2328\ufe0f #TapIntoTwitter""
                      },
                      ""place"": {   
                      },
                      ""entities"": {
                        ""hashtags"": [      
                        ],
                        ""urls"": [
                          {
                            ""url"": ""https:\/\/t.co\/XweGngmxlP"",
                            ""unwound"": {
                              ""url"": ""https:\/\/cards.twitter.com\/cards\/18ce53wgo4h\/3xo1c"",
                              ""title"": ""Building the Future of the Twitter API Platform""
                            }
                          }
                        ],
                        ""user_mentions"": [     
                        ]
                      }
                    }" + @"
	            ]
            }";

            var eventsReceived = new List<TweetCreatedEventArgs>();
            activityStream.TweetCreated += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Tweet.CreatedBy.Id, 100);
            Assert.AreEqual(eventsReceived[0].CreatedBy, TweetCreatedBy.AnotherUserReplyingToAccountUser);
        }

        [TestMethod]
        public void TweetEventRaisedWhenCurrentUserIsMentioned()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""tweet_create_events"": [
	              " + @"
                    {
                      ""created_at"": ""Thu Apr 06 15:24:15 +0000 2017"",
                      ""id_str"": ""850006245121695744"",
                      ""text"": ""1\/ Today we\u2019re sharing our vision for the future of the Twitter API platform!\nhttps:\/\/t.co\/XweGngmxlP"",
                      ""in_reply_to_status_id"": null,
                      ""in_reply_to_user_id"": 42,
                      ""user"": {
                        ""id"": 100,
                        ""name"": ""Twitter Dev"",
                        ""screen_name"": ""TwitterDev"",
                        ""location"": ""Internet"",
                        ""url"": ""https:\/\/dev.twitter.com\/"",
                        ""description"": ""Your official source for Twitter Platform news, updates & events. Need technical help? Visit https:\/\/twittercommunity.com\/ \u2328\ufe0f #TapIntoTwitter""
                      },
                      ""place"": {   
                      },
                      ""entities"": {
                        ""hashtags"": [      
                        ],
                        ""urls"": [
                          {
                            ""url"": ""https:\/\/t.co\/XweGngmxlP"",
                            ""unwound"": {
                              ""url"": ""https:\/\/cards.twitter.com\/cards\/18ce53wgo4h\/3xo1c"",
                              ""title"": ""Building the Future of the Twitter API Platform""
                            }
                          }
                        ],
                        ""user_mentions"": [     
                        ]
                      }
                    }" + @"
	            ]
            }";

            var eventsReceived = new List<TweetCreatedEventArgs>();
            activityStream.TweetCreated += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Tweet.CreatedBy.Id, 100);
            Assert.AreEqual(eventsReceived[0].CreatedBy, TweetCreatedBy.AnotherUserMentioningTheAccountUser);
        }

        [TestMethod]
        public void TweetDeletedRaised()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
                ""for_user_id"": ""3198576760"",
                ""tweet_delete_events"": [
            {
                    ""status"": {
                        ""id"": ""601430178305220608"",
                        ""user_id"": ""3198576760""
                    },
                    ""timestamp_ms"": ""1432228155593""
                }
               ]
            }";

            var eventsReceived = new List<TweetDeletedEventArgs>();
            activityStream.TweetDeleted += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].UserId, 3198576760);
            Assert.AreEqual(eventsReceived[0].TweetId, 601430178305220608);
            Assert.AreEqual(eventsReceived[0].Timestamp, 1432228155593);
        }

        [TestMethod]
        public void FavouriteTweetRaised()
        {
            var activityStream = CreateAccountActivityStream();

            var tweetFavouritedJson = @"{
	            ""for_user_id"": ""100"",
	            ""favorite_events"": [{
                  ""favorited_status"" : " + JsonTests.TWEET_TEST_JSON + @",
                  ""user"": " + JsonTests.USER_TEST_JSON(4242) + @"
	            }]
            }";

            var eventsReceived = new List<TweetFavouritedEventArgs>();
            activityStream.TweetFavourited += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(tweetFavouritedJson));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Tweet.CreatedBy.Id, 42);
            Assert.AreEqual(eventsReceived[0].FavouritingUser.Id, 4242);
        }

        [TestMethod]
        public void FollowedUserRaised()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""follow_events"": [{
                  ""type"" : ""follow"",
                  ""source"": " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @",
                  ""target"" : " + JsonTests.USER_TEST_JSON(41) + @"
	            }]
            }";

            var eventsReceived = new List<UserFollowedEventArgs>();
            activityStream.FollowedUser += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Source.Id, ACCOUNT_ACTIVITY_USER_ID);
            Assert.AreEqual(eventsReceived[0].Target.Id, 41);
        }

        [TestMethod]
        public void FollowedEventWithUnexpectedType()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""follow_events"": [{
                  ""type"" : ""UNEXPECTED_TYPE"",
                  ""source"": " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @",
                  ""target"" : " + JsonTests.USER_TEST_JSON(41) + @"
	            }]
            }";

            var followedUserEvents = new List<UserEventArgs>();
            activityStream.FollowedUser += (sender, args) =>
            {
                followedUserEvents.Add(args);
            };

            activityStream.UnfollowedUser += (sender, args) =>
            {
                followedUserEvents.Add(args);
            };

            activityStream.FollowedByUser += (sender, args) =>
            {
                followedUserEvents.Add(args);
            };

            var unmanagedEvents = new List<UnmanagedMessageReceivedEventArgs>();
            activityStream.UnmanagedEventReceived += (sender, args) =>
            {
                unmanagedEvents.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(followedUserEvents.Count, 0);
            Assert.AreEqual(unmanagedEvents.Count, 1);
        }

        [TestMethod]
        public void FollowedByUserRaised()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""follow_events"": [{
                  ""type"" : ""follow"",
                  ""source"": " + JsonTests.USER_TEST_JSON(40) + @",
                  ""target"" : " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @"
	            }]
            }";

            var eventsReceived = new List<UserFollowedEventArgs>();
            activityStream.FollowedByUser += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Source.Id, 40);
            Assert.AreEqual(eventsReceived[0].Target.Id, ACCOUNT_ACTIVITY_USER_ID);
        }

        [TestMethod]
        public void UnFollowedUserRaised()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""follow_events"": [{
                  ""type"" : ""unfollow"",
                  ""source"": " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @",
                  ""target"" : " + JsonTests.USER_TEST_JSON(40) + @"
	            }]
            }";

            var eventsReceived = new List<UserUnFollowedEventArgs>();
            activityStream.UnfollowedUser += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Source.Id, ACCOUNT_ACTIVITY_USER_ID);
            Assert.AreEqual(eventsReceived[0].Target.Id, 40);
        }

        [TestMethod]
        public void UserBlockedRaised_WithTargetUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""block_events"": [{
                  ""target"" : " + JsonTests.USER_TEST_JSON(41) + @",
                  ""source"": " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @"
	            }]
            }";

            var eventsReceived = new List<UserBlockedEventArgs>();
            activityStream.UserBlocked += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Target.Id, 41);
        }

        [TestMethod]
        public void UserBlockedRaised_WithSourceUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""block_events"": [{
                  ""target"" : " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @",
                  ""source"": " + JsonTests.USER_TEST_JSON(40) + @"
	            }]
            }";

            var eventsReceived = new List<UserBlockedEventArgs>();
            activityStream.UserBlocked += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Target.Id, 40);
        }

        [TestMethod]
        public void UserMutedRaised_WithTargetUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""mute_events"": [{
                  ""type"": ""mute"",
                  ""target"" : " + JsonTests.USER_TEST_JSON(41) + @",
                  ""source"": " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @"
	            }]
            }";

            var eventsReceived = new List<UserMutedEventArgs>();
            activityStream.UserMuted += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Target.Id, 41);
        }

        [TestMethod]
        public void UserMutedRaised_WithSourceUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""100"",
	            ""mute_events"": [{
                  ""type"": ""mute"",
                  ""target"" : " + JsonTests.USER_TEST_JSON(ACCOUNT_ACTIVITY_USER_ID) + @",
                  ""source"": " + JsonTests.USER_TEST_JSON(41) + @"
	            }]
            }";

            var eventsReceived = new List<UserMutedEventArgs>();
            activityStream.UserMuted += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Target.Id, 41);
        }

        [TestMethod]
        public void UserRevokedAppPermissions_WithTargetUser()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""user_event"": {
		            ""revoke"": {
			            ""date_time"": ""2018-05-24T09:48:12+00:00"",
			            ""target"": {
				            ""app_id"": ""13090192""
			            },
			            ""source"": {
				            ""user_id"": ""63046977""
			            }
		            }
	            }
            }";

            var eventsReceived = new List<UserRevokedAppPermissionsEventArgs>();
            activityStream.UserRevokedAppPermissions += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].AppId, 13090192);
            Assert.AreEqual(eventsReceived[0].UserId, 63046977);
        }

        [TestMethod]
        public void DirectMessageReceived()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
  	            ""for_user_id"": ""4337869213"",
	            ""direct_message_events"": [{
		            ""type"": ""message_create"",
		            ""id"": ""954491830116155396"",
		            ""created_timestamp"": ""1516403560557"",
		            ""message_create"": {
			            ""target"": {
				            ""recipient_id"": ""4337869213""
			            },
			            ""sender_id"": ""3001969357"",
			            ""source_app_id"": ""13090192"",
			            ""message_data"": {
				            ""text"": ""Hello World!"",
				            ""entities"": {
					            ""hashtags"": [],
					            ""symbols"": [],
					            ""user_mentions"": [],
					            ""urls"": []
				            }
			            }
		            }
	            }],
                ""users"": {
		            ""3001969357"": {
			            ""id"": ""3001969357"",
			            ""created_timestamp"": ""1422556069340"",
			            ""name"": ""Jordan Brinks"",
			            ""screen_name"": ""furiouscamper"",
			            ""location"": ""Boulder, CO"",
			            ""description"": ""Alter Ego - Twitter PE opinions-are-my-own"",
			            ""url"": ""https://t.co/SnxaA15ZuY"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 22,
			            ""friends_count"": 45,
			            ""statuses_count"": 494,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg""
		            },
		            ""4337869213"": {
			            ""id"": ""4337869213"",
			            ""created_timestamp"": ""1448312972328"",
			            ""name"": ""Harrison Test"",
			            ""screen_name"": ""Harris_0ff"",
			            ""location"": ""Burlington, MA"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 8,
			            ""friends_count"": 8,
			            ""profile_image_url"": ""http://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png"",
			            ""statuses_count"": 240,
			            ""profile_image_url_https"": ""https://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png""
		            }
	            }
            }";

            var eventsReceived = new List<MessageEventArgs>();
            activityStream.MessageReceived += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Message.Text, "Hello World!");
            Assert.AreEqual(eventsReceived[0].Message.SenderId, 3001969357);
            Assert.IsNull(eventsReceived[0].Message.App);
        }

        [TestMethod]
        public void DirectMessageSent()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
  	            ""for_user_id"": """ + ACCOUNT_ACTIVITY_USER_ID + @""",
	            ""direct_message_events"": [{
		            ""type"": ""message_create"",
		            ""id"": ""954491830116155396"",
		            ""created_timestamp"": ""1516403560557"",
		            ""message_create"": {
			            ""target"": {
				            ""recipient_id"": ""4337869213""
			            },
			            ""sender_id"": """ + ACCOUNT_ACTIVITY_USER_ID + @""",
			            ""source_app_id"": ""13090192"",
			            ""message_data"": {
				            ""text"": ""Hello World!"",
				            ""entities"": {
					            ""hashtags"": [],
					            ""symbols"": [],
					            ""user_mentions"": [],
					            ""urls"": []
				            }
			            }
		            }
	            }],
	            ""apps"": {
		            ""13090192"": {
			            ""id"": ""13090192"",
			            ""name"": ""FuriousCamperTestApp1"",
			            ""url"": ""https://twitter.com/furiouscamper""
		            }
                },
		        ""users"": {
		            ""3001969357"": {
			            ""id"": ""3001969357"",
			            ""created_timestamp"": ""1422556069340"",
			            ""name"": ""Jordan Brinks"",
			            ""screen_name"": ""furiouscamper"",
			            ""location"": ""Boulder, CO"",
			            ""description"": ""Alter Ego - Twitter PE opinions-are-my-own"",
			            ""url"": ""https://t.co/SnxaA15ZuY"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 22,
			            ""friends_count"": 45,
			            ""statuses_count"": 494,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg""
		            },
		            ""4337869213"": {
			            ""id"": ""4337869213"",
			            ""created_timestamp"": ""1448312972328"",
			            ""name"": ""Harrison Test"",
			            ""screen_name"": ""Harris_0ff"",
			            ""location"": ""Burlington, MA"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 8,
			            ""friends_count"": 8,
			            ""profile_image_url"": ""http://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png"",
			            ""statuses_count"": 240,
			            ""profile_image_url_https"": ""https://abs.twimg.com/sticky/default_profile_images/default_profile_normal.png""
		            }
	            }
            }";

            var eventsReceived = new List<MessageEventArgs>();
            activityStream.MessageSent += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].Message.Text, "Hello World!");
            Assert.AreEqual(eventsReceived[0].Message.SenderId, ACCOUNT_ACTIVITY_USER_ID);
            Assert.AreEqual(eventsReceived[0].Message.App.Name, "FuriousCamperTestApp1");
        }

        [TestMethod]
        public void UserIsTypingDirectMessage()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""4337869213"",
	            ""direct_message_indicate_typing_events"": [{
		            ""created_timestamp"": ""1518127183443"",
		            ""sender_id"": ""3284025577"",
		            ""target"": {
			            ""recipient_id"": ""3001969357""
		            }
	            }],
	            ""users"": {
		            ""3001969357"": {
			            ""id"": ""3001969357"",
			            ""created_timestamp"": ""1422556069340"",
			            ""name"": ""Jordan Brinks"",
			            ""screen_name"": ""furiouscamper"",
			            ""location"": ""Boulder, CO"",
			            ""description"": ""Alter Ego - Twitter PE opinions-are-my-own"",
			            ""url"": ""https://t.co/SnxaA15ZuY"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 23,
			            ""friends_count"": 47,
			            ""statuses_count"": 509,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg""
		            },
		            ""3284025577"": {
			            ""id"": ""3284025577"",
			            ""created_timestamp"": ""1437281176085"",
			            ""name"": ""Bogus Bogart"",
			            ""screen_name"": ""emilyannsheehan"",
			            ""protected"": true,
			            ""verified"": false,
			            ""followers_count"": 1,
			            ""friends_count"": 4,
			            ""statuses_count"": 35,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/763383202857779200/ndvZ96mE_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/763383202857779200/ndvZ96mE_normal.jpg""
		            }
	            }
            }";

            var eventsReceived = new List<UserIsTypingMessageEventArgs>();
            activityStream.UserIsTypingMessage += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].SenderId, 3284025577);
            Assert.AreEqual(eventsReceived[0].RecipientId, 3001969357);
            Assert.AreEqual(eventsReceived[0].Sender.Id, 3284025577);
            Assert.AreEqual(eventsReceived[0].Recipient.Id, 3001969357);
        }

        [TestMethod]
        public void UserReadMessage()
        {
            var activityStream = CreateAccountActivityStream();

            var json = @"{
	            ""for_user_id"": ""4337869213"",
	            ""direct_message_mark_read_events"": [{
		            ""created_timestamp"": ""1518452444662"",
		            ""sender_id"": ""199566737"",
		            ""target"": {
			            ""recipient_id"": ""3001969357""
		            },
		            ""last_read_event_id"": ""963085315333238788""
	            }],
	            ""users"": {
		            ""199566737"": {
			            ""id"": ""199566737"",
			            ""created_timestamp"": ""1286429788000"",
			            ""name"": ""Dan Brunsdon"",
			            ""screen_name"": ""LeBraat"",
			            ""location"": ""Denver, CO"",
			            ""description"": ""data by day @twitter, design by dusk"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 299,
			            ""friends_count"": 336,
			            ""statuses_count"": 752,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/936652894371119105/YHEozVAg_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/936652894371119105/YHEozVAg_normal.jpg""
		            },
		            ""3001969357"": {
			            ""id"": ""3001969357"",
			            ""created_timestamp"": ""1422556069340"",
			            ""name"": ""Jordan Brinks"",
			            ""screen_name"": ""furiouscamper"",
			            ""location"": ""Boulder, CO"",
			            ""description"": ""Alter Ego - Twitter PE opinions-are-my-own"",
			            ""url"": ""https://t.co/SnxaA15ZuY"",
			            ""protected"": false,
			            ""verified"": false,
			            ""followers_count"": 23,
			            ""friends_count"": 48,
			            ""statuses_count"": 510,
			            ""profile_image_url"": ""http://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg"",
			            ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/851526626785480705/cW4WTi7C_normal.jpg""
		            }
	            }
            }";

            var eventsReceived = new List<UserReadMessageConversationEventArgs>();
            activityStream.UserReadMessage += (sender, args) =>
            {
                eventsReceived.Add(args);
            };

            // Act
            activityStream.WebhookMessageReceived(new WebhookMessage(json));

            // Assert
            Assert.AreEqual(eventsReceived.Count, 1);
            Assert.AreEqual(eventsReceived[0].SenderId, 199566737);
            Assert.AreEqual(eventsReceived[0].RecipientId, 3001969357);
            Assert.AreEqual(eventsReceived[0].Sender.Id, 199566737);
            Assert.AreEqual(eventsReceived[0].Recipient.Id, 3001969357);
            Assert.AreEqual(eventsReceived[0].LastReadEventId, "963085315333238788");
        }
    }
}
