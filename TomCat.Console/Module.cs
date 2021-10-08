using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace TomCat.Main
{
    public class Module : ModuleBase<SocketCommandContext>
    {
        private readonly Logging _logger = new Logging();
        [Command("ping")]
        public async Task PingAsync()
        {
            _logger.LogCommand(Context.User, "!ping");
            await ReplyAsync("Pong!");
        }

        [Command("credits")]
        public async Task CreditsAsync()
        {
            await ReplyAsync("Developed by Necropixel#3479. Special thanks to Saber and The Tom Cat Staff!");
        }

        [Command("menu")]
        [Summary("A command for retrieving The Tom Cat Menu")]
        public async Task MenuAsync()
        {
            await Context.Channel.SendFileAsync(Environment.CurrentDirectory + "..\\..\\..\\..\\Images\\tcmenu_002.png");
        }
    }

    [Group("raffle")]
    [Summary("The command group for the Raffle Commands")]
    public class RaffleModule : ModuleBase<SocketCommandContext>
    {
        private readonly Logging _logger = new Logging();
        public static List<RaffleTicket> raffleEntries = new List<RaffleTicket>();

        [Command("draw")]
        [Summary("A command for Drawing the Raffle Winner")]
        public async Task DrawAsync()
        {

            var user = Context.User as SocketGuildUser;
            bool isStaff = _logger.IsStaff(user, Context);
            if (isStaff)
            {
                //Log the command in the console
                _logger.LogCommand(Context.User, "!raffle draw");
                //if the list is empty, return
                if (!raffleEntries.Any())
                {
                    await ReplyAsync("There are no Entries!");
                }
                else
                {
                    //create a new list for strings
                    List<string> raffle = new List<string>();
                    //for each entry in the list, add their name equalling the amount of tickets they have
                    foreach (var ticket in raffleEntries)
                    {
                        for (int i = 0; i < ticket.TicketAmount; i++)
                        {
                            raffle.Add(ticket.PlayerName);
                        }
                    }
                    //randomly draw a winner from the list of string names
                    var random = new Random();
                    int index = random.Next(0, raffle.Count());
                    string winner = raffle[index];
                    await ReplyAsync("The Winner is " + winner + "!");
                    raffleEntries.Clear();
                    raffle.Clear();
                }
            }
            else
            {
                await ReplyAsync("Only Staff are authorized to operate the Raffle Bot!");
            }
        }

        [Command("add")]
        [Summary("The command for adding tickets to the raffle list")]
        public async Task AddAsync(string buyer, int tickets)
        {

            var user = Context.User as SocketGuildUser;

            bool isStaff = _logger.IsStaff(user, Context);
            if (isStaff)
            {
                //log the command in the console
                _logger.LogCommand(Context.User, "!raffle add");
                //if the number is less than 1, return
                if (tickets < 1)
                {
                    await ReplyAsync("Tickets cannot be lower than 1!");
                    return;
                }
                //if the name is empty, return
                if (buyer == null)
                {
                    await ReplyAsync("Name cannot be empty!");
                    return;
                }
                //variables for determining and locating an existing entry
                bool isAlreadyEntered = false;
                int entryIndex = 0;
                //search the list to see if any existing entries are found
                foreach (var entry in raffleEntries)
                {
                    if (buyer.ToUpper().Equals(entry.PlayerName.ToUpper()))
                    {
                        isAlreadyEntered = true;
                        entryIndex = raffleEntries.IndexOf(entry);

                    }
                }
                //if one is found, add the new tickets to the existing tickets
                if (isAlreadyEntered)
                {
                    var entry = raffleEntries[entryIndex];
                    entry.TicketAmount += tickets;
                }
                //otherwise, create a new entry
                else
                {
                    raffleEntries.Add(new RaffleTicket { PlayerName = buyer, TicketAmount = tickets });
                }
                await ReplyAsync("Tickets added!");
            }
            else
            {
                await ReplyAsync("Only Staff are authorized to operate the Raffle Bot!");
            }

        }

        [Command("clear")]
        [Summary("Clears all entries in the raffle")]
        public async Task ClearRaffleAsync()
        {

            var user = Context.User as SocketGuildUser;
            bool isStaff = _logger.IsStaff(user, Context);
            if (isStaff)
            {
                _logger.LogCommand(Context.User, "!raffle clear");
                //clear the raffle, then return
                raffleEntries.Clear();
                await ReplyAsync("Tickets Cleared!");
            }
            else
            {
                await ReplyAsync("Only Staff are authorized to operate the Raffle Bot!");
            }
        }

        [Command("list")]
        [Summary("A command for displaying all the current entries in the raffle")]
        public async Task DisplayEntriesAsync()
        {
            var user = Context.User as SocketGuildUser;
            bool isStaff = _logger.IsStaff(user, Context);
            if (isStaff)
            {
                //log the command in the console
                _logger.LogCommand(Context.User, "!raffle list");
                //if the raffle is empty, return
                if (!raffleEntries.Any())
                {
                    await ReplyAsync("There are no raffle tickets!");
                    return;
                }
                //create the message with code block formatting for easy visibility
                string listOfRaffles = "```\n";
                //for each entry, add their name and amount of tickets
                foreach (var entry in raffleEntries)
                {
                    listOfRaffles += $"Name: {entry.PlayerName}\tTickets: {entry.TicketAmount}\n";
                }
                //finish the message and send it
                listOfRaffles += "```";
                await ReplyAsync(listOfRaffles);
            }
            else
            {
                await ReplyAsync("Only Staff are authorized to operate the Raffle Bot!");
            }

        }

        [Command("remove")]
        [Summary("A command for removing tickets from the raffle")]
        public async Task RemoveTicketAsync(string name, int tickets = 0)
        {
            var user = Context.User as SocketGuildUser;
            bool isStaff = _logger.IsStaff(user, Context);
            if (isStaff)
            {
                //Log the command in the console
                _logger.LogCommand(Context.User, "!raffle remove");
                //find the entry of desire
                RaffleTicket ticket = new RaffleTicket();
                foreach (var entry in raffleEntries)
                {
                    if (name.ToUpper().Equals(entry.PlayerName.ToUpper()))
                    {
                        ticket = entry;
                        break;
                    }
                }
                //if no entry was found, return
                if (ticket == null)
                {
                    await ReplyAsync("No Tickets were found!");
                    return;
                }
                //if the given amount of tickets exceeds the registered ticket count or the given number is 0, remove the entry entirely
                if (tickets >= ticket.TicketAmount || tickets == 0)
                {
                    raffleEntries.Remove(ticket);
                }
                //if the given number is less than 0, return
                else if (tickets <= -1)
                {
                    await ReplyAsync("You cant remove negative amounts of tickets!");
                    return;
                }
                //otherwise, subtract the given amount from the registered amount
                else
                {
                    ticket.TicketAmount -= tickets;
                }
                await ReplyAsync("Tickets Removed!");
            }
            else
            {
                await ReplyAsync("Only Staff are authorized to operate the Raffle Bot!");
            }
        }
    }

    [Group("help")]
    [Summary("A command group for help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command()]
        [Summary("The generic help command")]
        public async Task HelpAsync()
        {
            //The Generic Help text
            {
                await ReplyAsync("```\nThis is the Help Command!\nThe following are a list of commands TheTomCat can execute. Type !help {command} to view the help in more detail:\n\n"
                  + "raffle --- The Raffle Function for TheTomCat\n"
                  + "menu --- The Menu for The Tom Cat Nightclub\n"
                  //+ "theme --- Information regarding the next theme night at The Tom Cat"
                  + " ```");
            }
        }

        [Command("raffle")]
        [Summary("The Raffle Help Command")]
        public async Task HelpRaffleAsync()
        {
            //the raffle help text
            await ReplyAsync("```This is TheTomCat's Raffle function. To operate, you must type !raffle followed by one of the sub-commands\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
                "add --- add tickets to the raffle. Requires a name and ticket amount. for multiple word names, enclose it within quotation marks. E.g. !raffle add \"Necro Pixel\" 10\n\n" +
                "remove --- remove tickets from the raffle. Specify the name and the amount. If the amount is left empty or 0 is given, it will remove all the tickets of that entry. E.G !raffle remove \"Necro Pixel\" 3\n\n" +
                "list --- list all the registered tickets in the raffle\n\n" +
                "clear --- clear the raffle. This will remove **EVERY** ticket\n\n" +
                "draw --- draw the raffle. This will Choose the winner and then clear the raffle\n" +
                "\n```");
        }
    }
}