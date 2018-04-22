using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;


        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            
            _services = new ServiceCollection()
                  .AddSingleton(_client)
                  .AddSingleton(_commands)
                  .BuildServiceProvider();

            //create bot at https://discordapp.com/developers/applications/me
            //To add bot to the server, go to https://discordapp.com/oauth2/authorize?&client_id=YOUR_CLIENT_ID_HERE&scope=bot&permissions=0 and replace YOUR_CLIENT_ID_HERE with the client ID of the bot
            //string botToken = "WRITE TOKEN HERE"; 

            string botToken = System.IO.File.ReadAllText(@"token.txt"); //to get token from txt file

            //event subcription
            _client.Log += Log;
            _client.UserJoined += AnnouncementUserJoined;
            await RegisterCommandsAsync(); //register comand module

            await _client.LoginAsync(Discord.TokenType.Bot, botToken); //log in client asynchronously
            SetGame(_client, "Test"); //dispolay "Playing ***" status

            await _client.StartAsync();

            await Task.Delay(-1); //delay task to prevent it from closing
        }

        public void SetGame(DiscordSocketClient _client, string game)
        {
            _client.SetGameAsync(game);

        }

        private async Task AnnouncementUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel as SocketTextChannel;
            var x = guild.Channels.GetEnumerator();
            
            Console.WriteLine(channel.ToString());
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
            
        }

        private Task Log(Discord.LogMessage arg) //write in console what is going on
        {
            Console.WriteLine(arg);
            
            return Task.CompletedTask;

        }


        public async Task RegisterCommandsAsync()
        {

            _client.MessageReceived += HandleCommandAsync;
            
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage; //create message

            if (message is null || message.Author.IsBot) return; //if it is null, or if the the author is a bot, ignore

            int argPos = 0; //pointer which prefix ends


            //if message starts as ! or if someone mentions the bot
            if(message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message); //create socket comand context, give client and message

                var result = await _commands.ExecuteAsync(context, argPos, _services); //execute command, pass context, the position, and the services

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }

            }

        }

        private void BuildServiceProvider()
        {
            throw new NotImplementedException();
        }

        private object AddSingleton(DiscordSocketClient client)
        {
            throw new NotImplementedException();
        }
    }
}
