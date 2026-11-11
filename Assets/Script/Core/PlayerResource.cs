using System;
using UnityEngine;

public class PlayerResource 
{
    public int coinAmount;
    public int[][] ResourceCount;
    public void Initialize()
    {
        coinAmount= 100;
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
    public void AddCoins(int amount)
    {
        coinAmount+=amount;
        GameEventSystem.InvokeGoldChanged(coinAmount);
    }
    public void SpendCoins(int amount)
    {
        if (coinAmount < amount) return;
        coinAmount-=amount;
        GameEventSystem.InvokeGoldChanged(coinAmount);
    }
    public int GetResourceCount(ResourceType type)=> ResourceCount[(int)type][0];
    public int GetCoinCount() => coinAmount;
    
}


public enum ResourceType
{
    Iron, Wood, Wool,
    Stone, Milk
    
}
