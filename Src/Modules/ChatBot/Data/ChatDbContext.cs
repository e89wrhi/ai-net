using ChatBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatBot.Data;

using AI.Common.EFCore;
using AI.Common.Web;
using Microsoft.Extensions.Logging;

public sealed class ChatDbContext : AppDbContextBase
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<ChatDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<ChatSession> Chats => Set<ChatSession>();
    public DbSet<ChatMessage> Messages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}