using Bingora.Domain.Enums;

namespace Bingora.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public Guid BingoId { get; private set; }
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

    private readonly List<DrawnValue> _drawnValues = [];
    public IReadOnlyCollection<DrawnValue> DrawnValues => _drawnValues.AsReadOnly();

    private readonly List<Player> _players = [];
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    private readonly List<Winner> _winners = [];
    public IReadOnlyCollection<Winner> Winners => _winners.AsReadOnly();


    private Game()
    {
    }

    //Métodos fábrica
    public static Game Create(Guid bingoId, string title, int animationDuration, string? winnerSoundPath)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("El título de la partida es obligatorio.", nameof(title));

        if (animationDuration < 0)
            throw new ArgumentException("La duración de la animación no puede ser negativa.",
                nameof(animationDuration));

        return new Game
        {
            Id = Guid.NewGuid(),
            BingoId = bingoId,
            Title = title.Trim(),
            AnimationDuration = animationDuration,
            WinnerSoundPath = winnerSoundPath,
        };
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
}