using System;
using UnityEngine;

public class PlayerResource 
{
    public int goldCoins;
    public int[][] ResourceCount;
    public void Initialize()
    {
        goldCoins= 100;
        ResourceCount = new int[Enum.GetNames(typeof(ResourceType)).Length][];

        for (int i = 0; i < ResourceCount.Length; i++)
        {
            ResourceCount[i] = new int[1];
        }
        AddResource(ResourceType.Iron,200); 
        AddResource(ResourceType.Wood,5);
        AddResource(ResourceType.Stone,100);
    }


    public void InvokeResourceChanged()
    {
        GameEventSystem.InvokeResourceChanged(ResourceCount[(int)ResourceType.Iron][0],
            ResourceCount[(int)ResourceType.Wood][0],
            ResourceCount[(int)ResourceType.Wool][0],
            ResourceCount[(int)ResourceType.Stone][0],
            ResourceCount[(int)ResourceType.Milk][0]);
    }
    public void AddResource(ResourceType type,int amount)
    {
        ResourceCount[(int)type][0]+=amount;
        InvokeResourceChanged();
    }
    public void RemoveResource(ResourceType type,int amount)
    {
      if(ResourceCount[(int)type][0]<amount) return;
      ResourceCount[(int)type][0]-=amount;
      InvokeResourceChanged();
    }
    public int GetResourceCount(ResourceType type)
    {
        return ResourceCount[(int)type][0];
    }
    public void AddCoins(int amount)
    {
        goldCoins+=amount;
        GameEventSystem.InvokeGoldChanged(goldCoins);
    }
    public void SpendCoins(int amount)
    {
        if (goldCoins < amount) return;
        goldCoins-=amount;
        GameEventSystem.InvokeGoldChanged(goldCoins);
    }

    public int getGoldCoins()
    {
        return goldCoins;
    }
}


public enum ResourceType
{
    Iron, Wood, Wool,
    Stone, Milk
    
}
