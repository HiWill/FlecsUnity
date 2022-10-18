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
public unsafe class TestFlecs : MonoBehaviour
{
    public TextMeshProUGUI textInfo; 
    public static TextMeshProUGUI textInfoStatic; 
    
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
        textInfoStatic = textInfo; 
        try
        { 
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
            textInfo.text = e.ToString();
            print(e.ToString());
        } 
    }

    private void Update()
    {
        var t = Time.realtimeSinceStartup;
        world.Progress(0);
        textInfo.text = (Time.realtimeSinceStartup - t).ToString();
    }

    private static void HookCallback(Iterator iterator)
    {
        for (var i = 0; i < iterator.Count; i++)
        {
            var eventName = iterator.Event().Name();
            var entityName = iterator.Entity(i).Name(); 
            textInfoStatic.text=("\t" + eventName + ": " + entityName);
        }
    }
    
}
