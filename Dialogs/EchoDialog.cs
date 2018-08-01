using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsyncOrig(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            if (message.Text == "happy")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you happy?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }            
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }
        
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var message = (await result) as IMessageActivity;
            
            var reply = context.MakeMessage();
            
            reply.Text = "yes to " + message.Text;
            
            string s2 = @"{
                ""text"": ""Would you like to play a game ? "",
                ""attachments"": [
                    {
                        ""text"": ""Choose a game to play"",
                        ""fallback"": ""You are unable to choose a game"",
                        ""callback_id"": ""wopr_game"",
                        ""color"": ""#3AA3E3"",
                        ""attachment_type"": ""default"",
                        ""actions"": [
                            {
                                ""name"": ""game"",
                                ""text"": ""Chess"",
                                ""type"": ""button"",
                                ""value"": ""chess""
                            },
                            {
                                ""name"": ""game"",
                                ""text"": ""Falken's Maze"",
                                ""type"": ""button"",
                                ""value"": ""maze""
                            },
                            {
                                ""name"": ""game"",
                                ""text"": ""Thermonuclear War"",
                                ""style"": ""danger"",
                                ""type"": ""button"",
                                ""value"": ""war"",
                                ""confirm"": {
                                    ""title"": ""Are you sure?"",
                                    ""text"": ""Wouldn't you prefer a good game of chess?"",
                                    ""ok_text"": ""Yes"",
                                    ""dismiss_text"": ""No""
                                }
                            }
                        ]
                    }
                ]
            }";
            
            string s3 = @"{
    ""text"": ""Would you like to play a game ?? "",
    ""response_type"": ""in_channel"",
    ""attachments"": [
        {
            ""text"": ""Choose a game to play"",
            ""fallback"": ""If you could read this message, you'd be choosing something fun to do right now."",
            ""color"": ""#3AA3E3"",
            ""attachment_type"": ""default"",
            ""callback_id"": ""game_selection"",
            ""actions"": [
                {
                    ""name"": ""games_list"",
                    ""text"": ""Pick a game..."",
                    ""type"": ""select"",
                    ""options"": [
                        {
                            ""text"": ""Hearts"",
                            ""value"": ""hearts""
                        },
                        {
                            ""text"": ""Bridge"",
                            ""value"": ""bridge""
                        },
                        {
                            ""text"": ""Checkers"",
                            ""value"": ""checkers""
                        },
                        {
                            ""text"": ""Chess"",
                            ""value"": ""chess""
                        },
                        {
                            ""text"": ""Poker"",
                            ""value"": ""poker""
                        },
                        {
                            ""text"": ""Falken's Maze"",
                            ""value"": ""maze""
                        },
                        {
                            ""text"": ""Global Thermonuclear War"",
                            ""value"": ""war""
                        }
                    ]
                }
            ]
        }
    ]
}";
            
            reply.Text = null;
            //reply.ChannelData = @"{ ""text"" : ""Test"" }";
            
            reply.ChannelData = JObject.Parse(s3);
            
            /*
            reply.ChannelData = JObject.Parse("{\"text\":\"New comic book alert!\",\"attachments\":[{\"title\":\"The Further Adventures of Slackbot\",\"fields\":[{\"title\":\"Volume\",\"value\":\"1\",\"short\":true},{\"title\":\"Issue\",\"value\":\"3\",\"short\":true}],\"author_name\":\"Stanford S. Strickland\",\"author_icon\":\"https://api.slack.com/img/api/homepage_custom_integrations-2x.png\",\"image_url\":\"http://i.imgur.com/OJkaVOI.jpg?1\"},{\"title\":\"Synopsis\",\"text\":\"After @episod pushed exciting changes to a devious new branch back in Issue 1, Slackbot notifies @don about an unexpected deploy...\"},{\"fallback\":\"Would you recommend it to customers?\",\"title\":\"Would you recommend it to customers?\",\"callback_id\":\"comic_1234_xyz\",\"color\":\"#3AA3E3\",\"attachment_type\":\"default\",\"actions\":[{\"name\":\"recommend\",\"text\":\"Recommend\",\"type\":\"button\",\"value\":\"recommend\"},{\"name\":\"no\",\"text\":\"No\",\"type\":\"button\",\"value\":\"bad\"}]}]}");
            */
            
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }        

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}