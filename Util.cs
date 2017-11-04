
using System;
using System.Linq;
using System.Collections.Generic;

namespace Dalet.Util
{
    public class DisplayError 
    {
        private List<string> Text { get; }
        private string File { get; }
        public DisplayError( string fileName, string text )
        {
            Text = Lines( text ).ToList();
            File = fileName;
        }
        
        public string Error( int index, string message )
        {
            return Error( index, index, message );
        }

        public string Error( int start, int end, string message )
        { 
            if ( end < start )
            {
                throw new Exception( "The end happens before the start" );
            }
            var line = 1;
            foreach( var t in Text )
            {
                if ( start > t.Length - 1 )
                {
                    start = start - t.Length;
                    end = end - t.Length;
                }
                else if ( t[t.Length - 1] != '\n' && t[t.Length - 1] != '\r' )
                {
                    return $"Error in file:  {File}: line {line}: column {start + 1}\n"  
                           + message + "\n\n" 
                           +  t + "\n" 
                           + new string( '-', start ) 
                           + new string( '^', 1 + end - start );
                }
                else
                {
                    return $"Error in file:  {File}: line {line}: column {start + 1}\n"  
                           + message + "\n\n" 
                           + t 
                           + new string( '-', start ) 
                           + new string( '^', 1 + end - start );
                }
                line++;
            }
            throw new Exception( "error does not occur within limit set by the text length" );
        }

        private static IEnumerable<string> Lines( string t )
        {
            var line = UntilEndLine( t ).ToArray(); 
            yield return new string( line );

            var next = t.Substring( line.Length );
            while( next.Length != 0 )
            {
                line = UntilEndLine( next ).ToArray();
                yield return new string( line );
                next = next.Substring( line.Length );
            }
        }
        
        private static IEnumerable<char> UntilEndLine( string t )
        {
            for( var i = 0; i < t.Length; i++ )
            {
                if ( t.Length - 1 > i && t[i] == '\r' && t[i+1] == '\n' )
                {
                    yield return '\r';
                    yield return '\n';
                    yield break;
                }
                else if ( t[i] == '\r' )
                {
                    yield return '\r';
                    yield break;
                }
                else if ( t[i] == '\n' )
                {
                    yield return '\n';
                    yield break;
                }
                else
                {
                    yield return t[i];
                }
            }
        }
    }
}
