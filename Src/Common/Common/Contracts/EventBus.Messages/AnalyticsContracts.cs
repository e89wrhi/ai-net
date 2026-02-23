using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record AnalyticsReportGenerated(Guid Id, string ReportType) : IIntegrationEvent;
public record UserEngagementTracked(Guid Id, Guid UserId, string Action) : IIntegrationEvent;
public record SystemPerformanceLogged(Guid Id, string Metric, double Value) : IIntegrationEvent;
public record DashboardViewed(Guid Id, Guid UserId, string DashboardId) : IIntegrationEvent;
