namespace DroneServiceCenter.App
{
    partial class Form1
    {
        private System.ComponentModel.IContainer? components = null;

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;

        private System.Windows.Forms.DataGridView dataGridViewDrones;
        private System.Windows.Forms.Button btnReadyDrones;
        private System.Windows.Forms.Button btnAllDrones;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFirmware;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPropHours;
        private System.Windows.Forms.CheckBox chkCrashMarks;
        private System.Windows.Forms.Button btnAddDrone;
        private System.Windows.Forms.Button btnUpdateDrone;
        private System.Windows.Forms.Button btnDeleteDrone;
        private System.Windows.Forms.Button btnClearDrone;

        private System.Windows.Forms.DataGridView dataGridViewRepairs;
        private System.Windows.Forms.Button btnOpenRepairs;
        private System.Windows.Forms.Button btnAllRepairs;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbRepairDrone;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpRepairOpened;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRepairIssue;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtRepairParts;
        private System.Windows.Forms.CheckBox chkTestFlight;
        private System.Windows.Forms.CheckBox chkIsClosed;
        private System.Windows.Forms.DateTimePicker dtpRepairClosed;
        private System.Windows.Forms.Button btnAddRepair;
        private System.Windows.Forms.Button btnUpdateRepair;
        private System.Windows.Forms.Button btnDeleteRepair;
        private System.Windows.Forms.Button btnClearRepair;

        private System.Windows.Forms.DataGridView dataGridViewFlights;
        private System.Windows.Forms.Button btnRiskFlights;
        private System.Windows.Forms.Button btnAllFlights;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbFlightDrone;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpFlightDate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtPilot;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numRth;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.CheckBox chkSignalLost;
        private System.Windows.Forms.Button btnAddFlight;
        private System.Windows.Forms.Button btnUpdateFlight;
        private System.Windows.Forms.Button btnDeleteFlight;
        private System.Windows.Forms.Button btnClearFlight;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            tabPage2 = new System.Windows.Forms.TabPage();
            tabPage3 = new System.Windows.Forms.TabPage();

            dataGridViewDrones = new System.Windows.Forms.DataGridView();
            btnReadyDrones = new System.Windows.Forms.Button();
            btnAllDrones = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtSerial = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtModel = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtFirmware = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            cmbStatus = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            numPropHours = new System.Windows.Forms.NumericUpDown();
            chkCrashMarks = new System.Windows.Forms.CheckBox();
            btnAddDrone = new System.Windows.Forms.Button();
            btnUpdateDrone = new System.Windows.Forms.Button();
            btnDeleteDrone = new System.Windows.Forms.Button();
            btnClearDrone = new System.Windows.Forms.Button();

            dataGridViewRepairs = new System.Windows.Forms.DataGridView();
            btnOpenRepairs = new System.Windows.Forms.Button();
            btnAllRepairs = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            cmbRepairDrone = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            dtpRepairOpened = new System.Windows.Forms.DateTimePicker();
            label8 = new System.Windows.Forms.Label();
            txtRepairIssue = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            txtRepairParts = new System.Windows.Forms.TextBox();
            chkTestFlight = new System.Windows.Forms.CheckBox();
            chkIsClosed = new System.Windows.Forms.CheckBox();
            dtpRepairClosed = new System.Windows.Forms.DateTimePicker();
            btnAddRepair = new System.Windows.Forms.Button();
            btnUpdateRepair = new System.Windows.Forms.Button();
            btnDeleteRepair = new System.Windows.Forms.Button();
            btnClearRepair = new System.Windows.Forms.Button();

