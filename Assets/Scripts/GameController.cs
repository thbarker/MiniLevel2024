using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
public class GameController : MonoBehaviour
{
    public List<List<string>> questions;

    public List<bool> humans = new List<bool>();

    public int numOfRefugees, numOfHumans;

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
        List<string> myStrings = new List<string>();
        myStrings.Add("Uh Oh!\n");
        //DisplayParsedContent();

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

        Phase();
    }

    private void Phase()
    {
        // Randomly select from an AI or Human
        bool human = humans[0];

        // Select 2 Hypothetical questions from the questions list to offer the player
        int index = (int)Random.Range(0, questions.Count);
        List<string> question_1 = questions[index];
        questions.RemoveAt(index);
        index = (int)Random.Range(0, questions.Count);
        List<string> question_2 = questions[index];
        questions.RemoveAt(index);

        Debug.Log(question_1[0]);

        // Wait for the player to select one

        // Remove the question from the list

        // Respond to the player's question based on AI/Human

        // Counter ++

        // Repeat this loop while counter is less than 3 
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

    void OnGUI()
    {
        // Optionally display the parsed content in the Unity UI for debugging
        if (questions != null)
        {
            foreach (List<string> group in questions)
            {
                GUILayout.Label("New Group:");
                foreach (string element in group)
                {
                    GUILayout.Label(element);
                }
            }
        }
    }

    public List<bool> ShuffleListWithOrderBy(List<bool> list)
    {
        System.Random random = new System.Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}
