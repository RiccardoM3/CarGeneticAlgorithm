using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Carmanager : MonoBehaviour {

	//Initialising variables
	public float clickedMultiplier = 25f;
	public float timeMultiplier = 2f;
	public float resetFitness = 500f;
	public GameObject mainCam;
	public Text fitnessText;
	public Text statsText;

	private bool running = false;
	public GameObject carPrefab;

	public int population = 20;
	public static int generationNumber = 0;
	private int[] layers = new int[] { 5, 4, 3, 2 }; //3 input and 2 output
	private List<NeuralNetwork> nets;
	private List<Car> carList = null;

	// Use this for initialization
	void Start () {
		Time.timeScale = timeMultiplier;
		fitnessText = fitnessText.GetComponent<Text> ();
		statsText = statsText.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Running is when the cars are going
		if (running == false) {
			if (generationNumber == 0)
			{
				//This only happens upon startup
				InitCarNeuralNetworks();
			}

			else
			{
				//This happens every time a run ends
				nets.Sort();
				for (int i = 0; i < population / 2; i++)
				{
					nets[i] = new NeuralNetwork(nets[i+(population / 2)]);
					nets[i].Mutate();

					nets[i + (population / 2)] = new NeuralNetwork(nets[i + (population / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
				}

				for (int i = 0; i < population; i++)
				{
					nets[i].SetFitness(0f);
				}
			}
			//This is what starts the next run
			generationNumber++;

			running = true;
			CreateCarBodies();
		}
	
		//Check when all the cars have crashed
		//When true, turn running to false
		int counter = 0;
		for (int i = 0; i < population - 1; i++) {
			if (carList [i].GetCrashed () == true) {
				counter += 1;
			}
			if (counter == population - 1) {
				running = false;
			}
		}

		//Search for highest fitness neural net index
		int highestFitness = 0;
		for (int i = 1; i < population; i++)
		{
			if (nets [highestFitness].GetFitness () < nets [i].GetFitness ())
			{
				highestFitness = i;
			}
		}

		//Update text and Camera
		if (generationNumber > 1) {
			fitnessText.text = "Highest Fitness: " + nets [highestFitness].GetFitness ().ToString ();
			statsText.text = "Engine: " + carList [highestFitness].GetStats (1).ToString () + " Turn: " + (carList [highestFitness].GetStats (0)).ToString ();
		}
		mainCam.transform.position = new Vector3 (carList [highestFitness].gameObject.transform.position.x, carList [highestFitness].gameObject.transform.position.y, mainCam.transform.position.z);

		//Click to Speed up
		if (Input.GetMouseButtonDown (0)) {
			if (Time.timeScale == timeMultiplier) {
				Time.timeScale = clickedMultiplier;
			} else {
				Time.timeScale = timeMultiplier;
			}
		}

		//Restart if max fitness is above threshold
		if (nets [highestFitness].GetFitness () > resetFitness) {
			print (generationNumber);
			running = false;
		}
	}

	private void CreateCarBodies()
	{
		if (carList != null)
		{
			for (int i = 0; i < carList.Count; i++)
			{
				GameObject.Destroy(carList[i].gameObject);
			}

		}

		carList = new List<Car>();

		for (int i = 0; i < population; i++)
		{
			Car indivCar = ((GameObject)Instantiate(carPrefab)).GetComponent<Car>();
			indivCar.Init(nets[i]);
			carList.Add(indivCar);
		}

	}

	void InitCarNeuralNetworks()
	{
		nets = new List<NeuralNetwork>();

		for (int i = 0; i < population; i++)
		{
			NeuralNetwork net = new NeuralNetwork(layers);
			net.Mutate();
			nets.Add(net);
		}
	}

}
