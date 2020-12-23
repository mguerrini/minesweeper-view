using Minesweeper.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper.Controls
{
    public class BoardControl : Grid
    {
        public event EventHandler<CellClickEventArgs> CellClick;



        public BoardData Board { get; private set; }

        private CellControl[,] Cells { get; set; }

        public int? MinesLeft { get; internal set; }

        public int? MinesFinded { get; internal set; }

        public void Clear()
        {
            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();
            this.Children.Clear();
            this.MinesLeft = null;
            this.MinesFinded = null;
            

            if (this.Cells != null)
            {
                foreach (var c in this.Cells)
                {
                    c.CellClick-= OnCell_CellClick; 
                }
            }
        }


        public void Load(BoardData board)
        {
            int rows = board.RowCount;
            int cols = board.ColCount;

            this.Board = board;

            this.Clear();

            this.Cells = new CellControl[rows,cols];

            for (int c= 0; c < cols; c++)
            {
                var col = new ColumnDefinition()
                {
                    Width = new GridLength(1d, GridUnitType.Star)
                };

                this.ColumnDefinitions.Add(col);
            }

            for (int r = 0; r < rows; r++)
            {
                var row = new RowDefinition()
                {
                    Height = new GridLength(1d, GridUnitType.Star)
                };

                this.RowDefinitions.Add(row);
            }

            //Create the controls
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var cell = new CellControl();
                    cell.CellClick += OnCell_CellClick;

                    this.Cells[r, c] = cell;

                    this.Children.Add(cell);
                    //add to grid
                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                }
            }

            this.UpdateBoard(board);
        }

        public void UpdateBoard(BoardData board)
        {
            this.Board = board;
            int finded = 0;

            for (int r = 0; r<board.RowCount; r++)
            {
                for (int c = 0; c < board.ColCount; c++)
                {
                    CellControl cell = this.Cells[r, c];
                    cell.SetModel(board.Cells[r][c]);

                    if (board.Cells[r][c].Mark == CellMarkType.CellMarkType_Flag)
                        finded++;
                }
            }

            this.MinesFinded = finded;
            this.MinesLeft = board.MinesCount - this.MinesFinded;
        }

        private void OnCell_CellClick(object sender, CellClickEventArgs e)
        {
            if (this.CellClick != null)
            {
                this.CellClick(this, e);
            }
        }
    }
}
