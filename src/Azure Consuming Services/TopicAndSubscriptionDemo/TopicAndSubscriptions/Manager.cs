using Azure;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TopicAndSubscriptions
{
    internal class Manager
    {
        private ServiceBusAdministrationClient serviceBusAdministrationClient;
        public Manager(string connectionString)
        {
            serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);
        }

        public async Task<Response<TopicProperties>> CreateTopicAsync(string topicName)
        {
            Console.WriteLine($"Creating topic {topicName}");

            if (await serviceBusAdministrationClient.TopicExistsAsync(topicName))
            {
                await serviceBusAdministrationClient.DeleteTopicAsync(topicName);
            }

            return await serviceBusAdministrationClient.CreateTopicAsync(topicName);
        }

        public async Task<Response<SubscriptionProperties>> CreateSubscriptionAsync(string topicName, string subscriptionName)
        {
            Console.WriteLine($"Creating subscription {topicName}/{subscriptionName}");

            return await serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        public async Task<Response<SubscriptionProperties>> CreateSubscriptionWithSqlFilterAsync(string topicName, string subscriptionName, string sqlExpression)
        {
            Console.WriteLine($"Creating subscription with sql filter {topicName}/{subscriptionName} ({sqlExpression})");

            CreateRuleOptions ruleOptions = new CreateRuleOptions("Default", new SqlRuleFilter(sqlExpression));
            CreateSubscriptionOptions subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);

            return await serviceBusAdministrationClient.CreateSubscriptionAsync(subscriptionOptions, ruleOptions);
        }

        public async Task<Response<SubscriptionProperties>> CreateSubscriptionWithCorrelationFilterAsync(string topicName, string subscriptionName, string correlationId)
        {
            Console.WriteLine($"Creating subscription with correlation id {topicName}/{subscriptionName} ({correlationId})");

            CreateRuleOptions ruleOptions = new CreateRuleOptions("Default", new CorrelationRuleFilter(correlationId));
            CreateSubscriptionOptions subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);

            return await serviceBusAdministrationClient.CreateSubscriptionAsync(subscriptionOptions, ruleOptions);
        }

        public AsyncPageable<SubscriptionProperties> GetSubscriptionsForTopicAsync(string topicName)
        {
            return serviceBusAdministrationClient.GetSubscriptionsAsync(topicName);
        }
    }
}
