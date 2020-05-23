using System.Text;

namespace CyberCore.Utils
{
    public class CenterText
    {
        public readonly int CENTER_PX = 20;
        private int size;
        private string Text = "";

        public CenterText(string text, int Chars)
        {
            Text = text;
            size = Chars;
        }

        public static string sendCenteredMessage(string message, int cp)
        {
            if (message == null || message.Equals("")) return "";

            var messagePxSize = 0;
            var previousCode = false;
            var isBold = false;

            foreach (var c in message)
                if (c == '§')
                {
                    previousCode = true;
                }
                else if (previousCode)
                {
                    previousCode = false;
                    if (c == 'l' || c == 'L')
                        isBold = true;
                    else
                        isBold = false;
                }
                else
                {
                    var dFI = DefaultFontInfo.getDefaultFontInfo(c);
                    messagePxSize += isBold ? dFI.getBoldLength() : dFI.getLength();
                    messagePxSize++;
                }

            cp = cp / 2;
            var halvedMessageSize = messagePxSize / 2;
            var toCompensate = cp - halvedMessageSize;
            if ((toCompensate & 1) != 0) toCompensate -= 1;
            var spaceLength = DefaultFontInfo.SPACE.getLength() + 1;
            var compensated = 0;
            var sb = new StringBuilder();
            while (compensated < toCompensate)
            {
                sb.Append(" ");
                compensated += spaceLength * 2;
            }

            return sb + message + sb; //+"{|"+halvedMessageSize+" | "+toCompensate;
        }

        public static int getsize(string message)
        {
            if (message == null || message.Equals("")) return 0;

            var messagePxSize = 0;
            var previousCode = false;
            var isBold = false;

            foreach (var c in message)
                if (c == '§')
                {
                    previousCode = true;
                }
                else if (previousCode)
                {
                    previousCode = false;
                    if (c == 'l' || c == 'L')
                        isBold = true;
                    else
                        isBold = false;
                }
                else
                {
                    var dFI = DefaultFontInfo.getDefaultFontInfo(c);
                    messagePxSize += isBold ? dFI.getBoldLength() : dFI.getLength();
                    messagePxSize++;
                }

            return messagePxSize;
        }
    }
}