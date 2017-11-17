
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
                      , RCulry
                      , If
                      , Else
                      , ElseIf
                      , Foreach
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
                else if ( Try( '"' ) )
                {
                    yield return String();
                }
            }
        }
    }
}
