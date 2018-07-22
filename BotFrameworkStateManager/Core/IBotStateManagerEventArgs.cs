namespace BotFrameworkStateManager.Core
{
    public interface IBotStateManagerEventArgs
    {
        IBotState PreviousState { get; set; }
        IBotState CurrentState { get; set; }
    }
}
