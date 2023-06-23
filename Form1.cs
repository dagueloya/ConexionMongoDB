using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Proyecto01
{
    public partial class Form1 : Form
    {
        private IMongoCollection<BsonDocument> collection;

        public Form1()
        {
            InitializeComponent();
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("mydatabase");
            collection = database.GetCollection<BsonDocument>("mycollection");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            dataGridView.Rows.Clear();
            var documents = collection.Find(new BsonDocument()).ToList();
            foreach (var doc in documents)
            {
                dataGridView.Rows.Add(
                    doc.GetValue("_id"),
                    doc.GetValue("name"),
                    doc.GetValue("age"),
                    doc.GetValue("profession")
                );
            }
            ClearFields();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var document = new BsonDocument
            {
                { "_id", ObjectId.GenerateNewId() },
                { "name", txtName.Text },
                { "age", Convert.ToInt32(txtAge.Text) },
                { "profession", txtProfession.Text }
            };

            collection.InsertOne(document);
            RefreshData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var documentId = selectedRow.Cells[0].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));
                var update = Builders<BsonDocument>.Update
                    .Set("name", txtName.Text)
                    .Set("age", Convert.ToInt32(txtAge.Text))
                    .Set("profession", txtProfession.Text);

                collection.UpdateOne(filter, update);
                RefreshData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var documentId = selectedRow.Cells[0].Value.ToString();

                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));

                collection.DeleteOne(filter);
                RefreshData();
            }
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {          
        }

        private void ClearFields()
        {
            txtId.Text = string.Empty;
            txtName.Text = string.Empty;
            txtAge.Text = string.Empty;
            txtProfession.Text = string.Empty;
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtId.Text = dataGridView.SelectedCells[0].Value.ToString();
            txtName.Text = dataGridView.SelectedCells[1].Value.ToString();
            txtAge.Text = dataGridView.SelectedCells[2].Value.ToString();
            txtProfession.Text = dataGridView.SelectedCells[3].Value.ToString();
        }
    }
}
