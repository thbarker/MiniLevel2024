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
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public List<List<string>> questions;
    public List<bool> humans = new List<bool>();

    public List<GameObject> characters;
    public float characterSpeed = 5;
    public float frequency = 1;
    public float amplitude = 0.25f;
    public float fallSpeed = 1;

    public TMP_Text optionA, optionB, responseBubble, responseText, questionBubble, questionText;
    public UnityEngine.UI.Button buttonA, buttonB, admit, deny;
    public float typingSpeed = 0.05f; // time before displaying between each letter

    public int numOfRefugees;
    public int minHumans, maxHumans;
    private int numOfHumans;
    private bool isHuman;

    private List<string> question_1, question_2;  // To store the selected questions
    private bool optionSelected = false;
    private bool decisionMade = false;
    private int selectedOption = 0;
    private bool decision = false;
    private int robotsAdmitted = 0;
    private int humansDenied = 0;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        numOfHumans = Random.Range(minHumans, maxHumans + 1);

        /* Read and load Questions.txt from resources folder.
         * The question is index 0, AI answer is index 1, and human answers are indices 2 and 3.
         * so for one group: [question, aiAnswer, humanAnswer, humanAnswer]
         */
        questions = new List<List<string>>();
        /* load file at Assets/Resources/Questions.txt */
        TextAsset textAsset = Resources.Load<TextAsset>("Questions");
        if(textAsset == null)
        {
            Debug.LogError("File not Found!");
            return; // exit
        }
        questions = ParseFileContent(textAsset.text);

        // Setup bool array
        for (int i = 0; i < numOfHumans; i++)
            humans.Add(true);
        for (int i = 0; i < numOfRefugees-numOfHumans; i++)
            humans.Add(false);

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
            SceneManager.LoadScene("Lose");
            Debug.Log("The AI infiltrated your city and took over!");
        }
        else if (humansDenied > 0)
        {
            SceneManager.LoadScene("WinAmbiguous");
            Debug.Log("The AI failed to infiltrated your city, but at least one innocent human lost their life at your hands!");
        }
        else
        {
            SceneManager.LoadScene("Win");
            Debug.Log("The AI failed to infiltrated your city, and no innocent humans lost their lives at your hands!");
        }
    }

    private IEnumerator Phase()
    {
        // TODO: Animation of the character walking up
        int characterIndex = Random.Range(0, characters.Count);
        float sinAdder = -Time.time * frequency;
        while (characters[characterIndex].transform.position.x < 0)
        {
            if(Mathf.Sin(Time.time * frequency + sinAdder) < 0)
            {
                sinAdder += Mathf.PI;
            }
            characters[characterIndex].transform.position = new Vector3(characters[characterIndex].transform.position.x + Time.deltaTime * characterSpeed, Mathf.Sin(Time.time * frequency + sinAdder) * amplitude + 0.5f, 0);
            yield return null;
        }

        // Ask questions
        yield return StartCoroutine(AskQuestion());

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
        
        // Remove speech bubbles
        responseBubble.gameObject.SetActive(false);
        questionBubble.gameObject.SetActive(false);

        // Remove the last interview from the list
        humans.RemoveAt(0);

        // TODO: Animation of the character's fate
        // Use decision == false for denied, decision == true for admitted
        if(decision == false)
        {
            float startTime = Time.time;
            while(characters[characterIndex].transform.position.y > -15)
            {
                characters[characterIndex].transform.position -= new Vector3(0, fallSpeed * (Time.time - startTime) * (Time.time - startTime), 0);
                yield return null;
            }
        }
        else
        {
            sinAdder = -Time.time * frequency;
            while (characters[characterIndex].transform.position.x < 15)
            {
                if (Mathf.Sin(Time.time * frequency + sinAdder) < 0)
                {
                    sinAdder += Mathf.PI;
                }
                characters[characterIndex].transform.position = new Vector3(characters[characterIndex].transform.position.x + Time.deltaTime * characterSpeed, Mathf.Sin(Time.time * frequency + sinAdder) * amplitude + 0.5f, 0);
                yield return null;
            }
        }

        characters.RemoveAt(characterIndex);
        Debug.Log("End of Phase.");
    }

    private IEnumerator AskQuestion()
    {
        // Ask 3 questions to NPC
        int playerQuestionsToAsk = 3;
        while (playerQuestionsToAsk > 0)
        {
            isHuman = humans[0];

            // Select 2 hypothetical questions from the questions list
            int index = Random.Range(0, questions.Count);
            question_1 = questions[index];
            questions.RemoveAt(index);

            index = Random.Range(0, questions.Count);
            question_2 = questions[index];
            questions.RemoveAt(index);

            // Display questions in main box
            optionA.SetText(question_1[0]);
            optionB.SetText(question_2[0]);

            // Reset selection flags
            optionSelected = false;
            selectedOption = 0;

            // Wait until the player selects an option (either button A or B)
            yield return new WaitUntil(() => optionSelected);

            // Respond to the player's selection
            if (isHuman)
            {
                int rand = (int)Random.Range(2,4);
                // If it's a human, respond with a human answer
                if (selectedOption == 1)
                {
                    string question = question_1[0];
                    string response = question_1[rand];
                    yield return StartCoroutine(DisplayMessages(question, response));
                    Debug.Log("Human Answer: " + response);
                }
                else
                {
                    string question = question_2[0];
                    string response = question_2[rand];
                    yield return StartCoroutine(DisplayMessages(question, response));
                    Debug.Log("Human Answer: " + response);
                }
            }
            else // If it's AI, respond with the AI answer
            {
                if (selectedOption == 1)
                {
                    string question = question_1[0];
                    string response = question_1[1];
                    yield return StartCoroutine(DisplayMessages(question, response));
                    Debug.Log("AI Answer: " + response);
                }
                else
                {
                    string question = question_2[0];
                    string response = question_2[1];
                    yield return StartCoroutine(DisplayMessages(question, response));
                    Debug.Log("AI Answer: " + response);
                }
            }

            playerQuestionsToAsk--;
        }
    }

    private IEnumerator DisplayMessages(string question, string response)
    {
        // Start typing the first message in responseBubble
        questionBubble.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(questionText, questionBubble, question));
        
        // After the first message finishes, start typing the second message in textBubble
        responseBubble.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(responseText, responseBubble, response));
    }

    private IEnumerator TypeText(TMP_Text textComponent, TMP_Text bubbleTextComponent, string message)
    {
        // Clear previous text
        textComponent.text = "";
        bubbleTextComponent.text = "";

        // Display each letter one at a time
        foreach (char letter in message)
        {
            textComponent.text += letter; // Add one letter at a time
            bubbleTextComponent.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed); // Wait before next letter
        }
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
        List<string> currentGroup = new List<string>();
        string[] lines = fileContent.Split('\n'); // split file content into lines

        foreach (string line in lines)
        {
            // Trim whitespace and check if the line is non-empty
            if (!string.IsNullOrEmpty(line))
                currentGroup.Add(line);

            // Once we have 4 elements in the current group, add it to the list and start a new group
            if (currentGroup.Count == 4)
            {
                /* Create a new list and pass that reference to parsedData */
                parsedData.Add(new List<string>(currentGroup)); // add a copy of the current group
                currentGroup.Clear(); // remove all elements, reuse same list for next group
            }
        }

        return parsedData;
    }

    // Debug or display the parsed content
    public void DisplayParsedContent()
    {
        foreach (List<string> group in questions)
        {
            Debug.Log("New Group:");
            foreach (string element in group)
                Debug.Log(element);
        }
    }

    public List<bool> ShuffleListWithOrderBy(List<bool> list)
    {
        System.Random random = new System.Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}