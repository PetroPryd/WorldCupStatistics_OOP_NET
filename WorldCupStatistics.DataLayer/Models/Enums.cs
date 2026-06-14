using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.DataLayer.Models
{
    public enum Gender { Men, Women }

    public enum Position { Unknown, Goalie, Defender, Midfield, Forward }

    public enum MatchEventType
    {
        Unknown, Goal, GoalPenalty, OwnGoal, YellowCard, RedCard, SubstitutionIn, SubstitutionOut
    }

    public static class EnumParsing
    {
        public static Position ToPosition(string? value) => value switch
        {
            "Goalie" => Position.Goalie,
            "Defender" => Position.Defender,
            "Midfield" => Position.Midfield,
            "Forward" => Position.Forward,
            _ => Position.Unknown
        };

        public static MatchEventType ToMatchEventType(string? value) => value switch
        {
            "goal" => MatchEventType.Goal,
            "goal-penalty" => MatchEventType.GoalPenalty,
            "goal-own" => MatchEventType.OwnGoal,
            "own-goal" => MatchEventType.OwnGoal,
            "yellow-card" => MatchEventType.YellowCard,
            "red-card" => MatchEventType.RedCard,
            "substitution-in" => MatchEventType.SubstitutionIn,
            "substitution-out" => MatchEventType.SubstitutionOut,
            _ => MatchEventType.Unknown
        };

        public static bool IsGoal(this MatchEventType type)
            => type is MatchEventType.Goal or MatchEventType.GoalPenalty;
    }

    public static class GenderExtensions
    {
        public static string ToSegment(this Gender gender) => gender == Gender.Women ? "women" : "men";
    }
}
