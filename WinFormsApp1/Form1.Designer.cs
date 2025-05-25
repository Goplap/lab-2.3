using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BulletinBoard.BLL.Interfaces;
using BulletinBoard.BLL.Models;

namespace BulletinBoard.UI
{
    public partial class BulletinBoardForm : Form
    {
        private readonly IAdService _adService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public BulletinBoardForm(IAdService adService, ICategoryService categoryService, IUserService userService)
        {
            InitializeComponent();
            _adService = adService;
            _categoryService = categoryService;
            _userService = userService;
        }

        private void InitializeComponent()
        {
            this.Text = "Дошка оголошень";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tabs
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Ads Tab
            TabPage adsTab = new TabPage("Оголошення");
            CreateAdsTab(adsTab);
            tabControl.TabPages.Add(adsTab);

            // Categories Tab
            TabPage categoriesTab = new TabPage("Категорії");
            CreateCategoriesTab(categoriesTab);
            tabControl.TabPages.Add(categoriesTab);

            // Users Tab
            TabPage usersTab = new TabPage("Користувачі");
            CreateUsersTab(usersTab);
            tabControl.TabPages.Add(usersTab);

            this.Controls.Add(tabControl);

            // Load data when form loads
            this.Load += BulletinBoardForm_Load;
        }

        private async void BulletinBoardForm_Load(object sender, EventArgs e)
        {
            await RefreshAllData();
        }

        private async Task RefreshAllData()
        {
            await LoadAds();
            await LoadCategories();
            await LoadUsers();
        }

        #region Ads Tab
        private ListView adsListView;
        private Button addAdButton;
        private Button editAdButton;
        private Button deleteAdButton;

        private void CreateAdsTab(TabPage tab)
        {
            // ListView for ads
            adsListView = new ListView();
            adsListView.Dock = DockStyle.Fill;
            adsListView.View = View.Details;
            adsListView.FullRowSelect = true;
            adsListView.Columns.Add("ID", 40);
            adsListView.Columns.Add("Заголовок", 200);
            adsListView.Columns.Add("Категорія", 120);
            adsListView.Columns.Add("Користувач", 120);
            adsListView.Columns.Add("Дата створення", 120);
            adsListView.Columns.Add("Активне", 60);

            // Buttons panel
            Panel buttonsPanel = new Panel();
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Height = 40;

            addAdButton = new Button();
            addAdButton.Text = "Додати";
            addAdButton.Location = new Point(10, 10);
            addAdButton.Click += AddAdButton_Click;

            editAdButton = new Button();
            editAdButton.Text = "Редагувати";
            editAdButton.Location = new Point(100, 10);
            editAdButton.Click += EditAdButton_Click;

            deleteAdButton = new Button();
            deleteAdButton.Text = "Видалити";
            deleteAdButton.Location = new Point(190, 10);
            deleteAdButton.Click += DeleteAdButton_Click;

            buttonsPanel.Controls.Add(addAdButton);
            buttonsPanel.Controls.Add(editAdButton);
            buttonsPanel.Controls.Add(deleteAdButton);

            tab.Controls.Add(adsListView);
            tab.Controls.Add(buttonsPanel);
        }

        private async Task LoadAds()
        {
            adsListView.Items.Clear();
            var ads = await _adService.GetAllAdsAsync();

            foreach (var ad in ads)
            {
                var item = new ListViewItem(ad.Id.ToString());
                item.SubItems.Add(ad.Title);
                item.SubItems.Add(ad.Category?.Name ?? "N/A");
                item.SubItems.Add(ad.User?.Username ?? "N/A");
                item.SubItems.Add(ad.CreatedAt.ToShortDateString());
                item.SubItems.Add(ad.IsActive ? "Так" : "Ні");
                item.Tag = ad;

                adsListView.Items.Add(item);
            }
        }

