using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.LexEvents;
using Chatbot.HelperDataClasses;
using Chatbot.IntentProcessors;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace InfraHelperChatbot
{
    public class ChatbotStartupProgram
    {
        private bool IsLocalDebug { get; set; }

        public ChatbotStartupProgram()
        {
            this.IsLocalDebug = false;
        }

        public ChatbotStartupProgram(bool isLocalDebug)
        {
            this.IsLocalDebug = isLocalDebug;
        }
        /// <summary>
        /// Then entry point for the Lambda function that looks at the current intent and calls 
        /// the appropriate intent process.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public LexResponse LambdaFunctionHandler(LexEvent lexEvent, ILambdaContext context)
        {
            IIntentProcessor process = null;
            try
            {                
                switch (lexEvent.CurrentIntent.Name)
                {
                    case "StartServer":
                        process = new ServerIntentProcessor(IsLocalDebug);
                        break;
                    case "DescribeInstances":
                        process = new DescribeIntentProcessor(IsLocalDebug);
                        break;
                    case "Greetings":
                        process = new GreetingIntentProcessor();
                        break;
                    case "Thanks":
                        process = new ThanksIntentProcessor();
                        break;
                    case "Helper":
                        process = new HelperIntentProcessor();
                        break;
                    case "LaunchInstance":
                        process = new LaunchIntentProcessor(IsLocalDebug);
                        break;
                    default:
                        throw new Exception($"Intent with name {lexEvent.CurrentIntent.Name} not supported");
                }
            }
            catch(Exception ex)
            {
                context.Logger.LogLine($"ChatbotStartupProgram::LambdaFunctionHandler exception message : {ex.Message}");
                context.Logger.LogLine($"ChatbotStartupProgram::LambdaFunctionHandler stacktrace : {ex.StackTrace}");
            }
            return process.Process(lexEvent, context);
        }
    }
}
