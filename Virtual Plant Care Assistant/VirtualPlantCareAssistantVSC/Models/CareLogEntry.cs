using System;

namespace VirtualPlantCareAssistantVSC.Models
{
    public class CareLogEntry
    {
        public int LogId { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public int UserId { get; set; }
        public string CareType { get; set; } // "Watering" or "Fertilization"
        public DateTime CaredAt { get; set; }
        public string Notes { get; set; }

        public string CareTypeIcon => CareType == "Watering" ? "💧" : "🌿";
        public string CareTypeColor => CareType == "Watering" ? "#2196F3" : "#8BC34A";
        public string FormattedDate => CaredAt.ToString("MMM dd, yyyy  hh:mm tt");
    }

    public class ScheduleItem
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string ImagePath { get; set; }
        public string CareType { get; set; }
        public DateTime DueDate { get; set; }

        public int DaysUntilDue => (int)(DueDate.Date - DateTime.Today).TotalDays;
        public string StatusColor => DaysUntilDue < 0 ? "#F44336" : DaysUntilDue == 0 ? "#FF9800" : "#4CAF50";
        public string StatusText => DaysUntilDue < 0
            ? $"{-DaysUntilDue} day(s) overdue"
            : DaysUntilDue == 0 ? "Due today!" : $"In {DaysUntilDue} day(s)";
        public string CareIcon => CareType == "Watering" ? "💧" : "🌿";
        public string CareColor => CareType == "Watering" ? "#2196F3" : "#8BC34A";
        public string FormattedDueDate => DueDate.ToString("ddd, MMM dd");
    }

    public class ReminderItem
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string ImagePath { get; set; }
        public string CareType { get; set; }
        public int DaysOverdue { get; set; }

        public string UrgencyColor => DaysOverdue == 0 ? "#FF9800" : "#F44336";
        public string UrgencyText => DaysOverdue == 0 ? "Due Today" : $"{DaysOverdue} day(s) overdue";
        public string CareIcon => CareType == "Watering" ? "💧" : "🌿";
        public string ActionLabel => CareType == "Watering" ? "WATER NOW" : "FERTILIZE NOW";
    }
}
