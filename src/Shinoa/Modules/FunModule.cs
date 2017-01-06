﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Shinoa.Modules
{
    public class FunModule : Abstract.Module
    {
        public static Color MODULE_COLOR = new Color(63, 81, 181);

        public override void HandleMessage(CommandContext context)
        {
            if (context.User.Id != Shinoa.DiscordClient.CurrentUser.Id)
            {
                if (context.Message.Content == @"o/")
                    context.Channel.SendMessageAsync(@"\o");
                else if (context.Message.Content == @"\o")
                    context.Channel.SendMessageAsync(@"o/");
                else if (context.Message.Content == @"/o/")
                    context.Channel.SendMessageAsync(@"\o\");
                else if (context.Message.Content == @"\o\")
                    context.Channel.SendMessageAsync(@"/o/");
            }
        }

        public override void Init()
        {
            this.BoundCommands.Add("pick", (c) =>
            {
                var choices = GetCommandParametersAsString(c.Message.Content).Split(new string[] { " or " }, StringSplitOptions.RemoveEmptyEntries);
                var choice = choices[new Random().Next(choices.Length)].Trim();

                var embed = new EmbedBuilder()
                .WithTitle($"I choose '{choice}'.")
                .WithColor(MODULE_COLOR);
                
                c.Channel.SendEmbedAsync(embed.Build());
            });

            this.BoundCommands.Add("roll", (c) =>
            {
                var rng = new Random();
                var multiplier = int.Parse(GetCommandParameters(c.Message.Content)[0].Split('d')[0]);
                var dieSize = int.Parse(GetCommandParameters(c.Message.Content)[0].Split('d')[1]);
                var total = 0;

                List<int> rolls = new List<int>();
                foreach (int i in Enumerable.Range(0, multiplier))
                {
                    int roll = rng.Next(dieSize) + 1;
                    rolls.Add(roll);
                    total += roll;
                }

                var rollsString = "";
                foreach (var roll in rolls)
                {
                    rollsString += $"{roll}, ";
                }
                rollsString = rollsString.Trim(new char[] { ' ', ',' });

                var embed = new EmbedBuilder()
                    .AddField(f => f.WithName("Total").WithValue(total.ToString()))
                    .AddField(f => f.WithName("Rolls").WithValue(rollsString))
                    .WithColor(MODULE_COLOR);

                c.Channel.SendEmbedAsync(embed.Build());
            });
        }
    }
}
