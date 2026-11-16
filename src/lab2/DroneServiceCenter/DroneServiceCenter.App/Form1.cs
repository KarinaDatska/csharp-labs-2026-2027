using DroneServiceCenter.Data.Entities;
using DroneServiceCenter.Data.Services;

namespace DroneServiceCenter.App
{
    public partial class Form1 : Form
    {
        private DroneService droneService = new DroneService();
        private RepairService repairService = new RepairService();
        private FlightService flightService = new FlightService();

        private int selectedDroneId = 0;
        private int selectedRepairId = 0;
        private int selectedFlightId = 0;

        public Form1()
        {
            InitializeComponent();
            cmbStatus.SelectedIndex = 0;
            LoadDrones(false);
            LoadRepairs(false);
            LoadFlights(false);
        }

        private void UpdateComboBoxes(List<Drone> drones)
        {
            List<DroneComboItem> list1 = new List<DroneComboItem>();
            List<DroneComboItem> list2 = new List<DroneComboItem>();

            foreach (Drone d in drones)
            {
                string title = d.SerialNumber + " — " + d.Model;
                list1.Add(new DroneComboItem { Id = d.Id, Title = title });
                list2.Add(new DroneComboItem { Id = d.Id, Title = title });
            }

            cmbRepairDrone.DataSource = list1;
            cmbRepairDrone.DisplayMember = "Title";
            cmbRepairDrone.ValueMember = "Id";

            cmbFlightDrone.DataSource = list2;
            cmbFlightDrone.DisplayMember = "Title";
            cmbFlightDrone.ValueMember = "Id";
        }

