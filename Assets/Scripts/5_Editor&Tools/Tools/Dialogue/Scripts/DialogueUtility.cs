using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUtility : MonoBehaviour
{
    [SerializeField] SavedDialogue[] dialogues = new SavedDialogue[0];
    SavedDialogue currentDialogue = null;
    int currentIndex;
    [SerializeField] ActionDialogueBinder actionBinder = null;
    [SerializeField] FunctionDialogueBinder funcBinder = null;
    [SerializeField] GenericInteractable interact = null;
    NodeSave currentNode;

    private void Awake()
    {
        currentDialogue = dialogues[currentIndex];
    }
    public void StartDialogue()
    {

        currentNode = currentDialogue.firstNode;

        DialogueUIDisplay.instance.OpenDialog();
        ReadNode();
    }

    void NextDialogue(int answerIndex = -1)
    {
        if (GetNextNode(answerIndex))
        {
            ReadNode();
        }
        else
        {
            FinishDialog();
        }
    }

    void FinishDialog()
    {
        DialogueUIDisplay.instance.CloseDialog();
        currentNode = null;
        if (currentDialogue.nextDialogue)
            NextDialogueIndex();

        if (currentDialogue.repeatDialogue)
            interact.AddToInteractor(Character.instance.GetComponentInChildren<GenericInteractor>());
    }

    public void SkipDelay() => DialogueUIDisplay.instance.ForceEndDelay();

    public void NextDialogueIndex()
    {
        currentIndex += 1;

        if(currentIndex < dialogues.Length)
            currentDialogue = dialogues[currentIndex];
        else
            Debug.LogWarning("qué querés hacer pa");
    }

    public void SetDialogue(SavedDialogue dialogue)
    {
        currentDialogue = dialogue;
    }


    #region PARA LEER LOS NODOS

    void ReadNode()
    {
        switch (currentNode.myType)
        {
            case NodesType.BaseDialogue:
                ReadBaseDialog(currentNode.baseDialogueSD);
                break;

            case NodesType.Answer:
                ReadAnswerDialog(currentNode.answerDialogueSD);
                break;

            case NodesType.Action:
                ReadActionNode(currentNode.actionSD);
                break;

            case NodesType.Requirement:
                ReadRequireNode(currentNode.requireSD);
                break;

            case NodesType.Delay:
                ReadDelayNode(currentNode.delaySD);
                break;

            default:
                break;
        }
    }

    bool GetNextNode(int answerIndex = -1)
    {
        int ID = -1;
        switch (currentNode.myType)
        {
            case NodesType.BaseDialogue:
                ID = currentNode.baseDialogueSD.connect;

                break;
            case NodesType.Answer:
                ID = currentNode.answerDialogueSD.answerConnection[answerIndex];
                break;
            case NodesType.Action:
                ID = currentNode.actionSD.connect;
                break;
            case NodesType.Requirement:
                ID = answerIndex == 0 ? currentNode.requireSD.connectA : currentNode.requireSD.connectB;
                break;
            case NodesType.Delay:
                ID = currentNode.delaySD.connect;
                break;
            default:
                break;
        }
        if (ID == -1)
            return false;

        currentNode = currentDialogue.SearchNodeByID(ID);
        return true;
    }

    void ReadBaseDialog(BaseDialogueSaveData sd)
    {
        DialogueUIDisplay.instance.ShowDialog(sd.dialog, NextDialogue);
    }

    void ReadAnswerDialog(AnswerDialogueSaveData sd)
    {
        Debug.Log("Entra al read");
        DialogueUIDisplay.instance.ShowDialog(sd.dialog, NextDialogue);
        DialogueUIDisplay.instance.ShowAnswers(sd.answerText);
    }

    void ReadActionNode(ActionNodeSaveData sd)
    {
        actionBinder.ActionNode(sd.nodeName);

        NextDialogue();
    }

    void ReadRequireNode(RequireNodeSaveData sd)
    {
        bool condition = funcBinder.CheckCondition(sd.nodeName);

        if (condition) NextDialogue(0);
        else NextDialogue(1);
    }

    void ReadDelayNode(DelayNodeSaveData sd)
    {
        DialogueUIDisplay.instance.DisplayDelay(sd.delayTime, () => NextDialogue(-1));
    }

    #endregion

}
