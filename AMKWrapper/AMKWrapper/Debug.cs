using System;

namespace AMKWrapper
{
    public static class Debug {
       /// <summary>
       /// Debug logging (colorful so cute)
       /// </summary>
       /// <param name="msg"></param>
       /// <param name="color"></param>
        public static void Log(string msg, ConsoleColor color = ConsoleColor.Gray) {
            
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();

        }


    }
}