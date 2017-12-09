
using System;
using System.Collections.Generic;
using System.Linq;

using Dalet.Util;

namespace Dalet.Lex
{
    public enum TType { Int
                      , String
                      , Var
                      , Class
                      , Dot
                      , Public
                      , Symbol
                      , Return
                      , Break
                      , Continue
                      , Function
                      , Namespace
                      , Import
                      , Equal
                      , LessThan
                      , GreaterThan
                      , Add 
                      , Sub
                      , Mult
                      , Div
                      , And
                      , Or
                      , Xor
                      , Not
                      , LParen
                      , RParen
                      , LCurly
                      , RCurly
                      , If
                      , Else
                      , ElseIf
                      , Foreach
                      , In
                      , While
                      , SemiColon
                      , Boolean
                      }

    public class Token
    {
        public IEnumerable<string> Values { get; }
        public TType Type { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }
        public Token( TType t, int si, int ei, params string[] values )
        {
            Type = t;
            Values = values; 
            StartIndex = si;
            EndIndex = ei;
        }
        public Token( TType t, int i, params string[] values )
            : this( t, i, i, values ) { }
    }

    public class Lexer
    {
        private readonly DisplayError _de;
        private readonly string _text;
        private int _index;
        public Lexer( string file, string text )
        {
            _text = text;
            _index = 0;
            _de = new DisplayError( file, text );
        }
        private char Previous => _text[_index - 1];
        private char Current => _text[_index];
        private bool EndText => _text.Length <= _index;
        private bool TryKeyword(string s)
        {
            var t = _text.Substring( _index );
            if ( t.StartsWith( s ) && ( t.Length == s.Length || Char.IsWhiteSpace( t[s.Length] ) ) )
            {
                _index+=s.Length;
                return true;
            }
            return false;
        }
        private bool Try(string s)
        {
            var t = _text.Substring( _index );
            if ( t.StartsWith( s ) )
            {
                _index+=s.Length;
                return true;
            }
            return false;
        }
        private bool Try(Func<char, bool> p)
        {
            if ( p(Current) )
            {
                _index++;
                return true;
            }
            return false;
        }
        private bool Try(char c)
        {
            if ( c == Current )
            {
                _index++;
                return true;
            }
            return false;
        }
        private void Is(Func<char, bool> p)
        {
            if ( !p(Current) )
            {
                throw new Exception( _de.Error( _index, $"unknown symbol {Current} encountered" ) );
            }
            _index++;
        }
        private void Is(char c)
        {
            if ( c != Current )
            {
                throw new Exception( _de.Error( _index, $"unknown symbol {Current} encountered" ) );
            }
        }

        private Token Symbol( char init )
        {
            var start = _index - 1;
            var values = new List<char> { init };

            while( !EndText && (Try( Char.IsLetter ) || Try( Char.IsDigit ) || Try("_")) )
            {
                values.Add( Previous );
            }

            var end = _index;
            return new Token( TType.Symbol, start, end, new string( values.ToArray() ) );
        }

        private Token Base10Int( char init )
        {
            var start = _index - 1;
            var ds = new List<char> { init }; 
            
            while( !EndText && Try( Char.IsDigit ) )
            {
                ds.Add( Previous );
            }
            
            var end = _index;
            return new Token( TType.Int, start, end, new string( ds.ToArray() ) );
        }

        private char EscapeChar()
        {
            switch( Current )
            {
                case '\\':
                    return '\\';
                case '"':
                    return '"';
                case '0':
                    return '\0';
                case 'a':
                    return '\a';
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'v':
                    return '\v';
                default:
                    throw new Exception( _de.Error( _index, $"unknown escape character {Current} encountered" ) );
            }
        }

        private Token String()
        {
            var start = _index - 1;
            var escape = false;
            var c = new List<char>();
            while ( !EndText && (escape || Current != '"') ) 
            {
                if ( escape ) 
                {
                    var ec = EscapeChar();
                    c.Add( ec );
                    escape = false;
                }
                else if ( Current == '\\' )
                {
                    escape = true;
                }
                else
                {
                    c.Add( Current );
                }
                _index++;
            }
            if ( escape )
            {
                throw new Exception( _de.Error( _index, $"String ended with escape" ) );
            }
            _index++;
            var end = _index - 1;
            return new Token( TType.String, start, end, new string( c.ToArray() ) );
        }

        private void LineComment()
        {
            while ( !EndText && Current != '\n' && Current != '\r' )
            {
                Console.WriteLine( Current );
                _index++;
            }
        }

        private void BlockComment()
        {
            var s = false;
            var e = true;
            while ( !EndText && e )
            {
                if ( s && Current == '/' ) 
                {
                    e = false;
                }
                if ( Current == '*' )
                {
                    s = true;
                }
                _index++;
            }
        }
        
