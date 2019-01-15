using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// Note to future self: This still needs work. Menu system, different ways to play etc. I'm happy with it for now though.

namespace Gridlock2
{
    // The main logic and rendery shit.
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // I have no clue what's going on here. Leave it alone.
        #region INotify bullshit
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _cap1;
        public int Cap1
        {
            get { return _cap1; }
            set { _cap1 = value; NotifyPropertyChanged("Cap1"); }

        }
        private int _cap2;
        public int Cap2
        {
            get { return _cap2; }
            set { _cap2 = value; NotifyPropertyChanged("Cap2"); }

        }
        #endregion

        //Where variables live.
        #region Variables
        private bool player1Turn;
        private bool GameOver = false;
        public static SquareState[,] CurrentBoard { get; set; } // Contains 2D array representing the current board state in code.
        int x; int y;  // The x and y positions of the clicked square
        List<Button> buttons = new List<Button>();  // An array containing the button objects that make up the board.    
        #endregion

        //Makes everything
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            MakeGrid();
        }

        #endregion

        //The objects for each player.
        #region Instances
        Player player2 = new Player(SquareState.Player2, SquareState.Compromised2);
        Player player1 = new Player(SquareState.Player1, SquareState.Compromised1);
        #endregion

        //Function that creates the grid.
        public void MakeGrid()
        {
            CurrentBoard = new SquareState[10, 10]; // Creates empty 2D array representing the board.

            for (int row = 0; row < 10; row++)
                for (int col = 0; col < 10; col++)  //Fancy for loop for 2D arrays.
                {
                    //Makes button and adds click event
                    Button GameButton = new Button();
                    GameButton.Click += SquareClick;
                    //Generates a name and sets background color and border thickness.
                    GameButton.Name = "Button" + row.ToString() + col.ToString();
                    GameButton.BorderThickness = new Thickness(0,0,0,0);
                    GameButton.Background = (Brush)FindResource("Button.Static.Background");
                    //Adds button to the correct square on the grid and to a list of button objects.
                    Grid.SetColumn(GameButton, row);
                    Grid.SetRow(GameButton, col);
                    GameBoard.Children.Add(GameButton);
                    buttons.Add(GameButton);
                }
        }

        //Event that records the location of the button clicked.
        void SquareClick(object sender, EventArgs e)
        {
            //Gets the row and column of the button clicked.
            Button b = (Button)sender;
            x = Grid.GetRow(b);
            y = Grid.GetColumn(b);
            //Runs the game function with the new variables.
            Game(x, y);
        }

        //The actual game logic.
        public void Game(int x, int y)
        {
            if (GameOver != true)   // If the game isn't over, run.
            {
                try
                {
                    // It is no longer whoever's turn it was.
                    player1Turn = !player1Turn;

                    //If the clicked square is free, whoever's turn it is gets the square
                    if (CurrentBoard[x, y] == SquareState.Free)
                    {
                        CurrentBoard[x, y] = player1Turn ? SquareState.Player1 : SquareState.Player2;
                    }
                    // If the clicked square is not free, the player gets their turn back.
                    else
                    {
                        player1Turn ^= player1Turn;
                    }

                    // Runs player methods depending on the turn
                    if (player1Turn)
                    {
                        // Checks if any enemy pieces has been captured
                        player2.CaptureCount();
                        // Does weird wpf shit and hopefully displays how many pieces have been captured.
                        this.DataContext = this;
                        Cap1 = player2.captured;
                    }
                    else
                    {
                        // Same as above, different player.
                        player1.CaptureCount();
                        this.DataContext = this;
                        Cap2 = player1.captured;
                    }

                    DisplayGrid();
                }
                //Ignores it when the check methods try to check outside the grid.
                catch
                {
                    IndexOutOfRangeException e;
                    return;
                }
            }
        }

        // Makes the grid nice and pretty for show.
        public void DisplayGrid()
        {
            // Fancy fancy for loop.
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    int coor = 10 * col + row; // Converts fanciness into boring 1D numbers to access button grid.

                    // Sets the color styles depending on whose turn it is.
                    buttons[coor].Style = player1Turn ? (Style)FindResource("GameButton2") : (Style)FindResource("GameButton1");

                    switch (CurrentBoard[row, col])
                    {
                        // Black squares are free (race joke not intended).
                        case SquareState.Free:
                            buttons[coor].Background = Brushes.Black;
                            break;
                        // Player one is blue.
                        case SquareState.Player1:
                            buttons[coor].Background = new SolidColorBrush(Color.FromRgb(32, 102, 148));
                            break;
                        // Player 2 is pink? Red? idk. It looks pretty though.
                        case SquareState.Player2:
                            buttons[coor].Background = new SolidColorBrush(Color.FromRgb(173, 20, 87));
                            break;
                    }
                }
            }
        }
    }
}
