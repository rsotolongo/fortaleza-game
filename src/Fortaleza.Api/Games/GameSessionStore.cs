using System.Collections.Concurrent;
using Fortaleza.Localization;

namespace Fortaleza.Api.Games;

public sealed class GameSessionStore(TimeProvider time)
{
    public const int MaxSessions = 1000;
    public static readonly TimeSpan IdleTimeout = TimeSpan.FromHours(2);

    private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();
    private readonly Lock _gate = new();

    public GameSession? Create(int game, Language language)
    {
        lock (_gate)
        {
            Purge();
            if (_sessions.Count >= MaxSessions)
                return null;

            var session = new GameSession(Guid.NewGuid(), game, language, time.GetUtcNow());
            _sessions[session.Id] = session;
            return session;
        }
    }

    public GameSession? Find(Guid id)
    {
        if (!_sessions.TryGetValue(id, out var session))
            return null;

        var now = time.GetUtcNow();
        if (now - session.LastAccess > IdleTimeout)
        {
            _sessions.TryRemove(id, out _);
            return null;
        }

        session.Touch(now);
        return session;
    }

    public bool Remove(Guid id) => _sessions.TryRemove(id, out _);

    private void Purge()
    {
        var limit = time.GetUtcNow() - IdleTimeout;
        foreach (var (id, session) in _sessions)
            if (session.LastAccess < limit)
                _sessions.TryRemove(id, out _);
    }
}
