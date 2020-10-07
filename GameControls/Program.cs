using System;
using System.Windows.Forms;
using System.Drawing;
using static ChessUI.ControlsSizesAndPositions;
using ChessLogic;

namespace ChessUI
{
    public enum BacklightColour { Black, Red, Green };

    static class ControlsSizesAndPositions
    {
        public const int startX = 80;
        public const int startY = 24;
        public const int figureSize = 75;
        public const int halfFiguresGap = 3;
        public const int figuresGap = halfFiguresGap * 2;
        public const int squareCount = 8;
        public const int mainMenuBtnXSize = 200;
        public const int mainMenuBtnYSize = 100;
        public const int mainMenuBtnGap = 50;
        public const int mainMenuBtnCount = 2;
        public const int fieldSize = (figureSize + figuresGap) * squareCount - figuresGap;
        public const int fieldMiddleX = startX + fieldSize / 2;
        public const int fieldMiddleY = startY + fieldSize / 2;
        public const int mainMenuStartX = fieldMiddleX - mainMenuBtnXSize / 2;
        public const int mainMenuStartY = fieldMiddleY - ((mainMenuBtnYSize + mainMenuBtnGap) * mainMenuBtnCount - mainMenuBtnGap) / 2;
        public const int rightcolumnStartY = startY + 50;
        public const int rightColumnGap = 20;
        public const int rightColumnX = startX + fieldSize + rightColumnGap;
        public const int connMenuGapX = 30;
        public const int connMenuGapY = 20;
        public const int connMenuLabelSizeX = 50;
        public const int connMenuLabelSizeY = 30;
        public const int connMenuTextBoxSizeX = 250;
        public const int connMenuTextBoxSizeY = 30;
        public const int connMenuLabelX = fieldMiddleX - (connMenuLabelSizeX + connMenuTextBoxSizeX + connMenuGapX) / 2;
        public const int connMenuTextBoxX = connMenuLabelX + connMenuLabelSizeX + connMenuGapX;
        public const int hostLabelY = fieldMiddleY - (connMenuLabelSizeY + connMenuTextBoxSizeY + connMenuGapY * 2 + connMenuBtnSizeY) / 2;
        public const int portLabelY = hostLabelY + connMenuLabelSizeY + connMenuGapY;
        public const int connMenuBtnSizeX = 150;
        public const int connMenuBtnSizeY = 60;
        public const int ConnectBtnX = fieldMiddleX - connMenuBtnSizeX - connMenuGapX / 2;
        public const int ReturnToMenuBtn2X = ConnectBtnX + connMenuBtnSizeX + connMenuGapX;
        public const int connMenuBtnY = portLabelY + connMenuLabelSizeY + connMenuGapY;
    }

    public sealed class FieldSquare
    {
        public FieldSquare()
        {
            Figure = new PictureBox();
            TopBacklight = new PictureBox();
            BottomBacklight = new PictureBox();
            LeftBacklight = new PictureBox();
            RightBacklight = new PictureBox();
        }
        public PictureBox Figure { get; set; }
        public PictureBox TopBacklight { get; set; }
        public PictureBox BottomBacklight { get; set; }
        public PictureBox LeftBacklight { get; set; }
        public PictureBox RightBacklight { get; set; }

        public void Show()
        {
            Figure.Visible = true;
            TopBacklight.Visible = true;
            BottomBacklight.Visible = true;
            LeftBacklight.Visible = true;
            RightBacklight.Visible = true;
        }
        public void Hide()
        {
            Figure.Visible = false;
            TopBacklight.Visible = false;
            BottomBacklight.Visible = false;
            LeftBacklight.Visible = false;
            RightBacklight.Visible = false;
        }
        public void SetBacklight(BacklightColour colour)
        {
            string colourPrefix = "";
            switch (colour)
            {
                case BacklightColour.Black:
                    colourPrefix = "b";
                    break;
                case BacklightColour.Red:
                    colourPrefix = "r";
                    break;
                case BacklightColour.Green:
                    colourPrefix = "g";
                    break;
            }

            TopBacklight.Image = Properties.Resources.ResourceManager.GetObject(colourPrefix + "horizontalbacklight") as Bitmap;
            BottomBacklight.Image = Properties.Resources.ResourceManager.GetObject(colourPrefix + "horizontalbacklight") as Bitmap;
            LeftBacklight.Image = Properties.Resources.ResourceManager.GetObject(colourPrefix + "verticalbacklight") as Bitmap;
            RightBacklight.Image = Properties.Resources.ResourceManager.GetObject(colourPrefix + "verticalbacklight") as Bitmap;
        }
    }

