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

namespace WebSharper.InterfaceGenerator

open System

/// Implements common code generation patterns in binding code.
module Pattern =

    /// Constructs a new class with no constructors and a given
    /// list of static inline members.
    let EnumInlines name (values: seq<string * string>) =
        Class name
        |+> Static 
            [ for (n, c) in values ->
                Getter n TSelf
                |> WithGetterInline c :> _
        ]

    /// Constructs a new class with no constructors and a given
    /// list of static inline members. The values of the members
    /// are strings with their names.
    let EnumStrings name (words: seq<string>) =
        EnumInlines name [for w in words -> (w, Util.Quote w)]
               
    /// Generates properties for JavaSrcipt fields and an object constructor setting all of it.
    let RequiredFields (required: seq<string * Type.Type>) =
        let ctor : Type.Parameters =
            { This = None
              Variable = None
              Arguments =
                [ for (n, t) in required ->
                    (t :> Type.IParameter).Parameter |=> n
                ]
            }
        Instance [
            yield ObjectConstructor ctor :> _ 
            for (n, t) in required -> n =@ t :> _
        ]

    /// Generates setters for optional JavaScript fields and properties named `...Opt` with
    /// getting. 
    let OptionalFields (optional: seq<string * Type.Type>) =
        Instance [
            for (n, t) in optional do
                yield n =! t :> _
                let sn =
                    let name = n.Replace('.', '_')
                    name.Substring(0, 1).ToUpper() + name.Substring 1 + "Opt"
                yield n =@ !?t |> WithSourceName sn :> _
        ]

    /// Generates properties marked obsolete.
    let ObsoleteFields (obsolete: seq<string * Type.Type>) =
        Instance [
            for (n, t) in obsolete -> n =@ t :> _
        ]

    /// Represents the properties of a configuration object type
    /// to be used by the Config function.
    type ConfigProperties =
        {
            Required : seq<string * Type.Type>
            Optional : seq<string * Type.Type>
        }
        static member Empty =
            {
                Required = []
                Optional = []
            } 

    /// Generates a configuration object type.
    let Config (name: string) (properties: ConfigProperties) =
        let ctor : Type.Parameters =
            { This = None
              Variable = None
              Arguments =
                [ for (n, t) in properties.Required ->
                    (t :> Type.IParameter).Parameter |=> n
                ]
            }
        Class name
        |+> Instance [ 
                for (n, t) in properties.Required -> Getter n t :> _
                for (n, t) in properties.Optional -> Property n t :> _
            ]
        |+> Static [ ObjectConstructor ctor ]

    type ConfigObsProperties =
        {
            Required : seq<string * Type.Type>
            Optional : seq<string * Type.Type>
            Obsolete : seq<string * Type.Type>
        }
        static member Empty =
            {
                Required = []
                Optional = []
                Obsolete = []
            } 

    /// Generates a configuration object type with Obsolete support.
    let ConfigObs (name: string) (properties: ConfigObsProperties) =
        Config name {
            Required = properties.Required    
            Optional = properties.Optional
        }
        |+> Instance [ 
                for (n, t) in properties.Obsolete -> Property n t |> Obsolete :> _
            ]
