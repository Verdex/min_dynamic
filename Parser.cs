

using Dalet.Lex;

namespace Dalet.Parse
{
    publc interface Expr {}
    public interface Stm {}
    public class Var : Expr
    {
        public string Name { get; set; } 
    }
    public class VarName 
    {
    }
    public class Foreach : Stm
    {
        public Expr SeqExpr { get; set; }
        public VarName VarName { get; set; }
    }

    public class Parser
    {
    }
}
