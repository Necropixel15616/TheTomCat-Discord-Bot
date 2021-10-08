using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TomCat.Main
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _client.Log += Log;
            string token = "";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            var commandHandler = new CommandHandler(_client, _commands);
            await commandHandler.InstallCommandsAsync();
            await _client.SetGameAsync("?help");
            await Task.Delay(-1);
        }
        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.Message);
            return Task.CompletedTask;
        }

        
    }
}
