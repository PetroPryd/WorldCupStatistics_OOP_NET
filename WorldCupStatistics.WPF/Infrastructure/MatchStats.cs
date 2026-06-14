using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;

namespace WorldCupStatistics.WPF.Infrastructure
{
    public static class MatchStats
    {
        public static (int Goals, int Yellows) ForPlayer(Match match, Player player)
        {
            var events = match.HomeTeamEvents.Concat(match.AwayTeamEvents)
                .Where(e => string.Equals(e.Player, player.Name, StringComparison.OrdinalIgnoreCase));

            int goals = events.Count(e => e.Type.IsGoal());
            int yellows = events.Count(e => e.Type == MatchEventType.YellowCard);
            return (goals, yellows);
        }
    }
}
