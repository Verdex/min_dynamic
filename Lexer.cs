
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
        private void Next() 
        {
            _index++;    
        }
        private bool TryKeyword(string s)
        {
            var t = _text.Substring( _index );
            if ( t.StartsWith( s ) && Char.IsWhiteSpace( t[s.Length] ) )
            {
                for( var i = 0; i < s.Length; i++ )
                {
                    Next();
                }
                return true;
            }
            return false;
        }
        private bool Try(string s)
        {
            var t = _text.Substring( _index );
            if ( t.StartsWith( s ) )
            {
                for( var i = 0; i < s.Length; i++ )
                {
                    Next();
                }
                return true;
            }
            return false;
        }
        private bool Try(Func<char, bool> p)
        {
            if ( p(Current) )
            {
                Next();
                return true;
            }
            return false;
        }
        private bool Try(char c)
        {
            if ( c == Current )
            {
                Next();
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
            Next();
        }
        private void Is(char c)
        {
            if ( c != Current )
            {
                throw new Exception( _de.Error( _index, $"unknown symbol {Current} encountered" ) );
            }
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
                Next();
            }
            if ( escape )
            {
                throw new Exception( _de.Error( _index, $"String ended with escape" ) );
            }
            Next();
            var end = _index - 1;
            return new Token( TType.String, start, end, new string( c.ToArray() ) );
        }
        
        // TODO need to handle comments
        public IEnumerable<Token> Lex()
        {
            while ( !EndText )
            {
                if ( Try( Char.IsWhiteSpace ) )
                {
                   continue;
                }
                else if( Try( Char.IsDigit ) )
                {
                    yield return Base10Int( Previous );
                }
                else if( Try( '_' ) || Try( Char.IsLetter ) ) // TODO probably a TryKeyword is needed
                {
                //    yield return 
                }
                else if ( Try( "var" ) )
                {
                    yield return new Token( TType.Var, _index - 3, _index - 1, null );
                }
                else if ( Try( "else" ) )
                {
                    yield return new Token( TType.Else, _index - 4, _index -1, null );
                }
                else if ( Try( "if" ) )
                {
                    yield return new Token( TType.If, _index - 2, _index - 1, null );
                }
                else if ( Try( "elseif" ) )
                {
                    yield return new Token( TType.ElseIf, _index - 6, _index - 1, null );
                }
                else if ( Try( "while" ) )
                {
                    yield return new Token( TType.While, _index - 5, _index - 1, null );
                }
                else if ( Try( "foreach" ) )
                {
                    yield return new Token( TType.Foreach, _index - 7, _index - 1, null );
                }
                else if ( Try( "in" ) )
                {
                    yield return new Token( TType.In, _index - 2, _index - 1, null );
                }
                else if ( Try( "break" ) )
                {
                    yield return new Token( TType.Break, _index - 5, _index - 1, null );
                }
                else if ( Try( "continue" ) )
                {
                    yield return new Token( TType.Continue, _index - 8, _index - 1, null );
                }
                else if ( Try( '"' ) )
                {
                    yield return String();
                }
                else if ( Try( "class" ) )
                {
                    yield return new Token( TType.Class, _index - 5, _index - 1, null );
                }
                else if ( Try( '.' ) )
                {
                    yield return new Token( TType.Dot, _index - 1, null );
                }
                else if ( Try( "public" ) )
                {
                    yield return new Token( TType.Public, _index - 6, _index - 1, null );
                }
                else if ( Try( "return" ) )
                {
                    yield return new Token( TType.Return, _index - 5, _index - 1, null );
                }
                else if ( Try( "func" ) )
                {
                    yield return new Token( TType.Function, _index - 5, _index - 1, null );
                }
                else if ( Try( "namespace" ) )
                {
                    yield return new Token( TType.Namespace, _index - 9, _index - 1, null );
                }
                else if ( Try( "import" ) )
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
                else 
                {
                    throw new Exception( _de.Error( _index, $"unknown token encountered {Current}" ) );
                }
                // TODO handle symbols (might need them higher up; also remember to handle symbols that start with keywords) 
            }
        }
    }
}
