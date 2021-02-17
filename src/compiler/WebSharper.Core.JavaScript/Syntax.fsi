// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

/// Defines the JavaScript abstract syntax tree.
module WebSharper.Core.JavaScript.Syntax

/// Represents JavaScript identifiers.
type Id = string

/// Represents JavaScript labels.
type Label = string

/// Represents JavaScript regular expression literals verbatim.
type Regex = string

/// Represents JavaScript prefix operators.
type UnaryOperator =
    | ``!`` = 0
    | ``++`` = 1
    | ``+`` = 2
    | ``--`` = 3
    | ``-`` = 4
    | ``delete`` = 5
    | ``typeof`` = 6
    | ``void`` = 7
    | ``~`` = 8

/// Represents JavaScript postfix operators.
type PostfixOperator =
    | ``++`` = 0
    | ``--`` = 1

/// Represents JavaScript binary operators.
type BinaryOperator =
    | ``!==`` = 0
    | ``!=`` = 1
    | ``%=`` = 2
    | ``%`` = 3
    | ``&&`` = 4
    | ``&=`` = 5
    | ``&`` = 6
    | ``*=`` = 7
    | ``*`` = 8
    | ``+=`` = 9
    | ``+`` = 10
    | ``,`` = 11
    | ``-=`` = 12
    | ``-`` = 13
    | ``.`` = 14
    | ``/=`` = 15
    | ``/`` = 16
    | ``<<=`` = 17
    | ``<<`` = 18
    | ``<=`` = 19
    | ``<`` = 20
    | ``===`` = 21
    | ``==`` = 22
    | ``=`` = 23
    | ``>=`` = 24
    | ``>>=`` = 25
    | ``>>>=`` = 26
    | ``>>>`` = 27
    | ``>>`` = 28
    | ``>`` = 29
    | ``^=`` = 30
    | ``^`` = 31
    | ``in`` = 32
    | ``instanceof`` = 33
    | ``|=`` = 34
    | ``|`` = 35
    | ``||`` = 36

type SourcePos =
    {
        File : string
        Line : int
        Column : int
        EndLine : int
        EndColumn : int
    }

/// Represents literals.
type Literal =
    | False
    | Null
    | Number of string
    | String of string
    | True

    /// Lifts to an expression.
    static member ( !~ ) : Literal -> Expression

/// Represents JavaScript expressions.
and Expression =
    private
    | Application of E * list<E>
    | Binary      of E * BinaryOperator * E
    | Conditional of E * E * E
    | Constant    of Literal
    | Lambda      of option<Id> * list<Id> * list<S>
    | New         of E * list<E>
    | NewArray    of list<option<E>>
    | NewObject   of list<Id * E>
    | NewRegex    of Regex
    | Postfix     of E * PostfixOperator
    | This
    | Unary       of UnaryOperator * E
    | Var         of Id
    | VarNamed    of Id * string
    | ExprPos     of E * SourcePos
    | ExprComment of E * string
    | ImportFunc

    static member ( + ) : E * E -> E
    static member ( - ) : E * E -> E
    static member ( * ) : E * E -> E
    static member ( / ) : E * E -> E
    static member ( % ) : E * E -> E
    static member ( ^= ) : E * E -> E
    static member ( &== ) : E * E -> E
    static member ( &!= ) : E * E -> E
    static member ( &=== ) : E * E -> E
    static member ( &!== ) : E * E -> E
    static member ( &< ) : E * E -> E
    static member ( &> ) : E * E -> E
    static member ( &<= ) : E * E -> E
    static member ( &>= ) : E * E -> E
    static member ( ? ) : E * string -> E
    static member ( ! ) : E -> E
    static member ( ~+ ) : E -> E
    static member ( ~- ) : E -> E

    member Delete : E
    member Void : E
    member TypeOf : E
    member In : E -> E
    member InstanceOf : E -> E

    member Item : E -> E with get
    member Item : list<E> -> E  with get

/// JavaScript statements.
and Statement =
    | Block        of list<S>
    | Break        of option<Label>
    | Continue     of option<Label>
    | Debugger     
    | Do           of S * E
    | Empty        
    | For          of option<E> * option<E> * option<E> * S
    | ForIn        of E * E * S
    | ForVarIn     of Id * option<E> * E * S
    | ForVars      of list<Id * option<E>> * option<E> * option<E> * S
    | If           of E * S * S
    | Ignore       of E
    | Labelled     of Label * S
    | Return       of option<E>
    | Switch       of E * list<SwitchElement>
    | Throw        of E
    | TryFinally   of S * S
    | TryWith      of S * Id * S * option<S>
    | Vars         of list<Id * option<E>>
    | While        of E * S
    | With         of E * S
    | Function     of Id * list<Id> * list<S>
    | StatementPos of S * SourcePos 
    | StatementComment of S * string
    | Import       of option<string> * Id * string 

/// Represents switch elements.
and SwitchElement =
    | Case of E * list<S>
    | Default of list<S>

and private E = Expression
and private S = Statement
     
