namespace Lifehandled.Application.Session
{
    /// <summary>
    /// Lightweight registry for current initialized session context.
    /// Used by VS01 startup flow to expose selected player/household data.
    /// </summary>
    public static class SessionContextRegistry
    {
        public static GameSessionContext Current { get; private set; }

        public static void Set(GameSessionContext context)
        {
            Current = context;
        }

        public static void Clear()
        {
            Current = null;
        }
    }
}
