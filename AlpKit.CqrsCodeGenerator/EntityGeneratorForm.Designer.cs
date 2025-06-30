namespace AlpKit.CqrsCodeGenerator
{
    partial class EntityGeneratorForm
    {
        private Label lblEntityName;
        private TextBox txtEntityName;

        private Label lblDbContext;
        private TextBox txtDbContextName;

        private Label lblPropName;
        private TextBox txtPropName;

        private Label lblPropType;
        private ComboBox cmbType;

        private Button btnAddProperty;
        private ListBox lstProperties;

        private Label lblCqrsType;
        private ComboBox cmbCqrsType;

        private Button btnGenerateEntity;
        private Button btnGenerateCqrs;
        private Button btnGenerateRepo;
        private Button btnGenerateValidator;
        
        private TextBox txtResult;

        private void InitializeComponent()
        {
            this.lblEntityName = new Label();
            this.txtEntityName = new TextBox();
            this.lblDbContext = new Label();
            this.txtDbContextName = new TextBox();
            this.lblPropName = new Label();
            this.txtPropName = new TextBox();
            this.lblPropType = new Label();
            this.cmbType = new ComboBox();
            this.btnAddProperty = new Button();
            this.lstProperties = new ListBox();
            this.lblCqrsType = new Label();
            this.cmbCqrsType = new ComboBox();
            this.btnGenerateEntity = new Button();
            this.btnGenerateCqrs = new Button();
            this.btnGenerateRepo = new Button();
            this.btnGenerateValidator = new Button();
            
            this.txtResult = new TextBox();

            this.SuspendLayout();

            int labelWidth = 110;
            int leftMargin = 30;
            int top = 20;
            int spacing = 35;

            // Entity Adı
            this.lblEntityName.Text = "🧩 Entity Adı:";
            this.lblEntityName.Location = new System.Drawing.Point(leftMargin, top);
            this.txtEntityName.Location = new System.Drawing.Point(leftMargin + labelWidth, top);
            this.txtEntityName.Size = new System.Drawing.Size(250, 23);

            // DbContext Adı
            top += spacing;
            this.lblDbContext.Text = "📦 DbContext:";
            this.lblDbContext.Location = new System.Drawing.Point(leftMargin, top);
            this.txtDbContextName.Location = new System.Drawing.Point(leftMargin + labelWidth, top);
            this.txtDbContextName.Size = new System.Drawing.Size(250, 23);

            // Property Adı
            top += spacing;
            this.lblPropName.Text = "🔤 Property Adı:";
            this.lblPropName.Location = new System.Drawing.Point(leftMargin, top);
            this.txtPropName.Location = new System.Drawing.Point(leftMargin + labelWidth, top);
            this.txtPropName.Size = new System.Drawing.Size(250, 23);

            // Property Tipi
            top += spacing;
            this.lblPropType.Text = "🧬 Property Tipi:";
            this.lblPropType.Location = new System.Drawing.Point(leftMargin, top);
            this.cmbType.Location = new System.Drawing.Point(leftMargin + labelWidth, top);
            this.cmbType.Size = new System.Drawing.Size(250, 23);
            this.cmbType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Ekle Butonu
            this.btnAddProperty.Text = "➕ Ekle";
            this.btnAddProperty.Location = new System.Drawing.Point(leftMargin + labelWidth + 260, top);
            this.btnAddProperty.Size = new System.Drawing.Size(75, 23);
            this.btnAddProperty.Click += new EventHandler(this.btnAddProperty_Click);

            // Property Listesi
            top += spacing + 10;
            this.lstProperties.Location = new System.Drawing.Point(leftMargin, top);
            this.lstProperties.Size = new System.Drawing.Size(500, 100);

            // CQRS Tipi
            top += 120;
            this.lblCqrsType.Text = "⚙️ CQRS Tipi:";
            this.lblCqrsType.Location = new System.Drawing.Point(leftMargin, top);
            this.cmbCqrsType.Location = new System.Drawing.Point(leftMargin + labelWidth, top);
            this.cmbCqrsType.Size = new System.Drawing.Size(250, 23);
            this.cmbCqrsType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Üretim Butonları
            top += spacing;
            this.btnGenerateEntity.Text = "🛠 Entity & Config";
            this.btnGenerateEntity.Location = new System.Drawing.Point(leftMargin, top);
            this.btnGenerateEntity.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateEntity.Click += new EventHandler(this.btnGenerate_Click);

            this.btnGenerateCqrs.Text = "🧱 CQRS Üret";
            this.btnGenerateCqrs.Location = new System.Drawing.Point(leftMargin + 150, top);
            this.btnGenerateCqrs.Size = new System.Drawing.Size(120, 35);
            this.btnGenerateCqrs.Click += new EventHandler(this.btnGenerateCqrs_Click);

            this.btnGenerateRepo.Text = "📁 Repository Üret";
            this.btnGenerateRepo.Location = new System.Drawing.Point(leftMargin + 280, top);
            this.btnGenerateRepo.Size = new System.Drawing.Size(140, 35);
            this.btnGenerateRepo.Click += new EventHandler(this.btnGenerateRepo_Click);

            this.btnGenerateValidator = new System.Windows.Forms.Button();
            this.btnGenerateValidator.Location = new System.Drawing.Point(leftMargin + 425, top);
            this.btnGenerateValidator.Size = new System.Drawing.Size(120, 35);
            this.btnGenerateValidator.Text = "Validator Üret";
            this.btnGenerateValidator.Click += new System.EventHandler(this.btnGenerateValidator_Click);

            // Sonuç
            top += 50;
            this.txtResult.Location = new System.Drawing.Point(leftMargin, top);
            this.txtResult.Size = new System.Drawing.Size(800, 300);
            this.txtResult.Multiline = true;
            this.txtResult.ScrollBars = ScrollBars.Both;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 10);

            // Form Ayarları
            this.ClientSize = new System.Drawing.Size(870, top + 330);
            this.Controls.AddRange(new Control[]
            {
            lblEntityName, txtEntityName,
            lblDbContext, txtDbContextName,
            lblPropName, txtPropName,
            lblPropType, cmbType,
            btnAddProperty, lstProperties,
            lblCqrsType, cmbCqrsType,
            btnGenerateEntity, btnGenerateCqrs, btnGenerateRepo,btnGenerateValidator,
            txtResult
            });

            this.Text = "AlpKit - Entity/CQRS/Repository Generator";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}