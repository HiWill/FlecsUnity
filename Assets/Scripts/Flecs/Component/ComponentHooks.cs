// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using AOT;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs{

[PublicAPI]
public unsafe struct ComponentHooks
{
    public CallbackComponentConstructor? Constructor;
    public CallbackComponentDeconstructor? Deconstructor;
    public CallbackComponentCopy? Copy;
    public CallbackComponentMove? Move;
    public CallbackIterator? OnAdd;
    public CallbackIterator? OnSet;
    public CallbackIterator? OnRemove;

    
    private static void CallbackConstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentConstructorContext(pointer, count);
        data.Hooks.Constructor?.Invoke(ref context);
    }

    
    private static void CallbackDeconstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentDeconstructorContext(pointer, count);
        data.Hooks.Deconstructor?.Invoke(ref context);
    }

    
    private static void CallbackCopy(void* destinationPointer, void* sourcePointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentCopyContext(destinationPointer, sourcePointer, count);
        data.Hooks.Copy?.Invoke(ref context);
    }

    
    private static void CallbackMove(void* destinationPointer, void* sourcePointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentMoveContext(destinationPointer, sourcePointer, count);
        data.Hooks.Move?.Invoke(ref context);
    }

    [MonoPInvokeCallback(typeof(FnPtr_Ecs_iter_tPtr_Void.@delegate))]
    private static void CallbackOnAdd(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnAdd?.Invoke(iterator);
    }

    
    private static void CallbackOnSet(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnSet?.Invoke(iterator);
    }

    
    private static void CallbackOnRemove(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnRemove?.Invoke(iterator);
    }

    internal static void Fill(World world, ref ComponentHooks hooks, ecs_type_hooks_t* desc)
    {
        // FnPtr_VoidPtr_Int_Ecs_type_info_tPtr_Void.@delegate PointerCallbackConstructor  =  CallbackConstructor;
        // desc->ctor.Data.Pointer =Marshal.GetFunctionPointerForDelegate(PointerCallbackConstructor) ;//(delegate* unmanaged[Cdecl]<void*, int, ecs_type_info_t*, void>)PointerCallbackConstructor;//Marshal.GetFunctionPointerForDelegate(new CallbackConstructorDelegate(CallbackConstructor));//&CallbackConstructor;
        //
        // delegate* <void*, int, ecs_type_info_t*, void> PointerCallbackDeconstructor  = & CallbackDeconstructor;
        // desc->dtor.Data.Pointer = (delegate* unmanaged[Cdecl]<void*, int, ecs_type_info_t*, void>)PointerCallbackDeconstructor;
        //
        // delegate* <void*, void*, int, ecs_type_info_t*, void> PointerCallbackCopy = &CallbackCopy;
        // desc->copy.Data.Pointer = (delegate* unmanaged[Cdecl]<void*, void*, int, ecs_type_info_t*, void>)PointerCallbackCopy;
        //
        // delegate* <void*, void*, int, ecs_type_info_t*, void> PointerCallbackMove = &CallbackMove;
        // desc->move.Data.Pointer = (delegate* unmanaged[Cdecl]<void*, void*, int, ecs_type_info_t*, void>)PointerCallbackMove;
        
        FnPtr_Ecs_iter_tPtr_Void.@delegate t = CallbackOnAdd;
        desc->on_add.Data.Pointer = Marshal.GetFunctionPointerForDelegate(t) ;
        
        delegate* <ecs_iter_t*, void> p7 = &CallbackOnSet;
        desc->binding_ctx = (void*)CallbacksHelper.CreateComponentHooksCallbackContext(world, hooks);
    }
}
}