            dataGridViewFlights = new System.Windows.Forms.DataGridView();
            btnRiskFlights = new System.Windows.Forms.Button();
            btnAllFlights = new System.Windows.Forms.Button();
            label10 = new System.Windows.Forms.Label();
            cmbFlightDrone = new System.Windows.Forms.ComboBox();
            label11 = new System.Windows.Forms.Label();
            dtpFlightDate = new System.Windows.Forms.DateTimePicker();
            label12 = new System.Windows.Forms.Label();
            txtPilot = new System.Windows.Forms.TextBox();
            label13 = new System.Windows.Forms.Label();
            txtLocation = new System.Windows.Forms.TextBox();
            label14 = new System.Windows.Forms.Label();
            numRth = new System.Windows.Forms.NumericUpDown();
            label15 = new System.Windows.Forms.Label();
            numDuration = new System.Windows.Forms.NumericUpDown();
            chkSignalLost = new System.Windows.Forms.CheckBox();
            btnAddFlight = new System.Windows.Forms.Button();
            btnUpdateFlight = new System.Windows.Forms.Button();
            btnDeleteFlight = new System.Windows.Forms.Button();
            btnClearFlight = new System.Windows.Forms.Button();

            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewDrones).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPropHours).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRepairs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFlights).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).BeginInit();
            SuspendLayout();

            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1024, 581);
            tabControl1.TabIndex = 0;

            // --- TAB 1: ДРОНИ ---
            tabPage1.Controls.Add(btnClearDrone);
            tabPage1.Controls.Add(btnDeleteDrone);
            tabPage1.Controls.Add(btnUpdateDrone);
            tabPage1.Controls.Add(btnAddDrone);
            tabPage1.Controls.Add(chkCrashMarks);
            tabPage1.Controls.Add(numPropHours);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(cmbStatus);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(txtFirmware);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(txtModel);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(txtSerial);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(btnAllDrones);
            tabPage1.Controls.Add(btnReadyDrones);
            tabPage1.Controls.Add(dataGridViewDrones);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(1016, 553);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Дрони";
            tabPage1.UseVisualStyleBackColor = true;

            btnReadyDrones.Location = new System.Drawing.Point(12, 12);
            btnReadyDrones.Name = "btnReadyDrones";
            btnReadyDrones.Size = new System.Drawing.Size(145, 29);
            btnReadyDrones.Text = "Готові до вильоту";
            btnReadyDrones.UseVisualStyleBackColor = true;
            btnReadyDrones.Click += btnReadyDrones_Click;

            btnAllDrones.Location = new System.Drawing.Point(165, 12);
            btnAllDrones.Name = "btnAllDrones";
            btnAllDrones.Size = new System.Drawing.Size(115, 29);
            btnAllDrones.Text = "Показати всі";
            btnAllDrones.UseVisualStyleBackColor = true;
            btnAllDrones.Click += btnAllDrones_Click;

            dataGridViewDrones.AllowUserToAddRows = false;
            dataGridViewDrones.AllowUserToDeleteRows = false;
            dataGridViewDrones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewDrones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewDrones.Location = new System.Drawing.Point(12, 47);
            dataGridViewDrones.MultiSelect = false;
            dataGridViewDrones.Name = "dataGridViewDrones";
            dataGridViewDrones.ReadOnly = true;
            dataGridViewDrones.RowHeadersVisible = false;
            dataGridViewDrones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewDrones.Size = new System.Drawing.Size(705, 490);
            dataGridViewDrones.SelectionChanged += dataGridViewDrones_SelectionChanged;

            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(735, 18);
            label1.Text = "Серійний номер:";

            txtSerial.Location = new System.Drawing.Point(735, 38);
            txtSerial.Name = "txtSerial";
            txtSerial.Size = new System.Drawing.Size(260, 23);

            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(735, 70);
            label2.Text = "Модель:";

            txtModel.Location = new System.Drawing.Point(735, 90);
            txtModel.Name = "txtModel";
            txtModel.Size = new System.Drawing.Size(260, 23);

            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(735, 122);
            label3.Text = "Версія прошивки:";

            txtFirmware.Location = new System.Drawing.Point(735, 142);
            txtFirmware.Name = "txtFirmware";
            txtFirmware.Size = new System.Drawing.Size(260, 23);

            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(735, 174);
            label4.Text = "Статус:";

            cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Items.AddRange(new object[] { "У строю", "На ремонті", "Списаний" });
            cmbStatus.Location = new System.Drawing.Point(735, 194);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new System.Drawing.Size(260, 23);

            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(735, 228);
            label5.Text = "Наліт гвинтів (год):";

            numPropHours.Location = new System.Drawing.Point(735, 248);
            numPropHours.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numPropHours.Name = "numPropHours";
            numPropHours.Size = new System.Drawing.Size(120, 23);

            chkCrashMarks.AutoSize = true;
            chkCrashMarks.Location = new System.Drawing.Point(735, 285);
            chkCrashMarks.Name = "chkCrashMarks";
            chkCrashMarks.Size = new System.Drawing.Size(113, 19);
            chkCrashMarks.Text = "Є сліди падінь";
            chkCrashMarks.UseVisualStyleBackColor = true;

            btnAddDrone.Location = new System.Drawing.Point(735, 320);
            btnAddDrone.Name = "btnAddDrone";
            btnAddDrone.Size = new System.Drawing.Size(125, 30);
            btnAddDrone.Text = "Додати";
            btnAddDrone.UseVisualStyleBackColor = true;
            btnAddDrone.Click += btnAddDrone_Click;

            btnUpdateDrone.Location = new System.Drawing.Point(870, 320);
            btnUpdateDrone.Name = "btnUpdateDrone";
            btnUpdateDrone.Size = new System.Drawing.Size(125, 30);
            btnUpdateDrone.Text = "Зберегти";
            btnUpdateDrone.UseVisualStyleBackColor = true;
            btnUpdateDrone.Click += btnUpdateDrone_Click;

            btnDeleteDrone.Location = new System.Drawing.Point(735, 358);
            btnDeleteDrone.Name = "btnDeleteDrone";
            btnDeleteDrone.Size = new System.Drawing.Size(125, 30);
            btnDeleteDrone.Text = "Видалити";
            btnDeleteDrone.UseVisualStyleBackColor = true;
            btnDeleteDrone.Click += btnDeleteDrone_Click;

            btnClearDrone.Location = new System.Drawing.Point(870, 358);
            btnClearDrone.Name = "btnClearDrone";
            btnClearDrone.Size = new System.Drawing.Size(125, 30);
            btnClearDrone.Text = "Очистити";
            btnClearDrone.UseVisualStyleBackColor = true;
            btnClearDrone.Click += btnClearDrone_Click;

            // --- TAB 2: РЕМОНТНІ НАРЯДИ ---
            tabPage2.Controls.Add(btnClearRepair);
            tabPage2.Controls.Add(btnDeleteRepair);
            tabPage2.Controls.Add(btnUpdateRepair);
            tabPage2.Controls.Add(btnAddRepair);
            tabPage2.Controls.Add(dtpRepairClosed);
            tabPage2.Controls.Add(chkIsClosed);
            tabPage2.Controls.Add(chkTestFlight);
            tabPage2.Controls.Add(txtRepairParts);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(txtRepairIssue);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(dtpRepairOpened);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(cmbRepairDrone);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(btnAllRepairs);
            tabPage2.Controls.Add(btnOpenRepairs);
            tabPage2.Controls.Add(dataGridViewRepairs);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(1016, 553);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Ремонтні наряди";
            tabPage2.UseVisualStyleBackColor = true;

            btnOpenRepairs.Location = new System.Drawing.Point(12, 12);
            btnOpenRepairs.Name = "btnOpenRepairs";
            btnOpenRepairs.Size = new System.Drawing.Size(135, 29);
            btnOpenRepairs.Text = "Відкриті наряди";
            btnOpenRepairs.UseVisualStyleBackColor = true;
            btnOpenRepairs.Click += btnOpenRepairs_Click;

            btnAllRepairs.Location = new System.Drawing.Point(155, 12);
            btnAllRepairs.Name = "btnAllRepairs";
            btnAllRepairs.Size = new System.Drawing.Size(115, 29);
            btnAllRepairs.Text = "Показати всі";
            btnAllRepairs.UseVisualStyleBackColor = true;
            btnAllRepairs.Click += btnAllRepairs_Click;

            dataGridViewRepairs.AllowUserToAddRows = false;
            dataGridViewRepairs.AllowUserToDeleteRows = false;
            dataGridViewRepairs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewRepairs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRepairs.Location = new System.Drawing.Point(12, 47);
            dataGridViewRepairs.MultiSelect = false;
            dataGridViewRepairs.Name = "dataGridViewRepairs";
            dataGridViewRepairs.ReadOnly = true;
            dataGridViewRepairs.RowHeadersVisible = false;
            dataGridViewRepairs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewRepairs.Size = new System.Drawing.Size(705, 490);
            dataGridViewRepairs.SelectionChanged += dataGridViewRepairs_SelectionChanged;

            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(735, 18);
            label6.Text = "Дрон:";

            cmbRepairDrone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbRepairDrone.Location = new System.Drawing.Point(735, 38);
            cmbRepairDrone.Name = "cmbRepairDrone";
            cmbRepairDrone.Size = new System.Drawing.Size(260, 23);

            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(735, 70);
            label7.Text = "Дата відкриття:";

            dtpRepairOpened.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpRepairOpened.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtpRepairOpened.Location = new System.Drawing.Point(735, 90);
            dtpRepairOpened.Name = "dtpRepairOpened";
            dtpRepairOpened.Size = new System.Drawing.Size(260, 23);

            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(735, 122);
            label8.Text = "Опис несправності:";

            txtRepairIssue.Location = new System.Drawing.Point(735, 142);
            txtRepairIssue.Multiline = true;
            txtRepairIssue.Name = "txtRepairIssue";
            txtRepairIssue.Size = new System.Drawing.Size(260, 55);

            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(735, 205);
            label9.Text = "Замінені деталі:";

            txtRepairParts.Location = new System.Drawing.Point(735, 225);
            txtRepairParts.Name = "txtRepairParts";
            txtRepairParts.Size = new System.Drawing.Size(260, 23);

            chkTestFlight.AutoSize = true;
            chkTestFlight.Location = new System.Drawing.Point(735, 260);
            chkTestFlight.Name = "chkTestFlight";
            chkTestFlight.Size = new System.Drawing.Size(192, 19);
            chkTestFlight.Text = "Контрольний обліт пройдено";
            chkTestFlight.UseVisualStyleBackColor = true;

            chkIsClosed.AutoSize = true;
            chkIsClosed.Location = new System.Drawing.Point(735, 286);
            chkIsClosed.Name = "chkIsClosed";
            chkIsClosed.Size = new System.Drawing.Size(107, 19);
            chkIsClosed.Text = "Наряд закрито";
            chkIsClosed.UseVisualStyleBackColor = true;
            chkIsClosed.CheckedChanged += chkIsClosed_CheckedChanged;

            dtpRepairClosed.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpRepairClosed.Enabled = false;
            dtpRepairClosed.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtpRepairClosed.Location = new System.Drawing.Point(735, 312);
            dtpRepairClosed.Name = "dtpRepairClosed";
            dtpRepairClosed.Size = new System.Drawing.Size(260, 23);

            btnAddRepair.Location = new System.Drawing.Point(735, 350);
            btnAddRepair.Name = "btnAddRepair";
            btnAddRepair.Size = new System.Drawing.Size(125, 30);
            btnAddRepair.Text = "Додати";
            btnAddRepair.UseVisualStyleBackColor = true;
            btnAddRepair.Click += btnAddRepair_Click;

            btnUpdateRepair.Location = new System.Drawing.Point(870, 350);
            btnUpdateRepair.Name = "btnUpdateRepair";
            btnUpdateRepair.Size = new System.Drawing.Size(125, 30);
            btnUpdateRepair.Text = "Зберегти";
            btnUpdateRepair.UseVisualStyleBackColor = true;
            btnUpdateRepair.Click += btnUpdateRepair_Click;

            btnDeleteRepair.Location = new System.Drawing.Point(735, 388);
            btnDeleteRepair.Name = "btnDeleteRepair";
            btnDeleteRepair.Size = new System.Drawing.Size(125, 30);
            btnDeleteRepair.Text = "Видалити";
            btnDeleteRepair.UseVisualStyleBackColor = true;
            btnDeleteRepair.Click += btnDeleteRepair_Click;

            btnClearRepair.Location = new System.Drawing.Point(870, 388);
            btnClearRepair.Name = "btnClearRepair";
            btnClearRepair.Size = new System.Drawing.Size(125, 30);
            btnClearRepair.Text = "Очистити";
            btnClearRepair.UseVisualStyleBackColor = true;
            btnClearRepair.Click += btnClearRepair_Click;

            // --- TAB 3: ЖУРНАЛ ПОЛЬОТІВ ---
            tabPage3.Controls.Add(btnClearFlight);
            tabPage3.Controls.Add(btnDeleteFlight);
            tabPage3.Controls.Add(btnUpdateFlight);
            tabPage3.Controls.Add(btnAddFlight);
            tabPage3.Controls.Add(chkSignalLost);
            tabPage3.Controls.Add(numDuration);
            tabPage3.Controls.Add(label15);
            tabPage3.Controls.Add(numRth);
            tabPage3.Controls.Add(label14);
            tabPage3.Controls.Add(txtLocation);
            tabPage3.Controls.Add(label13);
            tabPage3.Controls.Add(txtPilot);
            tabPage3.Controls.Add(label12);
            tabPage3.Controls.Add(dtpFlightDate);
            tabPage3.Controls.Add(label11);
            tabPage3.Controls.Add(cmbFlightDrone);
            tabPage3.Controls.Add(label10);
            tabPage3.Controls.Add(btnAllFlights);
            tabPage3.Controls.Add(btnRiskFlights);
            tabPage3.Controls.Add(dataGridViewFlights);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(1016, 553);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Журнал польотів";
            tabPage3.UseVisualStyleBackColor = true;

            btnRiskFlights.Location = new System.Drawing.Point(12, 12);
            btnRiskFlights.Name = "btnRiskFlights";
            btnRiskFlights.Size = new System.Drawing.Size(145, 29);
            btnRiskFlights.Text = "Ризикові вильоти";
            btnRiskFlights.UseVisualStyleBackColor = true;
            btnRiskFlights.Click += btnRiskFlights_Click;

            btnAllFlights.Location = new System.Drawing.Point(165, 12);
            btnAllFlights.Name = "btnAllFlights";
            btnAllFlights.Size = new System.Drawing.Size(115, 29);
            btnAllFlights.Text = "Показати всі";
            btnAllFlights.UseVisualStyleBackColor = true;
            btnAllFlights.Click += btnAllFlights_Click;

            dataGridViewFlights.AllowUserToAddRows = false;
            dataGridViewFlights.AllowUserToDeleteRows = false;
            dataGridViewFlights.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewFlights.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewFlights.Location = new System.Drawing.Point(12, 47);
            dataGridViewFlights.MultiSelect = false;
            dataGridViewFlights.Name = "dataGridViewFlights";
            dataGridViewFlights.ReadOnly = true;
            dataGridViewFlights.RowHeadersVisible = false;
            dataGridViewFlights.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridViewFlights.Size = new System.Drawing.Size(705, 490);
            dataGridViewFlights.SelectionChanged += dataGridViewFlights_SelectionChanged;

            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(735, 18);
            label10.Text = "Дрон:";

            cmbFlightDrone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbFlightDrone.Location = new System.Drawing.Point(735, 38);
            cmbFlightDrone.Name = "cmbFlightDrone";
            cmbFlightDrone.Size = new System.Drawing.Size(260, 23);

            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(735, 70);
            label11.Text = "Дата вильоту:";

            dtpFlightDate.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpFlightDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtpFlightDate.Location = new System.Drawing.Point(735, 90);
            dtpFlightDate.Name = "dtpFlightDate";
            dtpFlightDate.Size = new System.Drawing.Size(260, 23);

            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(735, 122);
            label12.Text = "Пілот:";

            txtPilot.Location = new System.Drawing.Point(735, 142);
            txtPilot.Name = "txtPilot";
            txtPilot.Size = new System.Drawing.Size(260, 23);

            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(735, 174);
            label13.Text = "Район польоту:";

            txtLocation.Location = new System.Drawing.Point(735, 194);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new System.Drawing.Size(260, 23);

            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(735, 226);
            label14.Text = "Висота RTH (м):";

            numRth.Location = new System.Drawing.Point(735, 246);
            numRth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numRth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRth.Name = "numRth";
            numRth.Size = new System.Drawing.Size(120, 23);
            numRth.Value = new decimal(new int[] { 60, 0, 0, 0 });

            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(735, 278);
            label15.Text = "Тривалість (хв):";

            numDuration.Location = new System.Drawing.Point(735, 298);
            numDuration.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDuration.Name = "numDuration";
            numDuration.Size = new System.Drawing.Size(120, 23);
            numDuration.Value = new decimal(new int[] { 25, 0, 0, 0 });

            chkSignalLost.AutoSize = true;
            chkSignalLost.Location = new System.Drawing.Point(735, 332);
            chkSignalLost.Name = "chkSignalLost";
            chkSignalLost.Size = new System.Drawing.Size(113, 19);
            chkSignalLost.Text = "Втрата сигналу";
            chkSignalLost.UseVisualStyleBackColor = true;

            btnAddFlight.Location = new System.Drawing.Point(735, 365);
            btnAddFlight.Name = "btnAddFlight";
            btnAddFlight.Size = new System.Drawing.Size(125, 30);
            btnAddFlight.Text = "Додати";
            btnAddFlight.UseVisualStyleBackColor = true;
            btnAddFlight.Click += btnAddFlight_Click;

            btnUpdateFlight.Location = new System.Drawing.Point(870, 365);
            btnUpdateFlight.Name = "btnUpdateFlight";
            btnUpdateFlight.Size = new System.Drawing.Size(125, 30);
            btnUpdateFlight.Text = "Зберегти";
            btnUpdateFlight.UseVisualStyleBackColor = true;
            btnUpdateFlight.Click += btnUpdateFlight_Click;

            btnDeleteFlight.Location = new System.Drawing.Point(735, 403);
            btnDeleteFlight.Name = "btnDeleteFlight";
            btnDeleteFlight.Size = new System.Drawing.Size(125, 30);
            btnDeleteFlight.Text = "Видалити";
            btnDeleteFlight.UseVisualStyleBackColor = true;
            btnDeleteFlight.Click += btnDeleteFlight_Click;

            btnClearFlight.Location = new System.Drawing.Point(870, 403);
            btnClearFlight.Name = "btnClearFlight";
            btnClearFlight.Size = new System.Drawing.Size(125, 30);
            btnClearFlight.Text = "Очистити";
            btnClearFlight.UseVisualStyleBackColor = true;
            btnClearFlight.Click += btnClearFlight_Click;

            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1024, 581);
            Controls.Add(tabControl1);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Облік сервісного центру дронів";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewDrones).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPropHours).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRepairs).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFlights).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).EndInit();
            ResumeLayout(false);
        }
    }
}