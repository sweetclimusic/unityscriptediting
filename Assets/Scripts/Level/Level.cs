using UnityEngine;

namespace RunAndJump {
	public partial class Level : MonoBehaviour {
		[SerializeField]
		public int _totalTime = 60;
		[SerializeField]
		private float _gravity = -30;
		[SerializeField]
		private AudioClip _bgm;
		[SerializeField]
		private Sprite _background;
		[SerializeField]
		private int _totalColumns;
		[SerializeField]
		private int _totalRows;

		[SerializeField]
		private LevelPiece[] levelPieces;


		//not sure the reason for this size.. camera span?
		public const float GridSize = 1.28f;

		private readonly Color _normalColor = Color.grey;  
		private readonly Color _selectedColor = Color.yellow;


		public int TotalTime {
			get { return _totalTime; }
			set { _totalTime = value; }
		}

		public float Gravity {
			get { return _gravity; }
			set { _gravity = value; }
		}

		public AudioClip Bgm {
			get { return _bgm;}
			set { _bgm = value; }
		}

		public Sprite Background {
			get { return _background; }
			set { _background = value; }
		}
		//poperty for columns and rows
		public int TotalColumns {
			get { return _totalColumns; }
			set { _totalColumns = value; }
		}
		public int TotalRows {
			get { return _totalRows; }
			set { _totalRows = value; }
		}
		//array to store all prefabs in the level.
		public LevelPiece[] LevelPieces{
			set{ levelPieces = value;}
			get{ return levelPieces;}
		}
		//create a grid with the help of Gizoms
		private void GridFrameGizmo(int columns, int rows){
			// 0 to End
			Gizmos.DrawLine ( 
				Vector3.zero ,
				new Vector3(columns * GridSize,0,0)
			);
			// 0 to End Down
			Gizmos.DrawLine (
				Vector3.zero,
				new Vector3(0,rows * GridSize,0)
			);
			//draw from X end to Y UP	
			Gizmos.DrawLine ( 
				new Vector3 (columns * GridSize ,0,0 ),
				new Vector3 (columns * GridSize ,rows * GridSize,0 )
			);
			//draw from Y end to X end
			Gizmos.DrawLine ( 
				new Vector3 (0,rows * GridSize,0),
				new Vector3 (columns * GridSize ,rows * GridSize,0)
			);
		}
		//Draw lines across each section per unit
		private void GridGizmo(int columns,int rows){
			//for the lentgh of our level draw one unit till end level.
			for (int i = 0; i < columns; i++) {
				Gizmos.DrawLine (
					new Vector3(i*GridSize,0,0),
					new Vector3(i*GridSize,rows*GridSize,0)
				);

			}
			for (int j = 0; j < rows; j++) {
				Gizmos.DrawLine (
					new Vector3(0,j*GridSize,0),
					new Vector3(columns*GridSize,j*GridSize,0)
				);
			}
				
		}

		private void OnDrawGizmos(){
			//save old color?
			Color ogColor = Gizmos.color;
			//capture matrix to reposition grids
			Matrix4x4 ogMatrix = Gizmos.matrix;

			//update current Gizmos;
			Gizmos.color = _normalColor;
			Gizmos.matrix = transform.localToWorldMatrix;
			GridFrameGizmo (_totalColumns,_totalRows);
			GridGizmo (_totalColumns,_totalRows);

			//reset color of Gizmos
			Gizmos.color = ogColor;
			Gizmos.matrix = ogMatrix;
		}
		private void OnDrawGizmosSelected(){
			//save old color as its static
			Color ogColor = Gizmos.color;
			//capture matrix to reposition grids
			Matrix4x4 ogMatrix = Gizmos.matrix;
			//update selected Gizmos
			Gizmos.color = _selectedColor;
			Gizmos.matrix = transform.localToWorldMatrix;
			GridFrameGizmo (_totalColumns,_totalRows);
			GridGizmo (_totalColumns,_totalRows);
			Gizmos.color = ogColor;
			Gizmos.matrix = ogMatrix;
		}

		//TODO Solve the following
		//A way to convert 3D coordinates to grid coordinates and vice versa
		//2D grid for world points, raycast hit, convert 3d to 2d grid position

		//A way to know when these coordinates are outside the boundaries of the grid

		//Translate the given vector world space to our 2d grid gizom.
		public Vector3 WorldToGridCoordinates(Vector3 point){
			//cast to int snap inplace based on a whole number.
			return new Vector3 (
				(int)((point.x - transform.position.x) / GridSize),
				(int)((point.y - transform.position.y) / GridSize),
				(int)((point.z - transform.position.z) / GridSize)
			);
		
		}

		//Translate the grid coordiantes to a Vector3 to represent in world space.
		public Vector3 GridToWorldCoordinates(int column, int row){
			return new Vector3 (
				transform.position.x + (column * GridSize + GridSize / 2.0f),
				transform.position.y + (row * GridSize + GridSize / 2.0f),
				0.0f);
		}

		//given a Vector or 2d point cooridinate, find out if it's inside this Gizmos grid
		public bool IsInsideGridBounds(Vector3 point) {
			//first find Min and Max of X and Y
			float minX = transform.position.x;
			float maxX = minX + TotalColumns * GridSize;
			float minY = transform.position.y;
			float maxY = minY + TotalRows * GridSize;
			return (point.x >= minX && point.x <= maxX) && (point.y >= minY && point.y <= maxY);
		}
		//check if inbetween values of _total Columns and row
		public bool IsInsideGridBounds(int x, int y) {
			return (x >= 0 && x < _totalColumns) && (y >= 0 && y < _totalRows);
		}

		//The Snapping is a bit werid as the code maybe shouldn't be here? moving the grid, objects don't truly follow it.
			
	}
}