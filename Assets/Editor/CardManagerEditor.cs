using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CardManagerEditor : EditorWindow
{


    Vector2 scrollPos, scrollRight;
    GameInformation game;
    private int currentCard = -1;
    private Color col;
    [MenuItem("Tools/Cards Manager")]
    public static void ShowWindows()
    {
        GetWindow<CardManagerEditor>("Manager");

    }
    private void OnEnable()
    {
        game = GameObject.Find("GameInformation").GetComponent<GameInformation>();
        col = GUI.backgroundColor;
    }
    private void OnGUI()
    {
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create Card", GUILayout.Height(30)))
        {
            CreateNewCard();
        }
        GUI.backgroundColor = col;
        EditorGUILayout.BeginHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(310));
        ShowAllCards();
        EditorGUILayout.EndScrollView();
        scrollRight = EditorGUILayout.BeginScrollView(scrollRight);
        if (currentCard < 0) EditorGUILayout.HelpBox("No cards", MessageType.Warning);
        ShowAction();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
        if (GUI.changed)
        {
            if (!game) return;
            SetDirty(game);
        }
    }
    private void ShowAllCards()
    {
       
        for (int i = 0; i < game.allCards.Count; i++)
        {
            EditorGUILayout.BeginVertical("window", GUILayout.MinHeight(50));
            EditorGUILayout.BeginHorizontal();
            game.allCards[i].sprite = (Sprite)EditorGUILayout.ObjectField(game.allCards[i].sprite, typeof(Sprite), allowSceneObjects: false, GUILayout.Height(120), GUILayout.Width(80));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Nedeed resources");
            game.allCards[i].nedeedResourcesType = (CardBase.NedeedType)EditorGUILayout.EnumPopup(game.allCards[i].nedeedResourcesType);
            game.allCards[i].nedeedCount = EditorGUILayout.IntField("Count: ", game.allCards[i].nedeedCount);
            EditorGUILayout.Space();
            game.allCards[i].isDropped = EditorGUILayout.Toggle("Is dropped: ", game.allCards[i].isDropped);
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(35), GUILayout.Height(25)))
            {
                RemoveCard(game.allCards[i].index);
                return;
            }
            GUI.backgroundColor = col;
            if (currentCard == i)
            {
                GUI.backgroundColor = Color.green;
            }

            if (GUILayout.Button("Actions", GUILayout.Height(25)))
            {
                SelectCard(game.allCards[i].index);
            }
            GUI.backgroundColor = col;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
        
    }
    private void ShowAction()
    {
        if (currentCard < 0) return;
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Create Action")) CreateAction();
        if (GUILayout.Button("Create Contition")) CreateCondition();
        GUI.backgroundColor = col;
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < game.allCards[currentCard].AllActions.Count; i++)
        {
            ActionBase action = game.allCards[currentCard].AllActions[i];
            EditorGUILayout.BeginVertical("window", GUILayout.MinHeight(30));
            string miniTitile = "Contition";
            if (action.actionType == ActionBase.ActionType.ActionBase) miniTitile = action.mainEvent.ToString();
            action.IsDisplay = EditorGUILayout.Foldout(action.IsDisplay, miniTitile, true);
            if (game.allCards[currentCard].AllActions[i].actionType == ActionBase.ActionType.ActionBase) DisplayActionBase(action);
            else DisplayCondition((action as ActionCondition));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
    }
    private void DisplayActionBase(ActionBase action)
    {
        if (action.IsDisplay)
        {
            action.mainEvent = (ActionBase.MainEvent)EditorGUILayout.EnumPopup("Main Event", action.mainEvent);
            if(action.mainEvent != ActionBase.MainEvent.PlayAgain && action.mainEvent != ActionBase.MainEvent.SwichWalls && action.mainEvent != ActionBase.MainEvent.Discard && action.mainEvent != ActionBase.MainEvent.MagicEqualsHighestPlayer
                && action.mainEvent  != ActionBase.MainEvent.EqualizeQuarry)
            {
                action.sendValue = EditorGUILayout.IntField("Send Value", action.sendValue);
                action.isForEmpty = EditorGUILayout.Toggle("For Empty", action.isForEmpty);
            }
            if (action.mainEvent == ActionBase.MainEvent.ResourcesEdit)
            {
                action.resourceType = (ActionBase.ResourceType)EditorGUILayout.EnumPopup("Resource", action.resourceType);
            }
            if (action.mainEvent == ActionBase.MainEvent.AddingEdit)
            {
                action.addingResourcesType = (ActionBase.AddingResourcesType)EditorGUILayout.EnumPopup("Adding Resource", action.addingResourcesType);
            }
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove", GUILayout.Height(20)))
            {
                RemoveAction(action);
                game.allCards[currentCard].AllActions.Remove(action);
                return;
            }
            GUI.backgroundColor = col;
        }

    }
    private void DisplayCondition(ActionCondition action)
    {
       
        if (action.IsDisplay)
        {
            EditorGUILayout.LabelField("IF");
            EditorGUILayout.BeginHorizontal();
            action.left = (ActionCondition.Condition)EditorGUILayout.EnumPopup(action.left);
            action.calc = (ActionCondition.Calculator)EditorGUILayout.EnumPopup(action.calc);
            action.right = (ActionCondition.Condition)EditorGUILayout.EnumPopup(action.right);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);
            action.isShowLeft = EditorGUILayout.Foldout(action.isShowLeft, "Condiction true", true);
            if (action.isShowLeft)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Create Action")) action.ActionsForYes.Add(CreateNewAction());
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical("box");
                for (int i = 0; i < action.ActionsForYes.Count; i++)
                {
                    ActionBase ab = action.ActionsForYes[i];
                    ab.IsDisplay = EditorGUILayout.Foldout(ab.IsDisplay, ab.mainEvent.ToString(), true);
                    if (ab.IsDisplay)
                    {
                        ab.mainEvent = (ActionBase.MainEvent)EditorGUILayout.EnumPopup("Main Event", ab.mainEvent);
                        if(ab.mainEvent != ActionBase.MainEvent.EqualizeQuarry)
                        {
                            ab.sendValue = EditorGUILayout.IntField("Send Value", ab.sendValue);
                            ab.isForEmpty = EditorGUILayout.Toggle("For Enemy", ab.isForEmpty);
                        }                        
                        if (ab.mainEvent == ActionBase.MainEvent.ResourcesEdit)
                        {
                            ab.resourceType = (ActionBase.ResourceType)EditorGUILayout.EnumPopup("Resource", ab.resourceType);
                        }
                        if (ab.mainEvent == ActionBase.MainEvent.AddingEdit)
                        {
                            ab.addingResourcesType = (ActionBase.AddingResourcesType)EditorGUILayout.EnumPopup("Adding Resource", ab.addingResourcesType);
                        }
                        EditorGUILayout.Space();
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Remove", GUILayout.Height(20)))
                        {
                            RemoveAction(ab);
                            action.ActionsForYes.Remove(ab);
                            return;
                        }
                        GUI.backgroundColor = col;
                    }
                }
                EditorGUILayout.EndVertical();
            }

            action.isShowRight = EditorGUILayout.Foldout(action.isShowRight, "Condiction false", true);
            if (action.isShowRight)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Create Action")) action.ActionsForNo.Add(CreateNewAction());
                
                GUI.backgroundColor = col;
                EditorGUILayout.BeginVertical("box");
                for (int i = 0; i < action.ActionsForNo.Count; i++)
                {
                    ActionBase ab = action.ActionsForNo[i];
                    ab.IsDisplay = EditorGUILayout.Foldout(ab.IsDisplay, ab.mainEvent.ToString(), true);
                    if (ab.IsDisplay)
                    {
                        ab.mainEvent = (ActionBase.MainEvent)EditorGUILayout.EnumPopup("Main Event", ab.mainEvent);
                        ab.sendValue = EditorGUILayout.IntField("Send Value", ab.sendValue);
                        ab.isForEmpty = EditorGUILayout.Toggle("For Enemy", ab.isForEmpty);
                        if (ab.mainEvent == ActionBase.MainEvent.ResourcesEdit)
                        {
                            ab.resourceType = (ActionBase.ResourceType)EditorGUILayout.EnumPopup("Resource", ab.resourceType);
                        }
                        if (ab.mainEvent == ActionBase.MainEvent.AddingEdit)
                        {
                            ab.addingResourcesType = (ActionBase.AddingResourcesType)EditorGUILayout.EnumPopup("Adding Resource", ab.addingResourcesType);
                        }
                        EditorGUILayout.Space();
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Remove", GUILayout.Height(20)))
                        {
                            RemoveAction(ab);
                            action.ActionsForNo.Remove(ab);
                            return;
                        }
                        GUI.backgroundColor = col;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Condition"))
            {
                for (int i = 0; i < action.ActionsForYes.Count; i++) RemoveAction(action.ActionsForYes[i]);
                for (int i = 0; i < action.ActionsForNo.Count; i++) RemoveAction(action.ActionsForNo[i]);
                RemoveAction(action);
                game.allCards[currentCard].AllActions.Remove(action);
            }
            GUI.backgroundColor = col;
        }
    }
    private void CreateNewCard()
    {
        game.allCards.Add(new CardBase());
        UpdateIndex();
    }
    private void RemoveCard(int id)
    {
        foreach (var action in game.allCards[id].AllActions)
        {
            if(action.actionType == ActionBase.ActionType.ActionCondition)
            {
                foreach (var item in (action as ActionCondition).ActionsForYes) RemoveAction(item);
                foreach (var item in (action as ActionCondition).ActionsForNo) RemoveAction(item);                
            }
            RemoveAction(action);
        }
        game.allCards.RemoveAt(id);
        currentCard = -1;
        UpdateIndex();
    }
    private void RemoveAction(ActionBase action)
    {
        FileUtil.DeleteFileOrDirectory("Assets/Actions/" + action.name + ".asset");
        AssetDatabase.Refresh();
    }
    private void UpdateIndex()
    {
        for (int i = 0; i < game.allCards.Count; i++) game.allCards[i].index = i;
    }
    private void SelectCard(int id)
    {
        currentCard = id;
    }
    private void CreateAction()
    {
        if (currentCard < 0) return;
        ActionBase action = ScriptableObject.CreateInstance<ActionBase>();
        AssetDatabase.CreateAsset(action, "Assets/Actions/" + GetFileName() + ".asset");
        AssetDatabase.SaveAssets();
        action.actionType = ActionBase.ActionType.ActionBase;
        game.allCards[currentCard].AllActions.Add(action);
    }
    private ActionBase CreateNewAction()
    {
        ActionBase action = ScriptableObject.CreateInstance<ActionBase>();
        AssetDatabase.CreateAsset(action, "Assets/Actions/" + GetFileName() + ".asset");
        AssetDatabase.SaveAssets();
        action.actionType = ActionBase.ActionType.ActionBase;        
        return action;
    }
    private void CreateCondition()
    {
        if (currentCard < 0) return;
        ActionCondition action = ScriptableObject.CreateInstance<ActionCondition>();
       

        AssetDatabase.CreateAsset(action, "Assets/Actions/" + GetFileName() + ".asset");
        AssetDatabase.SaveAssets();
        action.actionType = ActionBase.ActionType.ActionCondition;
        game.allCards[currentCard].AllActions.Add(action);

    }
    public string GetFileName()
    {
        string _id = string.Empty;
        for (int i = 0; i < 15; i++)
        {
            int random = Random.Range(0, 36);
            if (random < 26) _id += (char)(random + 65);
            else _id += (random - 26).ToString();
        }
        return _id;
    }
    private void SetDirty(GameInformation obj)
    {
        EditorUtility.SetDirty(obj);
        foreach (var card in game.allCards)
        {
            
            foreach (var action in card.AllActions)
            {
                if(action.actionType == ActionBase.ActionType.ActionCondition)
                {
                    foreach (var item in (action as ActionCondition).ActionsForYes)
                    {
                        EditorUtility.SetDirty(item);
                    }
                    foreach (var item in (action as ActionCondition).ActionsForNo)
                    {
                        EditorUtility.SetDirty(item);
                    }
                    EditorUtility.SetDirty(action);
                }
                else
                {
                    EditorUtility.SetDirty(action);
                }
                
            }
            
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        for (int i = 0; i < game.allCards.Count; i++) game.allCards[i].index = i;
        
    }
}