    public static class ControlCollectionExtensions
    {
        public static void Add(this Control.ControlCollection controls, GameControls gameControls)
        {
            controls.Add(gameControls.PlayingField);
            controls.Add(gameControls.RightBar);
            controls.Add(gameControls.CheckmateDialog);
        }

        public static void Add(this Control.ControlCollection controls, PlayingField field)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    controls.Add(field[i, j].Figure);
                    controls.Add(field[i, j].TopBacklight);
                    controls.Add(field[i, j].BottomBacklight);
                    controls.Add(field[i, j].LeftBacklight);
                    controls.Add(field[i, j].RightBacklight);
                }
        }

        public static void Add(this Control.ControlCollection controls, RightBar RightBar)
        {
            controls.Add(RightBar.CurColourLabel);
            controls.Add(RightBar.CurColourPict);
            controls.Add(RightBar.PlayerColourLabel);
            controls.Add(RightBar.PlayerColourPict);
            controls.Add(RightBar.CheckLabel);
        }

        public static void Add(this Control.ControlCollection controls, CheckmateDialog CheckmateDialog)
        {
            controls.Add(CheckmateDialog.CheckmateLabel);
            controls.Add(CheckmateDialog.ReturnToMenuBtn);
        }

        public static void Add(this Control.ControlCollection controls, ConnectionMenu connectionMenu)
        {
            controls.Add(connectionMenu.HostLabel);
            controls.Add(connectionMenu.HostTextBox);
            controls.Add(connectionMenu.PortLabel);
            controls.Add(connectionMenu.PortTextBox);
            controls.Add(connectionMenu.StartStopWaitingConnectionBtn);
            controls.Add(connectionMenu.ReturnToMenuBtn2);
        }

        public static void Add(this Control.ControlCollection controls, GameMenu gameMenu)
        {
            for (int i = 0; i < 2; i++)
            {
                controls.Add(gameMenu.Menu[i]);
            }
        }
    }

    public sealed class GameControls
    {
        public ChessUILogic UILogic { get; }
        public PlayingField PlayingField { get; }
        public RightBar RightBar { get; }

        public CheckmateDialog CheckmateDialog { get; }

        public GameControls(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;
            PlayingField = new PlayingField(chessUILogic);
            RightBar = new RightBar(chessUILogic);
            CheckmateDialog = new CheckmateDialog(chessUILogic);
        }

        public void Show()
        {
            PlayingField.Show();
            RightBar.Show();
        }

        public void Hide()
        {
            PlayingField.Hide();
            RightBar.Hide();
            CheckmateDialog.Hide();
        }
    }

    public sealed class CheckmateDialog
    {
        public ChessUILogic UILogic;
        public Label CheckmateLabel { get; }
        public Button ReturnToMenuBtn { get; }

        public CheckmateDialog(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;

            CheckmateLabel = new Label();
            ReturnToMenuBtn = new Button();

            CheckmateLabel.AutoSize = true;
            CheckmateLabel.Name = "CheckmateLabel";
            CheckmateLabel.Text = "Checkmate!!! Black won !!!";
            CheckmateLabel.Font = new Font(CheckmateLabel.Font.FontFamily, 30);
            CheckmateLabel.Size = new Size(650, 46);
            CheckmateLabel.Location = new Point(startX + fieldSize / 2 - CheckmateLabel.Width / 2, startY + fieldSize / 2 - CheckmateLabel.Height / 2);
            CheckmateLabel.Visible = false;

            ReturnToMenuBtn.Name = "ReturnToMenuBtn";
            ReturnToMenuBtn.Size = new Size(mainMenuBtnXSize, mainMenuBtnYSize);
            ReturnToMenuBtn.Location = new Point(startX + fieldSize / 2 - ReturnToMenuBtn.Width / 2, CheckmateLabel.Location.Y + CheckmateLabel.Height + mainMenuBtnGap);
            ReturnToMenuBtn.Text = "Return to Menu";
            ReturnToMenuBtn.TabIndex = 0;
            ReturnToMenuBtn.TabStop = false;
            ReturnToMenuBtn.Visible = false;
            ReturnToMenuBtn.UseVisualStyleBackColor = true;
            ReturnToMenuBtn.Click += new EventHandler(ReturnToMenuBtn_Click);

        }

        public void Show(bool isWhitesTurn)
        {
            CheckmateLabel.Text = "Checkmate!!! " + (isWhitesTurn ? "Black" : "White") + " won!!!";
            CheckmateLabel.BringToFront();
            ReturnToMenuBtn.BringToFront();
            CheckmateLabel.Show();
            ReturnToMenuBtn.Show();
        }

        public void Hide()
        {
            CheckmateLabel.Hide();
            ReturnToMenuBtn.Hide();
        }

        private void ReturnToMenuBtn_Click(object sender, EventArgs e)
        {
            UILogic.ReturnToMenu();
        }

    }

    public sealed class RightBar
    {
        public ChessUILogic UILogic { get; }
        public Label PlayerColourLabel { get; }
        public PictureBox PlayerColourPict { get; }
        public Label CurColourLabel { get; }
        public PictureBox CurColourPict { get; }
        public Label CheckLabel { get; }


        public RightBar(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;
            PlayerColourLabel = new Label();
            PlayerColourPict = new PictureBox();
            CurColourLabel = new Label();
            CurColourPict = new PictureBox();
            CheckLabel = new Label();

            PlayerColourLabel.AutoSize = false;
            PlayerColourLabel.Name = "PlayerColourLabel";
            PlayerColourLabel.Text = "You:";
            PlayerColourLabel.Font = new Font(PlayerColourLabel.Font.FontFamily, 10);
            PlayerColourLabel.Location = new Point(rightColumnX, rightcolumnStartY);
            PlayerColourLabel.Size = new Size(125, 20);
            PlayerColourLabel.TextAlign = ContentAlignment.MiddleCenter;
            PlayerColourLabel.Visible = false;

            PlayerColourPict.Location = new Point(rightColumnX, rightcolumnStartY + PlayerColourLabel.Height + rightColumnGap);
            PlayerColourPict.Name = "PlayerColourPict";
            PlayerColourPict.Size = new Size(125, 125);
            PlayerColourPict.SizeMode = PictureBoxSizeMode.StretchImage;
            PlayerColourPict.Visible = false;

            CurColourLabel.AutoSize = false;
            CurColourLabel.Name = "CurColourLabel";
            CurColourLabel.Text = "Whose move:";
            CurColourLabel.Font = new Font(CurColourLabel.Font.FontFamily, 10);
            CurColourLabel.Location = new Point(rightColumnX, PlayerColourPict.Location.Y + PlayerColourPict.Height + rightColumnGap);
            CurColourLabel.Size = new Size(125, 20);
            CurColourLabel.TextAlign = ContentAlignment.MiddleCenter;
            CurColourLabel.Visible = false;

            CurColourPict.Location = new Point(rightColumnX, CurColourLabel.Location.Y + CurColourLabel.Height + rightColumnGap);
            CurColourPict.Name = "CurColourPict";
            CurColourPict.Size = new Size(125, 125);
            CurColourPict.SizeMode = PictureBoxSizeMode.StretchImage;
            CurColourPict.Visible = false;

            CheckLabel.AutoSize = false;
            CheckLabel.Name = "CheckLabel";
            CheckLabel.Text = "Check";
            CheckLabel.Font = new Font(CheckLabel.Font.FontFamily, 20);
            CheckLabel.Location = new Point(rightColumnX, CurColourPict.Location.Y + CurColourPict.Height + rightColumnGap);
            CheckLabel.Size = new Size(125, 40);
            CheckLabel.TextAlign = ContentAlignment.MiddleCenter;
            CheckLabel.Visible = false;

        }

        public void Show()
        {
            PlayerColourLabel.Show();
            PlayerColourPict.Show();
            CurColourLabel.Show();
            CurColourPict.Show();
        }

        public void Hide()
        {
            PlayerColourLabel.Hide();
            PlayerColourPict.Hide();
            CurColourLabel.Hide();
            CurColourPict.Hide();
        }

        public void Refresh(bool isWhitesTurn, bool isWhitePlayer)
        {
            if (isWhitesTurn)
            {
                CurColourPict.Image = global::ChessUI.Properties.Resources.wfigures;
            }
            else
            {
                CurColourPict.Image = global::ChessUI.Properties.Resources.bfigures;
            }

            if (isWhitePlayer)
                PlayerColourPict.Image = global::ChessUI.Properties.Resources.wfigures;
            else
                PlayerColourPict.Image = global::ChessUI.Properties.Resources.bfigures;
        }
    }

    public sealed class PlayingField
    {
        public ChessUILogic UILogic { get; }
        private readonly FieldSquare[,] field;
        public PlayingField(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;

            field = new FieldSquare[8, 8];

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    field[i, j] = new FieldSquare();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    field[i, j].Figure.Location = new Point(startX + i * (figureSize + figuresGap), startY + j * (figureSize + figuresGap));
                    field[i, j].Figure.Name = "field" + i.ToString() + j.ToString();
                    field[i, j].Figure.Size = new Size(figureSize, figureSize);
                    field[i, j].Figure.TabIndex = i * 8 + j;
                    field[i, j].Figure.TabStop = false;
                    field[i, j].Figure.Tag = i * 8 + j;
                    field[i, j].Figure.Click += new EventHandler(PlayingField_Click);
                    field[i, j].Figure.Visible = false;
                    field[i, j].Figure.SizeMode = PictureBoxSizeMode.StretchImage;
                    field[i, j].Figure.Margin = new Padding(0, 0, 0, 0);


                    field[i, j].TopBacklight.Location = new Point(startX + i * (figureSize + figuresGap) - halfFiguresGap, startY + j * (figureSize + figuresGap) - halfFiguresGap);
                    field[i, j].TopBacklight.Name = "fieldTopBacklight" + i.ToString() + j.ToString();
                    field[i, j].TopBacklight.Size = new Size((figureSize + figuresGap), halfFiguresGap);
                    field[i, j].TopBacklight.TabIndex = 0;
                    field[i, j].TopBacklight.TabStop = false;
                    field[j, i].TopBacklight.Image = global::ChessUI.Properties.Resources.bhorizontalbacklight;
                    field[i, j].TopBacklight.SizeMode = PictureBoxSizeMode.StretchImage;
                    field[i, j].TopBacklight.Visible = false;
                    field[i, j].TopBacklight.Margin = new Padding(0, 0, 0, 0);


                    field[i, j].BottomBacklight.Location = new Point(startX + i * (figureSize + figuresGap) - halfFiguresGap, startY + (j + 1) * (figureSize + figuresGap) - figuresGap);
                    field[i, j].BottomBacklight.Name = "fieldBottomBacklight" + i.ToString() + j.ToString();
                    field[i, j].BottomBacklight.Size = new Size((figureSize + figuresGap), halfFiguresGap);
                    field[i, j].BottomBacklight.TabIndex = 0;
                    field[i, j].BottomBacklight.TabStop = false;
                    field[j, i].BottomBacklight.Image = global::ChessUI.Properties.Resources.bhorizontalbacklight;
                    field[i, j].BottomBacklight.SizeMode = PictureBoxSizeMode.StretchImage;
                    field[i, j].BottomBacklight.Visible = false;
                    field[i, j].BottomBacklight.Margin = new Padding(0, 0, 0, 0);

                    field[i, j].LeftBacklight.Location = new Point(startX + i * (figureSize + figuresGap) - halfFiguresGap, startY + j * (figureSize + figuresGap));
                    field[i, j].LeftBacklight.Name = "fieldLeftBacklight" + i.ToString() + j.ToString();
                    field[i, j].LeftBacklight.Size = new Size(halfFiguresGap, figureSize);
                    field[i, j].LeftBacklight.TabIndex = 0;
                    field[i, j].LeftBacklight.TabStop = false;
                    field[j, i].LeftBacklight.Image = global::ChessUI.Properties.Resources.bverticalbacklight;
                    field[i, j].LeftBacklight.SizeMode = PictureBoxSizeMode.StretchImage;
                    field[i, j].LeftBacklight.Visible = false;
                    field[i, j].LeftBacklight.Margin = new Padding(0, 0, 0, 0);


                    field[i, j].RightBacklight.Location = new Point(startX + (i + 1) * (figureSize + figuresGap) - figuresGap, startY + j * (figureSize + figuresGap));
                    field[i, j].RightBacklight.Name = "fieldRightBacklight" + i.ToString() + j.ToString();
                    field[i, j].RightBacklight.Size = new Size(halfFiguresGap, figureSize);
                    field[i, j].RightBacklight.TabIndex = 0;
                    field[i, j].RightBacklight.TabStop = false;
                    field[j, i].RightBacklight.Image = global::ChessUI.Properties.Resources.bverticalbacklight;
                    field[i, j].RightBacklight.SizeMode = PictureBoxSizeMode.StretchImage;
                    field[i, j].RightBacklight.Visible = false;
                    field[i, j].RightBacklight.Margin = new Padding(0, 0, 0, 0);
                }
        }

        public FieldSquare this[int i, int j]
        {
            get
            {
                return field[i, j];
            }
            set
            {
                field[i, j] = value;
            }
        }

        public void Show()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    field[i, j].Show();
        }

        public void Hide()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    field[i, j].Hide();
        }

        public void ResetBacklight()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    field[i, j].SetBacklight(BacklightColour.Black);
                }
        }

        public void Refresh(Figure[,] field)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    string resourceName = ((i + j) % 2 == 0) ? "w" : "b";
                    if (field[i, j] == null)
                        resourceName += "empty";
                    else
                    {
                        if (field[i, j].IsWhite)
                            resourceName += "w";
                        else
                            resourceName += "b";

                        switch (field[i, j].FigType) //здесь бы модный свитч, но что-то не c#8
                        {
                            case FigureType.Pawn:
                                resourceName += "pawn";
                                break;
                            case ChessLogic.FigureType.Rook:
                                resourceName += "rook";
                                break;
                            case FigureType.Knight:
                                resourceName += "knight";
                                break;
                            case FigureType.Bishop:
                                resourceName += "bishop";
                                break;
                            case FigureType.Queen:
                                resourceName += "queen";
                                break;
                            case FigureType.King:
                                resourceName += "king";
                                break;

                        }
                    }
                    this.field[j, i].Figure.Image = Properties.Resources.ResourceManager.GetObject(resourceName) as Bitmap;

                }
        }

        private void PlayingField_Click(object sender, EventArgs e)
        {
            var pictureBox = sender as PictureBox;
            int? y = (pictureBox.Tag as int?) % 8;
            int? x = (pictureBox.Tag as int?) / 8;

            UILogic.StartOrMakeAMove(y.Value, x.Value);
        }
    }

    public sealed class ConnectionMenu
    {
        public ChessUILogic UILogic { get; }
        public Label HostLabel { get; }
        public Label PortLabel { get; }
        public TextBox HostTextBox { get; }
        public TextBox PortTextBox { get; }
        public Button StartStopWaitingConnectionBtn { get; }
        public Button ReturnToMenuBtn2 { get; }

        public ConnectionMenu(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;
            HostLabel = new Label();
            PortLabel = new Label();
            HostTextBox = new TextBox();
            PortTextBox = new TextBox();
            StartStopWaitingConnectionBtn = new Button();
            ReturnToMenuBtn2 = new Button();

            HostLabel.AutoSize = false;
            HostLabel.Name = "HostLabel";
            HostLabel.Text = "Host:";
            HostLabel.Location = new Point(connMenuLabelX, hostLabelY);
            HostLabel.Size = new Size(connMenuLabelSizeX, connMenuLabelSizeY);
            HostLabel.TextAlign = ContentAlignment.MiddleCenter;
            HostLabel.Visible = false;

            PortLabel.AutoSize = false;
            PortLabel.Name = "PortLabel";
            PortLabel.Text = "Port:";
            PortLabel.Location = new Point(connMenuLabelX, portLabelY);
            PortLabel.Size = new Size(connMenuLabelSizeX, connMenuLabelSizeY);
            PortLabel.TextAlign = ContentAlignment.MiddleCenter;
            PortLabel.Visible = false;

            HostTextBox.Location = new Point(connMenuTextBoxX, hostLabelY);
            HostTextBox.Name = "HostTextBox";
            HostTextBox.Size = new Size(connMenuTextBoxSizeX, connMenuTextBoxSizeY);
            HostTextBox.TabIndex = 0;
            HostTextBox.Visible = false;

            PortTextBox.Location = new Point(connMenuTextBoxX, portLabelY);
            PortTextBox.Name = "PortTextBox";
            PortTextBox.Size = new Size(connMenuTextBoxSizeX, connMenuTextBoxSizeY);
            PortTextBox.TabIndex = 1;
            PortTextBox.Visible = false;

            StartStopWaitingConnectionBtn.Name = "StartStopWaitingConnectionBtn";
            StartStopWaitingConnectionBtn.Size = new Size(connMenuBtnSizeX, connMenuBtnSizeY);
            StartStopWaitingConnectionBtn.Location = new Point(ConnectBtnX, connMenuBtnY);
            StartStopWaitingConnectionBtn.Text = "Start waiting for connection";
            StartStopWaitingConnectionBtn.TabIndex = 2;
            StartStopWaitingConnectionBtn.TabStop = false;
            StartStopWaitingConnectionBtn.Visible = false;
            StartStopWaitingConnectionBtn.UseVisualStyleBackColor = true;
            StartStopWaitingConnectionBtn.Click += new EventHandler(StartStopWaitingConnectionBtn_Click);

            ReturnToMenuBtn2.Name = "ReturnToMenuBtn2";
            ReturnToMenuBtn2.Size = new Size(connMenuBtnSizeX, connMenuBtnSizeY);
            ReturnToMenuBtn2.Location = new Point(ReturnToMenuBtn2X, connMenuBtnY);
            ReturnToMenuBtn2.Text = "Return to main menu";
            ReturnToMenuBtn2.TabIndex = 3;
            ReturnToMenuBtn2.TabStop = false;
            ReturnToMenuBtn2.Visible = false;
            ReturnToMenuBtn2.UseVisualStyleBackColor = true;
            ReturnToMenuBtn2.Click += new EventHandler(ReturnToMenuBtn_Click);

        }

        public void Show(bool isServerSide)
        {
            HostLabel.Show();
            PortLabel.Show();
            HostTextBox.Show();
            PortTextBox.Show();
            StartStopWaitingConnectionBtn.Show();
            ReturnToMenuBtn2.Show();
            StartStopWaitingConnectionBtn.Text = "Waiting for connection";
            HostTextBox.ReadOnly = isServerSide;
            if (isServerSide)
            {
                HostTextBox.Text = "127.0.0.1";
                PortTextBox.Text = "20";
            }
            else
            {
                HostTextBox.Text = "localhost";
                PortTextBox.Text = "20";
            }
        }

        public void Hide()
        {
            HostLabel.Hide();
            PortLabel.Hide();
            HostTextBox.Hide();
            PortTextBox.Hide();
            StartStopWaitingConnectionBtn.Hide();
            ReturnToMenuBtn2.Hide();
        }

        private void StartStopWaitingConnectionBtn_Click(object sender, EventArgs e)
        {
            UILogic.StartStopWaitingConnection();
        }

        private void ReturnToMenuBtn_Click(object sender, EventArgs e)
        {
            UILogic.ReturnToMenu();
        }
    }

    public sealed class GameMenu
    {
        private ChessUILogic UILogic { get; }

        public Button[] Menu { get; }
        public GameMenu(ChessUILogic chessUILogic)
        {
            UILogic = chessUILogic;
            Menu = new Button[2];

            for (int i = 0; i < 2; i++)
            {
                Menu[i] = new Button
                {
                    Location = new Point(mainMenuStartX, mainMenuStartY + (mainMenuBtnYSize + mainMenuBtnGap) * i),
                    Name = "mainMenu" + i.ToString(),
                    Size = new Size(mainMenuBtnXSize, mainMenuBtnYSize),
                    TabIndex = i,
                    TabStop = false,
                    Visible = true,
                    UseVisualStyleBackColor = true
                };
            }
            Menu[0].Text = "New Game";
            Menu[1].Text = "Connect";
            Menu[0].Click += new EventHandler(CreateNewGameBtn_Click);
            Menu[1].Click += new EventHandler(ConnectGameBtn_Click);

        }

        public void Show()
        {
            for (int i = 0; i < 2; i++)
                Menu[i].Show();
        }

        public void Hide()
        {
            for (int i = 0; i < 2; i++)
                Menu[i].Hide();
        }

        private void CreateNewGameBtn_Click(object sender, EventArgs e)
        {
            UILogic.SetupConnection(true);
        }

        private void ConnectGameBtn_Click(object sender, EventArgs e)
        {
            UILogic.SetupConnection(false);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
