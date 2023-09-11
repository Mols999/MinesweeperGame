using System;
using System.Drawing;
using System.Windows.Forms;

namespace MinesweeperGame
{
    public partial class Form1 : Form
    {
        // Constants for grid size and mine count
        private const int gridSize = 9;
        private const int mineCount = 10;

        // Game state variables
        private bool isGameOver = false;
        private bool gameStarted = false;
        private DateTime startTime;

        // Arrays to represent the game grid
        private Button[,] gridButtons = new Button[gridSize, gridSize];
        private bool[,] isMine = new bool[gridSize, gridSize];
        private bool[,] isRevealed = new bool[gridSize, gridSize];

        public Form1()
        {
            InitializeComponent();
            InitializeGame(); // Call the game initialization method
        }



        private void InitializeGame()
        {
            // Initialize the grid
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    gridButtons[x, y] = new Button
                    {
                        Name = $"btnMineField_{x}_{y}",
                        Width = 30,
                        Height = 30,
                        Top = x * 30,
                        Left = y * 30,
                        Tag = new Point(x, y), 
                    };
                    gridButtons[x, y].MouseDown += BtnMineField_MouseDown; // Attach event handler for mouse clicks
                    Controls.Add(gridButtons[x, y]); // Add the button to the form's controls
                }
            }

            // Randomly place mines on the grid
            Random random = new Random();
            int minesPlaced = 0;

            while (minesPlaced < mineCount)
            {
                int x = random.Next(0, gridSize);
                int y = random.Next(0, gridSize);

                if (!isMine[x, y])
                {
                    isMine[x, y] = true; // Set the mine at the specified position
                    minesPlaced++;
                }
            }

            // Configure and start the game timer 
            timer1.Interval = 1000; 
            timer1.Tick += timer1_Tick; 
            timer1.Start(); 
            startTime = DateTime.Now; 
            gameStarted = true; 
        }



        // Event handler for mouse clicks on grid cells
        private void BtnMineField_MouseDown(object sender, MouseEventArgs e)
        {
            if (isGameOver)
                return; // If the game is over, ignore further clicks

            Button clickedButton = (Button)sender;
            Point position = (Point)clickedButton.Tag;
            int x = position.X;
            int y = position.Y;

            if (e.Button == MouseButtons.Left)
            {
                // Left-click logic here
                if (!isRevealed[x, y])
                {
                    if (isMine[x, y])
                    {
                        // Handle game over (player clicked on a mine)
                        EndGame(false); 
                    }
                    else
                    {
                        // Reveal the cell and check neighboring mines
                        RevealCell(x, y);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right-click logic here (flagging)
                if (!isRevealed[x, y])
                {
                    // Toggle flag
                    isRevealed[x, y] = !isRevealed[x, y];
                    clickedButton.Text = isRevealed[x, y] ? "F" : ""; // Display "F" for flagged cells
                }
            }
        }



        private void EndGame(bool playerWon)
        {
            isGameOver = true;

            // Reveal all cells
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (isMine[x, y])
                    {
                        gridButtons[x, y].BackColor = Color.Red; // Highlight mines in red
                    }
                    gridButtons[x, y].Enabled = false; // Disable all buttons
                }
            }

            if (playerWon)
            {
                // Display a message box with the elapsed time if the player won
                MessageBox.Show($"Congratulations! You won!\nTime: {lblElapsedTime.Text}");
            }
            else
            {
                // Display a message box indicating game over if the player lost
                MessageBox.Show($"Game Over. You clicked on a mine.\nTime: {lblElapsedTime.Text}");
            }
        }



        private void RevealCell(int x, int y)
        {
            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize || isRevealed[x, y])
                return; // Skip cells that are out of bounds or already revealed

            isRevealed[x, y] = true;

            int neighboringMines = CountNeighboringMines(x, y);

            if (neighboringMines == 0)
            {
                gridButtons[x, y].BackColor = Color.White; 
                gridButtons[x, y].Enabled = false; 

                // Recursively reveal neighboring cells if there are no neighboring mines
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
                        {
                            RevealCell(newX, newY);
                        }
                    }
                }
            }
            else
            {
                gridButtons[x, y].BackColor = Color.White; 
                gridButtons[x, y].Text = neighboringMines.ToString(); // Display the number of neighboring mines
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if (gameStarted && !isGameOver)
            {
                // Update the elapsed time label
                TimeSpan elapsedTime = DateTime.Now - startTime;
                lblElapsedTime.Text = elapsedTime.ToString(@"hh\:mm\:ss");
            }
        }



        private int CountNeighboringMines(int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;
                    if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize && isMine[newX, newY])
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
