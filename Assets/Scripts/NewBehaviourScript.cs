using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Flecs;
using flecs_hub;
using TMPro;
using UnityEngine;
[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{
    public double X;
    public double Y;
}

[StructLayout(LayoutKind.Sequential)]
public struct Velocity : IComponent
{
    public double X;
    public double Y;
}
public unsafe class NewBehaviourScript : MonoBehaviour
{
    public TextMeshProUGUI te;
    // [DllImport("flecs")]
    // public static extern int add(int a, int b);
    public static TextMeshProUGUI teInfo;
 
    public struct callback_ecs_type_hooks_t
    { 
        public callback_ecs_iter_action_t on_add; 
        public callback_ecs_iter_action_t on_Remove;
    }
    public struct callback_ecs_iter_t
    {
        public int a;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void callback_ecs_iter_action_t(ref callback_ecs_iter_t t);
    
    [MonoPInvokeCallback(typeof(callback_ecs_iter_action_t))] 
    public static void TestCallback(ref callback_ecs_iter_t t)
    {
        teInfo.text=(t.a).ToString();
    }
    [MonoPInvokeCallback(typeof(callback_ecs_iter_action_t))] 
    public static void TestCallbackRemove(ref callback_ecs_iter_t t)
    {
        teInfo.text+=" "+(t.a).ToString();
    }
    
    [DllImport("flecs", CallingConvention = CallingConvention.Cdecl)]
    public static extern void callback_ecs_set_hooks_id(ref callback_ecs_type_hooks_t hooks);
    
    private static void Move(Iterator iterator)
    {
        var p = iterator.Field<Position>(1);
        var v = iterator.Field<Velocity>(2);
 

        // Iterate entities for the current group 
        for (var i = 0; i < iterator.Count; i++)
        {
            ref var position = ref p[i];
            var velocity = v[i];

            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }

    private World world;
    void Start()
    {
        teInfo = te;
        // var hooks = new callback_ecs_type_hooks_t
        // {
        //     on_add = TestCallback,
        //     on_Remove = TestCallbackRemove
        // };
        // callback_ecs_set_hooks_id(ref hooks);
        try
        {
            //te.text = (add(1,2)).ToString();
            //flecs.ecs_world_t* Handle=flecs.ecs_init_w_args(0,flecs.Runtime.CStrings.CStringArray(ReadOnlySpan<string>.Empty));
             world = new World(new String []{}); 
            var componentHooks = new ComponentHooks
            { 
                OnAdd = HookCallback,
                OnSet = HookCallback,
                OnRemove = HookCallback
            };
            world.RegisterComponent<Position>();
            world.RegisterComponent<Velocity>();
            world.RegisterSystem<Position, Velocity>(Move);
            for (int i = 0; i < 1999999; i++)
            {
                var bob = world.CreateEntity();
                bob.Set(new Position { X = 0, Y = 0 });
                bob.Set(new Velocity { X = 2, Y = 2 });
            }
            
        }
        catch (Exception e)
        {
            te.text = e.ToString();
            print(e.ToString());
        } 
    }

    private void Update()
    {
        var t = Time.realtimeSinceStartup;
        world.Progress(0);
        te.text = (Time.realtimeSinceStartup - t).ToString();
    }

    private static void HookCallback(Iterator iterator)
    {
        for (var i = 0; i < iterator.Count; i++)
        {
            var eventName = iterator.Event().Name();
            var entityName = iterator.Entity(i).Name(); 
            teInfo.text=("\t" + eventName + ": " + entityName);
        }
    }
    
}
