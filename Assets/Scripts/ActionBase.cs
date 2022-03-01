using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionBase: ScriptableObject
{
    public enum ActionType { ActionBase = 0, ActionCondition = 1 }
    public ActionType actionType;
    public enum MainEvent { Damage = 0, WallVitals = 1, TownViatals = 2, ResourcesEdit = 3, AddingEdit = 4, PlayAgain = 5, Diacard = 6, WallReverce = 7, MagicEqualsHighestPlayer = 8, QuarryLocalEqualEmnemy=9}
    public MainEvent mainEvent;

    public enum ResourceType { Recruts = 0, Gems = 1, Bricks = 2 };
    public ResourceType resourceType;

    public enum AddingResourcesType { Dungeon = 0, Magic = 1, Quarry = 2 }
    public AddingResourcesType addingResourcesType;

    public int sendValue;
    public bool isForEmpty;
    public bool IsDisplay=true;

    public virtual void UseAction()
    {

    }
}