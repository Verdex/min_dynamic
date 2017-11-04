
using System;
using System.Collections.Generic;

using Dalet.Util;

namespace Dalet
{
    public enum TType { Int
                      , String
                      , Var
                      , Class
                      , Util
                      , Dot
                      , Env
                      , Test
                      , Public
                      , Symbol
                      , Return
                      , Yield
                      , Break
                      , Continue
                      , Function
                      , Generator
                      , Constant
                      , Unique
                      , Import
                      , Injector // Environment injection function
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
                      // TODO Injectors might need some characters for programmatic definitions
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
            }
        }
    }
}
