using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Dtos;
using WorldCupStatistics.DataLayer.Exceptions;
using WorldCupStatistics.DataLayer.Mapping;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Serialization;

namespace WorldCupStatistics.DataLayer.Repositories
{
    internal sealed class ApiWorldCupRepository : IWorldCupRepository
    {
        private readonly HttpClient _http;

        public ApiWorldCupRepository(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<Team>> GetTeamsAsync(Gender gender, CancellationToken ct = default)
        {
            var dtos = await GetAsync<List<TeamDto>>($"{gender.ToSegment()}/teams/results", ct);
            return dtos.Select(DtoMapper.ToTeam).ToList();
        }

        public async Task<IReadOnlyList<Match>> GetMatchesAsync(Gender gender, CancellationToken ct = default)
        {
            var dtos = await GetAsync<List<MatchDto>>($"{gender.ToSegment()}/matches", ct);
            return dtos.Select(DtoMapper.ToMatch).ToList();
        }

        public async Task<IReadOnlyList<Match>> GetMatchesForCountryAsync(Gender gender, string fifaCode, CancellationToken ct = default)
        {
            var url = $"{gender.ToSegment()}/matches/country?fifa_code={Uri.EscapeDataString(fifaCode)}";
            var dtos = await GetAsync<List<MatchDto>>(url, ct);
            return dtos.Select(DtoMapper.ToMatch).ToList();
        }

        private async Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct)
        {
            try
            {
                await using var stream = await _http.GetStreamAsync(relativeUrl, ct);
                var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptionsFactory.Default, ct);
                return result ?? throw new DataLayerException($"Empty response from '{relativeUrl}'.");
            }
            catch (HttpRequestException ex)
            {
                throw new DataLayerException($"API request failed for '{relativeUrl}': {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new DataLayerException($"API request timed out for '{relativeUrl}'.", ex);
            }
            catch (JsonException ex)
            {
                throw new DataLayerException($"Invalid JSON from '{relativeUrl}': {ex.Message}", ex);
            }
        }
    }
}
