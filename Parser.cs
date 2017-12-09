
using System.Collections.Generic;

using Dalet.Lex;

namespace Dalet.Parse
{
    public interface Expr {}
    public interface Stm {}
    public class Var : Expr
    {
        public string Name { get; set; } 
    }
    public class VarName 
    {
        public string Name { get; set; }
    }
    public class Foreach : Stm
    {
        public Expr SeqExpr { get; set; }
        public VarName VarName { get; set; }
        public IEnumerable<Stm> Body { get; set; } 
    }
    public class While : Stm 
    {
        public Expr BoolExpr { get; set; }
        public IEnumerable<Stm> Body { get; set; }
    }
    public class If : Stm
    {
        public Expr BoolExpr { get; set; }
        public IEnumerable<Stm> Body { get; set }
        public Stm Else { get; set; }
    }
    public class Parser
    {
    }
}
