using System;

namespace AMKWrapper.Debugging
{
    public static class Debug {
       /// <summary>
       /// Debug logging (colorful so cute)
       /// </summary>
       /// <param name="msg"></param>
       /// <param name="color"></param>
        public static void Log(string msg, ConsoleColor color = ConsoleColor.DarkGray) {

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[" + DateTime.Now + "] ");
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();

        }


    }
}