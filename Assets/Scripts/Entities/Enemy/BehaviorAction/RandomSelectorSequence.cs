using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomSelector", category: "Flow", id: "a1bbf2fc4ed3de98d54a31ade002c9f5")]
public partial class RandomSelectorSequence : Composite
{
    [SerializeReference] public Node Port1;
    [SerializeReference] public Node Port2;
    private List<int> randomizedList;
    int currentIndex;
    
    protected override Status OnStart()
    {
        currentIndex = 0;
        
        randomizedList = Enumerable.Range(0, Children.Count).OrderBy(x=>UnityEngine.Random.value).ToList();
        return StartChild(randomizedList[currentIndex]);
    }

    protected override Status OnUpdate()
    {
        var currentChild = Children[randomizedList[currentIndex]];
        Status childStatus = currentChild.CurrentStatus;
        if (childStatus == Status.Failure)
        {
            return ++currentIndex >= Children.Count ? Status.Failure : StartChild(randomizedList[currentIndex]);
        }
        return childStatus == Status.Running ? Status.Waiting : childStatus;
    }

    protected override void OnEnd()
    {
    }
    
    protected Status StartChild(int childIndex)
    {
        if (currentIndex >= Children.Count)
        {
            return Status.Success;
        }
        var childStatus = StartNode(Children[childIndex]);
        
        return childStatus switch
        {
            Status.Failure => childIndex + 1 >= Children.Count ? Status.Failure : Status.Running,
            Status.Running => Status.Waiting,
            _ => childStatus
        };
    }
}

