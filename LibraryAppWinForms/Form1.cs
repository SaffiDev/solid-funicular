using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryAppWinForms
{
    public partial class Form1 : Form
    {
        private LibraryManager _libMan;
        private DataGridView _dataGrid;

        // Элементы управления
        private TextBox _txtTitle, _txtAuthor, _txtYear, _txtSearch;
        private Button _btnAdd, _btnDelete, _btnSearch;
        private ComboBox _cmbSearchType;

        // Компонент адаптивной верстки
        private TableLayoutPanel _tlpControls;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            _libMan = new LibraryManager();
            _ = LoadDataAsync();
        }

        private void SetupUI()
        {
            // Инициализация элементов
            _dataGrid = new DataGridView();
            _txtTitle = new TextBox();
            _txtAuthor = new TextBox();
            _txtYear = new TextBox();
            _txtSearch = new TextBox();
            _btnAdd = new Button();
            _btnDelete = new Button();
            _btnSearch = new Button();
            _cmbSearchType = new ComboBox();

            _tlpControls = new TableLayoutPanel();

            var lblTitle = new Label { Text = "Название:", AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblAuthor = new Label { Text = "Автор:", AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblYear = new Label { Text = "Год:", AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            var lblSearch = new Label { Text = "Искать:", AutoSize = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };

            SuspendLayout();

            // Основные настройки окна
            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.ClientSize = new Size(900, 550);
            this.MinimumSize = new Size(700, 450);

            // 1. ТАБЛИЦА 
            _dataGrid.Dock = DockStyle.Fill;
            _dataGrid.BackgroundColor = Color.White;
            _dataGrid.BorderStyle = BorderStyle.None;
            _dataGrid.RowHeadersVisible = false;
            _dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dataGrid.MultiSelect = false;
            _dataGrid.ReadOnly = true;
            _dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dataGrid.AllowUserToAddRows = false;

            _dataGrid.GridColor = Color.FromArgb(224, 224, 224);
            _dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 237, 240);
            _dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            _dataGrid.EnableHeadersVisualStyles = false;
            _dataGrid.RowTemplate.Height = 32;

            // 2. ПАНЕЛЬ ИНСТРУМЕНТОВ
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 135, BackColor = Color.FromArgb(248, 249, 250) };

            var pnlSeparator = new Panel { Dock = DockStyle.Top, Height = 2, BackColor = Color.FromArgb(218, 220, 224) };
            pnlBottom.Controls.Add(pnlSeparator);

            _tlpControls.Dock = DockStyle.Fill;
            _tlpControls.Padding = new Padding(15, 5, 15, 10);
            _tlpControls.ColumnCount = 4;
            _tlpControls.RowCount = 4;

            _tlpControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            _tlpControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            _tlpControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            _tlpControls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));

            _tlpControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tlpControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _tlpControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tlpControls.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));

            Padding elementMargin = new Padding(4, 2, 4, 2);
            _txtTitle.Margin = _txtAuthor.Margin = _txtYear.Margin = _txtSearch.Margin = _cmbSearchType.Margin = elementMargin;
            _btnAdd.Margin = _btnSearch.Margin = _btnDelete.Margin = elementMargin;

            _txtTitle.PlaceholderText = "Введите название";
            _txtAuthor.PlaceholderText = "Введите автора";
            _txtYear.PlaceholderText = "Год";
            _txtSearch.PlaceholderText = "Введите запрос";

            // Строка 1: Добавление
            _tlpControls.Controls.Add(lblTitle, 0, 0);
            _tlpControls.Controls.Add(_txtTitle, 0, 1); _txtTitle.Dock = DockStyle.Fill;
            _tlpControls.Controls.Add(lblAuthor, 1, 0);
            _tlpControls.Controls.Add(_txtAuthor, 1, 1); _txtAuthor.Dock = DockStyle.Fill;
            _tlpControls.Controls.Add(lblYear, 2, 0);
            _tlpControls.Controls.Add(_txtYear, 2, 1); _txtYear.Dock = DockStyle.Fill;
            _tlpControls.Controls.Add(_btnAdd, 3, 1); _btnAdd.Dock = DockStyle.Fill; _btnAdd.Text = "Добавить"; _btnAdd.Click += btnAdd_Click;

            // Строка 2: Поиск и Удаление
            _tlpControls.Controls.Add(lblSearch, 0, 2);
            _tlpControls.Controls.Add(_txtSearch, 0, 3); _txtSearch.Dock = DockStyle.Fill;

            _cmbSearchType.Items.AddRange(new object[] { "По названию", "По автору" });
            _cmbSearchType.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbSearchType.SelectedIndex = 0;
            _tlpControls.Controls.Add(_cmbSearchType, 1, 3); _cmbSearchType.Dock = DockStyle.Fill;

            _tlpControls.Controls.Add(_btnSearch, 2, 3); _btnSearch.Dock = DockStyle.Fill; _btnSearch.Text = "Поиск"; _btnSearch.Click += btnSearch_Click;
            _tlpControls.Controls.Add(_btnDelete, 3, 3); _btnDelete.Dock = DockStyle.Fill; _btnDelete.Text = "Удалить"; _btnDelete.Click += btnDelete_Click;
            _btnDelete.Enabled = true;

            pnlBottom.Controls.Add(_tlpControls);

            // Сборка формы
            Controls.Add(_dataGrid);
            Controls.Add(pnlBottom);

            Text = "Менеджер библиотеки";
            ResumeLayout(false);
            PerformLayout();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                var books = await _libMan.GetAllBooks();
                _dataGrid.DataSource = null;
                _dataGrid.DataSource = books;

                if (_dataGrid.Columns["Id"] != null) _dataGrid.Columns["Id"].HeaderText = "ID";
                if (_dataGrid.Columns["Title"] != null) _dataGrid.Columns["Title"].HeaderText = "Название";
                if (_dataGrid.Columns["Author"] != null) _dataGrid.Columns["Author"].HeaderText = "Автор";
                if (_dataGrid.Columns["Year"] != null) _dataGrid.Columns["Year"].HeaderText = "Год";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            _btnDelete.Enabled = true;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string term = _txtSearch.Text.Trim();
            try
            {
                List<Book> result;
                if (_cmbSearchType.SelectedIndex == 0)
                    result = await _libMan.SearchByTitle(term);
                else
                    result = await _libMan.SearchByAuthor(term);

                if (result == null || result.Count == 0)
                {
                    MessageBox.Show($"По запросу \"{term}\" ничего не найдено. Список сброшен.",
                        "Результаты поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _txtSearch.Clear();
                    await LoadDataAsync();
                    return;
                }

                _dataGrid.DataSource = null;
                _dataGrid.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTitle.Text) || string.IsNullOrWhiteSpace(_txtAuthor.Text) || string.IsNullOrWhiteSpace(_txtYear.Text))
            {
                MessageBox.Show("Заполните все поля для добавления книги.");
                return;
            }

            if (!int.TryParse(_txtYear.Text, out int year) || year < 0 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Введите корректный год.");
                return;
            }

            try
            {
                var book = new Book { Title = _txtTitle.Text.Trim(), Author = _txtAuthor.Text.Trim(), Year = year };
                await _libMan.AddBook(book);
                await LoadDataAsync();
                _txtTitle.Clear(); _txtAuthor.Clear(); _txtYear.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_dataGrid.SelectedRows.Count == 0) return;

            var selected = (Book)_dataGrid.SelectedRows[0].DataBoundItem;
            var confirm = MessageBox.Show($"Удалить книгу \"{selected.Title}\"?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                await _libMan.DeleteBook(selected.Id);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
        }
    }
}