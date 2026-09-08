using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogDisplay : MonoBehaviour
{
    [SerializeField] private Button dialogButton;
    private GameObject dialogPanel;
    private TextMeshProUGUI dialogText;
    private Transform dialogPanelTransform;
    

    public void Start()
    {
        DialogRunner.Instance.OnNodeChanged += HandleNodeChanged;
        DialogRunner.Instance.OnDialogEnded += HideDialog;
        dialogPanel = transform.Find("DialogPanel").gameObject;
        dialogText = dialogPanel.transform.Find("DialogText").GetComponent<TMPro.TextMeshProUGUI>();
        dialogPanelTransform = dialogPanel.transform;
        dialogPanel.SetActive(false);
    }
    public void HandleNodeChanged(DialogNode node, List<DialogChoice> choices)
    {
        ShowDialog(node.text);
        DestroyExistingButtons();

        for (int i = 0; i < choices.Count; i++)
        {
            int choiceIndex = i;

            GameObject buttonObj = Instantiate(dialogButton.gameObject, dialogPanelTransform);
            buttonObj.name = "DialogButton" + i;

            Button button = buttonObj.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = choices[i].text;

            button.onClick.AddListener(() => OnChoiceSelected(choices[choiceIndex]));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogPanelTransform as RectTransform);
    }
    public void DestroyExistingButtons()
    {
        for (int i = dialogPanelTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = dialogPanelTransform.GetChild(i);
            if (child.name.StartsWith("DialogButton"))
            {
                Destroy(child.gameObject);
            }
        }
    }
    public void OnChoiceSelected(DialogChoice choice)
    {
        DialogRunner.Instance.SelectChoice(choice);
    }
    public void ShowDialog(string message)
    {
        dialogText.text = message;
        dialogPanel.SetActive(true);
    }

    public void HideDialog()
    {
        DestroyExistingButtons();
        dialogPanel.SetActive(false);
    }
}