// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs{

[PublicAPI]
public readonly unsafe struct Table
{
    private readonly ecs_world_t* _worldHandle;
    public readonly ecs_table_t* Handle;

    internal Table(ecs_world_t* world, ecs_table_t* handle)
    {
        _worldHandle = world;
        Handle = handle;
    }

    public string String()
    {
        var cString = ecs_table_str(_worldHandle, Handle);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
        return result;
    }
}
}