        private void LoadDrones(bool onlyReady)
        {
            try
            {
                List<Drone> list = onlyReady ? droneService.GetReadyForFlight() : droneService.GetAll();
                dataGridViewDrones.DataSource = list.Select(d => new
                {
                    d.Id,
                    СерійнийНомер = d.SerialNumber,
                    Модель = d.Model,
                    Прошивка = d.FirmwareVersion,
                    Статус = d.Status,
                    НалітГод = d.PropellerHours,
                    СлідиПадінь = d.HasCrashMarks ? "Так" : "Ні"
                }).ToList();

                if (!onlyReady)
                {
                    UpdateComboBoxes(list);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void btnReadyDrones_Click(object? sender, EventArgs e)
        {
            LoadDrones(true);
        }

        private void btnAllDrones_Click(object? sender, EventArgs e)
        {
            LoadDrones(false);
        }

        private void dataGridViewDrones_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridViewDrones.CurrentRow == null) return;

            selectedDroneId = Convert.ToInt32(dataGridViewDrones.CurrentRow.Cells["Id"].Value);
            txtSerial.Text = dataGridViewDrones.CurrentRow.Cells["СерійнийНомер"].Value?.ToString() ?? "";
            txtModel.Text = dataGridViewDrones.CurrentRow.Cells["Модель"].Value?.ToString() ?? "";
            txtFirmware.Text = dataGridViewDrones.CurrentRow.Cells["Прошивка"].Value?.ToString() ?? "";
            cmbStatus.SelectedItem = dataGridViewDrones.CurrentRow.Cells["Статус"].Value?.ToString() ?? "У строю";
            numPropHours.Value = Convert.ToDecimal(dataGridViewDrones.CurrentRow.Cells["НалітГод"].Value);
            chkCrashMarks.Checked = dataGridViewDrones.CurrentRow.Cells["СлідиПадінь"].Value?.ToString() == "Так";
        }

        private void btnAddDrone_Click(object? sender, EventArgs e)
        {
            try
            {
                Drone d = new Drone();
                d.SerialNumber = txtSerial.Text.Trim();
                d.Model = txtModel.Text.Trim();
                d.FirmwareVersion = txtFirmware.Text.Trim();
                d.Status = cmbStatus.SelectedItem?.ToString() ?? "У строю";
                d.PropellerHours = (int)numPropHours.Value;
                d.HasCrashMarks = chkCrashMarks.Checked;

                droneService.Add(d);
                LoadDrones(false);
                ClearDroneFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateDrone_Click(object? sender, EventArgs e)
        {
            if (selectedDroneId == 0) return;

            try
            {
                Drone d = new Drone();
                d.Id = selectedDroneId;
                d.SerialNumber = txtSerial.Text.Trim();
                d.Model = txtModel.Text.Trim();
                d.FirmwareVersion = txtFirmware.Text.Trim();
                d.Status = cmbStatus.SelectedItem?.ToString() ?? "У строю";
                d.PropellerHours = (int)numPropHours.Value;
                d.HasCrashMarks = chkCrashMarks.Checked;

                droneService.Update(d);
                LoadDrones(false);
                LoadRepairs(false);
                LoadFlights(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteDrone_Click(object? sender, EventArgs e)
        {
            if (selectedDroneId == 0) return;

            DialogResult res = MessageBox.Show("Видалити вибраний дрон?", "Підтвердження", MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            try
            {
                droneService.Delete(selectedDroneId);
                LoadDrones(false);
                ClearDroneFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearDrone_Click(object? sender, EventArgs e)
        {
            ClearDroneFields();
        }

        private void ClearDroneFields()
        {
            selectedDroneId = 0;
            txtSerial.Clear();
            txtModel.Clear();
            txtFirmware.Clear();
            cmbStatus.SelectedIndex = 0;
            numPropHours.Value = 0;
            chkCrashMarks.Checked = false;
        }

        private void LoadRepairs(bool onlyOpen)
        {
            try
            {
                List<RepairOrder> list = onlyOpen ? repairService.GetOpenOrders() : repairService.GetAll();
                dataGridViewRepairs.DataSource = list.Select(r => new
                {
                    r.Id,
                    r.DroneId,
                    Дрон = r.Drone != null ? r.Drone.SerialNumber : "",
                    Відкрито = r.OpenedAt.ToString("dd.MM.yyyy HH:mm"),
                    Несправність = r.IssueDescription,
                    Деталі = r.ReplacedParts ?? "",
                    Обліт = r.IsTestFlightPassed ? "Так" : "Ні",
                    Закрито = r.ClosedAt.HasValue ? r.ClosedAt.Value.ToString("dd.MM.yyyy HH:mm") : "Відкритий"
                }).ToList();

                if (dataGridViewRepairs.Columns["DroneId"] != null)
                {
                    dataGridViewRepairs.Columns["DroneId"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void btnOpenRepairs_Click(object? sender, EventArgs e)
        {
            LoadRepairs(true);
        }

        private void btnAllRepairs_Click(object? sender, EventArgs e)
        {
            LoadRepairs(false);
        }

        private void chkIsClosed_CheckedChanged(object? sender, EventArgs e)
        {
            dtpRepairClosed.Enabled = chkIsClosed.Checked;
        }

        private void dataGridViewRepairs_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridViewRepairs.CurrentRow == null) return;

            selectedRepairId = Convert.ToInt32(dataGridViewRepairs.CurrentRow.Cells["Id"].Value);
            cmbRepairDrone.SelectedValue = Convert.ToInt32(dataGridViewRepairs.CurrentRow.Cells["DroneId"].Value);

            DateTime opened;
            if (DateTime.TryParse(dataGridViewRepairs.CurrentRow.Cells["Відкрито"].Value?.ToString(), out opened))
            {
                dtpRepairOpened.Value = opened;
            }

            txtRepairIssue.Text = dataGridViewRepairs.CurrentRow.Cells["Несправність"].Value?.ToString() ?? "";
            txtRepairParts.Text = dataGridViewRepairs.CurrentRow.Cells["Деталі"].Value?.ToString() ?? "";
            chkTestFlight.Checked = dataGridViewRepairs.CurrentRow.Cells["Обліт"].Value?.ToString() == "Так";

            DateTime closed;
            if (DateTime.TryParse(dataGridViewRepairs.CurrentRow.Cells["Закрито"].Value?.ToString(), out closed))
            {
                chkIsClosed.Checked = true;
                dtpRepairClosed.Value = closed;
            }
            else
            {
                chkIsClosed.Checked = false;
            }
        }

        private void btnAddRepair_Click(object? sender, EventArgs e)
        {
            if (cmbRepairDrone.SelectedValue == null) return;

            try
            {
                RepairOrder order = new RepairOrder();
                order.DroneId = (int)cmbRepairDrone.SelectedValue;
                order.OpenedAt = dtpRepairOpened.Value;
                order.IssueDescription = txtRepairIssue.Text.Trim();
                order.ReplacedParts = txtRepairParts.Text.Trim();
                order.IsTestFlightPassed = chkTestFlight.Checked;
                order.ClosedAt = chkIsClosed.Checked ? dtpRepairClosed.Value : null;

                repairService.Add(order);
                LoadRepairs(false);
                LoadDrones(false);
                ClearRepairFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateRepair_Click(object? sender, EventArgs e)
        {
            if (selectedRepairId == 0 || cmbRepairDrone.SelectedValue == null) return;

            try
            {
                RepairOrder order = new RepairOrder();
                order.Id = selectedRepairId;
                order.DroneId = (int)cmbRepairDrone.SelectedValue;
                order.OpenedAt = dtpRepairOpened.Value;
                order.IssueDescription = txtRepairIssue.Text.Trim();
                order.ReplacedParts = txtRepairParts.Text.Trim();
                order.IsTestFlightPassed = chkTestFlight.Checked;
                order.ClosedAt = chkIsClosed.Checked ? dtpRepairClosed.Value : null;

                repairService.Update(order);
                LoadRepairs(false);
                LoadDrones(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteRepair_Click(object? sender, EventArgs e)
        {
            if (selectedRepairId == 0) return;

            DialogResult res = MessageBox.Show("Видалити вибраний наряд?", "Підтвердження", MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            try
            {
                repairService.Delete(selectedRepairId);
                LoadRepairs(false);
                ClearRepairFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearRepair_Click(object? sender, EventArgs e)
        {
            ClearRepairFields();
        }

        private void ClearRepairFields()
        {
            selectedRepairId = 0;
            dtpRepairOpened.Value = DateTime.Now;
            txtRepairIssue.Clear();
            txtRepairParts.Clear();
            chkTestFlight.Checked = false;
            chkIsClosed.Checked = false;
        }

        private void LoadFlights(bool onlyIncidents)
        {
            try
            {
                List<FlightLog> list = onlyIncidents ? flightService.GetSignalLossIncidents() : flightService.GetAll();
                dataGridViewFlights.DataSource = list.Select(f => new
                {
                    f.Id,
                    f.DroneId,
                    Дрон = f.Drone != null ? f.Drone.SerialNumber : "",
                    Дата = f.FlightDate.ToString("dd.MM.yyyy HH:mm"),
                    Пілот = f.PilotName,
                    Район = f.Location,
                    RTH_м = f.RthAltitudeM,
                    Хвилин = f.DurationMin,
                    ВтратаЗвязку = f.SignalLost ? "Так" : "Ні"
                }).ToList();

                if (dataGridViewFlights.Columns["DroneId"] != null)
                {
                    dataGridViewFlights.Columns["DroneId"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void btnRiskFlights_Click(object? sender, EventArgs e)
        {
            LoadFlights(true);
        }

        private void btnAllFlights_Click(object? sender, EventArgs e)
        {
            LoadFlights(false);
        }

        private void dataGridViewFlights_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridViewFlights.CurrentRow == null) return;

            selectedFlightId = Convert.ToInt32(dataGridViewFlights.CurrentRow.Cells["Id"].Value);
            cmbFlightDrone.SelectedValue = Convert.ToInt32(dataGridViewFlights.CurrentRow.Cells["DroneId"].Value);

            DateTime dt;
            if (DateTime.TryParse(dataGridViewFlights.CurrentRow.Cells["Дата"].Value?.ToString(), out dt))
            {
                dtpFlightDate.Value = dt;
            }

            txtPilot.Text = dataGridViewFlights.CurrentRow.Cells["Пілот"].Value?.ToString() ?? "";
            txtLocation.Text = dataGridViewFlights.CurrentRow.Cells["Район"].Value?.ToString() ?? "";
            numRth.Value = Convert.ToDecimal(dataGridViewFlights.CurrentRow.Cells["RTH_м"].Value);
            numDuration.Value = Convert.ToDecimal(dataGridViewFlights.CurrentRow.Cells["Хвилин"].Value);
            chkSignalLost.Checked = dataGridViewFlights.CurrentRow.Cells["ВтратаЗвязку"].Value?.ToString() == "Так";
        }

        private void btnAddFlight_Click(object? sender, EventArgs e)
        {
            if (cmbFlightDrone.SelectedValue == null) return;

            try
            {
                FlightLog log = new FlightLog();
                log.DroneId = (int)cmbFlightDrone.SelectedValue;
                log.FlightDate = dtpFlightDate.Value;
                log.PilotName = txtPilot.Text.Trim();
                log.Location = txtLocation.Text.Trim();
                log.RthAltitudeM = (int)numRth.Value;
                log.DurationMin = (int)numDuration.Value;
                log.SignalLost = chkSignalLost.Checked;

                flightService.Add(log);
                LoadFlights(false);
                ClearFlightFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateFlight_Click(object? sender, EventArgs e)
        {
            if (selectedFlightId == 0 || cmbFlightDrone.SelectedValue == null) return;

            try
            {
                FlightLog log = new FlightLog();
                log.Id = selectedFlightId;
                log.DroneId = (int)cmbFlightDrone.SelectedValue;
                log.FlightDate = dtpFlightDate.Value;
                log.PilotName = txtPilot.Text.Trim();
                log.Location = txtLocation.Text.Trim();
                log.RthAltitudeM = (int)numRth.Value;
                log.DurationMin = (int)numDuration.Value;
                log.SignalLost = chkSignalLost.Checked;

                flightService.Update(log);
                LoadFlights(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteFlight_Click(object? sender, EventArgs e)
        {
            if (selectedFlightId == 0) return;

            DialogResult res = MessageBox.Show("Видалити запис про політ?", "Підтвердження", MessageBoxButtons.YesNo);
            if (res != DialogResult.Yes) return;

            try
            {
                flightService.Delete(selectedFlightId);
                LoadFlights(false);
                ClearFlightFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearFlight_Click(object? sender, EventArgs e)
        {
            ClearFlightFields();
        }

        private void ClearFlightFields()
        {
            selectedFlightId = 0;
            dtpFlightDate.Value = DateTime.Now;
            txtPilot.Clear();
            txtLocation.Clear();
            numRth.Value = 60;
            numDuration.Value = 25;
            chkSignalLost.Checked = false;
        }

        private class DroneComboItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
        }
    }
}