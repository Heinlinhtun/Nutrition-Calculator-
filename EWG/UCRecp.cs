﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EWG
{
    public partial class UCRecp : UserControl
    {
        public UCRecp()
        {
            InitializeComponent();
        }
        int editMode = 0; // 0 is InsertMode, 1 is Edit mode
        private void BtnNew_Click(object sender, EventArgs e)
        {
            FrmAddCategory addCategory = new FrmAddCategory(this);
            addCategory.ShowDialog();
        }

        private void UCRecp_Load(object sender, EventArgs e)
        {
            comboLoad();
            dg_load();
        }

        public void comboLoad()
        {
            string query = "SELECT id ,name  from TblCategories WHERE Type = 0";
            SQLHelper.ComboFills(query, ComboCat, "name", "id");
            ComboCat.Text = "Select Categories";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string query;
            if (TxtName.Text != "" || ComboCat.Text != "Select Categories" || TxtPortion.Text != "")
            {
                if (editMode == 0)
                {
                    query = "INSERT INTO TblReceipts (name,catID, portionSize) Values (@name, @catid, @portionsize)";
                    string[] varArr = { "@name", "@catid", "@portionsize" };
                    object[] datArr = { TxtName.Text, Convert.ToInt16(ComboCat.SelectedValue), TxtPortion.Text };
                    SQLHelper.InsertAndUpdateQuery(query, varArr, datArr);
                }
                else
                {
                    query = "UPDATE TblReceipts SET name = @name, catID = @catid, portionSize = @portionsize WHERE name = '" + DGReceipts.CurrentRow.Cells[1].Value.ToString() + "' AND portionSize = '" + DGReceipts.CurrentRow.Cells[3].Value.ToString() + "'";
                    string[] varArr = { "@name", "@catid", "@portionsize" };
                    object[] datArr = { TxtName.Text, Convert.ToInt16(ComboCat.SelectedValue), TxtPortion.Text };
                    SQLHelper.InsertAndUpdateQuery(query, varArr, datArr);
                }

                TxtClr();
                dg_load();
            }
            else
            {
                MessageBox.Show("Fill all fields!");
            }
            editMode = 0;

        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TxtClr();
        }

        private void TxtClr()
        {
            TxtName.Text = "";
            ComboCat.Text = "Select Categories";
            TxtPortion.Text = "";
        }

        private void dg_load()
        {
            string loadDG;
            if (TxtSearch.Text != "")
            {
                loadDG = "SELECT TblReceipts.name as RecpName, TblCategories.name as CatName, TblReceipts.portionSize as PortionSize From TblReceipts INNER JOIN TblCategories ON TblReceipts.catID= TblCategories.id WHERE TblReceipts.name LIKE '%" + TxtSearch.Text + "%' ";

            }
            else
            {
                loadDG = "SELECT TblReceipts.name as RecpName, TblCategories.name as CatName, TblReceipts.portionSize as PortionSize From TblReceipts INNER JOIN TblCategories ON TblReceipts.catID= TblCategories.id";
            }
            SQLHelper.dgView_Load(loadDG, DGReceipts);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DGReceipts.SelectedRows.Count != 0)
            {
                var result = MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {

                    string query = "DELETE FROM TblReceipts WHERE name = '" + DGReceipts.CurrentRow.Cells[1].Value.ToString() + "' AND portionSize = '" + Convert.ToDouble(DGReceipts.CurrentRow.Cells[3].Value) + "'";
                    SQLHelper.ExeQuery(query);
                    dg_load();
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TxtName.Text = DGReceipts.CurrentRow.Cells[1].Value.ToString();
            ComboCat.Text = DGReceipts.CurrentRow.Cells[2].Value.ToString();
            TxtPortion.Text = DGReceipts.CurrentRow.Cells[3].Value.ToString();
            editMode = 1;
        }

        private void addIngredientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DGReceipts.SelectedRows.Count > 0)
            {
                GlobalVariables.RecpName = DGReceipts.CurrentRow.Cells[1].Value.ToString();
                FrmAddIngredients addIngredients = new FrmAddIngredients();
                addIngredients.ShowDialog();
            }



        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            dg_load();
        }
    }
}
