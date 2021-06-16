using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace MangaChecker.Commands.Modules
{
    public class HelpCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Provides list of commands with their descriptions.")]
        public async Task GetHelpInfo()
        {
            var allTypes = typeof(Program).Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(ModuleBase<SocketCommandContext>));

            List<string> commands = new();
            foreach (var type in allTypes)
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    var command = "";
                    var summary = "";
                    foreach (var attr in methodInfo.GetCustomAttributes(false))
                    {
                        if (attr is CommandAttribute commandAttribute)
                            command = commandAttribute.Text;
                        if (attr is SummaryAttribute summaryAttribute)
                            summary = summaryAttribute.Text;
                        
                        if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(summary))
                            continue;
                        
                        commands.Add($"**.{command}** - {summary}");
                    }
                }
            }

            StringBuilder builder = new();
            foreach (var commandDesc in commands)
                builder.AppendLine(commandDesc);

            await Context.Channel.SendMessageAsync(builder.ToString());
        }
    }
}