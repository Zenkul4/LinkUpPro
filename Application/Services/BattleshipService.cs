using LinkUpProject.Application.Interfaces.Repositories;
using LinkUpProject.Application.Interfaces.Services;
using LinkUpProject.Application.ViewModels.Battleship;
using LinkUpProject.Domain.Common;
using LinkUpProject.Domain.Entities;

namespace LinkUpProject.Application.Services;

public class BattleshipService : IBattleshipService
{
    private const int BoardSize = 12;
    private const string SetupStatus = "Setup";
    private const string ReadyStatus = "Ready";
    private static readonly int[] RequiredShipSizes = [5, 4, 3, 3, 2];
    private readonly IBattleshipRepository _battleshipRepository;

    public BattleshipService(IBattleshipRepository battleshipRepository)
    {
        _battleshipRepository = battleshipRepository;
    }

    public async Task<Result<BattleshipIndexViewModel>> GetIndexAsync(string userId)
    {
        var friendships = await _battleshipRepository.GetActiveFriendshipsAsync(userId);
        var matches = await _battleshipRepository.GetMatchesForUserAsync(userId);

        var model = new BattleshipIndexViewModel
        {
            Opponents = friendships
                .Select(f => GetOtherUser(f, userId))
                .Where(u => u.IsActive && u.EmailConfirmed)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new BattleshipOpponentViewModel
                {
                    Id = u.Id,
                    FullName = GetFullName(u),
                    UserName = u.UserName ?? string.Empty
                })
                .ToList(),
            Matches = matches.Select(m => ToSummaryViewModel(m, userId)).ToList()
        };

