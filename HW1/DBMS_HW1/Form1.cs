using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBMS_HW1
{
    public partial class Form1 : Form
    {
        SqlConnection cs = new SqlConnection("Data Source=DESKTOP-FP0A8DF\\SQLEXPRESS;Initial Catalog=EventManagementDB;Integrated Security=True;TrustServerCertificate=true;");
        SqlDataAdapter da = new SqlDataAdapter();
        SqlDataAdapter reviewsDataAdapter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        DataSet dAd = new DataSet();
        BindingSource bs = new BindingSource();
        BindingSource bs_child = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridViewUpdate()
        {
            dataGridView.ClearSelection();
            dataGridView.Rows[bs.Position].Selected = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                da.SelectCommand = new SqlCommand("Select * from Users", cs);
                ds.Clear();
                da.Fill(ds);
                dataGridView.DataSource = ds.Tables[0];
                bs.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
            dataGridViewUpdate();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
            dataGridViewUpdate();
        }

        private void ReloadProductTableView()
        {
            if (ds.Tables["Users"] != null)
            {
                ds.Tables["Users"].Clear();
            }
            da.Fill(ds, "Users");
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                int reviewID = Int32.Parse(reviewIDTextBox.Text);

                SqlCommand insertCommand = new SqlCommand("INSERT INTO Reviews (ReviewID, Content, Rating, UserID) VALUES (@reviewID, @content, @rating, @userID);", cs);

                insertCommand.Parameters.Add("@reviewID", SqlDbType.Int).Value = reviewID;
                insertCommand.Parameters.Add("@content", SqlDbType.NVarChar, 100).Value = contentTextBox.Text;
                insertCommand.Parameters.Add("@rating", SqlDbType.Int).Value = Int32.Parse(ratingTextBox.Text);
                insertCommand.Parameters.Add("@userID", SqlDbType.Int).Value = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["UserID"].Value); // Use the selected UserID from the DataGridView

                cs.Open();

                insertCommand.ExecuteNonQuery();

                cs.Close();

                MessageBox.Show("Review added successfully with ID: " + reviewID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding review: " + ex.Message);
                cs.Close();
            }
        }

        private void ReloadReviewsDataGridView()
        {
            try
            {
                // Clear existing data in the dataset
                dAd.Clear();

                // Refill the dataset with updated data from the Reviews table
                reviewsDataAdapter.SelectCommand = new SqlCommand("SELECT * FROM Reviews", cs);
                reviewsDataAdapter.Fill(dAd);

                // Set the DataSource of the DataGridView to display the updated reviews
                dataGridView.DataSource = dAd.Tables[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing reviews: " + ex.Message);
            }
        }


        private void Update_Click(object sender, EventArgs e)
        {
            int x;
            da.UpdateCommand = new SqlCommand("Update Reviews set Rating=@r, Content=@c where ReviewID = @reviewID", cs);
            try
            {
                da.UpdateCommand.Parameters.Add("@r",
               SqlDbType.Int).Value = int.Parse(ratingTextBox.Text);
                da.UpdateCommand.Parameters.Add("@c",
               SqlDbType.NVarChar, 100).Value = contentTextBox.Text;
                da.UpdateCommand.Parameters.Add("@reviewID",
               SqlDbType.Int).Value = dAd.Tables[0].Rows[bs_child.Position][0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            cs.Open();
            x = da.UpdateCommand.ExecuteNonQuery();
            cs.Close();
            if (x >= 1)
            {
                MessageBox.Show("The record has been updated");
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("Are you sure?\n No undo afterde lete", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                da.DeleteCommand = new SqlCommand("Delete from Reviews where ReviewID = @reviewID", cs);

                da.DeleteCommand.Parameters.Add("@reviewID",
               SqlDbType.Int).Value = dAd.Tables[0].Rows[bs_child.Position][0];
                cs.Open();
                da.DeleteCommand.ExecuteNonQuery();
                cs.Close();
                ds.Clear();
                da.SelectCommand = new SqlCommand("SELECT * FROM Users", cs);

                da.Fill(ds);
            }
            else
            {
                MessageBox.Show("Deletion Aborded");
            }
        }

        private void dataGridViewUpdateChild()
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[bs_child.Position].Selected = true;
        }

        private void Next2_Click(object sender, EventArgs e)
        {
            bs_child.MoveNext();
            dataGridViewUpdateChild();
        }

        private void Previous2_Click(object sender, EventArgs e)
        {
            bs_child.MovePrevious();
            dataGridViewUpdateChild();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void contentTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                // Get the value of the "UserID" column from the selected row
                int selectedId = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["UserID"].Value);

                da.SelectCommand = new SqlCommand("SELECT * FROM Reviews WHERE UserID = @selectedId", cs);
                da.SelectCommand.Parameters.Add("@selectedId",
           SqlDbType.Int).Value = selectedId;
                dAd.Clear();
                da.Fill(dAd);
                bs_child.DataSource = dAd.Tables[0];

                // Filter the child DataGridView based on the selected "UserID"
                DataView dv = new DataView(dAd.Tables[0]);
                dv.RowFilter = $"UserID = {selectedId}";
                dataGridView1.DataSource = dv;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
