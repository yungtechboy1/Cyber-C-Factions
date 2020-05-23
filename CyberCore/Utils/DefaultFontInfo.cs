using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CyberCore.Utils
{
    public struct DefaultFontInfo
    {
        public static readonly DefaultFontInfo A = new DefaultFontInfo('A', 5);
        public static readonly DefaultFontInfo a = new DefaultFontInfo('a', 5);
        public static readonly DefaultFontInfo B = new DefaultFontInfo('B', 5);
        public static readonly DefaultFontInfo b = new DefaultFontInfo('b', 5);
        public static readonly DefaultFontInfo C = new DefaultFontInfo('C', 5);
        public static readonly DefaultFontInfo c = new DefaultFontInfo('c', 5);
        public static readonly DefaultFontInfo D = new DefaultFontInfo('D', 5);
        public static readonly DefaultFontInfo d = new DefaultFontInfo('d', 5);
    public static readonly DefaultFontInfo E = new DefaultFontInfo('E', 5);
    public static readonly DefaultFontInfo e = new DefaultFontInfo('e', 5);
    public static readonly DefaultFontInfo F = new DefaultFontInfo('F', 5);
    public static readonly DefaultFontInfo f = new DefaultFontInfo('f', 4);
    public static readonly DefaultFontInfo G = new DefaultFontInfo('G', 5);
    public static readonly DefaultFontInfo g = new DefaultFontInfo('g', 5);
    public static readonly DefaultFontInfo H = new DefaultFontInfo('H', 5);
    public static readonly DefaultFontInfo h = new DefaultFontInfo('h', 5); 
 public static readonly DefaultFontInfo I = new DefaultFontInfo('I', 3); 
 public static readonly DefaultFontInfo i = new DefaultFontInfo('i', 1); 
 public static readonly DefaultFontInfo J = new DefaultFontInfo('J', 5); 
 public static readonly DefaultFontInfo j = new DefaultFontInfo('j', 5); 
 public static readonly DefaultFontInfo K = new DefaultFontInfo('K', 5); 
 public static readonly DefaultFontInfo k = new DefaultFontInfo('k', 4); 
 public static readonly DefaultFontInfo L = new DefaultFontInfo('L', 5); 
 public static readonly DefaultFontInfo l = new DefaultFontInfo('l', 1); 
 public static readonly DefaultFontInfo M = new DefaultFontInfo('M', 5); 
 public static readonly DefaultFontInfo m = new DefaultFontInfo('m', 5); 
 public static readonly DefaultFontInfo N = new DefaultFontInfo('N', 5); 
 public static readonly DefaultFontInfo n = new DefaultFontInfo('n', 5); 
 public static readonly DefaultFontInfo O = new DefaultFontInfo('O', 5); 
 public static readonly DefaultFontInfo o = new DefaultFontInfo('o', 5); 
 public static readonly DefaultFontInfo P = new DefaultFontInfo('P', 5); 
 public static readonly DefaultFontInfo p = new DefaultFontInfo('p', 5); 
 public static readonly DefaultFontInfo Q = new DefaultFontInfo('Q', 5); 
 public static readonly DefaultFontInfo q = new DefaultFontInfo('q', 5); 
 public static readonly DefaultFontInfo R = new DefaultFontInfo('R', 5); 
 public static readonly DefaultFontInfo r = new DefaultFontInfo('r', 5); 
 public static readonly DefaultFontInfo S = new DefaultFontInfo('S', 5); 
 public static readonly DefaultFontInfo s = new DefaultFontInfo('s', 5); 
 public static readonly DefaultFontInfo T = new DefaultFontInfo('T', 5); 
 public static readonly DefaultFontInfo t = new DefaultFontInfo('t', 4); 
 public static readonly DefaultFontInfo U = new DefaultFontInfo('U', 5); 
 public static readonly DefaultFontInfo u = new DefaultFontInfo('u', 5); 
 public static readonly DefaultFontInfo V = new DefaultFontInfo('V', 5); 
 public static readonly DefaultFontInfo v = new DefaultFontInfo('v', 5); 
 public static readonly DefaultFontInfo W = new DefaultFontInfo('W', 5); 
 public static readonly DefaultFontInfo w = new DefaultFontInfo('w', 5); 
 public static readonly DefaultFontInfo X = new DefaultFontInfo('X', 5); 
 public static readonly DefaultFontInfo x = new DefaultFontInfo('x', 5); 
 public static readonly DefaultFontInfo Y = new DefaultFontInfo('Y', 5); 
 public static readonly DefaultFontInfo y = new DefaultFontInfo('y', 5); 
 public static readonly DefaultFontInfo Z = new DefaultFontInfo('Z', 5); 
 public static readonly DefaultFontInfo z = new DefaultFontInfo('z', 5); 
 public static readonly DefaultFontInfo NUM_1 = new DefaultFontInfo('1', 5); 
 public static readonly DefaultFontInfo NUM_2 = new DefaultFontInfo('2', 5); 
 public static readonly DefaultFontInfo NUM_3 = new DefaultFontInfo('3', 5); 
 public static readonly DefaultFontInfo NUM_4 = new DefaultFontInfo('4', 5); 
 public static readonly DefaultFontInfo NUM_5 = new DefaultFontInfo('5', 5); 
 public static readonly DefaultFontInfo NUM_6 = new DefaultFontInfo('6', 5); 
 public static readonly DefaultFontInfo NUM_7 = new DefaultFontInfo('7', 5); 
 public static readonly DefaultFontInfo NUM_8 = new DefaultFontInfo('8', 5); 
 public static readonly DefaultFontInfo NUM_9 = new DefaultFontInfo('9', 5); 
 public static readonly DefaultFontInfo NUM_0 = new DefaultFontInfo('0', 5); 
 public static readonly DefaultFontInfo EXCLAMATION_POINT = new DefaultFontInfo('!', 1); 
 public static readonly DefaultFontInfo AT_SYMBOL = new DefaultFontInfo('@', 6); 
 public static readonly DefaultFontInfo NUM_SIGN = new DefaultFontInfo('#', 5); 
 public static readonly DefaultFontInfo DOLLAR_SIGN = new DefaultFontInfo('$', 5); 
 public static readonly DefaultFontInfo PERCENT = new DefaultFontInfo('%', 5); 
 public static readonly DefaultFontInfo UP_ARROW = new DefaultFontInfo('^', 5); 
 public static readonly DefaultFontInfo AMPERSAND = new DefaultFontInfo('&', 5); 
 public static readonly DefaultFontInfo ASTERISK = new DefaultFontInfo('*', 5); 
 public static readonly DefaultFontInfo LEFT_PARENTHESIS = new DefaultFontInfo('(' , 4); 
 public static readonly DefaultFontInfo RIGHT_PERENTHESIS = new DefaultFontInfo(')', 4); 
 public static readonly DefaultFontInfo MINUS = new DefaultFontInfo('-', 5); 
 public static readonly DefaultFontInfo UNDERSCORE = new DefaultFontInfo('_', 5); 
 public static readonly DefaultFontInfo PLUS_SIGN = new DefaultFontInfo('+', 5); 
 public static readonly DefaultFontInfo EQUALS_SIGN = new DefaultFontInfo('=', 5); 
 public static readonly DefaultFontInfo LEFT_CURL_BRACE = new DefaultFontInfo('{', 4); 
 public static readonly DefaultFontInfo RIGHT_CURL_BRACE = new DefaultFontInfo('}', 4); 
 public static readonly DefaultFontInfo LEFT_BRACKET = new DefaultFontInfo('[', 3); 
 public static readonly DefaultFontInfo RIGHT_BRACKET = new DefaultFontInfo(']', 3); 
 public static readonly DefaultFontInfo COLON = new DefaultFontInfo(':', 1); 
 public static readonly DefaultFontInfo SEMI_COLON = new DefaultFontInfo(';', 1); 
 public static readonly DefaultFontInfo DOUBLE_QUOTE = new DefaultFontInfo('"', 3); 
 public static readonly DefaultFontInfo SINGLE_QUOTE = new DefaultFontInfo('\'', 1); 
 public static readonly DefaultFontInfo LEFT_ARROW = new DefaultFontInfo('<', 4); 
 public static readonly DefaultFontInfo RIGHT_ARROW = new DefaultFontInfo('>', 4); 
 public static readonly DefaultFontInfo QUESTION_MARK = new DefaultFontInfo('?', 5); 
 public static readonly DefaultFontInfo SLASH = new DefaultFontInfo('/', 5); 
 public static readonly DefaultFontInfo BACK_SLASH = new DefaultFontInfo('\\', 5); 
 public static readonly DefaultFontInfo LINE = new DefaultFontInfo('|', 1); 
 public static readonly DefaultFontInfo TILDE = new DefaultFontInfo('~', 5); 
 public static readonly DefaultFontInfo TICK = new DefaultFontInfo('`', 2); 
 public static readonly DefaultFontInfo PERIOD = new DefaultFontInfo('.', 1); 
 public static readonly DefaultFontInfo COMMA = new DefaultFontInfo(',', 1); 
 public static readonly DefaultFontInfo SPACE = new DefaultFontInfo(' ', 3); 
 public static readonly DefaultFontInfo DEFAULT = new DefaultFontInfo('a', 4);

    private char character;
    private int length;

    DefaultFontInfo(char character, int length) {
        this.character = character;
        this.length = length;
    }

    public static DefaultFontInfo getDefaultFontInfo(char c) {
        foreach (DefaultFontInfo dFI in GetFieldValues().Values) {
            if (dFI.getCharacter() == c) return dFI;
        }
        return DefaultFontInfo.DEFAULT;
    }

    public static Dictionary<string, DefaultFontInfo> GetFieldValues()
    {
        return new DefaultFontInfo('4',0).GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .ToDictionary(f => f.Name,
                f => (DefaultFontInfo) f.GetValue(null));
    }
    
    public char getCharacter() {
        return this.character;
    }

    public int getLength() {
        return this.length;
    }

    public int getBoldLength() {
        if (this.character == DefaultFontInfo.SPACE.character) return this.getLength();
        return this.length + 1;
    }
    }
}