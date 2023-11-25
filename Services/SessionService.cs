using MongoDB.Driver;
using RetroCarsWebApp.Data;
using RetroCarsWebApp.Models;

namespace RetroCarsWebApp.Services;

public class SessionService
{
    private readonly IMongoCollection<Session> _sessionCollection;
    
    public SessionService(string connectionString, string dbName)
    {
        var client = new MongoDbContext(connectionString, dbName);
        _sessionCollection = client.GetCollection<Session>("Sessions");
    }
    
    public Session CreateSession(Session session)
    {
        _sessionCollection.InsertOne(session);
        return session;
    }
    
    public Session? GetLastActiveSession(DateTime timestamp) =>
        _sessionCollection.FindAsync(session => session.ExpiresAt > timestamp).Result.FirstOrDefault();        
    
    public Session? GetSession(string? id) =>
        _sessionCollection.FindAsync(session => session.Id == id).Result.FirstOrDefault();
    
    public void UpdateSession(Session session) =>
        _sessionCollection.ReplaceOneAsync(s => s.Id == session.Id, session).Wait();
    
    public void DeleteSession(string id) =>
        _sessionCollection.DeleteOneAsync(session => session.Id == id).Wait();
    
    public IEnumerable<Session> GetUserSessions(string userId) =>
        new List<Session>(_sessionCollection.FindAsync(session => session.UserId == userId).Result.ToList());
}