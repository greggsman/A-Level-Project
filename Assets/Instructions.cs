using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    private Text instructions;
    // Start is called before the first frame update
    void Start()
    {
        instructions = GetComponent<Text>();
        instructions.text = "This simulation renders consumers and producers. Consumers are the organims that have the capacity to reproduce, " +
            "mutate and evolve over time. Their characteristcs which can mutate are strength, speed, stealth, perceptiveness, maximum energy and hunger. Strength and speed," +
            "stealth and perceptivness, maximum energy and hunger are all inversely proportional to each other. If a consumer runs out of energy, it dies. " +
            "These characteristics are stored numerically. Consumers chase other consumers with lower values for strength and can eat them. Consumers can also eat producers " +
            "which spawn at a regular interval (set by you). Consumers come in three types with varying energy values. When a consumer eats another organism, it gains its energy. " +
            "Consumers lose energy by moving and reproducing. If a consumer runs out of energy, it dies. Once a consumer's energy reaches the reproduction threshold, it " +
            "reproduces by binary fission, with a random chance to mutate. \n" +
            "To run the simulation, go to the main menu and create a new save, which will allow you to create a new environment and will save its details" +
            "locally on your computer in the location " + HomeMenu.preferenceFolderPath + " You will have the options to decide on the size of the terrain, " +
            "the inital population of consumers, the population of each producer generation and the initial attribute values for the first generation of consumers. " +
            "For the initial attributes, you are presented with an option such as attribute1/attribute2. Higher values will benefit attribute1, lower values will benefit attribute 2." +
            "You can then run this file in the menu, which will generate your simulation. The on screen timer is synced with the timescale set by the user. Change reproduciton " +
            "threshold, mutation chance and the limit of energy consumers must have before they die. To see the evolution happening, take a snapshot of the simultation which will " +
            "output a csv file to " + HomeMenu.snapshotFolderPath + " which can be viewed in excel. You will notice many 'family trees' which are records of the offspring of the first " +
            "generation of consumer. You might notice one tree has a much greater population than all the others: this is natural selection. Note: the program can only render up to" +
            SimulationSystemManager.consumerLimit + " at a time.";
    }
    public void GoToHomeScreen()
    {
        SceneManager.LoadScene("Home Screen");
    }
}
