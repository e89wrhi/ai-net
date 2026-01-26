using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserModel : Aggregate<UserId>
{
}
