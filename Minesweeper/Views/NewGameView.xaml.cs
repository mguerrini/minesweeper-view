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
    /// Interaction logic for NewGameView.xaml
    /// </summary>
    public partial class NewGameView : Window, INotifyPropertyChanged
    {
        public NewGameView()
        {
            InitializeComponent();
            this.DataContext = this;
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

        private int _rows;
        private int _columns;
        private int _mines;

        public int Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
                this.IsCustomButtonEnabled = this.Rows > 0 && this.Columns > 0 && this.Mines > 0;
                this.RaisePropertyChanged("IsCustomButtonEnabled");
            }
        }

        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
                this.IsCustomButtonEnabled = this.Rows > 0 && this.Columns > 0 && this.Mines > 0;
                this.RaisePropertyChanged("IsCustomButtonEnabled");
            }
        }
        public int Mines
        {
            get
            {
                return _mines;
            }
            set
            {
                _mines = value;
                this.IsCustomButtonEnabled = this.Rows > 0 && this.Columns > 0 && this.Mines > 0;
                this.RaisePropertyChanged("IsCustomButtonEnabled");
            }
        }

        public bool IsCustomButtonEnabled { get; set; }

        private void OnEasy_Click(object sender, RoutedEventArgs e)
        {
            this.Rows = 9;
            this.Columns = 9;
            this.Mines = 10;
            this.DialogResult = true;
            this.Close();
        }

        private void OnMedium_Click(object sender, RoutedEventArgs e)
        {
            this.Rows = 16;
            this.Columns = 16;
            this.Mines = 40;
            this.DialogResult = true;
            this.Close();
        }

        private void OnHard_Click(object sender, RoutedEventArgs e)
        {
            this.Rows = 16;
            this.Columns = 30;
            this.Mines = 99;
            this.DialogResult = true;
            this.Close();
        }

        private void OnCustom_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
