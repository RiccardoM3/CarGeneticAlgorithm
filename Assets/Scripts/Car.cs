using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

	private float distanceTravelled = 0;
	private Vector3 lastPosition;
	private float fitness;

	public GameObject Sensor1;
	public GameObject Sensor2;
	public GameObject Sensor3;
	public GameObject Sensor4;
	public GameObject Sensor5;

	private bool crashed = false;

	private bool initilized = false;

	private NeuralNetwork net;
	private Rigidbody2D rBody;

	private float minspeed = 5f;
	private float speed = 0f;
	public float turnSpeed = 75f;

	public static float[] output;

	void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		lastPosition = transform.position;
		fitness = 0;
	}

	void FixedUpdate ()
	{
		if (initilized == true)
		{
			float[] inputs = new float[5];

			distanceTravelled += Vector3.Distance (lastPosition, transform.position);
			lastPosition = transform.position;

			fitness = distanceTravelled;

			net.SetFitness(fitness);

			RaycastHit2D hit1 = Physics2D.Raycast (Sensor1.transform.position, Quaternion.AngleAxis(30.0f, gameObject.transform.forward) * transform.up);
			RaycastHit2D hit2 = Physics2D.Raycast (Sensor2.transform.position, Quaternion.AngleAxis(15.0f, gameObject.transform.forward) * transform.up);
			RaycastHit2D hit3 = Physics2D.Raycast (Sensor3.transform.position, Quaternion.AngleAxis(0f, gameObject.transform.forward) * transform.up);
			RaycastHit2D hit4 = Physics2D.Raycast (Sensor4.transform.position, Quaternion.AngleAxis(-15.0f, gameObject.transform.forward) * transform.up);
			RaycastHit2D hit5 = Physics2D.Raycast (Sensor5.transform.position, Quaternion.AngleAxis(-30.0f, gameObject.transform.forward) * transform.up);

			Debug.DrawLine (Sensor1.transform.position, hit1.point);
			Debug.DrawLine (Sensor2.transform.position, hit2.point);
			Debug.DrawLine (Sensor3.transform.position, hit3.point);
			Debug.DrawLine (Sensor4.transform.position, hit4.point);
			Debug.DrawLine (Sensor5.transform.position, hit5.point);

			inputs [0] = hit1.distance;
			inputs [1] = hit2.distance;
			inputs [2] = hit3.distance;
			inputs [3] = hit4.distance;
			inputs [4] = hit5.distance;

			output = net.FeedForward(inputs);

			speed = output [1];

			rBody.velocity = (minspeed + speed*4) * transform.up;
			rBody.angularVelocity = turnSpeed * output[0];

			float vel = rBody.velocity.magnitude;
		}
	}

	public void Init(NeuralNetwork net)
	{
		this.net = net;
		initilized = true;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Environment") {
			crashed = true;
			rBody.constraints = RigidbodyConstraints2D.FreezeAll;
		}
	}

	public bool GetCrashed()
	{
		return crashed;
	}

	public float GetStats(int num) {
		return output[num];
	}
}
