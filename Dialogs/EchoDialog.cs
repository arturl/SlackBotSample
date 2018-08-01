using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using Newtonsoft.Json.Linq;
// 2
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

        private async Task DemoButtonsAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            string s = @"{
                ""text"": ""Would you like to play a game ? "",
                ""attachments"": [
                    {
                        ""text"": ""Choose a game to play!"",
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

            reply.Text = null;
            reply.ChannelData = JObject.Parse(s);
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private async Task DemoMenusAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            string s = @"{
                ""text"": ""Would you like to play a game ? "",
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
            reply.ChannelData = JObject.Parse(s);
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private async Task DemoDialogsAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            string s = @"{
                ""trigger_id"": ""13345224609.738474920.8088930838d88f008e0"",
                ""dialog"": {
                    ""callback_id"": ""ryde-46e2b0"",
                    ""title"": ""Request a Ride"",
                    ""submit_label"": ""Request"",
                    ""notify_on_cancel"": true,
                    ""elements"": [
                        {
                            ""type"": ""text"",
                            ""label"": ""Pickup Location"",
                            ""name"": ""loc_origin""
                        },
                        {
                            ""type"": ""text"",
                            ""label"": ""Dropoff Location"",
                            ""name"": ""loc_destination""
                        }
                    ]
                }
            }";

            reply.Text = null;
            reply.ChannelData = JObject.Parse(s);
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var message = (await result) as IMessageActivity;
            
            switch(message.Text)
            {
                case "buttons":
                    await DemoButtonsAsync(context);
                    return;
                case "menus":
                    await DemoMenusAsync(context);
                    return;
                case "dialogs":
                    await DemoDialogsAsync(context);
                    return;
                case "help":
                {
                    var reply = context.MakeMessage();
                    reply.Text = null;
                    string s =
                    @"{
                        ""text"": 
""You can do one of the following commands:\n
""help - get this help\n
""buttons\n
""menus\n
""dialogs\n
                }";
                    reply.ChannelData = JObject.Parse(s);
                    await context.PostAsync(reply);
                    context.Wait(MessageReceivedAsync);
                    return;
                }
                default:
                {
                    var reply = context.MakeMessage();
                    reply.Text = $"Received: '{message.Text}'";
                    await context.PostAsync(reply);
                    context.Wait(MessageReceivedAsync);
                    return;
                }

            }
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