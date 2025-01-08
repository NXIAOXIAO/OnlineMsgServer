namespace OnlineMsgServer.Common
{
    class Log
    {
        public static void Debug(string msg)
        {
#if DEBUG
            Console.WriteLine(msg);
#endif
        }
    }
}