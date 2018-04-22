using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class Hello : ModuleBase<SocketCommandContext>
    {
        [Command("hello")] 
        public async Task CommandAsync()
        {
            await ReplyAsync("Hello World!");
        }
    }

    [Group("test")] //type one word or more for specific commands
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command ("ping")]
        [Alias("p")]
        public async Task TestAsync()
        {
            
            EmbedBuilder builder = new EmbedBuilder(); //the nice text
            
            builder.WithTitle("Ping!")
                .WithDescription("this is a really nice ping!")
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder.Build()); //return an embed (the fancy looking text)


        }

        [Command] //default when group command is called
        public async Task CommandAsync()
        {
            await ReplyAsync("Ping");
        }

        

    }
}
