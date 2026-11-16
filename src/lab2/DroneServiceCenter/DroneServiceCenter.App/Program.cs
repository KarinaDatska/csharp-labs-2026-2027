using DroneServiceCenter.Data.Services;

namespace DroneServiceCenter.App
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                DbContextFactory.EnsureInitialized();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка ініціалізації бази даних: {ex.Message}",
                    "Критична помилка БД",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Form1());
        }
    }
}