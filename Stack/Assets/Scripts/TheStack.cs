using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour {

	public Text scoreText;
	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	public GameObject EndPanel;

	private const float BOUNDS_SIZE = 3.5f;				// size of the sway
	private const float STACK_MOVING_SPEED = 5.0f;		// how fast it moves
	private const float ERROR_MARGIN = 0.20f;			// error threshold of tile placement
	private const float STACK_BOUNDS_GAIN = 0.25f;		// incr cube bounds slowly
	private const int COMBO_START_GAIN = 4;				// how much you need to combo before gaining size

	private GameObject[] theStack;						// array of cubes
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);	// have size of cube bounds

	private int stackIndex;								// global stack Index; know which one we are at
	private int scoreCount = 0;							// keep track of score
	private int combo = 0;								// track of combo

	private float tileTransition = 0.0f;
	private float tileSpeed = 2.5f;
	private float secondaryPosition;

	private bool isMovingOnX = true;					// is it moving in x position
	private bool gameOver = false;						// is it game over?

	private Vector3 desiredPosition;					// where we want the cube to be placed
	private Vector3 lastTilePosition;					// where last cube was placed

	// Use this for initialization
	private void Start () {
		theStack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {			// initialize stack into array
			theStack [i] = transform.GetChild (i).gameObject;
			ColorMesh (theStack [i].GetComponent<MeshFilter> ().mesh);
		}
		stackIndex = transform.childCount - 1;
	}

	private void CreateRubble(Vector3 pos, Vector3 scale) 			// create debris
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();

		go.GetComponent<MeshRenderer> ().material = stackMat;
		ColorMesh(go.GetComponent<MeshFilter>().mesh);
	}
	
	// Update is called once per frame
	private void Update () {
		if (gameOver)											// dont try to spawn more tiles after game over
			return;
		
		if (Input.GetMouseButtonDown (0)) {						// click to spawn tile and incr score
			if (PlaceTile ()) {
				SpawnTile ();
				scoreCount++;
				scoreText.text = scoreCount.ToString ();
			} else {											// if you couldn't place tile, game over
				EndGame ();
			}
		}

		MoveTile ();

		// Move the Stack
		transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

	// Sway the tile until game over
	private void MoveTile() {
		tileTransition += Time.deltaTime * tileSpeed;
		if (isMovingOnX)
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		else
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransition) * BOUNDS_SIZE);
		
	}

	// spawn new tile at top of stack
	private void SpawnTile() {
		lastTilePosition = theStack [stackIndex].transform.localPosition;
		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;

		// place in new position at correct position and sclae
		desiredPosition = (Vector3.down) * scoreCount;
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		theStack [stackIndex].transform.localScale = (new Vector3 (stackBounds.x, 1, stackBounds.y));

		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
	}

	// place tile when moving in either x or z directions
	private bool PlaceTile() {
		Transform t = theStack [stackIndex].transform;

		// condition for moving on x direction
		// computation to determine location from previous and currently placed tile
		if (isMovingOnX) {
			float deltaX = lastTilePosition.x - t.position.x;
			// if difference is greater than threshold, cut the current tile
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) {
				// CUT THE TILE
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if (stackBounds.x <= 0)
					return false;

				float middle = lastTilePosition.x + t.localPosition.x / 2;
				t.localScale = (new Vector3 (stackBounds.x, 1, stackBounds.y));
				CreateRubble (
					new Vector3 ((t.position.x > 0) 
						? t.position.x + (t.localScale.x / 2)
						: t.position.x - (t.localScale.x / 2)
						, t.position.y, t.position.z),
					new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
				);
				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
			} else {										// incr combo, update position, and grow bounds 
				if (combo > COMBO_START_GAIN) {
					stackBounds.x += STACK_BOUNDS_GAIN;
					if (stackBounds.x > BOUNDS_SIZE)			// dont want to get bigger than start/initial
						stackBounds.x = BOUNDS_SIZE;
					
					float middle = lastTilePosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
			// condition for moving in z direction; very similar to x direction
		} else {
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {
				// CUT THE TILE
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0)
					return false;

				float middle = lastTilePosition.z + t.localPosition.z / 2;
				t.localScale = (new Vector3 (stackBounds.x, 1, stackBounds.y));
				CreateRubble
				(
					new Vector3 (t.position.x
						, t.position.y
						, (t.position.z > 0)
						? t.position.z + (t.localScale.z/2)
						: t.position.z + (t.localScale.z/2)),
					new Vector3 (t.localScale.z, 1, Mathf.Abs (deltaZ))
				);
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
			} else {
				if (combo > COMBO_START_GAIN) {
					stackBounds.y += STACK_BOUNDS_GAIN;
					if (stackBounds.y > BOUNDS_SIZE)
						stackBounds.y = BOUNDS_SIZE;
					
					float middle = lastTilePosition.z + t.localPosition.z / 2;
					t.localScale = (new Vector3 (stackBounds.x, 1, stackBounds.y));
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}

		// ternary for changing direction
		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;
		isMovingOnX = !isMovingOnX;

		return true;
	}

	private void ColorMesh(Mesh mesh) 
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);

		for (int i = 0; i < vertices.Length; i++) 
			colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);

		mesh.colors32 = colors;
	}

	private Color32 Lerp4( Color32 a , Color32 b, Color32 c, Color32 d, float t) 
	{
		if (t < 0.33f)
			return Color.Lerp (a, b, t / 0.33f);
		else if (t < 0.66f)
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		else
			return Color.Lerp (c, d, (t - 0.66f) / 0.66f);
	}

	private void EndGame() {
		if (PlayerPrefs.GetInt ("score") < scoreCount) {		// setting the high score
			PlayerPrefs.SetInt ("score", scoreCount);
		}
		gameOver = true;
		EndPanel.SetActive (true);
		theStack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}
}
