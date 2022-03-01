using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCondition : ActionBase
{
    public bool isShowLeft, isShowRight;
    public enum Condition
    {
        CurrentWall = 0, CurrentTown = 1, CurrentRecruts = 2, CurrentGems = 3, CurrentBricks = 4, CurrentDungeon = 5, CurrentMagic = 6, CurrentQuarry = 7,
        OtherWall = 8, OtherTown = 9, OtherRecruts = 10, OtherGems = 11, OtherBricks = 12, OtherDungeon = 13, OtherMagic = 14, OtherQuarry = 15, ziro = 16
    }
    public Condition left, right;
    public enum Calculator { Less = 0, Evenly = 1, Great = 2 }
    public Calculator calc;

    public List<ActionBase> ActionsForYes = new List<ActionBase>();
    public List<ActionBase> ActionsForNo = new List<ActionBase>();


    public override void UseAction()
    {

    }
}
