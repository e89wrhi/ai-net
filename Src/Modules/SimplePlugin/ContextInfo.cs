
namespace SimplePlugin;

public class ContextInfo
{
    [KernelFunction]
    public int GetAge(string name)
    {
        return name switch
        {
            "Alice" => 25,
            "Bob" => 30,
            _ => 0
        };
    }
}