        return Result<BattleshipIndexViewModel>.Success(model);
    }

    public async Task<Result<int>> CreateMatchAsync(string playerId, string opponentId)
    {
        if (playerId == opponentId)
            return Result<int>.Failure("No puedes iniciar una partida contra ti mismo.");

        var friendships = await _battleshipRepository.GetActiveFriendshipsAsync(playerId);
        var opponent = friendships
            .Select(f => GetOtherUser(f, playerId))
            .FirstOrDefault(u => u.Id == opponentId && u.IsActive && u.EmailConfirmed);

        if (opponent == null)
            return Result<int>.Failure("Solo puedes retar a un amigo activo.");

        if (await _battleshipRepository.HasOpenMatchAsync(playerId, opponentId))
            return Result<int>.Failure("Ya existe una partida abierta con este oponente.");

        var now = DateTime.UtcNow;
        var match = new BattleshipMatch
        {
            Player1Id = playerId,
            Player2Id = opponentId,
            CurrentTurnId = playerId,
            Status = SetupStatus,
            StartedAt = now,
            LastActivityAt = now
        };

        var createdMatch = await _battleshipRepository.AddMatchAsync(match);
        if (createdMatch == null)
            return Result<int>.Failure("Ya existe una partida abierta con este oponente.");

        return Result<int>.Success(createdMatch.Id);
    }

    public async Task<Result<BattleshipSetupViewModel>> GetSetupAsync(int matchId, string userId)
    {
        var match = await _battleshipRepository.GetMatchByIdAsync(matchId);
        if (match == null || !IsPlayer(match, userId))
            return Result<BattleshipSetupViewModel>.Failure("La partida no existe o no tienes acceso.");

        var currentUserShips = match.Ships
            .Where(s => s.PlayerId == userId)
            .OrderByDescending(s => s.Size)
            .ThenBy(s => s.Id)
            .Select(s => new SaveBattleshipShipViewModel
            {
                Size = s.Size,
                StartX = s.StartX + 1,
                StartY = s.StartY + 1,
                Direction = s.Direction
            })
            .ToList();

        var model = new BattleshipSetupViewModel
        {
            MatchId = match.Id,
            OpponentName = GetFullName(GetOpponent(match, userId)),
            Status = match.Status,
            RequiredShipSizes = RequiredShipSizes,
            Ships = currentUserShips.Any() ? currentUserShips : BuildEmptyShipRows(),
            CurrentUserHasPlacedShips = currentUserShips.Count == RequiredShipSizes.Length
        };

        return Result<BattleshipSetupViewModel>.Success(model);
    }

    public async Task<Result> SaveShipsAsync(BattleshipSetupViewModel viewModel, string userId)
    {
        var match = await _battleshipRepository.GetMatchByIdAsync(viewModel.MatchId);
        if (match == null || !IsPlayer(match, userId))
            return Result.Failure("La partida no existe o no tienes acceso.");

        if (match.Status != SetupStatus && match.Status != ReadyStatus)
            return Result.Failure("Esta partida ya no permite modificar la colocacion de barcos.");

        var validation = ValidateShips(viewModel.Ships);
        if (!validation.IsSuccess)
            return validation;

        var ships = viewModel.Ships.Select(ship => new BattleshipShip
        {
            MatchId = match.Id,
            PlayerId = userId,
            Size = ship.Size,
            StartX = ship.StartX - 1,
            StartY = ship.StartY - 1,
            Direction = NormalizeDirection(ship.Direction)
        }).ToList();

        await _battleshipRepository.ReplaceShipsAsync(match.Id, userId, ships);

        var refreshedMatch = await _battleshipRepository.GetMatchByIdAsync(match.Id);
        if (refreshedMatch != null)
        {
            var player1Ready = refreshedMatch.Ships.Count(s => s.PlayerId == refreshedMatch.Player1Id) == RequiredShipSizes.Length;
            var player2Ready = refreshedMatch.Ships.Count(s => s.PlayerId == refreshedMatch.Player2Id) == RequiredShipSizes.Length;

            refreshedMatch.Status = player1Ready && player2Ready ? ReadyStatus : SetupStatus;
            refreshedMatch.LastActivityAt = DateTime.UtcNow;
            await _battleshipRepository.UpdateMatchAsync(refreshedMatch);
        }

        return Result.Success();
    }

    private static Result ValidateShips(IReadOnlyList<SaveBattleshipShipViewModel> ships)
    {
        if (ships.Count != RequiredShipSizes.Length)
            return Result.Failure("Debes colocar exactamente 5 barcos.");

        var submittedSizes = ships.Select(s => s.Size).OrderBy(s => s).ToArray();
        var requiredSizes = RequiredShipSizes.OrderBy(s => s).ToArray();
        if (!submittedSizes.SequenceEqual(requiredSizes))
            return Result.Failure("Los barcos requeridos son de tamanos 5, 4, 3, 3 y 2.");

        var occupiedCells = new HashSet<string>();

        foreach (var ship in ships)
        {
            var direction = NormalizeDirection(ship.Direction);
            if (direction != "Horizontal" && direction != "Vertical")
                return Result.Failure("La direccion de cada barco debe ser Horizontal o Vertical.");

            var startX = ship.StartX - 1;
            var startY = ship.StartY - 1;

            if (startX < 0 || startX >= BoardSize || startY < 0 || startY >= BoardSize)
                return Result.Failure("Todos los barcos deben iniciar dentro del tablero.");

            for (var offset = 0; offset < ship.Size; offset++)
            {
                var x = direction == "Horizontal" ? startX + offset : startX;
                var y = direction == "Vertical" ? startY + offset : startY;

                if (x >= BoardSize || y >= BoardSize)
                    return Result.Failure($"El barco de tamano {ship.Size} se sale del tablero.");

                var key = $"{x}:{y}";
                if (!occupiedCells.Add(key))
                    return Result.Failure("Los barcos no pueden superponerse.");
            }
        }

        return Result.Success();
    }

    private static List<SaveBattleshipShipViewModel> BuildEmptyShipRows()
    {
        return RequiredShipSizes.Select(size => new SaveBattleshipShipViewModel
        {
            Size = size,
            StartX = 1,
            StartY = 1,
            Direction = "Horizontal"
        }).ToList();
    }

    private static BattleshipMatchSummaryViewModel ToSummaryViewModel(BattleshipMatch match, string userId)
    {
        var opponent = GetOpponent(match, userId);
        var currentUserShips = match.Ships.Count(s => s.PlayerId == userId);
        var opponentShips = match.Ships.Count(s => s.PlayerId == opponent.Id);

        return new BattleshipMatchSummaryViewModel
        {
            Id = match.Id,
            OpponentName = GetFullName(opponent),
            OpponentUserName = opponent.UserName ?? string.Empty,
            Status = match.Status,
            CurrentUserHasPlacedShips = currentUserShips == RequiredShipSizes.Length,
            CurrentUserShipsCount = currentUserShips,
            OpponentShipsCount = opponentShips,
            StartedAt = match.StartedAt,
            LastActivityAt = match.LastActivityAt,
            FinishedAt = match.FinishedAt,
            ElapsedHours = Math.Max(0, (int)Math.Floor(((match.FinishedAt ?? DateTime.UtcNow) - match.StartedAt).TotalHours)),
            IsFinished = match.FinishedAt.HasValue
        };
    }

    private static bool IsPlayer(BattleshipMatch match, string userId)
    {
        return match.Player1Id == userId || match.Player2Id == userId;
    }

    private static ApplicationUser GetOpponent(BattleshipMatch match, string userId)
    {
        return match.Player1Id == userId ? match.Player2 : match.Player1;
    }

    private static ApplicationUser GetOtherUser(Friendship friendship, string userId)
    {
        return friendship.User1Id == userId ? friendship.User2 : friendship.User1;
    }

    private static string GetFullName(ApplicationUser user)
    {
        return $"{user.FirstName} {user.LastName}".Trim();
    }

    private static string NormalizeDirection(string direction)
    {
        return string.Equals(direction, "Vertical", StringComparison.OrdinalIgnoreCase)
            ? "Vertical"
            : "Horizontal";
    }
}
