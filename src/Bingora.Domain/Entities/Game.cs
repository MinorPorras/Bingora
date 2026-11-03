using Bingora.Domain.Enums;

namespace Bingora.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public Guid BingoId { get; private set; }
    public string GameCode { get; private set; } = string.Empty; //Ej: "AK9P2"
    public string? AccessPassword { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public GameStatus Status { get; private set; } = GameStatus.Created;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public int AnimationDuration { get; private set; } = 3;
    public string? WinnerSoundPath { get; private set; }

    //Navegación
    public Bingo Bingo { get; private set; }

    private readonly List<GamePattern> _gamePatterns = [];
    public IReadOnlyCollection<GamePattern> GamePatterns => _gamePatterns.AsReadOnly();

    private readonly List<Card> _cards = [];
    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    private readonly List<DrawnValue> _drawnValues = [];
    public IReadOnlyCollection<DrawnValue> DrawnValues => _drawnValues.AsReadOnly();

    private readonly List<Player> _players = [];
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    private readonly List<Winner> _winners = [];
    public IReadOnlyCollection<Winner> Winners => _winners.AsReadOnly();


    private Game()
    {
    }

    public static void Validate(string title, int animationDuration, string? accessPassword)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("El título de la partida es obligatorio.", nameof(title));

        if (animationDuration < 0)
            throw new ArgumentException("La duración de la animación no puede ser negativa.",
                nameof(animationDuration));

        if (!string.IsNullOrWhiteSpace(accessPassword) && accessPassword.Length > 10)
            throw new ArgumentException("La contraseña de acceso no puede exceder los 10 caracteres");
    }

    //Métodos fábrica
    public static Game Create(Guid bingoId, string title, int animationDuration, string? winnerSoundPath,
        string? accessPassword = null)
    {
        Validate(title, animationDuration, accessPassword);
        return new Game
        {
            Id = Guid.NewGuid(),
            BingoId = bingoId,
            GameCode = GenerateUniqueCode(),
            AccessPassword = accessPassword?.Trim(),
            Title = title.Trim(),
            AnimationDuration = animationDuration,
            WinnerSoundPath = winnerSoundPath,
            Status = GameStatus.Created,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateConfiguration(string title, int animationDuration, string? winnerSoundPath,
        string? accessPassword = null)
    {
        Validate(title, animationDuration, accessPassword);
        Title = title.Trim();
        AnimationDuration = animationDuration;
        WinnerSoundPath = winnerSoundPath;
        AccessPassword = accessPassword?.Trim();
    }

    //Maquinas de estados
    public void Start()
    {
        if (Status != GameStatus.Created)
            throw new InvalidOperationException(
                "Solo se puede iniciar una partida que no se haya iniciado previamente.");
        StartedAt = DateTime.UtcNow;
        Status = GameStatus.InProgress;
    }

    public void Pause()
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Solo se puede pausar una partida que esté en progreso.");
        Status = GameStatus.Paused;
    }

    public void Resume()
    {
        if (Status != GameStatus.Paused)
            throw new InvalidOperationException("Solo se puede reanudar una partida que esté pausado.");
        Status = GameStatus.InProgress;
    }

    public void Finish()
    {
        if (Status != GameStatus.InProgress && Status != GameStatus.Paused)
            throw new InvalidOperationException("No se puede finalizar una partida que no esté en juego o pausada.");
        Status = GameStatus.Finished;
        FinishedAt = DateTime.UtcNow;
    }

    //Gestión de colecciones
    public void AddDrawnValue(string value, int drawOrder)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Solo se pueden sacar valores en una partida en progreso.");

        var drawnValue = new DrawnValue(this.Id, value, drawOrder);
        _drawnValues.Add(drawnValue);
    }

    public void AddPattern(Guid patternId, int totalPrizes)
    {
        if (Status != GameStatus.Created)
            throw new InvalidOperationException(
                "Los patrones de una partida solo se pueden configurar antes de iniciarla.");

        if (_gamePatterns.Any(gp => gp.PatternId == patternId))
            throw new InvalidOperationException("Este patrón ya está configurado en la partida.");

        var gamePattern = GamePattern.Create(this.Id, patternId, totalPrizes);
        _gamePatterns.Add(gamePattern);
    }

    /// <summary>
    /// Genera una lista de nuevos cartones válidos para esta partida, 
    /// sin persistirlos aún en la base de datos.
    /// </summary>
    public IEnumerable<Card> GenerateCards(int quantity, Func<string> generateCardValuesFunc)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad de cartones debe ser mayor a 0.", nameof(quantity));

        if (Status == GameStatus.Finished)
            throw new ArgumentException("No se pueden generar nuevos cartones para una partida ya finalizada.");
        var cards = new List<Card>();
        for (var i = 0; i < quantity; i++)
        {
            var cardCode = GenerateUniqueCardCode();
            var values = generateCardValuesFunc();
            var newCard = Card.Create(BingoId, Id, cardCode, values, 0);
            cards.Add(newCard);
        }

        return cards;
    }

    public Player AddPlayer(string playerName, Card assignedCard)
    {
        if (Status == GameStatus.Finished)
            throw new InvalidOperationException("No se puede agregar jugadores a un juego ya finalizado.");
        if (_players.Any(p => p.Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase)))
            throw new InvalidOperationException("Ya existe un jugador con ese nombre en la partida.");
        if (assignedCard.Player != null)
            throw new InvalidOperationException("Este cartón ya está asignado a otro jugador.");
        var player = Player.Create(this.Id, assignedCard.Id, playerName);
        _players.Add(player);
        return player;
    }

    public void RemovePlayer(Guid playerId)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player is null)
            throw new InvalidOperationException("El jugador no se encuentra en la partida");
        _players.Remove(player);
    }

    // Helper
    public bool VerifyJoinPassword(string? inputPassword)
    {
        if (string.IsNullOrWhiteSpace(AccessPassword)) return true;
        return AccessPassword.Equals(inputPassword?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string GenerateUniqueCardCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        // Ej: "CARD-X7K9"
        return "CARD-" + new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string GenerateUniqueCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}