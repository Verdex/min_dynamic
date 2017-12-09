
using System.Collections.Generic;

using Dalet.Lex;

namespace Dalet.Parse
{
    // TODO error handling info
    // TODO binary expr 
    public interface Expr {}
    public interface Stm {}
    public class Var : Expr
    {
        public string Name { get; set; } 
    }
    public class Symbol
    {
        public IEnumerable<string> Namespace { get; set; }
        public string Value { get; set; }
    }
    public class Lambda : Expr 
    {
        public IEnumerable<Symbol> Parameters { get; set; }
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
        public Symbol VarName { get; set; }
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
    public class Function : Stm 
    {
        public Symbol Name { get; set; }
        public bool IsPublic { get; set; }
        public IEnumerable<Symbol> Parameters { get; set; }
        public IEnumerable<Stm> Body { get; set; }
    }
    public class Field : Stm 
    {
        public Symbol Name { get; set; }
        public bool IsPublic { get; set; }
    }
    public class Class : Stm 
    {
        public Symbol Name { get; set; }
        public IEnumerable<Field> Fields { get; set; }
        public IEnumerable<Function> Methods { get; set; }
        public IENumerable<Class> NestedClasses { get; set; }
        public bool IsPublic { get; set; }
    }
    public class Import : Stm 
    {
        public Symbol Name { get; set; }
    }
    public class Namespace : Stm
    {
        public Symbol Name { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public IEnumerable<Function> Functions { get; set; }
        public IEnumerable<Import> Imports { get; set; }
    }
    public class Assignment : Stm 
    {
        public Symbol Name { get; set; }
        public Expr Expr { get; set; }
    }
    public class Parser
    {
    }
}
