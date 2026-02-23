namespace AI.Common.Constant;

/// <summary>
/// Hardcoded role names used across the system for authorization checks.
/// Using constants here prevents typos when using [Authorize(Roles = ...)] attributes.
/// </summary>

public static class IdentityConstant
{
    public static class Role
    {
        public const string User = "user";
    }
}