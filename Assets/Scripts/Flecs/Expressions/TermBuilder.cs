﻿// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flecs{

public class TermBuilder
{
    private string _entity1 = string.Empty;
    private string _entity2 = string.Empty;
    private string _source = "$This";
    private string _accessModifier = string.Empty; // empty == default == [InOut]
    private string _access = ",";
    private string _prefix = string.Empty;

    protected StringBuilder _stringBuilder;
    protected World _world;

    public TermBuilder(World world)
    {
        _world = world;
        _stringBuilder = new();
    }

    public TermBuilder First<T>()
        where T : unmanaged, IEcsComponent
    => First(_world.GetFlecsTypeName<T>());

    public TermBuilder First(string name)
    {
        _entity1 = name;
        return this;
    }

    public TermBuilder Second<T>()
       where T : unmanaged, IEcsComponent
   => Second(_world.GetFlecsTypeName<T>());

    public TermBuilder Second(string name)
    {
        _entity2 = name;
        return this;
    }

    public TermBuilder Source(string name)
    {
        _source = name;
        return this;
    }

    public TermBuilder Or()
    {
        _access = "||";
        return this;
    }

    public TermBuilder Not()
    {
        _prefix = "!";
        return this;
    }

    public TermBuilder Optional()
    {
        _prefix = "?";
        return this;
    }

    public TermBuilder Term<T>()
        where T : unmanaged, IEcsComponent
    {
        FlushToBuilder();
        First<T>();
        return this;
    }

    public TermBuilder Term<T1, T2>()
        where T1 : unmanaged, IEcsComponent
        where T2 : unmanaged, IEcsComponent
    {
        var termBuilder = new TermBuilder(_world);
        termBuilder.First<T1>();
        termBuilder.Second<T2>();
        return termBuilder;
    }

    public TermBuilder Term()
    {
        FlushToBuilder();
        return this;
    }

    public TermBuilder In()
    {
        _accessModifier = "[in]";
        return this;
    }

    public TermBuilder Out()
    {
        _accessModifier = "[out]";
        return this;
    }

    public TermBuilder None()
    {
        _accessModifier = "[none]";
        return this;
    }

    public TermBuilder InOut() // default, empty == inout
    {
        _accessModifier = "[inout]";
        return this;
    }

    private void Reset()
    {
        _entity1 = string.Empty;
        _entity2 = string.Empty;
        _source = "$This";
        _accessModifier = string.Empty;
        _access = ",";
        _prefix = string.Empty;
    }

    private void FlushToBuilder()
    {
        if (_stringBuilder.Length != 0)
        {
            _stringBuilder.Append(_access).Append(' ');
        }

        if (_access != string.Empty)
        {
            _stringBuilder.Append(_accessModifier).Append(' ');
        }

        _stringBuilder.Append(_prefix).Append(_entity1)
            .Append('(').Append(_source);

        if (_entity2 != string.Empty)
        {
            _stringBuilder.Append(", ").Append(_entity2);
        }

        _stringBuilder.Append(')');
        Reset();
    }

    public string Build()
    {
        FlushToBuilder();
        return _stringBuilder.ToString();
    }
}
}