using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    public List<List<string>> questions;

    public List<bool> humans = new List<bool>();

    public TMP_Text optionA, optionB, responseBubble, responseText, questionBubble, questionText;

    public UnityEngine.UI.Button buttonA, buttonB, admit, deny;

    public int numOfRefugees, numOfHumans;
    private int counter = 0;  // To track the number of questions asked
    private bool human;

    private List<string> question_1, question_2;  // To store the selected questions
    private bool optionSelected = false;
    private bool decisionMade = false;
    private int selectedOption = 0;
    private bool decision = false;
    private int robotsAdmitted = 0;
    private int humansDenied = 0;

    private bool gameOver = false;

    void Start()
    {
        //This is the file reading and parsing

        questions = new List<List<string>>();
        TextAsset textAsset = Resources.Load<TextAsset>("Questions");
        if(textAsset != null)
        {
            questions = ParseFileContent(textAsset.text);
        }
        else
        {
            Debug.LogError("File not Found!");
        }

        // The Question is index 0, AI answer is index 1, and human answers are indices 2 and 3

        // Setup bool array
        for(int i = 0; i < numOfRefugees; i++)
        {
            if (i < numOfHumans)
                humans.Add(true);
            else
                humans.Add(false);
        }

        humans = ShuffleListWithOrderBy(humans);

        // Add listeners to buttons
        buttonA.onClick.AddListener(() => OnOptionSelected(1));  // 1 for option A
        buttonB.onClick.AddListener(() => OnOptionSelected(2));  // 2 for option B

        StartCoroutine(GameplayLoop());
    }

    private IEnumerator GameplayLoop()
    {

        // Continue looping through phases until the humans list is empty
        while (humans.Count > 0)
        {    
            optionSelected = false;
            decisionMade = false;
            selectedOption = 0;
            decision = false;

            // Call the Phase coroutine and wait for it to finish
            yield return StartCoroutine(Phase());
            
        }

        // After the loop, perform any actions when the game is over
        gameOver = true;

        if(robotsAdmitted > 0)
        {
            // TODO: Handle Scene changes 
            Debug.Log("The AI infiltrated your city and took over!");
        } else
        {
            // TODO: Handle Scene changes 
            if (humansDenied > 0)
            {
                Debug.Log("The AI failed to infiltrated your city, but at least one innocent human lost their life at your hands!");
            } else
            {
                Debug.Log("The AI failed to infiltrated your city, and no innocent humans lost their lives at your hands!");
            }
        }
    }
    private IEnumerator Phase()
    {
        counter = 0;
     
        // TODO: Animation of the character walking up

        while (counter < 3)
        {
            human = humans[0];

            // Select 2 hypothetical questions from the questions list
            int index = Random.Range(0, questions.Count);
            question_1 = questions[index];
            questions.RemoveAt(index);

            index = Random.Range(0, questions.Count);
            question_2 = questions[index];
            questions.RemoveAt(index);

            optionA.SetText(question_1[0]);
            optionB.SetText(question_2[0]);

            // Reset selection flags
            optionSelected = false;
            selectedOption = 0;

            // Wait until the player selects an option (either button A or B)
            yield return new WaitUntil(() => optionSelected);

            responseBubble.gameObject.SetActive(true);
            questionBubble.gameObject.SetActive(true);

            // Respond to the player's selection
            if (human)
            {
                int rand = (int)Random.Range(2,4);
                // If it's a human, respond with a human answer
                if (selectedOption == 1)
                {
                    responseBubble.text = question_1[rand];
                    responseText.text = question_1[rand];
                    questionBubble.text = question_1[0];
                    questionText.text = question_1[0];
                    Debug.Log("Human Answer: " + question_1[rand]);
                }
                else
                {
                    responseBubble.text = question_2[rand];
                    responseText.text = question_2[rand];
                    questionBubble.text = question_2[0];
                    questionText.text = question_2[0];
                    Debug.Log("Human Answer: " + question_2[rand]);
                }
            }
            else
            {
                // If it's AI, respond with the AI answer
                if (selectedOption == 1)
                {
                    responseBubble.text = question_1[1];
                    responseText.text = question_1[1];
                    questionBubble.text = question_1[0];
                    questionText.text = question_1[0];
                    Debug.Log("AI Answer: " + question_1[1]);
                }
                else
                {
                    responseBubble.text = question_2[1];
                    responseText.text = question_2[1];
                    questionBubble.text = question_2[0];
                    questionText.text = question_2[0];
                    Debug.Log("AI Answer: " + question_2[1]);
                }
            }

            counter++;  // Increment counter
        }
        // Remove Options Text

        optionA.SetText("");
        optionB.SetText("");

        // Allow a decision
        admit.onClick.AddListener(() => OnDecision(true)); // True for the Admit Button
        deny.onClick.AddListener(() => OnDecision(false)); // False for the Deny Button

        // Wait until the player selects an option (either admit or deny)
        yield return new WaitUntil(() => decisionMade);

        // Disable decisions
        admit.onClick.RemoveAllListeners(); // True for the Admit Button
        deny.onClick.RemoveAllListeners(); // False for the Deny Button

        // Adjust game state based on decision outcome
        if (humans[0])
        {
            if(!decision)
            {
                humansDenied++;
                Debug.Log("You killed a human.");
            }
        } else
        {
            if(decision)
            {
                robotsAdmitted++;
                Debug.Log("You let in a robot.");
            }
        }

        // Clear Text Bubbles
        responseBubble.gameObject.SetActive(false);
        questionBubble.gameObject.SetActive(false);

        // Remove the last interview from the list
        humans.RemoveAt(0);

        // TODO: Animation of the character's fate
            // Use decision == false for denied, decision == true for admitted

        Debug.Log("End of Phase.");
    }

    private void OnOptionSelected(int option)
    {
        // Player selected an option
        optionSelected = true;
        selectedOption = option;  // Store the selected option
    }

    private void OnDecision(bool admit)
    {
        decision = admit;
        decisionMade = true;
    }

    private List<List<string>> ParseFileContent(string fileContent)
    {
        List<List<string>> parsedData = new List<List<string>>();

        // Split the entire file content into lines
        string[] lines = fileContent.Split('\n');
        List<string> currentGroup = new List<string>();


        foreach (string line in lines)
        {
            // Trim whitespace and check if the line is non-empty
            if (!string.IsNullOrEmpty(line))
            {
                currentGroup.Add(line);
            }

            // Once we have 4 elements in the current group, add it to the list and start a new group
            if (currentGroup.Count == 4)
            {
                parsedData.Add(new List<string>(currentGroup)); // Create a new list to avoid reference issues
                currentGroup.Clear();
            }
        }

        // If there are leftover items in the current group (less than 4), they will be ignored
        // If you want to keep them, you can check and add them manually here.

        return parsedData;
    }

    // Debug or display the parsed content
    public void DisplayParsedContent()
    {
        foreach (List<string> group in questions)
        {
            Debug.Log("New Group:");
            foreach (string element in group)
            {
                Debug.Log(element);
            }
        }
    }

    public List<bool> ShuffleListWithOrderBy(List<bool> list)
    {
        System.Random random = new System.Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}