        public IEnumerable<Token> Lex()
        {
            while ( !EndText )
            {
                if ( Try( Char.IsWhiteSpace ) )
                {
                   continue;
                }
                else if ( Try( "//" ) )
                { 
                    LineComment(); 
                }
                else if ( Try( "/*" ) )
                {
                    BlockComment();
                }
                else if ( Try( Char.IsDigit ) )
                {
                    yield return Base10Int( Previous );
                }
                else if ( TryKeyword( "true" ) )
                {
                    yield return new Token( TType.Boolean, _index - 4, _index - 1, null );
                }
                else if ( TryKeyword( "false" ) )
                {
                    yield return new Token( TType.Boolean, _index - 5, _index - 1, null );
                }
                else if ( TryKeyword( "var" ) )
                {
                    yield return new Token( TType.Var, _index - 3, _index - 1, null );
                }
                else if ( TryKeyword( "else" ) )
                {
                    yield return new Token( TType.Else, _index - 4, _index -1, null );
                }
                else if ( TryKeyword( "if" ) )
                {
                    yield return new Token( TType.If, _index - 2, _index - 1, null );
                }
                else if ( TryKeyword( "elseif" ) )
                {
                    yield return new Token( TType.ElseIf, _index - 6, _index - 1, null );
                }
                else if ( TryKeyword( "while" ) )
                {
                    yield return new Token( TType.While, _index - 5, _index - 1, null );
                }
                else if ( TryKeyword( "foreach" ) )
                {
                    yield return new Token( TType.Foreach, _index - 7, _index - 1, null );
                }
                else if ( TryKeyword( "in" ) )
                {
                    yield return new Token( TType.In, _index - 2, _index - 1, null );
                }
                else if ( TryKeyword( "break" ) )
                {
                    yield return new Token( TType.Break, _index - 5, _index - 1, null );
                }
                else if ( TryKeyword( "continue" ) )
                {
                    yield return new Token( TType.Continue, _index - 8, _index - 1, null );
                }
                else if ( Try( '"' ) )
                {
                    yield return String();
                }
                else if ( TryKeyword( "class" ) )
                {
                    yield return new Token( TType.Class, _index - 5, _index - 1, null );
                }
                else if ( Try( '.' ) )
                {
                    yield return new Token( TType.Dot, _index - 1, null );
                }
                else if ( TryKeyword( "public" ) )
                {
                    yield return new Token( TType.Public, _index - 6, _index - 1, null );
                }
                else if ( TryKeyword( "return" ) )
                {
                    yield return new Token( TType.Return, _index - 5, _index - 1, null );
                }
                else if ( TryKeyword( "func" ) )
                {
                    yield return new Token( TType.Function, _index - 5, _index - 1, null );
                }
                else if ( TryKeyword( "namespace" ) )
                {
                    yield return new Token( TType.Namespace, _index - 9, _index - 1, null );
                }
                else if ( TryKeyword( "import" ) )
                {
                    yield return new Token( TType.Import, _index - 6, _index - 1, null );
                }
                else if ( Try( '=' ) )
                {
                    yield return new Token( TType.Equal, _index - 1, null );
                }
                else if ( Try( '+' ) )
                {
                    yield return new Token( TType.Add, _index - 1, null );
                }
                else if ( Try( '-' ) )
                {
                    yield return new Token( TType.Sub, _index - 1, null );
                }
                else if ( Try( '*' ) )
                {
                    yield return new Token( TType.Mult, _index - 1, null );
                }
                else if ( Try( '/' ) )
                {
                    yield return new Token( TType.Div, _index - 1, null );
                }
                else if ( Try( '!' ) )
                {
                    yield return new Token( TType.Not, _index - 1, null );
                }
                else if ( Try( "&&" ) )
                {
                    yield return new Token( TType.And, _index - 1, null );
                }
                else if ( Try( "||" ) )
                {
                    yield return new Token( TType.Or, _index - 1, null );
                }
                else if ( Try( '^' ) )
                {
                    yield return new Token( TType.Xor, _index - 1, null );
                }
                else if ( Try( '<' ) )
                {
                    yield return new Token( TType.LessThan, _index - 1, null );
                }
                else if ( Try( '>' ) )
                {
                    yield return new Token( TType.GreaterThan, _index - 1, null );
                }
                else if ( Try( '{' ) )
                {
                    yield return new Token( TType.LCurly, _index - 1, null );
                }
                else if ( Try( '}' ) )
                {
                    yield return new Token( TType.RCurly, _index - 1, null );
                }
                else if ( Try( '(' ) )
                {
                    yield return new Token( TType.LParen, _index - 1, null );
                }
                else if ( Try( ')' ) )
                {
                    yield return new Token( TType.RParen, _index - 1, null );
                }
                else if ( Try( ';' ) )
                {
                    yield return new Token( TType.SemiColon, _index - 1, null );
                }
                else if( Try( '_' ) || Try( Char.IsLetter ) ) 
                {
                    yield return Symbol( Previous );
                }
                else 
                {
                    throw new Exception( _de.Error( _index, $"unknown token encountered {Current}" ) );
                }
            }
        }
    }
}
