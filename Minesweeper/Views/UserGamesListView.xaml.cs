using Minesweeper.Services;
using Minesweeper.Shared;
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
using System.Windows.Shapes;

namespace Minesweeper.Views
{
    /// <summary>
    /// Interaction logic for UserGamesListView.xaml
    /// </summary>
    public partial class UserGamesListView : Window, INotifyPropertyChanged
    {
        public UserGamesListView(string userId, IMinesweeperServices service)
        {
            InitializeComponent();

            this.DataContext = this;

            this.UserId = userId;

            this.Service = service;

            this.Loaded += UserGamesListView_Loaded;
        }

        private void UserGamesListView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UserGamesListView_Loaded;

            this.LoadGames();
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


        private string UserId { get; set; }

        private IMinesweeperServices Service { get;  set; }

        public IList<GameData> Games { get; set; }

        private GameData _selectedGame;
        public GameData SelectedGame
        {
            get
            {
                return _selectedGame;
            }
            set
            {
                _selectedGame = value;
                this.IsAcceptEnabled = this.SelectedGame != null;
                this.IsDeleteEnabled = this.SelectedGame != null;
                this.RaisePropertyChanged("IsAcceptEnabled");
                this.RaisePropertyChanged("IsDeleteEnabled");
            }
        }

        public bool IsAcceptEnabled { get; set; }

        public bool IsDeleteEnabled { get; set; }
        

        private void LoadGames()
        {
            try
            {
                this.Games = this.Service.GetGameListByUserId(this.UserId);
                this.Games = this.Games.OrderByDescending(f => f.StartTime).ToList();
                this.SelectedGame = null;

                this.RaisePropertyChanged("Games");
            }
            catch (Exception)
            {
                MessageBox.Show("Its not posible to get the list of games for user " + this.UserId, "Game List Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }


            this.SelectedGame = null;
        }


        #region -- Accept / cancel --

        private void OnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion


        #region -- Delete / Delete All --



        private void OnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete Game " + this.SelectedGame.Id, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    this.Service.DeleteGame(this.UserId, this.SelectedGame.Id);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting game", "Delete Game Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                this.LoadGames();
            }
        }

        private void OnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete All Games", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    this.Service.DeleteGamesByUser(this.UserId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting all games", "Delete All Games Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

                this.LoadGames();
            }
        }

        #endregion
    }
}