val (|Application|_|) : E -> (E * list<E>                    ) option
val (|Binary     |_|) : E -> (E * BinaryOperator * E         ) option
val (|Conditional|_|) : E -> (E * E * E                      ) option
val (|Constant   |_|) : E -> (Literal                        ) option
val (|Lambda     |_|) : E -> (option<Id> * list<Id> * list<S>) option
val (|New        |_|) : E -> (E * list<E>                    ) option
val (|NewArray   |_|) : E -> (list<option<E>>                ) option
val (|NewObject  |_|) : E -> (list<Id * E>                   ) option
val (|NewRegex   |_|) : E -> (Regex                          ) option
val (|Postfix    |_|) : E -> (E * PostfixOperator            ) option
val (|This       |_|) : E -> (unit                           ) option
val (|Unary      |_|) : E -> (UnaryOperator * E              ) option
val (|Var        |_|) : E -> (Id                             ) option
val (|VarNamed   |_|) : E -> (Id * string                    ) option
val (|ExprPos    |_|) : E -> (Expression * SourcePos         ) option
val (|ExprComment |_|) : E -> (Expression * string) option
val (|ExprComment |_|) : E -> (Expression * string) option
val (|ImportFunc |_|) : E -> (unit                           ) option

val Application : E * list<E>                     -> E
val Binary      : E * BinaryOperator * E          -> E
val Conditional : E * E * E                       -> E
val Constant    : Literal                         -> E
val Lambda      : option<Id> * list<Id> * list<S> -> E
val New         : E * list<E>                     -> E
val NewArray    : list<option<E>>                 -> E
val NewObject   : list<Id * E>                    -> E
val NewRegex    : Regex                           -> E
val Postfix     : E * PostfixOperator             -> E
val This        :                                    E
val Unary       : UnaryOperator * E               -> E
val Var         : Id                              -> E
val VarNamed    : Id * string                     -> E
val ExprPos     : Expression * SourcePos          -> E
val ExprComment : Expression * string             -> E
val ImportFunc  :                                    E
                                                                           
val (|Block       |_|) : S -> (list<S>                                         ) option                  
val (|Break       |_|) : S -> (option<Label>                                   ) option                        
val (|Continue    |_|) : S -> (option<Label>                                   ) option                        
val (|Debugger    |_|) : S -> (unit                                            ) option           
val (|Do          |_|) : S -> (S * E                                           ) option                
val (|Empty       |_|) : S -> (unit                                            ) option           
val (|For         |_|) : S -> (option<E> * option<E> * option<E> * S           ) option                                                
val (|ForIn       |_|) : S -> (E * E * S                                       ) option                    
val (|ForVarIn    |_|) : S -> (Id * option<E> * E * S                          ) option                                 
val (|ForVars     |_|) : S -> (list<Id * option<E>> * option<E> * option<E> * S) option                                                           
val (|If          |_|) : S -> (E * S * S                                       ) option                    
val (|Ignore      |_|) : S -> (E                                               ) option            
val (|Labelled    |_|) : S -> (Label * S                                       ) option                    
val (|Return      |_|) : S -> (option<E>                                       ) option                    
val (|Switch      |_|) : S -> (E * list<SwitchElement>                         ) option                                  
val (|Throw       |_|) : S -> (E                                               ) option            
val (|TryFinally  |_|) : S -> (S * S                                           ) option                
val (|TryWith     |_|) : S -> (S * Id * S * option<S>                          ) option                                 
val (|Vars        |_|) : S -> (list<Id * option<E>>                            ) option                               
val (|While       |_|) : S -> (E * S                                           ) option                
val (|With        |_|) : S -> (E * S                                           ) option                
val (|Function    |_|) : S -> (Id * list<Id> * list<S>                         ) option                
val (|StatementPos|_|) : S -> (S * SourcePos                                   ) option                         
val (|StatementComment|_|) : S -> (S * string) option                         

val Block        : list<S>                                          -> S
val Break        : option<Label>                                    -> S
val Continue     : option<Label>                                    -> S
val Debugger     :                                                     S
val Do           : S * E                                            -> S
val Empty        :                                                     S
val For          : option<E> * option<E> * option<E> * S            -> S
val ForIn        : E * E * S                                        -> S
val ForVarIn     : Id * option<E> * E * S                           -> S
val ForVars      : list<Id * option<E>> * option<E> * option<E> * S -> S
val If           : E * S * S                                        -> S
val Ignore       : E                                                -> S
val Labelled     : Label * S                                        -> S
val Return       : option<E>                                        -> S
val Switch       : E * list<SwitchElement>                          -> S
val Throw        : E                                                -> S
val TryFinally   : S * S                                            -> S
val TryWith      : S * Id * S * option<S>                           -> S
val Vars         : list<Id * option<E>>                             -> S
val While        : E * S                                            -> S
val With         : E * S                                            -> S
val Function     : Id * list<Id> * list<S>                          -> S
val StatementPos : S * SourcePos                                    -> S
val StatementComment : S * string                                   -> S

/// Represents complete programs.
type Program = list<S>

/// Maps over the immediate sub-nodes of an expression.
val TransformExpression : (E -> E) -> (S -> S) -> E -> E

/// Maps over the immediate sub-nodes of a statement.
val TransformStatement : (E -> E) -> (S -> S) -> S -> S

/// Performs a fold over the immediate sub-nodes of an expression.
val FoldExpression<'T> : ('T -> E -> 'T) -> ('T -> S -> 'T) -> 'T -> E -> 'T

/// Performs a fold over the immediate sub-nodes of a statement.
val FoldStatement<'T> : ('T -> E -> 'T) -> ('T -> S -> 'T) -> 'T -> S -> 'T

/// Performs a variable renaming pass that closes the expression.
/// All global references are rewired to explicit field lookups on the
/// global object, represented by the given identifier.
val Close : Id -> E -> E

/// Performs simple optimizations.
val Optimize : E -> E

/// Remove source position wrapper.
val RemoveOuterStatementSourcePos : S -> S
