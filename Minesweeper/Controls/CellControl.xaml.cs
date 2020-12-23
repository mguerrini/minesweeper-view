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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper.Controls
{
    /// <summary>
    /// Interaction logic for CellControl.xaml
    /// </summary>
    public partial class CellControl : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<CellClickEventArgs> CellClick;

        public CellControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public CellData Model { get; set; }

        public string CellContent { get; set; }

        public Brush CellColor { get; set; }

        public Brush TextColor { get; set; }

        public void SetModel(CellData model)
        {
            this.Model = model;

            if (model.IsRevealed)
            {
                //Backgroud color
                this.CellColor = Brushes.Beige;

                switch (model.Type)
                {
                    case CellType.CellType_Mine:
                        this.CellContent = "M";
                        this.TextColor = Brushes.Red;
                        break;
                    case CellType.CellType_Empty:
                        this.CellContent = "";
                        break;
                    case CellType.CellType_Number:
                        this.CellContent = model.Number.ToString();
                        this.TextColor = Brushes.Blue;
                        break;
                    case CellType.CellType_Unknown:
                        this.CellContent = "";
                        break;
                    default:
                        this.CellContent = "";
                        break;
                }
            }
            else
            {
                this.CellColor = Brushes.LightBlue;

                switch (model.Mark)
                {
                    case CellMarkType.CellMarkType_Flag:
                        this.CellContent = "F";
                        this.TextColor = Brushes.Red;
                        break;
                    case CellMarkType.CellMarkType_None:
                        this.CellContent = "";
                        break;
                    case CellMarkType.CellMarkType_Question:
                        this.CellContent = "?";
                        this.TextColor = Brushes.Blue;
                        break;
                    default:
                        this.CellContent = "";
                        break;
                }
            }

            this.RaisePropertyChanged("CellColor");
            this.RaisePropertyChanged("CellContent");
            this.RaisePropertyChanged("TextColor");
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


        private void OnCell_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (this.CellClick != null)
            {
                CellClickEventArgs args = new CellClickEventArgs() { Col = this.Col, Row = this.Row };
                this.CellClick(this, args);
            }
            */
        }

        private void OnCell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Model.IsRevealed)
                return;

            if (this.CellClick != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    CellClickEventArgs args = new CellClickEventArgs() { IsRevealAction= true, Cell= this.Model };
                    this.CellClick(this, args);
                }
                else
                {
                    CellClickEventArgs args = new CellClickEventArgs() { IsMarkAction = true, Cell = this.Model };
                    this.CellClick(this, args);
                }
            }
        }
    }
}
