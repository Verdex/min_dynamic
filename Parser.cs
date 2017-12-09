
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
    public class Lambda : Expr 
    {
        public IEnumerable<VarName> Parameters { get; set; }
        public IEnumerable<Stm> Body { get; set; }
    }
    public class Number : Expr
    {
        public int Value { get; set; }
    }
    public class Boolean : Expr
    {
        public bool Value { get; set; }
    }
    public class Str : Expr 
    {
        public string Value { get; set; }
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
    public class Break : Stm { }
    public class Continue : Stm { }
    public class Return : Stm { }
    public class Parser
    {
    }
}
