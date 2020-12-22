using Minesweeper.Core.Configurations;
using Minesweeper.Core.Factories;
using Minesweeper.Configuration;
using Minesweeper.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Minesweeper.Views;
using System.IO;
using Minesweeper.Shared;
using Minesweeper.Controls;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IMinesweeperServices _service;
        private string _userName;
        private string _gameId;
        private bool _isGameEnabled = false;
        private DateTime? _startTime;
        private int? _mines;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.Initialize();
        }

        private void Initialize()
        {
            this.Configuration = ConfigurationProvider.Instance.GetConfiguration<MinesweeperConfiguration>();
            this.mainBoard.CellClick += OnCell_Click;
        }


        #region -- INotifyPropertyChanged --

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
        /// where 'this' is derived from <seealso cref="BaseViewModel"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        protected virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)lambda.Body;

            this.RaisePropertyChanged(memberExpression.Member.Name);
        }


        protected virtual void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged == null)
                return;

            if (Application.Current.Dispatcher.CheckAccess())
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            else
                Application.Current.Dispatcher.BeginInvoke((Action)delegate { this.RaisePropertyChanged(propertyName); });
        }

        #endregion

        private MinesweeperConfiguration Configuration { get; set; }

        private IMinesweeperServices Service
        {
            get
            {
                if (this._service == null)
                {
                    _service = FactoryProvider.Instance.CreateEntity<IMinesweeperServices>(this.Configuration.MinesweeperServiceFactoryName);
                }

                return _service;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;

                this.IsGameEnabled = !string.IsNullOrEmpty(value);
                this.RaisePropertyChanged();
            }
        }

        public string GameId
        {
            get
            {
                return _gameId;
            }
            set
            {
                _gameId = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsGameEnabled
        {
            get
            {
                return _isGameEnabled;
            }
            set
            {
                _isGameEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                this.RaisePropertyChanged();
            }
        }

        public int? Mines
        {
            get
            {
                return _mines;
            }
            set
            {
                _mines = value;
                this.RaisePropertyChanged();
            }
        }

        public GameData CurrentGame { get; set; }

        private void LoadNewGame(GameData game)
        {
            this.CurrentGame = game;

            if (game != null)
            {
                this.Mines = game.Board.MinesCount;
                this.GameId = game.Id;

                if (game.Status == GameStatusType.GameStatus_Created)
                    this.StartTime = null;
                else
                    this.StartTime = game.StartTime;

                this.mainBoard.Load(game.Board);
            }
            else
            {
                this.Mines = null;
                this.StartTime = null;
                this.GameId = null;

                this.mainBoard.Clear();
            }
        }

        private void UpdateGame(GameData game)
        {
            if (game.Status == GameStatusType.GameStatus_Created)
                this.StartTime = null;
            else
                this.StartTime = game.StartTime;

            this.mainBoard.UpdateBoard(game.Board);
        }

        #region -- Menu events --

        private void OnNewGameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var view = new NewGameView();
            var b = view.ShowDialog();

            if (b.Value)
            {
                var rows = view.Rows;
                var cols = view.Columns;
                var mines = view.Mines;

                var game = this.Service.NewGame(this.UserName, rows, cols, mines);
                this.LoadNewGame(game);
            }
        }



        private void OnResumeGameMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnSignInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var view = new SignInView();
            var b = view.ShowDialog();

            if (b.Value)
            {
                this.UserName = view.UserName;
            }
        }

        #endregion

        #region -- Board events --

        private void OnCell_Click(object sender, CellClickEventArgs e)
        {
            GameData game = null;
            if (e.IsRevealAction)
            {
                game = this.Service.RevealCell(this.UserName, this.GameId, e.Cell.Row, e.Cell.Col);
            }
            else
            {
                switch (e.Cell.Mark)
                {
                    case CellMarkType.CellMarkType_None:
                        game = this.Service.MarkCell(this.UserName, this.GameId, e.Cell.Row, e.Cell.Col, CellMarkType.CellMarkType_Flag);
                        break;
                    case CellMarkType.CellMarkType_Flag:
                        game = this.Service.MarkCell(this.UserName, this.GameId, e.Cell.Row, e.Cell.Col, CellMarkType.CellMarkType_Question);
                        break;
                    case CellMarkType.CellMarkType_Question:
                        game = this.Service.MarkCell(this.UserName, this.GameId, e.Cell.Row, e.Cell.Col, CellMarkType.CellMarkType_None);
                        break;
                }
            }

            this.UpdateGame(game);

            if (game.Status == GameStatusType.GameStatus_Won)
            {
                MessageBox.Show("WIN!!", "Game Over", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else if (game.Status == GameStatusType.GameStatus_Lost)
            {
                MessageBox.Show("LOST!!", "Game Over", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        #endregion
    }
}
