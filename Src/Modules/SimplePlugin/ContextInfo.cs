using System.ComponentModel;

namespace SimplePlugin;

[Description("Provides contextual information about users.")]
public class ContextInfo
{
    [Description("Gets the age of a person by their name.")]
    public int GetAge([Description("The name of the person.")] string name)
    {
        return name switch
        {
            "Alice" => 25,
            "Bob" => 30,
            _ => 0
        };
    }
}
