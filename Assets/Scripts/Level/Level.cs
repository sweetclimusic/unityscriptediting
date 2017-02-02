using UnityEngine;
using System.Collections.Generic;
using System;
namespace RunAndJump {
	public partial class Level : MonoBehaviour {
		//insert custom property
		[sweetcli.LevelCreator.Time]
		public int _totalTime = 60;


		[SerializeField]
		private int _totalColumns = 25;
		[SerializeField]
		private int _totalRows = 10;

		[SerializeField]
		private List <LevelPiece> levelPieces;
		[SerializeField]
		public int[][] LevelPieceGridPositions;

		[SerializeField]
		private LevelSettings _settings;
		//property to load a levelsettings asset.
		public LevelSettings Settings {
			get { return _settings; }
			set { _settings = value; }
		}


		//not sure the reason for this size.. camera span?
		public const float GridSize = 1.28f;

		private readonly Color _normalColor = Color.grey;  
		private readonly Color _selectedColor = Color.yellow;


		public int TotalTime {
			get { return _totalTime; }
			set { _totalTime = value; }
		}

		public float Gravity {
			get { return ((_settings != null) ? _settings.gravity : 0); }
			set { 
				if(_settings != null) {
					_settings.gravity = value; 
				}
			}
		}

		public AudioClip Bgm {
			get { return (_settings != null) ? _settings.bgm : null; }
			set { 
				if(_settings != null) {
					_settings.bgm = value; 
				}
			}
		}

		public Sprite Background {
			get { return (_settings != null) ? _settings.background : null; }
			set { 
				if(_settings != null) {
					_settings.background = value; 
				}
			}
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
		public LevelPiece setLevelPiece(int col = -1,int row = -1 ,LevelPiece value = null){
			//do not process
			if(col == -1 || row == -1 ||  value == null){
				return null;
			}
			levelPieces.Add (value);
			//store index
			int index = levelPieces.IndexOf (value);
			LevelPieceGridPositions [col] [row] = index;
			return levelPieces[index];
		}
		public LevelPiece getLevelPiece(int col,int row){
			int index = LevelPieceGridPositions [col] [row];

			return index > -1 ? levelPieces[index] : null;
		}
		/// <summary>
		/// Gets or sets the level pieces.
		/// </summary>
		/// <value>The level pieces.</value>
		public List<LevelPiece> LevelPieces{
			get{
				return levelPieces;
			}
			set {
				levelPieces = value;
			}
		}
		public void initiateRows(){
			int unset = -1;
			for (int c = 0; c < TotalColumns; c++)
			{
				LevelPieceGridPositions [c] = new int[TotalRows];
				for (int r = 0; r < TotalRows; r++) {
					LevelPieceGridPositions [c] [r] = unset;
				}
			}
		}
		///<summary>
		///For the given column erase the rows
		///</summary>
		public void unsetRowsForColumn(int colIndex = -1){
			//TODO removeAt from list, and remove from LevelPieceGridPositions
			if(colIndex != -1){
				for (int r = 0; r < TotalRows;r++)
				{
					int index = LevelPieceGridPositions [colIndex] [r];
					LevelPiece piece =  levelPieces[index];
					if(piece){
						UnityEngine.Object.DestroyImmediate (piece.gameObject);
						levelPieces[index] = null;
						LevelPieceGridPositions [colIndex] [r] = -1;
					}
				}
			}
		}
		/// <summary>
		/// Resizes the coordinate grid.
		/// </summary>
		/// <returns>The new coordinate grid.</returns>
		public bool resizeCoordinateGrid(){
			//resize the coordinate grid and initiate rows needed
			int[][] newGrid = new int[TotalColumns][];
			int maxCapacity = TotalColumns + TotalRows * TotalColumns;
			List<LevelPiece> newList = new List<LevelPiece> (maxCapacity);
			int unset = -1;
			for (int c = 0; c < TotalColumns;c++)
			{
				newGrid [c] = new int[TotalRows];
				for (int r = 0; r < TotalRows; r++) {
					newGrid [c] [r] = unset;
				}
			}
			//copy old pieces to new list
			//avoid out of bounds exception
			int firstLen = this.LevelPieceGridPositions.GetLength (0) -1;
			int secLen = this.LevelPieceGridPositions.GetLength (1) -1;
			//as the arrays can be different sizes do a dobl loop
			for (int c = 0; c < TotalColumns; c++) {
				if (c >= firstLen) {
					for (int r = 0; r < TotalRows; r++) {
						if (r >= secLen) {
							int cacheIndex = LevelPieceGridPositions [c] [r];
							newGrid [c] [r] = cacheIndex;
							//update index of prefabs in new resized grid
							newList.Insert(cacheIndex,levelPieces[cacheIndex]);
						}	
					}
				}
			}
			//update levelPieces
			levelPieces.Clear ();
			levelPieces = newList;
			LevelPieceGridPositions = newGrid;
			return true;
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