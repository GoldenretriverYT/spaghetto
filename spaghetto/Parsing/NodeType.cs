﻿namespace spaghetto.Parsing {
    public enum NodeType
    {
        Return,
        BinaryExpression,
        Token,
        BooleanLiteral,
        Block,
        Continue,
        Break,
        InitVariable,
        AssignVariable,
        UnaryExpression,
        Dot,
        Call,
        IntLiteral,
        FloatLiteral,
        StringLiteral,
        Identifier,
        List,
        If,
        For,
        Cast,
        While,
        FunctionDefinition,
        NativeImport,
        Instantiate,
        ClassDefinition,
        ClassFunctionDefinition,
        DotAssign,
        Export,
        Import,
        ClassPropertyDefinition,
        Repeat,
        Dict
    }
}
