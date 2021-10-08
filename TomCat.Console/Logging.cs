using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TomCat.Main
{
    class Logging
    {
        public void LogCommand(SocketUser user, string command)
        {
            Console.WriteLine($"{command} executed by {user.Username}. Timestamp: {DateTime.Now.ToString("yyyy/MM/dd hh:mm tt")}");
        }

        public bool IsStaff(SocketGuildUser user, SocketCommandContext context)
        {
            var roles = context.Guild.Roles;

            if(user.Roles.Contains(roles.FirstOrDefault(x=>x.Name == "General Staff")))
            {
                return true;
            }
            else if (user.Roles.Contains(roles.FirstOrDefault(x => x.Name == "Event Staff")))
            {
                return true;
            }
            else if (user.Roles.Contains(roles.FirstOrDefault(x => x.Name == "Proprietor")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