        private void AddAdButton_Click(object sender, EventArgs e)
        {
            using (var form = new AdForm(_categoryService, _userService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _adService.CreateAdAsync(form.Ad);
                        await LoadAds();
                    });
                }
            }
        }

        private void EditAdButton_Click(object sender, EventArgs e)
        {
            if (adsListView.SelectedItems.Count == 0) return;

            var ad = (AdDto)adsListView.SelectedItems[0].Tag;
            using (var form = new AdForm(_categoryService, _userService, ad))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _adService.UpdateAdAsync(form.Ad);
                        await LoadAds();
                    });
                }
            }
        }

        private void DeleteAdButton_Click(object sender, EventArgs e)
        {
            if (adsListView.SelectedItems.Count == 0) return;

            var ad = (AdDto)adsListView.SelectedItems[0].Tag;
            if (MessageBox.Show($"Видалити оголошення '{ad.Title}'?", "Підтвердження",
                  MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Task.Run(async () =>
                {
                    await _adService.DeleteAdAsync(ad.Id);
                    await LoadAds();
                });
            }
        }
        #endregion

        #region Categories Tab
        private ListView categoriesListView;
        private Button addCategoryButton;
        private Button editCategoryButton;
        private Button deleteCategoryButton;

        private void CreateCategoriesTab(TabPage tab)
        {
            // ListView for categories
            categoriesListView = new ListView();
            categoriesListView.Dock = DockStyle.Fill;
            categoriesListView.View = View.Details;
            categoriesListView.FullRowSelect = true;
            categoriesListView.Columns.Add("ID", 40);
            categoriesListView.Columns.Add("Назва", 200);
            categoriesListView.Columns.Add("Батьківська категорія", 150);

            // Buttons panel
            Panel buttonsPanel = new Panel();
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Height = 40;

            addCategoryButton = new Button();
            addCategoryButton.Text = "Додати";
            addCategoryButton.Location = new Point(10, 10);
            addCategoryButton.Click += AddCategoryButton_Click;

            editCategoryButton = new Button();
            editCategoryButton.Text = "Редагувати";
            editCategoryButton.Location = new Point(100, 10);
            editCategoryButton.Click += EditCategoryButton_Click;

            deleteCategoryButton = new Button();
            deleteCategoryButton.Text = "Видалити";
            deleteCategoryButton.Location = new Point(190, 10);
            deleteCategoryButton.Click += DeleteCategoryButton_Click;

            buttonsPanel.Controls.Add(addCategoryButton);
            buttonsPanel.Controls.Add(editCategoryButton);
            buttonsPanel.Controls.Add(deleteCategoryButton);

            tab.Controls.Add(categoriesListView);
            tab.Controls.Add(buttonsPanel);
        }

        private async Task LoadCategories()
        {
            categoriesListView.Items.Clear();
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoriesDict = categories.ToDictionary(c => c.Id);

            foreach (var category in categories)
            {
                var item = new ListViewItem(category.Id.ToString());
                item.SubItems.Add(category.Name);

                string parentName = "—";
                if (category.ParentCategoryId.HasValue &&
                    categoriesDict.TryGetValue(category.ParentCategoryId.Value, out var parentCategory))
                {
                    parentName = parentCategory.Name;
                }

                item.SubItems.Add(parentName);
                item.Tag = category;

                categoriesListView.Items.Add(item);
            }
        }

        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            using (var form = new CategoryForm(_categoryService))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _categoryService.CreateCategoryAsync(form.Category);
                        await LoadCategories();
                    });
                }
            }
        }

        private void EditCategoryButton_Click(object sender, EventArgs e)
        {
            if (categoriesListView.SelectedItems.Count == 0) return;

            var category = (CategoryDto)categoriesListView.SelectedItems[0].Tag;
            using (var form = new CategoryForm(_categoryService, category))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _categoryService.UpdateCategoryAsync(form.Category);
                        await LoadCategories();
                    });
                }
            }
        }

        private void DeleteCategoryButton_Click(object sender, EventArgs e)
        {
            if (categoriesListView.SelectedItems.Count == 0) return;

            var category = (CategoryDto)categoriesListView.SelectedItems[0].Tag;
            if (MessageBox.Show($"Видалити категорію '{category.Name}'?", "Підтвердження",
                  MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Task.Run(async () =>
                {
                    await _categoryService.DeleteCategoryAsync(category.Id);
                    await LoadCategories();
                });
            }
        }
        #endregion

        #region Users Tab
        private ListView usersListView;
        private Button addUserButton;
        private Button editUserButton;
        private Button deleteUserButton;

        private void CreateUsersTab(TabPage tab)
        {
            // ListView for users
            usersListView = new ListView();
            usersListView.Dock = DockStyle.Fill;
            usersListView.View = View.Details;
            usersListView.FullRowSelect = true;
            usersListView.Columns.Add("ID", 40);
            usersListView.Columns.Add("Ім'я користувача", 150);
            usersListView.Columns.Add("Email", 200);
            usersListView.Columns.Add("Дата реєстрації", 120);
            usersListView.Columns.Add("Активний", 60);

            // Buttons panel
            Panel buttonsPanel = new Panel();
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Height = 40;

            addUserButton = new Button();
            addUserButton.Text = "Додати";
            addUserButton.Location = new Point(10, 10);
            addUserButton.Click += AddUserButton_Click;

            editUserButton = new Button();
            editUserButton.Text = "Редагувати";
            editUserButton.Location = new Point(100, 10);
            editUserButton.Click += EditUserButton_Click;

            deleteUserButton = new Button();
            deleteUserButton.Text = "Видалити";
            deleteUserButton.Location = new Point(190, 10);
            deleteUserButton.Click += DeleteUserButton_Click;

            buttonsPanel.Controls.Add(addUserButton);
            buttonsPanel.Controls.Add(editUserButton);
            buttonsPanel.Controls.Add(deleteUserButton);

            tab.Controls.Add(usersListView);
            tab.Controls.Add(buttonsPanel);
        }

        private async Task LoadUsers()
        {
            usersListView.Items.Clear();
            var users = await _userService.GetAllUsersAsync();

            foreach (var user in users)
            {
                var item = new ListViewItem(user.Id.ToString());
                item.SubItems.Add(user.Username);
                item.SubItems.Add(user.Email);
                item.SubItems.Add(user.RegisteredAt.ToShortDateString());
                item.SubItems.Add(user.IsActive ? "Так" : "Ні");
                item.Tag = user;

                usersListView.Items.Add(item);
            }
        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            using (var form = new UserForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _userService.CreateUserAsync(form.User, form.Password);
                        await LoadUsers();
                    });
                }
            }
        }

        private void EditUserButton_Click(object sender, EventArgs e)
        {
            if (usersListView.SelectedItems.Count == 0) return;

            var user = (UserDto)usersListView.SelectedItems[0].Tag;
            using (var form = new UserForm(user))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(async () =>
                    {
                        await _userService.UpdateUserAsync(form.User);
                        await LoadUsers();
                    });
                }
            }
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (usersListView.SelectedItems.Count == 0) return;

            var user = (UserDto)usersListView.SelectedItems[0].Tag;
            if (MessageBox.Show($"Видалити користувача '{user.Username}'?", "Підтвердження",
                  MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Task.Run(async () =>
                {
                    await _userService.DeleteUserAsync(user.Id);
                    await LoadUsers();
                });
            }
        }
        #endregion
    }

    // Форма для редагування оголошення
    public class AdForm : Form
    {
        private TextBox titleTextBox;
        private TextBox descriptionTextBox;
        private ComboBox categoryComboBox;
        private ComboBox userComboBox;
        private CheckBox activeCheckBox;
        private Button saveButton;
        private Button cancelButton;

        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public AdDto Ad { get; private set; }

        public AdForm(ICategoryService categoryService, IUserService userService, AdDto ad = null)
        {
            _categoryService = categoryService;
            _userService = userService;
            Ad = ad ?? new AdDto();

            InitializeComponent();
            LoadFormData();
        }

        private void InitializeComponent()
        {
            this.Text = Ad.Id == 0 ? "Нове оголошення" : "Редагування оголошення";
            this.Size = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Controls
            Label titleLabel = new Label();
            titleLabel.Text = "Заголовок:";
            titleLabel.Location = new Point(10, 15);

            titleTextBox = new TextBox();
            titleTextBox.Location = new Point(120, 12);
            titleTextBox.Width = 250;

            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Опис:";
            descriptionLabel.Location = new Point(10, 45);

            descriptionTextBox = new TextBox();
            descriptionTextBox.Location = new Point(120, 42);
            descriptionTextBox.Width = 250;
            descriptionTextBox.Height = 100;
            descriptionTextBox.Multiline = true;

            Label categoryLabel = new Label();
            categoryLabel.Text = "Категорія:";
            categoryLabel.Location = new Point(10, 155);

            categoryComboBox = new ComboBox();
            categoryComboBox.Location = new Point(120, 152);
            categoryComboBox.Width = 250;
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            Label userLabel = new Label();
            userLabel.Text = "Користувач:";
            userLabel.Location = new Point(10, 185);

            userComboBox = new ComboBox();
            userComboBox.Location = new Point(120, 182);
            userComboBox.Width = 250;
            userComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            activeCheckBox = new CheckBox();
            activeCheckBox.Text = "Активне";
            activeCheckBox.Location = new Point(120, 212);
            activeCheckBox.Checked = true;

            saveButton = new Button();
            saveButton.Text = "Зберегти";
            saveButton.Location = new Point(120, 250);
            saveButton.DialogResult = DialogResult.OK;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button();
            cancelButton.Text = "Скасувати";
            cancelButton.Location = new Point(210, 250);
            cancelButton.DialogResult = DialogResult.Cancel;

            this.Controls.Add(titleLabel);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(descriptionLabel);
            this.Controls.Add(descriptionTextBox);
            this.Controls.Add(categoryLabel);
            this.Controls.Add(categoryComboBox);
            this.Controls.Add(userLabel);
            this.Controls.Add(userComboBox);
            this.Controls.Add(activeCheckBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private async void LoadFormData()
        {
            // Заповнюємо поля форми, якщо редагуємо існуюче оголошення
            titleTextBox.Text = Ad.Title;
            descriptionTextBox.Text = Ad.Description;
            activeCheckBox.Checked = Ad.IsActive;

            // Завантажуємо категорії
            var categories = await _categoryService.GetAllCategoriesAsync();
            categoryComboBox.DisplayMember = "Name";
            categoryComboBox.ValueMember = "Id";
            categoryComboBox.DataSource = categories.ToList();

            if (Ad.CategoryId > 0)
            {
                categoryComboBox.SelectedValue = Ad.CategoryId;
            }

            // Завантажуємо користувачів
            var users = await _userService.GetAllUsersAsync();
            userComboBox.DisplayMember = "Username";
            userComboBox.ValueMember = "Id";
            userComboBox.DataSource = users.ToList();

            if (Ad.UserId > 0)
            {
                userComboBox.SelectedValue = Ad.UserId;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Введіть заголовок оголошення");
                DialogResult = DialogResult.None;
                return;
            }

            // Збереження даних
            Ad.Title = titleTextBox.Text;
            Ad.Description = descriptionTextBox.Text;
            Ad.IsActive = activeCheckBox.Checked;

            if (categoryComboBox.SelectedItem is CategoryDto selectedCategory)
            {
                Ad.CategoryId = selectedCategory.Id;
            }

            if (userComboBox.SelectedItem is UserDto selectedUser)
            {
                Ad.UserId = selectedUser.Id;
            }
        }
    }

    // Форма для редагування категорії
    public class CategoryForm : Form
    {
        private TextBox nameTextBox;
        private ComboBox parentCategoryComboBox;
        private Button saveButton;
        private Button cancelButton;

        private readonly ICategoryService _categoryService;

        public CategoryDto Category { get; private set; }

        public CategoryForm(ICategoryService categoryService, CategoryDto category = null)
        {
            _categoryService = categoryService;
            Category = category ?? new CategoryDto();

            InitializeComponent();
            LoadFormData();
        }

        private void InitializeComponent()
        {
            this.Text = Category.Id == 0 ? "Нова категорія" : "Редагування категорії";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Controls
            Label nameLabel = new Label();
            nameLabel.Text = "Назва:";
            nameLabel.Location = new Point(10, 15);

            nameTextBox = new TextBox();
            nameTextBox.Location = new Point(120, 12);
            nameTextBox.Width = 250;

            Label parentLabel = new Label();
            parentLabel.Text = "Батьківська категорія:";
            parentLabel.Location = new Point(10, 45);

            parentCategoryComboBox = new ComboBox();
            parentCategoryComboBox.Location = new Point(120, 42);
            parentCategoryComboBox.Width = 250;
            parentCategoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            saveButton = new Button();
            saveButton.Text = "Зберегти";
            saveButton.Location = new Point(120, 120);
            saveButton.DialogResult = DialogResult.OK;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button();
            cancelButton.Text = "Скасувати";
            cancelButton.Location = new Point(210, 120);
            cancelButton.DialogResult = DialogResult.Cancel;

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(parentLabel);
            this.Controls.Add(parentCategoryComboBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private async void LoadFormData()
        {
            // Заповнюємо поля форми, якщо редагуємо існуючу категорію
            nameTextBox.Text = Category.Name;

            // Завантажуємо батьківські категорії
            var categories = await _categoryService.GetAllCategoriesAsync();

            // Додаємо "немає" як перший елемент
            var parentCategories = new List<CategoryDto>
            {
                new CategoryDto { Id = 0, Name = "-- Немає батьківської категорії --" }
            };

            // Додаємо всі категорії крім поточної (щоб уникнути циклічних посилань)
            parentCategories.AddRange(categories.Where(c => c.Id != Category.Id));

            parentCategoryComboBox.DisplayMember = "Name";
            parentCategoryComboBox.ValueMember = "Id";
            parentCategoryComboBox.DataSource = parentCategories;

            if (Category.ParentCategoryId.HasValue && Category.ParentCategoryId.Value > 0)
            {
                parentCategoryComboBox.SelectedValue = Category.ParentCategoryId.Value;
            }
            else
            {
                parentCategoryComboBox.SelectedIndex = 0; // "немає"
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Введіть назву категорії");
                DialogResult = DialogResult.None;
                return;
            }

            // Збереження даних
            Category.Name = nameTextBox.Text;

            if (parentCategoryComboBox.SelectedValue is int parentId && parentId > 0)
            {
                Category.ParentCategoryId = parentId;
            }
            else
            {
                Category.ParentCategoryId = null;
            }
        }
    }

    // Форма для редагування користувача
    public class UserForm : Form
    {
        private TextBox usernameTextBox;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private CheckBox activeCheckBox;
        private Button saveButton;
        private Button cancelButton;

        public UserDto User { get; private set; }
        public string Password => passwordTextBox.Text;

        public UserForm(UserDto user = null)
        {
            User = user ?? new UserDto();

            InitializeComponent();
            LoadFormData();
        }

        private void InitializeComponent()
        {
            this.Text = User.Id == 0 ? "Новий користувач" : "Редагування користувача";
            this.Size = new Size(400, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Controls
            Label usernameLabel = new Label();
            usernameLabel.Text = "Ім'я користувача:";
            usernameLabel.Location = new Point(10, 15);

            usernameTextBox = new TextBox();
            usernameTextBox.Location = new Point(120, 12);
            usernameTextBox.Width = 250;

            Label emailLabel = new Label();
            emailLabel.Text = "Email:";
            emailLabel.Location = new Point(10, 45);

            emailTextBox = new TextBox();
            emailTextBox.Location = new Point(120, 42);
            emailTextBox.Width = 250;

            Label passwordLabel = new Label();
            passwordLabel.Text = "Пароль:";
            passwordLabel.Location = new Point(10, 75);

            passwordTextBox = new TextBox();
            passwordTextBox.Location = new Point(120, 72);
            passwordTextBox.Width = 250;
            passwordTextBox.UseSystemPasswordChar = true;

            activeCheckBox = new CheckBox();
            activeCheckBox.Text = "Активний";
            activeCheckBox.Location = new Point(120, 102);
            activeCheckBox.Checked = true;

            saveButton = new Button();
            saveButton.Text = "Зберегти";
            saveButton.Location = new Point(120, 140);
            saveButton.DialogResult = DialogResult.OK;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button();
            cancelButton.Text = "Скасувати";
            cancelButton.Location = new Point(210, 140);
            cancelButton.DialogResult = DialogResult.Cancel;

            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(emailLabel);
            this.Controls.Add(emailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(activeCheckBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private void LoadFormData()
        {
            // Заповнюємо поля форми, якщо редагуємо існуючого користувача
            usernameTextBox.Text = User.Username;
            emailTextBox.Text = User.Email;
            activeCheckBox.Checked = User.IsActive;

            if (User.Id > 0)
            {
                // При редагуванні пароль не заповнюємо
                passwordTextBox.Text = string.Empty;

                // Можемо показати підказку
                Label passwordHintLabel = new Label();
                passwordHintLabel.Text = "Залиште порожнім, щоб не змінювати пароль";
                passwordHintLabel.Location = new Point(120, 95);
                passwordHintLabel.Size = new Size(250, 15);
                passwordHintLabel.ForeColor = Color.Gray;
                passwordHintLabel.Font = new Font(passwordHintLabel.Font.FontFamily, 7);
                this.Controls.Add(passwordHintLabel);

                // Зсуваємо чекбокс нижче
                activeCheckBox.Location = new Point(120, 115);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Валідація
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show("Введіть ім'я користувача");
                DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Введіть email користувача");
                DialogResult = DialogResult.None;
                return;
            }

            if (User.Id == 0 && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Введіть пароль для нового користувача");
                DialogResult = DialogResult.None;
                return;
            }

            // Збереження даних
            User.Username = usernameTextBox.Text;
            User.Email = emailTextBox.Text;
            User.IsActive = activeCheckBox.Checked;

            // Пароль обробляється в BulletinBoardForm при створенні/оновленні користувача
        }
    }
}